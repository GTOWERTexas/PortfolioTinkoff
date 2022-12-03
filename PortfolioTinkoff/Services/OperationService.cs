using Tinkoff.InvestApi.V1;
using Tinkoff.InvestApi;
using Google.Protobuf.WellKnownTypes;
using RateLimiter;
using ComposableAsync;
using Microsoft.Extensions.Caching.Memory;
using PortfolioTinkoff.Models;
using System.Linq;

namespace PortfolioTinkoff.Services
{
    public class OperationService
    {
        private InvestApiClient investApiClient;
        private InstrumentService instrumentService;
        private QuatationService quatationService;
        private IMemoryCache memoryCache;
        public OperationService(InvestApiClient investApiClient, InstrumentService instrumentService, QuatationService quatationService, IMemoryCache memoryCache)
        {
            this.investApiClient = investApiClient;
            this.instrumentService = instrumentService;
            this.quatationService = quatationService;
            this.memoryCache = memoryCache;
        }

        internal async Task<List<Models.PortfolioPosition>> GetDescriptionPorfolioPositionsAsync(CancellationToken cancellationToken)
        {
            var accounts = await investApiClient.Users.GetAccountsAsync();
            var accountId = accounts.Accounts.First().Id;

            var operations = investApiClient.Operations;
            var portfolioResponse = await operations.GetPortfolioAsync(new PortfolioRequest { AccountId = accountId });
            var positionsResponse = await operations.GetPositionsAsync(new PositionsRequest { AccountId = accountId });
            var withdrawLimits = await operations.GetWithdrawLimitsAsync(new WithdrawLimitsRequest { AccountId = accountId });

            var operationsList = await operations.GetOperationsAsync(new OperationsRequest
            {
                AccountId = accountId,
                From = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                To = Timestamp.FromDateTime(DateTime.UtcNow)
            });


            var currenciesBase = await investApiClient.Instruments.CurrenciesAsync(new InstrumentsRequest { InstrumentStatus = InstrumentStatus.Base });
            List<Models.PortfolioPosition> positions = new List<Models.PortfolioPosition>();
            foreach (var this_pos in portfolioResponse.Positions)
            {
                if(this_pos.Figi == "RUB000UTSTOM") { continue; }
                var this_pos_instrument = await instrumentService.GetInstrumentByFigi(this_pos.Figi);
                var market_data_currency_price = await quatationService.GetCurrentMarketPrice(this_pos.Figi)/* ?? await quatationService.GetClosePrice(this_pos.Figi)*/;
                var current_price2 = this_pos.CurrentPrice.Units + (decimal)this_pos.CurrentPrice.Nano / 1000000000;
                var currency = this_pos.AveragePositionPrice.Currency;
                decimal market_rate_today = 1;
                //foreach (var item in currenciesBase.Instruments)
                //{
                //    if (currency == item.IsoCurrencyName)
                //    {
                //        if (currency == "Rub")
                //        {
                //            market_rate_today = 1;
                //        }
                //        market_rate_today = await quatationService.GetCurrentMarketPrice(item.Figi); 
                //    }
                //}
                Models.PortfolioPosition pos = Models.PortfolioPosition.FromApiData(this_pos, this_pos_instrument, current_price2, market_rate_today);

                positions.Add(pos);
            }
            positions.OrderBy(c => c.Name);
            GetAveragePercent(positions);
            await GetPortfolioCostRub(positions);
            return positions;
        }
        internal async Task<decimal> GetCashRub()
        {
            var accounts = await investApiClient.Users.GetAccountsAsync();
            var accountId = accounts.Accounts.First().Id;
            var operations = investApiClient.Operations;
            var withdrawLimits = await operations.GetWithdrawLimitsAsync(new WithdrawLimitsRequest { AccountId = accountId });

            foreach (var item in withdrawLimits.Money)
            {
                if (item.Currency == "Rub" || item.Currency == "rub" || item.Currency == "RUB")
                {
                    decimal v = item.Units + (decimal)item.Nano / 1000000000;
                    return v;
                }
            }
            return 0;
        }
        internal static decimal GetAveragePercent(List<Models.PortfolioPosition> positions)
        {
            decimal sumbuyList = 0;
            decimal expyeildList = 0;
            foreach (var item in positions)
            {
                sumbuyList += item.SumBye;
                expyeildList += item.Expected_yield;
            }
            if (sumbuyList > 0)
            {
                return Math.Round(expyeildList / sumbuyList * 100, 3);
            }
            return 0;
        }
        internal async Task<decimal> GetPortfolioCostRub(List<Models.PortfolioPosition> positions)
        {
            decimal marketcosts = 0;
            foreach (var item in positions)
            {
                marketcosts += item.MarketCost;
            }
            decimal cashrub = await GetCashRub();
            return marketcosts + cashrub;
        }

        internal async Task<List<Models.AllOperations>> GetOperationsAppAsync(CancellationToken cancellationToken)
        {
            List<Models.AllOperations> allOperations;

            var accounts = await investApiClient.Users.GetAccountsAsync();
            var accountId = accounts.Accounts.First().Id;
            var operations = investApiClient.Operations;

            var operationsList = await operations.GetOperationsAsync(new OperationsRequest
            {
                AccountId = accountId,
                From = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-4)),/*accounts.Accounts.First().OpenedDate,*/
                To = Timestamp.FromDateTime(DateTime.UtcNow)
            });
            decimal paymentRub;
            var key = accountId + "02";
            if (!memoryCache.TryGetValue(key, out allOperations))
            {
                allOperations = new List<Models.AllOperations>();

             foreach (var this_op in operationsList.Operations)
             {
                    if (this_op.OperationType.ToString() == "MarginFee" || this_op.OperationType.ToString() == "Input" || this_op.OperationType.ToString() == "Output" || this_op.OperationType.ToString() == "TaxCorrection" || this_op.OperationType.ToString() == "Tax")
                    {
                        this_op.Figi = await instrumentService.GetCurrencyByIso(this_op.Currency);
                    }
                    if (this_op.Currency != "rub")
                    {
                        var marketcurrate = await quatationService.GetFigiHistoryPrice(this_op.Figi, this_op.Date);
                        paymentRub = (this_op.Payment.Units + (decimal)this_op.Payment.Nano / 1000000000) * marketcurrate;
                    }
                    else
                    {
                        paymentRub = this_op.Payment.Units + (decimal)this_op.Payment.Nano / 1000000000;
                    }
                    if (this_op.State != OperationState.Canceled)
                    {
                        allOperations.Add(new Models.AllOperations()
                        {
                            OperationId = this_op.Id,
                            Figi = this_op.Figi,
                            OperationType = this_op.OperationType.ToString(),
                            Datetime = this_op.Date.ToDateTime(),
                            OperationCurrency = this_op.Currency,
                            Payment = this_op.Payment.Units + (decimal)this_op.Payment.Nano / 1000000000,
                            Tiker = instrumentService.GetInstrumentByFigi(this_op.Figi).Result.Ticker,
                            PaymentRub = paymentRub,
                            OperationFigi = this_op.Figi,
                            OperationStatus = this_op.State.ToString()
                        }); ;
                        Thread.Sleep(200);
                    }
             }
                memoryCache.Set(key, allOperations, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2)));
            }
            return allOperations;
        }
        internal decimal SumOperationsByType(List<Models.AllOperations> operations, string opertionType)
        {
            decimal sum = 0;

                foreach (var item in operations)
                {                    
                    if (item.OperationType.ToString() == opertionType && item.Payment != 0)
                    {
                        sum += (decimal)item.PaymentRub;
                    }
                }
            return Math.Round(sum,2);
        }
       
        
    }
   
}
