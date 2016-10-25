using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Data;
using System.Xml;
using YahooFinanceAPI.YahooDataClient;
using System;

namespace YahooFinance
{
    public class YahooFinanceDataBase
    {
        string _yahooAPI = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.historicaldata%20where%20symbol%20%3D%20%22[TICKER]%22%20and%20startDate%20%3D%20%22[STARTDATE]%22%20and%20endDate%20%3D%20%22[ENDDATE]%22&diagnostics=true&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";
        YahooData _data;
        public YahooData portFolioData
        {
            set { _data = value; }
        }

        private DataTable getSingleIndexPrice(string p_ticker, string p_startdate, string p_enddate)
        {
            string sURL = _yahooAPI.Replace("[TICKER]", p_ticker).Replace("[STARTDATE]", p_startdate).Replace("[ENDDATE]", p_enddate);

            sURL = System.Net.WebUtility.UrlEncode(sURL);

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            StreamReader objReader = new StreamReader(objStream);

            string sXML = objReader.ReadToEnd();

            DataTable dt = new DataTable(p_ticker);
            dt.Columns.Add("Date");
            dt.Columns.Add("Open");
            dt.Columns.Add("High");
            dt.Columns.Add("Low");
            dt.Columns.Add("Close");
            dt.Columns.Add("Volume");
            dt.Columns.Add("Networth");
            dt.Columns.Add("Cash");

            XmlDocument xResults = new XmlDocument();

            xResults.LoadXml(sXML);
            //xResults.Load(@"C:\Users\d.biswas\Desktop\yahooResults.xml");

            XmlNode results = xResults.SelectSingleNode("//results");

            foreach (XmlNode quote in results)
            {
                DataRow dr = dt.NewRow();
                dr["Date"] = quote.SelectSingleNode("Date").InnerText;
                dr["Open"] = quote.SelectSingleNode("Open").InnerText;
                dr["High"] = quote.SelectSingleNode("High").InnerText;
                dr["Low"] = quote.SelectSingleNode("Low").InnerText;
                dr["Close"] = quote.SelectSingleNode("Close").InnerText;
                dr["Volume"] = quote.SelectSingleNode("Volume").InnerText;
                dt.Rows.Add(dr);

            }
            return dt;
        }

        public DataSet getHistoricalPrice()
        {
            DataSet HistoricalData = new DataSet();

            foreach (Tuple<string, string, string> t in _data.getSeries())
            {
                DataTable dtPriceList = getSingleIndexPrice(t.Item1, t.Item2, t.Item3);

                if(!HistoricalData.Tables.Contains(dtPriceList.TableName))
                    HistoricalData.Tables.Add(dtPriceList);
                else
                    HistoricalData.Tables[dtPriceList.TableName].Merge(dtPriceList);
            }

            return HistoricalData;
        }

    }
}
