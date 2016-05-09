using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;



namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class LegendController : Controller
    {

        public FileResult InvoiceAging()
        {
            //IEnumerable<Technician> technicians =
            //    TempData.Peek("Technicians") as IEnumerable<Technician>;
            //if (technicians == null)
            //{
            //    return null;
            //}
            Chart chart = CreateInvoiceAging();
            
            return CreateFile(chart);
        }

        private static Chart CreateInvoiceAging()
        {
            string xCol = "#c2d5fc";
            Chart chart = new Chart
            {
                Width = 800,
                Height = 408,
                BackColor = System.Drawing.ColorTranslator.FromHtml(xCol),
                //BackSecondaryColor = Color.WhiteSmoke,
                //BackGradientStyle = GradientStyle.DiagonalRight,
                //BorderlineDashStyle = ChartDashStyle.Solid,
                //BorderlineWidth = 1,
                //BorderlineColor = Color.FromArgb(26, 59, 105),
                //Palette = ChartColorPalette.BrightPastel,
                //RenderType = RenderType.BinaryStreaming,
                //AntiAliasing = AntiAliasingStyles.All,
                //TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };
            //chart.Titles.Add(CreateTitle(mTechniciansSalesAndDiscountTitle1));
            chart.Legends.Add(CreateTechniciansSalesAndDiscountLegend());
            chart.Legends[0].Docking = Docking.Top;
            
            chart.Legends[0].LegendItemOrder = LegendItemOrder.ReversedSeriesOrder;
            chart.Legends[0].TableStyle = LegendTableStyle.Auto;
           
            //chart.Series[0].ChartType = SeriesChartType.Bar;
            //chart.Series[0]["PieLabelStyle"] = "Inside";
            //chart.Series[0]["PieLabelStyle"] = "Disabled";
            foreach (Series series in CreateInvoiceAgingSeries())
            {
                chart.Series.Add(series);
            }
            chart.ChartAreas.Add(CreateTechniciansSalesAndDiscountArea1());
           
            return chart;
        }



        private static Series[] CreateInvoiceAgingSeries()
        {
            Series[] seriesArray =
                new Series[]
                    {
                        new Series
                            {
                                Name = "Returning Customer",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Postcard",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Door Hanger",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Yellow Pages",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Internet",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Truck",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Radio",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Pole Sign",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Yard Sign",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Billboard",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Friend or Neighbor",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "TV",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Val-Pak",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Business Card",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Marriage Letter",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Marriage Postcard",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Meals on Wheels",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Better Business Bureau",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Service Magic",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Outbound Call",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Senior Citizen Services",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Tip of the Week",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "HPCA - Atlanta",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Post Card - Tip of the Month",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "American Home Shield",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "True Value",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Assoc of Construction",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Home Buyers Resale",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Contractors.com",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Indoor Air Quality Card",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Recall on Previous Work",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Post Card - Thank You",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            }, new Series
                            {
                                Name = "Chamber of Commerce",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Outside Sales",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Clipper Magazine",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Atlanta Business Chronicle",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Water Analysis",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Community Connector",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Trade Show",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Chamblee Coupon",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Taste of Chamblee Show",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Super Saver Coupon",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Magellan Realty",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "RSVP",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "BNI",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "The Warranty Group",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Church Bulletin",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Brown Bag",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Loclly",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Angie's List",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Ask A Plumber",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Sticker on Unit",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Traveler's Insurance",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Commercial Customer",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Penny Saver",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Silverlake",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Lowes",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Groupon",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "First American",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Golf Tournament",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Business Showcase",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Email",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Unknown",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Black,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea"
                            }
                            
                      
                    };
           
            return seriesArray;
        }

        private static ChartArea CreateTechniciansSalesAndDiscountArea1()
        {
            ChartArea chartArea = new ChartArea
            {
                InnerPlotPosition = new ElementPosition(5, 0, 100, 85),
                Name = "InvoiceAgingDataArea",
                BackColor = Color.Transparent
               
            };
            //chartArea.Area3DStyle.Enable3D = true;
            //chartArea.Area3DStyle.LightStyle = LightStyle.Realistic;
            //chartArea.Area3DStyle.Perspective = 10;
            //chartArea.AxisX.IsLabelAutoFit = false;
            //chartArea.AxisY.IsLabelAutoFit = false;
            //chartArea.AxisX.LabelStyle.Font =
            //    new Font("Verdana,Arial,Helvetica,sans-serif",
            //             8F,
            //             FontStyle.Regular);
            //chartArea.AxisX.LabelStyle.Angle = -90;
            //chartArea.AxisY.LabelStyle.Font =
            //   new Font("Verdana,Arial,Helvetica,sans-serif",
            //            8F,
            //            FontStyle.Regular);

            //chartArea.AxisY.LabelStyle.Format = "C";
            //chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.Interval = 1;
            //chartArea.AxisY.Interval = 1000.0;
            return chartArea;
        }


        private FileResult CreateFile(Chart chart)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                chart.SaveImage(memoryStream);
                chart.ChartAreas.Dispose();
                return File(memoryStream.ToArray(), "image/png");
            }
        }



        //hfhf

        public FileResult CallReceiveDate()
        {
            //IEnumerable<Technician> technicians =
            //    TempData.Peek("Technicians") as IEnumerable<Technician>;
            //if (technicians == null)
            //{
            //    return null;
            //}
            Chart chart = CreateJobClosingRateChart();
            return CreateFile(chart);
        }


        private static Chart CreateJobClosingRateChart()
        {
            Chart chart = new Chart
            {
                Width = 118,
                Height = 118,
                //BackColor = Color.FromArgb(211, 223, 240),
                //BackSecondaryColor = Color.WhiteSmoke,
                //BackGradientStyle = GradientStyle.DiagonalRight,
                //BorderlineDashStyle = ChartDashStyle.Solid,
                //BorderlineWidth = 1,
                //BorderlineColor = Color.FromArgb(26, 59, 105),
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
            //chart.Titles.Add(CreateTitle(mJobClosingRateTitle1));
            //foreach (Title title in CreateJobClosingRateTitles())
            //{
            //    chart.Titles.Add(title);
            //}
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



        private static List<Title> CreateJobClosingRateTitles()
        {
            List<Title> titles = new List<Title>();
            int i = 0;
            //foreach (Technician technician in technicians)
            //{
            titles.Add(new Title
            {
                //Text = "rajeev",
                DockedToChartArea = "JobArea" + ++i,
                Font = new Font("Trebuchet MS", 8F, FontStyle.Bold),
                DockingOffset = 50

            });
            //}
            return titles;
        }


        private static List<Series> CreateJobClosingRateSeries()
        {
            List<Series> seriesList = new List<Series>();
            int i = 0;
            //foreach (Technician technician in technicians)
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
                //series.Points[0].Label = "Friend,2";
                //series.Points[1].Label = "Meals,1";
                //series.Points[2].Label = "Postcard,1";
                //series.Points[3].Label = "Recall,2";
                //series.Points[4].Label = "Returning Customer,4";
                //series.Points[5].Label = "Truck,1";
                //series.Points[6].Label = "Unknown,7";
                //series.Points[7].Label = "Yellow Page,3";
                //series.Points[0]["Exploded"] = "true";
                //series.Points[1]["Exploded"] = "true";
                //series.Points[2]["Exploded"] = "true";
                //series.Points[3]["Exploded"] = "true";
                //series.Points[4]["Exploded"] = "true";
                //series.Points[5]["Exploded"] = "true";
                //series.Points[6]["Exploded"] = "true";
                //series.Points[7]["Exploded"] = "true";

                //series.Points[8]["Exploded"] = "true";

                seriesList.Add(series);
            }
            return seriesList;
        }


        private static List<ChartArea> CreateJobClosingRateAreas()
        {
            List<ChartArea> chartAreas = new List<ChartArea>();
            int i = 0;
            //foreach (Technician technician in technicians)
            //{
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



        private static Title CreateTitle(string text)
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



        private static Legend CreateTechniciansSalesAndDiscountLegend()
        {
            return new Legend
            {
                BackColor = Color.Transparent
            };
        }



        private static ChartArea CreateTechniciansSalesAndDiscountArea()
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


        private const string mTechniciansSalesAndDiscountTitle =
            "Technicians Sales and Discount";

        private const string mJobClosingRateTitle = "Job Closing Rate";

        private const string mTechniciansSalesAndDiscountTitle1 =
            "Legend";
        private const string mJobClosingRateTitle1 = "Call Receive Date";
    }
}
