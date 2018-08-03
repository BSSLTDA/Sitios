using System;
using System.ComponentModel.DataAnnotations;

namespace PanelAsistentes.Models
{
    public class PanelAsistProceso2
    {
        [Display(Name = "N° Sol.")]
        public int IdPanelAsistProceso { get; set; }
        [Display(Name = "Creada")]
        public DateTime FechaCrea { get; set; }
        [Display(Name = "Procesada")]
        public Nullable<System.DateTime> FechaCierra { get; set; }
        public string Creador { get; set; }
        public string Solicitante { get; set; }
        public short STS01 { get; set; }
        public short STS02 { get; set; }
        public short STS03 { get; set; }
        public short STS04 { get; set; }
        public short STS05 { get; set; }
        public string Asistente { get; set; }
        [Display(Name = "Centro de Costos")]
        public string CodCentroCosto { get; set; }
        [Display(Name = "Cuenta Contable")]
        public string CodCuentaContable { get; set; }
        [Display(Name = "Nota")]
        public string NotaSolicitante { get; set; }
        public string NotaAprobador { get; set; }
        public int IdPanelAsistArea { get; set; }
        [Display(Name = "Descripción Gastos")]
        public string DescripcionGastos { get; set; }
        [Display(Name = "Nombre Proveedor")]
        public string NombreProveedor { get; set; }
        [Display(Name = "N° Cotización/Propuesta")]
        public string NumeroCotizacion { get; set; }
        [Display(Name = "Nit Proveedor")]
        public string NitProveedor { get; set; }
        [Display(Name = "Correo Proveedor")]
        public string CorreoProveedor { get; set; }
        public string AsistenteCierra { get; set; }
        [Display(Name = "N° Ped.SAP")]
        public string NumPedidoSAP { get; set; }
        [Display(Name = "Fec. Max Radicación")]
        public Nullable<System.DateTime> FechaMaximaRadicacion { get; set; }
        public string Area { get; set; }        
        public string Adjunto { get; set; }
        public string AdjuntoNme { get; set; }
        public string Estado { get; set; }
    }
}