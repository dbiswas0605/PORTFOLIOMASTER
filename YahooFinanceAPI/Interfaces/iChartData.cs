using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;
using YahooFinanceAPI.YahooDataClient;

namespace ChartClient
{
    public interface iChartData
    {
        DataSet getHistoricalPrices();
        void setSeries(string stock, string startdate, string enddate);
        void setSeries(YahooData yahoodata);
    }
}
