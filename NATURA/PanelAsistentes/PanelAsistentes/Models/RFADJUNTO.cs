using System;

namespace PanelAsistentes.Models
{
    public class RFADJUNTO2
    {
        public string FID { get; set; }
        public long FANUMREG { get; set; }
        public long FAPROCNUM { get; set; }
        public long FAPROCLIN { get; set; }
        public string FACATEG01 { get; set; }
        public string FACATEG02 { get; set; }
        public string FACATEG03 { get; set; }
        public string FADESCRIP { get; set; }
        public string FACRTUSR { get; set; }
        public DateTime FACRTDAT { get; set; }
        public string FAFILE { get; set; }
        public string FATITURL { get; set; }
        public string FAURL { get; set; }
        public Nullable<decimal> STS01 { get; set; }
        public Nullable<decimal> STS02 { get; set; }
        public Nullable<decimal> STS03 { get; set; }
        public Nullable<decimal> STS04 { get; set; }
        public Nullable<decimal> STS05 { get; set; }
        public Nullable<long> FREFNUM { get; set; }
    }
}