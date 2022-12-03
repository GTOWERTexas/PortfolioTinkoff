using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;
using Microsoft.Extensions.DependencyInjection;


namespace PortfolioTinkoff.Services
{
    public class DataParcer
    {
        private InvestApiClient _investApiClient;
        public DataParcer(InvestApiClient investApiClient)
        {
            _investApiClient = investApiClient;
        }
        public async Task<Google.Protobuf.Collections.RepeatedField<Account>> GetAccounts()
        {
            var accounts = await _investApiClient.Users.GetAccountsAsync();
            return accounts.Accounts;
        }
        
    }
}
