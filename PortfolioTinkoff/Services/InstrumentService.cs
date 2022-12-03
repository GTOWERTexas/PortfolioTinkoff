using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace PortfolioTinkoff.Services
{
    public class InstrumentService
    {       
        private InvestApiClient _investApiClient;
        public InstrumentService(InvestApiClient investApiClient)
        {
            _investApiClient = investApiClient;
        }
        public async Task<Instrument> GetInstrumentByFigi(string figi)
        {
            var positionData = await _investApiClient.Instruments.GetInstrumentByAsync(new InstrumentRequest { IdType  = InstrumentIdType.Figi, Id = figi});
            return positionData.Instrument;
        }
        public async Task<string> GetCurrencyByIso(string iso)
        {
            string currencyFigi = string.Empty;
            var insmass = await _investApiClient.Instruments.CurrenciesAsync(new InstrumentsRequest { InstrumentStatus = InstrumentStatus.Base });
            foreach (var item in insmass.Instruments)
            {
                if (item.IsoCurrencyName == iso)
                {
                    currencyFigi = item.Figi;
                }
                
            }
            return currencyFigi;
        }
      
    }
}
