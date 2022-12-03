using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using PortfolioTinkoff.Services;
using PortfolioTinkoff.Models;
using PortfolioTinkoff.Models.OperationsClasses;
using System.Threading;
using System.Linq;

namespace PortfolioTinkoff.Controllers
{
    public class OpenPositionsController : Controller
    {
        private OperationService operationService;
        private IMainTable mainTable;
        private OperationTable operationTable;
        public OpenPositionsController(OperationService _operationService, IMainTable mainTable, OperationTable operationTable)
        {
            operationService = _operationService;
            this.mainTable = mainTable;
            this.operationTable = operationTable;
        }
       
        public async Task<IActionResult> Index()
        {
            mainTable.portfolioPositions = await operationService.GetDescriptionPorfolioPositionsAsync(CancellationToken.None);
           
            
            return View(mainTable);
        }
        public async Task<IActionResult> OperationList()
        {
            operationTable.AllOperations = await operationService.GetOperationsAppAsync(CancellationToken.None);
            return View(operationTable);
        }
    }
}
