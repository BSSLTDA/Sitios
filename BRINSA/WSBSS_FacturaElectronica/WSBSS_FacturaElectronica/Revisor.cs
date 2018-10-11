using System.ServiceProcess;
using WSBSS_FacturaElectronica.Logs;
using WSBSS_FacturaElectronica.Threads;

namespace WSBSS_FacturaElectronica
{
    public partial class Revisor : ServiceBase
    {
        private readonly int tiempoHiloFE = Properties.Settings.Default.TiempoHiloFE;
        private static ThreadFE hiloFE = new ThreadFE();
        private LogReenvio logProcesos = new LogReenvio();

        public Revisor()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            hiloFE.GenerarHiloFE();
        }

        protected override void OnStop()
        {
            logProcesos.CapturaLog("Detiene Servicio de manera Correcta.", "Servicio Windows");
        }

        public static void GenerarHIlo()
        {
            hiloFE.GenerarHiloFE();
        }
    }
}
