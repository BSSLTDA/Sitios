namespace CLCommon
{
    public class RFPARAM
    {
        public string CCNOT1 { get; set; }
    }
    public interface IRFPARAMRepository
    {
        RFPARAM GetAPI(string Code);
    }
}
