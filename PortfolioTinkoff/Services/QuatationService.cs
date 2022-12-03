using Tinkoff.InvestApi.V1;
using Tinkoff.InvestApi;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace PortfolioTinkoff.Services
{
    public class QuatationService
    {
        private InvestApiClient _investApiClient;
        public QuatationService(InvestApiClient investApiClient)
        {
            _investApiClient = investApiClient;
        }
        public async Task<decimal> GetCurrentMarketPrice(string figi)
        {
                var lastprice = await _investApiClient.MarketData.GetLastPricesAsync(new GetLastPricesRequest
                {
                    Figi =
                        {
                           figi
                        }
                });

                return await Task.Run(() => lastprice.LastPrices.FirstOrDefault().Price.Units + (decimal)lastprice.LastPrices.FirstOrDefault().Price.Nano / 1000000000);
            
            //var candlesnow = await _investApiClient.MarketData.GetCandlesAsync(new GetCandlesRequest {
            //    Figi = figi,
            //    From = Timestamp.FromDateTime(DateTime.UtcNow.AddHours(-1)),
            //    To = Timestamp.FromDateTime(DateTime.UtcNow),
            //    Interval = CandleInterval._1Min});
            //if (candlesnow.Candles.Count > 0)
            //{
            //    return candlesnow.Candles.Last().Close.Units + (decimal)candlesnow.Candles.Last().Close.Nano/1000000000;
            //}
            //else
            //{
            //  var closeprice =  await _investApiClient.MarketData.GetClosePricesAsync(new GetClosePricesRequest { Instruments = { new InstrumentClosePriceRequest { InstrumentId = figi } } });
            //  return closeprice.ClosePrices.FirstOrDefault().Price.Units + (decimal)closeprice.ClosePrices.FirstOrDefault().Price.Nano / 1000000000;
            //}                                                                    
        }
        public async Task<decimal> GetClosePrice(string figi)
        {
            var closeprice = await _investApiClient.MarketData.GetClosePricesAsync(new GetClosePricesRequest { Instruments = { new InstrumentClosePriceRequest { InstrumentId = figi } } });
            return closeprice.ClosePrices.FirstOrDefault().Price.Units + (decimal)closeprice.ClosePrices.FirstOrDefault().Price.Nano / 1000000000;
        }
        public async Task<decimal> GetFigiHistoryPrice(string figi, Timestamp date)
        {
            var historycandles = await _investApiClient.MarketData.GetCandlesAsync(new GetCandlesRequest
            {
                Figi = figi,
                From = date,
                To = Timestamp.FromDateTime(date.ToDateTime().AddDays(+1)),
                Interval = CandleInterval.Day
            });
            try
            {
                 return await Task.Run(()=>(historycandles.Candles.FirstOrDefault().High.Units + (decimal)historycandles.Candles.FirstOrDefault().High.Nano / 1000000000 +
                                            historycandles.Candles.FirstOrDefault().Low.Units + (decimal)historycandles.Candles.FirstOrDefault().Low.Nano / 1000000000)/2);
            }
            catch 
            {
                return 0;               
            }
            // return aveprice;
        }
       
    }
}
