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
    public class SolarDataPoint
    {
        public DateTime Time { get; set; }
        public double Value { get; set; } // Wir nutzen 'Value' als generischen Namen für Watt/kW
        public double Gs10Raw { get; set; } // Zum Zwischenspeichern des DWD-Rohwerts
    }

    public class SolarSurface
    {
        public string Name { get; set; }
        public double Area { get; set; }
        public double Tilt { get; set; }    // 0=Flach, 90=Zaun
        public double Azimuth { get; set; } // -90=Ost, 0=Süd, 90=West
        public double Efficiency { get; set; } = 0.21;

        public double TiltRad => Tilt * Math.PI / 180.0;
        public double AzimuthRad => Azimuth * Math.PI / 180.0;
    }

    public class SunPosition
    {
        public double Elevation { get; set; } // Höhe über Horizont (Grad)
        public double Azimuth { get; set; }   // 0 = Süd, -90 = Ost, 90 = West (Grad)

        public double ElevationRad => Elevation * Math.PI / 180.0;
        public double AzimuthRad => Azimuth * Math.PI / 180.0;

        public static double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static SunPosition Calculate(DateTime dt, double lat = 48.4, double lon = 9.5)
        {
            // Tag des Jahres
            int dayOfYear = dt.DayOfYear;
            double hour = dt.Hour + dt.Minute / 60.0 + dt.Second / 3600.0;

            // Deklination der Sonne (Näherungsformel)
            double declination = 23.45 * Math.Sin((2 * Math.PI / 365.0) * (dayOfYear - 81));
            double declRad = declination * Math.PI / 180.0;
            double latRad = lat * Math.PI / 180.0;

            // Zeitgleichung / Stundenwinkel
            // (Vereinfacht: 15 Grad pro Stunde, 12:00 ist Peak im Süden bei lon 9.5)
            double LSTM = 15 * 1; // Mitteleuropäische Zeit (GMT+1)
            double EoT = 9.87 * Math.Sin(2 * (2 * Math.PI / 360.0) * (dayOfYear - 81))
                         - 7.53 * Math.Cos((2 * Math.PI / 360.0) * (dayOfYear - 81))
                         - 1.5 * Math.Sin((2 * Math.PI / 360.0) * (dayOfYear - 81));

            double tc = 4 * (lon - LSTM) + EoT;
            double lst = hour + tc / 60.0;
            double hra = 15 * (lst - 12);
            double hraRad = hra * Math.PI / 180.0;

            // Elevation berechnen
            double sinElevation = Math.Sin(declRad) * Math.Sin(latRad) +
                                  Math.Cos(declRad) * Math.Cos(latRad) * Math.Cos(hraRad);
            double elevationRad = Math.Asin(sinElevation);

            // Azimut berechnen
            double cosAzimuth = (Math.Sin(declRad) * Math.Cos(latRad) -
                                 Math.Cos(declRad) * Math.Sin(latRad) * Math.Cos(hraRad)) / Math.Cos(elevationRad);

            // Clamp für Rundungsfehler
            cosAzimuth = Clamp(cosAzimuth, -1, 1);
            double azimuth = Math.Acos(cosAzimuth) * 180.0 / Math.PI;

            if (hra > 0) azimuth = 360 - azimuth; // Korrektur für Nachmittag
            azimuth -= 180; // Umrechnung auf 0 = Süd

            return new SunPosition { Elevation = elevationRad * 180.0 / Math.PI, Azimuth = azimuth };
        }
    }

    public class PvSystem
    {
        // Deine geplante Hardware-Konfiguration
        public static double PanelAreaM2 { get; set; } = 100.0; // z.B. 30m² Modulfläche
        public static double Efficiency { get; set; } = 0.20;  // 20% Wirkungsgrad der Module
        public static double InverterLoss { get; set; } = 0.05; // 5% Verluste im Wechselrichter

        // Die Projektion (Dachneigung/Ausrichtung)
        // Vereinfachter Faktor: Süd-Dach 35° in Süddeutschland ca. 1.1x vs. Flach
        public static double OrientationFactor { get; set; } = 1.1;

        public static double CalculateOutputPower(double dwdWattPerM2)
        {
            // P = Einstrahlung * Fläche * Wirkungsgrad * (1 - Verluste) * Ausrichtung
            double dcPower = dwdWattPerM2 * PanelAreaM2 * Efficiency * OrientationFactor;
            return dcPower * (1 - InverterLoss);
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formsPlot.Enabled = true;
        }

        private void buttonLoadData_Click(object sender, EventArgs e)
        {
            // 1. Deine Hardware-Flächen definieren
            var mySurfaces = new List<SolarSurface>
            {
                new SolarSurface { Name = "Dach Ost", Area = 35.0, Tilt = 45, Azimuth = -90 },
                new SolarSurface { Name = "Dach West", Area = 35.0, Tilt = 45, Azimuth = 90 },
                new SolarSurface { Name = "Zaun Ost", Area = 18.0, Tilt = 90, Azimuth = -90 },
                new SolarSurface { Name = "Zaun Süd", Area = 18.0, Tilt = 90, Azimuth = 0 },
                new SolarSurface { Name = "Zaun West", Area = 18.0, Tilt = 90, Azimuth = 90 }
            };

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DWD Daten (produkt*.txt)|produkt*.txt";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 2. Rohdaten einlesen (Geputzte Version ohne -999 Fehler)
                    var rawData = ParseDwdFile(openFileDialog.FileName);

                    if (rawData.Count > 0)
                    {
                        // 3. HIER die Simulation starten
                        // Sie berechnet Ertrag, Last und Batterie für jeden Datenpunkt
                        RunFullSimulation(rawData, mySurfaces);
                    }
                    else
                    {
                        MessageBox.Show("Keine validen Daten in der Datei gefunden.");
                    }
                }
            }
        }

        public double GetHouseLoadWatt(DateTime time)
        {
            // Grundlast: ca. 300W - 400W (immer an)
            double load = 350;

            // Morgens: Kaffee, Bad (07:00 - 08:30)
            if (time.Hour >= 7 && time.Hour <= 8) load += 1500;

            // Mittags: Kochen (12:00 - 13:30)
            if (time.Hour >= 12 && (time.Hour < 13 || time.Minute < 30)) load += 2500;

            // Abends: Licht, TV, Kochen (18:00 - 21:00)
            if (time.Hour >= 18 && time.Hour <= 21) load += 1200;

            // Zufällige Rausch-Komponente (+/- 50W)
            Random rnd = new Random(time.Minute);
            load += rnd.Next(-50, 50);

            return load;
        }

        private void PlotData(List<SolarDataPoint> data)
        {
            formsPlot.Plot.Clear();

            // ScottPlot 5 benötigt Double-Arrays
            double[] xs = data.Select(p => p.Time.ToOADate()).ToArray();
            double[] ys = data.Select(p => p.Value).ToArray();

            // 'Add.Scatter' ist korrekt, aber 'Label' ist jetzt 'LegendText'
            var scatter = formsPlot.Plot.Add.Scatter(xs, ys);
            scatter.LegendText = "Einstrahlung [W/m²]";

            // Zeitachse einstellen: In SP5 nutzt man jetzt 'DateTime' Helper direkt an der Achse
            formsPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.DateTimeAutomatic();

            // Achsen-Beschriftungen (modernes Interface)
            formsPlot.Plot.XLabel("Zeit");
            formsPlot.Plot.YLabel("Leistung [W/m²]");
            formsPlot.Plot.Title("Solar-Verlauf Station 4928");

            // Legende anzeigen (optional)
            formsPlot.Plot.ShowLegend();

            // Zoom auf Daten anpassen
            formsPlot.Plot.Axes.AutoScale();

            formsPlot.Refresh();
        }

        private void RunFullSimulation(List<SolarDataPoint> rawData, List<SolarSurface> mySurfaces)
        {
            double batterySocWh = 15000; // Start bei 15 kWh (50%)
            const double maxCapWh = 30000; // 30 kWh Akku
            const double efficiency = 0.95; // 95% Lade-Effizienz

            var plotPointsSolar = new List<SolarDataPoint>();
            var plotPointsBattery = new List<SolarDataPoint>();

            foreach (var point in rawData)
            {
                // 1. Solarertrag berechnen
                double solarWatt = CalculateTotalSystemWatt(point.Gs10Raw, point.Time, mySurfaces);

                // 2. Verbrauch berechnen
                double loadWatt = GetHouseLoadWatt(point.Time);

                // 3. Bilanz (Netto-Leistung)
                double deltaWatt = solarWatt - loadWatt;

                // 4. Akku updaten (10 Minuten = 1/6 Stunde)
                double hours = 10.0 / 60.0;
                if (deltaWatt > 0)
                    batterySocWh += deltaWatt * hours * efficiency; // Laden
                else
                    batterySocWh += deltaWatt * hours; // Entladen

                // Grenzen einhalten (Clamping)
                if (batterySocWh > maxCapWh) batterySocWh = maxCapWh;
                if (batterySocWh < 0) batterySocWh = 0;

                // Daten für Plot speichern
                plotPointsSolar.Add(new SolarDataPoint { Time = point.Time, Value = solarWatt / 1000.0 }); // in kW
                plotPointsBattery.Add(new SolarDataPoint { Time = point.Time, Value = (batterySocWh / maxCapWh) * 100.0 }); // in %
            }

            PlotDoubleData(plotPointsSolar, plotPointsBattery);
        }

        private void PlotDoubleData(List<SolarDataPoint> solar, List<SolarDataPoint> battery)
        {
            formsPlot.Plot.Clear();

            // 1. Zeit-Achse (X) vorbereiten
            double[] xs = solar.Select(p => p.Time.ToOADate()).ToArray();

            // 2. Solar-Daten (kW) hinzufügen
            double[] ysSolar = solar.Select(p => p.Value).ToArray();
            var sPlot = formsPlot.Plot.Add.Scatter(xs, ysSolar);
            sPlot.LegendText = "PV Ertrag (kW)";
            sPlot.Color = new ScottPlot.Color(0, 0, 255); // Blau

            // 3. Akku-Daten (%) hinzufügen
            double[] ysBattery = battery.Select(p => p.Value).ToArray();
            var bPlot = formsPlot.Plot.Add.Scatter(xs, ysBattery);
            bPlot.LegendText = "Akku (%)";
            bPlot.Color = new ScottPlot.Color(255, 0, 0); // Rot

            // --- NEUE ACHSEN-LOGIK FÜR SCOTTPLOT 5 ---
            // Ersetzt DateTimeTicks durch den expliziten Generator
            formsPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.DateTimeAutomatic();

            // Zoom auf die Daten (wichtig, falls der Graph "leer" aussieht)
            formsPlot.Plot.Axes.AutoScale();

            formsPlot.Plot.ShowLegend();
            formsPlot.Refresh();
        }

        private List<SolarDataPoint> ParseDwdFile(string filePath)
        {
            var points = new List<SolarDataPoint>();
            var lines = File.ReadAllLines(filePath); // Zum Debuggen erst mal alles laden

            // Wir fangen bei Zeile 1 an (Index 0 ist Header)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("eor")) continue;

                var parts = line.Split(';');

                // Sicherstellen, dass wir genug Spalten haben (mindestens bis GS_10 an Index 4)
                if (parts.Length < 5) continue;

                // Datum extrahieren (Spalte 1) - Leerzeichen entfernen!
                string datePart = parts[1].Trim();

                // Strahlung extrahieren (Spalte 4: GS_10) - Leerzeichen entfernen!
                string gs10Part = parts[4].Trim();

                if (DateTime.TryParseExact(datePart, "yyyyMMddHHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                {
                    if (double.TryParse(gs10Part, NumberStyles.Any, CultureInfo.InvariantCulture, out double gs10))
                    {
                        points.Add(new SolarDataPoint { Time = dt, Gs10Raw = gs10 });
                    }
                }
            }

            // Kleiner Check für dich:
            if (points.Count == 0) MessageBox.Show("Keine Datenpunkte geparst! Prüfe das Datumsformat.");
            else if (points.Max(p => p.Value) == 0) MessageBox.Show("Daten geladen, aber alle Werte sind 0.0. Prüfe Spalte GS_10!");

            return points;
        }

        public double CalculateTotalSystemWatt(double dwdGs10Raw, DateTime time, List<SolarSurface> surfaces)
        {
            var sun = SunPosition.Calculate(time);
            if (sun.Elevation <= 0 || dwdGs10Raw <= 0) return 0;

            // 1. Horizontalstrahlung in W/m²
            double wattHorizontal = (dwdGs10Raw * 10000.0) / 600.0;

            // 2. WICHTIG: Hochrechnen auf Strahlung SENKRECHT zur Sonne (Extraterrestrisch/Normal)
            // Da wattHorizontal = wattNormal * sin(Elevation)
            double wattNormal = wattHorizontal / Math.Sin(sun.ElevationRad);

            // Sicherheitsdeckel: wattNormal kann bei Sonnenaufgang (Division durch fast 0) 
            // theoretisch unendlich werden. Wir deckeln es auf 1100 W/m² (Max. Sonne).
            wattNormal = Math.Min(wattNormal, 1100.0);

            double totalSystemWatt = 0;

            foreach (var surface in surfaces)
            {
                // 3. Projektion der NORMAL-Strahlung auf deine geneigte Fläche
                double cosTheta = Math.Sin(sun.ElevationRad) * Math.Cos(surface.TiltRad) +
                                 Math.Cos(sun.ElevationRad) * Math.Sin(surface.TiltRad) *
                                 Math.Cos(sun.AzimuthRad - surface.AzimuthRad);

                double projectionFactor = Math.Max(0, cosTheta);

                // Ertrag basierend auf wattNormal (nicht Horizontal!)
                totalSystemWatt += wattNormal * projectionFactor * surface.Area * surface.Efficiency;
            }

            return totalSystemWatt * 0.85;
        }
    }
}
