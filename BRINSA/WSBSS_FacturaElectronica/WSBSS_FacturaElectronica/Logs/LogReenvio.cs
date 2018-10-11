using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace WSBSS_FacturaElectronica.Logs
{
    class LogReenvio
    {
        private string appPath = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\\Log\\";
        
        private StreamWriter sw;
        private StringBuilder sb;
        private StringBuilder Error = new StringBuilder();

        /// <summary>
        /// directory exists
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        public static bool DirectoryExists(string name, bool create = false)
        {
            if (!Directory.Exists(name))
            {
                if (create)
                    Directory.CreateDirectory(name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Guarda txt en carpeta log de instalación
        /// </summary>
        /// <param name="err">Descripción del error</param>
        /// <param name="proceso">Proceso en el que ocurrio el error</param>
        /// <remarks>Autor: JASC Fecha: 2018-06-06</remarks>
        public void CapturaLog(string err, string proceso)
        {
            DirectoryExists(appPath, true);
            sb = new StringBuilder();
            sw = new StreamWriter(appPath + "\\" + proceso + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt");
            sb.AppendLine(proceso);
            sb.AppendLine(err);
            sb.AppendLine(DateTime.Now.ToString());
            sw.WriteLine(sb.ToString());
            sw.Close();
        }

        public void GuardaLog(string OPERACION, string EVENTO, string TXTSQL, decimal ALERT, string KEY)
        {
            try
            {
                var ensamblado = Assembly.GetExecutingAssembly();
                Console.WriteLine("USUARIO: " + System.Net.Dns.GetHostName());
                Console.WriteLine("OPERACION: " + OPERACION);
                Console.WriteLine("PROGRAMA: " + ensamblado.GetName().Name + "- V" + ensamblado.GetName().Version.ToString());
                Console.WriteLine("EVENTO: " + EVENTO);
                Console.WriteLine("TXTSQL: " + TXTSQL);
                Console.WriteLine("ALERT: " + ALERT);
                Console.WriteLine("LKEY: " + KEY);                
            }            
            catch (Exception ex)
            {
                Error.AppendLine(ex.ToString());
                Console.WriteLine("ERROR: " + Error);
                CapturaLog(Error.ToString(), "GuardaLog " + KEY);
            }
        }
    }
}
