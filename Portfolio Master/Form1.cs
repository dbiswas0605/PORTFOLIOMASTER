using ChartClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using YahooFinanceAPI.YahooDataClient;

namespace Portfolio_Master
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Transaction myTransaction = new Transaction();
            YahooData mySeries = myTransaction.getSeries(false);

            LineChart lc = new LineChart();
            lc.setSeries(mySeries);

            DataSet ds = lc.getHistoricalPrices();
            DataSet dsTemp = myTransaction.consolidatedPortfolio(ds);
            myTransaction.updateTransaction(ds);

            BuildChart(chart1,dsTemp);
        }

        public void BuildChart(Chart chart1,DataSet IndexData)
        {
            chart1.Series.Clear();

            chart1.Titles.Add("Total Income");

            //Series series = chart1.Series.Add("Total Income");
            //series.ChartType = SeriesChartType.Spline;

            // Set palette.
            this.chart1.Palette = ChartColorPalette.SeaGreen;

            // Set title.
            this.chart1.Titles.Add("Pets");

            chart1.ChartAreas[0].AxisX.Interval = 0.5;
            chart1.ChartAreas[0].AxisY.Maximum = 30000;

            foreach (DataRow dr in IndexData.Tables["dtConsolidatedByTimeLine"].Rows)
            {
                //series.Points.AddXY(dr["DATE"].ToString(), Convert.ToDouble(dr["NETWORTH"].ToString()));
                //series.Points.AddXY(Convert.ToDouble(dr["NETWORTH"].ToString()));


                // Add series.
                Series series = this.chart1.Series.Add(dr["DATE"].ToString());

                // Add point.
                series.Points.Add(Convert.ToDouble(dr["NETWORTH"].ToString()));


            }

        }
    }
}
