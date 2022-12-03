using Google.Protobuf.WellKnownTypes;
using PortfolioTinkoff.Services;

namespace PortfolioTinkoff.Models
{
    public class MainTable : IMainTable
    {
        private OperationService _operationService;
        public MainTable(OperationService operationService)
        {
            _operationService = operationService;
        }

        public List<PortfolioPosition> portfolioPositions { get; set; }
        //public List<AllOperations> AllOperations { get ; set ; }
       // public Dictionary<string, Dictionary<Timestamp, decimal>> MyOperations { get ; set; }

        public  async Task<decimal> GetCashRub()
        {
            return await  _operationService.GetCashRub();
        }
        public decimal GetAveragePercent()
        {
            return OperationService.GetAveragePercent(portfolioPositions);
        }

        public async Task<decimal> GetPortfolioCostRub()
        {
           return await _operationService.GetPortfolioCostRub(portfolioPositions);
        }

        //public decimal GetOpearationByTypeSum(string operationType)
        //{
        //    return _operationService.SumOpertionsByType(AllOperations, operationType);
        //}

        //public async Task<Dictionary<string, Dictionary<DateTime, decimal>>> GetMyOpeations()
        //{
        //    return await _operationService.GetMyOperations(CancellationToken.None);
        //}




        //public async Task<List<PortfolioPosition>> GetPortfolioPosition()
        //{
        //  return await _operationService.GetDescriptionPorfolioPositionsAsync(CancellationToken.None);
        //}
    }
}
