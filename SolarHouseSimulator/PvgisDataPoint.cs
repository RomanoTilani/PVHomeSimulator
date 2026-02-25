using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHouseSimulator
{
    public class PvgisDataPoint
    {
        public DateTime Time { get; set; }
        public double PowerWatt { get; set; } // Spalte 'P'
        public double Temperature { get; set; } // Spalte 'T2m' (optional für Akku-Chemie)
    }
}
