using Google.Protobuf.WellKnownTypes;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace PortfolioTinkoff.Models
{
    public class AllOperations
    {
        public string OperationId { get; set; }
        public string? ParentOperationId { get; set; } 
        public string OperationType { get; set; } 
        public DateTime Datetime { get; set; }
        public string OperationCurrency { get; set; }
        public decimal Payment { get; set; }
        public string Tiker { get; set; }
        public decimal PaymentRub { get; set; }
        public string OperationFigi { get; set; }
        public string OperationStatus { get; set; }
        public string Figi { get; set; }

        //public AllOperations(string optype, Timestamp timestamp, string operationCurrency, decimal pay, string Tiker, decimal paymentrub, string operationfigi, string operstatus)
        //{

        //}

    }
}
