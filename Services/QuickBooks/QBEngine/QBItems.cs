using System;
using System.Collections.Generic;
using System.Text;
using Common;
using QBFC10Lib;

namespace QBEngine
{
    public class QBItems:QBBase
    {
        List<Items> ItemList = null;
        public List<Items> GetQBItems() {
           try
            {
             RequestNonInventory();
             RequestInventory();
             RequestService();
            }
            catch (Exception ex)
            {
                //ex.Message;
                return null;
            }

            return ItemList;
        }

        void RequestService() {
            requestMsgSet.ClearRequests();
            IItemServiceQuery service = requestMsgSet.AppendItemServiceQueryRq();
            responseMsgSet = sessionManager.DoRequests(requestMsgSet);
            IResponse response = responseMsgSet.ResponseList.GetAt(0);
            if (response.StatusCode >= 0)
            {
                //the request-specific response is in the details, make sure we have some
                if (response.Detail != null)
                {
                    //make sure the response is the type we're expecting
                    ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                    if (responseType == ENResponseType.rtItemServiceQueryRs)
                    {
                        //upcast to more specific type here, this is safe because we checked with response.Type check above
                        IItemServiceRetList ItemServiceRetList = (IItemServiceRetList)response.Detail;
                        int count = ItemServiceRetList.Count;
                        if (count > 0)
                            ItemList = new List<Items>();
                        for (int a = 0; a < count; a++)
                        {
                            ItemList.Add(WalkServiceItem(ItemServiceRetList.GetAt(a)));
                        }
                    }
                }
            }
            else
            {

              //  throw new QBException(response.StatusCode, response.StatusMessage.ToString(), requestMsgSet.ToXMLString());
            }
        }
        Items WalkServiceItem(IItemServiceRet ItemRet) { 
        
            Items item = new Items();
           item.ItemType = ItemRet.Type.GetAsString();
           item.Name = ItemRet.FullName.GetValue();
           item.EditSequence = ItemRet.EditSequence.GetValue();
           item.TxnID = ItemRet.ListID.GetValue();
           if (ItemRet.ORSalesPurchase.SalesAndPurchase != null)
           {
               if (ItemRet.ORSalesPurchase.SalesAndPurchase.SalesPrice != null)
                   item.Rate = ItemRet.ORSalesPurchase.SalesAndPurchase.SalesPrice.GetValue();
               if (ItemRet.ORSalesPurchase.SalesAndPurchase.SalesDesc != null)
                   item.Description = ItemRet.ORSalesPurchase.SalesAndPurchase.SalesDesc.GetValue();
           }
           if (ItemRet.ORSalesPurchase.SalesOrPurchase != null)
           {
               if (ItemRet.ORSalesPurchase.SalesOrPurchase.ORPrice.Price != null)
                   item.Rate = ItemRet.ORSalesPurchase.SalesOrPurchase.ORPrice.Price.GetValue();
               if (ItemRet.ORSalesPurchase.SalesOrPurchase.Desc != null)
                   item.Description = ItemRet.ORSalesPurchase.SalesOrPurchase.Desc.GetValue();
           }
           //if(ItemRet. != null)
           //item.Description = ItemRet.SalesDesc.GetValue();
           if (ItemRet.IsActive != null)
               if (ItemRet.IsActive.GetValue())
                   return item;
               else
                   return null;
           else
               return null;
          
        }
       void RequestNonInventory() {

           requestMsgSet.ClearRequests();
            IItemNonInventoryQuery Inventory = requestMsgSet.AppendItemNonInventoryQueryRq();
                //Inventory.IsActive.SetValue(true);
                responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponseList responseList = responseMsgSet.ResponseList;
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                if (response.StatusCode >= 0)
                {
                    //the request-specific response is in the details, make sure we have some
                    if (response.Detail != null)
                    {
                        //make sure the response is the type we're expecting
                        ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                        if (responseType == ENResponseType.rtItemNonInventoryQueryRs)
                        {
                            //upcast to more specific type here, this is safe because we checked with response.Type check above
                            IItemNonInventoryRetList ItemNonInventoryRetList = (IItemNonInventoryRetList)response.Detail;
                            int count = ItemNonInventoryRetList.Count;
                            if (count > 0)
                                ItemList = new List<Items>();
                            for (int a = 0; a < count; a++)
                            {
                                ItemList.Add(WalkNonInventoryItem(ItemNonInventoryRetList.GetAt(a)));
                            }
                        }
                    }
                }
                else {

                    throw new QBException(response.StatusCode, response.StatusMessage.ToString(),requestMsgSet.ToXMLString());
                }
        }

       void RequestInventory()
       {

           requestMsgSet.ClearRequests();
           IItemInventoryQuery Inventory = requestMsgSet.AppendItemInventoryQueryRq();           
           responseMsgSet = sessionManager.DoRequests(requestMsgSet);
           IResponseList responseList = responseMsgSet.ResponseList;
           IResponse response = responseMsgSet.ResponseList.GetAt(0);
           if (response.StatusCode >= 0)
           {
               //the request-specific response is in the details, make sure we have some
               if (response.Detail != null)
               {
                   //make sure the response is the type we're expecting
                   ENResponseType responseType = (ENResponseType)response.Type.GetValue();
                   if (responseType == ENResponseType.rtItemInventoryQueryRs)
                   {
                       //upcast to more specific type here, this is safe because we checked with response.Type check above
                       IItemInventoryRetList ItemInventoryRetList = (IItemInventoryRetList)response.Detail;
                       int count = ItemInventoryRetList.Count;
                       if (count > 0 && ItemList == null)
                           ItemList = new List<Items>();
                       for (int a = 0; a < count; a++)
                       {
                           ItemList.Add(WalkInventoryItem(ItemInventoryRetList.GetAt(a)));
                       }
                   }
               }
           }
           else
           {

               throw new QBException(response.StatusCode, response.StatusMessage.ToString(),requestMsgSet.ToXMLString());
           }
       }


       Items WalkInventoryItem(IItemInventoryRet ItemRet)
       {

           if (ItemRet == null) return null;
           Items item = new Items();
           item.ItemType = ItemRet.Type.GetAsString();
           item.Name = ItemRet.FullName.GetValue();
           item.EditSequence = ItemRet.EditSequence.GetValue();
           item.TxnID = ItemRet.ListID.GetValue();
           if(ItemRet.SalesPrice != null) 
           item.Rate = ItemRet.SalesPrice.GetValue();
           if(ItemRet.SalesDesc != null)
           item.Description = ItemRet.SalesDesc.GetValue();
           if (ItemRet.IsActive != null)
               if (ItemRet.IsActive.GetValue())
                   return item;
               else
                   return null;
           else
               return null;
       }


       Items WalkNonInventoryItem(IItemNonInventoryRet ItemRet)
       {
           if (ItemRet == null) return null;
           Items item = new Items();
           item.ItemType = ItemRet.Type.GetAsString();
           item.Name = ItemRet.FullName.GetValue();
           item.EditSequence = ItemRet.EditSequence.GetValue();
           item.TxnID = ItemRet.ListID.GetValue();
           if (ItemRet.ORSalesPurchase.SalesAndPurchase != null)
           {
               if (ItemRet.ORSalesPurchase.SalesAndPurchase.SalesPrice != null)
               item.Rate = ItemRet.ORSalesPurchase.SalesAndPurchase.SalesPrice.GetValue();
               if(ItemRet.ORSalesPurchase.SalesAndPurchase.SalesDesc != null)
               item.Description = ItemRet.ORSalesPurchase.SalesAndPurchase.SalesDesc.GetValue();
           }
           if (ItemRet.ORSalesPurchase.SalesOrPurchase != null)
           {
               if (ItemRet.ORSalesPurchase.SalesOrPurchase.ORPrice.Price != null)
                   item.Rate = ItemRet.ORSalesPurchase.SalesOrPurchase.ORPrice.Price.GetValue();
               if (ItemRet.ORSalesPurchase.SalesOrPurchase.Desc != null)
                   item.Description = ItemRet.ORSalesPurchase.SalesOrPurchase.Desc.GetValue();
           }
           if (ItemRet.IsActive.GetValue())
               return item;
           else
               return null;
       }

    }
}
