using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuentasporPagar.Models
{
    public class MdlImpuestos
    {
        public int IdCxPImpuesto { get; set; }
        public int? IdCxPConcepto { get; set; }
        public int? IdCxPRegimen { get; set; }
        public int? IdCxPTasa { get; set; }
        public string Concepto { get; set; }
        public string Regimen { get; set; }
        public string Tasa { get; set; }
    }
}