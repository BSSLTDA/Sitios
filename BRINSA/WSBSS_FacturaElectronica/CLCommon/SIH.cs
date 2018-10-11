using System.Collections.Generic;

namespace CLCommon
{
    public class SIH
    {
        public string SICUST { get; set; }
        public string PDF { get; set; }
        public string SIINVD { get; set; }
        public string IHDPFX { get; set; }
        public string IHDOCN { get; set; }
        public string SINAM { get; set; }
        public string SIWHSE { get; set; }
        public string FACTURA { get; set; }
        public string CCUDV1 { get; set; }
    }

    public interface ISIHRepository
    {
        List<SIH> GetFacturas();
        List<SIH> GetNotas();
    }
}
