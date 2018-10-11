using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WSBSS_FacturaElectronica
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new Revisor()
            //};
            //ServiceBase.Run(ServicesToRun);


            if (Environment.UserInteractive)
            {
                Console.WriteLine("Inicia Servicio Consola");
                Revisor.GenerarHIlo();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new Revisor()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
