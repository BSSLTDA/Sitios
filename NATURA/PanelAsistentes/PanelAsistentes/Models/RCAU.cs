﻿using System;
using System.ComponentModel.DataAnnotations;

namespace PanelAsistentes.Models
{
    public class RCAU2
    {
        public string USTS { get; set; }
        public string UTIPO { get; set; }
        [Display(Name = "Usuario")]
        public string UUSR { get; set; }
        public string UMOD { get; set; }
        [Display(Name = "Carga")]
        public string UGRP01 { get; set; }
        public string UGRP02 { get; set; }
        public string UGRP03 { get; set; }
        public Nullable<decimal> UGRP { get; set; }
        public Nullable<decimal> UTRA { get; set; }
        public Nullable<decimal> UVEND { get; set; }
        public string UPASS { get; set; }
        [Display(Name = "Nombre")]
        public string UNOM { get; set; }
        [Display(Name = "Correo")]
        public string UEMAIL { get; set; }
        public Nullable<DateTime> UFECCAD { get; set; }
        public Nullable<decimal> UDIAS { get; set; }
        public string UCATEG1 { get; set; }
        public string UCATEG2 { get; set; }
        public string UCATEG3 { get; set; }
        public string UCATEG4 { get; set; }
        public string UCATEG5 { get; set; }
        public string UVIAPAG { get; set; }
        public string UANILL { get; set; }
        public string USFCNS { get; set; }
        public string UBODS { get; set; }
        public string UQRY { get; set; }
        public Nullable<decimal> UTU { get; set; }
        public string UEMAILBAK { get; set; }
        public string UPERFIL { get; set; }
        public string USTYLE { get; set; }
        public string USEPDEC { get; set; }
        public string USEPLST { get; set; }
        public Nullable<DateTime> UUPDDAT { get; set; }
        public Nullable<decimal> UINGR { get; set; }
        public Nullable<decimal> UPRF400 { get; set; }
        public string UNUMDOC { get; set; }
        public string UDIGVER { get; set; }
        public string UUDFA01 { get; set; }
        public string UUDFA02 { get; set; }
        public string UUDFA03 { get; set; }
        public Nullable<decimal> UUDFN01 { get; set; }
        public Nullable<decimal> UUDFN02 { get; set; }
        public Nullable<decimal> UUDFN03 { get; set; }
        public Nullable<decimal> UFLAG01 { get; set; }
        public Nullable<decimal> UFLAG02 { get; set; }
        public Nullable<decimal> UFLAG03 { get; set; }
        public string UUPLFILE1 { get; set; }
        public string UUPLFILE2 { get; set; }
        public string UNUMTC { get; set; }
        public string UBANCO { get; set; }
        public string UTOKEN { get; set; }
        public string UUSRABR { get; set; }
        public string UPLANT { get; set; }
        public string UUEN { get; set; }
        public string UWHS { get; set; }
        public Nullable<decimal> UACC00 { get; set; }
        public Nullable<decimal> UACC01 { get; set; }
        public Nullable<decimal> UACC02 { get; set; }
        public Nullable<decimal> UACC03 { get; set; }
        public Nullable<decimal> UACC04 { get; set; }
        public Nullable<decimal> UACC05 { get; set; }
        public Nullable<decimal> UACC06 { get; set; }
        public Nullable<decimal> UACC07 { get; set; }
        public Nullable<decimal> UACC08 { get; set; }
        public Nullable<decimal> UACC09 { get; set; }
        public Nullable<decimal> UACC10 { get; set; }
        public string Tipo { get; set; }
    }
}