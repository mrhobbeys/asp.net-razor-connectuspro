using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using SiteBlue.Areas.OwnerPortal.Models;
using System.Linq;
using System;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class ChartsController : Controller
    {

        EightHundredEntities db = new EightHundredEntities();
        public FileResult InvoiceAging(int id)
        {
            try
            {
                IEnumerable<Aging> jobs = getcharts(id);
                Chart chart = CreateInvoiceAging(jobs);
                return CreateFile(chart);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }
        private IList<Aging> getcharts(int id)
        {
            try
            {
                //List<Job> j = (from frn in db.tbl_Job join c in db.tbl_Customer on frn.CustomerID equals c.CustomerID select frn).Take(10).ToList();
                //return j;

                var result = (from j in db.tbl_Job
                              join c in db.tbl_Customer on j.CustomerID equals c.CustomerID
                              join ci in db.tbl_Customer_Info on c.CustomerID equals ci.CustomerID
                              join cct in db.tbl_Customer_CreditTerms on ci.CreditTermsID equals cct.CreditTermsID
                              join f in db.tbl_Franchise on j.FranchiseID equals f.FranchiseID

                              select new
                              {
                                  InvoiceDate = j.InvoicedDate,
                                  TotalSales = j.TotalSales,
                                  CreditTerms = cct.CreditTerms,
                                  FranchiseID = f.FranchiseID
                              }).Take(20).ToList();
                int tt;
                var newresult = from r in result.ToList()

                                select new
                                {

                                    InvoiceDate = r.InvoiceDate.Value.ToShortDateString(),
                                    TotalSales = r.TotalSales,
                                    CreditTerms = (r.CreditTerms == "Net 30 Days" ? tt = (DateTime.Now - Convert.ToDateTime(r.InvoiceDate)).Days - 30 : (DateTime.Now - Convert.ToDateTime(r.InvoiceDate)).Days),
                                    FranchiseID = r.FranchiseID
                                };
                var s = (from p in newresult.Where(k => k.FranchiseID == id)
                         select new
                         {

                             InvoiceDate = p.InvoiceDate,
                             TotalSales = p.TotalSales,
                             CreditTerms = (p.CreditTerms > 0 ? p.CreditTerms : 0),
                             FranchiseID = p.FranchiseID
                         });

                var aging = from g in s.ToList()

                            select new Aging
                            {
                                DaysLate = g.CreditTerms,
                                Amount = g.TotalSales

                            };

                return aging.Take(7).ToList();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }
        private static Chart CreateInvoiceAging(IEnumerable<Aging> job)
        {
            try
            {
                string xCol = "#d5f1ff";
                Chart chart = new Chart
                {
                    Width = 612,
                    Height = 527,
                    BackColor = System.Drawing.ColorTranslator.FromHtml(xCol),
                    Palette = ChartColorPalette.BrightPastel,
                    RenderType = RenderType.BinaryStreaming,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };
                chart.Titles.Add(CreateTitle(mTechniciansSalesAndDiscountTitle1));
                chart.Legends.Add(CreateTechniciansSalesAndDiscountLegend());
                foreach (Series series in CreateInvoiceAgingSeries(job))
                {
                    chart.Series.Add(series);
                }
                chart.ChartAreas.Add(CreateTechniciansSalesAndDiscountArea1());
                return chart;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }



        private static Series[] CreateInvoiceAgingSeries(IEnumerable<Aging> job)
        {
            try
            {
                Series[] seriesArray =
                new Series[]
                    {
                        new Series
                            {
                                Name = "Dollar Amount",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            }
                       
                    };
                const string drawingStyle = "Cylinder";
                seriesArray[0]["DrawingStyle"] = drawingStyle;
                const string pointWidth = "0.6";
                seriesArray[0]["PointWidth"] = pointWidth;
                foreach (var item in job)
                {
                    seriesArray[0].Points.Add(new DataPoint
                    {
                        AxisLabel = (item.DaysLate).ToString(),
                        YValues = new[] 
                                                            { 
                                                                (double) (item.Amount) 
 
                                                            }
                    });
                }


                return seriesArray;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }

        private static ChartArea CreateTechniciansSalesAndDiscountArea1()
        {
            try
            {

                ChartArea chartArea = new ChartArea
                {
                    InnerPlotPosition = new ElementPosition(5, 0, 100, 85),
                    Name = "InvoiceAgingDataArea",
                    BackColor = Color.Transparent
                };
                chartArea.Area3DStyle.Enable3D = true;
                chartArea.Area3DStyle.LightStyle = LightStyle.Realistic;
                chartArea.Area3DStyle.Perspective = 10;
                chartArea.AxisX.IsLabelAutoFit = false;
                chartArea.AxisY.IsLabelAutoFit = false;
                chartArea.AxisX.LabelStyle.Font =
                    new Font("Verdana,Arial,Helvetica,sans-serif",
                             8F,
                             FontStyle.Regular);
                chartArea.AxisX.LabelStyle.Angle = -90;
                chartArea.AxisY.LabelStyle.Font =
                   new Font("Verdana,Arial,Helvetica,sans-serif",
                            8F,
                            FontStyle.Regular);

                chartArea.AxisY.LabelStyle.Format = "C";
                chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisX.Interval = 1;
                chartArea.AxisY.Interval = 1000.0;
                return chartArea;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }


        private FileResult CreateFile(Chart chart)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    chart.SaveImage(memoryStream);
                    return File(memoryStream.ToArray(), "image/png");
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }





        public FileResult CallReceiveDate()
        {
            try
            {
                Chart chart = CreateJobClosingRateChart();
                return CreateFile(chart);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }


        private static Chart CreateJobClosingRateChart()
        {
            try
            {
                Chart chart = new Chart
                {
                    Width = 118,
                    Height = 118,
                    Palette = ChartColorPalette.None,
                    RenderType = RenderType.BinaryStreaming,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High

                };
                chart.PaletteCustomColors = new Color[]
                                            {
                                                Color.FromArgb(200, 53, 99, 50),
                                                Color.FromArgb(70, 200, 0, 0),
                                                Color.FromArgb(200, 115, 0, 6),
                                                Color.FromArgb(100, 70, 20, 10),
                                                Color.FromArgb(50, 10, 40, 20),
                                                Color.FromArgb(200, 0, 60, 30),
                                                Color.FromArgb(100, 100, 0, 200),
                                                Color.FromArgb(200, 50, 0, 30),
                                                
                                            };
                foreach (Series series in CreateJobClosingRateSeries())
                {
                    chart.Series.Add(series);
                }
                foreach (ChartArea area in CreateJobClosingRateAreas())
                {
                    chart.ChartAreas.Add(area);
                }
                return chart;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }



        private static List<Title> CreateJobClosingRateTitles()
        {
            try
            {
                List<Title> titles = new List<Title>();
                int i = 0;
                titles.Add(new Title
                {
                    DockedToChartArea = "JobArea" + ++i,
                    Font = new Font("Trebuchet MS", 8F, FontStyle.Bold),
                    DockingOffset = 50

                });

                return titles;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }


        private static List<Series> CreateJobClosingRateSeries()
        {
            try
            {

                List<Series> seriesList = new List<Series>();
                int i = 0;

                {
                    Series series = new Series
                    {
                        Name = "JobSeries" + ++i,
                        ChartType = SeriesChartType.Pie,
                        ChartArea = "JobArea" + i

                    };
                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(2.00)
                                                    }
                    });
                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(1.00)
                                                    }
                    });
                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(1.00),
                                                        
                                                    }
                    });


                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(2.00)
                                                    }
                    });
                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(4.00)
                                                    }
                    });

                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(1.00)
                                                    }
                    });
                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(7.00)
                                                    }
                    });
                    series.Points.Add(new DataPoint
                    {
                        YValues = new[] 
                                                    { 
                                                        (double)(3.00)
                                                    }
                    });
                    series.Points[0].BackSecondaryColor = Color.White;
                    series.Points[0].BackGradientStyle = GradientStyle.TopBottom;
                    series.Points[1].BackSecondaryColor = Color.White;
                    series.Points[1].BackGradientStyle = GradientStyle.TopBottom;
                    series.Points[2].BackSecondaryColor = Color.White;
                    series.Points[2].BackGradientStyle = GradientStyle.TopBottom;
                    series.Points[3].BackSecondaryColor = Color.White;
                    series.Points[3].BackGradientStyle = GradientStyle.TopBottom;
                    series.Points[4].BackSecondaryColor = Color.White;
                    series.Points[4].BackGradientStyle = GradientStyle.TopBottom;
                    series.Points[5].BackSecondaryColor = Color.White;
                    series.Points[5].BackGradientStyle = GradientStyle.TopBottom;
                    series.Points[6].BackSecondaryColor = Color.White;
                    series.Points[6].BackGradientStyle = GradientStyle.TopBottom;
                    series.Points[7].BackSecondaryColor = Color.White;
                    series.Points[7].BackGradientStyle = GradientStyle.TopBottom;

                    seriesList.Add(series);
                }
                return seriesList;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }


        private static List<ChartArea> CreateJobClosingRateAreas()
        {
            try
            {
                List<ChartArea> chartAreas = new List<ChartArea>();
                int i = 0;

                ChartArea chartArea =
                    new ChartArea
                    {
                        InnerPlotPosition = new ElementPosition(0, 0, 100, 100),
                        Name = "JobArea" + ++i,
                        BackColor = Color.Transparent
                    };
                chartArea.Area3DStyle.Enable3D = true;
                chartArea.Area3DStyle.Inclination = 15;
                chartArea.Area3DStyle.Rotation = 90;
                chartAreas.Add(chartArea);

                //}
                return chartAreas;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }



        private static Title CreateTitle(string text)
        {
            try
            {
                return new Title
                {
                    Text = text,
                    ShadowColor = Color.FromArgb(32, 0, 0, 0),
                    ShadowOffset = 3,
                    Font = new Font("Trebuchet MS", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(26, 59, 105)
                };
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }



        private static Legend CreateTechniciansSalesAndDiscountLegend()
        {
            try
            {
                return new Legend
                {
                    BackColor = Color.Transparent
                };
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }



        private static ChartArea CreateTechniciansSalesAndDiscountArea()
        {
            try
            {

                ChartArea chartArea = new ChartArea
                {
                    InnerPlotPosition = new ElementPosition(5, 0, 100, 85),
                    Name = "TechniciansSalesAndDiscountArea",
                    BackColor = Color.Transparent
                };
                chartArea.Area3DStyle.Enable3D = true;
                chartArea.Area3DStyle.LightStyle = LightStyle.Realistic;
                chartArea.Area3DStyle.Perspective = 10;
                chartArea.AxisX.IsLabelAutoFit = false;
                chartArea.AxisY.IsLabelAutoFit = false;
                chartArea.AxisX.LabelStyle.Font =
                    new Font("Verdana,Arial,Helvetica,sans-serif",
                             8F,
                             FontStyle.Regular);
                chartArea.AxisX.LabelStyle.Angle = -90;
                chartArea.AxisY.LabelStyle.Font =
                   new Font("Verdana,Arial,Helvetica,sans-serif",
                            8F,
                            FontStyle.Regular);
                chartArea.AxisY.LabelStyle.Format = "C";
                chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
                chartArea.AxisX.Interval = 1;
                chartArea.AxisY.Interval = 1000.0;
                return chartArea;
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }


        private const string mTechniciansSalesAndDiscountTitle =
            "Technicians Sales and Discount";

        private const string mJobClosingRateTitle = "Job Closing Rate";

        private const string mTechniciansSalesAndDiscountTitle1 =
            "Invoice Aging Data";
        private const string mJobClosingRateTitle1 = "Call Receive Date";
    }
}
