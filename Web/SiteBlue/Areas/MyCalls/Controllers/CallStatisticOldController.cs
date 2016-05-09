using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Areas.SecurityGuard.Models;
using SiteBlue.Data.EightHundred;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.IO;
using SiteBlue.Controllers;
namespace SiteBlue.Areas.MyCalls.Controllers
{
    public class CallStatisticOldController : SiteBlueBaseController
    {
        //
        // GET: /MyCalls/CallStatistic/
        EightHundredEntities db = new EightHundredEntities();
        private MembershipConnection memberShipContext = new MembershipConnection();
        //static string DefaultCompamyName = default(String);
        public ActionResult Index(FormCollection formcollection)
        {

            if (User.Identity.Name != "")
            {
                Session["showInactive"] = true;

            }
            else
            {
                return new RedirectResult("/SGAccount/LogOn");
            }
        
            return View();


        }

        public ActionResult databyCompanyCode(string code)
        {
            try
            {
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                int FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();



                return PartialView("CallStatistic", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult UserCompanyCode()
        {
            IMembershipService membershipService;
            IAuthenticationService authenticationService;
            membershipService = new MembershipService(Membership.Provider);
            authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());

            MembershipUser user = membershipService.GetUser(User.Identity.Name);
            var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
            var isCorporate = User.IsInRole("Corporate");
            string username = user.UserName;
            int[] assignedFranchises;

            var DefaultCompamyName = default(String);
            var DefaultCompanyID = default(int);

            DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                  where g.FranchiseID == 56 && g.UserId == userId
                                  select g.Franchise.FranchiseNumber).FirstOrDefault();
            if (DefaultCompamyName == null)
            {
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
            }

            DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                where g.FranchiseID == 56 && g.UserId == userId
                                select g.Franchise.FranchiseID).FirstOrDefault();
            if (DefaultCompanyID == 0)
            {
                DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                    where g.UserId == userId
                                    select g.Franchise.FranchiseID).FirstOrDefault();
            }


            if (RouteData.Values["id"] != null)
            {
                int companyCodeID = int.Parse(Convert.ToString(RouteData.Values["id"]));
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == companyCodeID && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                DefaultCompanyID = (from g in memberShipContext.UserFranchise
                                    where g.FranchiseID == companyCodeID && g.UserId == userId
                                    select g.Franchise.FranchiseID).FirstOrDefault();
            }

            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == userId)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }

            var model = new GrantCompaniesToUser
            {
                UserName = username,
                GrantedCompanyCode =
                memberShipContext.MembershipFranchise
                               .Where(f => assignedFranchises.Contains(f.FranchiseID))
                               .OrderBy(f => f.FranchiseNumber)
                               .Select(d => new SelectListItem
                               {
                                   Text = d.FranchiseNumber,
                                   Value = System.Data.Objects.SqlClient.SqlFunctions.StringConvert((double)d.FranchiseID)
                               })
                               .ToList(),
                defaultCompanyName = DefaultCompamyName,
                defaultCompanyID = DefaultCompanyID
            };
            return PartialView("CompanyCodeUser", model);


        }






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
            string xCol = "#d5f1ff";
            Chart chart = new Chart
            {
                Width = 350,
                Height = 300,
                BackColor = System.Drawing.ColorTranslator.FromHtml(xCol),
                //BackSecondaryColor = Color.WhiteSmoke,
                //BackGradientStyle = GradientStyle.DiagonalRight,
                //BorderlineDashStyle = ChartDashStyle.Solid,
                //BorderlineWidth = 1,
                //BorderlineColor = Color.FromArgb(26, 59, 105),
                Palette = ChartColorPalette.BrightPastel,
                RenderType = RenderType.BinaryStreaming,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };
            chart.Titles.Add(CreateTitle(mTechniciansSalesAndDiscountTitle));
            chart.Legends.Add(CreateTechniciansSalesAndDiscountLegend());
            foreach (Series series in CreateInvoiceAgingSeries())
            {
                chart.Series.Add(series);
            }

            //chart.Legends.h;
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
                                Name = "Technician",
                                ChartType = SeriesChartType.Pie,
                                ChartArea = "InvoiceAgingDataArea",
                                 IsVisibleInLegend=false
                                
                            }
                        //new Series
                        //    {
                        //        //Name = "Actual",
                        //        //ChartType = SeriesChartType.StackedColumn,
                        //        //Color = Color.LightBlue,
                        //        //BorderColor = Color.Black,
                        //        //BorderWidth = 1,
                        //        //ChartArea = "TechniciansSalesAndDiscountArea"
                        //    },
                        //new Series
                        //    {
                        //        //Name = "Up-Sales",
                        //        //ChartType = SeriesChartType.StackedColumn,
                        //        //Color = Color.LightSteelBlue,
                        //        //BorderColor = Color.Black,
                        //        //BorderWidth = 1,
                        //        //ChartArea = "TechniciansSalesAndDiscountArea"
                        //    }
                    };
            const string drawingStyle = "Cylinder";
            seriesArray[0]["DrawingStyle"] = drawingStyle;
            //seriesArray[1]["DrawingStyle"] = drawingStyle;
            //seriesArray[2]["DrawingStyle"] = drawingStyle;
            const string pointWidth = "0.6";
            seriesArray[0]["PointWidth"] = pointWidth;
            //seriesArray[1]["PointWidth"] = pointWidth;
            //seriesArray[2]["PointWidth"] = pointWidth;

            //for(int i=1;i<=20;i++)
            //{
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "25%",

                YValues = new[] 
                                                            { 
                                                                (double)(25) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "20%",
                YValues = new[] 
                                                            { 
                                                                (double) (20) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "15%",

                YValues = new[] 
                                                            { 
                                                                (double) (15) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "10%",
                YValues = new[] 
                                                            { 
                                                                (double) (10) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "10%",
                YValues = new[] 
                                                            { 
                                                                (double) (10) 
 
                                                            }
            });

            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "20%",
                YValues = new[] 
                                                            { 
                                                                (double) (20) 
 
                                                            }
            });



            //seriesArray[1].Points.Add(new DataPoint
            //{
            //    XValue = 298,
            //    YValues = new[] 
            //                                            { 
            //                                                (double)( 655.00M )
            //                                            }
            //});
            //seriesArray[2].Points.Add(new DataPoint
            //{
            //    XValue = 283,
            //    YValues = new[] 
            //                                            { 
            //                                                (double) (1111.00M) 

            //                                            }
            //});
            // }

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
            chartArea.Area3DStyle.Enable3D = false;
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


            chartArea.AxisY.LabelStyle.Format = "{0;0}" + "%";
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Interval = 10;
            return chartArea;
        }
        private FileResult CreateFile(Chart chart)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                chart.SaveImage(memoryStream);
                return File(memoryStream.ToArray(), "image/png");
            }
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
        private const string mTechniciansSalesAndDiscountTitle =
            "";






        public FileResult Legend()
        {
            //IEnumerable<Technician> technicians =
            //    TempData.Peek("Technicians") as IEnumerable<Technician>;
            //if (technicians == null)
            //{
            //    return null;
            //}
            Chart chart = CreateLegend();

            return CreateFile(chart);
        }
        private static Chart CreateLegend()
        {
            string xCol = "#D5F1FF";
            Chart chart = new Chart
            {
                Width = 200,
                Height = 200,
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
            chart.Legends.Add(CreateLegend1());
            chart.Legends[0].Docking = Docking.Top;

            chart.Legends[0].LegendItemOrder = LegendItemOrder.ReversedSeriesOrder;
            chart.Legends[0].TableStyle = LegendTableStyle.Auto;

            //chart.Series[0].ChartType = SeriesChartType.Bar;
            //chart.Series[0]["PieLabelStyle"] = "Inside";
            //chart.Series[0]["PieLabelStyle"] = "Disabled";
            foreach (Series series in CreateLegendSeries())
            {
                chart.Series.Add(series);
            }
            chart.ChartAreas.Add(CreateLegendArea());

            return chart;
        }
        private static Series[] CreateLegendSeries()
        {
            Series[] seriesArray =
                new Series[]
                    {
                        new Series
                            {
                                Name = "Booked",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = System.Drawing.ColorTranslator.FromHtml("#418cf0"),
                                ChartArea = "InvoiceAgingDataArea"
                                
                            },
                            new Series
                            {
                                Name = "Call Back",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = System.Drawing.ColorTranslator.FromHtml("#fcb441"),
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Cancelled Call",
                                ChartType = SeriesChartType.StackedColumn,
                               Color = System.Drawing.ColorTranslator.FromHtml("#e0400a"),
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Corporate Transfer",
                                ChartType = SeriesChartType.StackedColumn,
                               Color = System.Drawing.ColorTranslator.FromHtml("#056492"),
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Hang Up",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = System.Drawing.ColorTranslator.FromHtml("#bfbfbf"),
                                ChartArea = "InvoiceAgingDataArea"
                            },
                            new Series
                            {
                                Name = "Not Booked",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = System.Drawing.ColorTranslator.FromHtml("#1a3b69"),
                                ChartArea = "InvoiceAgingDataArea"
                            }
                           
                            
                      
                    };

            return seriesArray;
        }
        private static ChartArea CreateLegendArea()
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
        private static Legend CreateLegend1()
        {
            return new Legend
            {
                BackColor = Color.Transparent
            };
        }

    }
}
