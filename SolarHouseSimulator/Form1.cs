using ScottPlot;
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

        private Dictionary<DateTime, double> LoadAndSumPvgisFiles(string[] filePaths)
        {
            var combined = new Dictionary<DateTime, double>();

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

                        if (combined.ContainsKey(dt)) combined[dt] += power;
                        else combined[dt] = power;
                    }
                }
            }
            return combined;
        }

        private double[] CalculateMovingAverage(double[] data, int windowSize)
        {
            double[] result = new double[data.Length];
            double sum = 0;

            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
                if (i >= windowSize)
                {
                    sum -= data[i - windowSize];
                    result[i] = sum / windowSize;
                }
                else
                {
                    // Während der Aufwärmphase des Fensters
                    result[i] = sum / (i + 1);
                }
            }
            return result;
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

            // Simulations-Variablen
            double batteryWh = 15000; // Start 50%
            double maxBatteryWh = 30000;

            foreach (var time in sortedTimes)
            {
                double solarWatt = solarData[time];
                double loadWatt = GetHouseLoadWatt(time); // Deine Funktion

                // Simulation (Stunden-Intervall)
                double deltaWh = solarWatt - loadWatt;
                batteryWh += deltaWh;

                if (batteryWh > maxBatteryWh) batteryWh = maxBatteryWh;
                if (batteryWh < 0) batteryWh = 0;

                // Daten zu Listen hinzufügen
                xs.Add(time.ToOADate());
                ysSolarKw.Add(solarWatt / 1000.0);
                ysBatteryPct.Add((batteryWh / maxBatteryWh) * 100.0);
            }

            // 3. Gleitender Mittelwert berechnen (Fenster: 24 Stunden)
            double[] solarMovingAvg = CalculateMovingAverage(ysSolarKw.ToArray(), 24);

            // 1. Alles Vorherige löschen
            formsPlot.Plot.Clear();

            // 2. Zeit-Achse (X) vorbereiten
            double[] xsArray = xs.ToArray();

            // 3. Solar-Kurve (Linke Achse)
            var solarPlot = formsPlot.Plot.Add.Scatter(xsArray, ysSolarKw.ToArray());
            solarPlot.LegendText = "PV Leistung (kW)";
            solarPlot.Color = ScottPlot.Colors.Blue.WithAlpha(0.3);
            solarPlot.Axes.YAxis = formsPlot.Plot.Axes.Left; // Explizit Links

            // 4. Mittelwert-Kurve (Linke Achse)
            var avgPlot = formsPlot.Plot.Add.Scatter(xsArray, solarMovingAvg);
            avgPlot.LegendText = "Tagesmittel (kW)";
            avgPlot.Color = ScottPlot.Colors.Gold;
            avgPlot.LineWidth = 2;
            avgPlot.Axes.YAxis = formsPlot.Plot.Axes.Left; // Explizit Links

            // 5. Akku-Kurve (Rechte Achse)
            var batteryPlot = formsPlot.Plot.Add.Scatter(xsArray, ysBatteryPct.ToArray());
            batteryPlot.LegendText = "Akku (%)";
            batteryPlot.Color = ScottPlot.Colors.Red;
            batteryPlot.Axes.YAxis = formsPlot.Plot.Axes.Right; // Explizit Rechts

            // 6. ACHSEN-LOGIK (Stabilisiert)
            // X-Achse auf Zeit umstellen
            formsPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.DateTimeAutomatic();

            // WICHTIG: Erst AutoScale für alle Achsen, dann spezifische Limits
            formsPlot.Plot.Axes.AutoScale();

            // Jetzt nur die RECHTE Achse auf 0-100 pinnen (für die Prozentanzeige)
            formsPlot.Plot.Axes.Right.Range.Min = 0;
            formsPlot.Plot.Axes.Right.Range.Max = 105; // Etwas Puffer nach oben

            // Beschriftungen aktivieren
            formsPlot.Plot.Axes.Left.Label.Text = "PV Leistung [kW]";
            formsPlot.Plot.Axes.Right.Label.Text = "Akku [%]";
            // In ScottPlot 5 muss die rechte Achse oft explizit sichtbar geschaltet werden:
            formsPlot.Plot.Axes.Right.IsVisible = true;

            formsPlot.Plot.ShowLegend();
            formsPlot.Refresh();
        }

        public double GetHouseLoadWatt(DateTime time)
        {
            // Grundlast: 
            double load = (double)numericUpDownPower.Value;
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
