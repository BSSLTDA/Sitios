using System.Collections.Generic;

namespace CLCommon
{
    public class RFLOGFE
    {
        public string DOCTIPO { get; set; }
        public string DOCPREFI { get; set; }
        public string DOCNUMERO { get; set; }
        public string FECAPI { get; set; }
        public string MINUTOS { get; set; }
        public string BUSY { get; set; }        
    }
    public interface IRFLOGFERepository
    {
        List<RFLOGFE> SinPDF();
        List<RFLOGFE> SinPDFN();
        string UpdBusy(string Prefijo, string Numero);
        List<RFLOGFE> GetLog(string Prefijo, string Numero);
    }
}
