using CLCommon;
using CLDB2;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WSBSS_FacturaElectronica.Logs;

namespace WSBSS_FacturaElectronica.Threads
{
    class ThreadFE
    {
        private Thread nuevoHilo;
        private Thread HiloReenvio;
        private Thread HiloReenvioNotas;
        private Timer timerFE;
        private readonly int tiempoHiloFE = Properties.Settings.Default.TiempoHiloFE;
        private LogReenvio LogReenvio = new LogReenvio();
        IRFLOGFERepository dRFLOGFE = new RFLOGFERepository();
        IRFNCCABRepository dRFNCCAB = new RFNCCABRepository();
        IRFFACCABRepository dRFFACCAB = new RFFACCABRepository();
        IRFPARAMRepository dRFPARAM = new RFPARAMRepository();
        List<RFLOGFE> lmRFLOGFE = new List<RFLOGFE>();
        RFPARAM mRFPARAM = new RFPARAM();
        ISIHRepository dSIH = new SIHRepository();
        List<SIH> lmSIH = new List<SIH>();
        List<SIH> lmSIHN = new List<SIH>();
        SIH mSIH = new SIH();
        SIH mSIHN = new SIH();

        public void GenerarHiloFE()
        {
            try
            {
                if (Environment.UserInteractive)
                {
                    ProcesoPrincipalFE("");
                }
                else
                {
                    nuevoHilo = new Thread(new ThreadStart(Run));
                    nuevoHilo.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.ToString());
                LogReenvio.CapturaLog(ex.ToString(), "GenerarHiloFE");
                LogReenvio.GuardaLog("GenerarHiloFE", ex.ToString(), "", 1, "RFE");
            }
        }

        private void Run()
        {
            timerFE = new Timer(ProcesoPrincipalFE, null, 0, tiempoHiloFE);
        }

        private void RunReenvio()
        {
            string Baseurl = mRFPARAM.CCNOT1;
            string data = "{" + string.Format(" 'ACCION': 'Cargar', 'Prefijo': '{0}', 'Factura': '{1}' ", mSIH.IHDPFX, Conversion.Val(mSIH.IHDOCN)) + "}";
            
            try
            {
                Console.WriteLine(data);
                using (HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { Timeout = TimeSpan.FromMinutes(30) })
                {
                    var Hrm = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(Baseurl),
                        Method = HttpMethod.Post
                    };
                    Hrm.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    Hrm.Headers.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Hrm.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage HresponseM = client.SendAsync(Hrm).Result;
                    Task<Stream> streamTask = HresponseM.Content.ReadAsStreamAsync();
                    Stream stream = streamTask.Result;
                    StreamReader sr = new StreamReader(stream);
                    var res = sr.ReadToEnd();
                    Console.WriteLine(data + ": " + res);
                    client.Dispose();
                    Hrm.Dispose();
                    HresponseM.Dispose();
                    streamTask.Dispose();
                    stream.Close();
                    stream.Flush();
                    stream.Dispose();
                    sr.Close();
                    sr.Dispose();
                }
                    
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Baseurl);
                //request.Method = "POST";
                //request.ContentType = "application/json";
                //request.ContentLength = data.Length;
                //request.Timeout = 600000;
                //StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                //requestWriter.Write(data);
                //requestWriter.Close();
                //WebResponse webResponse = request.GetResponse();
                //Stream webStream = webResponse.GetResponseStream();
                //StreamReader responseReader = new StreamReader(webStream);
                //var response = responseReader.ReadToEnd();
                //Console.WriteLine(mSIH.IHDPFX + mSIH.IHDOCN + ": " + response);
                //responseReader.Close();
                //webStream.Close();
                //webResponse.Close();
            }
            catch (Exception ex)
            {
                string err = "ERROR: " + data + ": " + ex.ToString();
            }
        }

        private void RunReenvioNotas()
        {
            string Baseurl = mRFPARAM.CCNOT1;
            string data = "{" + string.Format(" 'ACCION': 'Cargar', 'Prefijo': '{0}', 'Nota': '{1}' ", mSIHN.IHDPFX, Conversion.Val(mSIHN.IHDOCN)) + "}";
            
            try
            {
                Console.WriteLine(data);
                using (HttpClient client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { Timeout = TimeSpan.FromMinutes(30) })
                {
                    var Hrm = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(Baseurl),
                        Method = HttpMethod.Post
                    };
                    Hrm.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    Hrm.Headers.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Hrm.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage HresponseM = client.SendAsync(Hrm).Result;
                    Task<Stream> streamTask = HresponseM.Content.ReadAsStreamAsync();
                    Stream stream = streamTask.Result;
                    StreamReader sr = new StreamReader(stream);
                    var res = sr.ReadToEnd();
                    Console.WriteLine(data + ": " + res);
                    client.Dispose();
                    Hrm.Dispose();
                    HresponseM.Dispose();
                    streamTask.Dispose();
                    stream.Close();
                    stream.Flush();
                    stream.Dispose();
                    sr.Close();
                    sr.Dispose();
                }

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Baseurl);
                //request.Method = "POST";
                //request.ContentType = "application/json";
                //request.ContentLength = data.Length;
                //request.Timeout = 600000;
                //StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);                
                //requestWriter.Write(data);
                //requestWriter.Close();
                //WebResponse webResponse = request.GetResponse();
                //Stream webStream = webResponse.GetResponseStream();
                //StreamReader responseReader = new StreamReader(webStream);
                //var response = responseReader.ReadToEnd();
                //Console.WriteLine(mSIHN.IHDPFX + mSIHN.IHDOCN + ": " + response);
                //responseReader.Close();
                //webStream.Close();
                //webResponse.Close();
            }
            catch (Exception ex)
            {
                string err = "ERROR: " + data + ": " + ex.ToString();
            }
        }

        private void ProcesoPrincipalFE(Object o)
        {
            string res = "";
            lmSIH = dSIH.GetFacturas();
            lmSIHN = dSIH.GetNotas();
            foreach (SIH item in lmSIH)
            {
                if (File.Exists(item.PDF))
                {
                    Console.WriteLine("Cambiando nombre: " + item.IHDPFX + item.IHDOCN);
                    res = dRFFACCAB.UpdExistePDF(item.IHDPFX, item.IHDOCN);
                    Console.WriteLine(res);
                }
                else
                {
                    lmRFLOGFE = dRFLOGFE.GetLog(item.IHDPFX, item.IHDOCN);
                    if (lmRFLOGFE.Count > 0)
                    {
                        dRFLOGFE.UpdBusy(item.IHDPFX, item.IHDOCN);
                        mRFPARAM = dRFPARAM.GetAPI("API_FE");
                        if (mRFPARAM != null)
                        {
                            mSIH = item;
                            HiloReenvio = new Thread(new ThreadStart(RunReenvio));
                            HiloReenvio.Start();
                            Thread.Sleep(300);
                        }
                    }                    
                }
            }

            foreach (SIH item in lmSIHN)
            {
                if (File.Exists(item.PDF))
                {
                    Console.WriteLine("Cambiando nombre: " + item.IHDPFX + item.IHDOCN);
                    res = dRFNCCAB.UpdExistePDF(item.IHDPFX, item.IHDOCN);
                    Console.WriteLine(res);
                }
                else
                {
                    lmRFLOGFE = dRFLOGFE.GetLog(item.IHDPFX, item.IHDOCN);
                    if (lmRFLOGFE.Count > 0)
                    {
                        dRFLOGFE.UpdBusy(item.IHDPFX, item.IHDOCN);
                        mRFPARAM = dRFPARAM.GetAPI("API_NCE");
                        if (mRFPARAM != null)
                        {
                            mSIHN = item;
                            HiloReenvioNotas = new Thread(new ThreadStart(RunReenvioNotas));
                            HiloReenvioNotas.Start();
                            Thread.Sleep(300);
                        }
                    }
                }
           }
        }
    }
}
