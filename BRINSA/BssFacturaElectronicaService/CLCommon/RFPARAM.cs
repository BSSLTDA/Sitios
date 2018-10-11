using System.Collections.Generic;

namespace CLCommon
{
    public class RFPARAM
    {
        public string CCDESC { get; set; }
        public string CCSDSC { get; set; }
        public string CCCODE { get; set; }
        public string CCCODE2 { get; set; }
        public string CCCODE3 { get; set; }
        public string CCCODE4 { get; set; }
        public string CCNOT1 { get; set; }
        public string CCNOT2 { get; set; }
        public string CCNOTE { get; set; }
        public string CCCODEN { get; set; }
        public string CCCODEN2 { get; set; }
        public string CCALTC { get; set; }
    }
    public interface IRFPARAMRepository
    {
        List<RFPARAM> GetParams();
        string MontoEscrito(string num);
    }
}
