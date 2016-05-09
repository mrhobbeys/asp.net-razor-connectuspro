using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using SiteBlue.Areas.OwnerPortal.Models;
using SiteBlue.Business;
using SiteBlue.Business.Reporting;
using SiteBlue.Controllers;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.OwnerPortal.Controllers
{
    [Authorize(Roles = "CompanyOwner,Corporate")]
    [OutputCache(Duration = 1)]
    public class KpiController : SiteBlueBaseController
    {
        #region "Charts"
        private const string MTechniciansSalesAndDiscountTitle =
            "Average Ticket";
        private const string MJobClosingRateTitle = "Job Closing Rate";

        public FileResult TechniciansSalesAndDiscountChart(int franchiseId, DateTime startDate, DateTime endDate)
        {
            var model = GetModel(franchiseId, startDate, endDate, false, false);
            var technicians = model.TechnicianList;
            if (technicians == null) return null;
            
            var chart = CreateTechniciansSalesAndDiscountChart(technicians, model.Total_JobsAVG);
            var stripLine1 = new StripLine();
            chart.Width = 590;
            
            // Set threshold line so that it is only shown once
            stripLine1.Interval = 0;
            stripLine1.IntervalOffset = Convert.ToDouble(TempData["AvgTicket"]);
            stripLine1.BackColor = Color.Red;
            stripLine1.StripWidth = 1;
          
            // Set text properties for the threshold line
            stripLine1.ForeColor = Color.Black;
            stripLine1.TextOrientation = TextOrientation.Horizontal;
            
            // Add strip line to the chart
            chart.ChartAreas[0].AxisY.StripLines.Add(stripLine1);
            return CreateFile(chart);
        }
        
        public FileResult JobClosingRateChart(int franchiseId, DateTime startDate, DateTime endDate)
        {
            var technicians = GetModel(franchiseId, startDate, endDate, false, false).TechnicianList;
            return technicians == null ? null : CreateFile(CreateJobClosingRateChart(technicians));
        }

        private FileResult CreateFile(Chart chart)
        {
            using (var memoryStream = new MemoryStream())
            {
                chart.SaveImage(memoryStream);
                return File(memoryStream.ToArray(), "image/png");
            }
        }

        private static Chart CreateTechniciansSalesAndDiscountChart(IEnumerable<Technician> technicians,decimal avg)
        {
            var chart = new Chart
            {
                Width = 700,
                Height = 700,
                BackColor = Color.White,
                BackSecondaryColor = Color.White,
                BackGradientStyle = GradientStyle.DiagonalRight,
                BorderlineDashStyle = ChartDashStyle.Solid,
                BorderlineWidth = 1,
                BorderlineColor = Color.FromArgb(26, 59, 105),
                Palette = ChartColorPalette.None,
                RenderType = RenderType.BinaryStreaming,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High
            };
            
            chart.Titles.Add(CreateTitle(MTechniciansSalesAndDiscountTitle));
            chart.Legends.Add(CreateTechniciansSalesAndDiscountLegend());

            foreach (var series in CreateTechniciansSalesAndDiscountSeries(technicians))
            {
                chart.Series.Add(series);
            }

            chart.Series.Add(new Series { Name = "Heading", LegendText = "Company Avg. (" + string.Format("{0:C}", Math.Round(avg)).Replace(".00", "") + ")", Color = Color.Red });
            chart.ChartAreas.Add(CreateTechniciansSalesAndDiscountArea());
           
            chart.Series["Heading"].IsVisibleInLegend = true;
            chart.Series["test"].IsVisibleInLegend = false;
            
            return chart;
        }

        private static Chart CreateJobClosingRateChart(IEnumerable<Technician> technicians)
        {
            var chart = new Chart
                            {
                                Width = 400,
                                Height = 600,
                                BackColor = Color.White,
                                BackSecondaryColor = Color.White,
                                BackGradientStyle = GradientStyle.DiagonalRight,
                                BorderlineDashStyle = ChartDashStyle.Solid,
                                BorderlineWidth = 1,
                                BorderlineColor = Color.FromArgb(26, 59, 105),
                                Palette = ChartColorPalette.None,
                                RenderType = RenderType.BinaryStreaming,
                                AntiAliasing = AntiAliasingStyles.All,
                                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
                                PaletteCustomColors = new []
                                                          {
                                                              Color.FromArgb(200, 53, 99, 50),
                                                              Color.FromArgb(200, 200, 0, 0),
                                                              Color.FromArgb(200, 115, 0, 6)
                                                          }
                            };
            chart.Titles.Add(CreateTitle(MJobClosingRateTitle));
           
            
            foreach (var title in CreateJobClosingRateTitles(technicians))
            {
                chart.Titles.Add(title);
            }

            foreach (var series in CreateJobClosingRateSeries(technicians))
            {
                chart.Series.Add(series);

            }
            
            chart.Legends.Add(CreateJobClosingRateLegend());

            for (var j = 0; j < technicians.Count(); j++)
            {
                chart.Series[j].IsVisibleInLegend = (j == 0);
            }
            
            foreach (var area in CreateJobClosingRateAreas(technicians))
            {
                chart.ChartAreas.Add(area);
            }

            return chart;
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

        private static IEnumerable<Title> CreateJobClosingRateTitles(IEnumerable<Technician> technicians)
        {
            var i = -1;
            return technicians.Select(technician => new Title
                                                        {
                                                            Text = technician.Name,
                                                            DockedToChartArea = "JobArea" + ++i,
                                                            Font = new Font("Trebuchet MS", 8F, FontStyle.Bold),
                                                            DockingOffset = -3
                                                        }).ToArray();
        }

        private static Legend CreateTechniciansSalesAndDiscountLegend()
        {
            return new Legend {BackColor = Color.Transparent, Docking = Docking.Top};
        }
       
        private static Legend CreateJobClosingRateLegend()
        {
            return new Legend
                             {
                                 Name = "Result Chart",
                                 Docking = Docking.Top,
                                 Alignment = StringAlignment.Center,
                                 LegendStyle = LegendStyle.Row
                             };
        }

        private static IEnumerable<Series> CreateTechniciansSalesAndDiscountSeries(IEnumerable<Technician> technicians)
        {

            var seriesArray =
                new []
                    {                       
                        new Series
                            {
                                Name = "test",
                                ChartType = SeriesChartType.StackedColumn,
                                Color = Color.FromArgb(255, 53, 99, 50),
                                
                                BorderColor = Color.Black,
                                BorderWidth = 1,
                                ChartArea = "TechniciansSalesAndDiscountArea"
                            }
                    };

            const string drawingStyle = "Cylinder";
            seriesArray[0]["DrawingStyle"] = drawingStyle;
            const string pointWidth = "0.6";
            seriesArray[0]["PointWidth"] = pointWidth;

            foreach (var technician in technicians)
            {
                seriesArray[0].Points.Add(new DataPoint
                {
                    AxisLabel = technician.Name,
                    YValues = new[] 
                                { 
                                    (double)technician.JobsAVG //.DiscountsCost 
                                }
                });
            }

            return seriesArray;
        }

        private static IEnumerable<Series> CreateJobClosingRateSeries(IEnumerable<Technician> technicians)
        {

            var seriesList = new List<Series>();

            var i = -1;
            foreach (Technician technician in technicians)
            {
                i++;
                var series = new Series
                {
                    Name = "JobSeries" + i,
                    ChartType = SeriesChartType.Pie,
                    ChartArea = "JobArea" + i,
                    
                };

                series.Points.Add(new DataPoint
                {
                    YValues = new[] 
                                                    { 
                                                        (double)technician.JobsComplete
                                                    }
                });
                series.Points.Add(new DataPoint
                {
                    YValues = new[] 
                                                    { 
                                                        (double)technician.JobsEstimate
                                                    }
                });
                series.Points.Add(new DataPoint
                {
                    YValues = new[] 
                                                    { 
                                                        (double)technician.JobsRecall
                                                    }
                });
                
                series.Points[1]["Exploded"] = "true";
                series.Points[2]["Exploded"] = "true";
                series.Points[0].LegendText = "Complete";
                series.Points[1].LegendText = "Estimate";
                series.Points[2].LegendText = "Recall";

                seriesList.Add(series);
            }

            return seriesList;
        }

        private static ChartArea CreateTechniciansSalesAndDiscountArea()
        {
            var chartArea = new ChartArea
                                {
                                    InnerPlotPosition = new ElementPosition(5, 0, 100, 85),
                                    Name = "TechniciansSalesAndDiscountArea",
                                    BackColor = Color.Transparent,
                                    Area3DStyle =
                                        {
                                            Enable3D = true,
                                            LightStyle = LightStyle.Realistic,
                                            Perspective = 0
                                        },
                                    AxisX = {IsLabelAutoFit = false},
                                    AxisY = {IsLabelAutoFit = false},
                                };
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
            chartArea.AxisX.LineColor = Color .FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.White;
            chartArea.AxisX.MajorGrid.Enabled=false;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Interval = 400;
            chartArea.AxisY.LabelStyle.Format = "{0:C0}";
            return chartArea;
        }

        private static IEnumerable<ChartArea> CreateJobClosingRateAreas(IEnumerable<Technician> technicians)
        {
            var chartAreas = new List<ChartArea>();
            for (var i = 0; i < technicians.Count();i++)
            {
                chartAreas.Add(new ChartArea
                        {
                            InnerPlotPosition = new ElementPosition(0, 0, 100, 100),
                            Name = "JobArea" + i,
                            BackColor = Color.Transparent,
                            Area3DStyle = {Enable3D = true, Inclination = 15, Rotation = -90}
                        });
            }
            return chartAreas;
        }
        #endregion

        private static tbl_Employee[] GetTechnicians(int franchiseId)
        {
            using (var dbeight = GetContext())
            {
                return (from s in dbeight.tbl_Employee
                 where s.FranchiseID == franchiseId
                 orderby s.Employee
                 select s).ToArray();
            }
        }

        #region DisplayKPI

        public ActionResult Index(int? franchiseId, FormCollection formcollection)
        {
            var endDate = !String.IsNullOrWhiteSpace(formcollection["txtEndDate"])
                              ? DateTime.Parse(formcollection["txtEndDate"])
                              : DateTime.Today.AddDays(-1*((int) DateTime.Today.DayOfWeek + 1));
            var startDate = !String.IsNullOrWhiteSpace(formcollection["txtStartDate"])
                                ? DateTime.Parse(formcollection["txtStartDate"])
                                : endDate.AddDays(-6);

            var vals = formcollection.GetValues("cbAllTechs");
            var allTechs = vals != null && vals.Length > 0 ? bool.Parse(vals[0]) : false;

            return View(GetModel(franchiseId ?? UserInfo.CurrentFranchise.FranchiseID, startDate, endDate, true, allTechs));
        }

        private DisplayKPI GetModel(int franchiseId, DateTime startDate, DateTime endDate, bool freshData, bool allTechs)
        {
            var kpi = GetKpiData(franchiseId, startDate.ToShortDateString(), endDate.ToShortDateString(), freshData);
            var emp = GetTechnicians(franchiseId);
            var hash = emp.ToDictionary(sp => sp, sp => kpi.GetByTechnician(sp.EmployeeID));

            var model = new DisplayKPI
                            {
                                StartDate = startDate,
                                EndDate = endDate,
                                FranchiseId = franchiseId,
                                Total_SalesActual = kpi.ActualSales,
                                Total_SalesEstimates = kpi.EstimateSales,
                                Total_SalesOutstandingEstimates = kpi.OutstandingEstimateSales,
                                Total_UpSalesNumber = kpi.AddOns,
                                Total_UpSalesCost = kpi.AddOnSales,
                                Total_UpSalesHG = kpi.HomeGuard,
                                Total_UpSalesBio = kpi.Bio,
                                Total_DiscountsNumber = kpi.Discounts,
                                Total_DiscountsCost = kpi.DiscountSales,
                                Total_JobsComplete = kpi.CompletedCount,
                                Total_JobsEstimate = kpi.EstimateCount,
                                Total_JobsOutstandingEstimate = kpi.OutstandingEstimateCount,
                                Total_JobsRecall = kpi.RecallCount,
                                Total_ClosingRate = Convert.ToInt32(Math.Round(kpi.CloseRate)),
                                Total_RecoverRate = Convert.ToInt32(Math.Round(kpi.RecoverRate)),
                                Total_JobsAVG = kpi.AverageTicket,
                                ShowAllTechs = allTechs,
                                TechnicianList = hash.Where(pair => (allTechs && pair.Key.ServiceProYN) || (pair.Value != null && pair.Value.AllJobs.Count() != 0))
                                                     .Select(pair => new Technician
                                                                         {
                                                                             SalesActual = pair.Value.ActualSales,
                                                                             SalesEstimates = pair.Value.EstimateSales,
                                                                             OutstandingEstimateSales = pair.Value.OutstandingEstimateSales,
                                                                             JobsComplete = pair.Value.CompletedCount,
                                                                             JobsEstimate = pair.Value.EstimateCount,
                                                                             JobsOutstandingEstimate = pair.Value.OutstandingEstimateCount,
                                                                             JobsRecall = pair.Value.RecallCount,
                                                                             UpSalesNumber = pair.Value.AddOns,
                                                                             UpSalesCost = pair.Value.AddOnSales,
                                                                             UpSalesHG = pair.Value.HomeGuard,
                                                                             UpSalesBio = pair.Value.Bio,
                                                                             DiscountsNumber = pair.Value.Discounts,
                                                                             DiscountsCost = pair.Value.DiscountSales,
                                                                             ClosingRate =
                                                                                 Convert.ToInt32(
                                                                                     Math.Round(pair.Value.CloseRate)),
                                                                             RecoverRate =
                                                                                 Convert.ToInt32(
                                                                                     Math.Round(pair.Value.RecoverRate)),
                                                                             JobsDispatched = pair.Value.DispatchCount,
                                                                             EmployeeId = pair.Key.EmployeeID,
                                                                             Name = pair.Key.Employee,
                                                                             JobsAVG = pair.Value.AverageTicket
                                                                         })
                            };


            return model;
        }

        public ActionResult DisplayAllDetails(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {
            var joblist = GetKpiForTech(employeeId, franchiseId, strStartDate, strEndDate).AllJobs.Select(GetKpiDetail);

            return Json(joblist);
        }

        public ActionResult DisplayDetails(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {

            var kpi = GetKpiForTech(employeeId, franchiseId, strStartDate, strEndDate);
            var joblist = kpi.CompletedJobs.Select(GetKpiDetail);

            return Json(joblist);
        }

        public ActionResult DisplayEstimateSales(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {
            var joblist = GetKpiForTech(employeeId, franchiseId, strStartDate, strEndDate).Estimates.Select(GetKpiDetail);

            return Json(joblist);
        }

        public ActionResult DisplayOutstandingEstimateSales(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {
            var joblist = GetKpiForTech(employeeId, franchiseId, strStartDate, strEndDate).OutstandingEstimates.Select(GetKpiDetail);

            return Json(joblist);
        }

        public ActionResult DisplayUpsales(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {
            var kpi = GetKpiForTech(employeeId, franchiseId, strStartDate, strEndDate);
            var joblist = kpi.AddOnJobs.Select(GetKpiDetail).ToArray();

            joblist.ToList().ForEach(j => j.JobAmt = kpi.GetAddOnSales(j.JobId));
            return Json(joblist);
        }

        public ActionResult DisplayRecall(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {
            var joblist = GetKpiForTech(employeeId, franchiseId, strStartDate, strEndDate).RecallJobs.Select(GetKpiDetail);

            return Json(joblist);
        }

        public ActionResult DisplayDiscount(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {
            var kpi = GetKpiForTech(employeeId, franchiseId, strStartDate, strEndDate);
            var joblist = kpi.DiscountJobs.Select(GetKpiDetail).ToArray();

            joblist.ToList().ForEach(j => j.JobAmt = kpi.GetDiscountSales(j.JobId));
            return Json(joblist);
        }

        private KeyPerformanceIndicator GetKpiForTech(int employeeId, int franchiseId, string strStartDate, string strEndDate)
        {
            return GetKpiData(franchiseId, strStartDate, strEndDate, false).GetByTechnician(employeeId);
        }

        private static KPIJobDetail GetKpiDetail(Business.Reporting.Job job)
        {
            return new KPIJobDetail
                       {
                JobId = job.Id,
                Balance = job.Balance,
                BillTo = job.CustomerName,
                ClosedDate = job.Completed,
                Comments = string.Empty,
                CompletedDate = job.Completed,
                JobAmt = job.SubTotal,
                JobLocation = job.Address ?? string.Empty,
                JobType = job.Service ?? string.Empty,
                Status = job.Status ?? "N/A",
                EstimateDate = job.EstimateDate,
                Tech = job.Tech
            };
        }

        private KeyPerformanceIndicator GetKpiData(int franchiseId, string strStartDate, string strEndDate, bool force)
        {
            if (UserInfo.CurrentFranchise == null || UserInfo.CurrentFranchise.FranchiseID != franchiseId)
                SetCurrentFranchise(franchiseId, UserInfo.ShowInactiveFranchises);

            const string key = "KPI_DATA";
            KeyPerformanceIndicator kpi = null;
            if (HttpContext.Session != null && HttpContext.Session[key] != null)
                    kpi = HttpContext.Session[key] as KeyPerformanceIndicator;

            var startDate = DateTime.Parse(strStartDate).Date;
            var endDate = DateTime.Parse(strEndDate).Date.AddDays(1);

            if (kpi == null || force)
            {
                kpi = AbstractBusinessService.Create<ReportingService>(UserInfo.UserKey).GetKeyPerformanceIndicators(
                        franchiseId, startDate, endDate);
                Session[key] = kpi;
            }

            return kpi;
        }

        #endregion
    }
}
