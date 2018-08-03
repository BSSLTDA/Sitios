﻿using System;

namespace PanelAsistentes.Models
{
    public class MdlContrato
    {
        public int NContrato { get; set; }
        public string RazonSocial { get; set; }
        public string NIT { get; set; }
        public Nullable<DateTime> FechaDeInicio { get; set; }
        public Nullable<DateTime> FechaDeTerminacion { get; set; }
        public Nullable<int> DuracionDias { get; set; }
        public string ProrrogaAutomatica { get; set; }
        public string Ubicacion { get; set; }
        public string ObjetoDelContrato { get; set; }
        public string ValorUsd { get; set; }
        public string ValorCop { get; set; }
        public string PeriodicidadDelPago { get; set; }
        public string AplicaImpTimbre { get; set; }
        public string Cuantia { get; set; }
        public string Observaciones { get; set; }
        public string SeguimientoMensual { get; set; }
        public string FechaDePago { get; set; }
        public string AreaSolicitante { get; set; }
        public string IdArea { get; set; }
        public string Inflacion { get; set; }
        public string ComentarioContrato { get; set; }
        public Nullable<short> STS01 { get; set; }
        public Nullable<short> STS02 { get; set; }
        public Nullable<short> STS03 { get; set; }
        public Nullable<short> STS04 { get; set; }
        public Nullable<short> STS05 { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
    }
}