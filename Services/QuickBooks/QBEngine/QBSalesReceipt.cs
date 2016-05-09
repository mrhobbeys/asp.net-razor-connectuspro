using System;
using System.Collections.Generic;
using System.Text;
using QBFC10Lib;
using Common;
namespace QBEngine
{
    public class QBSalesReceipt:QBBase
    {

        List<SalesReceipt> Receipts;

        #region Create

        public void CreateSaleReceipt(SalesReceipt Receipt)
        {

            BuildSalesReceiptAddRq(Receipt);
            //Send the request and get the response from QuickBooks
            IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            WalkSalesReceiptAddRs(responseMsgSet);
        }
        void BuildSalesReceiptAddRq(SalesReceipt Receipt)
        {
            requestMsgSet.ClearRequests();
            ISalesReceiptAdd SalesReceiptAddRq = requestMsgSet.AppendSalesReceiptAddRq();
            //Set attributes

            SalesReceiptAddRq.CustomerRef.FullName.SetValue(Receipt.Customer);
            //SalesReceiptAddRq.ClassRef.FullName.SetValue("ab");            
            if (Receipt.TxnDate != null)
                SalesReceiptAddRq.TxnDate.SetValue(Receipt.TxnDate.Value);
            if(!string.IsNullOrEmpty(Receipt.RefNumber))
            SalesReceiptAddRq.RefNumber.SetValue(Receipt.RefNumber);
            if (!string.IsNullOrEmpty(Receipt.BillAddress1))
            SalesReceiptAddRq.BillAddress.Addr1.SetValue(Receipt.BillAddress1);
            if (!string.IsNullOrEmpty(Receipt.City))
            SalesReceiptAddRq.BillAddress.City.SetValue(Receipt.City);
            if (!string.IsNullOrEmpty(Receipt.State))
            SalesReceiptAddRq.BillAddress.State.SetValue(Receipt.State);
            if (!string.IsNullOrEmpty(Receipt.PostalCode))
            SalesReceiptAddRq.BillAddress.PostalCode.SetValue(Receipt.PostalCode);
            if (!string.IsNullOrEmpty(Receipt.Country))
            SalesReceiptAddRq.BillAddress.Country.SetValue(Receipt.Country);
            if (!string.IsNullOrEmpty(Receipt.Note))
            SalesReceiptAddRq.BillAddress.Note.SetValue(Receipt.Note);


            if (!string.IsNullOrEmpty(Receipt.ShipAddress))
            SalesReceiptAddRq.ShipAddress.Addr1.SetValue(Receipt.ShipAddress);
            if (!string.IsNullOrEmpty(Receipt.Ship_City))
            SalesReceiptAddRq.ShipAddress.City.SetValue(Receipt.Ship_City);
            if (!string.IsNullOrEmpty(Receipt.Ship_State))
            SalesReceiptAddRq.ShipAddress.State.SetValue(Receipt.Ship_State);
            if (!string.IsNullOrEmpty(Receipt.Ship_PostalCode))
            SalesReceiptAddRq.ShipAddress.PostalCode.SetValue(Receipt.Ship_PostalCode);
            if (!string.IsNullOrEmpty(Receipt.Ship_Country))
            SalesReceiptAddRq.ShipAddress.Country.SetValue(Receipt.Ship_Country);
            if (!string.IsNullOrEmpty(Receipt.Ship_Note))
            SalesReceiptAddRq.ShipAddress.Note.SetValue(Receipt.Ship_Note);
            
            SalesReceiptAddRq.IsPending.SetValue(Receipt.IsPending);

            if (!string.IsNullOrEmpty(Receipt.CheckNumber))
                SalesReceiptAddRq.CheckNumber.SetValue(Receipt.CheckNumber);
            if (!string.IsNullOrEmpty(Receipt.PaymentMethodRef))
            SalesReceiptAddRq.PaymentMethodRef.FullName.SetValue(Receipt.PaymentMethodRef);

            if (Receipt.DueDate != null)
                SalesReceiptAddRq.DueDate.SetValue(Receipt.DueDate.Value);

            if (!string.IsNullOrEmpty(Receipt.SalesRepRef))
            SalesReceiptAddRq.SalesRepRef.FullName.SetValue(Receipt.SalesRepRef);
            if (Receipt.Ship_Date != null)
                SalesReceiptAddRq.ShipDate.SetValue(Receipt.Ship_Date.Value);


            if (!string.IsNullOrEmpty(Receipt.Ship_Method))
                SalesReceiptAddRq.ShipMethodRef.FullName.SetValue(Receipt.Ship_Method);

            if (!string.IsNullOrEmpty(Receipt.Memo))
            SalesReceiptAddRq.Memo.SetValue(Receipt.Memo);

            SalesReceiptAddRq.IsToBePrinted.SetValue(Receipt.isPrinted);
            SalesReceiptAddRq.IsToBeEmailed.SetValue(Receipt.IsEmail);

            if (!string.IsNullOrEmpty(Receipt.DepositToAccountRef))
            SalesReceiptAddRq.DepositToAccountRef.FullName.SetValue(Receipt.DepositToAccountRef);
         
            foreach( SaleItems Item in Receipt.SalesItems) {

            IORSalesReceiptLineAdd ORSalesReceiptLineAddListElement1 = SalesReceiptAddRq.ORSalesReceiptLineAddList.Append();
            
            
                ORSalesReceiptLineAddListElement1.SalesReceiptLineAdd.ItemRef.FullName.SetValue(Item.ItemRef);                
                ORSalesReceiptLineAddListElement1.SalesReceiptLineAdd.Desc.SetValue(Item.Desc);                
                ORSalesReceiptLineAddListElement1.SalesReceiptLineAdd.Quantity.SetValue(Item.Quantity);
                if (Item.Amount != null)
                ORSalesReceiptLineAddListElement1.SalesReceiptLineAdd.Amount.SetValue(Item.Amount.Value);
                if(Item.ORRate != null )
                ORSalesReceiptLineAddListElement1.SalesReceiptLineAdd.ORRatePriceLevel.Rate.SetValue(Item.ORRate.Value);
                
                

            }
            

        }
        void WalkSalesReceiptAddRs(IMsgSetResponse responseMsgSet)
        {
            if (responseMsgSet == null) return;

            IResponseList responseList = responseMsgSet.ResponseList;
            if (responseList == null) return;

            //if we sent only one request, there is only one response, we'll walk the list for this sample
            for (int i = 0; i < responseList.Count; i++)
            {
                IResponse response = responseList.GetAt(i);
                //check the status code of the response, 0=ok, >0 is warning
                if (response.StatusCode >= 0)
                {
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType == ENResponseType.rtSalesReceiptAddRs)
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            ISalesReceiptRet SalesReceiptRet = (ISalesReceiptRet)response.Detail;
                            WalkSalesReceiptRet(SalesReceiptRet);
                        }
                    }
                }
                else {
                    throw new QBException(response.StatusCode, response.StatusMessage.ToString(),requestMsgSet.ToXMLString());
                }
            }
        }
        void WalkSalesReceiptRet(ISalesReceiptRet SalesReceiptRet)
        {
            if (SalesReceiptRet == null) return;

            //Go through all the elements of ISalesReceiptRet
            //Get value of TxnID
            string TxnID6 = (string)SalesReceiptRet.TxnID.GetValue();
            //Get value of TimeCreated
            DateTime TimeCreated7 = (DateTime)SalesReceiptRet.TimeCreated.GetValue();
            //Get value of TimeModified
            DateTime TimeModified8 = (DateTime)SalesReceiptRet.TimeModified.GetValue();
            //Get value of EditSequence
            string EditSequence9 = (string)SalesReceiptRet.EditSequence.GetValue();
            //Get value of TxnNumber
            if (SalesReceiptRet.TxnNumber != null)
            {
                int TxnNumber10 = (int)SalesReceiptRet.TxnNumber.GetValue();
            }
            if (SalesReceiptRet.CustomerRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.CustomerRef.ListID != null)
                {
                    string ListID11 = (string)SalesReceiptRet.CustomerRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.CustomerRef.FullName != null)
                {
                    string FullName12 = (string)SalesReceiptRet.CustomerRef.FullName.GetValue();
                }
            }
            if (SalesReceiptRet.ClassRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.ClassRef.ListID != null)
                {
                    string ListID13 = (string)SalesReceiptRet.ClassRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.ClassRef.FullName != null)
                {
                    string FullName14 = (string)SalesReceiptRet.ClassRef.FullName.GetValue();
                }
            }
            if (SalesReceiptRet.TemplateRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.TemplateRef.ListID != null)
                {
                    string ListID15 = (string)SalesReceiptRet.TemplateRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.TemplateRef.FullName != null)
                {
                    string FullName16 = (string)SalesReceiptRet.TemplateRef.FullName.GetValue();
                }
            }
            //Get value of TxnDate
            DateTime TxnDate17 = (DateTime)SalesReceiptRet.TxnDate.GetValue();
            //Get value of RefNumber
            if (SalesReceiptRet.RefNumber != null)
            {
                string RefNumber18 = (string)SalesReceiptRet.RefNumber.GetValue();
            }
            if (SalesReceiptRet.BillAddress != null)
            {
                //Get value of Addr1
                if (SalesReceiptRet.BillAddress.Addr1 != null)
                {
                    string Addr119 = (string)SalesReceiptRet.BillAddress.Addr1.GetValue();
                }
                //Get value of Addr2
                if (SalesReceiptRet.BillAddress.Addr2 != null)
                {
                    string Addr220 = (string)SalesReceiptRet.BillAddress.Addr2.GetValue();
                }
                //Get value of Addr3
                if (SalesReceiptRet.BillAddress.Addr3 != null)
                {
                    string Addr321 = (string)SalesReceiptRet.BillAddress.Addr3.GetValue();
                }
                //Get value of Addr4
                if (SalesReceiptRet.BillAddress.Addr4 != null)
                {
                    string Addr422 = (string)SalesReceiptRet.BillAddress.Addr4.GetValue();
                }
                //Get value of Addr5
                if (SalesReceiptRet.BillAddress.Addr5 != null)
                {
                    string Addr523 = (string)SalesReceiptRet.BillAddress.Addr5.GetValue();
                }
                //Get value of City
                if (SalesReceiptRet.BillAddress.City != null)
                {
                    string City24 = (string)SalesReceiptRet.BillAddress.City.GetValue();
                }
                //Get value of State
                if (SalesReceiptRet.BillAddress.State != null)
                {
                    string State25 = (string)SalesReceiptRet.BillAddress.State.GetValue();
                }
                //Get value of PostalCode
                if (SalesReceiptRet.BillAddress.PostalCode != null)
                {
                    string PostalCode26 = (string)SalesReceiptRet.BillAddress.PostalCode.GetValue();
                }
                //Get value of Country
                if (SalesReceiptRet.BillAddress.Country != null)
                {
                    string Country27 = (string)SalesReceiptRet.BillAddress.Country.GetValue();
                }
                //Get value of Note
                if (SalesReceiptRet.BillAddress.Note != null)
                {
                    string Note28 = (string)SalesReceiptRet.BillAddress.Note.GetValue();
                }
            }
            if (SalesReceiptRet.BillAddressBlock != null)
            {
                //Get value of Addr1
                if (SalesReceiptRet.BillAddressBlock.Addr1 != null)
                {
                    string Addr129 = (string)SalesReceiptRet.BillAddressBlock.Addr1.GetValue();
                }
                //Get value of Addr2
                if (SalesReceiptRet.BillAddressBlock.Addr2 != null)
                {
                    string Addr230 = (string)SalesReceiptRet.BillAddressBlock.Addr2.GetValue();
                }
                //Get value of Addr3
                if (SalesReceiptRet.BillAddressBlock.Addr3 != null)
                {
                    string Addr331 = (string)SalesReceiptRet.BillAddressBlock.Addr3.GetValue();
                }
                //Get value of Addr4
                if (SalesReceiptRet.BillAddressBlock.Addr4 != null)
                {
                    string Addr432 = (string)SalesReceiptRet.BillAddressBlock.Addr4.GetValue();
                }
                //Get value of Addr5
                if (SalesReceiptRet.BillAddressBlock.Addr5 != null)
                {
                    string Addr533 = (string)SalesReceiptRet.BillAddressBlock.Addr5.GetValue();
                }
            }
            if (SalesReceiptRet.ShipAddress != null)
            {
                //Get value of Addr1
                if (SalesReceiptRet.ShipAddress.Addr1 != null)
                {
                    string Addr134 = (string)SalesReceiptRet.ShipAddress.Addr1.GetValue();
                }
                //Get value of Addr2
                if (SalesReceiptRet.ShipAddress.Addr2 != null)
                {
                    string Addr235 = (string)SalesReceiptRet.ShipAddress.Addr2.GetValue();
                }
                //Get value of Addr3
                if (SalesReceiptRet.ShipAddress.Addr3 != null)
                {
                    string Addr336 = (string)SalesReceiptRet.ShipAddress.Addr3.GetValue();
                }
                //Get value of Addr4
                if (SalesReceiptRet.ShipAddress.Addr4 != null)
                {
                    string Addr437 = (string)SalesReceiptRet.ShipAddress.Addr4.GetValue();
                }
                //Get value of Addr5
                if (SalesReceiptRet.ShipAddress.Addr5 != null)
                {
                    string Addr538 = (string)SalesReceiptRet.ShipAddress.Addr5.GetValue();
                }
                //Get value of City
                if (SalesReceiptRet.ShipAddress.City != null)
                {
                    string City39 = (string)SalesReceiptRet.ShipAddress.City.GetValue();
                }
                //Get value of State
                if (SalesReceiptRet.ShipAddress.State != null)
                {
                    string State40 = (string)SalesReceiptRet.ShipAddress.State.GetValue();
                }
                //Get value of PostalCode
                if (SalesReceiptRet.ShipAddress.PostalCode != null)
                {
                    string PostalCode41 = (string)SalesReceiptRet.ShipAddress.PostalCode.GetValue();
                }
                //Get value of Country
                if (SalesReceiptRet.ShipAddress.Country != null)
                {
                    string Country42 = (string)SalesReceiptRet.ShipAddress.Country.GetValue();
                }
                //Get value of Note
                if (SalesReceiptRet.ShipAddress.Note != null)
                {
                    string Note43 = (string)SalesReceiptRet.ShipAddress.Note.GetValue();
                }
            }
            if (SalesReceiptRet.ShipAddressBlock != null)
            {
                //Get value of Addr1
                if (SalesReceiptRet.ShipAddressBlock.Addr1 != null)
                {
                    string Addr144 = (string)SalesReceiptRet.ShipAddressBlock.Addr1.GetValue();
                }
                //Get value of Addr2
                if (SalesReceiptRet.ShipAddressBlock.Addr2 != null)
                {
                    string Addr245 = (string)SalesReceiptRet.ShipAddressBlock.Addr2.GetValue();
                }
                //Get value of Addr3
                if (SalesReceiptRet.ShipAddressBlock.Addr3 != null)
                {
                    string Addr346 = (string)SalesReceiptRet.ShipAddressBlock.Addr3.GetValue();
                }
                //Get value of Addr4
                if (SalesReceiptRet.ShipAddressBlock.Addr4 != null)
                {
                    string Addr447 = (string)SalesReceiptRet.ShipAddressBlock.Addr4.GetValue();
                }
                //Get value of Addr5
                if (SalesReceiptRet.ShipAddressBlock.Addr5 != null)
                {
                    string Addr548 = (string)SalesReceiptRet.ShipAddressBlock.Addr5.GetValue();
                }
            }
            //Get value of IsPending
            if (SalesReceiptRet.IsPending != null)
            {
                bool IsPending49 = (bool)SalesReceiptRet.IsPending.GetValue();
            }
            //Get value of CheckNumber
            if (SalesReceiptRet.CheckNumber != null)
            {
                string CheckNumber50 = (string)SalesReceiptRet.CheckNumber.GetValue();
            }
            if (SalesReceiptRet.PaymentMethodRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.PaymentMethodRef.ListID != null)
                {
                    string ListID51 = (string)SalesReceiptRet.PaymentMethodRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.PaymentMethodRef.FullName != null)
                {
                    string FullName52 = (string)SalesReceiptRet.PaymentMethodRef.FullName.GetValue();
                }
            }
            //Get value of DueDate
            if (SalesReceiptRet.DueDate != null)
            {
                DateTime DueDate53 = (DateTime)SalesReceiptRet.DueDate.GetValue();
            }
            if (SalesReceiptRet.SalesRepRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.SalesRepRef.ListID != null)
                {
                    string ListID54 = (string)SalesReceiptRet.SalesRepRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.SalesRepRef.FullName != null)
                {
                    string FullName55 = (string)SalesReceiptRet.SalesRepRef.FullName.GetValue();
                }
            }
            //Get value of ShipDate
            if (SalesReceiptRet.ShipDate != null)
            {
                DateTime ShipDate56 = (DateTime)SalesReceiptRet.ShipDate.GetValue();
            }
            if (SalesReceiptRet.ShipMethodRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.ShipMethodRef.ListID != null)
                {
                    string ListID57 = (string)SalesReceiptRet.ShipMethodRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.ShipMethodRef.FullName != null)
                {
                    string FullName58 = (string)SalesReceiptRet.ShipMethodRef.FullName.GetValue();
                }
            }
            //Get value of FOB
            if (SalesReceiptRet.FOB != null)
            {
                string FOB59 = (string)SalesReceiptRet.FOB.GetValue();
            }
            //Get value of Subtotal
            if (SalesReceiptRet.Subtotal != null)
            {
                double Subtotal60 = (double)SalesReceiptRet.Subtotal.GetValue();
            }
            if (SalesReceiptRet.ItemSalesTaxRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.ItemSalesTaxRef.ListID != null)
                {
                    string ListID61 = (string)SalesReceiptRet.ItemSalesTaxRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.ItemSalesTaxRef.FullName != null)
                {
                    string FullName62 = (string)SalesReceiptRet.ItemSalesTaxRef.FullName.GetValue();
                }
            }
            //Get value of SalesTaxPercentage
            if (SalesReceiptRet.SalesTaxPercentage != null)
            {
                double SalesTaxPercentage63 = (double)SalesReceiptRet.SalesTaxPercentage.GetValue();
            }
            //Get value of SalesTaxTotal
            if (SalesReceiptRet.SalesTaxTotal != null)
            {
                double SalesTaxTotal64 = (double)SalesReceiptRet.SalesTaxTotal.GetValue();
            }
            //Get value of TotalAmount
            if (SalesReceiptRet.TotalAmount != null)
            {
                double TotalAmount65 = (double)SalesReceiptRet.TotalAmount.GetValue();
            }
            if (SalesReceiptRet.CurrencyRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.CurrencyRef.ListID != null)
                {
                    string ListID66 = (string)SalesReceiptRet.CurrencyRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.CurrencyRef.FullName != null)
                {
                    string FullName67 = (string)SalesReceiptRet.CurrencyRef.FullName.GetValue();
                }
            }
            //Get value of ExchangeRate
            if (SalesReceiptRet.ExchangeRate != null)
            {
                float ExchangeRate68 = (float)SalesReceiptRet.ExchangeRate.GetValue();
            }
            //Get value of TotalAmountInHomeCurrency
            if (SalesReceiptRet.TotalAmountInHomeCurrency != null)
            {
                double TotalAmountInHomeCurrency69 = (double)SalesReceiptRet.TotalAmountInHomeCurrency.GetValue();
            }
            //Get value of Memo
            if (SalesReceiptRet.Memo != null)
            {
                string Memo70 = (string)SalesReceiptRet.Memo.GetValue();
            }
            if (SalesReceiptRet.CustomerMsgRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.CustomerMsgRef.ListID != null)
                {
                    string ListID71 = (string)SalesReceiptRet.CustomerMsgRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.CustomerMsgRef.FullName != null)
                {
                    string FullName72 = (string)SalesReceiptRet.CustomerMsgRef.FullName.GetValue();
                }
            }
            //Get value of IsToBePrinted
            if (SalesReceiptRet.IsToBePrinted != null)
            {
                bool IsToBePrinted73 = (bool)SalesReceiptRet.IsToBePrinted.GetValue();
            }
            //Get value of IsToBeEmailed
            if (SalesReceiptRet.IsToBeEmailed != null)
            {
                bool IsToBeEmailed74 = (bool)SalesReceiptRet.IsToBeEmailed.GetValue();
            }
            if (SalesReceiptRet.CustomerSalesTaxCodeRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.CustomerSalesTaxCodeRef.ListID != null)
                {
                    string ListID75 = (string)SalesReceiptRet.CustomerSalesTaxCodeRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.CustomerSalesTaxCodeRef.FullName != null)
                {
                    string FullName76 = (string)SalesReceiptRet.CustomerSalesTaxCodeRef.FullName.GetValue();
                }
            }
            if (SalesReceiptRet.DepositToAccountRef != null)
            {
                //Get value of ListID
                if (SalesReceiptRet.DepositToAccountRef.ListID != null)
                {
                    string ListID77 = (string)SalesReceiptRet.DepositToAccountRef.ListID.GetValue();
                }
                //Get value of FullName
                if (SalesReceiptRet.DepositToAccountRef.FullName != null)
                {
                    string FullName78 = (string)SalesReceiptRet.DepositToAccountRef.FullName.GetValue();
                }
            }
            if (SalesReceiptRet.CreditCardTxnInfo != null)
            {
                //Get value of CreditCardNumber
                string CreditCardNumber79 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardNumber.GetValue();
                //Get value of ExpirationMonth
                int ExpirationMonth80 = (int)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.ExpirationMonth.GetValue();
                //Get value of ExpirationYear
                int ExpirationYear81 = (int)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.ExpirationYear.GetValue();
                //Get value of NameOnCard
                string NameOnCard82 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.NameOnCard.GetValue();
                //Get value of CreditCardAddress
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardAddress != null)
                {
                    string CreditCardAddress83 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardAddress.GetValue();
                }
                //Get value of CreditCardPostalCode
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardPostalCode != null)
                {
                    string CreditCardPostalCode84 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardPostalCode.GetValue();
                }
                //Get value of CommercialCardCode
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CommercialCardCode != null)
                {
                    string CommercialCardCode85 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CommercialCardCode.GetValue();
                }
                //Get value of TransactionMode
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.TransactionMode != null)
                {
                    ENTransactionMode TransactionMode86 = (ENTransactionMode)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.TransactionMode.GetValue();
                }
                //Get value of CreditCardTxnType
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardTxnType != null)
                {
                    ENCreditCardTxnType CreditCardTxnType87 = (ENCreditCardTxnType)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardTxnType.GetValue();
                }
                //Get value of ResultCode
                int ResultCode88 = (int)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ResultCode.GetValue();
                //Get value of ResultMessage
                string ResultMessage89 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ResultMessage.GetValue();
                //Get value of CreditCardTransID
                string CreditCardTransID90 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CreditCardTransID.GetValue();
                //Get value of MerchantAccountNumber
                string MerchantAccountNumber91 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.MerchantAccountNumber.GetValue();
                //Get value of AuthorizationCode
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AuthorizationCode != null)
                {
                    string AuthorizationCode92 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AuthorizationCode.GetValue();
                }
                //Get value of AVSStreet
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSStreet != null)
                {
                    ENAVSStreet AVSStreet93 = (ENAVSStreet)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSStreet.GetValue();
                }
                //Get value of AVSZip
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSZip != null)
                {
                    ENAVSZip AVSZip94 = (ENAVSZip)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSZip.GetValue();
                }
                //Get value of CardSecurityCodeMatch
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CardSecurityCodeMatch != null)
                {
                    ENCardSecurityCodeMatch CardSecurityCodeMatch95 = (ENCardSecurityCodeMatch)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CardSecurityCodeMatch.GetValue();
                }
                //Get value of ReconBatchID
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ReconBatchID != null)
                {
                    string ReconBatchID96 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ReconBatchID.GetValue();
                }
                //Get value of PaymentGroupingCode
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentGroupingCode != null)
                {
                    int PaymentGroupingCode97 = (int)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentGroupingCode.GetValue();
                }
                //Get value of PaymentStatus
                ENPaymentStatus PaymentStatus98 = (ENPaymentStatus)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentStatus.GetValue();
                //Get value of TxnAuthorizationTime
                DateTime TxnAuthorizationTime99 = (DateTime)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationTime.GetValue();
                //Get value of TxnAuthorizationStamp
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationStamp != null)
                {
                    int TxnAuthorizationStamp100 = (int)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationStamp.GetValue();
                }
                //Get value of ClientTransID
                if (SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ClientTransID != null)
                {
                    string ClientTransID101 = (string)SalesReceiptRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ClientTransID.GetValue();
                }
            }
            //Get value of Other
            if (SalesReceiptRet.Other != null)
            {
                string Other102 = (string)SalesReceiptRet.Other.GetValue();
            }
            //Get value of ExternalGUID
            if (SalesReceiptRet.ExternalGUID != null)
            {
                string ExternalGUID103 = (string)SalesReceiptRet.ExternalGUID.GetValue();
            }
            if (SalesReceiptRet.ORSalesReceiptLineRetList != null)
            {
                for (int i104 = 0; i104 < SalesReceiptRet.ORSalesReceiptLineRetList.Count; i104++)
                {
                    IORSalesReceiptLineRet ORSalesReceiptLineRet = SalesReceiptRet.ORSalesReceiptLineRetList.GetAt(i104);
                    if (ORSalesReceiptLineRet.SalesReceiptLineRet != null)
                    {
                        if (ORSalesReceiptLineRet.SalesReceiptLineRet != null)
                        {
                            //Get value of TxnLineID
                            string TxnLineID105 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.TxnLineID.GetValue();
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef != null)
                            {
                                //Get value of ListID
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef.ListID != null)
                                {
                                    string ListID106 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef.ListID.GetValue();
                                }
                                //Get value of FullName
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef.FullName != null)
                                {
                                    string FullName107 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef.FullName.GetValue();
                                }
                            }
                            //Get value of Desc
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.Desc != null)
                            {
                                string Desc108 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.Desc.GetValue();
                            }
                            //Get value of Quantity
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.Quantity != null)
                            {
                                int Quantity109 = (int)ORSalesReceiptLineRet.SalesReceiptLineRet.Quantity.GetValue();
                            }
                            //Get value of UnitOfMeasure
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.UnitOfMeasure != null)
                            {
                                string UnitOfMeasure110 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.UnitOfMeasure.GetValue();
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.OverrideUOMSetRef != null)
                            {
                                //Get value of ListID
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.OverrideUOMSetRef.ListID != null)
                                {
                                    string ListID111 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.OverrideUOMSetRef.ListID.GetValue();
                                }
                                //Get value of FullName
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.OverrideUOMSetRef.FullName != null)
                                {
                                    string FullName112 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.OverrideUOMSetRef.FullName.GetValue();
                                }
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate != null)
                            {
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.Rate != null)
                                {
                                    //Get value of Rate
                                    if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.Rate != null)
                                    {
                                        double Rate113 = (double)ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.Rate.GetValue();
                                    }
                                }
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.RatePercent != null)
                                {
                                    //Get value of RatePercent
                                    if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.RatePercent != null)
                                    {
                                        double RatePercent114 = (double)ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.RatePercent.GetValue();
                                    }
                                }
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.ClassRef != null)
                            {
                                //Get value of ListID
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ClassRef.ListID != null)
                                {
                                    string ListID115 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.ClassRef.ListID.GetValue();
                                }
                                //Get value of FullName
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ClassRef.FullName != null)
                                {
                                    string FullName116 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.ClassRef.FullName.GetValue();
                                }
                            }
                            //Get value of Amount
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.Amount != null)
                            {
                                double Amount117 = (double)ORSalesReceiptLineRet.SalesReceiptLineRet.Amount.GetValue();
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.InventorySiteRef != null)
                            {
                                //Get value of ListID
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.InventorySiteRef.ListID != null)
                                {
                                    string ListID118 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.InventorySiteRef.ListID.GetValue();
                                }
                                //Get value of FullName
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.InventorySiteRef.FullName != null)
                                {
                                    string FullName119 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.InventorySiteRef.FullName.GetValue();
                                }
                            }
                            //Get value of ServiceDate
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.ServiceDate != null)
                            {
                                DateTime ServiceDate120 = (DateTime)ORSalesReceiptLineRet.SalesReceiptLineRet.ServiceDate.GetValue();
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.SalesTaxCodeRef != null)
                            {
                                //Get value of ListID
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.SalesTaxCodeRef.ListID != null)
                                {
                                    string ListID121 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.SalesTaxCodeRef.ListID.GetValue();
                                }
                                //Get value of FullName
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.SalesTaxCodeRef.FullName != null)
                                {
                                    string FullName122 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.SalesTaxCodeRef.FullName.GetValue();
                                }
                            }
                            //Get value of Other1
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.Other1 != null)
                            {
                                string Other1123 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.Other1.GetValue();
                            }
                            //Get value of Other2
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.Other2 != null)
                            {
                                string Other2124 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.Other2.GetValue();
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo != null)
                            {
                                //Get value of CreditCardNumber
                                string CreditCardNumber125 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardNumber.GetValue();
                                //Get value of ExpirationMonth
                                int ExpirationMonth126 = (int)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.ExpirationMonth.GetValue();
                                //Get value of ExpirationYear
                                int ExpirationYear127 = (int)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.ExpirationYear.GetValue();
                                //Get value of NameOnCard
                                string NameOnCard128 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.NameOnCard.GetValue();
                                //Get value of CreditCardAddress
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardAddress != null)
                                {
                                    string CreditCardAddress129 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardAddress.GetValue();
                                }
                                //Get value of CreditCardPostalCode
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardPostalCode != null)
                                {
                                    string CreditCardPostalCode130 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardPostalCode.GetValue();
                                }
                                //Get value of CommercialCardCode
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CommercialCardCode != null)
                                {
                                    string CommercialCardCode131 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CommercialCardCode.GetValue();
                                }
                                //Get value of TransactionMode
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.TransactionMode != null)
                                {
                                    ENTransactionMode TransactionMode132 = (ENTransactionMode)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.TransactionMode.GetValue();
                                }
                                //Get value of CreditCardTxnType
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardTxnType != null)
                                {
                                    ENCreditCardTxnType CreditCardTxnType133 = (ENCreditCardTxnType)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardTxnType.GetValue();
                                }
                                //Get value of ResultCode
                                int ResultCode134 = (int)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ResultCode.GetValue();
                                //Get value of ResultMessage
                                string ResultMessage135 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ResultMessage.GetValue();
                                //Get value of CreditCardTransID
                                string CreditCardTransID136 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CreditCardTransID.GetValue();
                                //Get value of MerchantAccountNumber
                                string MerchantAccountNumber137 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.MerchantAccountNumber.GetValue();
                                //Get value of AuthorizationCode
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AuthorizationCode != null)
                                {
                                    string AuthorizationCode138 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AuthorizationCode.GetValue();
                                }
                                //Get value of AVSStreet
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSStreet != null)
                                {
                                    ENAVSStreet AVSStreet139 = (ENAVSStreet)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSStreet.GetValue();
                                }
                                //Get value of AVSZip
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSZip != null)
                                {
                                    ENAVSZip AVSZip140 = (ENAVSZip)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSZip.GetValue();
                                }
                                //Get value of CardSecurityCodeMatch
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CardSecurityCodeMatch != null)
                                {
                                    ENCardSecurityCodeMatch CardSecurityCodeMatch141 = (ENCardSecurityCodeMatch)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CardSecurityCodeMatch.GetValue();
                                }
                                //Get value of ReconBatchID
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ReconBatchID != null)
                                {
                                    string ReconBatchID142 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ReconBatchID.GetValue();
                                }
                                //Get value of PaymentGroupingCode
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentGroupingCode != null)
                                {
                                    int PaymentGroupingCode143 = (int)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentGroupingCode.GetValue();
                                }
                                //Get value of PaymentStatus
                                ENPaymentStatus PaymentStatus144 = (ENPaymentStatus)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentStatus.GetValue();
                                //Get value of TxnAuthorizationTime
                                DateTime TxnAuthorizationTime145 = (DateTime)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationTime.GetValue();
                                //Get value of TxnAuthorizationStamp
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationStamp != null)
                                {
                                    int TxnAuthorizationStamp146 = (int)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationStamp.GetValue();
                                }
                                //Get value of ClientTransID
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ClientTransID != null)
                                {
                                    string ClientTransID147 = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ClientTransID.GetValue();
                                }
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.DataExtRetList != null)
                            {
                                for (int i148 = 0; i148 < ORSalesReceiptLineRet.SalesReceiptLineRet.DataExtRetList.Count; i148++)
                                {
                                    IDataExtRet DataExtRet = ORSalesReceiptLineRet.SalesReceiptLineRet.DataExtRetList.GetAt(i148);
                                    //Get value of OwnerID
                                    if (DataExtRet.OwnerID != null)
                                    {
                                        string OwnerID149 = (string)DataExtRet.OwnerID.GetValue();
                                    }
                                    //Get value of DataExtName
                                    string DataExtName150 = (string)DataExtRet.DataExtName.GetValue();
                                    //Get value of DataExtType
                                    ENDataExtType DataExtType151 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                                    //Get value of DataExtValue
                                    string DataExtValue152 = (string)DataExtRet.DataExtValue.GetValue();
                                }
                            }
                        }
                    }
                    if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet != null)
                    {
                        if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet != null)
                        {
                            //Get value of TxnLineID
                            string TxnLineID153 = (string)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.TxnLineID.GetValue();
                            //Get value of ListID
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.ItemGroupRef.ListID != null)
                            {
                                string ListID154 = (string)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.ItemGroupRef.ListID.GetValue();
                            }
                            //Get value of FullName
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.ItemGroupRef.FullName != null)
                            {
                                string FullName155 = (string)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.ItemGroupRef.FullName.GetValue();
                            }
                            //Get value of Desc
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.Desc != null)
                            {
                                string Desc156 = (string)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.Desc.GetValue();
                            }
                            //Get value of Quantity
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.Quantity != null)
                            {
                                int Quantity157 = (int)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.Quantity.GetValue();
                            }
                            //Get value of UnitOfMeasure
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.UnitOfMeasure != null)
                            {
                                string UnitOfMeasure158 = (string)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.UnitOfMeasure.GetValue();
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.OverrideUOMSetRef != null)
                            {
                                //Get value of ListID
                                if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.OverrideUOMSetRef.ListID != null)
                                {
                                    string ListID159 = (string)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.OverrideUOMSetRef.ListID.GetValue();
                                }
                                //Get value of FullName
                                if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.OverrideUOMSetRef.FullName != null)
                                {
                                    string FullName160 = (string)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.OverrideUOMSetRef.FullName.GetValue();
                                }
                            }
                            //Get value of IsPrintItemsInGroup
                            bool IsPrintItemsInGroup161 = (bool)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.IsPrintItemsInGroup.GetValue();
                            //Get value of TotalAmount
                            double TotalAmount162 = (double)ORSalesReceiptLineRet.SalesReceiptLineGroupRet.TotalAmount.GetValue();
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.SalesReceiptLineRetList != null)
                            {
                                for (int i163 = 0; i163 < ORSalesReceiptLineRet.SalesReceiptLineGroupRet.SalesReceiptLineRetList.Count; i163++)
                                {
                                    ISalesReceiptLineRet SalesReceiptLineRet = ORSalesReceiptLineRet.SalesReceiptLineGroupRet.SalesReceiptLineRetList.GetAt(i163);
                                    //Get value of TxnLineID
                                    string TxnLineID164 = (string)SalesReceiptLineRet.TxnLineID.GetValue();
                                    if (SalesReceiptLineRet.ItemRef != null)
                                    {
                                        //Get value of ListID
                                        if (SalesReceiptLineRet.ItemRef.ListID != null)
                                        {
                                            string ListID165 = (string)SalesReceiptLineRet.ItemRef.ListID.GetValue();
                                        }
                                        //Get value of FullName
                                        if (SalesReceiptLineRet.ItemRef.FullName != null)
                                        {
                                            string FullName166 = (string)SalesReceiptLineRet.ItemRef.FullName.GetValue();
                                        }
                                    }
                                    //Get value of Desc
                                    if (SalesReceiptLineRet.Desc != null)
                                    {
                                        string Desc167 = (string)SalesReceiptLineRet.Desc.GetValue();
                                    }
                                    //Get value of Quantity
                                    if (SalesReceiptLineRet.Quantity != null)
                                    {
                                        int Quantity168 = (int)SalesReceiptLineRet.Quantity.GetValue();
                                    }
                                    //Get value of UnitOfMeasure
                                    if (SalesReceiptLineRet.UnitOfMeasure != null)
                                    {
                                        string UnitOfMeasure169 = (string)SalesReceiptLineRet.UnitOfMeasure.GetValue();
                                    }
                                    if (SalesReceiptLineRet.OverrideUOMSetRef != null)
                                    {
                                        //Get value of ListID
                                        if (SalesReceiptLineRet.OverrideUOMSetRef.ListID != null)
                                        {
                                            string ListID170 = (string)SalesReceiptLineRet.OverrideUOMSetRef.ListID.GetValue();
                                        }
                                        //Get value of FullName
                                        if (SalesReceiptLineRet.OverrideUOMSetRef.FullName != null)
                                        {
                                            string FullName171 = (string)SalesReceiptLineRet.OverrideUOMSetRef.FullName.GetValue();
                                        }
                                    }
                                    if (SalesReceiptLineRet.ORRate != null)
                                    {
                                        if (SalesReceiptLineRet.ORRate.Rate != null)
                                        {
                                            //Get value of Rate
                                            if (SalesReceiptLineRet.ORRate.Rate != null)
                                            {
                                                double Rate172 = (double)SalesReceiptLineRet.ORRate.Rate.GetValue();
                                            }
                                        }
                                        if (SalesReceiptLineRet.ORRate.RatePercent != null)
                                        {
                                            //Get value of RatePercent
                                            if (SalesReceiptLineRet.ORRate.RatePercent != null)
                                            {
                                                double RatePercent173 = (double)SalesReceiptLineRet.ORRate.RatePercent.GetValue();
                                            }
                                        }
                                    }
                                    if (SalesReceiptLineRet.ClassRef != null)
                                    {
                                        //Get value of ListID
                                        if (SalesReceiptLineRet.ClassRef.ListID != null)
                                        {
                                            string ListID174 = (string)SalesReceiptLineRet.ClassRef.ListID.GetValue();
                                        }
                                        //Get value of FullName
                                        if (SalesReceiptLineRet.ClassRef.FullName != null)
                                        {
                                            string FullName175 = (string)SalesReceiptLineRet.ClassRef.FullName.GetValue();
                                        }
                                    }
                                    //Get value of Amount
                                    if (SalesReceiptLineRet.Amount != null)
                                    {
                                        double Amount176 = (double)SalesReceiptLineRet.Amount.GetValue();
                                    }
                                    if (SalesReceiptLineRet.InventorySiteRef != null)
                                    {
                                        //Get value of ListID
                                        if (SalesReceiptLineRet.InventorySiteRef.ListID != null)
                                        {
                                            string ListID177 = (string)SalesReceiptLineRet.InventorySiteRef.ListID.GetValue();
                                        }
                                        //Get value of FullName
                                        if (SalesReceiptLineRet.InventorySiteRef.FullName != null)
                                        {
                                            string FullName178 = (string)SalesReceiptLineRet.InventorySiteRef.FullName.GetValue();
                                        }
                                    }
                                    //Get value of ServiceDate
                                    if (SalesReceiptLineRet.ServiceDate != null)
                                    {
                                        DateTime ServiceDate179 = (DateTime)SalesReceiptLineRet.ServiceDate.GetValue();
                                    }
                                    if (SalesReceiptLineRet.SalesTaxCodeRef != null)
                                    {
                                        //Get value of ListID
                                        if (SalesReceiptLineRet.SalesTaxCodeRef.ListID != null)
                                        {
                                            string ListID180 = (string)SalesReceiptLineRet.SalesTaxCodeRef.ListID.GetValue();
                                        }
                                        //Get value of FullName
                                        if (SalesReceiptLineRet.SalesTaxCodeRef.FullName != null)
                                        {
                                            string FullName181 = (string)SalesReceiptLineRet.SalesTaxCodeRef.FullName.GetValue();
                                        }
                                    }
                                    //Get value of Other1
                                    if (SalesReceiptLineRet.Other1 != null)
                                    {
                                        string Other1182 = (string)SalesReceiptLineRet.Other1.GetValue();
                                    }
                                    //Get value of Other2
                                    if (SalesReceiptLineRet.Other2 != null)
                                    {
                                        string Other2183 = (string)SalesReceiptLineRet.Other2.GetValue();
                                    }
                                    if (SalesReceiptLineRet.CreditCardTxnInfo != null)
                                    {
                                        //Get value of CreditCardNumber
                                        string CreditCardNumber184 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardNumber.GetValue();
                                        //Get value of ExpirationMonth
                                        int ExpirationMonth185 = (int)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.ExpirationMonth.GetValue();
                                        //Get value of ExpirationYear
                                        int ExpirationYear186 = (int)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.ExpirationYear.GetValue();
                                        //Get value of NameOnCard
                                        string NameOnCard187 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.NameOnCard.GetValue();
                                        //Get value of CreditCardAddress
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardAddress != null)
                                        {
                                            string CreditCardAddress188 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardAddress.GetValue();
                                        }
                                        //Get value of CreditCardPostalCode
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardPostalCode != null)
                                        {
                                            string CreditCardPostalCode189 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardPostalCode.GetValue();
                                        }
                                        //Get value of CommercialCardCode
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CommercialCardCode != null)
                                        {
                                            string CommercialCardCode190 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CommercialCardCode.GetValue();
                                        }
                                        //Get value of TransactionMode
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.TransactionMode != null)
                                        {
                                            ENTransactionMode TransactionMode191 = (ENTransactionMode)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.TransactionMode.GetValue();
                                        }
                                        //Get value of CreditCardTxnType
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardTxnType != null)
                                        {
                                            ENCreditCardTxnType CreditCardTxnType192 = (ENCreditCardTxnType)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnInputInfo.CreditCardTxnType.GetValue();
                                        }
                                        //Get value of ResultCode
                                        int ResultCode193 = (int)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ResultCode.GetValue();
                                        //Get value of ResultMessage
                                        string ResultMessage194 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ResultMessage.GetValue();
                                        //Get value of CreditCardTransID
                                        string CreditCardTransID195 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CreditCardTransID.GetValue();
                                        //Get value of MerchantAccountNumber
                                        string MerchantAccountNumber196 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.MerchantAccountNumber.GetValue();
                                        //Get value of AuthorizationCode
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AuthorizationCode != null)
                                        {
                                            string AuthorizationCode197 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AuthorizationCode.GetValue();
                                        }
                                        //Get value of AVSStreet
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSStreet != null)
                                        {
                                            ENAVSStreet AVSStreet198 = (ENAVSStreet)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSStreet.GetValue();
                                        }
                                        //Get value of AVSZip
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSZip != null)
                                        {
                                            ENAVSZip AVSZip199 = (ENAVSZip)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.AVSZip.GetValue();
                                        }
                                        //Get value of CardSecurityCodeMatch
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CardSecurityCodeMatch != null)
                                        {
                                            ENCardSecurityCodeMatch CardSecurityCodeMatch200 = (ENCardSecurityCodeMatch)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.CardSecurityCodeMatch.GetValue();
                                        }
                                        //Get value of ReconBatchID
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ReconBatchID != null)
                                        {
                                            string ReconBatchID201 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ReconBatchID.GetValue();
                                        }
                                        //Get value of PaymentGroupingCode
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentGroupingCode != null)
                                        {
                                            int PaymentGroupingCode202 = (int)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentGroupingCode.GetValue();
                                        }
                                        //Get value of PaymentStatus
                                        ENPaymentStatus PaymentStatus203 = (ENPaymentStatus)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.PaymentStatus.GetValue();
                                        //Get value of TxnAuthorizationTime
                                        DateTime TxnAuthorizationTime204 = (DateTime)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationTime.GetValue();
                                        //Get value of TxnAuthorizationStamp
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationStamp != null)
                                        {
                                            int TxnAuthorizationStamp205 = (int)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.TxnAuthorizationStamp.GetValue();
                                        }
                                        //Get value of ClientTransID
                                        if (SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ClientTransID != null)
                                        {
                                            string ClientTransID206 = (string)SalesReceiptLineRet.CreditCardTxnInfo.CreditCardTxnResultInfo.ClientTransID.GetValue();
                                        }
                                    }
                                    if (SalesReceiptLineRet.DataExtRetList != null)
                                    {
                                        for (int i207 = 0; i207 < SalesReceiptLineRet.DataExtRetList.Count; i207++)
                                        {
                                            IDataExtRet DataExtRet = SalesReceiptLineRet.DataExtRetList.GetAt(i207);
                                            //Get value of OwnerID
                                            if (DataExtRet.OwnerID != null)
                                            {
                                                string OwnerID208 = (string)DataExtRet.OwnerID.GetValue();
                                            }
                                            //Get value of DataExtName
                                            string DataExtName209 = (string)DataExtRet.DataExtName.GetValue();
                                            //Get value of DataExtType
                                            ENDataExtType DataExtType210 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                                            //Get value of DataExtValue
                                            string DataExtValue211 = (string)DataExtRet.DataExtValue.GetValue();
                                        }
                                    }
                                }
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineGroupRet.DataExtRetList != null)
                            {
                                for (int i212 = 0; i212 < ORSalesReceiptLineRet.SalesReceiptLineGroupRet.DataExtRetList.Count; i212++)
                                {
                                    IDataExtRet DataExtRet = ORSalesReceiptLineRet.SalesReceiptLineGroupRet.DataExtRetList.GetAt(i212);
                                    //Get value of OwnerID
                                    if (DataExtRet.OwnerID != null)
                                    {
                                        string OwnerID213 = (string)DataExtRet.OwnerID.GetValue();
                                    }
                                    //Get value of DataExtName
                                    string DataExtName214 = (string)DataExtRet.DataExtName.GetValue();
                                    //Get value of DataExtType
                                    ENDataExtType DataExtType215 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                                    //Get value of DataExtValue
                                    string DataExtValue216 = (string)DataExtRet.DataExtValue.GetValue();
                                }
                            }
                        }
                    }
                }
            }
            if (SalesReceiptRet.DataExtRetList != null)
            {
                for (int i217 = 0; i217 < SalesReceiptRet.DataExtRetList.Count; i217++)
                {
                    IDataExtRet DataExtRet = SalesReceiptRet.DataExtRetList.GetAt(i217);
                    //Get value of OwnerID
                    if (DataExtRet.OwnerID != null)
                    {
                        string OwnerID218 = (string)DataExtRet.OwnerID.GetValue();
                    }
                    //Get value of DataExtName
                    string DataExtName219 = (string)DataExtRet.DataExtName.GetValue();
                    //Get value of DataExtType
                    ENDataExtType DataExtType220 = (ENDataExtType)DataExtRet.DataExtType.GetValue();
                    //Get value of DataExtValue
                    string DataExtValue221 = (string)DataExtRet.DataExtValue.GetValue();
                }
            }
        }

        #endregion

        #region Update

        public string UpdateSalesInvoice(SalesReceipt Receipt) {

            try
            {
                BuildSalesReceiptUpdateRq(Receipt);
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
        void BuildSalesReceiptUpdateRq(SalesReceipt Receipt)
        {
            requestMsgSet.ClearRequests();
            ISalesReceiptMod SalesReceiptAddRq = requestMsgSet.AppendSalesReceiptModRq();
            //Set Update Attribute 

            SalesReceiptAddRq.EditSequence.SetValue(Receipt.EditSequence);
            SalesReceiptAddRq.TxnID.SetValue(Receipt.TxnID);

            SalesReceiptAddRq.CustomerRef.FullName.SetValue(Receipt.Customer);
            //SalesReceiptAddRq.ClassRef.FullName.SetValue("ab");            
            if (Receipt.TxnDate != null)
                SalesReceiptAddRq.TxnDate.SetValue(Receipt.TxnDate.Value);
            SalesReceiptAddRq.RefNumber.SetValue(Receipt.RefNumber);
            SalesReceiptAddRq.BillAddress.Addr1.SetValue(Receipt.BillAddress1);
            SalesReceiptAddRq.BillAddress.City.SetValue(Receipt.City);
            SalesReceiptAddRq.BillAddress.State.SetValue(Receipt.State);
            SalesReceiptAddRq.BillAddress.PostalCode.SetValue(Receipt.PostalCode);
            SalesReceiptAddRq.BillAddress.Country.SetValue(Receipt.Country);
            SalesReceiptAddRq.BillAddress.Note.SetValue(Receipt.Note);

            SalesReceiptAddRq.ShipAddress.Addr1.SetValue(Receipt.ShipAddress);
            SalesReceiptAddRq.ShipAddress.City.SetValue(Receipt.Ship_City);
            SalesReceiptAddRq.ShipAddress.State.SetValue(Receipt.Ship_State);
            SalesReceiptAddRq.ShipAddress.PostalCode.SetValue(Receipt.Ship_PostalCode);
            SalesReceiptAddRq.ShipAddress.Country.SetValue(Receipt.Ship_Country);
            SalesReceiptAddRq.ShipAddress.Note.SetValue(Receipt.Ship_Note);
            SalesReceiptAddRq.IsPending.SetValue(Receipt.IsPending);

            if (!string.IsNullOrEmpty(Receipt.CheckNumber))
                SalesReceiptAddRq.CheckNumber.SetValue(Receipt.CheckNumber);
            SalesReceiptAddRq.PaymentMethodRef.FullName.SetValue(Receipt.PaymentMethodRef);

            if (Receipt.DueDate != null)
                SalesReceiptAddRq.DueDate.SetValue(Receipt.DueDate.Value);

            if(! string.IsNullOrEmpty( Receipt.SalesRepRef))
            SalesReceiptAddRq.SalesRepRef.FullName.SetValue(Receipt.SalesRepRef);
            if (Receipt.Ship_Date != null)
                SalesReceiptAddRq.ShipDate.SetValue(Receipt.Ship_Date.Value);


            if (!string.IsNullOrEmpty(Receipt.Ship_Method))
                SalesReceiptAddRq.ShipMethodRef.FullName.SetValue(Receipt.Ship_Method);

            SalesReceiptAddRq.Memo.SetValue(Receipt.Memo);
            SalesReceiptAddRq.IsToBePrinted.SetValue(Receipt.isPrinted);
            SalesReceiptAddRq.IsToBeEmailed.SetValue(Receipt.IsEmail);
            SalesReceiptAddRq.DepositToAccountRef.FullName.SetValue(Receipt.DepositToAccountRef);

            foreach (SaleItems Item in Receipt.SalesItems)
            {

                IORSalesReceiptLineMod ORSalesReceiptLineAddListElement1 = SalesReceiptAddRq.ORSalesReceiptLineModList.Append();

                ORSalesReceiptLineAddListElement1.SalesReceiptLineMod.TxnLineID.SetValue(Item.TxnLineID);
                ORSalesReceiptLineAddListElement1.SalesReceiptLineMod.ItemRef.FullName.SetValue(Item.ItemRef);
                ORSalesReceiptLineAddListElement1.SalesReceiptLineMod.Desc.SetValue(Item.Desc);
                ORSalesReceiptLineAddListElement1.SalesReceiptLineMod.Quantity.SetValue(Item.Quantity);
                if (Item.Amount != null)
                    ORSalesReceiptLineAddListElement1.SalesReceiptLineMod.Amount.SetValue(Item.Amount.Value);
                if (Item.ORRate != null)
                    ORSalesReceiptLineAddListElement1.SalesReceiptLineMod.ORRatePriceLevel.Rate.SetValue(Item.ORRate.Value);



            }




        }
        #endregion

        #region Search

        public List<SalesReceipt> SearchSalesReceipt()
        {

            ISalesReceiptQuery SalesReceiptQueryRq = requestMsgSet.AppendSalesReceiptQueryRq();
            SalesReceiptQueryRq.IncludeLineItems.SetValue(true);
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            return WalkSalesReceiptSearchRs(responseMsgSet);
        }
        List<SalesReceipt> WalkSalesReceiptSearchRs(IMsgSetResponse responseMsgSet)
        {
            if (responseMsgSet == null) return null;

            IResponseList responseList = responseMsgSet.ResponseList;
            if (responseList == null) return null;
            Receipts = new List<SalesReceipt>();
            //if we sent only one request, there is only one response, we'll walk the list for this sample
            for (int i = 0; i < responseList.Count; i++)
            {
                IResponse response = responseList.GetAt(i);
                //check the status code of the response, 0=ok, >0 is warning
                if (response.StatusCode >= 0)
                {
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType == ENResponseType.rtSalesReceiptQueryRs)
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            ISalesReceiptRetList SalesReceiptRet = (ISalesReceiptRetList)response.Detail;
                            int count = SalesReceiptRet.Count;
                            for (int a = 0; a < count; a++)
                            {
                                
                                WalkSalesReceiptSearch(SalesReceiptRet.GetAt(a));
                            }
                        }
                    }
                }
            }

            return Receipts;
        }
        void WalkSalesReceiptSearch(ISalesReceiptRet SalesReceiptRet)
        {
            if (SalesReceiptRet == null) return;

            SalesReceipt receipt = new SalesReceipt();
            SaleItems Item ;
            receipt.TxnID = (string)SalesReceiptRet.TxnID.GetValue();            
            receipt.EditSequence = (string)SalesReceiptRet.EditSequence.GetValue();
            //Get value of TxnNumber
            if (SalesReceiptRet.TxnNumber != null)
            {
                receipt.TxNumber = (int)SalesReceiptRet.TxnNumber.GetValue();
            }
            if (SalesReceiptRet.CustomerRef != null)
            {
                //Get value of FullName
                if (SalesReceiptRet.CustomerRef.FullName != null)
                {
                    receipt.Customer = (string)SalesReceiptRet.CustomerRef.FullName.GetValue();
                }
            }
            if (SalesReceiptRet.ClassRef != null)
            {  
                if (SalesReceiptRet.ClassRef.FullName != null)
                {
                   receipt.ClassRef = (string)SalesReceiptRet.ClassRef.FullName.GetValue();
                }
            }
            //Get value of TxnDate
            receipt.TxnDate = (DateTime)SalesReceiptRet.TxnDate.GetValue();
            //Get value of RefNumber
            if (SalesReceiptRet.RefNumber != null)
            {
                receipt.RefNumber = (string)SalesReceiptRet.RefNumber.GetValue();
            }
            if (SalesReceiptRet.BillAddress != null)
            {
                //Get value of Addr1
                if (SalesReceiptRet.BillAddress.Addr1 != null)
                {
                    receipt.BillAddress1 = (string)SalesReceiptRet.BillAddress.Addr1.GetValue();
                }
              
                if (SalesReceiptRet.BillAddress.City != null)
                {
                    receipt.City = (string)SalesReceiptRet.BillAddress.City.GetValue();
                }
                //Get value of State
                if (SalesReceiptRet.BillAddress.State != null)
                {
                  receipt.State = (string)SalesReceiptRet.BillAddress.State.GetValue();
                }
                //Get value of PostalCode
                if (SalesReceiptRet.BillAddress.PostalCode != null)
                {
                   receipt.PostalCode = (string)SalesReceiptRet.BillAddress.PostalCode.GetValue();
                }
                //Get value of Country
                if (SalesReceiptRet.BillAddress.Country != null)
                {
                   receipt.Country = (string)SalesReceiptRet.BillAddress.Country.GetValue();
                }
                //Get value of Note
                if (SalesReceiptRet.BillAddress.Note != null)
                {
                    receipt.Note = (string)SalesReceiptRet.BillAddress.Note.GetValue();
                }
            }
           
            if (SalesReceiptRet.ShipAddress != null)
            {
                //Get value of Addr1
                if (SalesReceiptRet.ShipAddress.Addr1 != null)
                {
                   receipt.ShipAddress= (string)SalesReceiptRet.ShipAddress.Addr1.GetValue();
                }
                //Get value of City
                if (SalesReceiptRet.ShipAddress.City != null)
                {
                   receipt.Ship_City = (string)SalesReceiptRet.ShipAddress.City.GetValue();
                }
                //Get value of State
                if (SalesReceiptRet.ShipAddress.State != null)
                {
                    receipt.State = (string)SalesReceiptRet.ShipAddress.State.GetValue();
                }
                //Get value of PostalCode
                if (SalesReceiptRet.ShipAddress.PostalCode != null)
                {
                    receipt.PostalCode= (string)SalesReceiptRet.ShipAddress.PostalCode.GetValue();
                }
                //Get value of Country
                if (SalesReceiptRet.ShipAddress.Country != null)
                {
                    receipt.Ship_Country = (string)SalesReceiptRet.ShipAddress.Country.GetValue();
                }
                //Get value of Note
                if (SalesReceiptRet.ShipAddress.Note != null)
                {
                    receipt.Ship_Note = (string)SalesReceiptRet.ShipAddress.Note.GetValue();
                }
            }
           
            //Get value of IsPending
            if (SalesReceiptRet.IsPending != null)
            {
                receipt.IsPending = (bool)SalesReceiptRet.IsPending.GetValue();
            }
            //Get value of CheckNumber
            if (SalesReceiptRet.CheckNumber != null)
            {
                receipt.CheckNumber = (string)SalesReceiptRet.CheckNumber.GetValue();
            }
            if (SalesReceiptRet.PaymentMethodRef != null)
            {  
                if (SalesReceiptRet.PaymentMethodRef.FullName != null)
                {
                    receipt.PaymentMethodRef = (string)SalesReceiptRet.PaymentMethodRef.FullName.GetValue();
                }
            }
            //Get value of DueDate
            if (SalesReceiptRet.DueDate != null)
            {
                receipt.DueDate = (DateTime)SalesReceiptRet.DueDate.GetValue();
            }
            if (SalesReceiptRet.SalesRepRef != null)
            {   
                //Get value of FullName
                if (SalesReceiptRet.SalesRepRef.FullName != null)
                {
                     receipt.SalesRepRef = (string)SalesReceiptRet.SalesRepRef.FullName.GetValue();
                }
            }
            //Get value of ShipDate
            if (SalesReceiptRet.ShipDate != null)
            {
                receipt.Ship_Date= (DateTime)SalesReceiptRet.ShipDate.GetValue();
            }
            if (SalesReceiptRet.ShipMethodRef != null)
            {
                //Get value of FullName
                if (SalesReceiptRet.ShipMethodRef.FullName != null)
                {
                    receipt.Ship_Method = (string)SalesReceiptRet.ShipMethodRef.FullName.GetValue();
                }
            }
            
            //Get value of Subtotal
            if (SalesReceiptRet.Subtotal != null)
            {
              receipt.SubTotal = (double)SalesReceiptRet.Subtotal.GetValue();
            }
            if (SalesReceiptRet.ItemSalesTaxRef != null)
            {
                
                if (SalesReceiptRet.ItemSalesTaxRef.FullName != null)
                {
                    receipt.ItemSalesTaxRef = (string)SalesReceiptRet.ItemSalesTaxRef.FullName.GetValue();
                }
            }
            //Get value of SalesTaxPercentage
            if (SalesReceiptRet.SalesTaxPercentage != null)
            {
                double SalesTaxPercentage63 = (double)SalesReceiptRet.SalesTaxPercentage.GetValue();
            }
            //Get value of SalesTaxTotal
            if (SalesReceiptRet.SalesTaxTotal != null)
            {
                receipt.SalesTaxTotal = (double)SalesReceiptRet.SalesTaxTotal.GetValue();
            }
            //Get value of TotalAmount
            if (SalesReceiptRet.TotalAmount != null)
            {
                receipt.Total = (double)SalesReceiptRet.TotalAmount.GetValue();
            }
           
            
            //Get value of Memo
            if (SalesReceiptRet.Memo != null)
            {
                receipt.Memo = (string)SalesReceiptRet.Memo.GetValue();
            }
            if (SalesReceiptRet.CustomerMsgRef != null)
            {
               
                if (SalesReceiptRet.CustomerMsgRef.FullName != null)
                {
                    receipt.CustomerMessage = (string)SalesReceiptRet.CustomerMsgRef.FullName.GetValue();
                }
            }
            //Get value of IsToBePrinted
            if (SalesReceiptRet.IsToBePrinted != null)
            {
                receipt.isPrinted = (bool)SalesReceiptRet.IsToBePrinted.GetValue();
            }
            //Get value of IsToBeEmailed
            if (SalesReceiptRet.IsToBeEmailed != null)
            {
                receipt.IsEmail = (bool)SalesReceiptRet.IsToBeEmailed.GetValue();
            }
            if (SalesReceiptRet.CustomerSalesTaxCodeRef != null)
            {
                
                //Get value of FullName
                if (SalesReceiptRet.CustomerSalesTaxCodeRef.FullName != null)
                {
                    receipt.CustomerSalesTaxCodeRef = (string)SalesReceiptRet.CustomerSalesTaxCodeRef.FullName.GetValue();
                }
            }
            if (SalesReceiptRet.DepositToAccountRef != null)
            {                
                //Get value of FullName
                if (SalesReceiptRet.DepositToAccountRef.FullName != null)
                {
                    receipt.DepositToAccountRef = (string)SalesReceiptRet.DepositToAccountRef.FullName.GetValue();
                }
            }            
            //Get value of Other
            if (SalesReceiptRet.Other != null)
            {
                receipt.Other = (string)SalesReceiptRet.Other.GetValue();
            }           
            if (SalesReceiptRet.ORSalesReceiptLineRetList != null)
            {
                receipt.SalesItems = new List<SaleItems>();
                for (int i104 = 0; i104 < SalesReceiptRet.ORSalesReceiptLineRetList.Count; i104++)
                {
                    IORSalesReceiptLineRet ORSalesReceiptLineRet = SalesReceiptRet.ORSalesReceiptLineRetList.GetAt(i104);
                    Item = new SaleItems();
                    if (ORSalesReceiptLineRet.SalesReceiptLineRet != null)
                    {
                        //Gett value of TxnLineID
                        Item.TxnLineID = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.TxnLineID.GetValue();
                        if (ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef != null)
                        {
                            //Get value of FullName
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef.FullName != null)
                            {
                                Item.ItemRef = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.ItemRef.FullName.GetValue();
                            }
                        }
                        //Get value of Desc
                        if (ORSalesReceiptLineRet.SalesReceiptLineRet.Desc != null)
                        {
                            Item.Desc = (string)ORSalesReceiptLineRet.SalesReceiptLineRet.Desc.GetValue();
                        }
                        //Get value of Quantity
                        if (ORSalesReceiptLineRet.SalesReceiptLineRet.Quantity != null)
                        {
                            Item.Quantity = (double)ORSalesReceiptLineRet.SalesReceiptLineRet.Quantity.GetValue();
                        }
                        if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate != null)
                        {
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.Rate != null)
                            {
                                //Get value of Rate
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.Rate != null)
                                {
                                    Item.ORRate = (double)ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.Rate.GetValue();
                                }
                            }
                            if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.RatePercent != null)
                            {
                                //Get value of RatePercent
                                if (ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.RatePercent != null)
                                {
                                    Item.RatePercent = (double)ORSalesReceiptLineRet.SalesReceiptLineRet.ORRate.RatePercent.GetValue();
                                }
                            }
                        }
                        //Get value of Amount
                        if (ORSalesReceiptLineRet.SalesReceiptLineRet.Amount != null)
                        {
                            Item.Amount = (double)ORSalesReceiptLineRet.SalesReceiptLineRet.Amount.GetValue();
                        }
                      }
                    receipt.SalesItems.Add(Item);
                }

            }

            
            //Adding receipts 
                Receipts.Add(receipt);
        }

                
            }
        
        #endregion
    }

