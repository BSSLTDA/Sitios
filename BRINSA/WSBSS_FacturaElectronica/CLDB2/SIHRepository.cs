using CLCommon;
using Dapper;
using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CLDB2
{
    public class SIHRepository : ISIHRepository
    {
        private readonly IDbConnection db = new iDB2Connection(ConfigurationManager.ConnectionStrings["ConexionDB2"].ToString());
        public List<SIH> GetFacturas()
        {
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" SELECT DISTINCT");
            sbF.Add("            SICUST");
            sbF.Add(@"         , TRIM(C.CCNOT1) || TRIM(IHDPFX) || DIGITS(IHDOCN) || '.pdf' PDF");
            sbF.Add("          , SIINVD");
            sbF.Add("          , TRIM(IHDPFX) IHDPFX");
            sbF.Add("          , DIGITS(IHDOCN) IHDOCN");
            sbF.Add("          , SINAM");
            sbF.Add("          , SIWHSE");
            sbF.Add("          , TRIM(IHDPFX) || DIGITS(IHDOCN) AS FACTURA");
            sbF.Add("          , P.CCUDV1");
            sbF.Add(" FROM");
            sbF.Add("            SIH F");
            sbF.Add("            INNER JOIN RFFACCAB FE ON FE.FPREFIJ     = F.IHDPFX AND FE.FFACTUR = F.IHDOCN");
            sbF.Add("            INNER JOIN RFPARAM P ON P.CCCODE2     = F.IHDPFX AND F.IHDOCN >= P.CCUDV1 AND P.CCTABL  = 'FEPARAMG' AND P.CCCODE  = 'PREFIJO'");
            sbF.Add("            INNER JOIN RFPARAM C ON C.CCTABL  = 'FEPARAMG' AND C.CCCODE = 'PDF_FE'");
            sbF.Add(" WHERE");
            sbF.Add("            (");
            sbF.Add("                       Ihdtyp     = 1");
            sbF.Add("                       AND Siqty >=0");
            sbF.Add("                       AND Sitot >=0");
            sbF.Add("            )");
            sbF.Add("            AND (FAFLAG04 IN(0, -1)");
            sbF.Add("            OR");
            sbF.Add("            (");
            sbF.Add("                       FAREF02      = 'ENVIANDO'");
            sbF.Add("                       AND FAFLAG04 = 1");
            sbF.Add("            ))");
            sbF.Add(" Limit 24");
            List<SIH> lmSIH = new List<SIH>();
            try
            {                
                lmSIH = db.Query<SIH>(sbF.GetSQL(), commandTimeout: 60).ToList();             
            }
            catch (iDB2Exception ex)
            {
                string err = ex.ToString();
            }            
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
            return lmSIH;
        }

        public List<SIH> GetNotas()
        {
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" SELECT DISTINCT");
            sbF.Add("            SICUST");
            sbF.Add(@"         , TRIM(C.CCNOT1) || TRIM(IHDPFX) || DIGITS(IHDOCN) || '.pdf' PDF");
            sbF.Add("          , SIINVD");
            sbF.Add("          , IHDPFX");
            sbF.Add("          , DIGITS(IHDOCN) IHDOCN");
            sbF.Add("          , SINAM");
            sbF.Add("          , SIWHSE");
            sbF.Add("          , TRIM(IHDPFX) || DIGITS(IHDOCN) AS FACTURA");
            sbF.Add("          , P.CCUDV1");
            sbF.Add(" FROM");
            sbF.Add("            SIH F");
            sbF.Add("            INNER JOIN RFNCCAB FE ON FE.FPREFIJ   = F.IHDPFX AND FE.FNOTA = F.IHDOCN");
            sbF.Add("            INNER JOIN RFPARAM P ON P.CCCODE2    = F.IHDPFX AND F.IHDOCN >= P.CCUDV1 AND P.CCTABL = 'FEPARAMG' AND P.CCCODE = 'PREFIJO'");
            sbF.Add("            INNER JOIN RFPARAM C ON C.CCTABL  = 'FEPARAMG' AND C.CCCODE = 'PDF_NCE'");
            sbF.Add(" WHERE");
            sbF.Add("            (");
            sbF.Add("                       F.IHDTYP = 3");
            sbF.Add("                       OR");
            sbF.Add("                       (Ihdtyp = 2 OR (Ihdtyp = 1 AND (Siqty    < 0 OR Sitot < 0)))");
            sbF.Add("            )");
            sbF.Add("            AND (FAFLAG04 IN(0, -1)");
            sbF.Add("            OR");
            sbF.Add("            (");
            sbF.Add("                       FAREF02      = 'ENVIANDO'");
            sbF.Add("                       AND FAFLAG04 = 1");
            sbF.Add("            ))");
            sbF.Add(" Limit 24");
            return db.Query<SIH>(sbF.GetSQL(), commandTimeout: 60).ToList();
        }
    }
}
