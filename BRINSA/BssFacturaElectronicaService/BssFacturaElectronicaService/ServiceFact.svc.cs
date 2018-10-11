using CLCommon;
using CLDB2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
//using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
//using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

namespace BssFacturaElectronicaService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "ServiceFact" en el código, en svc y en el archivo de configuración a la vez.
    // NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione ServiceFact.svc o ServiceFact.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class ServiceFact : IServiceFact
    {
        IRFFACDETRepository dRFFACDET = new RFFACDETRepository();
        IRFFACCABRepository dRFFACCAB = new RFFACCABRepository();
        IRCORepository dRCO = new RCORepository();
        IRFPARAMRepository dRFPARAM = new RFPARAMRepository();
        IZCCRepository dZCC = new ZCCRepository();
        List<RFFACDET> lmRFFACDET = new List<RFFACDET>();
        List<RFPARAM> lmRFPARAM = new List<RFPARAM>();
        List<ZCC> lmZCC = new List<ZCC>();
        RCO mRCO = new RCO();
        RFFACCAB mRFFACCAB = new RFFACCAB();
        RFPARAM mRFPARAM = new RFPARAM();

        private List<ENC> CabeceraENC(RFFACCAB model)
        {
            ENC mENC = new ENC();
            List<ENC> lmENC = new List<ENC>();
            string NombreXml = "";

            mENC.ENC_1 = "INVOIC";
            mENC.ENC_4 = lmRFPARAM.Where(m => m.CCCODE == "ENC_4").SingleOrDefault().CCDESC;
            mENC.ENC_5 = lmRFPARAM.Where(m => m.CCCODE == "ENC_5").SingleOrDefault().CCDESC;

            NombreXml = model.FPREFIJ + model.FFACTUR;
            mRFPARAM = lmRFPARAM.Where(m => m.CCCODE == "PREFIJO" && m.CCCODE2 == model.FPREFIJ).SingleOrDefault();
            if (mRFPARAM != null)
            {
                if (mRFPARAM.CCCODEN != "0")
                {
                    NombreXml = mRFPARAM.CCALTC + (double.Parse(mRFPARAM.CCCODEN) + double.Parse(model.FFACTUR));
                }
            }
            mENC.ENC_6 = NombreXml;

            mENC.ENC_7 = model.FFECFAC;
            mENC.ENC_8 = model.FACRTDAT.ToString("HH:mm:ss");
            mENC.ENC_9 = (model.FORDCOM.StartsWith("EXP")) ? "2" : "1";
            mENC.ENC_10 = model.FMONEDA;
            mENC.ENC_13 = model.FCLIENT;
            mENC.ENC_15 = model.TotLineas;
            mENC.ENC_16 = model.FFECVEN;

            lmENC.Add(mENC);

            return lmENC;
        }

        private List<EMI> CabeceraEMI(RFFACCAB model)
        {
            EMI mEMI = new EMI();
            List<EMI> lmEMI = new List<EMI>();

            ICC mICC = new ICC();
            List<ICC> lmICC = new List<ICC>();

            TAC mTAC;
            List<TAC> lmTAC = new List<TAC>();

            mEMI.EMI_1 = mRCO.COEXID;
            mEMI.EMI_2 = mRCO.CVATNM;
            mEMI.EMI_3 = mRCO.COAUTN;
            mEMI.EMI_4 = mRCO.COAUTB;
            mEMI.EMI_6 = mRCO.CMPNAM;
            mEMI.EMI_10 = mRCO.CMPAD1;
            mEMI.EMI_11 = mRCO.COSTE;
            mEMI.EMI_12 = mRCO.COADR3;
            mEMI.EMI_13 = mRCO.CMPOST;
            mEMI.EMI_15 = mRCO.COCRCC;
            mEMI.EMI_18 = mRCO.CMPAD1;

            mICC.ICC_1 = mRCO.COCRNO;
            mICC.ICC_3 = mRCO.CMPOST;
            mICC.ICC_5 = mRCO.COSTE;
            mICC.ICC_6 = mRCO.CMPAD1;
            mICC.ICC_7 = mRCO.COCRCC;
            mICC.ICC_8 = mRCO.COADR4;
            lmICC.Add(mICC);
            mEMI.LMdlICC = lmICC;

            lmZCC = dZCC.GetTablesRut();
            foreach (ZCC item in lmZCC)
            {
                mTAC = new TAC()
                {
                    TAC_1 = item.CCCODE
                };
                lmTAC.Add(mTAC);
            }
            mEMI.LMdlTAC = lmTAC;

            lmEMI.Add(mEMI);

            return lmEMI;
        }

        private List<ADQ> CabeceraADQ(RFFACCAB model)
        {
            ADQ mADQ = new ADQ();
            List<ADQ> lmADQ = new List<ADQ>();
            ICR mICR = new ICR();
            List<ICR> lmICR = new List<ICR>();

            mADQ.ADQ_1 = model.SUFD13;
            mADQ.ADQ_2 = (model.SUFD19 != "") ? model.SUFD19 : model.FNIT;
            mADQ.ADQ_3 = model.SUFD17;
            mADQ.ADQ_4 = model.SUFD14;
            mADQ.ADQ_5 = model.FCLIENT;
            mADQ.ADQ_6 = (model.SUFD13 != "2") ? model.FNOMCLI : "";
            mADQ.ADQ_8 = (model.SUFD13 == "2") ? model.SUFD06 : "";
            mADQ.ADQ_9 = (model.SUFD13 == "2") ? model.SUFD05 : "";
            mADQ.ADQ_10 = model.FDIRCLI1;
            mADQ.ADQ_11 = model.FDEPCLI;
            mADQ.ADQ_13 = model.FDIRCLI3;
            mADQ.ADQ_15 = model.FPAICLI;
            mADQ.ADQ_18 = model.FDIRCLI1;

            mICR.ICR_3 = model.FDIRCLI3;
            mICR.ICR_5 = model.FDEPCLI;
            mICR.ICR_6 = model.FDIRCLI1;
            mICR.ICR_7 = model.FPAICLI;
            lmICR.Add(mICR);
            mADQ.LMdlICR = lmICR;

            lmADQ.Add(mADQ);

            return lmADQ;
        }

        private List<TOT> CabeceraTOT(RFFACCAB model)
        {
            TOT mTOT = new TOT();
            List<TOT> lmTOT = new List<TOT>();

            mTOT.TOT_1 = model.FSUBTOT;
            mTOT.TOT_2 = model.FMONEDA;
            mTOT.TOT_3 = model.FSUBTOT;
            mTOT.TOT_4 = model.FMONEDA;
            mTOT.TOT_5 = model.FTOTFAC;
            mTOT.TOT_6 = model.FMONEDA;
            lmTOT.Add(mTOT);

            return lmTOT;
        }

        private List<TIM> CabeceraTIM(RFFACCAB model)
        {
            TIM mTIM = new TIM();
            List<TIM> lmTIM = new List<TIM>();

            if (model.FIMPUES != null)
            {
                mTIM.TIM_1 = "false";
                mTIM.TIM_2 = model.FIMPUES;
                mTIM.TIM_3 = model.FMONEDA;
                lmTIM.Add(mTIM);
            }

            return lmTIM;
        }

        private List<TDC> CabeceraTDC(RFFACCAB model)
        {
            List<TDC> lmTDC = new List<TDC>();
            TDC mTDC = new TDC
            {
                TDC_1 = model.FMONEDA,
                TDC_2 = model.FMONEDA
            };
            lmTDC.Add(mTDC);

            return lmTDC;
        }

        private List<ORC> CabeceraORC(RFFACCAB model)
        {
            ORC mORC = new ORC();
            List<ORC> lmORC = new List<ORC>();

            mORC.ORC_1 = model.FORDCOM;
            lmORC.Add(mORC);

            return lmORC;
        }

        private List<REF> CabeceraREF(RFFACCAB model)
        {
            REF mREF;
            List<REF> lmREF = new List<REF>();

            mREF = new REF()
            {
                REF_1 = "RF1",
                REF_2 = model.FPEDIDO
            };
            lmREF.Add(mREF);

            mREF = new REF()
            {
                REF_1 = "RF2",
                REF_2 = model.FPLANIL
            };
            lmREF.Add(mREF);

            return lmREF;
        }

        private List<IEN> CabeceraIEN(RFFACCAB model)
        {
            IEN mIEN = new IEN();
            List<IEN> lmIEN = new List<IEN>();

            mIEN.IEN_1 = model.FDIRPEN1;
            mIEN.IEN_2 = model.FDEPPEN;
            mIEN.IEN_4 = model.FDIRPEN3;
            mIEN.IEN_6 = model.FPAIPEN;
            mIEN.IEN_7 = model.FNOMPEN;
            lmIEN.Add(mIEN);

            return lmIEN;
        }

        private List<ITE> DetalleITE(RFFACCAB modelC, List<RFFACDET> modelD)
        {
            ITE mITE = new ITE();
            List<ITE> lmITE = new List<ITE>();

            TII mTII = new TII();
            List<TII> lmTII = new List<TII>();

            IIM mIIM = new IIM();
            List<IIM> lmIIM = new List<IIM>();

            foreach (RFFACDET Det in modelD)
            {
                mITE = new ITE
                {
                    ITE_1 = Det.DLINEA,
                    ITE_2 = (Det.DVALUNI == 0) ? "true" : "false",
                    ITE_3 = Det.DCANTID,
                    ITE_4 = Det.DUNIMED,//
                    ITE_5 = Det.DVALTOT,
                    ITE_6 = modelC.FMONEDA,
                    ITE_7 = Det.DVALUNI,
                    ITE_8 = modelC.FMONEDA,
                    ITE_18 = Det.DCODPRO,
                    ITE_10 = Det.DUNIMED,
                    ITE_11 = Det.DDESPRO,
                    ITE_19 = Det.DVALTOT,
                    ITE_20 = modelC.FMONEDA
                };

                lmIIM.Clear();
                lmTII.Clear();

                mIIM = new IIM()
                {
                    IIM_1 = "01",
                    IIM_2 = modelC.FIMPUES,
                    IIM_3 = modelC.FMONEDA,
                    IIM_4 = modelC.FSUBTOT,
                    IIM_5 = modelC.FMONEDA
                };
                lmIIM.Add(mIIM);

                mTII = new TII()
                {
                    LMdlIIM = lmIIM
                };

                lmTII.Add(mTII);

                mITE.LMdlTII = lmTII;

                lmITE.Add(mITE);
            }

            return lmITE;
        }

        private List<NOT> DetalleNOT()
        {
            List<NOT> lmNOT = new List<NOT>();

            foreach (var item in lmRFPARAM.Where(m => m.CCSDSC == "NOTAS" && m.CCCODE == "FV").ToList())
            {
                NOT mNOT = new NOT()
                {
                    NOT_1 = item.CCCODEN + ".-" + item.CCNOTE
                };
                lmNOT.Add(mNOT);
            }
            return lmNOT;
        }

        private List<DRF> DetalleDRF()
        {
            List<DRF> lmDRF = new List<DRF>();
            DRF mDRF;
            if (mRFPARAM != null)
            {
                mDRF = new DRF()
                {
                    DRF_1 = mRFPARAM.CCCODE3,
                    DRF_2 = mRFPARAM.CCCODE4,
                    DRF_4 = mRFPARAM.CCALTC,
                    DRF_5 = mRFPARAM.CCCODEN,
                    DRF_6 = mRFPARAM.CCCODEN2
                };
                lmDRF.Add(mDRF);
            }

            return lmDRF;
        }

        public UploadResult CargarFactura(string Prefijo, string Factura)
        {
            //var re = dRFPARAM.MontoEscrito("132561,327");

            UploadResult ur = new UploadResult();
            ErrorDetail ed = new ErrorDetail();
            string NombreXml = "";
            Prefijo = Prefijo ?? "A";
            Factura = Factura ?? "1058239";
            mRFFACCAB = dRFFACCAB.Find(Prefijo, Factura);
            lmRFFACDET = dRFFACDET.Find(Prefijo, Factura);

            mRCO = dRCO.GetCompany();
            lmRFPARAM = dRFPARAM.GetParams();
            NombreXml = Prefijo + Factura;
            mRFPARAM = lmRFPARAM.Where(m => m.CCCODE == "PREFIJO" && m.CCCODE2 == Prefijo).SingleOrDefault();
            if (mRFPARAM != null)
            {
                if (mRFPARAM.CCCODEN != "0")
                {
                    NombreXml = mRFPARAM.CCALTC + (double.Parse(mRFPARAM.CCCODEN) + double.Parse(Factura));
                }
            }

            FACTURA mFACTURA = new FACTURA
            {
                LMdlENC = CabeceraENC(mRFFACCAB),
                LMdlEMI = CabeceraEMI(mRFFACCAB),
                LMdlADQ = CabeceraADQ(mRFFACCAB),
                LMdlTOT = CabeceraTOT(mRFFACCAB),
                LMdlTIM = CabeceraTIM(mRFFACCAB),
                LMdlTDC = CabeceraTDC(mRFFACCAB),
                LMdlDRF = DetalleDRF(),
                LMdlNOT = DetalleNOT(),
                LMdlORC = CabeceraORC(mRFFACCAB),
                LMdlREF = CabeceraREF(mRFFACCAB),
                LMdlIEN = CabeceraIEN(mRFFACCAB),
                LMdlITE = DetalleITE(mRFFACCAB, lmRFFACDET)
            };

            var xml = "";
            XmlSerializer xs = new XmlSerializer(typeof(FACTURA));
            using (var sww = new StringWriter())
            {
                using (XmlWriter xwriter = XmlWriter.Create(sww))
                {
                    xs.Serialize(xwriter, mFACTURA);
                    xml = sww.ToString();
                }
            }

            TextWriter writer = new StreamWriter("C:\\Copia_Servidor\\Proyectos\\BRINSA\\BssFacturaElectronicaService\\Prueba.xml");
            xs.Serialize(writer, mFACTURA);
            writer.Close();

            string Cargue = "";
            string url = lmRFPARAM.Where(m => m.CCCODE == "WSDL").SingleOrDefault().CCNOT1 + lmRFPARAM.Where(m => m.CCCODE == "WSDL").SingleOrDefault().CCNOT2;
            string credid = lmRFPARAM.Where(m => m.CCCODE == "WSDLUSER").SingleOrDefault().CCNOT1;
            string credpassword = Sha256(lmRFPARAM.Where(m => m.CCCODE == "WSDLPASS").SingleOrDefault().CCNOT1);

            StringBuilder rawSOAP = new StringBuilder();
            //rawSOAP.Append(@"<soapenv:Envelope xmlns:inv=""http://invoice.carvajal.com/invoiceService/"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">");
            rawSOAP.Append(BuildSoapHeader(credid, credpassword));
            rawSOAP.Append(@"<soapenv:Body><inv:UploadRequest>");
            rawSOAP.AppendFormat(@"<fileName>{0}.xml</fileName>", NombreXml);
            rawSOAP.AppendFormat(@"<fileData>{0}</fileData>", Convert.ToBase64String(Encoding.UTF8.GetBytes(xml)));
            rawSOAP.AppendFormat(@"<companyId>{0}</companyId>", lmRFPARAM.Where(m => m.CCCODE == "COMPANYID").SingleOrDefault().CCNOT1);
            rawSOAP.AppendFormat(@"<accountId>{0}</accountId>", lmRFPARAM.Where(m => m.CCCODE == "ACCOUNTID").SingleOrDefault().CCNOT1);
            rawSOAP.Append(@"</inv:UploadRequest></soapenv:Body></soapenv:Envelope>");
            string SOAPObj = rawSOAP.ToString();

            using (var wb = new WebClient())
            {
                wb.Credentials = new NetworkCredential(credid, credpassword);
                try
                {
                    Cargue = wb.UploadString(url, "POST", SOAPObj);

                    ur.Status = Cargue.Substring((Cargue.IndexOf("<status>") + 8), (Cargue.IndexOf("</status>") - Cargue.IndexOf("<status>")) - 8);
                    ur.TransactionId = Cargue.Substring((Cargue.IndexOf("<transactionId>") + 15), (Cargue.IndexOf("</transactionId>") - Cargue.IndexOf("<transactionId>")) - 15);
                }
                catch (SoapException ex)
                {
                    Cargue = "ERROR: " + ex.ToString();
                }
                catch (FaultException ex)
                {
                    Cargue = "ERROR: " + ex.ToString();
                }
                catch (WebException ex)
                {
                    WebResponse errRsp = ex.Response;
                    using (StreamReader rdr = new StreamReader(errRsp.GetResponseStream()))
                    {
                        Cargue = "ERROR: " + rdr.ReadToEnd();
                        Console.WriteLine(rdr.ReadToEnd());
                    }

                    ed.StatusCode = Cargue.Substring((Cargue.IndexOf("<statusCode>") + 12), (Cargue.IndexOf("</statusCode>") - Cargue.IndexOf("<statusCode>")) - 12);
                    ed.ReasonPhrase = Cargue.Substring((Cargue.IndexOf("<reasonPhrase>") + 14), (Cargue.IndexOf("</reasonPhrase>") - Cargue.IndexOf("<reasonPhrase>")) - 14);
                    ed.ErrorMessage = Cargue.Substring((Cargue.IndexOf("<errorMessage>") + 14), (Cargue.IndexOf("</errorMessage>") - Cargue.IndexOf("<errorMessage>")) - 14);

                    ur.Faultcode = Cargue.Substring((Cargue.IndexOf("<faultcode>") + 11), (Cargue.IndexOf("</faultcode>") - Cargue.IndexOf("<faultcode>")) - 11);
                    ur.Faultstring = Cargue.Substring((Cargue.IndexOf("<faultstring>") + 13), (Cargue.IndexOf("</faultstring>") - Cargue.IndexOf("<faultstring>")) - 13);
                    ur.Detail = ed;
                }
                catch (Exception ex)
                {
                    Cargue = "ERROR: " + ex.ToString();
                }
            }

            return ur;
        }

        public string DescargarFactura(string Prefijo, string Factura)
        {
            lmRFPARAM = dRFPARAM.GetParams();
            string NombreXml = "", pref = "";
            string descarga = "";
            string url = lmRFPARAM.Where(m => m.CCCODE == "WSDL").SingleOrDefault().CCNOT1 + lmRFPARAM.Where(m => m.CCCODE == "WSDL").SingleOrDefault().CCNOT2;
            string credid = lmRFPARAM.Where(m => m.CCCODE == "WSDLUSER").SingleOrDefault().CCNOT1;
            string credpassword = Sha256(lmRFPARAM.Where(m => m.CCCODE == "WSDLPASS").SingleOrDefault().CCNOT1);

            Prefijo = Prefijo ?? "A";
            Factura = Factura ?? "1058239";

            //Prefijo + 
            NombreXml = Factura;
            pref = Prefijo;
            mRFPARAM = lmRFPARAM.Where(m => m.CCCODE == "PREFIJO" && m.CCCODE2 == Prefijo).SingleOrDefault();
            if (mRFPARAM != null)
            {
                if (mRFPARAM.CCCODEN != "0")
                {
                    pref = mRFPARAM.CCALTC;
                    NombreXml = (double.Parse(mRFPARAM.CCCODEN) + double.Parse(Factura)).ToString();
                }
            }

            StringBuilder rawSOAP = new StringBuilder();
            rawSOAP.Append(@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:inv=""http://invoice.carvajal.com/invoiceService/"">");
            rawSOAP.Append(BuildSoapHeader(credid, credpassword));
            rawSOAP.Append(@"<soapenv:Body><inv:DownloadRequest>");
            rawSOAP.Append(@"<documentType>FV</documentType>");
            rawSOAP.AppendFormat(@"<documentNumber>{0}</documentNumber>", NombreXml);
            rawSOAP.AppendFormat(@"<documentPrefix>{0}</documentPrefix>", pref);
            rawSOAP.Append(@"<resourceType>PDF</resourceType>");
            rawSOAP.AppendFormat(@"<companyId>{0}</companyId>", lmRFPARAM.Where(m => m.CCCODE == "COMPANYID").SingleOrDefault().CCNOT1);
            rawSOAP.AppendFormat(@"<accountId>{0}</accountId>", lmRFPARAM.Where(m => m.CCCODE == "ACCOUNTID").SingleOrDefault().CCNOT1);
            rawSOAP.Append(@"</inv:DownloadRequest></soapenv:Body></soapenv:Envelope>");
            string SOAPObj = rawSOAP.ToString();
            using (var wb = new WebClient())
            {
                wb.Credentials = new NetworkCredential(credid, credpassword);
                try
                {
                    descarga = wb.UploadString(url, "POST", SOAPObj);
                    //FDServ.UploadResponse rest = Newtonsoft.Json.JsonConvert.DeserializeObject<FDServ.UploadResponse>(responseVal);
                }
                catch (WebException ex)
                {
                    WebResponse errRsp = ex.Response;
                    using (StreamReader rdr = new StreamReader(errRsp.GetResponseStream()))
                    {
                        descarga = "ERROR: " + rdr.ReadToEnd();
                        Console.WriteLine(rdr.ReadToEnd());
                    }

                    //ed.StatusCode = Estado.Substring((Estado.IndexOf("<statusCode>") + 12), (Estado.IndexOf("</statusCode>") - Estado.IndexOf("<statusCode>")) - 12);
                    //ed.ReasonPhrase = Estado.Substring((Estado.IndexOf("<reasonPhrase>") + 14), (Estado.IndexOf("</reasonPhrase>") - Estado.IndexOf("<reasonPhrase>")) - 14);
                    //ed.ErrorMessage = Estado.Substring((Estado.IndexOf("<errorMessage>") + 14), (Estado.IndexOf("</errorMessage>") - Estado.IndexOf("<errorMessage>")) - 14);
                    //
                    //sr.Faultcode = Estado.Substring((Estado.IndexOf("<faultcode>") + 11), (Estado.IndexOf("</faultcode>") - Estado.IndexOf("<faultcode>")) - 11);
                    //sr.Faultstring = Estado.Substring((Estado.IndexOf("<faultstring>") + 13), (Estado.IndexOf("</faultstring>") - Estado.IndexOf("<faultstring>")) - 13);
                    //sr.Detail = ed;
                }
                catch (Exception ex)
                {
                    descarga = "ERROR: " + ex.ToString();
                }
            }
            return descarga;
        }

        public StatusResult EstadoFactura(string TransId = "")
        {
            StatusResult sr = new StatusResult();
            ErrorDetail ed = new ErrorDetail();
            lmRFPARAM = dRFPARAM.GetParams();
            string Estado = "";
            string url = lmRFPARAM.Where(m => m.CCCODE == "WSDL").SingleOrDefault().CCNOT1 + lmRFPARAM.Where(m => m.CCCODE == "WSDL").SingleOrDefault().CCNOT2;
            string credid = lmRFPARAM.Where(m => m.CCCODE == "WSDLUSER").SingleOrDefault().CCNOT1;
            string credpassword = Sha256(lmRFPARAM.Where(m => m.CCCODE == "WSDLPASS").SingleOrDefault().CCNOT1);

            StringBuilder rawSOAP = new StringBuilder();
            //rawSOAP.Append(@"<soapenv:Envelope xmlns:inv=""http://invoice.carvajal.com/invoiceService/"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">");
            rawSOAP.Append(BuildSoapHeader(credid, credpassword));
            rawSOAP.Append(@"<soapenv:Body><inv:DocumentStatusRequest>");
            rawSOAP.AppendFormat(@"<transactionId>{0}</transactionId>", TransId);
            rawSOAP.AppendFormat(@"<companyId>{0}</companyId>", lmRFPARAM.Where(m => m.CCCODE == "COMPANYID").SingleOrDefault().CCNOT1);
            rawSOAP.AppendFormat(@"<accountId>{0}</accountId>", lmRFPARAM.Where(m => m.CCCODE == "ACCOUNTID").SingleOrDefault().CCNOT1);
            rawSOAP.Append(@"</inv:DocumentStatusRequest></soapenv:Body></soapenv:Envelope>");
            string SOAPObj = rawSOAP.ToString();
            using (var wb = new WebClient())
            {
                wb.Credentials = new NetworkCredential(credid, credpassword);
                try
                {
                    Estado = wb.UploadString(url, "POST", SOAPObj);

                    sr.ProcessName = Estado.Substring((Estado.IndexOf("<processName>") + 13), (Estado.IndexOf("</processName>") - Estado.IndexOf("<processName>")) - 13);
                    sr.ProcessStatus = Estado.Substring((Estado.IndexOf("<processStatus>") + 15), (Estado.IndexOf("</processStatus>") - Estado.IndexOf("<processStatus>")) - 15);
                    sr.ProcessDate = Estado.Substring((Estado.IndexOf("<processDate>") + 13), (Estado.IndexOf("</processDate>") - Estado.IndexOf("<processDate>")) - 13);
                    sr.MessageType = Estado.Substring((Estado.IndexOf("<messageType>") + 13), (Estado.IndexOf("</messageType>") - Estado.IndexOf("<messageType>")) - 13);
                    sr.ErrorMessage = Estado.Substring((Estado.IndexOf("<errorMessage>") + 14), (Estado.IndexOf("</errorMessage>") - Estado.IndexOf("<errorMessage>")) - 14);
                }
                catch (WebException ex)
                {
                    WebResponse errRsp = ex.Response;
                    using (StreamReader rdr = new StreamReader(errRsp.GetResponseStream()))
                    {
                        Estado = "ERROR: " + rdr.ReadToEnd();
                        Console.WriteLine(rdr.ReadToEnd());
                    }

                    ed.StatusCode = Estado.Substring((Estado.IndexOf("<statusCode>") + 12), (Estado.IndexOf("</statusCode>") - Estado.IndexOf("<statusCode>")) - 12);
                    ed.ReasonPhrase = Estado.Substring((Estado.IndexOf("<reasonPhrase>") + 14), (Estado.IndexOf("</reasonPhrase>") - Estado.IndexOf("<reasonPhrase>")) - 14);
                    ed.ErrorMessage = Estado.Substring((Estado.IndexOf("<errorMessage>") + 14), (Estado.IndexOf("</errorMessage>") - Estado.IndexOf("<errorMessage>")) - 14);

                    sr.Faultcode = Estado.Substring((Estado.IndexOf("<faultcode>") + 11), (Estado.IndexOf("</faultcode>") - Estado.IndexOf("<faultcode>")) - 11);
                    sr.Faultstring = Estado.Substring((Estado.IndexOf("<faultstring>") + 13), (Estado.IndexOf("</faultstring>") - Estado.IndexOf("<faultstring>")) - 13);
                    sr.Detail = ed;
                }
                catch (Exception ex)
                {
                    Estado = "ERROR: " + ex.ToString();
                }
            }
            return sr;
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
            rawSOAP.Append(@"<soapenv:Envelope xmlns:inv=""http://invoice.carvajal.com/invoiceService/"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:wsse=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"" xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">");
            rawSOAP.Append(@"<soapenv:Header>");
            rawSOAP.Append(@"<wsse:Security>");
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

        private string Xmlfac()
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
            s.AppendLine(" <FACTURA>");
            s.AppendLine("     <ENC>");
            s.AppendLine("         <ENC_1>INVOIC</ENC_1>");
            s.AppendLine("         <ENC_2>890105927</ENC_2>");
            s.AppendLine("         <ENC_3>65-0462207</ENC_3>");
            s.AppendLine("         <ENC_4>UBL 2.0</ENC_4>");
            s.AppendLine("         <ENC_5>DIAN 1.0</ENC_5>");
            s.AppendLine("         <ENC_6>EX11617</ENC_6>");
            s.AppendLine("         <ENC_7>2018-07-02</ENC_7>");
            s.AppendLine("         <ENC_8>23:10:01</ENC_8>");
            s.AppendLine("         <ENC_9>1</ENC_9>");
            s.AppendLine("         <ENC_10>USD</ENC_10>");
            s.AppendLine("         <ENC_11></ENC_11>");
            s.AppendLine("         <ENC_12></ENC_12>");
            s.AppendLine("         <ENC_13></ENC_13>");
            s.AppendLine("         <ENC_14></ENC_14>");
            s.AppendLine("         <ENC_15></ENC_15>");
            s.AppendLine("         <ENC_16>2018-12-29</ENC_16>");
            s.AppendLine("         <ENC_17></ENC_17>");
            s.AppendLine("         <ENC_18></ENC_18>");
            s.AppendLine("     </ENC>");
            s.AppendLine("     <EMI>");
            s.AppendLine("         <EMI_1>3</EMI_1>");
            s.AppendLine("         <EMI_2>890105927</EMI_2>");
            s.AppendLine("         <EMI_3>31</EMI_3>");
            s.AppendLine("         <EMI_4>2</EMI_4>");
            s.AppendLine("         <EMI_5>10</EMI_5>");
            s.AppendLine("         <EMI_6>C.I. FARMACAPSULAS S.A.</EMI_6>");
            s.AppendLine("         <EMI_7>C.I. FARMACAPSULAS S.A.</EMI_7>");
            s.AppendLine("         <EMI_8></EMI_8>");
            s.AppendLine("         <EMI_9></EMI_9>");
            s.AppendLine("         <EMI_10>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_10>");
            s.AppendLine("         <EMI_11></EMI_11>");
            s.AppendLine("         <EMI_12>BARRANQUILLA</EMI_12>");
            s.AppendLine("         <EMI_13>BARRANQUILLA</EMI_13>");
            s.AppendLine("         <EMI_14></EMI_14>");
            s.AppendLine("         <EMI_15>CO</EMI_15>");
            s.AppendLine("         <EMI_16></EMI_16>");
            s.AppendLine("         <EMI_17></EMI_17>");
            s.AppendLine("         <EMI_18>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_18>");
            s.AppendLine("         <EMI_19></EMI_19>");
            s.AppendLine("         <EMI_20></EMI_20>");
            s.AppendLine("         <EMI_21></EMI_21>");
            s.AppendLine("         <ICC>");
            s.AppendLine("             <ICC_1></ICC_1>");
            s.AppendLine("             <ICC_2></ICC_2>");
            s.AppendLine("             <ICC_3></ICC_3>");
            s.AppendLine("             <ICC_4></ICC_4>");
            s.AppendLine("             <ICC_5></ICC_5>");
            s.AppendLine("             <ICC_6></ICC_6>");
            s.AppendLine("             <ICC_7></ICC_7>");
            s.AppendLine("             <ICC_8></ICC_8>");
            s.AppendLine("         </ICC>");
            s.AppendLine("     </EMI>");
            s.AppendLine("     <ADQ>");
            s.AppendLine("         <ADQ_1>-1</ADQ_1>");
            s.AppendLine("         <ADQ_2>65-0462207     </ADQ_2>");
            s.AppendLine("         <ADQ_3>31</ADQ_3>");
            s.AppendLine("         <ADQ_4>-1</ADQ_4>");
            s.AppendLine("         <ADQ_5>1010</ADQ_5>");
            s.AppendLine("         <ADQ_6></ADQ_6>");
            s.AppendLine("         <ADQ_7></ADQ_7>");
            s.AppendLine("         <ADQ_8></ADQ_8>");
            s.AppendLine("         <ADQ_9></ADQ_9>");
            s.AppendLine("         <ADQ_10>1893 SW 3RD STREET</ADQ_10>");
            s.AppendLine("         <ADQ_11></ADQ_11>");
            s.AppendLine("         <ADQ_12></ADQ_12>");
            s.AppendLine("         <ADQ_13></ADQ_13>");
            s.AppendLine("         <ADQ_14></ADQ_14>");
            s.AppendLine("         <ADQ_15>US</ADQ_15>");
            s.AppendLine("         <ADQ_16></ADQ_16>");
            s.AppendLine("         <ADQ_17></ADQ_17>");
            s.AppendLine("         <ADQ_18>1893 SW 3RD STREET</ADQ_18>");
            s.AppendLine("         <ADQ_19></ADQ_19>");
            s.AppendLine("         <ADQ_20></ADQ_20>");
            s.AppendLine("         <ADQ_21></ADQ_21>");
            s.AppendLine("         <TCR>");
            s.AppendLine("             <TCR_1>O-99</TCR_1>");
            s.AppendLine("             <TCR_2></TCR_2>");
            s.AppendLine("             <TCR_3></TCR_3>");
            s.AppendLine("             <TCR_4></TCR_4>");
            s.AppendLine("             <TCR_5></TCR_5>");
            s.AppendLine("             <TCR_6></TCR_6>");
            s.AppendLine("             <TCR_7></TCR_7>");
            s.AppendLine("             <TCR_8></TCR_8>");
            s.AppendLine("             <TCR_9></TCR_9>");
            s.AppendLine("             <TCR_10></TCR_10>");
            s.AppendLine("             <TCR_11></TCR_11>");
            s.AppendLine("         </TCR>");
            s.AppendLine("         <ICR>");
            s.AppendLine("             <ICR_1></ICR_1>");
            s.AppendLine("             <ICR_2></ICR_2>");
            s.AppendLine("             <ICR_3></ICR_3>");
            s.AppendLine("             <ICR_4></ICR_4>");
            s.AppendLine("             <ICR_5></ICR_5>");
            s.AppendLine("             <ICR_6></ICR_6>");
            s.AppendLine("             <ICR_7></ICR_7>");
            s.AppendLine("             <ICR_8></ICR_8>");
            s.AppendLine("         </ICR>");
            s.AppendLine("         <CDA>");
            s.AppendLine("             <CDA_1></CDA_1>");
            s.AppendLine("             <CDA_2></CDA_2>");
            s.AppendLine("             <CDA_3></CDA_3>");
            s.AppendLine("             <CDA_4></CDA_4>");
            s.AppendLine("         </CDA>");
            s.AppendLine("     </ADQ>");
            s.AppendLine("     <TOT>");
            s.AppendLine("         <TOT_1>4546000</TOT_1>");
            s.AppendLine("         <TOT_2>USD</TOT_2>");
            s.AppendLine("         <TOT_3>4546000</TOT_3>");
            s.AppendLine("         <TOT_4>USD</TOT_4>");
            s.AppendLine("         <TOT_5>4546000</TOT_5>");
            s.AppendLine("         <TOT_6>USD</TOT_6>");
            s.AppendLine("         <TOT_7>4546000</TOT_7>");
            s.AppendLine("         <TOT_8>USD</TOT_8>");
            s.AppendLine("         <TOT_9>4546000</TOT_9>");
            s.AppendLine("         <TOT_10>USD</TOT_10>");
            s.AppendLine("         <TOT_12>USD</TOT_12>");
            s.AppendLine("     </TOT>");
            s.AppendLine("     <TDC/>");
            s.AppendLine("     <OTC/>");
            s.AppendLine("     <DSC>");
            s.AppendLine("         <DSC_1>false</DSC_1>");
            s.AppendLine("         <DSC_2>1</DSC_2>");
            s.AppendLine("         <DSC_3>801.05</DSC_3>");
            s.AppendLine("         <DSC_4>USD</DSC_4>");
            s.AppendLine("         <DSC_5>NO ENCONTRO FLETES / FREIGHT</DSC_5>");
            s.AppendLine("     </DSC>");
            s.AppendLine("     <DSC>");
            s.AppendLine("         <DSC_1>false</DSC_1>");
            s.AppendLine("         <DSC_2>1</DSC_2>");
            s.AppendLine("         <DSC_3>45</DSC_3>");
            s.AppendLine("         <DSC_4>USD</DSC_4>");
            s.AppendLine("         <DSC_5>NO ENCONTRO SEGURO / INSURANCE</DSC_5>");
            s.AppendLine("     </DSC>");
            s.AppendLine("     <DRF>");
            s.AppendLine("         <DRF_1>18762007201018                                                                                                </DRF_1>");
            s.AppendLine("         <DRF_2>2018-03-05</DRF_2>");
            s.AppendLine("         <DRF_3>2020-03-04</DRF_3>");
            s.AppendLine("         <DRF_4>EX</DRF_4>");
            s.AppendLine("         <DRF_5>11317</DRF_5>");
            s.AppendLine("         <DRF_6>14000</DRF_6>");
            s.AppendLine("     </DRF>");
            s.AppendLine("     <NOT>");
            s.AppendLine("         <NOT_1>1.-SOMOS GRANDES CONTRIBUYENTES RESOLUCIÃ“N DIAN 0000076 DE 2016/12/01</NOT_1>");
            s.AppendLine("     </NOT>");
            s.AppendLine("     <NOT>");
            s.AppendLine("         <NOT_1>8.-39.23.50.90.00</NOT_1>");
            s.AppendLine("     </NOT>");
            s.AppendLine("     <NOT>");
            s.AppendLine("         <NOT_1>9.-MP-0075</NOT_1>");
            s.AppendLine("     </NOT>");
            s.AppendLine("     <NOT>");
            s.AppendLine("         <NOT_1>10.-39</NOT_1>");
            s.AppendLine("     </NOT>");
            s.AppendLine("     <NOT>");
            s.AppendLine("         <NOT_1>11.-73,284%</NOT_1>");
            s.AppendLine("     </NOT>");
            s.AppendLine("     <NOT>");
            s.AppendLine("         <NOT_1>12.-NET 180        </NOT_1>");
            s.AppendLine("     </NOT>");
            s.AppendLine("     <REF>");
            s.AppendLine("         <REF_1>AAJ</REF_1>");
            s.AppendLine("         <REF_2>279107</REF_2>");
            s.AppendLine("     </REF>");
            s.AppendLine("     <REF>");
            s.AppendLine("         <REF_1>AAJ</REF_1>");
            s.AppendLine("         <REF_2>279108</REF_2>");
            s.AppendLine("     </REF>");
            s.AppendLine("     <REF>");
            s.AppendLine("         <REF_1>AAJ</REF_1>");
            s.AppendLine("         <REF_2>279109</REF_2>");
            s.AppendLine("     </REF>");
            s.AppendLine("     <IEN>");
            s.AppendLine("         <IEN_1>1893 SW 3RD STREET</IEN_1>");
            s.AppendLine("         <IEN_6>US</IEN_6>");
            s.AppendLine("     </IEN>");
            s.AppendLine("     <ITE>");
            s.AppendLine("         <ITE_1>1</ITE_1>");
            s.AppendLine("         <ITE_2>false</ITE_2>");
            s.AppendLine("         <ITE_3>500000.000</ITE_3>");
            s.AppendLine("         <ITE_4>MIO</ITE_4>");
            s.AppendLine("         <ITE_5>1795000</ITE_5>");
            s.AppendLine("         <ITE_6>USD</ITE_6>");
            s.AppendLine("         <ITE_7>3.59</ITE_7>");
            s.AppendLine("         <ITE_8>USD</ITE_8>");
            s.AppendLine("         <ITE_9>K00003</ITE_9>");
            s.AppendLine("         <ITE_10>HARD EMPTY VEGETABLE CAPSULES|CAPSULAS  VGETALES DE DOS PIEZAS VACIAS|SIZE 0 1-0K/1-0K (NATURAL/NATURAL)</ITE_10>");
            s.AppendLine("         <ITE_11></ITE_11>");
            s.AppendLine("         <ITE_12></ITE_12>");
            s.AppendLine("         <TII>");
            s.AppendLine("             <TII_1>0</TII_1>");
            s.AppendLine("             <TII_2>USD</TII_2>");
            s.AppendLine("             <TII_3>false</TII_3>");
            s.AppendLine("         </TII>");
            s.AppendLine("     </ITE>");
            s.AppendLine("     <ITE>");
            s.AppendLine("         <ITE_1>2</ITE_1>");
            s.AppendLine("         <ITE_2>false</ITE_2>");
            s.AppendLine("         <ITE_3>400000.000</ITE_3>");
            s.AppendLine("         <ITE_4>MIO</ITE_4>");
            s.AppendLine("         <ITE_5>2304000</ITE_5>");
            s.AppendLine("         <ITE_6>USD</ITE_6>");
            s.AppendLine("         <ITE_7>5.76</ITE_7>");
            s.AppendLine("         <ITE_8>USD</ITE_8>");
            s.AppendLine("         <ITE_9>K00012</ITE_9>");
            s.AppendLine("         <ITE_10>HARD EMPTY VEGETABLE  CAPSULES|CAPSULAS  VEGETAL DE DOS PIEZAS VACIAS|SIZE/TAMAÑO 00 1-0K/1-0K (NATURAL/NATURAL)</ITE_10>");
            s.AppendLine("         <ITE_11></ITE_11>");
            s.AppendLine("         <ITE_12></ITE_12>");
            s.AppendLine("         <TII>");
            s.AppendLine("             <TII_1>0</TII_1>");
            s.AppendLine("             <TII_2>USD</TII_2>");
            s.AppendLine("             <TII_3>false</TII_3>");
            s.AppendLine("         </TII>");
            s.AppendLine("     </ITE>");
            s.AppendLine("     <ITE>");
            s.AppendLine("         <ITE_1>3</ITE_1>");
            s.AppendLine("         <ITE_2>false</ITE_2>");
            s.AppendLine("         <ITE_3>75000.000</ITE_3>");
            s.AppendLine("         <ITE_4>MIO</ITE_4>");
            s.AppendLine("         <ITE_5>447000</ITE_5>");
            s.AppendLine("         <ITE_6>USD</ITE_6>");
            s.AppendLine("         <ITE_7>5.96</ITE_7>");
            s.AppendLine("         <ITE_8>USD</ITE_8>");
            s.AppendLine("         <ITE_9>K00061</ITE_9>");
            s.AppendLine("         <ITE_10>HARD EMPTY TWO PIECES VEGETABLE CAPSULE|CAPSULAS VEGETALES DE DOS PIEZAS VACIAS|SIZE 00E 1-0K/1-0K (NATURAL/NATURAL)</ITE_10>");
            s.AppendLine("         <ITE_11>KC00E-1000</ITE_11>");
            s.AppendLine("         <ITE_12></ITE_12>");
            s.AppendLine("         <TII>");
            s.AppendLine("             <TII_1>0</TII_1>");
            s.AppendLine("             <TII_2>USD</TII_2>");
            s.AppendLine("             <TII_3>false</TII_3>");
            s.AppendLine("         </TII>");
            s.AppendLine("     </ITE>");
            s.AppendLine(" </FACTURA>");

            string x = s.ToString().Trim();

            string res = Convert.ToBase64String(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?> <FACTURA> 	<ENC> 		<ENC_1>INVOIC</ENC_1> 		<ENC_2>890105927</ENC_2> 		<ENC_3>65-0462207</ENC_3> 		<ENC_4>UBL 2.0</ENC_4> 		<ENC_5>DIAN 1.0</ENC_5> 		<ENC_6>EX11617</ENC_6> 		<ENC_7>2018-07-02</ENC_7> 		<ENC_8>23:10:01</ENC_8> 		<ENC_9>1</ENC_9> 		<ENC_10>USD</ENC_10> 		<ENC_11/> 		<ENC_12/> 		<ENC_13/> 		<ENC_14/> 		<ENC_15/> 		<ENC_16>2018-12-29</ENC_16> 		<ENC_17/> 		<ENC_18/> 	</ENC> 	<EMI> 		<EMI_1>3</EMI_1> 		<EMI_2>890105927</EMI_2> 		<EMI_3>31</EMI_3> 		<EMI_4>2</EMI_4> 		<EMI_5>10</EMI_5> 		<EMI_6>C.I. FARMACAPSULAS S.A.</EMI_6> 		<EMI_7>C.I. FARMACAPSULAS S.A.</EMI_7> 		<EMI_8/> 		<EMI_9/> 		<EMI_10>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_10> 		<EMI_11/> 		<EMI_12>BARRANQUILLA</EMI_12> 		<EMI_13>BARRANQUILLA</EMI_13> 		<EMI_14/> 		<EMI_15>CO</EMI_15> 		<EMI_16/> 		<EMI_17/> 		<EMI_18>CALLE 79B No 78C-21 APDO AEREO / CODE POSTAL 52486</EMI_18> 		<EMI_19/> 		<EMI_20/> 		<EMI_21/> 		<ICC> 			<ICC_1/> 			<ICC_2/> 			<ICC_3/> 			<ICC_4/> 			<ICC_5/> 			<ICC_6/> 			<ICC_7/> 			<ICC_8/> 		</ICC> 	</EMI> 	<ADQ> 		<ADQ_1>-1</ADQ_1> 		<ADQ_2>65-0462207 </ADQ_2> 		<ADQ_3>31</ADQ_3> 		<ADQ_4>-1</ADQ_4> 		<ADQ_5>1010</ADQ_5> 		<ADQ_6/> 		<ADQ_7/> 		<ADQ_8/> 		<ADQ_9/> 		<ADQ_10>1893 SW 3RD STREET</ADQ_10> 		<ADQ_11/> 		<ADQ_12/> 		<ADQ_13/> 		<ADQ_14/> 		<ADQ_15>US</ADQ_15> 		<ADQ_16/> 		<ADQ_17/> 		<ADQ_18>1893 SW 3RD STREET</ADQ_18> 		<ADQ_19/> 		<ADQ_20/> 		<ADQ_21/> 		<TCR> 			<TCR_1>O-99</TCR_1> 			<TCR_2/> 			<TCR_3/> 			<TCR_4/> 			<TCR_5/> 			<TCR_6/> 			<TCR_7/> 			<TCR_8/> 			<TCR_9/> 			<TCR_10/> 			<TCR_11/> 		</TCR> 		<ICR> 			<ICR_1/> 			<ICR_2/> 			<ICR_3/> 			<ICR_4/> 			<ICR_5/> 			<ICR_6/> 			<ICR_7/> 			<ICR_8/> 		</ICR> 		<CDA> 			<CDA_1/> 			<CDA_2/> 			<CDA_3/> 			<CDA_4/> 		</CDA> 	</ADQ> 	<TOT> 		<TOT_1>4546000</TOT_1> 		<TOT_2>USD</TOT_2> 		<TOT_3>4546000</TOT_3> 		<TOT_4>USD</TOT_4> 		<TOT_5>4546000</TOT_5> 		<TOT_6>USD</TOT_6> 		<TOT_7>4546000</TOT_7> 		<TOT_8>USD</TOT_8> 		<TOT_9>4546000</TOT_9> 		<TOT_10>USD</TOT_10> 		<TOT_12>USD</TOT_12> 	</TOT> 	<TDC/> 	<OTC/> 	<DSC> 		<DSC_1>false</DSC_1> 		<DSC_2>1</DSC_2> 		<DSC_3>801.05</DSC_3> 		<DSC_4>USD</DSC_4> 		<DSC_5>NO ENCONTRO FLETES / FREIGHT</DSC_5> 	</DSC> 	<DSC> 		<DSC_1>false</DSC_1> 		<DSC_2>1</DSC_2> 		<DSC_3>45</DSC_3> 		<DSC_4>USD</DSC_4> 		<DSC_5>NO ENCONTRO SEGURO / INSURANCE</DSC_5> 	</DSC> 	<DRF> 		<DRF_1>18762007201018 </DRF_1> 		<DRF_2>2018-03-05</DRF_2> 		<DRF_3>2020-03-04</DRF_3> 		<DRF_4>EX</DRF_4> 		<DRF_5>11317</DRF_5> 		<DRF_6>14000</DRF_6> 	</DRF> 	<NOT> 		<NOT_1>1.-SOMOS GRANDES CONTRIBUYENTES RESOLUCIÃ“N DIAN 0000076 DE 2016/12/01</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>8.-39.23.50.90.00</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>9.-MP-0075</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>10.-39</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>11.-73,284%</NOT_1> 	</NOT> 	<NOT> 		<NOT_1>12.-NET 180 </NOT_1> 	</NOT> 	<REF> 		<REF_1>AAJ</REF_1> 		<REF_2>279107</REF_2> 	</REF> 	<REF> 		<REF_1>AAJ</REF_1> 		<REF_2>279108</REF_2> 	</REF> 	<REF> 		<REF_1>AAJ</REF_1> 		<REF_2>279109</REF_2> 	</REF> 	<IEN> 		<IEN_1>1893 SW 3RD STREET</IEN_1> 		<IEN_6>US</IEN_6> 	</IEN> 	<ITE> 		<ITE_1>1</ITE_1> 		<ITE_2>false</ITE_2> 		<ITE_3>500000.000</ITE_3> 		<ITE_4>MIO</ITE_4> 		<ITE_5>1795000</ITE_5> 		<ITE_6>USD</ITE_6> 		<ITE_7>3.59</ITE_7> 		<ITE_8>USD</ITE_8> 		<ITE_9>K00003</ITE_9> 		<ITE_10>HARD EMPTY VEGETABLE CAPSULES|CAPSULAS VGETALES DE DOS PIEZAS VACIAS|SIZE 0 1-0K/1-0K (NATURAL/NATURAL)</ITE_10> 		<ITE_11/> 		<ITE_12/> 		<TII> 			<TII_1>0</TII_1> 			<TII_2>USD</TII_2> 			<TII_3>false</TII_3> 		</TII> 	</ITE> 	<ITE> 		<ITE_1>2</ITE_1> 		<ITE_2>false</ITE_2> 		<ITE_3>400000.000</ITE_3> 		<ITE_4>MIO</ITE_4> 		<ITE_5>2304000</ITE_5> 		<ITE_6>USD</ITE_6> 		<ITE_7>5.76</ITE_7> 		<ITE_8>USD</ITE_8> 		<ITE_9>K00012</ITE_9> 		<ITE_10>HARD EMPTY VEGETABLE CAPSULES|CAPSULAS VEGETAL DE DOS PIEZAS VACIAS|SIZE/TAMAÑO 00 1-0K/1-0K (NATURAL/NATURAL)</ITE_10> 		<ITE_11/> 		<ITE_12/> 		<TII> 			<TII_1>0</TII_1> 			<TII_2>USD</TII_2> 			<TII_3>false</TII_3> 		</TII> 	</ITE> 	<ITE> 		<ITE_1>3</ITE_1> 		<ITE_2>false</ITE_2> 		<ITE_3>75000.000</ITE_3> 		<ITE_4>MIO</ITE_4> 		<ITE_5>447000</ITE_5> 		<ITE_6>USD</ITE_6> 		<ITE_7>5.96</ITE_7> 		<ITE_8>USD</ITE_8> 		<ITE_9>K00061</ITE_9> 		<ITE_10>HARD EMPTY TWO PIECES VEGETABLE CAPSULE|CAPSULAS VEGETALES DE DOS PIEZAS VACIAS|SIZE 00E 1-0K/1-0K (NATURAL/NATURAL)</ITE_10> 		<ITE_11>KC00E-1000</ITE_11> 		<ITE_12/> 		<TII> 			<TII_1>0</TII_1> 			<TII_2>USD</TII_2> 			<TII_3>false</TII_3> 		</TII> 	</ITE> </FACTURA> "));
            res = Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(x));
            //return Convert.ToBase64String(Encoding.UTF8.GetBytes(s.ToString()));

            return "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9InllcyI/Pg0KPEZBQ1RVUkE+DQoJPEVOQz4NCgkJPEVOQ18xPklOVk9JQzwvRU5DXzE+DQoJCTxFTkNfMj44OTAxMDU5Mjc8L0VOQ18yPg0KCQk8RU5DXzM+NjUtMDQ2MjIwNzwvRU5DXzM+DQoJCTxFTkNfND5VQkwgMi4wPC9FTkNfND4NCgkJPEVOQ181PkRJQU4gMS4wPC9FTkNfNT4NCgkJPEVOQ182PkVYMTE2MTc8L0VOQ182Pg0KCQk8RU5DXzc+MjAxOC0wNy0wMjwvRU5DXzc+DQoJCTxFTkNfOD4yMzoxMDowMTwvRU5DXzg+DQoJCTxFTkNfOT4xPC9FTkNfOT4NCgkJPEVOQ18xMD5VU0Q8L0VOQ18xMD4NCgkJPEVOQ18xMS8+DQoJCTxFTkNfMTIvPg0KCQk8RU5DXzEzLz4NCgkJPEVOQ18xNC8+DQoJCTxFTkNfMTUvPg0KCQk8RU5DXzE2PjIwMTgtMTItMjk8L0VOQ18xNj4NCgkJPEVOQ18xNy8+DQoJCTxFTkNfMTgvPg0KCTwvRU5DPg0KCTxFTUk+DQoJCTxFTUlfMT4zPC9FTUlfMT4NCgkJPEVNSV8yPjg5MDEwNTkyNzwvRU1JXzI+DQoJCTxFTUlfMz4zMTwvRU1JXzM+DQoJCTxFTUlfND4yPC9FTUlfND4NCgkJPEVNSV81PjEwPC9FTUlfNT4NCgkJPEVNSV82PkMuSS4gRkFSTUFDQVBTVUxBUyBTLkEuPC9FTUlfNj4NCgkJPEVNSV83PkMuSS4gRkFSTUFDQVBTVUxBUyBTLkEuPC9FTUlfNz4NCgkJPEVNSV84Lz4NCgkJPEVNSV85Lz4NCgkJPEVNSV8xMD5DQUxMRSA3OUIgTm8gNzhDLTIxIEFQRE8gQUVSRU8gLyBDT0RFIFBPU1RBTCA1MjQ4NjwvRU1JXzEwPg0KCQk8RU1JXzExLz4NCgkJPEVNSV8xMj5CQVJSQU5RVUlMTEE8L0VNSV8xMj4NCgkJPEVNSV8xMz5CQVJSQU5RVUlMTEE8L0VNSV8xMz4NCgkJPEVNSV8xNC8+DQoJCTxFTUlfMTU+Q088L0VNSV8xNT4NCgkJPEVNSV8xNi8+DQoJCTxFTUlfMTcvPg0KCQk8RU1JXzE4PkNBTExFIDc5QiBObyA3OEMtMjEgQVBETyBBRVJFTyAvIENPREUgUE9TVEFMIDUyNDg2PC9FTUlfMTg+DQoJCTxFTUlfMTkvPg0KCQk8RU1JXzIwLz4NCgkJPEVNSV8yMS8+DQoJCTxJQ0M+DQoJCQk8SUNDXzEvPg0KCQkJPElDQ18yLz4NCgkJCTxJQ0NfMy8+DQoJCQk8SUNDXzQvPg0KCQkJPElDQ181Lz4NCgkJCTxJQ0NfNi8+DQoJCQk8SUNDXzcvPg0KCQkJPElDQ184Lz4NCgkJPC9JQ0M+DQoJPC9FTUk+DQoJPEFEUT4NCgkJPEFEUV8xPi0xPC9BRFFfMT4NCgkJPEFEUV8yPjY1LTA0NjIyMDcgICAgIDwvQURRXzI+DQoJCTxBRFFfMz4zMTwvQURRXzM+DQoJCTxBRFFfND4tMTwvQURRXzQ+DQoJCTxBRFFfNT4xMDEwPC9BRFFfNT4NCgkJPEFEUV82Lz4NCgkJPEFEUV83Lz4NCgkJPEFEUV84Lz4NCgkJPEFEUV85Lz4NCgkJPEFEUV8xMD4xODkzIFNXIDNSRCBTVFJFRVQ8L0FEUV8xMD4NCgkJPEFEUV8xMS8+DQoJCTxBRFFfMTIvPg0KCQk8QURRXzEzLz4NCgkJPEFEUV8xNC8+DQoJCTxBRFFfMTU+VVM8L0FEUV8xNT4NCgkJPEFEUV8xNi8+DQoJCTxBRFFfMTcvPg0KCQk8QURRXzE4PjE4OTMgU1cgM1JEIFNUUkVFVDwvQURRXzE4Pg0KCQk8QURRXzE5Lz4NCgkJPEFEUV8yMC8+DQoJCTxBRFFfMjEvPg0KCQk8VENSPg0KCQkJPFRDUl8xPk8tOTk8L1RDUl8xPg0KCQkJPFRDUl8yLz4NCgkJCTxUQ1JfMy8+DQoJCQk8VENSXzQvPg0KCQkJPFRDUl81Lz4NCgkJCTxUQ1JfNi8+DQoJCQk8VENSXzcvPg0KCQkJPFRDUl84Lz4NCgkJCTxUQ1JfOS8+DQoJCQk8VENSXzEwLz4NCgkJCTxUQ1JfMTEvPg0KCQk8L1RDUj4NCgkJPElDUj4NCgkJCTxJQ1JfMS8+DQoJCQk8SUNSXzIvPg0KCQkJPElDUl8zLz4NCgkJCTxJQ1JfNC8+DQoJCQk8SUNSXzUvPg0KCQkJPElDUl82Lz4NCgkJCTxJQ1JfNy8+DQoJCQk8SUNSXzgvPg0KCQk8L0lDUj4NCgkJPENEQT4NCgkJCTxDREFfMS8+DQoJCQk8Q0RBXzIvPg0KCQkJPENEQV8zLz4NCgkJCTxDREFfNC8+DQoJCTwvQ0RBPg0KCTwvQURRPg0KCTxUT1Q+DQoJCTxUT1RfMT40NTQ2MDAwPC9UT1RfMT4NCgkJPFRPVF8yPlVTRDwvVE9UXzI+DQoJCTxUT1RfMz40NTQ2MDAwPC9UT1RfMz4NCgkJPFRPVF80PlVTRDwvVE9UXzQ+DQoJCTxUT1RfNT40NTQ2MDAwPC9UT1RfNT4NCgkJPFRPVF82PlVTRDwvVE9UXzY+DQoJCTxUT1RfNz40NTQ2MDAwPC9UT1RfNz4NCgkJPFRPVF84PlVTRDwvVE9UXzg+DQoJCTxUT1RfOT40NTQ2MDAwPC9UT1RfOT4NCgkJPFRPVF8xMD5VU0Q8L1RPVF8xMD4NCgkJPFRPVF8xMj5VU0Q8L1RPVF8xMj4NCgk8L1RPVD4NCgk8VERDLz4NCgk8T1RDLz4NCgk8RFNDPg0KCQk8RFNDXzE+ZmFsc2U8L0RTQ18xPg0KCQk8RFNDXzI+MTwvRFNDXzI+DQoJCTxEU0NfMz44MDEuMDU8L0RTQ18zPg0KCQk8RFNDXzQ+VVNEPC9EU0NfND4NCgkJPERTQ181Pk5PIEVOQ09OVFJPIEZMRVRFUyAvIEZSRUlHSFQ8L0RTQ181Pg0KCTwvRFNDPg0KCTxEU0M+DQoJCTxEU0NfMT5mYWxzZTwvRFNDXzE+DQoJCTxEU0NfMj4xPC9EU0NfMj4NCgkJPERTQ18zPjQ1PC9EU0NfMz4NCgkJPERTQ180PlVTRDwvRFNDXzQ+DQoJCTxEU0NfNT5OTyBFTkNPTlRSTyBTRUdVUk8gLyBJTlNVUkFOQ0U8L0RTQ181Pg0KCTwvRFNDPg0KCTxEUkY+DQoJCTxEUkZfMT4xODc2MjAwNzIwMTAxOCAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIDwvRFJGXzE+DQoJCTxEUkZfMj4yMDE4LTAzLTA1PC9EUkZfMj4NCgkJPERSRl8zPjIwMjAtMDMtMDQ8L0RSRl8zPg0KCQk8RFJGXzQ+RVg8L0RSRl80Pg0KCQk8RFJGXzU+MTEzMTc8L0RSRl81Pg0KCQk8RFJGXzY+MTQwMDA8L0RSRl82Pg0KCTwvRFJGPg0KCTxOT1Q+DQoJCTxOT1RfMT4xLi1TT01PUyBHUkFOREVTIENPTlRSSUJVWUVOVEVTIFJFU09MVUNJw4PigJxOIERJQU4gMDAwMDA3NiBERSAyMDE2LzEyLzAxPC9OT1RfMT4NCgk8L05PVD4NCgk8Tk9UPg0KCQk8Tk9UXzE+OC4tMzkuMjMuNTAuOTAuMDA8L05PVF8xPg0KCTwvTk9UPg0KCTxOT1Q+DQoJCTxOT1RfMT45Li1NUC0wMDc1PC9OT1RfMT4NCgk8L05PVD4NCgk8Tk9UPg0KCQk8Tk9UXzE+MTAuLTM5PC9OT1RfMT4NCgk8L05PVD4NCgk8Tk9UPg0KCQk8Tk9UXzE+MTEuLTczLDI4NCU8L05PVF8xPg0KCTwvTk9UPg0KCTxOT1Q+DQoJCTxOT1RfMT4xMi4tTkVUIDE4MCAgICAgICAgPC9OT1RfMT4NCgk8L05PVD4NCgk8UkVGPg0KCQk8UkVGXzE+QUFKPC9SRUZfMT4NCgkJPFJFRl8yPjI3OTEwNzwvUkVGXzI+DQoJPC9SRUY+DQoJPFJFRj4NCgkJPFJFRl8xPkFBSjwvUkVGXzE+DQoJCTxSRUZfMj4yNzkxMDg8L1JFRl8yPg0KCTwvUkVGPg0KCTxSRUY+DQoJCTxSRUZfMT5BQUo8L1JFRl8xPg0KCQk8UkVGXzI+Mjc5MTA5PC9SRUZfMj4NCgk8L1JFRj4NCgk8SUVOPg0KCQk8SUVOXzE+MTg5MyBTVyAzUkQgU1RSRUVUPC9JRU5fMT4NCgkJPElFTl82PlVTPC9JRU5fNj4NCgk8L0lFTj4NCgk8SVRFPg0KCQk8SVRFXzE+MTwvSVRFXzE+DQoJCTxJVEVfMj5mYWxzZTwvSVRFXzI+DQoJCTxJVEVfMz41MDAwMDAuMDAwPC9JVEVfMz4NCgkJPElURV80Pk1JTzwvSVRFXzQ+DQoJCTxJVEVfNT4xNzk1MDAwPC9JVEVfNT4NCgkJPElURV82PlVTRDwvSVRFXzY+DQoJCTxJVEVfNz4zLjU5PC9JVEVfNz4NCgkJPElURV84PlVTRDwvSVRFXzg+DQoJCTxJVEVfOT5LMDAwMDM8L0lURV85Pg0KCQk8SVRFXzEwPkhBUkQgRU1QVFkgVkVHRVRBQkxFIENBUFNVTEVTfENBUFNVTEFTICBWR0VUQUxFUyBERSBET1MgUElFWkFTIFZBQ0lBU3xTSVpFIDAgMS0wSy8xLTBLIChOQVRVUkFML05BVFVSQUwpPC9JVEVfMTA+DQoJCTxJVEVfMTEvPg0KCQk8SVRFXzEyLz4NCgkJPFRJST4NCgkJCTxUSUlfMT4wPC9USUlfMT4NCgkJCTxUSUlfMj5VU0Q8L1RJSV8yPg0KCQkJPFRJSV8zPmZhbHNlPC9USUlfMz4NCgkJPC9USUk+DQoJPC9JVEU+DQoJPElURT4NCgkJPElURV8xPjI8L0lURV8xPg0KCQk8SVRFXzI+ZmFsc2U8L0lURV8yPg0KCQk8SVRFXzM+NDAwMDAwLjAwMDwvSVRFXzM+DQoJCTxJVEVfND5NSU88L0lURV80Pg0KCQk8SVRFXzU+MjMwNDAwMDwvSVRFXzU+DQoJCTxJVEVfNj5VU0Q8L0lURV82Pg0KCQk8SVRFXzc+NS43NjwvSVRFXzc+DQoJCTxJVEVfOD5VU0Q8L0lURV84Pg0KCQk8SVRFXzk+SzAwMDEyPC9JVEVfOT4NCgkJPElURV8xMD5IQVJEIEVNUFRZIFZFR0VUQUJMRSAgQ0FQU1VMRVN8Q0FQU1VMQVMgIFZFR0VUQUwgREUgRE9TIFBJRVpBUyBWQUNJQVN8U0laRS9UQU1Bw5FPIDAwIDEtMEsvMS0wSyAoTkFUVVJBTC9OQVRVUkFMKTwvSVRFXzEwPg0KCQk8SVRFXzExLz4NCgkJPElURV8xMi8+DQoJCTxUSUk+DQoJCQk8VElJXzE+MDwvVElJXzE+DQoJCQk8VElJXzI+VVNEPC9USUlfMj4NCgkJCTxUSUlfMz5mYWxzZTwvVElJXzM+DQoJCTwvVElJPg0KCTwvSVRFPg0KCTxJVEU+DQoJCTxJVEVfMT4zPC9JVEVfMT4NCgkJPElURV8yPmZhbHNlPC9JVEVfMj4NCgkJPElURV8zPjc1MDAwLjAwMDwvSVRFXzM+DQoJCTxJVEVfND5NSU88L0lURV80Pg0KCQk8SVRFXzU+NDQ3MDAwPC9JVEVfNT4NCgkJPElURV82PlVTRDwvSVRFXzY+DQoJCTxJVEVfNz41Ljk2PC9JVEVfNz4NCgkJPElURV84PlVTRDwvSVRFXzg+DQoJCTxJVEVfOT5LMDAwNjE8L0lURV85Pg0KCQk8SVRFXzEwPkhBUkQgRU1QVFkgVFdPIFBJRUNFUyBWRUdFVEFCTEUgQ0FQU1VMRXxDQVBTVUxBUyBWRUdFVEFMRVMgREUgRE9TIFBJRVpBUyBWQUNJQVN8U0laRSAwMEUgMS0wSy8xLTBLIChOQVRVUkFML05BVFVSQUwpPC9JVEVfMTA+DQoJCTxJVEVfMTE+S0MwMEUtMTAwMDwvSVRFXzExPg0KCQk8SVRFXzEyLz4NCgkJPFRJST4NCgkJCTxUSUlfMT4wPC9USUlfMT4NCgkJCTxUSUlfMj5VU0Q8L1RJSV8yPg0KCQkJPFRJSV8zPmZhbHNlPC9USUlfMz4NCgkJPC9USUk+DQoJPC9JVEU+DQo8L0ZBQ1RVUkE+DQo=";
        }
    }
}

