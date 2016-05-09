using System;
using System.Data;
using System.Linq;
using System.Transactions;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Business.Customer
{
    public class CustomerService : AbstractBusinessService
    {
        public OperationResult<bool> SaveCustomer(int franchiseId, tbl_Customer customer, tbl_Locations location,
                                                           tbl_Contacts primaryContact, tbl_Contacts secondaryContact,
                                                           bool isBillTo)
        {
            const int findByNameMaxLength = 150;

            try
            {
                using (var tScope = new TransactionScope())
                {
                    using (var dbContext = new EightHundredEntities(UserKey))
                    {
                        if (customer.CustomerID != 0)
                        {
                            var existingCust = dbContext.tbl_Customer.Single(c => c.CustomerID == customer.CustomerID);
                            dbContext.ApplyCurrentValues(existingCust.EntityKey.EntitySetName, customer);
                        }
                        else
                        {
                            dbContext.tbl_Customer.AddObject(customer);
                            var info = tbl_Customer_Info.Createtbl_Customer_Info(0, franchiseId, customer.CustomerID, 1, 1, 2, 1, 0);
                            customer.tbl_Customer_Info.Add(info);
                        }

                        customer.FindByName = !string.IsNullOrWhiteSpace(customer.CustomerName)
                                                  ? !string.IsNullOrWhiteSpace(customer.CompanyName)
                                                        ? (customer.CustomerName + " " + customer.CompanyName).Substring
                                                              (0)
                                                        : customer.CustomerName
                                                  : !string.IsNullOrWhiteSpace(customer.CompanyName)
                                                        ? customer.CompanyName
                                                        : string.Empty;
                        customer.FindByName = customer.FindByName.Substring(0,
                                                                            Math.Min(findByNameMaxLength,
                                                                                     customer.FindByName.Length));

                        dbContext.SaveChanges();

                        tbl_Locations otherLocation;

                        if (location.LocationID != 0)
                        {
                            var existingLoc = dbContext.tbl_Locations.Single(l => l.LocationID == location.LocationID);
                            dbContext.ApplyCurrentValues(existingLoc.EntityKey.EntitySetName, location);

                            otherLocation =
                                dbContext.tbl_Locations.SingleOrDefault(
                                    ol =>
                                    ol.LocationID != location.LocationID &&
                                    (ol.ActvieCustomerID == customer.CustomerID ||
                                     ol.BilltoCustomerID == customer.CustomerID));
                        }
                        else
                        {
                            if (isBillTo)
                            {
                                location.BilltoCustomerID = customer.CustomerID;
                                otherLocation =
                                    dbContext.tbl_Locations.SingleOrDefault(
                                        ol => ol.ActvieCustomerID == customer.CustomerID);
                            }

                            else
                            {
                                location.ActvieCustomerID = customer.CustomerID;
                                otherLocation =
                                    dbContext.tbl_Locations.SingleOrDefault(
                                        ol => ol.BilltoCustomerID == customer.CustomerID);
                            }

                            dbContext.tbl_Locations.AddObject(location);
                            dbContext.SaveChanges();
                        }

                        if (primaryContact.ContactID != 0)
                        {
                            var existingContact =
                                dbContext.tbl_Contacts.Single(c => c.ContactID == primaryContact.ContactID);
                            dbContext.ApplyCurrentValues(existingContact.EntityKey.EntitySetName, primaryContact);
                        }
                        else
                        {
                            primaryContact.CustomerID = customer.CustomerID;
                            primaryContact.LocationID = location.LocationID;
                            dbContext.tbl_Contacts.AddObject(primaryContact);
                        }

                        if (secondaryContact != null)
                        {
                            if (secondaryContact.ContactID != 0)
                            {
                                var existingContact =
                                    dbContext.tbl_Contacts.Single(c => c.ContactID == secondaryContact.ContactID);
                                dbContext.ApplyCurrentValues(existingContact.EntityKey.EntitySetName, secondaryContact);
                            }
                            else
                            {
                                secondaryContact.CustomerID = customer.CustomerID;
                                secondaryContact.LocationID = location.LocationID;
                                dbContext.tbl_Contacts.AddObject(secondaryContact);
                            }
                        }

                        dbContext.SaveChanges();

                        var duplicateContacts = false;

                        if (otherLocation == null)
                        {
                            dbContext.SaveChanges();

                            otherLocation = dbContext.tbl_Locations.Single(l => l.LocationID == location.LocationID);
                            dbContext.Detach(otherLocation);

                            if (otherLocation.BilltoCustomerID.HasValue)
                            {
                                otherLocation.ActvieCustomerID = otherLocation.BilltoCustomerID;
                                otherLocation.BilltoCustomerID = null;
                            }
                            else
                            {
                                otherLocation.BilltoCustomerID = otherLocation.ActvieCustomerID;
                                otherLocation.ActvieCustomerID = null;
                            }

                            dbContext.tbl_Locations.AddObject(otherLocation);
                            duplicateContacts = true;
                        }

                        if (duplicateContacts)
                        {
                            dbContext.SaveChanges();

                            if (primaryContact.EntityState != EntityState.Detached)
                            {
                                dbContext.Detach(primaryContact);

                                primaryContact.LocationID = otherLocation.LocationID;
                                primaryContact.ContactID = 0;
                                dbContext.tbl_Contacts.AddObject(primaryContact);
                            }

                            if (secondaryContact != null && secondaryContact.EntityState != EntityState.Detached)
                            {
                                dbContext.Detach(secondaryContact);
                                secondaryContact.LocationID = otherLocation.LocationID;
                                secondaryContact.ContactID = 0;
                                dbContext.tbl_Contacts.AddObject(secondaryContact);
                            }
                        }

                        dbContext.SaveChanges();
                        tScope.Complete();
                    }
                }

                return new OperationResult<bool> { Success = true, ResultData = true };
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                return new OperationResult<bool> { Success = false, Message = ex.Message };
            }
        }
    }
}
