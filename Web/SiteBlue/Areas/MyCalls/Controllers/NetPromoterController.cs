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
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using System.IO;
using SiteBlue.Data.EightHundred;
using SiteBlue.Controllers;
namespace SiteBlue.Areas.MyCalls.Controllers
{
    public class NetPromoterController : SiteBlueBaseController
    {
        //
        // GET: /MyCalls/NetPromoter/
        
        EightHundredEntities db = new EightHundredEntities();
        private MembershipConnection memberShipContext = new MembershipConnection();
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
        //public ActionResult databyCompanyCode(string code)
        //{
        //    try
        //    {
        //        if (code.LastIndexOf("-") > 0)
        //        {
        //            code = code.Substring(code.LastIndexOf("-") + 1).Trim();
        //        }
        //         FranchiseID = (from g in memberShipContext.MembershipFranchise
        //                           where g.FranchiseNumber == code
        //                           select g.FranchiseID).FirstOrDefault();



        //        return PartialView("NetPromoter", FranchiseID);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
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
            if (DefaultCompamyName != null)
            {
                DefaultCompamyName = (from g in db.tbl_Franchise
                                      where g.FranchiseNUmber == DefaultCompamyName
                                      select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
            }
            if (DefaultCompamyName == null)
            {
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();

                DefaultCompamyName = (from g in db.tbl_Franchise
                                      where g.FranchiseNUmber == DefaultCompamyName
                                      select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
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
                db.tbl_Franchise
                               .Where(f => assignedFranchises.Contains(f.FranchiseID))
                               .OrderBy(f => f.FranchiseNUmber)
                               .Select(d => new SelectListItem
                               {
                                   Text = string.Concat(d.LegalName, " - ", d.FranchiseNUmber),
                                   Value = System.Data.Objects.SqlClient.SqlFunctions.StringConvert((double)d.FranchiseID)
                               })
                               .ToList(),
                defaultCompanyName = DefaultCompamyName,
                defaultCompanyID = DefaultCompanyID
            };
            return PartialView("CompanyCodeUser", model);


        }
        public ActionResult Index2()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = db.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;

                if (Session["code"] == null)
                {
                    ViewBag.code = DefaultCompamyName;
                    Session["code"] = null;
                }
            }
            else
            {

                return new RedirectResult("/SGAccount/LogOn");

            }
            return View(reportTechnicians);


        }
        public ActionResult databyCompanyCode2(string code)
        {
            try
            {
                int FranchiseID = (from g in memberShipContext.MembershipFranchise
                                   where g.FranchiseNumber == code
                                   select g.FranchiseID).FirstOrDefault();



                return PartialView("NetPromoter2", FranchiseID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult NetPromoterScore()
        {
            //DateTime dt = Convert.ToDateTime(txt_refCode1);
            //DateTime dt1 = Convert.ToDateTime(txt_refCode2);
            //var detractors = (from a in db.tbl_Job_HappyScores
            //                  where
            //                      a.FranchiseID == FranchiseID &&
            //                      a.JobDate >= dt && a.JobDate <= dt1 &&
            //                      a.ScoreValue >= 0 && a.ScoreValue <= 6
            //                  select a.ScoreValue).Average();
            //var promoter = (from a in db.tbl_Job_HappyScores
            //                where
            //                    a.FranchiseID == FranchiseID &&
            //                    a.JobDate >= dt && a.JobDate <= dt1 &&
            //                    a.ScoreValue >= 9 && a.ScoreValue <= 10
            //                select a.ScoreValue).Average();

            //decimal Totalrespondents = Convert.ToDecimal(promoter) + Convert.ToDecimal(detractors);
            //var technicians = (from g in db.tbl_Employee
            //                   where g.FranchiseID == FranchiseID
            //                   select g.Employee).ToList();

            return View();
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
                Width = 600,
                Height = 500,
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
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
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
                AxisLabel = "Tom",

                YValues = new[] 
                                                            { 
                                                                (double)(43) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Jim",
                YValues = new[] 
                                                            { 
                                                                (double) (39) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Jason",

                YValues = new[] 
                                                            { 
                                                                (double) (51) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Les",
                YValues = new[] 
                                                            { 
                                                                (double) (92) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "James",
                YValues = new[] 
                                                            { 
                                                                (double) ( 58) 
 
                                                            }
            });

            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Mark",
                YValues = new[] 
                                                            { 
                                                                (double) (43) 
 
                                                            }
            });

            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Kevin",
                YValues = new[] 
                                                            { 
                                                                (double) ( 37) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Total Company",
                YValues = new[] 
                                                            { 
                                                                (double) (47) 
 
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





        public FileResult InvoiceAging1()
        {
            //IEnumerable<Technician> technicians =
            //    TempData.Peek("Technicians") as IEnumerable<Technician>;
            //if (technicians == null)
            //{
            //    return null;
            //}
            Chart chart = CreateInvoiceAging1();
            return CreateFile(chart);
        }
        private static Chart CreateInvoiceAging1()
        {
            string xCol = "#d5f1ff";
            Chart chart = new Chart
            {
                Width = 600,
                Height = 500,
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
            foreach (Series series in CreateInvoiceAgingSeries1())
            {
                chart.Series.Add(series);
            }

            //chart.Legends.h;
            chart.ChartAreas.Add(CreateTechniciansSalesAndDiscountArea2());
            return chart;
        }
        private static Series[] CreateInvoiceAgingSeries1()
        {
            Series[] seriesArray =
                new Series[]
                    {
                        new Series
                            {
                                Name = "Technician",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
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
                AxisLabel = "Les",

                YValues = new[] 
                                                            { 
                                                                (double)(95) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Json",
                YValues = new[] 
                                                            { 
                                                                (double) (60) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Jim",

                YValues = new[] 
                                                            { 
                                                                (double) (80) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Mark",
                YValues = new[] 
                                                            { 
                                                                (double) (55) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Kevin",
                YValues = new[] 
                                                            { 
                                                                (double) ( 66) 
 
                                                            }
            });

            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Tom",
                YValues = new[] 
                                                            { 
                                                                (double) (86) 
 
                                                            }
            });

            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "James",
                YValues = new[] 
                                                            { 
                                                                (double) ( 70) 
 
                                                            }
            });
            seriesArray[0].Points.Add(new DataPoint
            {
                AxisLabel = "Total Company",
                YValues = new[] 
                                                            { 
                                                                (double) (74) 
 
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
        private static ChartArea CreateTechniciansSalesAndDiscountArea2()
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






        public FileResult InvoiceAging2()
        {
            //IEnumerable<Technician> technicians =
            //    TempData.Peek("Technicians") as IEnumerable<Technician>;
            //if (technicians == null)
            //{
            //    return null;
            //}
            Chart chart = CreateInvoiceAging2();
            return CreateFile(chart);
        }
        private static Chart CreateInvoiceAging2()
        {
            string xCol = "#d5f1ff";
            Chart chart = new Chart
            {
                Width = 600,
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
            foreach (Series series in CreateInvoiceAgingSeries2())
            {
                chart.Series.Add(series);

            }

            //chart.Legends.h;
            chart.ChartAreas.Add(CreateTechniciansSalesAndDiscountArea3());
            return chart;
        }
        private static Series[] CreateInvoiceAgingSeries2()
        {
            Series[] seriesArray =
                new Series[]
                    {
                        new Series
                            {
                                Name = "Technician",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.Blue,
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "InvoiceAgingDataArea",
                                IsVisibleInLegend=false,
                                IsValueShownAsLabel=true,
                                LabelForeColor=Color.White,
                                Label="95%" 
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
                AxisLabel = "",

                YValues = new[] 
                                                            { 
                                                                (double)(95) 
 
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
        private static ChartArea CreateTechniciansSalesAndDiscountArea3()
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



    }
}
