using ScottPlot;
using ScottPlot.Interactivity.UserActionResponses;
using ScottPlot.Palettes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SolarHouseSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            numericUpDownPower_ValueChanged(null, null);
        }

        private void buttonLoadData_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "CSV Dateien (*.csv)|*.csv";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    RunPvgisSimulationAndPlot(openFileDialog.FileNames);
                }
            }
        }

        private Dictionary<DateTime, (double Power, double Temp, int Count)> LoadAndSumPvgisFiles(string[] filePaths)
        {
            var combined = new Dictionary<DateTime, (double Power, double Temp, int Count)>();

            foreach (var path in filePaths)
            {
                var lines = File.ReadLines(path);
                bool dataStarted = false;

                foreach (var line in lines)
                {
                    if (line.StartsWith("time,P,")) { dataStarted = true; continue; }
                    if (!dataStarted || string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    if (parts.Length < 2) continue;

                    // PVGIS Format: 20050101:0010
                    if (DateTime.TryParseExact(parts[0], "yyyyMMdd:HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                    {
                        double power = double.Parse(parts[1], CultureInfo.InvariantCulture); // Spalte 'P'
                        double temp = double.Parse(parts[4], CultureInfo.InvariantCulture);  // Spalte 'T2m'

                        // Speichere beides (z.B. in einem Dictionary mit einem Struct oder Tuple)
                        if (!combined.ContainsKey(dt))
                            combined[dt] = (Power: 0.0, Temp: 0.0, Count: 0);

                        var current = combined[dt];
                        combined[dt] = (current.Power + power, current.Temp + temp, current.Count + 1);
                    }
                }
            }
            return combined;
        }

        public bool IsSommer(DateTime time)
        {
            // April (4) bis August (8) inklusiv
            return time.Month >= 4 && time.Month <= 8;
        }

        public double GetHeatPumpElectricalLoad(double tempOutside, double tempTarget = 21.0)
        {
            if (tempOutside >= 16.0) return 0; // Heizgrenze: Über 16°C bleibt die WP aus

            // 1. Thermischer Bedarf des Hauses (Watt_thermisch)
            // H_T Wert für Model Fjord ca. 120 W/K
            double thermalNeedWatt = (tempTarget - tempOutside) * 120.0;

            // 2. COP Berechnung (Coefficient of Performance)
            // Die LG Therma V HM071MRS hat bei A7/W35 einen COP von ca. 4.5
            // Bei -7°C sinkt er auf ca. 2.5. Wir nähern das linear an:
            double cop = 3.0 + (tempOutside * 0.1);
            cop = Clamp(cop, 1.8, 5.0); // Sicherheitsgrenzen für den COP

            // 3. Elektrische Last = Thermischer Bedarf / COP
            double electricalLoad = thermalNeedWatt / cop;

            // Maximale elektrische Aufnahme der 7kW Klasse liegt bei ca. 2.5 - 3 kW
            return Math.Min(electricalLoad, 3000.0);
        }

        private void RunPvgisSimulationAndPlot(string[] filePaths)
        {
            // 1. Daten laden und summieren
            var solarData = LoadAndSumPvgisFiles(filePaths);
            var sortedTimes = solarData.Keys.OrderBy(t => t).ToList();

            // 2. Listen für den Plot (Hier werden sie definiert!)
            List<double> xs = new List<double>();
            List<double> ysSolarKw = new List<double>();
            List<double> ysBatteryPct = new List<double>();
            List<double> ysTemp = new List<double>();

            // Simulations-Variablen
            double batteryWh = (double)numericUpDownBat.Value / 2 * 1000; // Start 50%
            double maxBatteryWh = (double)numericUpDownBat.Value * 1000;
            double StromVerbrauch = 0;

            List<double> ysOfenKw = new List<double>();

            foreach (var time in sortedTimes)
            {
                var data = solarData[time];

                // FIX: solarWatt aus dem Tuple extrahieren
                double solarWatt = data.Power;

                double avgTemp = data.Temp / data.Count;
                double baseLoad = GetHouseLoadWatt(time);
                double wpLoad = GetHeatPumpElectricalLoad(avgTemp);

                bool istOfenZeit = time.Hour >= 8 && time.Hour < 20;
                bool strom = batteryWh < (maxBatteryWh * 0.2); //kleiner 20%

                // 1. Differenz berechnen (PV - (Haus + Wärmepumpe))
                double deltaWh = solarWatt - (baseLoad + wpLoad);

                if(strom)
                {
                    if(deltaWh > 0)
                    {
                        batteryWh = Clamp(batteryWh + deltaWh, 0, maxBatteryWh);
                    }
                    else
                    {
                        StromVerbrauch += (baseLoad + wpLoad);
                    }
                }
                else
                {
                    batteryWh = Clamp(batteryWh + deltaWh, 0, maxBatteryWh);
                }

                // 3. Listen befüllen
                xs.Add(time.ToOADate());
                ysSolarKw.Add(solarWatt);
                ysOfenKw.Add(wpLoad + baseLoad / 1000.0);
                ysBatteryPct.Add((batteryWh / maxBatteryWh) * 100.0);
                ysTemp.Add(avgTemp);
            }

            textBoxNetz.Text = (StromVerbrauch / 1000).ToString("0.00");

            PlotEverything(xs, ysSolarKw, ysBatteryPct, ysTemp, ysOfenKw);
        }

        private void PlotEverything(List<double> xs, List<double> ysSolar, List<double> ysBattery, List<double> ysTemp, List<double> ysOfen)
        {
            formsPlot.Plot.Clear();
            double[] xsArray = xs.ToArray();

            // --- LINKE ACHSE (kW) ---
            // 1. PV Echtzeit (Hellblau)
            var sPlot = formsPlot.Plot.Add.Scatter(xsArray, ysSolar.ToArray());
            sPlot.LegendText = "PV Echtzeit (kW)";
            sPlot.Color = ScottPlot.Colors.Blue.WithAlpha(0.2);
            sPlot.Axes.YAxis = formsPlot.Plot.Axes.Left;

            //// 2. Tagesmittel (Gold, dick)
            //var avgPlot = formsPlot.Plot.Add.Scatter(xsArray, ysSolarAvg);
            //avgPlot.LegendText = "Tagesmittel (kW)";
            //avgPlot.Color = ScottPlot.Colors.Gold;
            //avgPlot.LineWidth = 3;
            //avgPlot.Axes.YAxis = formsPlot.Plot.Axes.Left;

            // --- RECHTE ACHSE (% / °C) ---
            // 3. Temperatur (Grau, dünn im Hintergrund)
            var tPlot = formsPlot.Plot.Add.Scatter(xsArray, ysTemp.ToArray());
            tPlot.LegendText = "Temp (°C)";
            tPlot.Color = ScottPlot.Colors.Gray.WithAlpha(0.5);
            tPlot.LineWidth = 1;
            tPlot.Axes.YAxis = formsPlot.Plot.Axes.Right;

            // 4. Akku Stand (Rot, kräftig)
            var bPlot = formsPlot.Plot.Add.Scatter(xsArray, ysBattery.ToArray());
            bPlot.LegendText = "Akku (%)";
            bPlot.Color = ScottPlot.Colors.Red;
            bPlot.LineWidth = 2;
            bPlot.Axes.YAxis = formsPlot.Plot.Axes.Right;

            // 3. Pelletofen Leistung (Links, kW thermisch)
            // Wir nutzen hier ein "FillY" oder eine Area-Chart, damit es sich abhebt
            var ofenPlot = formsPlot.Plot.Add.Scatter(xsArray, ysOfen.ToArray());
            ofenPlot.LegendText = "Wärmepumpe (kW)";
            ofenPlot.Color = ScottPlot.Colors.Orange.WithAlpha(0.4);
            ofenPlot.LineWidth = 1;
            // Wir füllen die Fläche unter der Kurve aus:
            ofenPlot.FillY = true;
            ofenPlot.Axes.YAxis = formsPlot.Plot.Axes.Left;

            // --- ACHSEN-SETUP ---
            formsPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.DateTimeAutomatic();
            formsPlot.Plot.Axes.AutoScale();

            // Rechte Achse fixieren (frostsicher bis -20°C, oben bis 100%)
            formsPlot.Plot.Axes.Right.Range.Min = -20;
            formsPlot.Plot.Axes.Right.Range.Max = 105;
            formsPlot.Plot.Axes.Right.IsVisible = true;

            // Labels
            formsPlot.Plot.Axes.Left.Label.Text = "Leistung [kW]";
            formsPlot.Plot.Axes.Right.Label.Text = "Akku [%] / Temp [°C]";

            formsPlot.Plot.ShowLegend();
            formsPlot.Refresh();
        }

        public double GetHouseLoadWatt(DateTime time)
        {
            bool isSommer = IsSommer(time);
            // Grundlast: 
            double load = (double)numericUpDownPower.Value;

            if (isSommer)
            {
            }

            return load;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private void numericUpDownPower_ValueChanged(object sender, EventArgs e)
        {
            textBoxPowerDay.Text = (numericUpDownPower.Value * 24).ToString();
        }
    }
}
