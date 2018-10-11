using CLCommon;
using Dapper;
using IBM.Data.DB2.iSeries;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CLDB2
{
    public class RFPARAMRepository : IRFPARAMRepository
    {
        private IDbConnection db = new iDB2Connection(ConfigurationManager.ConnectionStrings["ConexionDB2"].ToString());
        public RFPARAM GetAPI(string Code)
        {
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" SELECT");
            sbF.Add("        CCNOT1");
            sbF.Add(" FROM");
            sbF.Add("        RFPARAM");
            sbF.Add(" WHERE");
            sbF.Add("        CCTABL     = 'FEPARAMG'");
            sbF.Add(string.Format("        AND CCCODE = '{0}'", Code));
            return db.Query<RFPARAM>(sbF.GetSQL()).SingleOrDefault();
        }
    }
}
