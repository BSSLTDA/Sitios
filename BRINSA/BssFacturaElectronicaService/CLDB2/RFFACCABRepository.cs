using CLCommon;
using Dapper;
using IBM.Data.DB2.iSeries;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace CLDB2
{
    public class RFFACCABRepository : IRFFACCABRepository
    {
        private IDbConnection db = new iDB2Connection(ConfigurationManager.ConnectionStrings["ConexionDB2"].ToString());

        public RFFACCAB Find(string Prefijo, string Factura)
        {
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT TRIM(H.FPREFIJ) FPREFIJ, H.FFACTUR, TRIM(H.FORDCOM) FORDCOM, TRIM(H.FMONEDA) FMONEDA,");
            query.Append(" YEAR(H.FFECFAC) || '-' || DIGITS(DECIMAL(MONTH(H.FFECFAC), 2, 0)) || '-' || DIGITS(DECIMAL(DAY(H.FFECFAC), 2, 0)) AS FFECFAC, ");
            query.Append(" YEAR(H.FFECVEN) || '-' || DIGITS(DECIMAL(MONTH(H.FFECVEN), 2, 0)) || '-' || DIGITS(DECIMAL(DAY(H.FFECVEN), 2, 0)) AS FFECVEN, ");
            query.Append(" H.FPEDIDO, H.FPLANIL, H.FCONSOL, H.FBODEGA, H.FTERMIN, H.FCLIENT, TRIM(H.FNOMCLI) FNOMCLI,");
            query.Append("");
            query.Append(" TRIM(H.FDIRCLI1) FDIRCLI1, TRIM(H.FDIRCLI3) FDIRCLI3, H.FDEPCLI, TRIM(P.CCCODE) FPAICLI, TRIM(H.FNIT) FNIT, TRIM(H.FNOMPEN) FNOMPEN,");
            query.Append(" TRIM(H.FDIRPEN1) FDIRPEN1, TRIM(H.FDIRPEN3) FDIRPEN3, H.FDEPPEN, TRIM(E.CCCODE) FPAIPEN, DECIMAL(H.FSUBTOT, 14, 4) FSUBTOT, DECIMAL(H.FTOTFAC, 14, 4) FTOTFAC, H.FACRTDAT,");
            query.AppendFormat(" (SELECT COUNT(*) FROM RFFACDET WHERE DPREFIJ = '{0}' AND DFACTUR = {1}) AS TotLineas,", Prefijo, Factura);
            query.Append(" TRIM(C.CTAX) CTAX,");
            query.Append(" TRIM(SUFD05) SUFD05, TRIM(SUFD06) SUFD06, SUFD13, SUFD14, TRIM(SUFD17) SUFD17, TRIM(SUFD19) SUFD19");
            query.Append(" FROM RFFACCAB H");
            query.Append(" INNER JOIN RCM C ON H.FCLIENT = C.CCUST");
            query.Append(" INNER JOIN ZCC P ON H.FPAICLI = P.CCSDSC AND P.CCTABL = 'FETABL01'");
            query.Append(" INNER JOIN ZCC E ON H.FPAIPEN = E.CCSDSC AND E.CCTABL = 'FETABL01'");
            query.Append(" INNER JOIN RSUL01 A ON H.FCLIENT = A.SUCUST");
            query.AppendFormat(" WHERE H.FPREFIJ = '{0}' AND H.FFACTUR = {1}", Prefijo, Factura);

            return db.Query<RFFACCAB>(query.ToString()).SingleOrDefault();
        }
    }
}
