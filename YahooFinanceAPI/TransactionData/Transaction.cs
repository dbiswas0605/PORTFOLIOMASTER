using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using YahooFinanceAPI.YahooDataClient;
using System.Configuration;

namespace Portfolio_Master
{
    public class Transaction
    {
        OleDbConnection con;
        OleDbDataAdapter da;
        OleDbCommand cmd;
        DataSet dsTransaction = new DataSet();

        public Transaction()
        {
            string MyTransactionsFile = ConfigurationManager.ConnectionStrings["MyTransactions"].ConnectionString;

            string m_sConn1 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + MyTransactionsFile + ";Extended Properties=Excel 12.0;";
            con = new OleDbConnection(m_sConn1);
            con.Open();

            DataTable dtSheets = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            da = new OleDbDataAdapter();
            cmd = new OleDbCommand();
            cmd.Connection = con;

            foreach (DataRow drSheet in dtSheets.Rows)
            {
                if (drSheet["TABLE_NAME"].ToString().Contains("IGNORE")) continue;

                cmd.CommandText= "select * from [" + drSheet["TABLE_NAME"].ToString().Replace("#",".") + "]"; ;
                da.SelectCommand = cmd;
                
                da.Fill(dsTransaction, drSheet["TABLE_NAME"].ToString().Replace("$", "").Replace("#", "."));
            }
        }

        public YahooData getSeries(bool readByRange)
        {
            YahooData yd = new YahooData();
            string stock, startDate, endDate;

            if (readByRange)
            {
                foreach (DataTable dt in dsTransaction.Tables)
                {
                    stock = dt.TableName;
                    startDate = dt.Rows[0][0].ToString();
                    endDate = dt.Rows[dt.Rows.Count - 1][0].ToString();

                    yd.addSeries(stock, startDate, endDate);
                }
            }
            else
            {
                foreach (DataTable dt in dsTransaction.Tables)
                {
                    stock = dt.TableName;
                    foreach(DataRow dr in dt.Rows)
                    {
                        startDate = dr[0].ToString();
                        endDate = dr[0].ToString();

                        yd.addSeries(stock, startDate, endDate);
                    }
                }
            }

            return yd;
        }

        public void updateTransaction(DataSet dsPrice)
        {
            cmd = new OleDbCommand();
            cmd.Connection = con;

            foreach(DataTable dt in dsTransaction.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow[] drPrice = dsPrice.Tables[dt.TableName].Select("Date = '" + dr["date"].ToString() + "'");
                    if (drPrice.Length > 0)
                    {
                        string sql = "UPDATE [" + dt.TableName + "$] SET NETWORTH = " + Convert.ToDouble(drPrice[0]["Close"].ToString()) * Convert.ToInt16(dr["QUANTITY"].ToString()) + ", UNITPRICE = " + Convert.ToDouble(drPrice[0]["Close"].ToString()) + " where date = '" + dr["date"].ToString() + "'";
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            con.Close();
        }

        public DataSet consolidatedPortfolio(DataSet dsPrice)
        {
            DataSet dsConsolidated = new DataSet();
            DataTable dtConsolidated = new DataTable();
            DataTable dtConsolidatedByTimeLine = new DataTable();

            dtConsolidated = dsTransaction.Tables[0].Clone();
            dtConsolidated.TableName = "dtConsolidated";
            dtConsolidated.Columns["NETWORTH"].DataType = Type.GetType("System.Double");

            dtConsolidatedByTimeLine = dsTransaction.Tables[0].Clone();
            dtConsolidatedByTimeLine.TableName = "dtConsolidatedByTimeLine";


            foreach (DataTable dtStocks in dsTransaction.Tables)
            {
                foreach (DataRow drPosition in dtStocks.Rows)
                {
                    DataRow[] drPrice = dsPrice.Tables[dtStocks.TableName].Select("Date = '" + drPosition["date"].ToString() + "'");
                    if (drPrice.Length > 0)
                    {
                        drPosition["NETWORTH"] = Convert.ToDouble(drPrice[0]["Close"].ToString()) * Convert.ToInt16(drPosition["QUANTITY"].ToString());
                        drPosition["UNITPRICE"] = Convert.ToDouble(drPrice[0]["Close"].ToString());

                        dtConsolidated.Rows.Add(drPosition.ItemArray);
                    }
                }
            }

            dsConsolidated.Tables.Add(dtConsolidated);

            var groupedData = from b in dtConsolidated.AsEnumerable()
                              group b by b.Field<string>("DATE") into g
                              select new
                              {
                                  DateTag = g.Key,
                                  NetworthSum = g.Sum(x => x.Field<double>("NETWORTH"))
                              };



            foreach (var grp in groupedData)
            {
                var dr = dtConsolidatedByTimeLine.NewRow();

                dr.BeginEdit();
                dr["DATE"] = grp.DateTag;
                dr["NETWORTH"] = grp.NetworthSum;
                dr.EndEdit();

                dtConsolidatedByTimeLine.Rows.Add(dr);
            }

            dsConsolidated.Tables.Add(dtConsolidatedByTimeLine);
            return dsConsolidated;
        }


    }
}
