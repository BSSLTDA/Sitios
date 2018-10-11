using System.Runtime.Serialization;
using System.ServiceModel;

namespace BssFacturaElectronicaService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IServiceFact" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IServiceFact
    {
        [OperationContract]
        UploadResult CargarFactura(string Prefijo, string Factura);

        [OperationContract]
        string DescargarFactura(string Prefijo, string Factura);

        [OperationContract]
        StatusResult EstadoFactura(string TransId = "");
    }


    [DataContract]
    public class UploadResult
    {
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string TransactionId { get; set; }
        [DataMember]
        public string Faultcode { get; set; }
        [DataMember]
        public string Faultstring { get; set; }
        [DataMember]
        public ErrorDetail Detail { get; set; }
    }

    [DataContract]
    public class ErrorDetail
    {
        [DataMember]
        public string StatusCode { get; set; }
        [DataMember]
        public string ReasonPhrase { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public class StatusResult
    {
        [DataMember]
        public string ProcessName { get; set; }
        [DataMember]
        public string ProcessStatus { get; set; }
        [DataMember]
        public string ProcessDate { get; set; }
        [DataMember]
        public string MessageType { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public string Faultcode { get; set; }
        [DataMember]
        public string Faultstring { get; set; }
        [DataMember]
        public ErrorDetail Detail { get; set; }
    }


}



