using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YahooFinanceAPI.YahooDataClient
{
    public class YahooData
    {
        List<Tuple<string, string, string>> stockQuery = new List<Tuple<string, string, string>>();

        public void addSeries(string stock, string startDate, string endDate)
        {
            Tuple<string, string, string> temp = new Tuple<string, string, string>(stock, startDate, endDate);
            stockQuery.Add(temp);
        }

        public List<Tuple<string, string, string>> getSeries()
        {
            return stockQuery;
        }
    }
}
