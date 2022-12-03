using PortfolioTinkoff.Services;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
namespace PortfolioTinkoff.Models.OperationsClasses
{
    public class SessionOperationTable : OperationTable
    {
        public SessionOperationTable(OperationService operationService) : base(operationService)
        {
        }

        public ISession Session { get; set; }
        public static OperationTable GetOperationTable(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            OperationTable sessionOperationTable = session?.GetJson<OperationTable>("OperationTable") ?? services.GetRequiredService<OperationTable>();
            sessionOperationTable.Session = session;
            return sessionOperationTable;
        }
    }
}
