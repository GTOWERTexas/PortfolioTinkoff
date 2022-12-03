using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;


namespace PortfolioTinkoff.Models
{
    public class PortfolioPosition
    {
        public string Figi { get; set; }
        public string Name { get; set; }
        public string Tiker { get; set; }
        public decimal Balance { get; set; }
        public string PositionType { get; set; }
        public string Currency { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal ExpYield { get; set; }
        public decimal AveBuyPriceRUB { get; set; }
        public MoneyValue Average_position_price { get; set; }
        public MoneyValue? Average_position_price_no_nkd { get; set; }
        public bool Blocked { get; set; }
        public Quotation Expected_yield { get; set; }
        public decimal Current_market_price { get; set; }
        public string Isin { get; set; }
        public long Lots { get; set; }
        public decimal Today_market_rate { get; set; } = 1;
        public Instrument Instrument { get; set; }
       
        
        public  static PortfolioPosition FromApiData(
                                                    Tinkoff.InvestApi.V1.PortfolioPosition this_pos, 
                                                    Instrument this_pos_instrument,
                                                    decimal curr_market_price, decimal market_rate)
        {
            PortfolioPosition pos = new PortfolioPosition() {Figi = this_pos.Figi, Name = this_pos_instrument.Name, Tiker = this_pos_instrument.Ticker, 
                                                             PositionType = this_pos.InstrumentType, Currency = this_pos.AveragePositionPrice.Currency.ToUpper(),
                                                             AveragePrice = this_pos.AveragePositionPrice.Units+(decimal)this_pos.AveragePositionPrice.Nano/1000000000,
                                                             ExpYield = this_pos.ExpectedYield.Units+(decimal)this_pos.ExpectedYield.Nano/1000000000,
                                                             Balance = this_pos.Quantity};
            pos.Current_market_price = curr_market_price;
            pos.Today_market_rate = market_rate;
            pos.Average_position_price = this_pos.AveragePositionPrice;
            pos.Average_position_price_no_nkd = this_pos.CurrentNkd;
            pos.Blocked = this_pos.Blocked;
            pos.Expected_yield = this_pos.ExpectedYield;
            pos.Lots = this_pos.QuantityLots.Units;
            pos.Isin = this_pos_instrument.Isin;
            if (pos.Currency != "Rub")
            {
                pos.AveBuyPriceRUB = pos.AveragePrice * pos.Today_market_rate;
            }

            pos.Instrument = this_pos_instrument;
            return pos;
        }
        


        public decimal MarketPrice
        {
            get
            {
                if (AveragePrice > 0 && PositionType == "Bond")
                {
                    return Math.Round(MarketCost / Balance, 2);
                };
                return Current_market_price;
            }
            set {

                ; }
        }
        public decimal MarketCost
        {
            get
            {
                if (AveragePrice > 0 && PositionType == "Bond")
                {
                    MarketCost = Math.Round(ExpYield + (AveragePrice*Balance)); 
                    return MarketCost;
                }

                return (decimal)Current_market_price * Balance;
            }
            set
            {
               
            }
        }
        public decimal MarketValueRub
        {
            get
            {
                return (decimal)Current_market_price * Today_market_rate;
            }
        }
        public decimal PercentChange
        {
            get
            {
                if (AveragePrice > 0)
                {
                    return ((MarketPrice / AveragePrice) * 100) - 100;
                }
                return 0;
            }
            set { }
        }
        public decimal SumBye
        {
            get { return (decimal)AveragePrice * Balance; }
        }
        public decimal SumByeRub
        {
            get { return (decimal)AveBuyPriceRUB * Balance; }
        }

        public decimal TaxBase
        {
            get {
                if (AveragePrice > 0)
                {
                    return MarketCost - SumBye;
                }
                return 0;
            }
        }
        public decimal ExpBase
        {
            get
            {
                int taxrate = 13;
                return TaxBase * (decimal)(taxrate/100);
            }
        }

        public decimal ExpPercent { get { return Math.Round(TaxBase / SumBye * 100, 2); } set { } }  
    }
}
