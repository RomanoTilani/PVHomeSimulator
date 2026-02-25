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

        private void RunPvgisSimulationAndPlot(string[] filePaths)
        {
            // 1. Daten laden und summieren
            var solarData = LoadAndSumPvgisFiles(filePaths);
            var sortedTimes = solarData.Keys.OrderBy(t => t).ToList();

            // 2. Simulations-Variablen
            double batteryWh = (double)numericUpDownBat.Value / 2; // Start 50%
            double maxBatteryWh = (double)numericUpDownBat.Value;

            List<double> xs = new List<double>();
            List<double> ysSolarKw = new List<double>();
            List<double> ysBatteryPct = new List<double>();

            foreach (var time in sortedTimes)
            {
                double solarWatt = solarData[time];
                double loadWatt = GetHouseLoadWatt(time); // Deine Verbrauchsfunktion

                // PVGIS sind Stundenwerte -> DeltaWh = Watt * 1 Stunde
                double deltaWh = solarWatt - loadWatt;
                batteryWh += deltaWh;

                // Akku-Grenzen (C# Clamp)
                if (batteryWh > maxBatteryWh) batteryWh = maxBatteryWh;
                if (batteryWh < 0) batteryWh = 0;

                // Daten für Plot sammeln
                xs.Add(time.ToOADate());
                ysSolarKw.Add(solarWatt / 1000.0);
                ysBatteryPct.Add((batteryWh / maxBatteryWh) * 100.0);
            }

            // 3. Plotten in ScottPlot 5
            formsPlot.Plot.Clear();

            var solarPlot = formsPlot.Plot.Add.Scatter(xs.ToArray(), ysSolarKw.ToArray());
            solarPlot.LegendText = "PV Summe (kW)";
            solarPlot.Color = ScottPlot.Colors.Blue;

            var batteryPlot = formsPlot.Plot.Add.Scatter(xs.ToArray(), ysBatteryPct.ToArray());
            batteryPlot.LegendText = "Akku (%)";
            batteryPlot.Color = ScottPlot.Colors.Red;
            // Optional: Batterie auf die rechte Achse legen
            batteryPlot.Axes.YAxis = formsPlot.Plot.Axes.Right;

            formsPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.DateTimeAutomatic();
            formsPlot.Plot.Axes.AutoScale();
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
