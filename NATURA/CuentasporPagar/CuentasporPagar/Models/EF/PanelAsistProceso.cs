//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CuentasporPagar.Models.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class PanelAsistProceso
    {
        public int IdPanelAsistProceso { get; set; }
        public System.DateTime FechaCrea { get; set; }
        public Nullable<System.DateTime> FechaCierra { get; set; }
        public string Creador { get; set; }
        public string Solicitante { get; set; }
        public short STS01 { get; set; }
        public short STS02 { get; set; }
        public short STS03 { get; set; }
        public short STS04 { get; set; }
        public short STS05 { get; set; }
        public string Asistente { get; set; }
        public string CodCentroCosto { get; set; }
        public string CodCuentaContable { get; set; }
        public string NotaSolicitante { get; set; }
        public string NotaAprobador { get; set; }
        public int IdPanelAsistArea { get; set; }
        public string DescripcionGastos { get; set; }
        public string NombreProveedor { get; set; }
        public string NitProveedor { get; set; }
        public string CorreoProveedor { get; set; }
        public string AsistenteCierra { get; set; }
        public string NumPedidoSAP { get; set; }
        public Nullable<System.DateTime> FechaMaximaRadicacion { get; set; }
    }
}
