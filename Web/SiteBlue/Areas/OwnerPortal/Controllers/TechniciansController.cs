using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using System.Collections.Generic;
using System.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Data.Entity;
using System.Web.UI.DataVisualization.Charting;
using SiteBlue.Areas.PriceBook.Models;
using System.Threading;
using SecurityGuard.Services;
using System.Web.Security;
using SiteBlue.Business.Reporting;
using SiteBlue.Data.EightHundred;
using SiteBlue.Controllers;
using DHTMLX.Export.Excel;
using SiteBlue.Business;
using SiteBlue.Business.Job;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    //[OutputCache(NoStore = true, Duration = 60, VaryByParam = "*")]
    public class TechniciansController : SiteBlueBaseController
    {
        ViewDataDictionary vdd = new ViewDataDictionary();
        private MembershipConnection memberShipContext = new MembershipConnection();
        EightHundredEntities dbeight = new EightHundredEntities();
        private readonly MembershipService membership = new MembershipService(Membership.Provider);
       
       
        private const string DispatchRoleName = "Dispatcher";
        private MembershipUser _currentUser;


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            _currentUser = _currentUser ?? membership.GetUser(User.Identity.Name);
        }

        public ViewResult Index()
        {
            return View();

        }

        public ViewResult Test(FormCollection fc)
        {
            string s1 = fc["txtArea"];
            return View();
        }

        public JsonResult UpdateSchedule(int scheduleId, string monStartTime, string monEndTime, string tueStartTime, string tueEndTime, string wedStartTime, string wedEndTime,
          string thruStartTime, string thruEndTime, string friStartTime, string friEndTime, string satStartTime, string satEndTime, string sunStartTime, string sunEndTime)
        {

            using (var context = new AuditedJobContext(UserInfo.UserKey, UserInfo.User.UserName, false))
            {
                var Schedule = (from p in context.tbl_Dispatch_Schedule where p.ScheduleID == scheduleId select p).Single();
                if (Schedule != null)
                {
                    if (!string.IsNullOrWhiteSpace(sunStartTime))
                    {
                        Schedule.SunStart = sunStartTime;
                    }
                    else
                    {
                        Schedule.SunStart = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(sunEndTime))
                    {
                        Schedule.SunEnd = sunEndTime;
                    }
                    else
                    {
                        Schedule.SunEnd = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(monStartTime))
                    {
                        Schedule.MonStart = monStartTime;
                    }
                    else
                    {
                        Schedule.MonStart = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(monEndTime))
                    {
                        Schedule.MonEnd = monEndTime;
                    }
                    else
                    {
                        Schedule.MonEnd = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(tueStartTime))
                    {
                        Schedule.TueStart = tueStartTime;
                    }
                    else
                    {
                        Schedule.TueStart = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(tueEndTime))
                    {
                        Schedule.TueEnd = tueEndTime;
                    }
                    else
                    {
                        Schedule.TueEnd = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(wedStartTime))
                    {
                        Schedule.WedStart = wedStartTime;
                    }
                    else
                    {
                        Schedule.WedStart = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(wedEndTime))
                    {
                        Schedule.WedEnd = wedEndTime;
                    }
                    else
                    {
                        Schedule.WedEnd = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(thruStartTime))
                    {
                        Schedule.ThuStart = thruStartTime;
                    }
                    else
                    {
                        Schedule.ThuStart = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(thruEndTime))
                    {
                        Schedule.ThuEnd = thruEndTime;
                    }
                    else
                    {
                        Schedule.ThuEnd = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(friStartTime))
                    {
                        Schedule.FriStart = friStartTime;
                    }
                    else
                    {
                        Schedule.FriStart = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(friEndTime))
                    {
                        Schedule.FriEnd = friEndTime;
                    }
                    else
                    {
                        Schedule.FriEnd = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(satStartTime))
                    {
                        Schedule.SatStart = satStartTime;
                    }
                    else
                    {
                        Schedule.SatStart = "Off";
                    }
                    if (!string.IsNullOrWhiteSpace(satEndTime))
                    {
                        Schedule.SatEnd = satEndTime;
                    }
                    else
                    {
                        Schedule.SatEnd = "Off";
                    }
                    context.SaveChanges();
                    return Json(new
                    {
                        Message = "Record Updated Successfully.",
                        ResultData = "",
                        Success = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        Message = string.Format("There is no schedule exists for ID: {0}.", scheduleId),
                        ResultData = "",
                        Success = false
                    });
                }
            }
        }

        public ActionResult TechSchedule()
        {
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;

                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = dbeight.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;
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
            return View();
        }

        public ActionResult GetTechSchedule(int id)
        {
            try
            {
                var tech = (from ff in dbeight.tbl_Dispatch_Schedule
                            join m in dbeight.tbl_Employee on ff.TechID equals m.EmployeeID
                            where ff.FranchiseID == id
                            select new
                            {
                                SchudleId = ff.ScheduleID,
                                Employee = m.Employee,
                                SunStart = ff.SunStart,
                                SunEnd = ff.SunEnd,
                                MonStart = ff.MonStart,
                                MonEnd = ff.MonEnd,
                                TueStart = ff.TueStart,
                                TueEnd = ff.TueEnd,
                                WedStart = ff.WedStart,
                                WedEnd = ff.WedEnd,
                                ThuStart = ff.ThuStart,
                                ThuEnd = ff.ThuEnd,
                                FriStart = ff.FriStart,
                                FriEnd = ff.FriEnd,
                                SatStart = ff.SatStart,
                                SatEnd = ff.SatEnd
                            }).Take(1000);

                return Json(tech);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ViewResult DailyBudget(int? frid, FormCollection formcollection)
        {
            var franchiseId = frid ?? UserInfo.CurrentFranchise.FranchiseID;

            DateTime todaydate;
            if (!DateTime.TryParse(formcollection["txtDate"], out todaydate))
                todaydate = DateTime.Today;

            var m = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetBudget(franchiseId, todaydate);
            return View(m);
        }

        public ActionResult PieChart(decimal dailyvalue, decimal budgetvalue, string ChartType)
        {
            var data = new Dictionary<string, decimal> { };
            if (ChartType == "Budget")
            {
                dailyvalue = 100 - budgetvalue;
                data = new Dictionary<string, decimal>
                {
                    {"A", dailyvalue},
                    {"B", budgetvalue}
                
                };
            }
            if (ChartType == "Daily" || ChartType == "Monthly")
            {
                budgetvalue = 100 - dailyvalue;
                data = new Dictionary<string, decimal>
                {
                    {"A", dailyvalue},
                    {"B", budgetvalue}
                
                };

            }

            var chart = new Chart { Width = 500, Height = 400 };
            var area = new ChartArea
                           {
                               InnerPlotPosition = new ElementPosition(5, 0, 100, 85),
                               Name = "Chart",
                               BackColor = Color.Transparent,
                               Area3DStyle =
                                   {
                                       Enable3D = true,
                                       LightStyle = LightStyle.Realistic,
                                       Perspective = 10
                                   },
                               AxisX = { IsLabelAutoFit = false },
                               AxisY = { IsLabelAutoFit = false }
                           };
            area.AxisX.LabelStyle.Font =
                new Font("Verdana,Arial,Helvetica,sans-serif",
                         40F,
                         FontStyle.Regular);

            area.AxisX.LabelStyle.Angle = 0;
            area.AxisY.LabelStyle.Angle = 90;
            area.AxisY.LabelStyle.Font =
               new Font("Verdana,Arial,Helvetica,sans-serif",
                        40F,
                        FontStyle.Regular);

            area.AxisY.LabelStyle.Format = "C";
            area.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            area.AxisX.Interval = 1;
            area.AxisY.Interval = 400;
            area.AxisY.LabelStyle.Format = "{0}";
            chart.ChartAreas.Add(area);

            // create and customize your data series.
            var series = new Series();
            foreach (var item in data)
            {
                series.Points.AddXY(item.Key, item.Value);

            }
            series.Color = Color.FromArgb(177, 177, 177);

            series.LabelForeColor = Color.FromArgb(0, 102, 204);

            var title = new Title
                            {
                                Text = ChartType.ToUpper(),
                                Font =
                                    new Font("Courier New,Courier,monospace", 40F,
                                                            FontStyle.Bold),
                                ForeColor = Color.FromArgb(56, 111, 179)
                            };

            chart.Titles.Add(title);
            chart.Palette = ChartColorPalette.None;

            if (ChartType == "Budget")
            {
                chart.PaletteCustomColors = new[]
                                            {
                                                Color.FromArgb(215, 148, 146),
                                                Color.FromArgb(215, 215, 215)
                                            };




                series.Points[0].Label = "      ";
                series.Points[1].Label = "#PERCENT{P0}";

            }

            if (ChartType == "Daily" || ChartType == "Monthly")
            {
                chart.PaletteCustomColors = new[]
                                            {
                                                Color.FromArgb(193,212,153),
                                                Color.FromArgb(215, 148, 146)
                                            };
                series.Points[0].Label = "#PERCENT{P0}";
                series.Points[1].Label = "        ";

            }

            series.Font = new Font("Segoe UI", 40F, FontStyle.Bold);
            series.ChartType = SeriesChartType.Pie;
            series.BorderColor = Color.FromArgb(64, 64, 64, 64);
            series["PieLabelStyle"] = "Inside";

            const string drawingStyle = "Cylinder";
            series["DrawingStyle"] = drawingStyle;
            chart.Series.Add(series);

            var returnStream = new MemoryStream();
            chart.ImageType = ChartImageType.Bmp;
            chart.SaveImage(returnStream);
            returnStream.Position = 0;
            return new FileStreamResult(returnStream, "image/bmp");
        }

        public ActionResult BarChart(decimal dailyvalue, decimal budgetvalue, bool SalesChart)
        {
            var data = new Dictionary<string, decimal>
            {
                {"B", budgetvalue},
                {"A", dailyvalue}
                
            };

            var chart = new Chart { Width = 200, Height = 110 };
            var area = new ChartArea
                           {
                               InnerPlotPosition = new ElementPosition(5, 0, 100, 85),
                               Name = "Chart",
                               BackColor = Color.Transparent,
                               Area3DStyle =
                                   {
                                       Enable3D = false,
                                       LightStyle = LightStyle.Realistic,
                                       Rotation = 10,
                                       Perspective = 10
                                   },
                               AxisX = { IsLabelAutoFit = false },
                               AxisY = { IsLabelAutoFit = false }
                           };

            area.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            area.AxisX.LabelStyle.Angle = 0;
            area.AxisY.LabelStyle.Angle = 0;
            area.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            area.AxisY.LabelStyle.Format = "C";

            area.AxisY.LineColor = Color.White;
            area.AxisX.LineColor = Color.White;
            area.AxisY.MajorGrid.LineColor = Color.White;
            area.AxisX.MajorGrid.LineColor = Color.White;
            area.AxisX.Interval = 1;

            var intervalval = dailyvalue > budgetvalue
                                  ? dailyvalue
                                  : budgetvalue;


            area.AxisY.Interval = Convert.ToDouble(Math.Round(intervalval) / 3);
            area.AxisY.LabelStyle.Format = "{0}";

            area.AxisY.LabelStyle.ForeColor = Color.White;
            area.AxisX.LabelStyle.ForeColor = Color.White;

            area.AxisX.LabelStyle.IsEndLabelVisible = false;
            area.AxisY.LabelStyle.IsEndLabelVisible = false;

            area.AxisY.MajorTickMark.LineColor = Color.White;
            area.AxisX.MajorTickMark.LineColor = Color.White;

            chart.ChartAreas.Add(area);

            // create and customize your data series.
            var series = new Series();
            foreach (var item in data)
            {
                series.Points.AddXY(item.Key, item.Value);
            }

            series.Points[1].Color = Color.FromArgb(193, 212, 153);
            series.Points[0].Color = Color.FromArgb(244, 244, 244);

            series.Label = SalesChart ? "#VALY{C0}" : "#VALY";
            series.LabelForeColor = Color.FromArgb(0, 102, 204);

            series.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8.0f, FontStyle.Bold);
            series.ChartType = SeriesChartType.Bar;

            series["BarLabelStyle"] = "Right";

            const string drawingStyle = "Cylinder";
            series["DrawingStyle"] = drawingStyle;
            chart.Series.Add(series);

            var returnStream = new MemoryStream();
            chart.ImageType = ChartImageType.Png;
            chart.SaveImage(returnStream);
            returnStream.Position = 0;

            return new FileStreamResult(returnStream, "image/png");
        }

        public ActionResult Customer()
        {
            return View();
        }

        #region marketing

        [HttpGet]
        public ActionResult Marketing()
        {
            return View();
        }

        #region Referral Code

        public ActionResult GetReferralCode(int frId)
        {
            var referral_list = (from r in dbeight.tbl_Referral
                                 where r.FranchiseID == frId
                                 select new
                                 {
                                     referralID = r.referralID,
                                     ReferralType = r.ReferralType,
                                     activeYN = r.activeYN
                                 }).ToList();

            return Json(referral_list);
        }

        [HttpPost]
        public ActionResult SaveReferralCode(tbl_Referral[] datas)
        {
            int frid = datas[0].FranchiseID;

            for (int i = 0; i < datas.Count(); i++)
            {
                if (datas[i].referralID == 0)
                {
                    dbeight.tbl_Referral.AddObject(datas[i]);
                    dbeight.SaveChanges();
                }
                else
                {
                    int refid = datas[i].referralID;
                    var data = dbeight.tbl_Referral.First(q => q.referralID == refid);

                    data.ReferralType = datas[i].ReferralType;
                    data.activeYN = datas[i].activeYN;

                    dbeight.SaveChanges();
                }
            }

            var referral_list = (from t in dbeight.tbl_Referral
                                 where t.FranchiseID == frid
                                 select new
                                 {
                                     ReferralType = t.ReferralType,
                                     ActiveYN = t.activeYN,
                                     ReferralID = t.referralID
                                 }).ToList();

            return Json(referral_list);
        }

        #endregion

        #region Coupon Code

        public ActionResult GetCouponCode(int frId)
        {
            var coupon = dbeight.tbl_coupon.Where(q => q.FranchiseID == frId).ToList();

            var coupon_list = from c in coupon
                              select new
                              {
                                  CouponID = c.CouponID,
                                  Code = c.Code,
                                  DiscountAmount = c.DiscountAmount,
                                  Percentage = c.Percentage,
                                  MarketingMedium = c.MarketingMedium,
                                  ExpirationDate = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToShortDateString() : ""
                              };

            return Json(coupon_list);
        }

        public ActionResult GetCouponAdd(string code, string medium, string exDate, string frid)
        {
            tbl_coupon Addcoupon = new tbl_coupon();

            if (frid != "" || frid != null)
            {
                if (exDate != "")
                {
                    Addcoupon.ExpirationDate = Convert.ToDateTime(exDate);
                }
                else
                {
                    Addcoupon.ExpirationDate = DateTime.Now;
                }
                int frId = Convert.ToInt32(frid);
                Addcoupon.Code = code;
                Addcoupon.MarketingMedium = medium;
                Addcoupon.FranchiseID = frId;
                dbeight.tbl_coupon.AddObject(Addcoupon);
                dbeight.SaveChanges();
            }

            return Json(string.Empty);
        }

        public ActionResult SaveCouponCode(tbl_coupon[] datas)
        {
            int? frId = datas[0].FranchiseID;

            for (int i = 0; i < datas.Count(); i++)
            {
                var code = datas[i].Code;

                if (datas[i].CouponID == 0)
                {
                    if (dbeight.tbl_coupon.Any(q => q.Code == code))
                        return Json("duplicate");
                }
                else
                {
                    int cid = datas[i].CouponID;

                    if (dbeight.tbl_coupon.Any(q => q.Code == code && q.CouponID != cid))
                        return Json("duplicate");
                }
            }

            for (int i = 0; i < datas.Count(); i++)
            {
                if (datas[i].CouponID == 0)
                {
                    dbeight.tbl_coupon.AddObject(datas[i]);
                    dbeight.SaveChanges();
                }
                else
                {
                    int cid = datas[i].CouponID;

                    tbl_coupon data = dbeight.tbl_coupon.First(q => q.CouponID == cid);

                    data.Code = datas[i].Code;
                    data.MarketingMedium = datas[i].MarketingMedium;
                    data.DiscountAmount = datas[i].DiscountAmount;
                    data.Percentage = datas[i].Percentage;
                    data.ExpirationDate = datas[i].ExpirationDate;

                    dbeight.SaveChanges();
                }
            }

            var coupon = dbeight.tbl_coupon.Where(q => q.FranchiseID == frId).ToList();

            var coupon_list = from c in coupon
                              select new
                              {
                                  CouponID = c.CouponID,
                                  Code = c.Code,
                                  DiscountAmount = c.DiscountAmount,
                                  Percentage = c.Percentage,
                                  MarketingMedium = c.MarketingMedium,
                                  ExpirationDate = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToShortDateString() : ""
                              };

            return Json(coupon_list);
        }

        #endregion

        #region Referral Tracking

        public ActionResult GetReferralTracking(DateTime fromdate, DateTime todate, int frId)
        {
            var referrallist = (from j in dbeight.tbl_Job
                                join l in dbeight.tbl_Locations on j.LocationID equals l.LocationID
                                join C in dbeight.tbl_Customer on j.CustomerID equals C.CustomerID
                                join S in dbeight.tbl_Referral on j.CallSourceID equals S.referralID
                                where j.FranchiseID == frId && j.CallCompleted >= fromdate
                                && j.CallCompleted <= todate
                                group j by new { j.CallSourceID, S.ReferralType }
                                    into grp
                                    select new
                                    {
                                        SumSubTotal = grp.Sum(j => j.SubTotal),
                                        AvgSubTotal = grp.Average(j => j.SubTotal),
                                        grp.Key.ReferralType,
                                        grp.Key.CallSourceID,
                                        jobCount = grp.Count()
                                    }).ToList();

            return Json(referrallist);
        }

        public ActionResult GetCouponTracking(DateTime fromdate, DateTime todate, int frId)
        {
            var coupon = dbeight.tbl_coupon.Where(q => q.ExpirationDate >= fromdate && q.ExpirationDate <= todate && q.FranchiseID == frId).ToList();

            var couponlist = from c in coupon.ToList()
                             select new
                             {
                                 Code = c.Code,
                                 MarketingMedium = c.MarketingMedium,
                                 DiscountAmount = c.DiscountAmount,
                                 Percentage = c.Percentage,
                                 ExpirationDate = c.ExpirationDate.HasValue ? c.ExpirationDate.Value.ToShortDateString() : "",
                                 CouponID = c.CouponID
                             };

            return Json(couponlist);
        }

        #endregion

        #endregion

        public PartialViewResult schedule(string code)
        {
            if (code.LastIndexOf("-") > 0)
            {
                code = code.Substring(code.LastIndexOf("-") + 1).Trim();
            }
            var franchise_id = (from g in memberShipContext.MembershipFranchise
                                where g.FranchiseNumber == code
                                select g.FranchiseID).FirstOrDefault();

            return PartialView("schedule", franchise_id);
            ;
        }

        public ActionResult TechniciansNew()
        {
            List<Technician> reportTechnicians = new List<Technician>();
            if (User.Identity.Name != "")
            {
                var membership = new MembershipService(Membership.Provider);
                var user = membership.GetUser(User.Identity.Name);
                var userId = user == null ? Guid.Empty : (Guid)(user.ProviderUserKey ?? Guid.Empty);
                var isCorporate = User.IsInRole("Corporate");
                int[] assignedFranchises;

                using (var ctx = new MembershipConnection())
                {
                    assignedFranchises = ctx.UserFranchise
                                            .Where(uf => uf.UserId == userId)
                                            .Select(f => f.FranchiseID)
                                            .ToArray();
                }

                var franchises = dbeight.tbl_Franchise
                                   .Where(f => isCorporate || assignedFranchises.Contains(f.FranchiseID))
                                   .OrderBy(f => f.FranchiseNUmber)
                                   .Select(d => new { d.FranchiseID, Name = string.Concat(d.FranchiseNUmber, " - ", d.LegalName) })
                                   .ToArray();


                ViewBag.frenchise = franchises;
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

        public FileResult CallReceiveDate()
        {

            Chart chart = CreateJobClosingRateChart();
            return CreateFile(chart);
        }

        private FileResult CreateFile(Chart chart)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                chart.SaveImage(memoryStream);
                return File(memoryStream.ToArray(), "image/png");
            }
        }

        private static Chart CreateJobClosingRateChart()
        {
            Chart chart = new Chart
            {
                Width = 128,
                Height = 128,
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
Color.FromArgb(100, 100, 0, 200),
//Color.FromArgb(100, 100, 0, 200),
//Color.FromArgb(200, 50, 0, 30),

};
            //chart.Titles.Add(CreateTitle(mJobClosingRateTitle1));
            //foreach (Title title in CreateJobClosingRateTitles())
            //{
            // chart.Titles.Add(title);
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

        private static Series[] CreateInvoiceAgingSeries()
        {
            Series[] seriesArray =
            new Series[]
{

new Series
{
Name = "returnu",
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
Name = "Chamblee Coupon",
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


};

            return seriesArray;
        }

        private static List<Series> CreateJobClosingRateSeries()
        {


            List<Series> seriesList = new List<Series>();
            int i = 0;
            //foreach (Technician technician in technicians)
            {
                Series series = new Series
                {
                    Name = "InvoiceAgingDataArea" + ++i,
                    ChartType = SeriesChartType.Pie,
                    ChartArea = "InvoiceAgingDataArea" + i

                };
                series.Points.Add(new DataPoint
                {
                    YValues = new[]
{
(double)(30.00)
}
                });
                series.Points.Add(new DataPoint
                {
                    YValues = new[]
{
(double)(24.00)
}
                });
                series.Points.Add(new DataPoint
                {
                    YValues = new[]
{
(double)(12.00),

}
                });


                series.Points.Add(new DataPoint
                {
                    YValues = new[]
{
(double)(24.00)
}
                });
                series.Points.Add(new DataPoint
                {
                    YValues = new[]
{
(double)(20.00)
}
                });

                series.Points.Add(new DataPoint
                {
                    YValues = new[]
{
(double)(40.00)
}
                });
                //series.Points.Add(new DataPoint
                //{
                // YValues = new[]
                // {
                // (double)(7.00)
                // }
                //});
                //series.Points.Add(new DataPoint
                //{
                // YValues = new[]
                // {
                // (double)(3.00)
                // }
                //});
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
                //series.Points[6].BackSecondaryColor = Color.White;
                //series.Points[6].BackGradientStyle = GradientStyle.TopBottom;
                //series.Points[7].BackSecondaryColor = Color.White;
                //series.Points[7].BackGradientStyle = GradientStyle.TopBottom;
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
            //return seriesList;
            Series[] seriesArray =
            new Series[]
{

new Series
{
Name = "returnu",
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
Name = "Chamblee Coupon",
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


};

            //return seriesArray;
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
                Name = "InvoiceAgingDataArea" + ++i,
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

        public ActionResult GetreffAdd(string refType, string refActive)
        {
            int code1 = Convert.ToInt32(refType);
            Referral referrel = new Referral();
            var ref1 = (from p in dbeight.tbl_Referral where p.referralID == code1 select p).Single();
            ref1.activeYN = Convert.ToBoolean(refActive);
            dbeight.SaveChanges();
            var test = "";
            Session["settab"] = "active";
            return Json(test);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadReferralCode()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "referralcode.xlsx");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadCouponCode()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "couponcode.xlsx");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadReferralTracking()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "referraltracking.xlsx");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadCouponTracking()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "coupontracking.xlsx");
        }
    }
}
