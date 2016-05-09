using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using SiteBlue.Core;
using SiteBlue.Core.Email;
using SiteBlue.Data.EightHundred;

using InvoiceGen = SiteBlue.Business.Enterprise.InvoiceGeneration;

namespace SiteBlue.Business.Invoice
{
    public class InvoiceService : AbstractBusinessService
    {
        private static InvoiceGen.IGenerateInvoices GetInvoiceGen()
        {
            var svc = new InvoiceGen.GenerateInvoicesClient();

            if (svc.ClientCredentials != null)
            {
                svc.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(GlobalConfiguration.DocumentGenerationUser, GlobalConfiguration.DocumentGenerationPassword, string.Empty);
                svc.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Identification;
            }

            return svc;
        }

        private void MarkInvoiced(int id, DateTime invoiceDate)
        {
            var job = new tbl_Job {JobID = id};

            using (var ctx = new EightHundredEntities(UserKey))
            {
                ctx.tbl_Job.Attach(job);
                job.InvoicedDate = invoiceDate;
                ctx.SaveChanges();
            }
        }

        public OperationResult<bool> SendToCustomer(int id, string to)
        {
            var result = new OperationResult<bool>();
            var invoice = TransformJobToInvoice(id);

            if (invoice == null)
            {
                result.Success = false;
                result.ResultData = false;
                result.Message = string.Format("Job '{0}' not found.", id);
                return result;
            }

            var client = GetInvoiceGen();
            var genResultPdf = GetInvoiceInternal(id);
            var genResultHtml = client.RenderHtml(invoice);
            var docType = invoice.IsEstimate ? "Estimate" : "Invoice";

            if (!genResultPdf.Success || !genResultHtml.Success)
            {
                result.Success = false;
                result.ResultData = false;
                result.Message = string.Format("Could not send {0} to customer: {1}", docType, (genResultPdf.Message ?? genResultHtml.ExceptionMessage));
                return result;
            }

            var html = Encoding.Unicode.GetString(genResultHtml.Data);
            var engine = new EmailEngine();
            var subject = string.Format("{0} - {1}", docType, id);
            var from = invoice.FranchiseTypeID == 6
                ? GlobalConfiguration.PlumberInvoiceFromAddress
                : GlobalConfiguration.ConnectUsProInvoiceFromAddress;


            //Do not send invoices to customers when running in test mode.
            if (GlobalConfiguration.TestMode) to = string.Empty;

            bool sent;
            using (var data = new MemoryStream(genResultPdf.ResultData.Value))
            {
                sent = engine.Send(from, to, null, GlobalConfiguration.InvoiceSendBcc, subject, html,
                                          new[] { new Attachment(data, genResultPdf.ResultData.Key) },
                                          true);
            }

            result.Success = sent;

            if (!result.Success)
                result.Message = string.Format("The {0} failed to send.", docType);
            else
                MarkInvoiced(id, DateTime.Now);

            return result;
        }

        /// <summary>
        /// Gets the PDF version of an invoice.
        /// </summary>
        /// <param name="id">The invoice id</param>
        /// <param name="forCustomer">A flag that indicates whether the PDF being generated will be sent to the customer or is just a preview.  If true (for customer) the invoice date is set.</param>
        /// <returns>A key/value pair where the key is the file name of the invoice PDF to be used and the value is the byte array containing the invoice itself.</returns>
        public OperationResult<KeyValuePair<string, byte[]>> GetInvoicePdf(int id, bool forCustomer)
        {
            try
            {
                var result =  GetInvoiceInternal(id);

                if (result.Success && forCustomer)
                    MarkInvoiced(id, DateTime.Now);

                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult<KeyValuePair<string, byte[]>> { Success = false, Message = "Could not generate invoice: " + ex.Message };
            }
        }

        private OperationResult<KeyValuePair<string, byte[]>> GetInvoiceInternal(int id)
        {
            var result = new OperationResult<KeyValuePair<string, byte[]>>();
            var invoice = TransformJobToInvoice(id);

            if (invoice == null)
            {
                result.Success = false;
                result.ResultData = default(KeyValuePair<string, byte[]>);
                result.Message = string.Format("Job '{0}' not found.", id);
                return result;
            }

            try
            {
                var client = GetInvoiceGen();
                var renderResult = client.Render(invoice, null);
                result.Success = renderResult.Success;
                var fileName = string.Format("{0}_{1}.pdf", invoice.IsEstimate ? "Estimate" : "Invoice", invoice.JobID);
                result.ResultData = new KeyValuePair<string, byte[]>(fileName, renderResult.Data);
                result.Message = renderResult.ExceptionMessage;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Error rendering invoice: " + ex.Message;
            }


            return result;
        }

        private InvoiceGen.Invoice TransformJobToInvoice(int id)
        {
            var objmodcommon = new mod_common(UserKey);

            var invoice = new InvoiceGen.Invoice();

            using (var ctx = new EightHundredEntities(UserKey))
            {
                var job = ctx.tbl_Job.SingleOrDefault(j => j.JobID == id);


                if (job == null)
                    return null;

                var franchise = ctx.tbl_Franchise.SingleOrDefault(f => f.FranchiseID == job.FranchiseID);
                var franchiseContract = ctx.tbl_Franchise_Contract.SingleOrDefault(fc => fc.FranchiseID == job.FranchiseID);
                var franchiseContact = ctx.tbl_Franchise_Contacts.FirstOrDefault(fc => fc.FranchiseID == job.FranchiseID && fc.PhoneTypeID == 2);

                if (franchise == null)
                    return null;

                invoice.BusinessType = job.BusinessTypeID == 3 ? "Commercial" : "Residential";
                invoice.JobID = job.JobID;
                invoice.IsEstimate = job.StatusID == 13;
                invoice.Number = invoice.JobID.ToString();
                invoice.Technician = ctx.tbl_Employee.Single(e => e.EmployeeID == job.ServiceProID).Employee;
                invoice.Warranty1 = objmodcommon.GetWarrantyType(job.WarrantyType1);
                invoice.Warranty1Length = objmodcommon.GetWarrantyLength(job.WarrantyLen1);
                invoice.Warranty2 = objmodcommon.GetWarrantyType(job.WarrantyType2);
                invoice.Warranty2Length = objmodcommon.GetWarrantyLength(job.WarrantyLen2);
                invoice.CallReason = job.CallReason;
                invoice.Diagnostics = job.Diagnostics;
                invoice.Recommendations = job.Recommendations;
                invoice.Lines = ctx.tbl_Job_Tasks.Where(jt => jt.JobID == job.JobID)
                                                 .Select(jt => new InvoiceGen.InvoiceLine
                                                 {
                                                     Description = jt.JobCodeDescription,
                                                     Quantity = jt.Quantity,
                                                     UnitPrice = jt.Price,
                                                     ExtendedPrice =
                                                         jt.Quantity * jt.Price
                                                 }).ToArray();
                invoice.Completed = (job.CallCompleted ?? job.ActualEnd ?? job.ScheduleEnd ?? job.ScheduleStart ?? job.CallTaken ?? job.DispatchedDate) ?? DateTime.Today;
                invoice.Date = job.InvoicedDate ?? DateTime.Today;

                var dba = ctx.tbl_Dispatch_DBA.SingleOrDefault(d => d.DBAID == job.AreaID);
                invoice.FromName = dba == null ? string.Empty : dba.DBAName;
                invoice.DBAId = job.AreaID;
                invoice.FranchiseId = job.FranchiseID;
                invoice.FranchiseTypeID = franchise.FranchiseTypeID;

                if (string.IsNullOrEmpty(invoice.FromName) && franchise.FranchiseTypeID == 6)
                    invoice.FromName = franchise.LegalName;

                invoice.FromAddress = franchise.MailAddress;
                invoice.FromCity = franchise.MailCity;
                invoice.FromState = franchise.MailState;
                invoice.FromZip = franchise.MailPostal;
                invoice.FromPhone = franchiseContact.PhoneNumber;
                invoice.LicenseNumber = franchiseContract.LicenseInfo;

                //load customer billto
                var customer = ctx.tbl_Customer.Single(c => c.CustomerID == job.CustomerID);
                var billto = ctx.tbl_Locations.Single(l => l.BilltoCustomerID == job.CustomerID);

                if (customer != null && billto != null)
                {
                    invoice.ToName = string.Format("{0}{1}{2}",
                                                   string.IsNullOrWhiteSpace(customer.CompanyName)
                                                       ? string.Empty
                                                       : customer.CompanyName,
                                                   string.IsNullOrWhiteSpace(customer.CompanyName) ||
                                                   string.IsNullOrWhiteSpace(customer.CustomerName)
                                                       ? string.Empty
                                                       : " - ",
                                                   string.IsNullOrWhiteSpace(customer.CustomerName)
                                                       ? string.Empty
                                                       : customer.CustomerName);
                    invoice.ToAddress = billto.Address;
                    invoice.ToCity = billto.City;
                    invoice.ToState = billto.State;
                    invoice.ToZip = billto.PostalCode;
                    invoice.ToEmail = customer.EMail;

                    var primaryPhone = ctx.tbl_Contacts.FirstOrDefault(co => co.CustomerID == job.CustomerID && co.LocationID == billto.LocationID && co.PhoneTypeID == 2);

                    invoice.ToPhone = primaryPhone == null ? string.Empty : objmodcommon.Format_PhoneNumber(primaryPhone.PhoneNumber);
                    invoice.IsMember = ctx.tbl_Customer_Members.Any(c => c.CustomerID == job.CustomerID);
                }

                //load job location
                var joblocation = objmodcommon.getLocation(job.LocationID);
                if (joblocation != null)
                {
                    invoice.LocationName = joblocation.LocationName == null ? objmodcommon.GetCustomerName(customer) : joblocation.LocationName;
                    invoice.LocationAddress = joblocation.Address;
                    invoice.LocationCity = joblocation.City;
                    invoice.LocationState = joblocation.State;
                    invoice.LocationZip = joblocation.PostalCode;

                    var locationcontact = ctx.tbl_Contacts.FirstOrDefault(c => c.LocationID == job.LocationID && c.PhoneTypeID == 2);
                    invoice.LocationPhone = locationcontact == null ? string.Empty : objmodcommon.Format_PhoneNumber(locationcontact.PhoneNumber);
                }

                invoice.TotalAmount = job.TotalSales;
                invoice.SubTotal = job.SubTotal;
                invoice.TaxAmount = job.TaxAmount;
                invoice.Payments = (from p in ctx.tbl_Payments
                                    join pt in ctx.tbl_Payment_Types
                                        on p.PaymentTypeID equals pt.PaymentTypeId into joined
                                    where p.JobID == job.JobID
                                    from j in joined
                                    select new { Payment = p, Type = j }
                                   ).ToArray().Select(p => new InvoiceGen.Payment
                                   {
                                       Amount = p.Payment.PaymentAmount.GetValueOrDefault(),
                                       Description = p.Payment.CheckNumber,
                                       PaymentType = p.Type == null ? "Other" : p.Type.PaymentType
                                   }).ToArray();
                invoice.HasAcceptSignature = job.AcceptedBy != null && job.AcceptedBy.Length > 0;
                invoice.HasAuthSignature = job.AuthorizationToStart != null && job.AuthorizationToStart.Length > 0;
            }

            return invoice;
        }

    }
}
