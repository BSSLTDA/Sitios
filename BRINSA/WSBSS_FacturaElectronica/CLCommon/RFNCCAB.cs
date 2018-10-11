namespace CLCommon
{
    public class RFNCCAB
    {
        public string FAREF02 { get; set; }
        public string FPREFIJ { get; set; }
        public string FNOTA { get; set; }
    }
    public interface IRFNCCABRepository
    {
        string UpdCab(string Prefijo, string Nota);
        string UpdExistePDF(string Prefijo, string Nota);
    }
}
