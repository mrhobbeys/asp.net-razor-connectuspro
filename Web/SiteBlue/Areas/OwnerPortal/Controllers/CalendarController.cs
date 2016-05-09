using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using SiteBlue.Areas.OwnerPortal.Models;
using SiteBlue.Areas.PriceBook.Models;
using System.Web.UI.DataVisualization.Charting;
using System.Threading;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    public class CalendarController : Controller
    {
        //
        // GET: /OwnerPortal/Calendar/

        Calendarmodel calendar;
        ViewDataDictionary vdd = new ViewDataDictionary();
        EightHundredEntities dbeight = new EightHundredEntities();
        private MembershipConnection memberShipContext = new MembershipConnection();
        public ActionResult Index(FormCollection formcollection)
        {
            Calendarmodel reportTechnicians = new Calendarmodel();
            if (User.Identity.Name != "")
            {
                int MyFranchiseID = 38;
                ViewBag.FrenchiseID = MyFranchiseID;
                List<tbl_Franchise> f = (from frn in dbeight.tbl_Franchise select frn).ToList();
                tbl_Franchise FranchiseName = (from franchise in dbeight.tbl_Franchise where franchise.FranchiseID == MyFranchiseID select franchise).FirstOrDefault();
                ViewBag.frenchise = f;
                Session["id"] = f;
                IMembershipService membershipService;
                IAuthenticationService authenticationService;
                membershipService = new MembershipService(Membership.Provider);
                authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());

                MembershipUser user = membershipService.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                string username = user.UserName;
                var DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.FranchiseID == 56 && g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();
                if (DefaultCompamyName == null)
                {
                    DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                          where g.UserId == userId
                                          select g.Franchise.FranchiseNumber).FirstOrDefault();
                }
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
        public ActionResult dec_charts(int id)
        {
            Calendarmodel calendar = new Calendarmodel
            {
                frachiseid = id

            };


            Session["partial"] = 1;
            return View("Index", calendar);


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
            if (DefaultCompamyName != null)
            {
                DefaultCompamyName = (from g in dbeight.tbl_Franchise
                                      where g.FranchiseNUmber == DefaultCompamyName
                                      select string.Concat(g.LegalName, " - ", g.FranchiseNUmber)).FirstOrDefault();
            }
            if (DefaultCompamyName == null)
            {
                DefaultCompamyName = (from g in memberShipContext.UserFranchise
                                      where g.UserId == userId
                                      select g.Franchise.FranchiseNumber).FirstOrDefault();

                DefaultCompamyName = (from g in dbeight.tbl_Franchise
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

            var model = new SiteBlue.Areas.SecurityGuard.Models.GrantCompaniesToUser
            {
                UserName = username,
                GrantedCompanyCode =
                dbeight.tbl_Franchise
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
            return PartialView("CalendarCompanyCodeUser", model);


        }

        public PartialViewResult calendarView(int id)
        {
            try
            {
                return PartialView("calendarView", id);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult databyCompanyCode(string code)
        {
            try
            {
                if (code.LastIndexOf("-") > 0)
                {
                    code = code.Substring(code.LastIndexOf("-") + 1).Trim();
                }
                var frenchID = (from g in memberShipContext.MembershipFranchise
                            where g.FranchiseNumber == code
                            select g.FranchiseID).FirstOrDefault();


                Calendarmodel calendar = new Calendarmodel
                {
                    FrenchiseID = frenchID,
                    frachiseid = frenchID

                };


                Session["partial"] = 1;
                return PartialView("calendarView", calendar);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public ActionResult PreviousDay(string code)
        {

            //if (month1 == DateTime.MinValue)
            //{
            //    System.DateTime today = System.DateTime.Now;

            //    System.DateTime month = today.AddMonths(-1);
            //    month1 = month;
            //}
            //else
            //{
            //    month1 = month1.AddMonths(-1);
            //}

            calendar = new Calendarmodel
            {
                //date = month1,
                //FrenchiseID = frenchID,
                //frachiseid = frenchID

            };
            //Session["month"] = month1;
            //Session["partial"] = 1;
            return PartialView("calendarView", calendar);
        }


        public ActionResult Previous()
        {
            //if (month1 == DateTime.MinValue)
            //{
            //    System.DateTime today = System.DateTime.Now;

            //    System.DateTime month = today.AddMonths(-1);
            //    month1 = month;
            //}
            //else
            //{
            //    month1 = month1.AddMonths(-1);
            //}

            calendar = new Calendarmodel
            {
            //    date = month1,
            //    FrenchiseID = frenchID,
            //    frachiseid = frenchID

            };
            //Session["month"] = month1;
            //Session["partial"] = 1;
            return PartialView("calendarView", calendar);
        }

        public ActionResult CurrentDay()
        {

            //System.DateTime today = System.DateTime.Now;
            calendar = new Calendarmodel
            {
            //    date = today,
            //    FrenchiseID = frenchID,
            //    frachiseid = frenchID

            };
            //month1 = System.DateTime.Now;
            //Session["month"] = today;
            //Session["partial"] = 1;
            return PartialView("calendarView", calendar);
        }

        public ActionResult Today()
        {

            //System.DateTime today = System.DateTime.Now;
            calendar = new Calendarmodel
            {
                //date = today,
                //FrenchiseID = frenchID,
                //frachiseid = frenchID


            };
            //month1 = System.DateTime.Now;
            //Session["month"] = today;
            //Session["partial"] = 2;
            return PartialView("calendarView", calendar);
        }

        public ActionResult Next()
        {

            //if (month1 == DateTime.MinValue)
            //{
            //    System.DateTime today = System.DateTime.Now;
            //    System.DateTime month = today.AddMonths(+1);
            //    month1 = month;
            //}
            //else
            //{
            //    month1 = month1.AddMonths(+1);
            //}

            calendar = new Calendarmodel
            {
                //date = month1,
                //frachiseid = frenchID,
                //FrenchiseID = frenchID


            };
            //Session["month"] = month1;
            //Session["partial"] = 2;
            return PartialView("calendarView", calendar);
        }

        public ActionResult NextDay()
        {

            //if (month1 == DateTime.MinValue)
            //{
            //    System.DateTime today = System.DateTime.Now;
            //    System.DateTime month = today.AddMonths(+1);
            //    month1 = month;
            //}
            //else
            //{
            //    month1 = month1.AddMonths(+1);
            //}

            calendar = new Calendarmodel
            {
                //date = month1,
                //FrenchiseID = frenchID,
                //frachiseid = frenchID


            };
            //Session["month"] = month1;
            //Session["partial"] = 1;
            return PartialView("calendarView", calendar);
        }
        public FileResult CallReceiveDate(int id, string dateday)
        {
            System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "MM-dd-yyyy";
            culture.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = culture;

            DateTime dt = Convert.ToDateTime(dateday);
            IEnumerable<Calendar> calendars = getchart(id, dt);
            Chart chart = CreateJobClosingRateChart(calendars);
            return CreateFile(chart);

        }

        private IList<Calendar> getchart(int id, DateTime date)
        {
            //DateTime dt = Convert.ToDateTime(date);
            var r = from tbl_calls in dbeight.tbl_Calls
                    join tbl_referral in dbeight.tbl_Referral on tbl_calls.ReferralID equals tbl_referral.referralID
                    where
                      tbl_calls.CallReceivedDate == date &&
                      tbl_calls.FranchiseID == id
                    group new { tbl_referral, tbl_calls } by new
                    {
                        tbl_referral.ReferralType
                    } into g
                    select new Calendar
                    {
                        Call_ReferralID = (Int32?)g.Count(p => p.tbl_calls.ReferralID != null),
                        Call_ReferralType = g.Key.ReferralType
                    };
            var list = r.ToList();
            return list;
        }
        private static Chart CreateJobClosingRateChart(IEnumerable<Calendar> calendars)
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

            Series series = CreateJobClosingRateSeries(calendars);

            chart.Series.Add(series);

            ChartArea area = CreateJobClosingRateAreas();

            chart.ChartAreas.Add(area);

            return chart;
        }






        private static Series CreateJobClosingRateSeries(IEnumerable<Calendar> calendars)
        {
            Series series = new Series
            {
                Name = "JobSeries",
                ChartType = SeriesChartType.Pie,
                ChartArea = "JobArea",

            };

            foreach (Calendar calendar in calendars)
            {

                series.Points.Add(new DataPoint
                {
                    YValues = new[]                                                     
{ 
                                                        (double)calendar.Call_ReferralID
                                                    }
                });

                series.Points[0].BackSecondaryColor = Color.White;
                series.Points[0].BackGradientStyle = GradientStyle.TopBottom;

            }
            return series;
        }

        private static ChartArea CreateJobClosingRateAreas()
        {
            ChartArea chartArea =
                new ChartArea
                {
                    InnerPlotPosition = new ElementPosition(0, 0, 100, 100),
                    Name = "JobArea",
                    BackColor = Color.Transparent
                };
            chartArea.Area3DStyle.Enable3D = true;
            chartArea.Area3DStyle.Inclination = 15;
            chartArea.Area3DStyle.Rotation = 90;
            return chartArea;
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


        private FileResult CreateFile(Chart chart)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                chart.SaveImage(memoryStream);
                return File(memoryStream.ToArray(), "image/png");
            }
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
            string xCol = "#D5F1FF";
            Chart chart = new Chart
            {
                Width = 800,
                Height = 355,
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

        private static Legend CreateTechniciansSalesAndDiscountLegend()
        {
            return new Legend
            {
                BackColor = Color.Transparent
            };
        }

    }
}
