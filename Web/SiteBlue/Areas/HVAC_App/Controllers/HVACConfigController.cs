using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using SiteBlue.Areas.HVAC_App.Controllers;
using SiteBlue.Areas.HVAC_App.Models;
using SiteBlue.Areas.HVAC_App.Models.Interfaces;
using SiteBlue.Areas.HVAC_App.Models.SetupModels;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;

namespace HVACapp.Areas.HVAC_App.Controllers
{
    public class BrowserCacheAttribute : ActionFilterAttribute
    {
        /// <summary>  
        /// Gets or sets the cache duration in seconds.   
        /// The default is 10 seconds.  
        /// </summary>  
        /// <value>The cache duration in seconds.</value>  
        public int Duration
        {
            get;
            set;
        }

        public bool PreventBrowserCaching
        {
            get;
            set;
        }

        public BrowserCacheAttribute()
        {
            Duration = 10;
        }

        public override void OnActionExecuted(
          ActionExecutedContext filterContext)
        {
            if (Duration < 0) return;

            HttpCachePolicyBase cache = filterContext.HttpContext
              .Response.Cache;

            if (PreventBrowserCaching)
            {
                cache.SetCacheability(HttpCacheability.NoCache);
                Duration = 0;
            }
            else
            {
                cache.SetCacheability(HttpCacheability.Public);
            }

            TimeSpan cacheDuration = TimeSpan.FromSeconds(Duration);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate,"
              + "proxy-revalidate");
        }
    }  

    //[OutputCache(NoStore = true, Duration = 1, VaryByParam = "*")]
    [BrowserCache(PreventBrowserCaching = true)]
    public class HVACConfigController : HVACController
    {
        protected override int GetFranchiseID()
        {
            
            return UserInfo.CurrentFranchise.FranchiseID;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //FranchiseId = new FranchiseStoreInUserInfo(UserInfo);
        }


        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllAccessoriesForPriceList(string tons)
        {
            var context = GetContext();
            var frId = GetFranchiseID();
            var priceBook = context.tbl_HVAC_ConfigFranchise.First(item => item.FranchiseID == frId).PricebookID;
            var priceBookId = priceBook.HasValue ? priceBook.Value : 177;
            var ton = double.Parse(tons.Replace('.', ','));
            var lastcode = Convert.ToInt32(12 * ton).ToString();
            if (lastcode.Length == 3)
                lastcode = lastcode.Remove(2, 1);
            var list = context.View_HVAC_APP.Where(item => item.PriceBookID == priceBookId && item.JobCode.StartsWith("ACC")).ToList();
            var listOfAcc = new List<AccessoryModel>();
            foreach (var acc in list)
            {
                if (acc.JobCode.Contains("GSASD"))
                {
                    if (acc.JobCode.Contains(lastcode))
                    {
                        listOfAcc.Add(new AccessoryModel
                        {
                            Code = acc.JobCode,
                            Count = 1,
                            JobCode = acc.JobCode,
                            JobCodeId = acc.JobCodeID,
                            ResAccountCode = acc.ResAccountCode,
                            Description = acc.JobCodeDescription,
                            Price = acc.JobStdPrice
                        });
                    }
                }
                else
                {
                    listOfAcc.Add(new AccessoryModel
                    {
                        Code = acc.JobCode,
                        Count = 1,
                        JobCode = acc.JobCode,
                        JobCodeId = acc.JobCodeID,
                        ResAccountCode = acc.ResAccountCode,
                        Description = acc.JobCodeDescription,
                        Price = acc.JobStdPrice
                    });
                }
            }
            return Json(listOfAcc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllAccessories()
        {
            var context = GetContext();
            var configId = GetConfigID();
            var frId = GetFranchiseID();
            var priceBook = context.tbl_HVAC_ConfigFranchise.First(item => item.FranchiseID == frId).PricebookID;
            var priceBookId = priceBook.HasValue ? priceBook.Value : 177;
            var list = context.View_HVAC_APP.Where(item => item.PriceBookID == priceBookId).Where(
                i => i.JobCode.StartsWith("ACC")).ToList();

            var listOfAcc = new List<AccessoryModel>();
            var bGSASD = true;
            foreach (var acc in list)
            {
                if (acc.JobCode.Contains("GSASD"))
                {
                    if (bGSASD)
                    {
                        listOfAcc.Add(new AccessoryModel
                        {
                            Code = "ACC-GSASD##",
                            Count = 1,
                            Description = acc.JobCodeDescription,
                            Price = acc.JobStdPrice,
                        });
                        bGSASD = false;
                    }
                }
                else
                {
                    listOfAcc.Add(new AccessoryModel
                    {
                        Code = acc.JobCode,
                        JobCode = acc.JobCode,
                        JobCodeId = acc.JobCodeID,
                        ResAccountCode = acc.ResAccountCode,
                        Count = 1,
                        Description = acc.JobCodeDescription,
                        Price = acc.JobStdPrice,
                    });
                }
            }
            var listOfAccCodes =
                context.tbl_HVAC_ConfigPackages.Where(
                    item =>
                    item.ConfigID == configId).GroupBy(item =>
                                                       item.JobCode).ToList();

            return Json(
                new
                {
                    rows = listOfAcc.Select(item =>
                               new
                               {
                                   id = item.Code,
                                   data = new[] { item.Code, item.Description, item.Price.ToString("c"),
                                           listOfAccCodes.Any(g=>g.Key==item.Code)?listOfAccCodes.First(g=>g.Key==item.Code).Any(g=>g.SystemID==4).ToString().ToLower():"false",
                                           listOfAccCodes.Any(g=>g.Key==item.Code)?listOfAccCodes.First(g=>g.Key==item.Code).Any(g=>g.SystemID==3).ToString().ToLower():"false",
                                           listOfAccCodes.Any(g=>g.Key==item.Code)?listOfAccCodes.First(g=>g.Key==item.Code).Any(g=>g.SystemID==5).ToString().ToLower():"false",
                                           listOfAccCodes.Any(g=>g.Key==item.Code)?listOfAccCodes.First(g=>g.Key==item.Code).Any(g=>g.SystemID==2).ToString().ToLower():"false",
                                           listOfAccCodes.Any(g=>g.Key==item.Code)?listOfAccCodes.First(g=>g.Key==item.Code).Any(g=>g.SystemID==1).ToString().ToLower():"false",
                                           listOfAccCodes.Any(g=>g.Key==item.Code)?listOfAccCodes.First(g=>g.Key==item.Code).First().LinkedQuestionID.GetValueOrDefault().ToString():"0"
                                           }
                               }
                           ).ToArray()
                }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult SaveAccessoriesPackage()
        {
            ////string gr_id, string AccDescrptn, bool? ue,bool? se,bool? he, bool? me, bool? le
            var context = GetContext();
            var configId = GetConfigID();
            var config = context.tbl_HVAC_ConfigsApp.First(item => item.ConfigID == configId);
            var listOfsystems = config.tbl_HVAC_ConfigSystems.ToList();
            var listofpackages = config.tbl_HVAC_ConfigPackages.ToList();
            foreach (var systemse in listOfsystems)
            {
                if (listofpackages.Any(item => item.SystemID == systemse.SystemID && item.JobCode == Request.Form["gr_id"]))
                {
                    if (Request.Form[systemse.tbl_HVAC_SystemType.SystemTypeName.ToLower()] == "0")
                    {
                        context.tbl_HVAC_ConfigPackages.DeleteObject(
                            listofpackages.Single(item => item.SystemID == systemse.SystemID && item.JobCode == Request.Form["gr_id"]));
                    }
                    else
                    {
                        var qw = int.Parse(Request.Form["qw"]);
                        var acc =
                            listofpackages.Single(
                                item => item.SystemID == systemse.SystemID && item.JobCode == Request.Form["gr_id"]);
                        acc.LinkedQuestionID = qw == 0 ? (int?)null : qw;
                    }
                }
                else
                {
                    if (Request.Form[systemse.tbl_HVAC_SystemType.SystemTypeName.ToLower()] != "0")
                    {
                        var qw = int.Parse(Request.Form["qw"]);
                        context.tbl_HVAC_ConfigPackages.AddObject(new tbl_HVAC_ConfigPackages
                        {
                            ConfigID = configId,
                            JobCode = Request.Form["gr_id"],
                            SystemID = systemse.SystemID,
                            LinkedQuestionID = qw == 0 ? (int?)null : qw
                        });
                    }
                }
            }
            context.SaveChanges();
            return new XmlResult(String.Format("<data><action type=\"{0}\" sid=\"{1}\" tid=\"{2}\"/></data>", "update", Request.Form["gr_id"], Request.Form["gr_id"]));
        }

        //public ActionResult GetSystemInforms(string code, string tons, string ac, string uv, string hm, string dw)
        //{
        //    var totalInfoList = new List<TotalInfo>();
        //    var testMainSystem = new SystemInfoModelWithParts
        //                             {
        //                                 AFUE = "10",
        //                                 Count = 1,
        //                                 Description = "Test sys",
        //                                 id = "SE",
        //                                 JobCode = "ASDD",
        //                                 JobCodeId = 1,
        //                                 Price = 1000m,
        //                                 SEER = "20",
        //                                 Parts = new List<JobPart>
        //                                             {
        //                                                 new JobPart
        //                                                     {
        //                                                         PartCode = "part1",
        //                                                         PartCost = 400m,
        //                                                         PartName = "Part1 Name",
        //                                                         PartID = 1,
        //                                                         PartStdPrice = 400m,
        //                                                         Qty = 1
        //                                                     },
        //                                                 new JobPart
        //                                                     {
        //                                                         PartCode = "part2",
        //                                                         PartCost = 600m,
        //                                                         PartName = "Part2 Name",
        //                                                         PartID = 2,
        //                                                         PartStdPrice = 600m,
        //                                                         Qty = 1
        //                                                     }
        //                                             }
        //                             };
        //    var t = new TotalInfo
        //                {
        //                    MainSystem = testMainSystem

        //                };

                
        //    t.AddMainSystem(testMainSystem);
        //    (t.Jobs.First() as SystemInfoModelWithParts).id += "1";
        //    t.AddAccessory(new AccessoryModel
        //                     {
        //                         AccountCode = "acccode1",
        //                         JobCode = "ACC1",
        //                         Count = 1,
        //                         Code = "ACCCC1",
        //                         Description = "ACC1 Name",
        //                         JobCodeId = 1,
        //                         Price = 300m
        //                     });
        //    t.AddAccessory(new AccessoryModel
        //                       {
        //                           AccountCode = "acccode2",
        //                           JobCode = "ACC2",
        //                           Count = 1,
        //                           Code = "ACCCC2",
        //                           Description = "ACC2 Name",
        //                           JobCodeId = 2,
        //                           Price = 300m
        //                       });
        //    t.Tax = 0.4m;
        //    totalInfoList.Add(t);
        //    return Json(new { system_ids = new[]{"SE"}, systems = totalInfoList.ToArray()});
        //}
        [HttpPost]
        public ActionResult GetSystemInforms(string code, string tons, string ac, string uv, string hm, string dw)
        {
            var noHVACDescription = "This package is not available\nin your pricebook.";//"No HVAC configuration\nhas been defined\nfor this selection.";
            var context = GetContext();
            var configId = GetConfigID();
            var frId = GetFranchiseID();
            var priceBookId = GetPriceBookId(frId, context);

            var ton = double.Parse(tons.Replace('.', ','));
            var lastcode = Convert.ToInt32(12 * ton).ToString();
            if (lastcode.Length == 3)
                lastcode = lastcode.Remove(2, 1);

            var linkedQustions = GetAllowedAcc(ac, uv, hm, dw);

            var mainSystemInfos = GetSelectedSystemsP().Select(s => new SystemInfoModelWithParts()
                                                        {
                                                            JobCode = s.tbl_HVAC_SystemType.SystemTypeName + "-" + code + lastcode,
                                                            id = s.tbl_HVAC_SystemType.SystemTypeName,
                                                            Description = noHVACDescription,
                                                            Price = 0,
                                                            SEER = s.SEER.GetValueOrDefault().ToString(), 
                                                            AFUE = s.AFUE.GetValueOrDefault().ToString() 
                                                        }).ToList();

            var list = GetPackagesFromDB(priceBookId, context, mainSystemInfos);

            var listparts = GetListOfPartsFromDB(priceBookId, context, list);

            var totalInfoList = new List<TotalInfo>();

            foreach (var viewHvacApp in list)
            {
                var hvacSys = mainSystemInfos.Single(item => item.JobCode == viewHvacApp.JobCode);
                hvacSys.Count = 1;
                hvacSys.Description = String.Format("SEER Rating {0}, AFUE {1}, {2}", hvacSys.SEER, hvacSys.AFUE,
                                                    viewHvacApp.JobCodeDescription);
                hvacSys.Price = viewHvacApp.JobStdPrice;
                hvacSys.ResAccountCode = viewHvacApp.ResAccountCode;
                hvacSys.Parts = listparts
                    .Where(item => item.JobCode == hvacSys.JobCode)
                    .Select(item => new JobPart
                                        {
                                            PartCode = item.PartCode,
                                            PartCost = item.PartCost,
                                            PartID = item.PartID,
                                            PartName = item.PartName,
                                            PartStdPrice = item.PartStdPrice,
                                            Qty = item.Qty
                                        }).ToList();
                if (hvacSys.Parts.Count == 0)
                {
                    hvacSys.Description = noHVACDescription;
                }
            }

            foreach (var hvacSys in mainSystemInfos)
            {
                var totalsystem = new TotalInfo
                                      {
                                          MainSystem = hvacSys,
                                          Tax = 0.05m
                                      };

                totalsystem.AddMainSystem(hvacSys);
                var listOfAccCodes =
                    context.tbl_HVAC_ConfigPackages.Where(
                        item =>
                        item.ConfigID == configId &&
                        item.tbl_HVAC_SystemType.SystemTypeName == hvacSys.id &&
                        linkedQustions.Contains(item.LinkedQuestionID.HasValue ? item.LinkedQuestionID.Value : 0)
                        ).ToArray().Select(item => item.JobCode.Replace("##", code)).ToArray();

                var listOfAcc = context.View_HVAC_APP.Where(item => item.PriceBookID == priceBookId).Where(
                    i => listOfAccCodes.Contains(i.JobCode)).ToList();

                foreach (var hvacApp1 in listOfAcc)
                {
                    totalsystem.AddAccessory(new AccessoryModel
                                                 {
                                                     JobCodeId = hvacApp1.JobCodeID,
                                                     Code = hvacApp1.JobCode,
                                                     JobCode = hvacApp1.JobCode,
                                                     ResAccountCode = hvacApp1.ResAccountCode,
                                                     Count = 1,
                                                     Description = hvacApp1.JobCodeDescription,
                                                     Price = hvacApp1.JobStdPrice
                                                 });
                }
                totalInfoList.Add(totalsystem);
            }
            return Json(new { system_ids = mainSystemInfos.Select(item => item.id).ToArray(), systems = totalInfoList.ToArray() });
        }

        private List<View_HVAC_APP_Parts> GetListOfPartsFromDB(int priceBookId, EightHundredEntities context, IEnumerable<View_HVAC_APP> list)
        {
            var jobcodes = list.Select(item => item.JobCode);
            var listparts =
                context.View_HVAC_APP_Parts.Where(item => item.PriceBookID == priceBookId).Where(
                    i => jobcodes.Contains(i.JobCode)).ToList();
            return listparts;
        }

        private List<View_HVAC_APP> GetPackagesFromDB(int priceBookId, EightHundredEntities context, IEnumerable<SystemInfoModelWithParts> mainSystemInfos)
        {
            var listofcodes = mainSystemInfos.Select(i => i.JobCode).ToArray();

            var list = context.View_HVAC_APP.Where(item => item.PriceBookID == priceBookId).Where(
                i => listofcodes.Contains(i.JobCode)).ToList();
            return list;
        }

        private int GetPriceBookId(int frId, EightHundredEntities context)
        {
            var priceBook = context.tbl_HVAC_ConfigFranchise.First(item => item.FranchiseID == frId).PricebookID;
            var priceBookId = priceBook.HasValue ? priceBook.Value : -1;
            return priceBookId;
        }

        private List<int> GetAllowedAcc(string ac, string uv, string hm, string dw)
        {
            //TODO
            var linkedQustions = new List<int> { 0 };
            if (ac != "N/A")
                linkedQustions.Add(7);
            if (uv != "N/A")
                linkedQustions.Add(8);
            if (hm != "N/A")
                linkedQustions.Add(9);
            if (dw != "N/A")
                linkedQustions.Add(34);
            return linkedQustions;
        }

        private IEnumerable<tbl_HVAC_ConfigSystems> GetSelectedSystemsP()
        {
            int configID = GetConfigID();
            var context = GetContext();
            var list =
                context.tbl_HVAC_ConfigSystems.Where(item => item.ConfigID == configID).OrderBy(i => i.OrderNum).ToList();
            return list;

        }

        public ActionResult SetupLogo()
        {
            int configID = GetConfigID();
            var context = GetContext();
            var model = new LogoModel(configID, context);
            return PartialView(model);
            //return PartialView(new LogoModel {Url = "../../../Areas/hvac_app/content/bigimage.png"});
        }

        public ActionResult SaveLogo()
        {
            int configID = GetConfigID();
            var context = GetContext();
            LogoModel.UpdateOrAddedUrl(Request.Form.Get("Url"), context, configID);
            try
            {
                context.SaveChanges();
                return Json(new {result = true});
            }
            catch (Exception)
            {
                return Json(new {result = false});
            }
        }

        public ActionResult SetupAccessoriesGrid()
        {
            return View();
        }
        
        [BrowserCache(PreventBrowserCaching = true)]
        public ActionResult SetupPackages()
        {
            return View();
        }

        [BrowserCache(PreventBrowserCaching = true)]
        public ActionResult GetForChooseSystem()
        {
            var context = GetContext();
            var listOfAll = context.tbl_HVAC_SystemType.ToList();

            var selectedList = GetSelectedSystemsP();

            return Json(listOfAll.Except(selectedList.Select(item => item.tbl_HVAC_SystemType), new ComparerSystemTypes()).Select(i => new { id = i.SystemTypeID, text = i.SystemTypeName, seer = "", afue = "" }), JsonRequestBehavior.AllowGet);
            //return Json(new[] { new { id = "1", text = "SE", seer = "", afue = "" }, new { id = "2", text = "ME", seer = "", afue = "" }, new { id = "3", text = "LE", seer = "", afue = "" } }, JsonRequestBehavior.AllowGet);
        }

        class ComparerSystemTypes : IEqualityComparer<tbl_HVAC_SystemType>
        {
            public bool Equals(tbl_HVAC_SystemType x, tbl_HVAC_SystemType y)
            {
                return x.SystemTypeID == y.SystemTypeID;
            }

            public int GetHashCode(tbl_HVAC_SystemType obj)
            {
                return obj.SystemTypeID;
            }
        }

        [BrowserCache(PreventBrowserCaching = true)]
        public ActionResult GetSelectedSystems()
        {
            //return Json(new[] { new { id = "4", text = "HE", seer = "12", afue = "20" }, new { id = "5", text = "UE", seer = "15", afue = "25" } }, JsonRequestBehavior.AllowGet);
            return Json(GetSelectedSystemsP().Select(i => new { id = i.SystemID, text = i.tbl_HVAC_SystemType.SystemTypeName, seer = i.SEER.GetValueOrDefault().ToString(), afue = i.AFUE.GetValueOrDefault().ToString() }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetSystems()
        {
            int configID = GetConfigID();
            var context = GetContext();
            foreach (var configsys in context.tbl_HVAC_ConfigSystems.Where(item => item.ConfigID == configID))
            {
                context.tbl_HVAC_ConfigSystems.DeleteObject(configsys);
            }
            var listOfPackages = Request.Form["packages"].Split(new[] { ',' });
            for (int i = 0; i < listOfPackages.Length; ++i)
            {
                var key = listOfPackages[i];
                var confsystem = new tbl_HVAC_ConfigSystems
                {
                    ConfigID = configID,
                    SystemID = int.Parse(key),
                    SEER = Double.Parse(Request.Form[key + "-seer"]),
                    AFUE = Double.Parse(Request.Form[key + "-afue"]),
                    OrderNum = i.ToString()
                };

                context.tbl_HVAC_ConfigSystems.AddObject(confsystem);
            }
            context.SaveChanges();
            return Json(new { result = true });
        }

        public ActionResult SetupVideos()
        {
            //return PartialView(new PackageVideoCode[] { new PackageVideoCode { PackageName = "UE", Code = "1NJrCj4PP78" }, new PackageVideoCode { PackageName = "HE", Code = "WwdArhNPJPE" }, new PackageVideoCode { PackageName = "ME", Code = "asdf" } });
            var configId = GetConfigID();
            var context = GetContext();
            var urls = context.tbl_HVAC_ConfigVideoUrls.Where(item => item.ConfigID == configId).ToList();
            var selectedSystems = GetSelectedSystemsP().ToArray();
            var ls = new PackageVideoCode[selectedSystems.Length];
            for (var i=0; i<ls.Length; ++i)
            {
                ls[i] = new PackageVideoCode
                            {
                                Id = selectedSystems[i].SystemID,
                                PackageName = selectedSystems[i].tbl_HVAC_SystemType.SystemTypeName,
                                Code = ""
                            };
                if (urls.Any(item=>item.SystemID==selectedSystems[i].SystemID))
                {
                    ls[i].Code = urls.First(item => item.SystemID == selectedSystems[i].SystemID).VideoUrl;
                }

            }
            return View(ls);
        }

        public ActionResult GetVideo(PackageVideoCode g)
        {
            return PartialView("GetVideo", g);
        }

        public ActionResult SaveVideoCode()
        {
            var configId = GetConfigID();
            var context = GetContext();
            var systemId = int.Parse(Request.Form["Id"]);
            if (context.tbl_HVAC_ConfigsApp.First(item => item.ConfigID == configId).tbl_HVAC_ConfigVideoUrls.Any(item => item.SystemID == systemId))
            {
                var code =
                    context.tbl_HVAC_ConfigsApp.First(item => item.ConfigID == configId).tbl_HVAC_ConfigVideoUrls.First(
                        item => item.SystemID == systemId);
                code.VideoUrl = Request.Form[Request.Form.AllKeys.First(i=>i.StartsWith("cd"))];
            }
            else
            {
                context.tbl_HVAC_ConfigVideoUrls.AddObject(new tbl_HVAC_ConfigVideoUrls
                {
                    ConfigID = configId,
                    SystemID = systemId,
                    VideoUrl = Request.Form[Request.Form.AllKeys.First(i => i.StartsWith("cd"))]
                });
            }
            context.SaveChanges();
            return Json(new { result = true });
        }

        public ActionResult SetupGuaranteeTexts()
        {
            var context = GetContext();
            var configId = GetConfigID();
            
            var texts = GuaranteeModel.GuaranteeModels(context, configId);
            //var texts = new[]
            //             {
            //                 new GuaranteeModel
            //                     {
            //                         Id = 1,
            //                         GuaranteeName = "guar1",
            //                         GuaranteeText = "guar1text"
            //                     },
            //                     new GuaranteeModel
            //                     {
            //                         Id = 2,
            //                         GuaranteeName = "guar2",
            //                         GuaranteeText = "guar2text"
            //                     }
            //             };
            return PartialView(texts);
        }

        //private GuaranteeModel[] GuaranteeModels(EightHundredEntities context, int configId)
        //{
        //    var listOfGuaranteeIds = context.tbl_HVAC_Guarantees.Select(item => item.GuaranteeID).ToList();
        //    var listOfselectedGuarantees =
        //        context.tbl_HVAC_ConfigQuestions.Where(item => item.ConfigID == configId && listOfGuaranteeIds.Contains(item.QuestionID)).Select(i => i.QuestionID).ToArray();
        //    var gt = context.tbl_HVAC_ConfigGuaranteeTexts.Where(item => item.ConfigID == configId).ToList();
        //    var texts = new GuaranteeModel[listOfselectedGuarantees.Length];
        //    for (var i = 0; i < texts.Length; ++i)
        //    {
        //        var grId = listOfselectedGuarantees[i];
        //        var gr = context.tbl_HVAC_Guarantees.Single(item => item.GuaranteeID == grId);
        //        texts[i] = new GuaranteeModel
        //                       {
        //                           Id = gr.GuaranteeID,
        //                           GuaranteeName = gr.GuaranteeName,
        //                           GuaranteeText = ""
        //                       };
        //        if (gt.Any(item => item.GuaranteeID == gr.GuaranteeID))
        //        {
        //            texts[i].GuaranteeText = gt.Single(item => item.GuaranteeID == gr.GuaranteeID).GuaranteeText;
        //        }
        //    }
        //    return texts;
        //}

        public ActionResult GetGuarantee(GuaranteeModel g)
        {
            return PartialView(g);
        }

        public ActionResult SaveGuarantee()
        {
            var configId = GetConfigID();
            var context = new EightHundredEntities();
            var guaranteeId = int.Parse(Request.Form["Id"]);
            if (context.tbl_HVAC_ConfigsApp.First(item => item.ConfigID == configId).tbl_HVAC_ConfigGuaranteeTexts.Any(item => item.GuaranteeID == guaranteeId))
            {
                var code =
                    context.tbl_HVAC_ConfigsApp.First(item => item.ConfigID == configId).tbl_HVAC_ConfigGuaranteeTexts.First(
                        item => item.GuaranteeID == guaranteeId);
                code.GuaranteeText = Request.Form["GuaranteeText"];
            }
            else
            {
                context.tbl_HVAC_ConfigGuaranteeTexts.AddObject(new tbl_HVAC_ConfigGuaranteeTexts
                {
                    ConfigID = configId,
                    GuaranteeID = guaranteeId,
                    GuaranteeText = Request.Form["GuaranteeText"]
                });
            }
            context.SaveChanges();
            return Json(new { result = true });
        }

        public ActionResult SetupReliableInstallations()
        {
            var context = new EightHundredEntities();
            var configId = GetConfigID();
            var code = "";
            if (context.tbl_HVAC_ConfigReliableInstallationsUrl.Any(item => item.ConfigID == configId))
            {
                code = context.tbl_HVAC_ConfigReliableInstallationsUrl.First(item => item.ConfigID == configId).UrlText;
            }
            return PartialView("SetupReliableInstallations", code);
        }

        public ActionResult SaveReliableInstallations()
        {
            var context = new EightHundredEntities();
            var configId = GetConfigID();
            var list =
                    context.tbl_HVAC_ConfigReliableInstallationsUrl.Where(item => item.ConfigID == configId).ToArray();
            if (list.Length > 0)
            {
                list[0].UrlText = Request.Form[Request.Form.AllKeys.First(i => i.StartsWith("cd"))];
            }
            else
            {
                var url = new tbl_HVAC_ConfigReliableInstallationsUrl
                {
                    ConfigID = configId,
                    UrlText = Request.Form[Request.Form.AllKeys.First(i => i.StartsWith("cd"))]
                };
                context.tbl_HVAC_ConfigReliableInstallationsUrl.AddObject(url);
            }
            context.SaveChanges();
            return Json(new { result = true });
        }

        public ActionResult SetupWhoWeAreScreen()
        {
            var context = new EightHundredEntities();
            var configId = GetConfigID();
            var text = "No text.";
            if (context.tbl_HVAC_ConfigWhoWeAreTexts.Any(item => item.ConfigID == configId))
                text = context.tbl_HVAC_ConfigWhoWeAreTexts.Single(item => item.ConfigID == configId).Text;
            return PartialView("SetupWhoWeAreScreen", text);
        }

        public ActionResult SaveHowWeAre()
        {
            var context = new EightHundredEntities();
            var configId = GetConfigID();
            var list =
                    context.tbl_HVAC_ConfigWhoWeAreTexts.Where(item => item.ConfigID == configId).ToArray();
            if (list.Length > 0)
            {
                list[0].Text = Request.Form["GuaranteeText"];
            }
            else
            {
                var url = new tbl_HVAC_ConfigWhoWeAreTexts
                {
                    ConfigID = configId,
                    Text = Request.Form["GuaranteeText"]
                };
                context.tbl_HVAC_ConfigWhoWeAreTexts.AddObject(url);
            }
            context.SaveChanges();
            return Json(new { result = true });
        }

        public ActionResult GetListOfQuestion()
        {
            var configId = GetConfigID();
            var context = new EightHundredEntities();
            return
                Json(
                    context.tbl_HVAC_ConfigQuestions.Where(item => item.ConfigID == configId).OrderBy(
                        item => item.OrderNum).Select(
                            item =>
                            new Question { Id = item.QuestionID, QuestionText = item.tbl_HVAC_Questions.QuestionText }).
                        ToArray());
            //return Json(new[]
            //                {
            //                    new Question() {Id = 1, QuestionText = "Question1"},
            //                    new Question() {Id = 0, QuestionText = "Question0"},
            //                    new Question() {Id = 5, QuestionText = "Question5"},
            //                    new Question() {Id = 3, QuestionText = "Question3"},
            //                    new Question() {Id = 2, QuestionText = "Question2"},
            //                     new Question() {Id = 6, QuestionText = "Question6"}
            //                });
        }

        public ActionResult SetupQuestionsList()
        {
            var context = new EightHundredEntities();
            var configId = GetConfigID();
            var selectedQuestions =
                context.tbl_HVAC_ConfigQuestions.Where(item => item.ConfigID == configId).OrderBy(item => item.OrderNum).Select(
                    item => new SetupQuestion { Id = item.QuestionID, QuestionText = item.tbl_HVAC_Questions.QuestionText }).
                    ToList();
            var allQuestions =
                context.tbl_HVAC_Questions.Select(
                    item => new SetupQuestion { Id = item.QuestionID, QuestionText = item.QuestionText }).ToList();
            var forChoose = allQuestions.Except(selectedQuestions, new QuestionsComparer()).ToList();
            var notOrderIds = new List<int> { 35, 33, 32, 31, 22, 21, 20, 19, 17 };
            var notDeleteIds = notOrderIds;
            return PartialView(new SetupQuestionsModel() { QuestionsSelected = selectedQuestions, QuestionsForChoose = forChoose, NotReorderIds = notOrderIds, NotDeleteIds = notDeleteIds });
        }

        public ActionResult SetQuestions()
        {
            var context = new EightHundredEntities();
            var configId = GetConfigID();
            foreach (var question in context.tbl_HVAC_ConfigQuestions.Where(item => item.ConfigID == configId))
            {
                context.tbl_HVAC_ConfigQuestions.DeleteObject(question);
            }
            foreach (var key in Request.Form.AllKeys)
            {
                context.tbl_HVAC_ConfigQuestions.AddObject(new tbl_HVAC_ConfigQuestions { ConfigID = configId, OrderNum = int.Parse(key), QuestionID = int.Parse(Request.Form[key]) });
            }
            context.SaveChanges();
            return Json(new { result = true });
        }

        public ActionResult SetupFinanceOptions()
        {
            var selectedSystems = GetSelectedSystemsP().Select(i=>new PackageModel{Id = i.SystemID, PackageName = i.tbl_HVAC_SystemType.SystemTypeName}).ToArray();
            return View(selectedSystems);
        }

        public PartialViewResult GetFinanceOptions(PackageModel package)
        {
            var context = GetContext();
            var configId = GetConfigID();
            var fo = new FinanceOptionsModel
                         {
                             Id = package.Id,
                             PackageName = package.PackageName,
                             FinanceOptions = new List<FinanceOption>
                                                  {
                                                      new FinanceOption {Id = 0, Url = ""},
                                                      new FinanceOption {Id = 1, Url = ""},
                                                      new FinanceOption {Id = 2, Url = ""}
                                                  }
                         };
            if (context.tbl_HVAC_ConfigFinanceOptions.Any(item => item.SystemID == package.Id && item.ConfigID == configId))
            {
                var d = context.tbl_HVAC_ConfigFinanceOptions.First(item => item.SystemID == package.Id && item.ConfigID == configId);
                fo.FinanceOptions = new List<FinanceOption>
                                        {
                                            new FinanceOption {Id = 0, Url = d.FinanceOption1},
                                            new FinanceOption {Id = 1, Url = d.FinanceOption2},
                                            new FinanceOption {Id = 2, Url = d.FinanceOption3}
                                        };
            }

            return PartialView(fo);
        }

        public ActionResult SetFinanceOptions()
        {
            var context = GetContext();
            var configId = GetConfigID();
            var packId = int.Parse(Request.Form["Id"]);
           
            if (context.tbl_HVAC_ConfigFinanceOptions.Any(item => item.SystemID == packId && item.ConfigID == configId))
            {
                var d = context.tbl_HVAC_ConfigFinanceOptions.First(item => item.SystemID == packId && item.ConfigID == configId);
                d.FinanceOption1 = Request.Form["fo0"];
                d.FinanceOption2 = Request.Form["fo1"];
                d.FinanceOption3 = Request.Form["fo2"];
            }
            else
            {
                var d = new tbl_HVAC_ConfigFinanceOptions { ConfigID = configId, SystemID = packId };
                d.FinanceOption1 = Request.Form["fo0"];
                d.FinanceOption2 = Request.Form["fo1"];
                d.FinanceOption3 = Request.Form["fo2"];
                context.tbl_HVAC_ConfigFinanceOptions.AddObject(d);
            }
            context.SaveChanges();
            return Json(true);
        }

        public ActionResult VerificationHvacPage()
        {
            var context = new EightHundredEntities();
            var ver = new VerificationHvacData(context, GetFranchiseID(), GetConfigID());
            var listOfVerification = ver.RunVerification();
            return View(listOfVerification);
        }


    }


}
