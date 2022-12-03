using Google.Protobuf.WellKnownTypes;
using PortfolioTinkoff.Services;
namespace PortfolioTinkoff.Models.OperationsClasses
{
    public class OperationTable /*: IOperationTable*/
    {
        private OperationService _operationService;
        public ISession Session { get; set; }
        public OperationTable(OperationService operationService)
        {
            _operationService = operationService;
        }
        
        public List<AllOperations> AllOperations { get ; set ; }

        public async Task<decimal> GetOpearationByTypeSumAsync(string operationType)
        {
            return await Task.Run(() => _operationService.SumOperationsByType(AllOperations, operationType));
        }
       
    }
}
