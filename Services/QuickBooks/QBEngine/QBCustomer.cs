using System;
using System.Data ;
using QBFC10Lib;
using System.Collections.Generic;

namespace QBEngine
{
    public class QBCustomer :QBBase ,ICustomer
    {
        #region Fields

        IList<Customer> CustomerList;

        #endregion 


        #region Methods 

        public string createCustomer(Customer customer) {

            requestMsgSet.ClearRequests();
            ICustomerAdd CustomerAddRq = requestMsgSet.AppendCustomerAddRq();
            //if (!string.IsNullOrEmpty(customer.Parent))
              //  CustomerAddRq.ParentRef.FullName.SetValue(customer.Parent);
            CustomerAddRq.Name.SetValue(customer.Name);
            CustomerAddRq.MiddleName.SetValue(customer.MiddleName);
            CustomerAddRq.AccountNumber.SetValue(customer.AccountNumber);
            CustomerAddRq.OpenBalance.SetValue(customer.OpeningBalance);
            CustomerAddRq.OpenBalanceDate.SetValue(customer.OpeningDate);
            CustomerAddRq.IsActive.SetValue(customer.isActive);
            CustomerAddRq.BillAddress.Addr1.SetValue(customer.BillAddress1);
            CustomerAddRq.BillAddress.Addr2.SetValue(customer.BillAddress2);
            CustomerAddRq.BillAddress.Addr3.SetValue(customer.BillAddress3);
            CustomerAddRq.BillAddress.Addr4.SetValue(customer.BillAddress4);
            //CustomerAddRq.BillAddress.Addr5.SetValue(customer.BillAddress5);
            CustomerAddRq.BillAddress.City.SetValue(customer.BillCity);
            CustomerAddRq.BillAddress.State.SetValue(customer.BillState);
            CustomerAddRq.BillAddress.PostalCode.SetValue(customer.BillPostcode);
            CustomerAddRq.BillAddress.Country.SetValue(customer.BillCounty);                    
            CustomerAddRq.Phone.SetValue(customer.Phone);
            CustomerAddRq.Email.SetValue(customer.Email);
            CustomerAddRq.Fax.SetValue(customer.Fax);
            CustomerAddRq.Pager.SetValue(customer.Pager);
            CustomerAddRq.Contact.SetValue(customer.Contact);
            CustomerAddRq.AltContact.SetValue(customer.AltContact);
            CustomerAddRq.AltPhone.SetValue(customer.AltPhone);
            CustomerAddRq.CreditCardInfo.CreditCardAddress.SetValue (customer.CreditCardAddress);
            CustomerAddRq.CreditCardInfo.CreditCardNumber.SetValue(customer.CreditCardNumber);
            CustomerAddRq.CreditCardInfo.CreditCardPostalCode.SetValue(customer.CreditCardPostalCode);
            if(customer.ExpirationMonth >=1) 
            CustomerAddRq.CreditCardInfo.ExpirationMonth.SetValue(customer.ExpirationMonth);
            if (customer.ExpirationYear >=1)
            CustomerAddRq.CreditCardInfo.ExpirationYear.SetValue(customer.ExpirationYear);
            CustomerAddRq.CreditCardInfo.NameOnCard.SetValue(customer.NameOnCard);          
            CustomerAddRq.Salutation.SetValue(customer.Salutation);
            CustomerAddRq.ShipAddress.Addr1.SetValue(customer.ShipAddress1);
            CustomerAddRq.ShipAddress.Addr2.SetValue(customer.ShipAddress2);
            CustomerAddRq.ShipAddress.Addr3.SetValue(customer.ShipAddress3); 
            CustomerAddRq.ShipAddress.Addr4.SetValue(customer.ShipAddress4); 
         
            CustomerAddRq.ShipAddress.City.SetValue(customer.ShipCity);
            CustomerAddRq.ShipAddress.Country.SetValue(customer.ShipCountry);
            CustomerAddRq.Notes.SetValue(customer.Note);
            CustomerAddRq.JobDesc.SetValue(customer.JobDesc);
            
            
            //------------------------- Customer Fields Need to be added 
            if (customer.SalesRepRef_FullName != null) 
            CustomerAddRq.SalesRepRef.FullName.SetValue(customer.SalesRepRef_FullName) ;
        if (customer.ResaleNumber != null) 
            CustomerAddRq.ResaleNumber.SetValue(customer.ResaleNumber );         
           
            if (customer.TermsRef_FullName != null) 

            CustomerAddRq.TermsRef.FullName.SetValue(customer.TermsRef_FullName );
        if (customer.Preferd_PaymentMethod_FullName != null)
            CustomerAddRq.PreferredPaymentMethodRef.FullName.SetValue(customer.Preferd_PaymentMethod_FullName);
         
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);

            if (responseMsgSet.ResponseList.GetAt(0).StatusCode == 0)
            {
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
               
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType == ENResponseType.rtCustomerAddRs)
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            ICustomerRet CustomerRet = (ICustomerRet)response.Detail;
                            if(CustomerRet != null)
                                return CustomerRet.ListID.GetValue();

                        }

                    }
                return "-1";
            }
            else
            {
                throw new QBException(responseMsgSet.ResponseList.GetAt(0).StatusCode, "QBEngine :" + responseMsgSet.ResponseList.GetAt(0).StatusMessage,requestMsgSet.ToXMLString());
                return "-1";
            }


        }


        public string UpdateCustomer(Customer customer) {

            return "";
        }

		public IList<Customer> GetAllCustomer(){
		
		
			            requestMsgSet.ClearRequests();
			            ICustomerQuery CustomerQueryRq = requestMsgSet.AppendCustomerQueryRq();

                        CustomerQueryRq.OwnerIDList.Add("0");
			          //  CustomerQueryRq.ORCustomerListQuery.CustomerListFilter.FromModifiedDate.SetValue(createdFrom, false);
			           // CustomerQueryRq.ORCustomerListQuery.CustomerListFilter.ToModifiedDate.SetValue(createdTo, false);
			            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
			            return WalkCustomerQuery(responseMsgSet);
			
		}

        private	IList<Customer> WalkCustomerQuery(IMsgSetResponse responseMsgSet)
        {
		            if (responseMsgSet == null) return null;
		
		            IResponseList responseList = responseMsgSet.ResponseList;
		            if (responseList == null) return null;
		
					int count = responseList.Count;
					
		            //if we sent only one request, there is only one response, we'll walk the list for this sample
		            for (int i = 0; i < count; i++)
		            {
		                IResponse response = responseList.GetAt(i);
		
		                if (response.StatusCode == 0)
		                {
		
		                    if (response.Detail != null)
		                    {
		                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
		                        if (responseType == ENResponseType.rtCustomerQueryRs)
		                        {
		                            ICustomerRetList CustomerRet = (ICustomerRetList)response.Detail;
		                            return WalkCustomers(CustomerRet);
		
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

        private IList<Customer> WalkCustomers(ICustomerRetList Customer){
			
			if (Customer == null) return null;
            CustomerList = new List<Customer>();
            Customer cust;		
			for (int a = 0; a < Customer.Count; a++)
			{
				ICustomerRet CustomerRet = Customer.GetAt(a);
                
                cust = new Customer();
                cust.ListID = CustomerRet.ListID.GetValue();
				cust.Name = (string)CustomerRet.Name.GetValue();
                if(CustomerRet.Phone != null)
                cust.Phone = CustomerRet.Phone.GetValue();
                if(CustomerRet.Email != null)
                cust.Email = CustomerRet.Email.GetValue();

				cust.FullName = (string)CustomerRet.FullName.GetValue();
                cust.EditSequence = CustomerRet.EditSequence.GetValue();
				cust.ListID = CustomerRet.ListID.GetValue().ToString();
                cust.TotalBalance = CustomerRet.TotalBalance.GetValue();
                if(CustomerRet.FirstName != null)
                cust.FirstName = CustomerRet.FirstName.GetValue();
                if(CustomerRet.LastName != null)
                cust.LastName = CustomerRet.LastName.GetValue();
                if (CustomerRet.BillAddress != null)
                {
                    if(CustomerRet.BillAddress.Addr1 != null) 

                    cust.BillAddress1 = CustomerRet.BillAddress.Addr1.GetValue();
                    if (CustomerRet.BillAddress.Addr2 != null) 
                    cust.BillAddress2 = CustomerRet.BillAddress.Addr2.GetValue();
                    if (CustomerRet.BillAddress.City != null) 
                    cust.BillCity = CustomerRet.BillAddress.City.GetValue();
                    if (CustomerRet.BillAddress.Country != null) 
                    cust.BillCounty = CustomerRet.BillAddress.Country.GetValue();
                    if (CustomerRet.BillAddress.PostalCode != null) 
                    cust.BillPostcode = CustomerRet.BillAddress.PostalCode.GetValue();
                }

                if (CustomerRet.ShipAddress != null)
                {
                    if (CustomerRet.ShipAddress.Addr1 != null)
                    cust.ShipAddress1 = CustomerRet.ShipAddress.Addr1.GetValue();
                    if (CustomerRet.ShipAddress.Addr2 != null)
                    cust.ShipAddress2 = CustomerRet.ShipAddress.Addr2.GetValue();
                    if (CustomerRet.ShipAddress.City != null)
                    cust.ShipCity = CustomerRet.ShipAddress.City.GetValue();
                    if (CustomerRet.ShipAddress.Country != null)
                    cust.ShipCountry = CustomerRet.ShipAddress.Country.GetValue();
                    if (CustomerRet.ShipAddress.PostalCode != null)
                    cust.ShipPostalCode = CustomerRet.ShipAddress.PostalCode.GetValue();
                }

                if (CustomerRet.DataExtRetList != null) {

                   cust.VaultID = CustomerRet.DataExtRetList.GetAt(0).DataExtValue.GetValue();
                }
                if(CustomerRet.Notes != null) 
                cust.Note = CustomerRet.Notes.GetValue();

                CustomerList.Add(cust);
			}
			return CustomerList;
        }

        #endregion
    }
}

