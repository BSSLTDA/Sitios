using System;

namespace PanelAsistentes.Models
{
    public class RMAILX2
    {
        public long LID { get; set; }
        public string LMOD { get; set; }
        public string LREF { get; set; }
        public string LASUNTO { get; set; }
        public string LENVIA { get; set; }
        public string LPARA { get; set; }
        public string LCOPIA { get; set; }
        public string LBCC { get; set; }
        public string LCUERPO { get; set; }
        public string LADJUNTOS { get; set; }
        public string LURL { get; set; }
        public string LTITURL { get; set; }
        public decimal LENVIADO { get; set; }
        public decimal LSTS01 { get; set; }
        public decimal LSTS02 { get; set; }
        public decimal LSTS03 { get; set; }
        public decimal LSTS04 { get; set; }
        public decimal LSTS05 { get; set; }
        public System.DateTime LCRTDAT { get; set; }
        public Nullable<System.DateTime> LUPDDAT { get; set; }
        public string LMSGERR { get; set; }
        public string LKEY { get; set; }
        public string LCATALOGO { get; set; }
        public string LSMTPHOST { get; set; }
        public string LSMTPUSERNAME { get; set; }
        public string LSMTPPASSWORD { get; set; }
        public Nullable<System.DateTime> LFPROXIMA { get; set; }
        public string LSQL { get; set; }
    }
}