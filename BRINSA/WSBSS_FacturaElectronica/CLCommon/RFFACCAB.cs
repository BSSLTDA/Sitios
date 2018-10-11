namespace CLCommon
{
    public class RFFACCAB
    {
        public string FAREF02 { get; set; }
        public string FPREFIJ { get; set; }
        public string FFACTUR { get; set; }
    }

    public interface IRFFACCABRepository
    {
        string UpdCab(string Prefijo, string Factura);
        string UpdExistePDF(string Prefijo, string Factura);
    }
}
