using System;
using System.Collections.Generic;
using System.Xml;
using YahooFinance;
using YahooFinanceAPI.YahooDataClient;
using System.Data;

namespace ChartClient
{
    public class LineChart : iChartData
    {
        YahooData seriesData = new YahooData();

        public void setSeries(string index, string startDate, string endDate)
        {
            seriesData.addSeries(index, startDate, endDate);
        }

        public void setSeries(YahooData yahoodata)
        {
            foreach (Tuple<string, string, string> t in yahoodata.getSeries())
            {
                setSeries(t.Item1, t.Item2, t.Item3);
            }
        }

        public DataSet getHistoricalPrices()
        {
            YahooFinanceDataBase YahooFD = new YahooFinanceDataBase();

            YahooFD.portFolioData = seriesData;
            DataSet allPriceSeries = YahooFD.getHistoricalPrice();

            return allPriceSeries;
        }
    }
}
