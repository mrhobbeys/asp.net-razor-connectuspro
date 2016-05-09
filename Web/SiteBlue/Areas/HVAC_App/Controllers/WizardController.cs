using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using SiteBlue.Areas.HVAC_App.Controllers;
using SiteBlue.Areas.HVAC_App.Models;
using SiteBlue.Areas.HVAC_App.Models.HVAC_App;
using SiteBlue.Business;
using SiteBlue.Business.Invoice;
using SiteBlue.Business.Job;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;
using tbl_HVAC_ConfigFinanceOptions = SiteBlue.Data.EightHundred.tbl_HVAC_ConfigFinanceOptions;
using tbl_HVAC_ConfigVideoUrls = SiteBlue.Data.EightHundred.tbl_HVAC_ConfigVideoUrls;
using tbl_HVAC_CustomersAnswers = SiteBlue.Data.EightHundred.tbl_HVAC_CustomersAnswers;


namespace HVACapp.Areas.HVAC_App.Controllers
{
 	[BrowserCache(PreventBrowserCaching = true)]
    public class WizardController : HVACController
    {
        //
        // GET: /HVAC_App/Wizard/
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                var user = membershipService.GetUser(User.Identity.Name);
                SetCookies((Guid) user.ProviderUserKey);
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetXmlList()
        {
            if (User.Identity.IsAuthenticated)
            {
                var configID = GetConfigID();
                var jobId = GetJobCode();
                var userid = GetUserID();
                var context = new EightHundredEntities();//new HVAC_appContext();
                

                if (context.tbl_HVAC_CustomersAnswers.Any(item => item.JobID == jobId && item.UserID == userid))
                    return
                        Json(
                            context.tbl_HVAC_CustomersAnswers.First(item => item.JobID == jobId && item.UserID == userid).tbl_HVAC_Answers.OrderBy(item => item.tbl_HVAC_Questions.tbl_HVAC_ConfigQuestions.First(d => d.ConfigID == configID && d.QuestionID == item.QuestionID).OrderNum)
                                .Select
                                (
                                    item =>
                                    new
                                        {
                                            id = item.QuestionID.ToString(),
                                            question = item.tbl_HVAC_Questions.QuestionText.ToString(),
                                            answer = item.Answer,
                                            data = item.Data
                                        }
                                ).ToArray(), JsonRequestBehavior.AllowGet);
            }
            return Json(new object[] {new {id = "0", question = "Approximately how old is your home?", answer = ""}},
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetXmlList(bool editing)
        {
            var userId = GetUserID();
            
            if (editing)
            {
                var jobId = GetJobCode();
                var context = new EightHundredEntities();//new HVAC_appContext();//
                var xml = "";
                if (context.tbl_HVAC_CustomersAnswers.Any(item => item.JobID == jobId && item.UserID == userId))
                {
                    var customerAnswer =
                        context.tbl_HVAC_CustomersAnswers.First(item => item.JobID == jobId && item.UserID == userId);
                    xml = SetAnswers(context, customerAnswer);
                }
                else
                {
                    var customerAnswer = new tbl_HVAC_CustomersAnswers { JobID = jobId, UserID = userId };
                    context.tbl_HVAC_CustomersAnswers.AddObject(customerAnswer);
                    context.SaveChanges();
                    xml = SetAnswers(context, customerAnswer);
                }
                context.SaveChanges();
                return new XmlResult(xml);
            }
            return Json(new {result = 0});
        }

        private string SetAnswers(EightHundredEntities context, tbl_HVAC_CustomersAnswers customerAnswer)
        {
            //<?xml version='1.0' ?><data><action type='delete' sid='149' tid='149' ></action><action type='delete' sid='151' tid='151' ></action></data>
            var xml = "<?xml version='1.0' ?><data>";
            foreach (var num in Request.Form.Get("ids").Split(new[] {','}).Select(int.Parse))
            {
                var answerText = Request.Form.Get(num + "_answer");
                var answerData = Request.Form.Get(num + "_data");
                var status = Request.Form.Get(num + "_!nativeeditor_status");
                xml += "<action type='"+status+"' sid='"+num+"' tid='"+num+"' ></action>";
                if (customerAnswer.tbl_HVAC_Answers.Any(item => item.QuestionID == num))
                {
                    var answer = customerAnswer.tbl_HVAC_Answers.Single(item => item.QuestionID == num);
                    if (status == "delete")
                    {
                        customerAnswer.tbl_HVAC_Answers.Remove(answer);
                    }
                    else
                    {
                        answer.Answer = answerText;
                        answer.Data = answerData;
                    }
                }
                else
                {
                    if (status != "delete")
                    {
                        var answer = new tbl_HVAC_Answers
                                         {
                                             AnswerID = customerAnswer.AnswerID,
                                             QuestionID = num,
                                             Answer = answerText,
                                             Data = answerData
                                         };
                        context.tbl_HVAC_Answers.AddObject(answer);
                    }
                }
            }
            xml += "</action></data>";
            return xml;
        }

        public ActionResult Manifest()
        {
            TextWriter b = new StringWriter();
            //b.WriteLine(Url.Action("Index"));
            //b.WriteLine(Url.Content("~/scripts/codebase/touchui.css"));
            //b.WriteLine(Url.Content("~/scripts/codebase/touichui.css"));
            //b.WriteLine(Url.Content("~/scripts/codebase/touchui.js"));
            //b.WriteLine(Url.Content("~/content/completed.png"));
            //b.WriteLine(Url.Content("~/content/background.png"));
            //b.WriteLine(Url.Content("~/scripts/codebase/imgs/list_group_icon.png"));
            return new ManifestResult(b.ToString());
        }

        [HttpGet]
        public ActionResult GetJobs()
        {
            var context = GetContext();
            var franchiseId = GetFranchiseID();
            var office = context.tbl_Franchise.First(item => item.FranchiseID == franchiseId);
            var frPoint = GetPointByAddress(office.LegalPostal + "," + office.LegalAddress + "," + office.LegalCity);
            var userId = GetUserID().ToString();
            var listtemp = GetJobs(userId, context);
            var list = listtemp.Select(item =>
                new
                    {
                        id = item.JobID.ToString(),
                        Address = String.Format("{0}{1} ({2})", String.IsNullOrEmpty(item.tbl_Customer.CompanyName) ? item.tbl_Customer.CompanyName + " - " : "", item.tbl_Customer.CustomerName, item.tbl_Locations.Address),
                        point = GetPointByAddress(item.tbl_Locations.Address),
                        franchisePoint = frPoint,
                        travelingStatusId = item.StatusID
                    }).ToList();
            list.Add(new
                         {
                             id = "-1",
                             Address = "Jonh Smith (16700 park row houston, tx) DEMO",
                             point = GetPointByAddress("16700 park row houston, tx"),
                             franchisePoint = frPoint,
                             travelingStatusId = 4
                         });
            return Json(list, JsonRequestBehavior.AllowGet);  
            //test data
            //var frPoint = new Point() { x = "1", y = "1" };
            //return Json(new[]
            //                {
            //                    new
            //                        {
            //                            id = "3000",
            //                            Address = "777 Post Oak Blvd Ste 950 Houston, Texas 77056",
            //                            point =frPoint, //GetPointByAddress("777 Post Oak Blvd Ste 950 Houston, Texas 77056"),
            //                            franchisePoint = frPoint
            //                        },
            //                    new
            //                        {
            //                            id = "3002",
            //                            Address = "16700 park row houston, tx",
            //                            point = frPoint,//GetPointByAddress("16700 park row houston, tx"),
            //                            franchisePoint = frPoint
            //                        }
            //                }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<tbl_Job> GetJobs(string userId, EightHundredEntities context)
        {
            return context.tbl_Job.Where(
                item =>
                item.tbl_Job_Status.ShowInHVACYN && item.tbl_Employee.UserKey == userId &&
                item.ServiceID == 10).ToList();
        }

        [HttpPost]
        public ActionResult GetJobStatusesForTablet()
        {
            var context = GetContext();
            return
                Json(
                    context.tbl_Job_Status.Where(item => item.ShowInHVACYN).Select(
                        item => new {id = item.StatusID, value = item.Status}));
        }

        class Point {
            public string x { get; set; }
            public string y { get; set; }
        }
        private Point GetPointByAddress(string address)
        {

            var url = string.Format("http://maps.google.com/maps/geo?q={0}&output=csv", address);
            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                var stream = request.GetResponse().GetResponseStream();
                var r = new StreamReader(stream);
                var str = r.ReadToEnd().Split(new[] {','});
                if (str[0] == "200")
                {
                    return new Point {x = str[3], y = str[2]};
                }
            }
            catch (Exception)
            {}
            return new Point { x = "-95.4572870", y = "29.7534240" };
        }

        [HttpPost]
        public ActionResult ChangeJobStatus()
        {
            var jobId = GetJobCode();
            var context = GetContext();
            if (context.tbl_Job.Any(item => item.JobID == jobId))
            {
                context.tbl_Job.First(item => item.JobID == jobId).StatusID = int.Parse(Request.Form["statusid"]);
                context.SaveChanges();
                return Json(new {result = true});
            }
            return Json(new {result = false});
        }

        public ActionResult CompleteJob()
        {
            //TODO need to add security verification
            try
            {
                var jobId = GetJobCode();
                var userId = GetUserID();
                var context = new AuditedJobContext(userId, userId.ToString(), false);

                var job = context.tbl_Job.First(item => item.JobID == jobId);

                GenerateInvoice(context, job, userId);
                CompleteJob(context, job);

                context.SaveChanges();
                return Json(new {result = true}, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { result = false, message = ex.Message + "\n" + (ex.InnerException!=null?ex.InnerException.Message:"") }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EstimateJob()
        {
            try
            {
                var jobId = GetJobCode();
                var userId = GetUserID();
                var context = new AuditedJobContext(userId, userId.ToString(), false);

                var job = context.tbl_Job.First(item => item.JobID == jobId);

                GenerateInvoice(context, job, userId);
                WaitEstimateJob(context, job);

                context.SaveChanges();
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { result = false, message = ex.Message + "\n" + (ex.InnerException != null ? ex.InnerException.Message : "") }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool CompleteJob(EightHundredEntities context, tbl_Job job)
        {
            job.StatusID = 6;
            context.SaveChanges();
            return true;
        }

        private bool WaitEstimateJob(EightHundredEntities contex, tbl_Job job)
        {
            job.StatusID = 13;
            contex.SaveChanges();
            return true;
        }

        public ActionResult GenerateInvoice()
        {
            var jobId = GetJobCode();
            var userId = GetUserID();
            var context = new AuditedJobContext(userId, "From HVAC app user", false);

            var job = context.tbl_Job.First(item => item.JobID == jobId);

            GenerateInvoice(context, job, userId);

            context.SaveChanges();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        private bool GenerateInvoice(EightHundredEntities context, tbl_Job job, Guid userId)
        {
            ClearJobTasks(context, job.JobID);
            UpdatePaymentsInJob(context, job.JobID);
            var jsonData =
                context.tbl_HVAC_Answers.Single(
                    item =>
                    item.tbl_HVAC_CustomersAnswers.JobID == job.JobID && item.tbl_HVAC_CustomersAnswers.UserID == userId &&
                    item.QuestionID == 35).Data;
            /*{"Jobs":
             * [            [
                 * {"Count":"1","JobCode":"LE-HPAH136","Description":"asd","ResAccountCode":"41000","Price":7380,"TotalPrice":"$7,380.00",
                 * "Parts":
                 * [
                 * {"PartCost":3690,"PartCode":"LE-HPAH136","PartName":"LE-3 Ton Air Handler Electric Heat - 10KW","PartStdPrice":7380,"PartID":503085,"Qty":1}
                 * ],"id":1337316467855},
                 * {"Count":"1","JobCode":"LE-HPAH136","Description":"asd3","ResAccountCode":"41000","Price":7380,"TotalPrice":"$7,380.00",
                 * "Parts":
                 * [
                 * {"PartCost":3690,"PartCode":"LE-HPAH136","PartName":"LE-3 Ton Air Handler Electric Heat - 10KW","PartStdPrice":7380,"PartID":503085,"Qty":2,"TotalPrice":"$14,760.00"}
                 * ],"id":1337316467857},
                 * {"id":"ACC-CMF001","isAccessory":true,"Code":"ACC-CMF001","JobCode":"ACC-CMF001","Description":"1\" Charged Media Filter, 20 x 25","ResAccountCode":"41000","Count":1,"Price":895,"TotalPrice":"$895.00"},
                 * {"id":"ACC-HUM001","isAccessory":true,"Code":"ACC-HUM001","JobCode":"ACC-HUM001","Description":"Humidifier","ResAccountCode":"41000","Count":3,"Price":595,"TotalPrice":"$1,785.00"}
             * ],
             * "MainSystem":{"Parts":[{"PartCost":3690,"PartCode":"LE-HPAH136","PartName":"LE-3 Ton Air Handler Electric Heat - 10KW","PartStdPrice":7380,"PartID":503085,"Qty":1}],"TotalPrice":7380,"id":"LE","AFUE":"80","SEER":"12","JobCodeId":0,"JobCode":"LE-HPAH136","ResAccountCode":"41000","Description":"SEER Rating 12, AFUE 80, LE-3 Ton Air Handler Electric Heat - 10KW","Count":1,"Price":7380},"TotalAmount":24820,"Tax":0,"GrandTotal":24820,"TaxRate":0} */
            //{"Jobs":[{"Count":"1","Description":"SEER Rating 16, AFUE 29, SE-2.5 Ton Gas Furnace Vertical","Price":12488,"TotalPrice":"$12,488.00","Parts":[{"PartCost":6244,"PartCode":"SE-HPGFV30","PartName":"SE-2.5 Ton Gas Furnace Vertical","PartStdPrice":12488,"PartID":503287,"Qty":1}],"id":1331745863188},{"id":"ACC-CMF001","isAccessory":true,"Code":"ACC-CMF001","Description":"1\" Charged Media Filter, 20 x 25","Count":1,"Price":895,"TotalPrice":"$895.00"},{"Count":"1","Description":"SEER Rating 16, AFUE 29, SE-2.5 Ton Gas Furnace Vertical","Price":12488,"TotalPrice":"$12,488.00","Parts":[{"PartCost":6244,"PartCode":"SE-HPGFV30","PartName":"SE-2.5 Ton Gas Furnace Vertical","PartStdPrice":12488,"PartID":503287,"Qty":1}],"id":1331745863190}],"MainSystem":{"Parts":[{"PartCost":6244,"PartCode":"SE-HPGFV30","PartName":"SE-2.5 Ton Gas Furnace Vertical","PartStdPrice":12488,"PartID":503287,"Qty":1}],"TotalPrice":12488,"id":"SE","AFUE":"29","SEER":"16","JobCodeId":0,"JobCode":"SE-HPGFV30","Description":"SEER Rating 16, AFUE 29, SE-2.5 Ton Gas Furnace Vertical","Count":1,"Price":12488},"TotalAmount":25871,"Tax":0,"GrandTotal":25871,"TaxRate":0}
            //var jsonData = "{\"Jobs\":[" +
            //            "{\"JobCode\":\"SE-HPGFV30\",\"Count\":\"1\",\"Description\":\"SEER Rating 16, AFUE 29, SE-2.5 Ton Gas Furnace Vertical\",\"Price\":12488,\"TotalPrice\":\"$12,488.00\",\"Parts\":[{\"PartCost\":6244,\"PartCode\":\"SE-HPGFV30\",\"PartName\":\"SE-2.5 Ton Gas Furnace Vertical\",\"PartStdPrice\":12488,\"PartID\":503287,\"Qty\":1}],\"id\":1331745863188}," +
            //            "{\"id\":\"ACC-CMF001\",\"isAccessory\":true,\"Code\":\"ACC-CMF001\",\"JobCode\":\"ACC-CMF001\",,\"Description\":\"1\\\" Charged Media Filter, 20 x 25\",\"Count\":1,\"Price\":895,\"TotalPrice\":\"$895.00\"}," +
            //            "{\"JobCode\":\"SE-HPGFV30\",\"Count\":\"1\",\"Description\":\"SEER Rating 16, AFUE 29, SE-2.5 Ton Gas Furnace Vertical\",\"Price\":12488,\"TotalPrice\":\"$12,488.00\",\"Parts\":[{\"PartCost\":6244,\"PartCode\":\"SE-HPGFV30\",\"PartName\":\"SE-2.5 Ton Gas Furnace Vertical\",\"PartStdPrice\":12488,\"PartID\":503287,\"Qty\":1}],\"id\":1331745863190}]," +
            //        "\"MainSystem\":{\"Parts\":[{\"PartCost\":6244,\"PartCode\":\"SE-HPGFV30\",\"PartName\":\"SE-2.5 Ton Gas Furnace Vertical\",\"PartStdPrice\":12488,\"PartID\":503287,\"Qty\":1}],\"TotalPrice\":12488,\"id\":\"SE\",\"AFUE\":\"29\",\"SEER\":\"16\",\"JobCodeId\":0,\"JobCode\":\"SE-HPGFV30\",\"Description\":\"SEER Rating 16, AFUE 29, SE-2.5 Ton Gas Furnace Vertical\",\"Count\":1,\"Price\":12488}," +
            //        "\"TotalAmount\":25871," +
            //        "\"Tax\":0," +
            //        "\"GrandTotal\":25871," +
            //        "\"TaxRate\":0}";
            //var jsonData33 = context.tbl_HVAC_Answers.Single(
            //        item =>
            //        item.tbl_HVAC_CustomersAnswers.JobID == jobId && item.tbl_HVAC_CustomersAnswers.UserID == userId &&
            //        item.QuestionID == 33).Data;
            
            var prMainJobs = DeserialiseMainJobs(jsonData);
            var prAccessories = DeserialiseAccessoriesJobs(jsonData);

            SetJobTasks(context, prMainJobs, prAccessories);

            var pr = JSONHelper.Deserialise<PriceListModel<SystemInfoModelWithParts>>(jsonData);
            job.SubTotal = decimal.Parse(pr.TotalAmount);
            job.TaxAmount = decimal.Parse(pr.Tax);
            job.TotalSales = decimal.Parse(pr.GrandTotal);

            context.SaveChanges();
            return true;
        }

        private IEnumerable<SystemInfoModelWithParts> DeserialiseMainJobs(string jsonData)
        {
            var temp = JSONHelper.Deserialise<PriceListModel<SystemInfoModelWithParts>>(jsonData);
            return temp.Jobs.Where(job => job.Parts.Count!=0).ToArray();
        }

        private IEnumerable<AccessoryModel> DeserialiseAccessoriesJobs(string jsonData)
        {
            var temp = JSONHelper.Deserialise<PriceListModel<AccessoryModel>>(jsonData);
            return temp.Jobs.Where(i => !String.IsNullOrEmpty(i.Code)).ToArray();
        }

        private void UpdatePaymentsInJob(EightHundredEntities context, int jobId)
        {
            var payments=context.tbl_Payments.Where(item => item.JobID == jobId).ToList();
            var hvacPayments = GetValue(context, jobId);
            foreach (var payment in payments)
            {
                if (hvacPayments.payments.All(i => i.id != payment.PaymentID))
                    context.tbl_Payments.DeleteObject(payment);
            }
            foreach (var paymentItem in hvacPayments.payments)
            {
                if (payments.All(i => i.PaymentID != paymentItem.id))
                {
                    context.tbl_Payments.AddObject(new tbl_Payments
                                                       {
                                                           CheckNumber = paymentItem.code, CreateDate = DateTime.Now,
                                                           DepositID = 0, DepositStatus = false, DriversLicNUm = null, ErrorFlag = false,
                                                           ExceptionComments = null, FranchiseID = GetFranchiseID(), JobID = jobId, 
                                                           PaymentAmount = decimal.Parse(paymentItem.payment, NumberStyles.Any),
                                                           PaymentDate = DateTime.Now, PaymentTypeID = paymentItem.typeId
                                                       });
                }
            }

            context.SaveChanges();

            //{"email":"","payments":[{"id":2,"type":"AmEx","payment":"$2,042.93","code":""}],"total_amount":12736.06}
        }

 	    private Answer33 GetValue(EightHundredEntities context, int jobId)
 	    {
 	        var userId = GetUserID();
 	        var jsonData =
 	            context.tbl_HVAC_Answers.Single(
 	                item =>
 	                item.tbl_HVAC_CustomersAnswers.JobID == jobId && item.tbl_HVAC_CustomersAnswers.UserID == userId &&
 	                item.QuestionID == 33).Data;
 	        var hvacPayments = JSONHelper.Deserialise<Answer33>(jsonData);
 	        return hvacPayments;
 	    }

 	    private void ClearJobTasks(EightHundredEntities context, int jobId)
        {
            var listofMainTask = context.tbl_Job_Tasks.Where(item => item.JobID == jobId).ToList();
            var listOfMainTaskId = listofMainTask.Select(i => i.JobTaskID);
            var listofPart =
                context.tbl_Job_Task_Parts.Where(item => listOfMainTaskId.Contains(item.JobTaskID)).ToList();
            foreach (var jobTaskPart in listofPart)
            {
                context.tbl_Job_Task_Parts.DeleteObject(jobTaskPart);
            }
            foreach (var tblJobTask in listofMainTask)
            {
                context.tbl_Job_Tasks.DeleteObject(tblJobTask);
            }
            context.SaveChanges();
        }

        private void SetJobTasks(EightHundredEntities context, IEnumerable<SystemInfoModelWithParts> prMainJobs, IEnumerable<AccessoryModel> prAccessories)
        {
            var frId = GetFranchiseID();
            var priceBookId = GetPriceBookId(context, frId);
            var jobId = GetJobCode();

            foreach (var jobtask in prMainJobs)
            {
                var qty = 1;
                //var jobDesription = context.View_HVAC_APP.First(i => i.JobCode == jobtask.JobCode).JobCodeDescription;
                var jobTask = new tbl_Job_Tasks
                                  {
                                      JobID = jobId,
                                      JobCodeDescription = jobtask.Description,
                                      JobCode = jobtask.JobCode,
                                      AccountCode = jobtask.ResAccountCode,
                                      AddOnYN = false,
                                      AuthorizedYN = true,
                                      MemberYN = false,
                                      Cost = jobtask.Parts.Sum(item => item.PartStdPrice * item.Qty),
                                      Quantity = qty,
                                      JobCodeID = jobtask.JobCodeId,
                                      Price = jobtask.Parts.Sum(item=>item.PartStdPrice*item.Qty), AdjustedPrice = 0, ErrorFlag = false, HomeGuardLink = 0, HomeGuardPrice = 0, LinePrice = 0, TabletTaskID = 0, UnitPrice = 0
                                  };
                context.tbl_Job_Tasks.AddObject(jobTask);
                context.SaveChanges();
                foreach (var listpart in jobtask.Parts.Where(listpart => listpart.Qty != 0))
                {
                    context.tbl_Job_Task_Parts.AddObject(new tbl_Job_Task_Parts
                                                             {
                                                                 Cost = listpart.PartCost,
                                                                 PartCode = listpart.PartCode,
                                                                 PartName = listpart.PartName,
                                                                 Price = listpart.PartStdPrice,
                                                                 PartsID = listpart.PartID,
                                                                 Quantity = listpart.Qty,
                                                             });
                }
                context.SaveChanges();
            }

            var codes = prAccessories.Select(i => new KeyValuePair<string, AccessoryModel>(i.JobCode, i));

            var jobcodes = codes.Select(item => item.Key);
            //var list = context.View_HVAC_APP.Where(item => item.PriceBookID == priceBookId).Where(
            //    i => jobcodes.Contains(i.JobCode)).ToList();
            var listparts =
                context.View_HVAC_APP_Parts.Where(item => item.PriceBookID == priceBookId).Where(
                    i => jobcodes.Contains(i.JobCode)).ToList();

            foreach (var jobtask in codes.Select(i => i.Value))
            {
                var job_task = new tbl_Job_Tasks
                                   {
                                       JobID = jobId,
                                       JobCodeDescription = jobtask.Description,
                                       JobCode = jobtask.JobCode,
                                       AccountCode = jobtask.ResAccountCode,
                                       AddOnYN = false,
                                       AuthorizedYN = true,
                                       MemberYN = false,
                                       Cost = jobtask.Price,
                                       Quantity = jobtask.Count,
                                       JobCodeID = jobtask.JobCodeId,
                                       Price = jobtask.Price, AdjustedPrice = 0, ErrorFlag = false, HomeGuardLink = 0, HomeGuardPrice = 0, LinePrice = 0, TabletTaskID = 0, UnitPrice = 0
                                   };
                context.tbl_Job_Tasks.AddObject(job_task);
                context.SaveChanges();
                foreach (var listpart in listparts.Where(i => i.JobCode == job_task.JobCode))
                {
                    context.tbl_Job_Task_Parts.AddObject(new tbl_Job_Task_Parts
                                                             {
                                                                 Cost = listpart.PartCost,
                                                                 PartCode = listpart.PartCode,
                                                                 PartName = listpart.PartName,
                                                                 Price = listpart.PartStdPrice,
                                                                 JobTaskID = job_task.JobTaskID,
                                                                 PartsID = listpart.PartID,
                                                                 Quantity = listpart.Qty*job_task.Quantity,
                                                             });
                }
                context.SaveChanges();
            }
        }

        private int GetPriceBookId(EightHundredEntities context, int frId)
        {
            var priceBook = context.tbl_HVAC_ConfigFranchise.First(item => item.FranchiseID == frId).PricebookID;
            var priceBookId = priceBook.HasValue ? priceBook.Value : 177;
            return priceBookId;
        }

        public ActionResult SendInvoiceByEmail()
        {
            var jobId = GetJobCode();
            var context = new EightHundredEntities();
            var userId = GetUserID();
            var jsonData33 = context.tbl_HVAC_Answers.Single(
                item =>
                item.tbl_HVAC_CustomersAnswers.JobID == jobId && item.tbl_HVAC_CustomersAnswers.UserID == userId &&
                item.QuestionID == 33).Data;

            var em = JSONHelper.Deserialise<EmailClass>(jsonData33);

            if (SendInvoiceByEmail(em, userId, jobId).Success)
            {
                return Json(new {result = true});
            }
            return Json(new {result = false});
        }

        private OperationResult<bool> SendInvoiceByEmail(EmailClass em, Guid userId, int jobId)
        {
            return AbstractBusinessService.Create<InvoiceService>(userId).SendToCustomer(jobId, em.email);
        }

        [HttpGet]
        public ActionResult GetVideoUrls(string sysTypeText)
        {
            var configId = GetConfigID();
            var context = new EightHundredEntities();
            var urls = context.tbl_HVAC_ConfigVideoUrls.Where(item => item.ConfigID == configId).ToList();
            if (sysTypeText == "ALL")
            {
                return Json(urls.Select(item => new
                                                    {
                                                        sys = item.tbl_HVAC_SystemType.SystemTypeName,
                                                        url = item.VideoUrl
                                                    }).ToArray(), JsonRequestBehavior.AllowGet);
            }
            if (urls.Any(item => item.tbl_HVAC_SystemType.SystemTypeName == sysTypeText))
            {
                return
                    Json(
                        new { url = urls.Single(item => item.tbl_HVAC_SystemType.SystemTypeName == sysTypeText).VideoUrl },
                        JsonRequestBehavior.AllowGet);
            }
            return Json(new {url = "no url"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSelectedVideoUrls()
        {
            int configId = GetConfigID();
            var context = new EightHundredEntities();
            return Json(context.tbl_HVAC_ConfigSystems.Where(item => item.ConfigID == configId).OrderBy(item => item.OrderNum).Select(
                item => new
                            {
                                url = item.tbl_HVAC_SystemType.tbl_HVAC_ConfigVideoUrls.FirstOrDefault(a => a.ConfigID == configId).VideoUrl,
                                sys = item.tbl_HVAC_SystemType.SystemTypeName
                            }).ToList());
        }

        public ActionResult GetFinanceOptions()
        {
            var configId = GetConfigID();
            var context = new EightHundredEntities();
            var listOffinanceOptions =
                context.tbl_HVAC_ConfigFinanceOptions.Where(item => item.ConfigID == configId).ToList();
            return Json(listOffinanceOptions.Select(item =>
                                                    new
                                                        {
                                                            sys = item.tbl_HVAC_SystemType.SystemTypeName,
                                                            item.FinanceOption1,
                                                            item.FinanceOption2,
                                                            item.FinanceOption3
                                                        }).ToArray());
        }

        [HttpPost]
        public ActionResult ClearAnswers()
        {
            //TODO Verify security
            //var user = membershipService.GetUser(User.Identity.Name);
            //if (user.ProviderUserKey != null)
            //{
                //var userID = (Guid) user.ProviderUserKey;
                var userId = GetUserID();
                var httpCookie = Request.Cookies["id_job"];
                if (httpCookie != null)
                {
                    var jobID = int.Parse(httpCookie.Value.Replace("\"", ""));
                    var context = new EightHundredEntities();
                    var customerAnswer = context.tbl_HVAC_CustomersAnswers.First(item => item.JobID == jobID && item.UserID == userId);
                    foreach (var source in customerAnswer.tbl_HVAC_Answers.ToArray())
                    {
                        context.tbl_HVAC_Answers.DeleteObject(source);
                    }
                    context.tbl_HVAC_CustomersAnswers.DeleteObject(customerAnswer);
                    context.SaveChanges();
                }
            //}
            return Json(new {result = "success"});
        }

        [HttpPost]
        public ActionResult GetCompanyTaxes()
        {
            var context = new EightHundredEntities();
            var franchiseId = GetFranchiseID();
            return Json(context.tbl_TaxRates.Where(item => item.FranchiseId == franchiseId).ToList().Select(
                item => new {Tax = item.TaxDescription, Value = item.LaborAmount.ToString().Replace(',','.')}));
        }

        public ActionResult GetForChooseSystem()
        {
            //var configID = 1;
            var context = new EightHundredEntities();
            var listOfAll = context.tbl_HVAC_SystemType.Select(item => item.SystemTypeName).ToList();

            var selectedList = GetSelectedSystemsP();

            return Json(listOfAll.Except(selectedList).Select(i => new { text = i }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSelectedSystems()
        {
            return Json(GetSelectedSystemsP().Select(i => new { text = i }), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<string> GetSelectedSystemsP()
        {
            int configID = GetConfigID();
            var context = new EightHundredEntities();
            var list =
                context.tbl_HVAC_ConfigSystems.Where(item => item.ConfigID == configID).OrderBy(i=>i.OrderNum).Select(
                    item => item.tbl_HVAC_SystemType.SystemTypeName).ToList();
            return list;
                                                 
        }

        public ActionResult GetGuaranteeTexts()
        {
            //26 - Comfort Guarantee (cg)
            //27 - Lemon Free Guarantee (lfg)
            //28 - Never Undersold Guarantee (nug)
            //29 - Home Respect Guarantee (hrg)
            //30 - Complete Satisfaction Guarantee (csg)
            //36 - 
            //37 - 
            var context = new EightHundredEntities();
            var configId = GetConfigID();
            var list = context.tbl_HVAC_ConfigGuaranteeTexts.Where(item => item.ConfigID == configId).ToList();
            var result = new GuaranteeTexts();
            result.SetValuesFromList(list);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetImagesUrl()
        {
            int configID = GetConfigID();
            var context = new EightHundredEntities();
            var img = new ImagesUrl
                                {
                                    Urls = context.tbl_HVAC_ConfigReliableInstallationsUrl.Where(item=>item.ConfigID==configID).Select(item=>item.UrlText).ToList()
                                };
            var img2 = new ImagesUrl
                                 {
                                     Urls = context.tbl_HVAC_ConfigWhoWeAreTexts.Where(item=>item.ConfigID==configID).Select(item=>item.Text).ToList()
                                 };
            return Json(new { ri = img, wwa=img2 });
        }

        public ActionResult GetLogo()
        {
            var configId = GetConfigID();
            var context = new EightHundredEntities();
            var listofurls = context.tbl_HVAC_ConfigLogoUrl.Where(item => item.ConfigID == configId).ToList();
            return Json(listofurls.Count != 0 ? new {url = listofurls.First().Logourl} : new {url = "../../../Areas/hvac_app/content/bigimage.png"});
        }

        public ActionResult SaveGPSPoint()
        {
            var context = new EightHundredEntities();
            var franchiseId = GetFranchiseID();
            var userId = GetUserID();
            context.AddTotbl_GPS_Tracks(new tbl_GPS_Tracks
                                            {
                                                FranchiseId = franchiseId,
                                                Lat = float.Parse(Request.Form["Lat"].Replace('.',',')),
                                                Lng = float.Parse(Request.Form["Lng"].Replace('.',',')),
                                                UserId = userId,
                                                TimeRecord = DateTime.Parse(Request.Form["TimeRecord"])
                                            });
            context.SaveChanges();
            return Json(new {result = true});
        }

        public ActionResult PrintInvoice(int id)
        {
            var contentType = "application/pdf";
            var result = AbstractBusinessService.Create<InvoiceService>(UserInfo.UserKey).GetInvoicePdf(id, true);
            var fileName = result.ResultData.Key;
            var data = result.ResultData.Value;

            if (!result.Success)
            {
                contentType = "text/plain";
                fileName = "error.txt";
                data = Encoding.Unicode.GetBytes(result.Message);
            }

            return new FileStreamResult(new MemoryStream(data), contentType) { FileDownloadName = fileName };
        }

        [HttpPost]
        public ActionResult GetPaymentTypes()
        {
            var context = GetContext();
            return Json(context.tbl_Payment_Types.Where(i => i.SendToTabletYN).Select(item => new HvacPaymentType(){Id=item.PaymentTypeId, Name = item.PaymentType}));
        }

        [HttpPost]
        public ActionResult GetPayments()
        {
            //var context = GetContext();
            //var jobId = GetJobCode();
            //var answer = GetValue(context, jobId);
            //var payments = context.tbl_Payments.Where(item => item.JobID == jobId).ToList();
            //foreach (var payment in payments)
            //{
            //    if (answer.payments.All(item => item.id != payment.PaymentID))
            //        answer.payments.Add(new HvacPaymentItem { id = payment.PaymentID, code = payment.CheckNumber, payment = String.Format("{0:c}",payment.PaymentAmount), typeId = payment.PaymentTypeID});
            //}
            return Json(new object());
        }
    }
}
