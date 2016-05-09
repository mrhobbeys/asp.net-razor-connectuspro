using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using QBFC10Lib;
using System.Collections.Generic;
using Common;

namespace QBEngine
{
    public class QBInvoice :QBBase
    {

        List<Invoice> InvoiceList = null;

        #region Search Invoices 

        private List<Invoice> walkInvoice(IMsgSetResponse responseMsgSet)
        {

            if (responseMsgSet == null) return null;

            IResponseList responseList = responseMsgSet.ResponseList;
            if (responseList == null) return null;

            IResponse response = responseList.GetAt(0);
            //check the status code of the response, 0=ok, >0 is warning
            if (response.StatusCode == 0)
            {
                //the request-specific response is in the details, make sure we have some
                if (response.Detail != null)
                {
                    //make sure the response is the type we're expecting
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtInvoiceQueryRs)
                    {
                        //upcast to more specific type here, this is safe because we checked with response.Type check above
                        IInvoiceRetList InvoiceRet = (IInvoiceRetList)response.Detail;
                        int count = InvoiceRet.Count;
                        if (count > 0)
                            InvoiceList = new List<Invoice>();
                        for(int a =0;a< count;a++) 
                       InvoiceList.Add( WalkInvoiceSearchRet(InvoiceRet.GetAt(a)));

                    }

                }

            }
            else
            {
                throw new QBException(response.StatusCode, response.StatusMessage.ToString(),requestMsgSet.ToXMLString());
            }

            return InvoiceList;
        }

        IList<Invoice> InvoiceRet(IInvoiceRetList InvoiceList)
        {
           // InvoiceList = new List<QBEngine.Invoice>();

            for (int i = 0; i < InvoiceList.Count; i++)
            {
                #region comments
                //  dsCustomer.InvoiceRow row = dataset.Invoice.NewInvoiceRow();
                //    IInvoiceRet InvoiceRet = InvoiceList.GetAt(i);


                //    if (InvoiceRet == null) return null;

                //    //string TxnID7 = (string)InvoiceRet.TxnID.GetValue();

                //    row.TimeCreated = InvoiceRet.TimeCreated.GetValue().ToString();
                //    row.TimeModified = InvoiceRet.TimeModified.GetValue().ToString();
                //    row.EditSequence = (string)InvoiceRet.EditSequence.GetValue();


                //    if (InvoiceRet.CustomerMsgRef != null) 
                //    row.CustomerMsg = InvoiceRet.CustomerMsgRef.FullName.GetValue().ToString();

                //if (InvoiceRet.TxnNumber!= null)
                //    row.TxnNumber = InvoiceRet.TxnNumber.GetValue().ToString();
                //if (InvoiceRet.TermsRef != null) 
                //   row.Terms=  InvoiceRet.TermsRef.FullName.GetValue().ToString();

                //    if (InvoiceRet.RefNumber != null)
                //        row.InvoiceNo = InvoiceRet.RefNumber.GetValue();
                //    else
                //        row.InvoiceNo = (string)InvoiceRet.EditSequence.GetValue();
                //    if (InvoiceRet.TxnNumber != null)
                //    {
                //        row.TxnNumber = InvoiceRet.TxnNumber.GetValue().ToString();
                //    }
                //    //Get value of FullName
                //    if (InvoiceRet.CustomerRef.FullName != null)
                //    {
                //        row.CustomerName = (string)InvoiceRet.CustomerRef.FullName.GetValue();

                //    }

                //    if (InvoiceRet.CustomerSalesTaxCodeRef != null )
                //   row.CustomerTaxCode = InvoiceRet.CustomerSalesTaxCodeRef.FullName.GetValue().ToString();

                //    if (InvoiceRet.ClassRef != null)
                //    {
                //        //Get value of FullName
                //        if (InvoiceRet.ClassRef.FullName != null)
                //        {
                //            row.ClassRef = (string)InvoiceRet.ClassRef.FullName.GetValue();
                //        }
                //        if (InvoiceRet.ClassRef.ListID != null)
                //        {
                //            row.ClassRef_listID = (string)InvoiceRet.ClassRef.ListID.GetValue();
                //        }

                //    }

                //    #region attributes


                //    if (InvoiceRet.BillAddress != null)
                //    {
                //        ///Get value of Addr1
                //        if (InvoiceRet.BillAddress.Addr1 != null)
                //        {
                //            row.BillAddress1 = (string)InvoiceRet.BillAddress.Addr1.GetValue();

                //        }
                //        if (InvoiceRet.BillAddress.Addr2 != null)
                //        {
                //            row.BillAddress2 = (string)InvoiceRet.BillAddress.Addr2.GetValue();

                //        }
                //        if (InvoiceRet.BillAddress.Addr3 != null)
                //        {
                //            row.BillAddress3 = (string)InvoiceRet.BillAddress.Addr3.GetValue();

                //        }
                //        if (InvoiceRet.BillAddress.Addr4 != null)
                //        {
                //            row.BillAddress4 = (string)InvoiceRet.BillAddress.Addr4.GetValue();

                //        }
                //        ////Get value of City
                //        if (InvoiceRet.BillAddress.City != null)
                //        {
                //            row.City = (string)InvoiceRet.BillAddress.City.GetValue();

                //        }
                //        ////Get value of State
                //        if (InvoiceRet.BillAddress.State != null)
                //        {
                //            row.State = (string)InvoiceRet.BillAddress.State.GetValue();

                //        }
                //        ////Get value of PostalCode
                //        if (InvoiceRet.BillAddress.PostalCode != null)
                //        {
                //            row.PostalCode = (string)InvoiceRet.BillAddress.PostalCode.GetValue();

                //        }
                //        ////Get value of Country
                //        if (InvoiceRet.BillAddress.Country != null)
                //        {
                //            row.Country = (string)InvoiceRet.BillAddress.Country.GetValue();

                //        }
                //        ////Get value of Note

                //    }

                //    double amount = Math.Abs(  double.Parse(InvoiceRet.AppliedAmount.GetValue().ToString()));
                //    row.Amount = Convert.ToString(amount);

                //    if (InvoiceRet.ShipAddress != null)
                //    {


                //        ////Get value of Addr1
                //        if (InvoiceRet.ShipAddress.Addr1 != null)
                //        {
                //            row.Ship_Address1 = (string)InvoiceRet.ShipAddress.Addr1.GetValue();
                //        }
                //        ////Get value of Addr1
                //        if (InvoiceRet.ShipAddress.Addr2 != null)
                //        {
                //            row.ShipAddress2 = (string)InvoiceRet.ShipAddress.Addr2.GetValue();
                //        }
                //        ////Get value of Addr1
                //        if (InvoiceRet.ShipAddress.Addr3 != null)
                //        {
                //            row.ShipAddress3 = (string)InvoiceRet.ShipAddress.Addr3.GetValue();
                //        }
                //        ////Get value of Addr1
                //        if (InvoiceRet.ShipAddress.Addr4 != null)
                //        {
                //            row.ShipAddress4 = (string)InvoiceRet.ShipAddress.Addr4.GetValue();
                //        }

                //        ////Get value of City
                //        if (InvoiceRet.ShipAddress.City != null)
                //        {
                //            row.Ship_City = (string)InvoiceRet.ShipAddress.City.GetValue();

                //        }
                //        ////Get value of State
                //        if (InvoiceRet.ShipAddress.State != null)
                //        {
                //            row.Ship_State = (string)InvoiceRet.ShipAddress.State.GetValue();

                //        }
                //        ////Get value of PostalCode
                //        if (InvoiceRet.ShipAddress.PostalCode != null)
                //        {
                //            row.Ship_PostalCode = (string)InvoiceRet.ShipAddress.PostalCode.GetValue();

                //        }
                //        ////Get value of Country
                //        if (InvoiceRet.ShipAddress.Country != null)
                //        {
                //            row.Ship_Country = (string)InvoiceRet.ShipAddress.Country.GetValue();

                //        }

                //    }

                //    ////Get value of DueDate
                //    if (InvoiceRet.DueDate != null)
                //    {
                //        row.DueDate = InvoiceRet.DueDate.GetValue().ToString();

                //    }

                //    ////Get value of ShipDate
                //    if (InvoiceRet.ShipDate != null)
                //    {
                //        row.ShipDate = InvoiceRet.ShipDate.GetValue().ToString();

                //    }
                //    if (InvoiceRet.PONumber != null)
                //        row.PONumber = InvoiceRet.PONumber.GetValue().ToString();
                //    if (InvoiceRet.IsPaid != null)
                //        row.IsPaid = InvoiceRet.IsPaid.GetValue().ToString();
                //    if (InvoiceRet.SalesRepRef != null)
                //    {

                //        if (InvoiceRet.SalesRepRef.FullName != null)
                //            row.SalesRep_Name = InvoiceRet.SalesRepRef.FullName.ToString();
                //        if (InvoiceRet.SalesRepRef.ListID != null)
                //            row.SalesRep_listID = InvoiceRet.SalesRepRef.ListID.GetValue().ToString();

                //    }

                //    if (InvoiceRet.ShipMethodRef != null)
                //    {
                //        if (InvoiceRet.ShipMethodRef.ListID != null)
                //        row.ShipMethod_listID = InvoiceRet.ShipMethodRef.ListID.GetValue().ToString();
                //        if (InvoiceRet.ShipMethodRef.FullName != null )
                //            row.ShipMethod_Name = InvoiceRet.ShipMethodRef.FullName.GetValue().ToString();
                //    }


                //    if (InvoiceRet.CustomerRef != null)
                //    {

                //        row.Customer_listID = InvoiceRet.CustomerRef.ListID.GetValue().ToString();

                //    }
                //    if (InvoiceRet.Other != null)
                //        row.Other = InvoiceRet.Other.ToString();

                //    ////Get value of Memo
                //    if (InvoiceRet.Memo != null)
                //    {
                //        row.Memo = (string)InvoiceRet.Memo.GetValue();

                //    }

                //    for (int a = 0; a < InvoiceRet.ORInvoiceLineRetList.Count; a++)
                //    {
                //        IORInvoiceLineRet ORInvoiceLineRet = InvoiceRet.ORInvoiceLineRetList.GetAt(a);
                //        //InvoiceLineItem item = new InvoiceLineItem();
                //        dsCustomer.InvoiceLineItemRow item = dataset.InvoiceLineItem.NewInvoiceLineItemRow();
                //        item.InvoiceNo = row.InvoiceNo;
                //        if (ORInvoiceLineRet.InvoiceLineRet != null)
                //        {
                //            ////Get value of TxnLineID
                //            //string TxnLineID90 = (string)ORInvoiceLineRet.InvoiceLineRet.TxnLineID.GetValue();
                //            if (ORInvoiceLineRet.InvoiceLineRet.ItemRef != null)
                //            {

                //                if (ORInvoiceLineRet.InvoiceLineRet.ItemRef.FullName != null)
                //                {
                //                    item.Item = (string)ORInvoiceLineRet.InvoiceLineRet.ItemRef.FullName.GetValue();

                //                }

                //            }
                //            ////Get value of Desc
                //            if (ORInvoiceLineRet.InvoiceLineRet.Desc != null)
                //            {
                //                item.Desc = (string)ORInvoiceLineRet.InvoiceLineRet.Desc.GetValue();

                //            }
                //            ////Get value of Quantity
                //            if (ORInvoiceLineRet.InvoiceLineRet.Quantity != null)
                //            {
                //                item.Quantity = ORInvoiceLineRet.InvoiceLineRet.Quantity.GetValue().ToString();

                //            }

                //            //}
                //            if (ORInvoiceLineRet.InvoiceLineRet.ClassRef != null)
                //            {

                //                ////Get value of FullName
                //                if (ORInvoiceLineRet.InvoiceLineRet.ClassRef.FullName != null)
                //                {
                //                    item.ClassRef = (string)ORInvoiceLineRet.InvoiceLineRet.ClassRef.FullName.GetValue();

                //                }

                //                if (ORInvoiceLineRet.InvoiceLineRet.IsTaxable.GetValue())
                //                    item.SalesTaxCodeRef = ORInvoiceLineRet.InvoiceLineRet.SalesTaxCodeRef.FullName.GetValue().ToString();
                //                if (ORInvoiceLineRet.InvoiceLineRet.ClassRef.ListID != null)
                //                {
                //                    item.ClassRef = (string)ORInvoiceLineRet.InvoiceLineRet.ClassRef.FullName.GetValue();

                //                }

                //            }

                //            if (ORInvoiceLineRet.InvoiceLineRet.Amount != null)
                //            {
                //                item.Amount = ORInvoiceLineRet.InvoiceLineRet.Amount.GetValue().ToString();

                //            }


                //    #endregion


                //        }

                //        dataset.InvoiceLineItem.AddInvoiceLineItemRow(item);


                //    }

                //    dataset.Invoice.AddInvoiceRow(row);
                //}


                //return dataset;
#endregion
               
            }

            return null;
        }          
      
        Invoice WalkInvoiceSearchRet(IInvoiceRet InvoiceRet)
        {

             
            
            Invoice invoice = new Invoice();
            if (InvoiceRet == null) return null;
            invoice.TxnID = InvoiceRet.TxnID.GetValue().ToString();
            invoice.TimeCreated = (DateTime)InvoiceRet.TimeCreated.GetValue();
            invoice.TimeModified = (DateTime)InvoiceRet.TimeModified.GetValue();
            invoice.EditSequence = (string)InvoiceRet.EditSequence.GetValue();
            if (InvoiceRet.RefNumber != null)
                invoice.ID = InvoiceRet.RefNumber.GetValue();
            if (InvoiceRet.TxnNumber != null)
            {
                invoice.TxnNumber = InvoiceRet.TxnNumber.GetValue().ToString();
            }
            //Get value of FullName
            if (InvoiceRet.CustomerRef.FullName != null)
            {
                invoice.CustomerName = (string)InvoiceRet.CustomerRef.FullName.GetValue();

            }
            if (InvoiceRet.ClassRef != null)
            {
                //Get value of FullName
                if (InvoiceRet.ClassRef.FullName != null)
                {
                    invoice.ClassRef = (string)InvoiceRet.ClassRef.FullName.GetValue();
                }

            }
            //Discount Items 
            if (InvoiceRet.DiscountLineRet != null) {

                if (InvoiceRet.DiscountLineRet.AccountRef != null) {
                    invoice.DiscountAccountRef = InvoiceRet.DiscountLineRet.AccountRef.FullName.GetValue();
                }
            }

            

            #region attributes
            
            if (InvoiceRet.BillAddress != null)
            {
                ///Get value of Addr1
                if (InvoiceRet.BillAddress.Addr1 != null)
                {
                    invoice.BillAddress1 = (string)InvoiceRet.BillAddress.Addr1.GetValue();

                }
                ////Get value of Addr2

                ////Get value of City
                if (InvoiceRet.BillAddress.City != null)
                {
                    invoice.City = (string)InvoiceRet.BillAddress.City.GetValue();

                }
                ////Get value of State
                if (InvoiceRet.BillAddress.State != null)
                {
                    invoice.State = (string)InvoiceRet.BillAddress.State.GetValue();

                }
                ////Get value of PostalCode
                if (InvoiceRet.BillAddress.PostalCode != null)
                {
                    invoice.PostalCode = (string)InvoiceRet.BillAddress.PostalCode.GetValue();

                }
                ////Get value of Country
                if (InvoiceRet.BillAddress.Country != null)
                {
                    invoice.Country = (string)InvoiceRet.BillAddress.Country.GetValue();

                }
                ////Get value of Note

            }
            
            invoice.Amount = InvoiceRet.BalanceRemaining.GetValue().ToString();

            if (InvoiceRet.ShipAddress != null)
            {
                ////Get value of Addr1
                if (InvoiceRet.ShipAddress.Addr1 != null)
                {
                    invoice.ShipAddress = (string)InvoiceRet.ShipAddress.Addr1.GetValue();
                }

                ////Get value of City
                if (InvoiceRet.ShipAddress.City != null)
                {
                    invoice.Ship_City = (string)InvoiceRet.ShipAddress.City.GetValue();

                }
                ////Get value of State
                if (InvoiceRet.ShipAddress.State != null)
                {
                    invoice.Ship_State = (string)InvoiceRet.ShipAddress.State.GetValue();

                }
                ////Get value of PostalCode
                if (InvoiceRet.ShipAddress.PostalCode != null)
                {
                    invoice.Ship_PostalCode = (string)InvoiceRet.ShipAddress.PostalCode.GetValue();

                }
                ////Get value of Country
                if (InvoiceRet.ShipAddress.Country != null)
                {
                    invoice.Ship_Country = (string)InvoiceRet.ShipAddress.Country.GetValue();

                }

            }

          
            ////Get value of DueDate
            if (InvoiceRet.DueDate != null)
            {
                invoice.DueDate = (DateTime)InvoiceRet.DueDate.GetValue();

            }

            //if (InvoiceRet.FOB != null)
            // {
            //= (string)InvoiceRet.FOB.GetValue();

            //}
            ////Get value of ShipDate
            if (InvoiceRet.ShipDate != null)
            {
                invoice.ShipDate = (DateTime)InvoiceRet.ShipDate.GetValue();

            }

            ////Get value of Memo
            if (InvoiceRet.Memo != null)
            {
                invoice.Memo = (string)InvoiceRet.Memo.GetValue();

            }
            ////Get value of IsPaid
            if (InvoiceRet.IsPaid != null)
            //{
            invoice.IsPaid = (bool)InvoiceRet.IsPaid.GetValue();

            //}


            ////Get value of Amount
            //double Amount88 = (double)LinkedTxn.Amount.GetValue();

            //}

            //}
            if (InvoiceRet.ORInvoiceLineRetList != null)
                invoice.lineitem = new List<InvoiceLineItem>();

            //{
            for (int i = 0; i < InvoiceRet.ORInvoiceLineRetList.Count; i++)
            {
                IORInvoiceLineRet ORInvoiceLineRet = InvoiceRet.ORInvoiceLineRetList.GetAt(i);
                InvoiceLineItem item = new InvoiceLineItem();
                if (ORInvoiceLineRet.InvoiceLineRet != null)
                {
                    ////Get value of TxnLineID
                    item.TxnLineID = (string)ORInvoiceLineRet.InvoiceLineRet.TxnLineID.GetValue();
                    if (ORInvoiceLineRet.InvoiceLineRet.ItemRef != null)
                    {
                        if (ORInvoiceLineRet.InvoiceLineRet.ItemRef.FullName != null)
                        {
                            item.Item = (string)ORInvoiceLineRet.InvoiceLineRet.ItemRef.FullName.GetValue();
                        }
                    }
                    ////Get value of Desc
                    if (ORInvoiceLineRet.InvoiceLineRet.Desc != null)
                    {
                        item.Desc = (string)ORInvoiceLineRet.InvoiceLineRet.Desc.GetValue();

                    }
                    ////Get value of Quantity
                    if (ORInvoiceLineRet.InvoiceLineRet.Quantity != null)
                    {
                        item.Quantity = (int)ORInvoiceLineRet.InvoiceLineRet.Quantity.GetValue();

                    }

                    //}
                    if (ORInvoiceLineRet.InvoiceLineRet.ClassRef != null)
                    {

                        ////Get value of FullName
                        if (ORInvoiceLineRet.InvoiceLineRet.ClassRef.FullName != null)
                            //{
                            item.ClassRef = (string)ORInvoiceLineRet.InvoiceLineRet.ClassRef.FullName.GetValue();

                    }

                    //}
                    ////Get value of Amount
                    if (ORInvoiceLineRet.InvoiceLineRet.Amount != null)
                    {
                        item.Amount = (double)ORInvoiceLineRet.InvoiceLineRet.Amount.GetValue();

                    }

                    invoice.AddItem(item);
                }
            }
            #endregion

             return invoice;
                }          
        
     
        public List<Invoice> searchbyInvoiceNo(string No)
        {
            
            requestMsgSet.ClearRequests();
            IInvoiceQuery InvoiceQueryRq = requestMsgSet.AppendInvoiceQueryRq();
            if (!string.IsNullOrEmpty(No))
            {
                InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.MatchCriterion.SetValue(ENMatchCriterion.mcContains);
                //Set field value for RefNumber
                InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.ORRefNumberFilter.RefNumberFilter.RefNumber.SetValue(No);
            }
            InvoiceQueryRq.IncludeLineItems.SetValue(true);            
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            
            return walkInvoice(responseMsgSet);

            //Bug fix 
           

        }

        #endregion 

        //public void searchbyCustomerName(string customerName) {

        //    requestMsgSet.ClearRequests();
        //    IInvoiceQuery InvoiceQueryRq = requestMsgSet.AppendInvoiceQueryRq();
        //    //All Open Invoices 
        //    InvoiceQueryRq.ORInvoiceQuery.InvoiceFilter.PaidStatus.SetValue(ENPaidStatus.psNotPaidOnly);            
            
        //    InvoiceQueryRq.IncludeLineItems.SetValue(true);
        //    //InvoiceQueryRq.IncludeLinkedTxns.SetValue(true);
        //    responseMsgSet = sessionManager.DoRequests(requestMsgSet); 
        //   // return walkInvoice(responseMsgSet);
        //}

        #region Update Invoices

        public string UpdateInvoice(Invoice invoice) {
            try
            {

            requestMsgSet.ClearRequests();
            
           
            
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            IResponseList responseList = responseMsgSet.ResponseList;
            IResponse response = responseList.GetAt(0);
            //check the status code of the response, 0=ok, >0 is warning
            if (response.StatusCode == 0)
                {

                    return response.StatusMessage;
                }
                else
                {
                    throw new QBException(response.StatusCode, response.StatusMessage.ToString(),requestMsgSet.ToXMLString());
                }
            }
            catch (Exception ex) {

                return ex.Message;
            }
            
            
        }
             
        public string BuildUpdateInvoiceRq(Invoice invoice)
        {
            requestMsgSet.ClearRequests();

            IInvoiceMod InvoiceAddRq = requestMsgSet.AppendInvoiceModRq();
            //For Update we need TxnID and Edit Sequence 
            InvoiceAddRq.TxnID.SetValue(invoice.TxnID);
            InvoiceAddRq.EditSequence.SetValue(invoice.EditSequence);

            InvoiceAddRq.CustomerRef.FullName.SetValue(invoice.CustomerName);
            if (invoice.ClassRef != null)
                InvoiceAddRq.ClassRef.FullName.SetValue(invoice.ClassRef);
            if (invoice.BillAddress1 != null)
                InvoiceAddRq.BillAddress.Addr1.SetValue(invoice.BillAddress1);
            if (invoice.City != null)
                InvoiceAddRq.BillAddress.City.SetValue(invoice.City);
            if (invoice.State != null)
                InvoiceAddRq.BillAddress.State.SetValue(invoice.State);
            if (invoice.State != null)
                InvoiceAddRq.BillAddress.PostalCode.SetValue(invoice.PostalCode);
            if (invoice.Country != null)
                InvoiceAddRq.BillAddress.Country.SetValue(invoice.Country);
            if (invoice.ShipAddress != null)
                InvoiceAddRq.ShipAddress.Addr1.SetValue(invoice.ShipAddress);
            if (invoice.Ship_City != null)
                InvoiceAddRq.ShipAddress.City.SetValue(invoice.Ship_City);
            if (invoice.Ship_State != null)
                InvoiceAddRq.ShipAddress.State.SetValue(invoice.Ship_State);
            if (invoice.Ship_PostalCode != null)
                InvoiceAddRq.ShipAddress.PostalCode.SetValue(invoice.PostalCode);
            if (invoice.Ship_Country != null)
                InvoiceAddRq.ShipAddress.Country.SetValue(invoice.Country);
            if (invoice.DueDate >= DateTime.Now)
                InvoiceAddRq.DueDate.SetValue(invoice.DueDate);
            if (invoice.ShipDate >= DateTime.Now)
                InvoiceAddRq.ShipDate.SetValue(invoice.ShipDate);
            InvoiceAddRq.Memo.SetValue(invoice.Memo);
            if (invoice.CustomerMsg != "")
                InvoiceAddRq.CustomerMsgRef.FullName.SetValue(invoice.CustomerMsg);

            if (invoice.Terms != "")
                InvoiceAddRq.TermsRef.FullName.SetValue(invoice.Terms);
            InvoiceAddRq.IsToBePrinted.SetValue(invoice.isPrinted);
            //istobeemail is supported in qbxml version 6.0 and greater 
            if (Manager.MjrVersion >= 6)
            {
                InvoiceAddRq.IsToBeEmailed.SetValue(invoice.IsEmail);
            }
            if (invoice.CustomerTaxCode != null && invoice.CustomerTaxCode != "")
                InvoiceAddRq.CustomerSalesTaxCodeRef.FullName.SetValue(invoice.CustomerTaxCode);
            if (invoice.TxnNumber != null && invoice.TxnNumber != "")
                InvoiceAddRq.ItemSalesTaxRef.FullName.SetValue(invoice.TxnNumber);
            foreach (InvoiceLineItem item in invoice.lineitem)
            {

                IORInvoiceLineMod ORInvoiceLineAddListElement1 = InvoiceAddRq.ORInvoiceLineModList.Append();

                //TxnID 
                ORInvoiceLineAddListElement1.InvoiceLineMod.TxnLineID.SetValue(item.TxnLineID);
                ORInvoiceLineAddListElement1.InvoiceLineMod.ItemRef.FullName.SetValue(item.Item);
                ORInvoiceLineAddListElement1.InvoiceLineMod.Desc.SetValue(item.Desc);
                ORInvoiceLineAddListElement1.InvoiceLineMod.Quantity.SetValue(item.Quantity);
                if (item.ClassRef != null)
                    ORInvoiceLineAddListElement1.InvoiceLineMod.ClassRef.FullName.SetValue(item.ClassRef);
                ORInvoiceLineAddListElement1.InvoiceLineMod.Amount.SetValue(item.Amount);
                if(item.Rate != null) 
                ORInvoiceLineAddListElement1.InvoiceLineMod.ORRatePriceLevel.Rate.SetValue(item.Rate.Value);
                if (item.SalesTaxCodeRef != null)
                    ORInvoiceLineAddListElement1.InvoiceLineMod.SalesTaxCodeRef.FullName.SetValue(item.SalesTaxCodeRef);
            }
            //Send the request and get the response from QuickBooks
            IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            return WalkInvoiceAddRs(responseMsgSet);

        }

        #endregion 

        #region CreateInvoice

        public string createInvoice(Invoice invoice)
        {
            requestMsgSet.ClearRequests();
            
            IInvoiceAdd InvoiceAddRq = requestMsgSet.AppendInvoiceAddRq();          
            InvoiceAddRq.CustomerRef.FullName.SetValue(invoice.CustomerName);
            if (invoice.ClassRef != null)
                InvoiceAddRq.ClassRef.FullName.SetValue(invoice.ClassRef);            
            if (invoice.BillAddress1 != null)
                InvoiceAddRq.BillAddress.Addr1.SetValue(invoice.BillAddress1);
            if (invoice.City != null)
                InvoiceAddRq.BillAddress.City.SetValue(invoice.City);
            if (invoice.State != null)
                InvoiceAddRq.BillAddress.State.SetValue(invoice.State);
            if (invoice.State != null)
                InvoiceAddRq.BillAddress.PostalCode.SetValue(invoice.PostalCode);
            if (invoice.Country != null)
                InvoiceAddRq.BillAddress.Country.SetValue(invoice.Country);           
            if (invoice.ShipAddress != null)
                InvoiceAddRq.ShipAddress.Addr1.SetValue(invoice.ShipAddress);
            if (invoice.Ship_City != null)
                InvoiceAddRq.ShipAddress.City.SetValue(invoice.Ship_City);
            if (invoice.Ship_State != null)
                InvoiceAddRq.ShipAddress.State.SetValue(invoice.Ship_State);
            if (invoice.Ship_PostalCode != null)
                InvoiceAddRq.ShipAddress.PostalCode.SetValue(invoice.PostalCode);
            if (invoice.Ship_Country != null)
                InvoiceAddRq.ShipAddress.Country.SetValue(invoice.Country);            
            if (invoice.DueDate >= DateTime.Now)
                InvoiceAddRq.DueDate.SetValue(invoice.DueDate);            
            if (invoice.ShipDate >= DateTime.Now)
                InvoiceAddRq.ShipDate.SetValue(invoice.ShipDate);            
                InvoiceAddRq.Memo.SetValue(invoice.Memo);
            if (invoice.CustomerMsg !="" )
                InvoiceAddRq.CustomerMsgRef.FullName.SetValue(invoice.CustomerMsg);
            
        if (invoice.Terms != "")
               InvoiceAddRq.TermsRef.FullName.SetValue(invoice.Terms);
               InvoiceAddRq.IsToBePrinted.SetValue(invoice.isPrinted);            
            //istobeemail is supported in qbxml version 6.0 and greater 
        if (Manager.MjrVersion >= 6)
        {
            InvoiceAddRq.IsToBeEmailed.SetValue(invoice.IsEmail);
        }
            if ( invoice.CustomerTaxCode != null && invoice.CustomerTaxCode !="" )
        InvoiceAddRq.CustomerSalesTaxCodeRef.FullName.SetValue(invoice.CustomerTaxCode);
  //  if (invoice.TxnNumber != null && invoice.TxnNumber != "")
    //    InvoiceAddRq.ItemSalesTaxRef.FullName.SetValue(invoice.TxnNumber);
      foreach (InvoiceLineItem item in invoice.lineitem)
            {

                IORInvoiceLineAdd ORInvoiceLineAddListElement1 = InvoiceAddRq.ORInvoiceLineAddList.Append();
                
     
                ORInvoiceLineAddListElement1.InvoiceLineAdd.ItemRef.FullName.SetValue(item.Item);
                
                
                ORInvoiceLineAddListElement1.InvoiceLineAdd.Desc.SetValue(item.Desc);
                ORInvoiceLineAddListElement1.InvoiceLineAdd.Quantity.SetValue(item.Quantity);
          
          

                if (item.ClassRef != null)
                    ORInvoiceLineAddListElement1.InvoiceLineAdd.ClassRef.FullName.SetValue(item.ClassRef);
                    ORInvoiceLineAddListElement1.InvoiceLineAdd.Amount.SetValue(item.Amount);
                    if (item.SalesTaxCodeRef != null) 
                    ORInvoiceLineAddListElement1.InvoiceLineAdd.SalesTaxCodeRef.FullName.SetValue(item.SalesTaxCodeRef);
           }
            //Send the request and get the response from QuickBooks
            IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            return WalkInvoiceAddRs(responseMsgSet);

        }
        string WalkInvoiceAddRs(IMsgSetResponse responseMsgSet)
        {
            if (responseMsgSet == null) return null;

            IResponseList responseList = responseMsgSet.ResponseList;
            if (responseList == null) return null;

            //if we sent only one request, there is only one response, we'll walk the list for this sample
            for (int i = 0; i < responseList.Count; i++)
            {
                IResponse response = responseList.GetAt(i);
                //check the status code of the response, 0=ok, >0 is warning
                if (response.StatusCode == 0)
                {
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType == ENResponseType.rtInvoiceAddRs)
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            IInvoiceRet InvoiceRet = (IInvoiceRet)response.Detail;
                            return WalkInvoiceRet(InvoiceRet);

                        }

                    }

                }
                else
                {
                    throw new QBException(response.StatusCode, response.StatusMessage,requestMsgSet.ToXMLString());
                }

            }
            return null;
        }
        string WalkInvoiceRet(IInvoiceRet InvoiceRet)
        {
            if (InvoiceRet == null) return null;
            return InvoiceRet.TxnID.GetValue().ToString();
        }

        #endregion 


    }
}