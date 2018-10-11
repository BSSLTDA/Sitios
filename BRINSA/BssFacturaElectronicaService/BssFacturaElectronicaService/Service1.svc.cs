using CLCommon;
using CLDB2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace BssFacturaElectronicaService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        IRFFACDETRepository dRFFACDET = new RFFACDETRepository();
        IRFFACCABRepository dRFFACCAB = new RFFACCABRepository();
        List<RFFACDET> lmRFFACDET = new List<RFFACDET>();
        RFFACCAB mRFFACCAB = new RFFACCAB();

        protected string GetNonce()
        {
            string phrase = Guid.NewGuid().ToString();
            return phrase;
        }

        private string BuildSoapHeader(string credid, string credpassword)
        {
            var nonce = GetNonce();
            string nonceToSend = Convert.ToBase64String(Encoding.UTF8.GetBytes(nonce));
            string utc = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            StringBuilder rawSOAP = new StringBuilder();
            rawSOAP.Append(@"<soapenv:Header>");
            rawSOAP.Append(@"<wsse:Security xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">");
            rawSOAP.Append(@"<wsse:UsernameToken wsu:Id=""UsernameToken-757E1C333DD11835B515295837182745"">");
            rawSOAP.Append(@"<wsse:Username>" + credid + "</wsse:Username>");
            rawSOAP.Append(@"<wsse:Password Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">" + credpassword + "</wsse:Password>");
            rawSOAP.Append(@"<wsse:Nonce EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">" + nonceToSend + "</wsse:Nonce>");
            rawSOAP.Append(@"<wsu:Created>" + utc + "</wsu:Created>");
            rawSOAP.Append(@"</wsse:UsernameToken>");
            rawSOAP.Append(@"</wsse:Security>");
            rawSOAP.Append(@"</soapenv:Header>");
            return rawSOAP.ToString();
        }

        static string Sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        public void servicio2()
        {
            string url = "https://cenfinancierolab.cen.biz/isows/InvoiceService";
            string credid = "facturacionelectronica@brinsa.com.co";
            string credpassword = Sha256("Brinsa2018*");

            StringBuilder rawSOAP = new StringBuilder();
            rawSOAP.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:inv=""http://invoice.carvajal.com/invoiceService/"">");
            rawSOAP.Append(BuildSoapHeader(credid, credpassword));
            rawSOAP.Append(@"<soapenv:Body><inv:DownloadRequest>");
            rawSOAP.Append(@"<documentType></documentType>");
            rawSOAP.Append(@"<documentNumber></converdocumentNumber>");
            rawSOAP.Append(@"<documentPrefix></documentPrefix>");
            rawSOAP.Append(@"<resourceType></resourceType>");
            rawSOAP.Append(@"<companyId>800221789</companyId>");
            rawSOAP.Append(@"<accountId>800221789_01</accountId>");
            rawSOAP.Append(@"</inv:DownloadRequest></soapenv:Body></soapenv:Envelope>");
            string SOAPObj = rawSOAP.ToString();
            using (var wb = new WebClient())
            {
                wb.Credentials = new NetworkCredential(credid, credpassword);
                try
                {
                    var responseVal = wb.UploadString(url, "POST", SOAPObj);
                    //FDServ.UploadResponse rest = Newtonsoft.Json.JsonConvert.DeserializeObject<FDServ.UploadResponse>(responseVal);
                }
                catch (Exception ex)
                {
                    string eee = ex.ToString();
                }
            }
        }


        private string xmlfac()
        {
            StringBuilder s = new StringBuilder();
            s.Append(" <?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            s.Append(" <FACTURA>");
            s.Append("     <ENC>");
            s.Append("         <ENC_1>INVOIC</ENC_1>");
            s.Append("         <ENC_2>890105927</ENC_2>");
            s.Append("         <ENC_3>65-0462207</ENC_3>");
            s.Append("         <ENC_4>UBL 2.0</ENC_4>");
            s.Append("         <ENC_5>DIAN 1.0</ENC_5>");
            s.Append("         <ENC_6>EX11617</ENC_6>");
            s.Append("         <ENC_7>2018-07-02</ENC_7>");
            s.Append("         <ENC_8>23:10:01</ENC_8>");
            s.Append("         <ENC_9>1</ENC_9>");
            s.Append("         <ENC_10>USD</ENC_10>");
            s.Append("         <ENC_11></ENC_11>");
            s.Append("         <ENC_12></ENC_12>");
            s.Append("         <ENC_13></ENC_13>");
            s.Append("         <ENC_14></ENC_14>");
            s.Append("         <ENC_15></ENC_15>");
            s.Append("         <ENC_16>2018-12-29</ENC_16>");
            s.Append("         <ENC_17></ENC_17>");
            s.Append("         <ENC_18></ENC_18>");
            s.Append("     </ENC>");
            s.Append("     <EMI>");
            s.Append("         <EMI_1>3</EMI_1>");
            s.Append("         <EMI_2>890105927</EMI_2>");
            s.Append("         <EMI_3>31</EMI_3>");
            s.Append("         <EMI_4>2</EMI_4>");
            s.Append("         <EMI_5>10</EMI_5>");
            s.Append("         <EMI_6>C.I. FARMACAPSULAS S.A.</EMI_6>");
            s.Append("         <EMI_7>C.I. FARMACAPSULAS S.A.</EMI_7>");
            s.Append("         <EMI_8></EMI_8>");
            s.Append("         <EMI_9></EMI_9>");
            s.Append("         <EMI_10>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_10>");
            s.Append("         <EMI_11></EMI_11>");
            s.Append("         <EMI_12>BARRANQUILLA</EMI_12>");
            s.Append("         <EMI_13>BARRANQUILLA</EMI_13>");
            s.Append("         <EMI_14></EMI_14>");
            s.Append("         <EMI_15>CO</EMI_15>");
            s.Append("         <EMI_16></EMI_16>");
            s.Append("         <EMI_17></EMI_17>");
            s.Append("         <EMI_18>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_18>");
            s.Append("         <EMI_19></EMI_19>");
            s.Append("         <EMI_20></EMI_20>");
            s.Append("         <EMI_21></EMI_21>");
            s.Append("         <ICC>");
            s.Append("             <ICC_1></ICC_1>");
            s.Append("             <ICC_2></ICC_2>");
            s.Append("             <ICC_3></ICC_3>");
            s.Append("             <ICC_4></ICC_4>");
            s.Append("             <ICC_5></ICC_5>");
            s.Append("             <ICC_6></ICC_6>");
            s.Append("             <ICC_7></ICC_7>");
            s.Append("             <ICC_8></ICC_8>");
            s.Append("         </ICC>");
            s.Append("     </EMI>");
            s.Append("     <ADQ>");
            s.Append("         <ADQ_1>-1</ADQ_1>");
            s.Append("         <ADQ_2>65-0462207     </ADQ_2>");
            s.Append("         <ADQ_3>31</ADQ_3>");
            s.Append("         <ADQ_4>-1</ADQ_4>");
            s.Append("         <ADQ_5>1010</ADQ_5>");
            s.Append("         <ADQ_6></ADQ_6>");
            s.Append("         <ADQ_7></ADQ_7>");
            s.Append("         <ADQ_8></ADQ_8>");
            s.Append("         <ADQ_9></ADQ_9>");
            s.Append("         <ADQ_10>1893 SW 3RD STREET</ADQ_10>");
            s.Append("         <ADQ_11></ADQ_11>");
            s.Append("         <ADQ_12></ADQ_12>");
            s.Append("         <ADQ_13></ADQ_13>");
            s.Append("         <ADQ_14></ADQ_14>");
            s.Append("         <ADQ_15>US</ADQ_15>");
            s.Append("         <ADQ_16></ADQ_16>");
            s.Append("         <ADQ_17></ADQ_17>");
            s.Append("         <ADQ_18>1893 SW 3RD STREET</ADQ_18>");
            s.Append("         <ADQ_19></ADQ_19>");
            s.Append("         <ADQ_20></ADQ_20>");
            s.Append("         <ADQ_21></ADQ_21>");
            s.Append("         <TCR>");
            s.Append("             <TCR_1>O-99</TCR_1>");
            s.Append("             <TCR_2></TCR_2>");
            s.Append("             <TCR_3></TCR_3>");
            s.Append("             <TCR_4></TCR_4>");
            s.Append("             <TCR_5></TCR_5>");
            s.Append("             <TCR_6></TCR_6>");
            s.Append("             <TCR_7></TCR_7>");
            s.Append("             <TCR_8></TCR_8>");
            s.Append("             <TCR_9></TCR_9>");
            s.Append("             <TCR_10></TCR_10>");
            s.Append("             <TCR_11></TCR_11>");
            s.Append("         </TCR>");
            s.Append("         <ICR>");
            s.Append("             <ICR_1></ICR_1>");
            s.Append("             <ICR_2></ICR_2>");
            s.Append("             <ICR_3></ICR_3>");
            s.Append("             <ICR_4></ICR_4>");
            s.Append("             <ICR_5></ICR_5>");
            s.Append("             <ICR_6></ICR_6>");
            s.Append("             <ICR_7></ICR_7>");
            s.Append("             <ICR_8></ICR_8>");
            s.Append("         </ICR>");
            s.Append("         <CDA>");
            s.Append("             <CDA_1></CDA_1>");
            s.Append("             <CDA_2></CDA_2>");
            s.Append("             <CDA_3></CDA_3>");
            s.Append("             <CDA_4></CDA_4>");
            s.Append("         </CDA>");
            s.Append("     </ADQ>");
            s.Append("     <TOT>");
            s.Append("         <TOT_1>4546000</TOT_1>");
            s.Append("         <TOT_2>USD</TOT_2>");
            s.Append("         <TOT_3>4546000</TOT_3>");
            s.Append("         <TOT_4>USD</TOT_4>");
            s.Append("         <TOT_5>4546000</TOT_5>");
            s.Append("         <TOT_6>USD</TOT_6>");
            s.Append("         <TOT_7>4546000</TOT_7>");
            s.Append("         <TOT_8>USD</TOT_8>");
            s.Append("         <TOT_9>4546000</TOT_9>");
            s.Append("         <TOT_10>USD</TOT_10>");
            s.Append("         <TOT_12>USD</TOT_12>");
            s.Append("     </TOT>");
            s.Append("     <TDC/>");
            s.Append("     <OTC/>");
            s.Append("     <DSC>");
            s.Append("         <DSC_1>false</DSC_1>");
            s.Append("         <DSC_2>1</DSC_2>");
            s.Append("         <DSC_3>801.05</DSC_3>");
            s.Append("         <DSC_4>USD</DSC_4>");
            s.Append("         <DSC_5>NO ENCONTRO FLETES / FREIGHT</DSC_5>");
            s.Append("     </DSC>");
            s.Append("     <DSC>");
            s.Append("         <DSC_1>false</DSC_1>");
            s.Append("         <DSC_2>1</DSC_2>");
            s.Append("         <DSC_3>45</DSC_3>");
            s.Append("         <DSC_4>USD</DSC_4>");
            s.Append("         <DSC_5>NO ENCONTRO SEGURO / INSURANCE</DSC_5>");
            s.Append("     </DSC>");
            s.Append("     <DRF>");
            s.Append("         <DRF_1>18762007201018                                                                                                </DRF_1>");
            s.Append("         <DRF_2>2018-03-05</DRF_2>");
            s.Append("         <DRF_3>2020-03-04</DRF_3>");
            s.Append("         <DRF_4>EX</DRF_4>");
            s.Append("         <DRF_5>11317</DRF_5>");
            s.Append("         <DRF_6>14000</DRF_6>");
            s.Append("     </DRF>");
            s.Append("     <NOT>");
            s.Append("         <NOT_1>1.-SOMOS GRANDES CONTRIBUYENTES RESOLUCIÃ“N DIAN 0000076 DE 2016/12/01</NOT_1>");
            s.Append("     </NOT>");
            s.Append("     <NOT>");
            s.Append("         <NOT_1>8.-39.23.50.90.00</NOT_1>");
            s.Append("     </NOT>");
            s.Append("     <NOT>");
            s.Append("         <NOT_1>9.-MP-0075</NOT_1>");
            s.Append("     </NOT>");
            s.Append("     <NOT>");
            s.Append("         <NOT_1>10.-39</NOT_1>");
            s.Append("     </NOT>");
            s.Append("     <NOT>");
            s.Append("         <NOT_1>11.-73,284%</NOT_1>");
            s.Append("     </NOT>");
            s.Append("     <NOT>");
            s.Append("         <NOT_1>12.-NET 180        </NOT_1>");
            s.Append("     </NOT>");
            s.Append("     <REF>");
            s.Append("         <REF_1>AAJ</REF_1>");
            s.Append("         <REF_2>279107</REF_2>");
            s.Append("     </REF>");
            s.Append("     <REF>");
            s.Append("         <REF_1>AAJ</REF_1>");
            s.Append("         <REF_2>279108</REF_2>");
            s.Append("     </REF>");
            s.Append("     <REF>");
            s.Append("         <REF_1>AAJ</REF_1>");
            s.Append("         <REF_2>279109</REF_2>");
            s.Append("     </REF>");
            s.Append("     <IEN>");
            s.Append("         <IEN_1>1893 SW 3RD STREET</IEN_1>");
            s.Append("         <IEN_6>US</IEN_6>");
            s.Append("     </IEN>");
            s.Append("     <ITE>");
            s.Append("         <ITE_1>1</ITE_1>");
            s.Append("         <ITE_2>false</ITE_2>");
            s.Append("         <ITE_3>500000.000</ITE_3>");
            s.Append("         <ITE_4>MIO</ITE_4>");
            s.Append("         <ITE_5>1795000</ITE_5>");
            s.Append("         <ITE_6>USD</ITE_6>");
            s.Append("         <ITE_7>3.59</ITE_7>");
            s.Append("         <ITE_8>USD</ITE_8>");
            s.Append("         <ITE_9>K00003</ITE_9>");
            s.Append("         <ITE_10>HARD EMPTY VEGETABLE CAPSULES|CAPSULAS  VGETALES DE DOS PIEZAS VACIAS|SIZE 0 1-0K/1-0K (NATURAL/NATURAL)</ITE_10>");
            s.Append("         <ITE_11></ITE_11>");
            s.Append("         <ITE_12></ITE_12>");
            s.Append("         <TII>");
            s.Append("             <TII_1>0</TII_1>");
            s.Append("             <TII_2>USD</TII_2>");
            s.Append("             <TII_3>false</TII_3>");
            s.Append("         </TII>");
            s.Append("     </ITE>");
            s.Append("     <ITE>");
            s.Append("         <ITE_1>2</ITE_1>");
            s.Append("         <ITE_2>false</ITE_2>");
            s.Append("         <ITE_3>400000.000</ITE_3>");
            s.Append("         <ITE_4>MIO</ITE_4>");
            s.Append("         <ITE_5>2304000</ITE_5>");
            s.Append("         <ITE_6>USD</ITE_6>");
            s.Append("         <ITE_7>5.76</ITE_7>");
            s.Append("         <ITE_8>USD</ITE_8>");
            s.Append("         <ITE_9>K00012</ITE_9>");
            s.Append("         <ITE_10>HARD EMPTY VEGETABLE  CAPSULES|CAPSULAS  VEGETAL DE DOS PIEZAS VACIAS|SIZE/TAMAÑO 00 1-0K/1-0K (NATURAL/NATURAL)</ITE_10>");
            s.Append("         <ITE_11></ITE_11>");
            s.Append("         <ITE_12></ITE_12>");
            s.Append("         <TII>");
            s.Append("             <TII_1>0</TII_1>");
            s.Append("             <TII_2>USD</TII_2>");
            s.Append("             <TII_3>false</TII_3>");
            s.Append("         </TII>");
            s.Append("     </ITE>");
            s.Append("     <ITE>");
            s.Append("         <ITE_1>3</ITE_1>");
            s.Append("         <ITE_2>false</ITE_2>");
            s.Append("         <ITE_3>75000.000</ITE_3>");
            s.Append("         <ITE_4>MIO</ITE_4>");
            s.Append("         <ITE_5>447000</ITE_5>");
            s.Append("         <ITE_6>USD</ITE_6>");
            s.Append("         <ITE_7>5.96</ITE_7>");
            s.Append("         <ITE_8>USD</ITE_8>");
            s.Append("         <ITE_9>K00061</ITE_9>");
            s.Append("         <ITE_10>HARD EMPTY TWO PIECES VEGETABLE CAPSULE|CAPSULAS VEGETALES DE DOS PIEZAS VACIAS|SIZE 00E 1-0K/1-0K (NATURAL/NATURAL)</ITE_10>");
            s.Append("         <ITE_11>KC00E-1000</ITE_11>");
            s.Append("         <ITE_12></ITE_12>");
            s.Append("         <TII>");
            s.Append("             <TII_1>0</TII_1>");
            s.Append("             <TII_2>USD</TII_2>");
            s.Append("             <TII_3>false</TII_3>");
            s.Append("         </TII>");
            s.Append("     </ITE>");
            s.Append(" </FACTURA>");
            s.Append(" ");


            string x = s.ToString().Trim();



            string res = Convert.ToBase64String(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?> <FACTURA> 	<ENC> 		<ENC_1>INVOIC</ENC_1> 		<ENC_2>890105927</ENC_2> 		<ENC_3>65-0462207</ENC_3> 		<ENC_4>UBL 2.0</ENC_4> 		<ENC_5>DIAN 1.0</ENC_5> 		<ENC_6>EX11617</ENC_6> 		<ENC_7>2018-07-02</ENC_7> 		<ENC_8>23:10:01</ENC_8> 		<ENC_9>1</ENC_9> 		<ENC_10>USD</ENC_10> 		<ENC_11/> 		<ENC_12/> 		<ENC_13/> 		<ENC_14/> 		<ENC_15/> 		<ENC_16>2018-12-29</ENC_16> 		<ENC_17/> 		<ENC_18/> 	</ENC> 	<EMI> 		<EMI_1>3</EMI_1> 		<EMI_2>890105927</EMI_2> 		<EMI_3>31</EMI_3> 		<EMI_4>2</EMI_4> 		<EMI_5>10</EMI_5> 		<EMI_6>C.I. FARMACAPSULAS S.A.</EMI_6> 		<EMI_7>C.I. FARMACAPSULAS S.A.</EMI_7> 		<EMI_8/> 		<EMI_9/> 		<EMI_10>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_10> 		<EMI_11/> 		<EMI_12>BARRANQUILLA</EMI_12> 		<EMI_13>BARRANQUILLA</EMI_13> 		<EMI_14/> 		<EMI_15>CO</EMI_15> 		<EMI_16/> 		<EMI_17/> 		<EMI_18>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_18> 		<EMI_19/> 		<EMI_20/> 		<EMI_21/> 		<ICC> 			<ICC_1/> 			<ICC_2/> 			<ICC_3/> 			<ICC_4/> 			<ICC_5/> 			<ICC_6/> 			<ICC_7/> 			<ICC_8/> 		</ICC> 	</EMI> 	<ADQ> 		<ADQ_1>-1</ADQ_1> 		<ADQ_2>65-0462207 </ADQ_2> 		<ADQ_3>31</ADQ_3> 		<ADQ_4>-1</ADQ_4> 		<ADQ_5>1010</ADQ_5> 		<ADQ_6/> 		<ADQ_7/> 		<ADQ_8/> 		<ADQ_9/> 		<ADQ_10>1893 SW 3RD STREET</ADQ_10> 		<ADQ_11/> 		<ADQ_12/> 		<ADQ_13/> 		<ADQ_14/> 		<ADQ_15>US</ADQ_15> 		<ADQ_16/> 		<ADQ_17/> 		<ADQ_18>1893 SW 3RD STREET</ADQ_18> 		<ADQ_19/> 		<ADQ_20/> 		<ADQ_21/> 		<TCR> 			<TCR_1>O-99</TCR_1> 			<TCR_2/> 			<TCR_3/> 			<TCR_4/> 			<TCR_5/> 			<TCR_6/> 			<TCR_7/> 			<TCR_8/> 			<TCR_9/> 			<TCR_10/> 			<TCR_11/> 		</TCR> 		<ICR> 			<ICR_1/> 			<ICR_2/> 			<ICR_3/> 			<ICR_4/> 			<ICR_5/> 			<ICR_6/> 			<ICR_7/> 			<ICR_8/> 		</ICR> 		<CDA> 			<CDA_1/> 			<CDA_2/> 			<CDA_3/> 			<CDA_4/> 		</CDA> 	</ADQ> 	<TOT> 		<TOT_1>4546000</TOT_1> 		<TOT_2>USD</TOT_2> 		<TOT_3>4546000</TOT_3> 		<TOT_4>USD</TOT_4> 		<TOT_5>4546000</TOT_5> 		<TOT_6>USD</TOT_6> 		<TOT_7>4546000</TOT_7> 		<TOT_8>USD</TOT_8> 		<TOT_9>4546000</TOT_9> 		<TOT_10>USD</TOT_10> 		<TOT_12>USD</TOT_12> 	</TOT> 	<TDC/> 	<OTC/> 	<DSC> 		<DSC_1>false</DSC_1> 		<DSC_2>1</DSC_2> 		<DSC_3>801.05</DSC_3> 		<DSC_4>USD</DSC_4> 		<DSC_5>NO ENCONTRO FLETES / FREIGHT</DSC_5> 	</DSC> 	<DSC> 		<DSC_1>false</DSC_1> 		<DSC_2>1</DSC_2> 		<DSC_3>45</DSC_3> 		<DSC_4>USD</DSC_4> 		<DSC_5>NO ENCONTRO SEGURO / INSURANCE</DSC_5> 	</DSC> 	<DRF> 		<DRF_1>18762007201018 </DRF_1> 		<DRF_2>2018-03-05</DRF_2> 		<DRF_3>2020-03-04</DRF_3> 		<DRF_4>EX</DRF_4> 		<DRF_5>11317</DRF_5> 		<DRF_6>14000</DRF_6> 	</DRF> 	<NOT> 		<NOT_1>1.-SOMOS GRANDES CONTRIBUYENTES RESOLUCIÃ“N DIAN 0000076 DE 2016/12/01</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>8.-39.23.50.90.00</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>9.-MP-0075</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>10.-39</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>11.-73,284%</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>12.-NET 180 </NOT_1> 	</NOT> 	<REF> 		<REF_1>AAJ</REF_1> 		<REF_2>279107</REF_2> 	</REF> 	<REF> 		<REF_1>AAJ</REF_1> 		<REF_2>279108</REF_2> 	</REF> 	<REF> 		<REF_1>AAJ</REF_1> 		<REF_2>279109</REF_2> 	</REF> 	<IEN> 		<IEN_1>1893 SW 3RD STREET</IEN_1> 		<IEN_6>US</IEN_6> 	</IEN> 	<ITE> 		<ITE_1>1</ITE_1> 		<ITE_2>false</ITE_2> 		<ITE_3>500000.000</ITE_3> 		<ITE_4>MIO</ITE_4> 		<ITE_5>1795000</ITE_5> 		<ITE_6>USD</ITE_6> 		<ITE_7>3.59</ITE_7> 		<ITE_8>USD</ITE_8> 		<ITE_9>K00003</ITE_9> 		<ITE_10>HARD EMPTY VEGETABLE CAPSULES|CAPSULAS VGETALES DE DOS PIEZAS VACIAS|SIZE 0 1-0K/1-0K (NATURAL/NATURAL)</ITE_10> 		<ITE_11/> 		<ITE_12/> 		<TII> 			<TII_1>0</TII_1> 			<TII_2>USD</TII_2> 			<TII_3>false</TII_3> 		</TII> 	</ITE> 	<ITE> 		<ITE_1>2</ITE_1> 		<ITE_2>false</ITE_2> 		<ITE_3>400000.000</ITE_3> 		<ITE_4>MIO</ITE_4> 		<ITE_5>2304000</ITE_5> 		<ITE_6>USD</ITE_6> 		<ITE_7>5.76</ITE_7> 		<ITE_8>USD</ITE_8> 		<ITE_9>K00012</ITE_9> 		<ITE_10>HARD EMPTY VEGETABLE CAPSULES|CAPSULAS VEGETAL DE DOS PIEZAS VACIAS|SIZE/TAMAÑO 00 1-0K/1-0K (NATURAL/NATURAL)</ITE_10> 		<ITE_11/> 		<ITE_12/> 		<TII> 			<TII_1>0</TII_1> 			<TII_2>USD</TII_2> 			<TII_3>false</TII_3> 		</TII> 	</ITE> 	<ITE> 		<ITE_1>3</ITE_1> 		<ITE_2>false</ITE_2> 		<ITE_3>75000.000</ITE_3> 		<ITE_4>MIO</ITE_4> 		<ITE_5>447000</ITE_5> 		<ITE_6>USD</ITE_6> 		<ITE_7>5.96</ITE_7> 		<ITE_8>USD</ITE_8> 		<ITE_9>K00061</ITE_9> 		<ITE_10>HARD EMPTY TWO PIECES VEGETABLE CAPSULE|CAPSULAS VEGETALES DE DOS PIEZAS VACIAS|SIZE 00E 1-0K/1-0K (NATURAL/NATURAL)</ITE_10> 		<ITE_11>KC00E-1000</ITE_11> 		<ITE_12/> 		<TII> 			<TII_1>0</TII_1> 			<TII_2>USD</TII_2> 			<TII_3>false</TII_3> 		</TII> 	</ITE> </FACTURA> "));
            res = Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(x));
            //return Convert.ToBase64String(Encoding.UTF8.GetBytes(s.ToString()));

            return "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9InllcyI/Pg0KPEZBQ1RVUkE+DQoJPEVOQz4NCgkJPEVOQ18xPklOVk9JQzwvRU5DXzE+DQoJCTxFTkNfMj44OTAxMDU5Mjc8L0VOQ18yPg0KCQk8RU5DXzM+NjUtMDQ2MjIwNzwvRU5DXzM+DQoJCTxFTkNfND5VQkwgMi4wPC9FTkNfND4NCgkJPEVOQ181PkRJQU4gMS4wPC9FTkNfNT4NCgkJPEVOQ182PkVYMTE2MTc8L0VOQ182Pg0KCQk8RU5DXzc+MjAxOC0wNy0wMjwvRU5DXzc+DQoJCTxFTkNfOD4yMzoxMDowMTwvRU5DXzg+DQoJCTxFTkNfOT4xPC9FTkNfOT4NCgkJPEVOQ18xMD5VU0Q8L0VOQ18xMD4NCgkJPEVOQ18xMS8+DQoJCTxFTkNfMTIvPg0KCQk8RU5DXzEzLz4NCgkJPEVOQ18xNC8+DQoJCTxFTkNfMTUvPg0KCQk8RU5DXzE2PjIwMTgtMTItMjk8L0VOQ18xNj4NCgkJPEVOQ18xNy8+DQoJCTxFTkNfMTgvPg0KCTwvRU5DPg0KCTxFTUk+DQoJCTxFTUlfMT4zPC9FTUlfMT4NCgkJPEVNSV8yPjg5MDEwNTkyNzwvRU1JXzI+DQoJCTxFTUlfMz4zMTwvRU1JXzM+DQoJCTxFTUlfND4yPC9FTUlfND4NCgkJPEVNSV81PjEwPC9FTUlfNT4NCgkJPEVNSV82PkMuSS4gRkFSTUFDQVBTVUxBUyBTLkEuPC9FTUlfNj4NCgkJPEVNSV83PkMuSS4gRkFSTUFDQVBTVUxBUyBTLkEuPC9FTUlfNz4NCgkJPEVNSV84Lz4NCgkJPEVNSV85Lz4NCgkJPEVNSV8xMD5DQUxMRSA3OUIgTm8gNzhDLTIxIEFQRE8gQUVSRU8gLyBDT0RFIFBPU1RBTCA1MjQ4NjwvRU1JXzEwPg0KCQk8RU1JXzExLz4NCgkJPEVNSV8xMj5CQVJSQU5RVUlMTEE8L0VNSV8xMj4NCgkJPEVNSV8xMz5CQVJSQU5RVUlMTEE8L0VNSV8xMz4NCgkJPEVNSV8xNC8+DQoJCTxFTUlfMTU+Q088L0VNSV8xNT4NCgkJPEVNSV8xNi8+DQoJCTxFTUlfMTcvPg0KCQk8RU1JXzE4PkNBTExFIDc5QiBObyA3OEMtMjEgQVBETyBBRVJFTyAvIENPREUgUE9TVEFMIDUyNDg2PC9FTUlfMTg+DQoJCTxFTUlfMTkvPg0KCQk8RU1JXzIwLz4NCgkJPEVNSV8yMS8+DQoJCTxJQ0M+DQoJCQk8SUNDXzEvPg0KCQkJPElDQ18yLz4NCgkJCTxJQ0NfMy8+DQoJCQk8SUNDXzQvPg0KCQkJPElDQ181Lz4NCgkJCTxJQ0NfNi8+DQoJCQk8SUNDXzcvPg0KCQkJPElDQ184Lz4NCgkJPC9JQ0M+DQoJPC9FTUk+DQoJPEFEUT4NCgkJPEFEUV8xPi0xPC9BRFFfMT4NCgkJPEFEUV8yPjY1LTA0NjIyMDcgICAgIDwvQURRXzI+DQoJCTxBRFFfMz4zMTwvQURRXzM+DQoJCTxBRFFfND4tMTwvQURRXzQ+DQoJCTxBRFFfNT4xMDEwPC9BRFFfNT4NCgkJPEFEUV82Lz4NCgkJPEFEUV83Lz4NCgkJPEFEUV84Lz4NCgkJPEFEUV85Lz4NCgkJPEFEUV8xMD4xODkzIFNXIDNSRCBTVFJFRVQ8L0FEUV8xMD4NCgkJPEFEUV8xMS8+DQoJCTxBRFFfMTIvPg0KCQk8QURRXzEzLz4NCgkJPEFEUV8xNC8+DQoJCTxBRFFfMTU+VVM8L0FEUV8xNT4NCgkJPEFEUV8xNi8+DQoJCTxBRFFfMTcvPg0KCQk8QURRXzE4PjE4OTMgU1cgM1JEIFNUUkVFVDwvQURRXzE4Pg0KCQk8QURRXzE5Lz4NCgkJPEFEUV8yMC8+DQoJCTxBRFFfMjEvPg0KCQk8VENSPg0KCQkJPFRDUl8xPk8tOTk8L1RDUl8xPg0KCQkJPFRDUl8yLz4NCgkJCTxUQ1JfMy8+DQoJCQk8VENSXzQvPg0KCQkJPFRDUl81Lz4NCgkJCTxUQ1JfNi8+DQoJCQk8VENSXzcvPg0KCQkJPFRDUl84Lz4NCgkJCTxUQ1JfOS8+DQoJCQk8VENSXzEwLz4NCgkJCTxUQ1JfMTEvPg0KCQk8L1RDUj4NCgkJPElDUj4NCgkJCTxJQ1JfMS8+DQoJCQk8SUNSXzIvPg0KCQkJPElDUl8zLz4NCgkJCTxJQ1JfNC8+DQoJCQk8SUNSXzUvPg0KCQkJPElDUl82Lz4NCgkJCTxJQ1JfNy8+DQoJCQk8SUNSXzgvPg0KCQk8L0lDUj4NCgkJPENEQT4NCgkJCTxDREFfMS8+DQoJCQk8Q0RBXzIvPg0KCQkJPENEQV8zLz4NCgkJCTxDREFfNC8+DQoJCTwvQ0RBPg0KCTwvQURRPg0KCTxUT1Q+DQoJCTxUT1RfMT40NTQ2MDAwPC9UT1RfMT4NCgkJPFRPVF8yPlVTRDwvVE9UXzI+DQoJCTxUT1RfMz40NTQ2MDAwPC9UT1RfMz4NCgkJPFRPVF80PlVTRDwvVE9UXzQ+DQoJCTxUT1RfNT40NTQ2MDAwPC9UT1RfNT4NCgkJPFRPVF82PlVTRDwvVE9UXzY+DQoJCTxUT1RfNz40NTQ2MDAwPC9UT1RfNz4NCgkJPFRPVF84PlVTRDwvVE9UXzg+DQoJCTxUT1RfOT40NTQ2MDAwPC9UT1RfOT4NCgkJPFRPVF8xMD5VU0Q8L1RPVF8xMD4NCgkJPFRPVF8xMj5VU0Q8L1RPVF8xMj4NCgk8L1RPVD4NCgk8VERDLz4NCgk8T1RDLz4NCgk8RFNDPg0KCQk8RFNDXzE+ZmFsc2U8L0RTQ18xPg0KCQk8RFNDXzI+MTwvRFNDXzI+DQoJCTxEU0NfMz44MDEuMDU8L0RTQ18zPg0KCQk8RFNDXzQ+VVNEPC9EU0NfND4NCgkJPERTQ181Pk5PIEVOQ09OVFJPIEZMRVRFUyAvIEZSRUlHSFQ8L0RTQ181Pg0KCTwvRFNDPg0KCTxEU0M+DQoJCTxEU0NfMT5mYWxzZTwvRFNDXzE+DQoJCTxEU0NfMj4xPC9EU0NfMj4NCgkJPERTQ18zPjQ1PC9EU0NfMz4NCgkJPERTQ180PlVTRDwvRFNDXzQ+DQoJCTxEU0NfNT5OTyBFTkNPTlRSTyBTRUdVUk8gLyBJTlNVUkFOQ0U8L0RTQ181Pg0KCTwvRFNDPg0KCTxEUkY+DQoJCTxEUkZfMT4xODc2MjAwNzIwMTAxOCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvRFJGXzE+DQoJCTxEUkZfMj4yMDE4LTAzLTA1PC9EUkZfMj4NCgkJPERSRl8zPjIwMjAtMDMtMDQ8L0RSRl8zPg0KCQk8RFJGXzQ+RVg8L0RSRl80Pg0KCQk8RFJGXzU+MTEzMTc8L0RSRl81Pg0KCQk8RFJGXzY+MTQwMDA8L0RSRl82Pg0KCTwvRFJGPg0KCTxOT1Q+DQoJCTxOT1RfMT4xLi1TT01PUyBHUkFOREVTIENPTlRSSUJVWUVOVEVTIFJFU09MVUNJw4PigJxOIERJQU4gMDAwMDA3NiBERSAyMDE2LzEyLzAxPC9OT1RfMT4NCgk8L05PVD4NCgk8Tk9UPg0KCQk8Tk9UXzE+OC4tMzkuMjMuNTAuOTAuMDA8L05PVF8xPg0KCTwvTk9UPg0KCTxOT1Q+DQoJCTxOT1RfMT45Li1NUC0wMDc1PC9OT1RfMT4NCgk8L05PVD4NCgk8Tk9UPg0KCQk8Tk9UXzE+MTAuLTM5PC9OT1RfMT4NCgk8L05PVD4NCgk8Tk9UPg0KCQk8Tk9UXzE+MTEuLTczLDI4NCU8L05PVF8xPg0KCTwvTk9UPg0KCTxOT1Q+DQoJCTxOT1RfMT4xMi4tTkVUIDE4MCAgICAgICAgPC9OT1RfMT4NCgk8L05PVD4NCgk8UkVGPg0KCQk8UkVGXzE+QUFKPC9SRUZfMT4NCgkJPFJFRl8yPjI3OTEwNzwvUkVGXzI+DQoJPC9SRUY+DQoJPFJFRj4NCgkJPFJFRl8xPkFBSjwvUkVGXzE+DQoJCTxSRUZfMj4yNzkxMDg8L1JFRl8yPg0KCTwvUkVGPg0KCTxSRUY+DQoJCTxSRUZfMT5BQUo8L1JFRl8xPg0KCQk8UkVGXzI+Mjc5MTA5PC9SRUZfMj4NCgk8L1JFRj4NCgk8SUVOPg0KCQk8SUVOXzE+MTg5MyBTVyAzUkQgU1RSRUVUPC9JRU5fMT4NCgkJPElFTl82PlVTPC9JRU5fNj4NCgk8L0lFTj4NCgk8SVRFPg0KCQk8SVRFXzE+MTwvSVRFXzE+DQoJCTxJVEVfMj5mYWxzZTwvSVRFXzI+DQoJCTxJVEVfMz41MDAwMDAuMDAwPC9JVEVfMz4NCgkJPElURV80Pk1JTzwvSVRFXzQ+DQoJCTxJVEVfNT4xNzk1MDAwPC9JVEVfNT4NCgkJPElURV82PlVTRDwvSVRFXzY+DQoJCTxJVEVfNz4zLjU5PC9JVEVfNz4NCgkJPElURV84PlVTRDwvSVRFXzg+DQoJCTxJVEVfOT5LMDAwMDM8L0lURV85Pg0KCQk8SVRFXzEwPkhBUkQgRU1QVFkgVkVHRVRBQkxFIENBUFNVTEVTfENBUFNVTEFTICBWR0VUQUxFUyBERSBET1MgUElFWkFTIFZBQ0lBU3xTSVpFIDAgMS0wSy8xLTBLIChOQVRVUkFML05BVFVSQUwpPC9JVEVfMTA+DQoJCTxJVEVfMTEvPg0KCQk8SVRFXzEyLz4NCgkJPFRJST4NCgkJCTxUSUlfMT4wPC9USUlfMT4NCgkJCTxUSUlfMj5VU0Q8L1RJSV8yPg0KCQkJPFRJSV8zPmZhbHNlPC9USUlfMz4NCgkJPC9USUk+DQoJPC9JVEU+DQoJPElURT4NCgkJPElURV8xPjI8L0lURV8xPg0KCQk8SVRFXzI+ZmFsc2U8L0lURV8yPg0KCQk8SVRFXzM+NDAwMDAwLjAwMDwvSVRFXzM+DQoJCTxJVEVfND5NSU88L0lURV80Pg0KCQk8SVRFXzU+MjMwNDAwMDwvSVRFXzU+DQoJCTxJVEVfNj5VU0Q8L0lURV82Pg0KCQk8SVRFXzc+NS43NjwvSVRFXzc+DQoJCTxJVEVfOD5VU0Q8L0lURV84Pg0KCQk8SVRFXzk+SzAwMDEyPC9JVEVfOT4NCgkJPElURV8xMD5IQVJEIEVNUFRZIFZFR0VUQUJMRSAgQ0FQU1VMRVN8Q0FQU1VMQVMgIFZFR0VUQUwgREUgRE9TIFBJRVpBUyBWQUNJQVN8U0laRS9UQU1Bw5FPIDAwIDEtMEsvMS0wSyAoTkFUVVJBTC9OQVRVUkFMKTwvSVRFXzEwPg0KCQk8SVRFXzExLz4NCgkJPElURV8xMi8+DQoJCTxUSUk+DQoJCQk8VElJXzE+MDwvVElJXzE+DQoJCQk8VElJXzI+VVNEPC9USUlfMj4NCgkJCTxUSUlfMz5mYWxzZTwvVElJXzM+DQoJCTwvVElJPg0KCTwvSVRFPg0KCTxJVEU+DQoJCTxJVEVfMT4zPC9JVEVfMT4NCgkJPElURV8yPmZhbHNlPC9JVEVfMj4NCgkJPElURV8zPjc1MDAwLjAwMDwvSVRFXzM+DQoJCTxJVEVfND5NSU88L0lURV80Pg0KCQk8SVRFXzU+NDQ3MDAwPC9JVEVfNT4NCgkJPElURV82PlVTRDwvSVRFXzY+DQoJCTxJVEVfNz41Ljk2PC9JVEVfNz4NCgkJPElURV84PlVTRDwvSVRFXzg+DQoJCTxJVEVfOT5LMDAwNjE8L0lURV85Pg0KCQk8SVRFXzEwPkhBUkQgRU1QVFkgVFdPIFBJRUNFUyBWRUdFVEFCTEUgQ0FQU1VMRXxDQVBTVUxBUyBWRUdFVEFMRVMgREUgRE9TIFBJRVpBUyBWQUNJQVN8U0laRSAwMEUgMS0wSy8xLTBLIChOQVRVUkFML05BVFVSQUwpPC9JVEVfMTA+DQoJCTxJVEVfMTE+S0MwMEUtMTAwMDwvSVRFXzExPg0KCQk8SVRFXzEyLz4NCgkJPFRJST4NCgkJCTxUSUlfMT4wPC9USUlfMT4NCgkJCTxUSUlfMj5VU0Q8L1RJSV8yPg0KCQkJPFRJSV8zPmZhbHNlPC9USUlfMz4NCgkJPC9USUk+DQoJPC9JVEU+DQo8L0ZBQ1RVUkE+DQo=";
        }


        public string GetData2(int value)
        {
            mRFFACCAB = dRFFACCAB.Find("A", "406506");
            lmRFFACDET = dRFFACDET.Find("A", "406506");
            FACTURA mFACTURA = new FACTURA();
            ENC mENC = new ENC();
            List<ENC> lmENC = new List<ENC>();


            mENC.ENC_1 = "INVOIC";
            mENC.ENC_2 = "123456789";
            mENC.ENC_3 = "25847996";
            mENC.ENC_4 = "UBL 2.0";
            mENC.ENC_5 = "UBL 2.0ENC_5";
            mENC.ENC_6 = "UBL 2.06";
            mENC.ENC_7 = "UBL 2.07";
            mENC.ENC_8 = "UBL 2.08";
            mENC.ENC_9 = "UBL 2.09";
            mENC.ENC_10 = "prueba";
            mENC.ENC_11 = "prueba1";
            mENC.ENC_12 = "prueba2";
            mENC.ENC_13 = "prueba3";
            mENC.ENC_14 = "prueba4";
            mENC.ENC_15 = "prueba5";
            mENC.ENC_16 = "prueba6";
            mENC.ENC_17 = "prueba7";
            //mENC.ENC_18 = "prueba8";
            lmENC.Add(mENC);
            lmENC.Add(mENC);
            mFACTURA.LMdlENC = lmENC;

            XmlSerializer xs = new XmlSerializer(typeof(FACTURA));

            TextWriter writer = new StreamWriter("C:\\Copia_Servidor\\Proyectos\\BRINSA\\BssFacturaElectronicaService\\Prueba.xml");
            xs.Serialize(writer, mFACTURA);            
            writer.Close();


            string url = "https://cenfinancierolab.cen.biz/isows/InvoiceService";
            string credid = "facturacionelectronica@brinsa.com.co";
            string credpassword = Sha256("Brinsa2018*");
            
            StringBuilder rawSOAP = new StringBuilder();
            rawSOAP.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:inv=""http://invoice.carvajal.com/invoiceService/"">");
            rawSOAP.Append(BuildSoapHeader(credid, credpassword));
            rawSOAP.Append(@"<soapenv:Body><inv:UploadRequest>");
            rawSOAP.Append(@"<fileName>NombreArchivo.xml</fileName>");
            rawSOAP.Append(@"<fileData> </fileData>");
            rawSOAP.Append(@"<companyId>800221789</companyId>");
            rawSOAP.Append(@"<accountId>800221789_01</accountId>");
            rawSOAP.Append(@"</inv:UploadRequest></soapenv:Body></soapenv:Envelope>");
            string SOAPObj = rawSOAP.ToString();
            using (var wb = new WebClient())
            {
                wb.Credentials = new NetworkCredential(credid, credpassword);
                try
                {
                    var responseVal = wb.UploadString(url, "POST", SOAPObj);
                    //FDServ.UploadResponse rest = Newtonsoft.Json.JsonConvert.DeserializeObject<FDServ.UploadResponse>(responseVal);
                }
                catch (Exception ex)
                {
                    string eee = ex.ToString();
                }
            }

            return string.Format("You entered: {0}", value);
        }

        public string GetData3(int value)
        {
            
            string url = "https://cenfinancierolab.cen.biz/isows/InvoiceService";
            string credid = "facturacionelectronica@farmacapsulas.com";
            string credpassword = Sha256("Farma$2018$");

            StringBuilder rawSOAP = new StringBuilder();
            rawSOAP.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:inv=""http://invoice.carvajal.com/invoiceService/"">");
            rawSOAP.Append(BuildSoapHeader(credid, credpassword));
            rawSOAP.Append(@"<soapenv:Body><inv:UploadRequest>");
            rawSOAP.Append(@"<fileName>NombreArchivo.xml</fileName>");
            rawSOAP.AppendFormat(@"<fileData>{0}</fileData>", xmlfac());
            rawSOAP.Append(@"<companyId>8901059273</companyId>");
            rawSOAP.Append(@"<accountId>8901059273_01</accountId>");
            rawSOAP.Append(@"</inv:UploadRequest></soapenv:Body></soapenv:Envelope>");
            string SOAPObj = rawSOAP.ToString();
            using (var wb = new WebClient())
            {
                wb.Credentials = new NetworkCredential(credid, credpassword);
                try
                {
                    var responseVal = wb.UploadString(url, "POST", SOAPObj);
                    //FDServ.UploadResponse rest = Newtonsoft.Json.JsonConvert.DeserializeObject<FDServ.UploadResponse>(responseVal);
                }
                catch (Exception ex)
                {
                    string eee = ex.ToString();
                }
            }

            return string.Format("You entered: {0}", value);
        }

        public string GetData(int value)
        {

            string url = "https://cenfinancierolab.cen.biz/isows/InvoiceService";
            string credid = "facturacionelectronica@farmacapsulas.com";
            string credpassword = Sha256("Farma$2018$");

            StringBuilder rawSOAP = new StringBuilder();
            rawSOAP.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:inv=""http://invoice.carvajal.com/invoiceService/"">");
            rawSOAP.Append(BuildSoapHeader(credid, credpassword));
            rawSOAP.Append(@"<soapenv:Body><inv:DocumentStatusRequest>");            
            rawSOAP.AppendFormat(@"<transactionId>{0}</transactionId>", "2f7817960dea437b957621f49f78a090");
            rawSOAP.Append(@"<companyId>8901059273</companyId>");
            rawSOAP.Append(@"<accountId>8901059273_01</accountId>");
            rawSOAP.Append(@"</inv:DocumentStatusRequest></soapenv:Body></soapenv:Envelope>");
            string SOAPObj = rawSOAP.ToString();
            using (var wb = new WebClient())
            {
                wb.Credentials = new NetworkCredential(credid, credpassword);
                try
                {
                    var responseVal = wb.UploadString(url, "POST", SOAPObj);
                    //FDServ.UploadResponse rest = Newtonsoft.Json.JsonConvert.DeserializeObject<FDServ.UploadResponse>(responseVal);
                }
                catch (Exception ex)
                {
                    string eee = ex.ToString();
                }
            }

            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
