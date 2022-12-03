using System.Collections.Generic;
using System.Linq;
using System;
using Tinkoff.InvestApi.V1;
using Tinkoff.InvestApi;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace PortfolioTinkoff.Models
{
    public interface IMainTable
    {
       // Task<List<PortfolioPosition>> GetPortfolioPosition(); 
       public List<PortfolioPosition> portfolioPositions { get; set; }
       //public List<AllOperations> AllOperations { get; set; }
       //public Dictionary<string,Dictionary<Timestamp, decimal>> MyOperations { get; set; }
     
       Task<decimal> GetCashRub();
        decimal GetAveragePercent();
        Task<decimal> GetPortfolioCostRub();
        //decimal GetOpearationByTypeSum(string operationType);
        //Task<Dictionary<string, Dictionary<DateTime, decimal>>> GetMyOpeations();
    }
}
