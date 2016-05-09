using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Areas.OwnerPortal.Models;
using System.Drawing;
using SiteBlue.Data.EightHundred;
using System.Resources;
using System.Reflection;
using SiteBlue.Controllers;
//using Microsoft.BusinessSolutions.SmallBusinessAccounting;
using SiteBlue.Business;
using DHTMLX.Export.Excel;


namespace SiteBlue.Areas.OwnerPortal.Controllers
{
	[Authorize(Roles = "CompanyOwner,Corporate")]
	public class ARController : SiteBlueBaseController
	{
		//
		// GET: /OwnerPortal/AR/
		//OwnerPortalContext DB = new OwnerPortalContext();
		EightHundredEntities DB = new EightHundredEntities();

		//internal ISmallBusinessInstance smallBusinessInstance = default(ISmallBusinessInstance);
		private string LoadScreen;
		string gbZeeAcctType = "MSA";
		internal bool SBASuccess = false;
		//public ISbaObjects sbaObject;
		internal ResourceManager isvResources;

		public ActionResult ARList()
		{
			var FranchiseeList = (from p in DB.tbl_Franchise
								  join o in DB.tbl_Franchise_Owner on p.OwnerID equals o.OwnerID
								  orderby p.FranchiseNUmber
								  select new
								  {
									  FranchiseID = p.FranchiseID,
									  FranchiseNumber = p.FranchiseNUmber + " - " + o.LegalName
								  }).ToList();
			ViewBag.frenchise = FranchiseeList;
			var objFranchisee = (from e in DB.tbl_Franchise
								 join o in DB.tbl_Franchise_Owner on e.OwnerID equals o.OwnerID
								 where e.FranchiseID == 56
								 select e).FirstOrDefault();
			ViewBag.FranchiseeId = objFranchisee.FranchiseID;
			ViewBag.FranchiseeNumber = objFranchisee.FranchiseNUmber + "-" + objFranchisee.LegalName;
			List<string> lstaddjustment = new List<string>();
			lstaddjustment.Add("");
			lstaddjustment.Add("58000 Plumbing Discount/Coupon/Adjustment");
			lstaddjustment.Add("59100 Electrical Discount/Coupon/Adjustment");
			lstaddjustment.Add("59000 HVAC      Discount/Coupon/Adjustment");
			lstaddjustment.Add("65900 WriteOff");

			ViewBag.AdjustmentAccount = lstaddjustment;

		   return View();
		}
		public ActionResult ARResults(string franchiseid, string strscreeen)
		{
            var objmodcommon = new mod_common(UserInfo.UserKey);

			decimal balance = 0;
			int fid = 0;
			if (franchiseid != "")
			{
				fid = Convert.ToInt32(franchiseid);
			}
			
			var ARList = (from a in DB.tbl_Job where a.Balance != 0 && a.FranchiseID == fid && a.InvoicedDate != null select a);
			int iARListCount = ARList.Count();
			

			string _Type = string.Empty;
			if (strscreeen == "" && strscreeen == null)
			{
				LoadScreen = "All";
			}
			else
			{
				LoadScreen = strscreeen;
			}
			DateTime? dt1;
			DateTime? dt2;
			switch (LoadScreen)
			{
				case "Current":
					{
						dt1 = objmodcommon.getCurrentDate();
						ARList = (from a in ARList where a.InvoicedDate > dt1 select a);
					} break;
				case "3060":
					{
						dt1 = objmodcommon.getCurrentDate();
						dt2 = objmodcommon.get60dayDate();
						ARList = (from a in ARList where a.InvoicedDate <= dt1 && a.InvoicedDate > dt2  select a);
						
					} break;
				case "6090":
					{
						dt1 = objmodcommon.get60dayDate();
						dt2 = objmodcommon.get90dayDate();
						ARList = (from a in ARList where a.InvoicedDate <= dt1 && a.InvoicedDate > dt2 select a);
					
					} break;
				case "Over90":
					{
						dt1 = objmodcommon.get90dayDate();
						ARList = (from a in ARList where a.InvoicedDate <= dt1 select a);

					} break;
				default:
					{
						
					} break;
			}

			Details objdetails = new Details();
			List<Details> lstdetails = new List<Details>();
			Details summarydetails;
			foreach (var arrec in ARList)
			{
				//this sub loads a line into the listview setting the color property properly
				
				int jobid  = arrec.JobID;

				 //set the correct color for the even lines
				if(iARListCount%3 == 0)
				{
					ViewBag.strcolor = Color.FromArgb(216, 228, 248);
				}

				//load the data
				objdetails.Invoicenumber = arrec.JobID;
				var customer = objmodcommon.GetCustomers(arrec.CustomerID);

				if(customer != null) {
					objdetails.BillTo = objmodcommon.GetCustomerName(customer).ToString();
				}else{
					objdetails.BillTo = "";
				}

				var location = objmodcommon.getLocation(arrec.LocationID);
				if(location != null) {
					objdetails.JobLocation = location.Address.ToString();
				}
				

				if(arrec.InvoicedDate != null)
				{
					objdetails.age = objmodcommon.getAge(arrec.InvoicedDate);
					objdetails.ClosedDate = arrec.InvoicedDate;
				}else{
					objdetails.age = ""; //add nothing for age
					objdetails.ClosedDate = objmodcommon.ParseNullableDateTime("");
				}

				var techlist = (from t in DB.tbl_Job_Technicians where t.JobID == jobid && t.PrimaryYN == true select t);
				int techID = 0;
				foreach (var Techrec in techlist)
				{
				  techID =  Techrec.ServiceProID;
				}
				
				objdetails.Tech = objmodcommon.Get_Employee_Name(techID);

				objdetails.strJobAmt = string.Format("{0:C}", arrec.TotalSales);
				objdetails.strBalance =  string.Format("{0:C}", arrec.Balance);

				balance = balance + arrec.Balance;

				objdetails.totalbalance = string.Format("{0:C}", balance);

				var contactlist = (from c in DB.tbl_Contacts 
								   join p in DB.tbl_PhoneType on  c.PhoneTypeID equals p.PhoneTypeID into righttableresults
								   from p in righttableresults.DefaultIfEmpty()
								  where (c.CustomerID == customer.CustomerID && p.PhoneType == "Primary")
								  select new { c, p });
				var phone = "";
				foreach (var contactrec in contactlist)
				{
					phone = contactrec.c.PhoneNumber;
				}
				
				objdetails.Phone = objmodcommon.Format_PhoneNumber(phone);

				summarydetails = new Details
								{
									
									Invoicenumber = objdetails.Invoicenumber,
									BillTo = objdetails.BillTo,
									JobLocation = objdetails.JobLocation,
									age = objdetails.age,
									ClosedDate = objdetails.ClosedDate,
									Tech = objdetails.Tech,
									strJobAmt = objdetails.strJobAmt,
									strBalance = objdetails.strBalance,
									Phone = objdetails.Phone,
									totalbalance = objdetails.totalbalance,
									shortdatestring = objdetails.ClosedDate.Value.ToShortDateString()
								};

				
				lstdetails.Add(summarydetails);

				
			}

			return Json(lstdetails);

			
		}

		private bool verifyAdjAccounts(string franchiseid, string invoiceid,string amount, string accountid, string comments)
		{

			try {
				//int i = 0;
				string holdprimarykey = "";
				string holdcustPrimKey = "";
				int fid = Convert.ToInt32(franchiseid);
				var customerlist = (from c in DB.tbl_AccountingCustomers where c.CustomerName == "800Receivables" & c.FranchiseID == fid select c);
				foreach (var customer_loopVariable in customerlist) {
					
					holdcustPrimKey = customer_loopVariable.PrimaryKey.ToString();
				}
				if (string.IsNullOrEmpty(holdcustPrimKey)) {
					ViewBag.lblmessage = "The CUSTOMER 800Receivables needs to be setup in your accouting program before transferring.";
					return false;
				}

				holdprimarykey = "";
				var ARAccountlist = (from a in DB.tbl_AccountingAccounts where a.AccountDesc == "Accounts Receivable" && a.FranchiseID == fid select a);
				foreach (var araccount_loopVariable in ARAccountlist) {
					
					holdprimarykey = araccount_loopVariable.PrimaryKey.ToString();
				}
				if (string.IsNullOrEmpty(holdprimarykey)) {
					ViewBag.lblmessage = "The Account Accounts Receivable needs to be setup in your accounting program before transferring.";
					return false;
				}

				return true;

			} catch (Exception ex) {
				ViewBag.lblmessage = "Verify Accounts: " + ex.Message;
				return false;
			}

		}
		//public void Write_Adjustment_Journal_Entries(string myInvoiceID, string MyAdjAmtText, string MyAdjAccounttext)
		//{
		//    //open a session
		//    if (Open_QB_Session() == false)
		//        return;

		//    try
		//    {
		//        //create a  message set
		//        IMsgSetRequest requestMsgSet = default(IMsgSetRequest);
		//        requestMsgSet = sessMgr.CreateMsgSetRequest("US", gbQBMasterVersion, 0.0);
		//        requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

		//        //now build the requests
		//        BuildJournalEntryAddRq_ForAdjustments(requestMsgSet, myInvoiceID, MyAdjAmtText, MyAdjAccounttext);

		//        //Send the request and get the response from QuickBooks
		//        IMsgSetResponse responseMsgSet = default(IMsgSetResponse);
		//        responseMsgSet = sessMgr.DoRequests(requestMsgSet);

		//        //End the session and close the connection to QuickBooks
		//        sessMgr.EndSession();
		//        sessMgr.CloseConnection();

		//        WalkJournalEntryAddRs_ForAdjustments(responseMsgSet, myInvoiceID, MyAdjAmtText, MyAdjAccounttext);

		//        sessMgr = null;
		//    }
		//    catch (Exception ex)
		//    {
		//        Interaction.MsgBox("Unable to make mani loop due to:" + ex.Message, MsgBoxStyle.Information, "MSG");
		//    }

		//}
		private void Apply_Adjustment_ForQB(string franchiseid, string invoiceid,string amount, string accountid, string comments)
		{
			try {
				//verify all acounts 
				if (verifyAdjAccounts(franchiseid, invoiceid, amount, accountid, comments)) {
					string CustPrimKey = "";
					//string UndepPrimKey = "";
					//string salesPrimKey = "";
					int fid = Convert.ToInt32(franchiseid);
					var customerlist = (from c in DB.tbl_AccountingCustomers where c.CustomerName == "800Receivables" && c.FranchiseID == fid select c);
					foreach (var customer_loopVariable in customerlist) {
						
						CustPrimKey = customer_loopVariable.PrimaryKey.ToString();
					}

					//transfer Deposits
					try {
						//Write_Adjustment_Journal_Entries(this.txt_InvNUmber.Text, this.txt_Adjamt.Text, this.cmb_AdjAccounts.Text);

						

					} catch (Exception ex) {
						ViewBag.lblmessage = "Transfer Adjustments error: " + ex.Message;
						return;
					}

				}

			} catch (Exception ex) {
				ViewBag.lblmessage = "Verify Accounts before Transfer: " + ex.Message;
			}

		}
		private void InitializeSBA_Adjustments(string franchiseid, string invoiceid,string amount, string accountid, string comments)
		{
			// Load resource file
			try
			{
				isvResources = new ResourceManager(resourceFILE, Assembly.GetExecutingAssembly());

				// set ISmallBusinessInstance, IFormsFactory
				//ISmallBusinessInstance objsmall;
				//objsmall = (ISmallBusinessInstance)sbaObject.SmallBusinessInstance;
			}
			catch (Exception ex)
			{
				ViewBag.lblmessage = "Initialize Connection to Accounting: " + ex.Message;
			}

			try {
				string CustPrimKey = "";
				//string UndepPrimKey = "";
				//string salesPrimKey = "";
				int fid = Convert.ToInt32(franchiseid);

				//adjustments
				try {
					//int i = 0;
					System.DateTime holddate = default(System.DateTime);
					//double holdAR = 0;
					//bool newrecord = true;
					holddate = DateTime.Now;

					//IJournalEntry ijournal = default(IJournalEntry);
					//ijournal = (IJournalEntry)ijournal.CreateJournalEntryLine(DocumentLineType.JournalLine);

					//ijournal.FinancialDateTime = holddate;
					//ijournal.Memo = "800Plumber Adjustment";

					string AccountKey = "";
					string AccountNum = "";

					switch (accountid.Substring(1, 5)) {
						case "59400":
							//plumbnig
							AccountNum = "59400";
							break;
						case "59450":
							//electrical
							AccountNum = "59450";
							break;
						case "59500":
							//hvac
							AccountNum = "59500";
							break;
						case "65900":
							//writeoff
							AccountNum = "65900";
							break;
						default:
							AccountNum = "";
							break;
					}

					//mark the adjustment entry as a debit
					var salesaccountlist = (from a in DB.tbl_AccountingAccounts where a.AccountNum == AccountNum & a.FranchiseID == fid select a);
					foreach (var account_loopVariable in salesaccountlist) {
						
						AccountKey = account_loopVariable.PrimaryKey.ToString();
					}

					//IJournalEntryLine journalline = (IJournalEntryLine)ijournal.CreateJournalEntryLine(DocumentLineType.JournalLine);
					//journalline.Account = (IAccount)smallBusinessInstance.FinancialAccounts.GetByPrimaryKey(Convert.ToInt32(AccountKey));
					//journalline.Debit = Convert.ToInt32(amount);

					//now credit the AR Account
					var customerlist = (from c in DB.tbl_AccountingCustomers where c.CustomerName == "800Receivables" & c.FranchiseID == fid select c);
					foreach (var customer_loopVariable in customerlist) {

						CustPrimKey = customer_loopVariable.PrimaryKey.ToString();
					}

					//IJournalEntryLine journalline4 = (IJournalEntryLine)ijournal.CreateJournalEntryLine(DocumentLineType.JournalLine);
					//journalline4.Account = (IAccount)smallBusinessInstance.CustomerAccounts.GetByPrimaryKey(Convert.ToInt32(CustPrimKey));
					//journalline4.Credit = Convert.ToInt32(amount);

					//ijournal.Save();
					//string RefNum = ijournal.DocumentDisplayNumber;
					//ViewBag.lblmessage = "You adjustment number is:" + RefNum;
					SBASuccess = true;

				} catch (Exception ex) {
					ViewBag.lblmessage = "Adjustment Sales Error: " + ex.Message;
					SBASuccess = false;
				}

			} catch (Exception ex) {
				ViewBag.lblmessage = "Verify Accounts before Transfer: " + ex.Message;
				SBASuccess = false;
			}
		}
		public void ApplyAdjustment(string franchiseid, string invoiceid,string amount, string accountid, string comments)
		{
			if (gbZeeAcctType == "QB") {
					Apply_Adjustment_ForQB(franchiseid, invoiceid, amount, accountid, comments);
				} else if (gbZeeAcctType == "PV") {
				//do nothing
				} else {
					//MSA system

					//adjust job payments
					try {
						SBASuccess = false;
						//SBAAppLoader.Main();

						// set sbaObjects

						// Initialize form
						//adjust msa accounting 
						InitializeSBA_Adjustments(franchiseid, invoiceid, amount, accountid, comments);
						

						if (SBASuccess == true) {
							tbl_Payments newpayment = new tbl_Payments();

							newpayment.FranchiseID = Convert.ToInt32(franchiseid);
							newpayment.JobID = Convert.ToInt32(invoiceid);
							newpayment.PaymentAmount = Convert.ToDecimal(amount);
							newpayment.PaymentDate = DateTime.Now;
							newpayment.DepositStatus = true;
							newpayment.CreateDate = DateTime.Now;
							//posted
							if (accountid == "65900 WriteOff") {
								newpayment.PaymentTypeID = -1;
								//writeoff
								newpayment.DepositID = -1;
							} else {
								newpayment.PaymentTypeID = -2;
								//discount
								newpayment.DepositID = -2;
							}
							DB.tbl_Payments.AddObject(newpayment);
							DB.SaveChanges();

							try {
								int ijobid = Convert.ToInt32(invoiceid);
								
								var Jobrec = (from J in DB.tbl_Job where J.JobID == ijobid select J).Single();
								Jobrec.Balance = Jobrec.Balance - Convert.ToInt32(amount);
								DB.SaveChanges();

							} catch (Exception ex) {
								ViewBag.lblmessage = "Unable to update job balance, but we will post the adjustment for you!  Error message is:" + ex.Message;
							}

						}

						

					} catch (Exception ex) {
						ViewBag.lblmessage =  "Adjustment was NOT posted since we are Unable to update job payments due to:" + ex.Message;
					}
				}
		}

		public string resourceFILE { get; set; }
        [HttpPost, ValidateInput(false)]
        public ActionResult DownloadARList()
        {
            var generator = new ExcelWriter();
            var xml = this.Request.Form["grid_xml"];
            xml = this.Server.UrlDecode(xml);
            var stream = generator.Generate(xml);
            return File(stream.ToArray(), generator.ContentType, "MyARList.xlsx");
        }
	}
}
