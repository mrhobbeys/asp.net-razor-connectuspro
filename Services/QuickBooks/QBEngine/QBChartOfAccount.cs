using System;
using System.Collections.Generic;
using System.Text;
using QBFC10Lib;
namespace QBEngine
{
    public class QBChartOfAccount :QBBase
    {
        IList<ChartOfAccount> ChartOfAccountList ;

        public IList<ChartOfAccount> GetAllChartOfAccount() {

            requestMsgSet.ClearRequests();
            IAccountQuery AccountQueryRq = requestMsgSet.AppendAccountQueryRq();
            //Set field value for ActiveStatus
            AccountQueryRq.ORAccountListQuery.AccountListFilter.ActiveStatus.SetValue(ENActiveStatus.asActiveOnly);
            AccountQueryRq.ORAccountListQuery.AccountListFilter.AccountTypeList.Add(ENAccountType.atBank);            
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            return WalkChartOfAccountQuery(responseMsgSet);
            
        }

        private IList<ChartOfAccount> WalkChartOfAccountQuery(IMsgSetResponse responseMsgSet) {

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
                        if (responseType == ENResponseType.rtAccountQueryRs)
                        {
                            IAccountRetList CustomerRet = (IAccountRetList)response.Detail;
                            return WalkChartOfAccount(CustomerRet);

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

        private IList<ChartOfAccount> WalkChartOfAccount(IAccountRetList  Account) {

            if (Account == null) return null;
            ChartOfAccountList = new List<ChartOfAccount>();
            ChartOfAccount BankAccount;
			for (int a = 0; a < Account.Count; a++)
			{
				IAccountRet AccountRet = Account.GetAt(a);
                BankAccount = new ChartOfAccount();
				//BankAccount.Name = (string)AccountRet.Name.GetValue();
				BankAccount.FullName = (string)AccountRet.FullName.GetValue();
				BankAccount.ListID = AccountRet.ListID.GetValue().ToString();
                ChartOfAccountList.Add(BankAccount);
			}
			return ChartOfAccountList;

        
        }


    }
}
