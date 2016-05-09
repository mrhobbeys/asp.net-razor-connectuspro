using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;
using SecurityGuard.Services;
using SecurityGuard.Core.Attributes;
using routeHelpers = SecurityGuard.Core.RouteHelpers;
using SecurityGuard.Interfaces;
using SecurityGuard.ViewModels;
using SiteBlue.Controllers;
using SiteBlue.Areas.SecurityGuard.Models;
using System.Collections;
using System.Data.Objects.SqlClient;
using SiteBlue.Data;

namespace SiteBlue.Areas.SecurityGuard.Controllers
{
    [Authorize(Roles = "SecurityGuard")]
    public partial class MembershipController : SecurityGuardBaseController
    {

        #region ctors

        private readonly IMembershipService membershipService;
        private readonly IRoleService roleService;
        private MembershipConnection memberShipContext = new MembershipConnection();
        private readonly MembershipEntities membershipEntities = new MembershipEntities();

        public MembershipController()
        {
            roleService = new RoleService(Roles.Provider);
            membershipService = new MembershipService(Membership.Provider);
        }

        #endregion

        #region Index Method and AutoComplete
        public virtual ActionResult Index(string searchterm, string filterby)
        {
            var viewModel = new ManageUsersViewModel { Users = null };

            if (!string.IsNullOrEmpty(searchterm))
            {
                string query = searchterm + "%";
                if (filterby == "email")
                {
                    viewModel.Users = membershipService.FindUsersByEmail(query);
                }
                else if (filterby == "username")
                {
                    viewModel.Users = membershipService.FindUsersByName(query);
                }
            }

            return View(viewModel);
        }

        public string AutoCompleteForUsers(string q, string filterby)        
        {
            var viewModel = new ManageUsersViewModel { Users = null };
            if (!string.IsNullOrEmpty(q))
            {
                string query = q + "%";
                viewModel.Users = membershipService.FindUsersByName(query);
                switch (filterby)
                {
                    case "email":
                        viewModel.Users = membershipService.FindUsersByEmail(query); 
                        break;
                    case "username":
                        viewModel.Users = membershipService.FindUsersByName(query);
                        break;
                }
            }
            var str = viewModel.Users.Cast<MembershipUser>().Select(item => item.UserName).Aggregate("", (current, a) => current + "\n" + a);
            return str;
        }
        #endregion

        #region Create User Methods

        public virtual ActionResult CreateUser()
        {
            var model = new RegisterViewModel
            {
                RequireSecretQuestionAndAnswer = membershipService.RequiresQuestionAndAnswer,
                Franchises = new SelectList(memberShipContext.MembershipFranchise, "FranchiseID", "FranchiseNUmber")
            };
            return View(model);
        }
        
        /// <summary>
        /// This method redirects to the GrantRolesToUser method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult CreateUser(RegisterViewModel model)
        {
            MembershipCreateStatus status;
            MembershipUser user = membershipService.CreateUser(model.UserName, model.Password, model.Email, model.SecretQuestion, model.SecretAnswer, model.Approve, out status);
            var userFranchise = new UserFranchise_guard
            {
                FranchiseID = model.FranchiseID,
                UserId =(Guid)user.ProviderUserKey,
            };

            memberShipContext.UserFranchise.Add(userFranchise);
            memberShipContext.SaveChanges();

            return routeHelpers.Actions.GrantRolesToUser(user.UserName);
        }

        /// <summary>
        /// This method gets all the users according to role.
        /// </summary>
        [HttpPost]
        public JsonResult GetUsersInRoles()
        {
            var applicationId = GetApplicationId();
             var type = Request["FranchiseType"];
             var name = Request["UserName"];
             List<int> listFranchiseID = null;
            
             var listUser = new List<UserDetailViewModel>();
             var listRole = new List<UserRoleViewModel>();
            //var listRole = new List<string>();

             string result = "";

             try
             {
                 //check for Corporate Frachise 
                 if (type == "0")
                 {
                     listFranchiseID = membershipEntities.ARCHIVE_tbl_Franchise.Where(franchise => franchise.FranchiseNUmber == "CORPORATE").Select(franchise => franchise.FranchiseID).ToList();
                 }

                 //check for All Company Codes
                 if (type == "1")
                 {
                     listFranchiseID = membershipEntities.ARCHIVE_tbl_Franchise.Where(franchise => franchise.FranchiseNUmber != "CORPORATE").Select(franchise => franchise.FranchiseID).ToList();
                 }

                 //Check for Display All
                 if (type == "2")
                 {
                     listFranchiseID = membershipEntities.ARCHIVE_tbl_Franchise.Select(franchise => franchise.FranchiseID).ToList();
                 }

                 //get users based on FranchiseId...
                 foreach (var id in listFranchiseID)
                 {
                     var users = (from user in membershipEntities.aspnet_Users
                                 join membership in membershipEntities.aspnet_Membership on user.UserId equals membership.UserId
                                 join franchise in membershipEntities.UserFranchise on user.UserId equals franchise.UserId
                                  where (user.UserName.StartsWith(name) && franchise.FranchiseID == id)
                                 select new UserDetailViewModel { UserID = user.UserId, UserName = user.UserName, Email = membership.Email }).Distinct();

                     var count1 = users.Count();

                     var users1 = (from user in membershipEntities.aspnet_Users
                                  join membership in membershipEntities.aspnet_Membership on user.UserId equals membership.UserId
                                  join franchise in membershipEntities.UserFranchise on user.UserId equals franchise.UserId
                                  where (franchise.FranchiseID == id)
                                  select new UserDetailViewModel { UserID = user.UserId, UserName = user.UserName, Email = membership.Email }).Distinct();

                     var count11 = users1.Count();

                     var users2 = (from user in membershipEntities.aspnet_Users
                                  join membership in membershipEntities.aspnet_Membership on user.UserId equals membership.UserId
                                  join franchise in membershipEntities.UserFranchise on user.UserId equals franchise.UserId
                                   where (user.UserName.StartsWith(name) && franchise.FranchiseID == id && user.ApplicationId == applicationId && membership.ApplicationId == applicationId)
                                  select new UserDetailViewModel { UserID = user.UserId, UserName = user.UserName, Email = membership.Email }).Distinct();

                     var count131 = users2.Count();

                     foreach (var userDetail in users)
                     {
                         //check for user existance in listUsers...
                         int index = listUser.FindIndex(item => item.UserID == userDetail.UserID);
                         if (index < 0)
                             listUser.Add(userDetail);
                     }
                     
                 }

                 //get user roles...
                 if (applicationId != Guid.Empty)
                 {
                     var roles = (from role in membershipEntities.aspnet_Roles.Distinct()
                                  where role.ApplicationId == applicationId
                                  select new UserRoleViewModel {RoleID = role.RoleId, RoleName = role.RoleName});

                     listRole.AddRange(roles);
                 }
                 result = GenerateUserDetailString(listUser, listRole);

                 return Json(new { UserDetails = result,UserCount=listUser.Count() });

             }
             catch (Exception)
             {
                 return Json(new { UserDetails = result, UserCount = 0});
             }
         }

      
        //Generate user detail string...
        public string GenerateUserDetailString(List<UserDetailViewModel> users ,List<UserRoleViewModel> roles)
        {
            string tableUser = "";
            //var count = 0;

            try
            {
                if (roles.Count > 0 && users.Count>0)
                {
                    tableUser = "<table id='tblUser' cellpadding='0' cellspacing='0' width='100%' class='sortable'><thead><tr><th class='text_center'>Name</th><th class='text_center'>Email</th>"; 
                                      
                     foreach (var role in roles)
                     {
                         tableUser = tableUser + "<th class='text_center'>" + role.RoleName + "</th>";
                     }

                    tableUser = tableUser + "</thead></tr><tbody>";           
                    
                    foreach (var user in users)
                    {
                        tableUser = tableUser + "<tr>" + "<td class='text_center'><a href='/SecurityGuard/Membership/Update/" + user.UserName + "'target='_blank'>" + user.UserName + "</a></td>" + "<td class='text_center'>" + user.Email + "</td>";

                        var listRolesToUser = roleService.GetRolesForUser(user.UserName).ToList();                                               
                        
                        foreach (var role in roles)
                        {
                            if(listRolesToUser.Contains(role.RoleName))
                                tableUser = tableUser + "<td class='text_center'><input type='checkbox' id='" + user.UserName + "_" + role.RoleName + "' checked='checked'/></td>";
                            else
                                tableUser = tableUser + "<td class='text_center'><input type='checkbox' id='" + user.UserName + "_" + role.RoleName + "'/></td>";
                            
                        }
                    }
                    tableUser = tableUser + "</tr></tbody></table>";

                }
                return tableUser;
            }
            catch(Exception)
            {
                return tableUser;
            }
        }

        //Save User Chnages...
        [HttpPost]       
        public ActionResult SaveUserChanges()
        {
            var response = new JsonResponse();
            response.Message="";

            var checkboxstring = Request["cbString"];
            string[] arrUserNameRoleName;
            string[] arrUserName=null;
            string[] arrRoleName=null;
            string[] arrUserStatus = null;
            string[] arrItem;
            var count=0;
            
            try
            {
                if(checkboxstring != "")
                {
                    checkboxstring = checkboxstring.Substring(0, checkboxstring.Length - 1);
                    arrUserNameRoleName = checkboxstring.Split('#');
                    
                    arrUserName = new string[arrUserNameRoleName.Count()];
                    arrRoleName = new string[arrUserNameRoleName.Count()];
                    arrUserStatus = new string[arrUserNameRoleName.Count()];

                    foreach (string item in arrUserNameRoleName)
                    {
                        arrItem = item.Split('_');
                        
                        arrUserName[count]=arrItem[0];
                        arrRoleName[count]=arrItem[1];
                        arrUserStatus[count] = arrItem[2];
                        count++;                                                                                        
                    }
                    
                    //check user and theire roles...                         
                    for(int i=0;i<arrUserName.Count();i++)
                    {
                        if(arrUserName[i]=="krishna")
                        {
                            //check,is user already in this role...
                            if(roleService.IsUserInRole(arrUserName[i],arrRoleName[i]))
                            {
                                //if user already exists in this role,then remove this user role...
                                roleService.RemoveUserFromRole(arrUserName[i],arrRoleName[i]);
                            }

                            //add role for user,if particular checkbox is checked...
                            if (arrUserStatus[i] == "checked")
                            {
                                roleService.AddUserToRole(arrUserName[i], arrRoleName[i]);
                            }
                        }
                    }

                    response.Message="Save Changes Successfully";
                    return Json(response);
                }
                
                return Json(response);
            }
            catch (Exception)
            {
                return Json(response);
            }
        }

        //Get the Current Application Id based on the Membership Application Name...
        public Guid GetApplicationId()
        {
            var appliationId = System.Guid.Empty;

            try
            {
                appliationId =membershipEntities.aspnet_Applications.Where(app => app.ApplicationName == membershipService.ApplicationName).Select(app => app.ApplicationId).FirstOrDefault();
                return appliationId;
            }
            catch (Exception)
            {
                return appliationId;
            }
        }
       
        /// <summary>
        /// An Ajax method to check if a username is unique.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckForUniqueUser(string userName)
        {
            MembershipUser user = membershipService.GetUser(userName);
            var response = new JsonResponse { Exists = user != null };

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Delete User Methods

        [HttpPost]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "DeleteUser")]
        public ActionResult DeleteUser(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new ArgumentNullException("userName");
            }

            try
            {
                //get userid from aspnet_Users by username...
                var userID =membershipEntities.aspnet_Users.Where(user => user.UserName == UserName).Select(user => user.UserId).FirstOrDefault();

                //get user franchise record based on the userID...
                var userFranchise = membershipEntities.UserFranchise.Where(uf => uf.UserId == userID).ToList();

                foreach (var franchise in userFranchise)
                {
                    membershipEntities.DeleteObject(franchise);
                }

                membershipEntities.SaveChanges();

                membershipService.DeleteUser(UserName);
                              

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "There was an error deleting this user. - " + ex.Message;
            }

            return RedirectToAction("Update", new { userName = UserName });
        }

        #endregion

        #region View User Details Methods

        [HttpGet]
        public ActionResult Update(string userName)
        {
            MembershipUser user = membershipService.GetUser(userName);

            var viewModel = new UserViewModel
            {
                User = user,
                RequiresSecretQuestionAndAnswer = membershipService.RequiresQuestionAndAnswer,
                Roles = roleService.GetRolesForUser(userName),
                GrantedCompanyCode = (from a in memberShipContext.MembershipFranchise
                                      where (from o in memberShipContext.UserFranchise
                                              where o.UserId == (Guid)user.ProviderUserKey
                                              select o.FranchiseID).Contains(a.FranchiseID)
                                      select a.FranchiseNumber).Distinct().ToArray()
            };

            return View(viewModel);
        }

        [HttpPost]
        //[ActionName("Update")]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UpdateUser")]
        public ActionResult UpdateUser(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new ArgumentNullException("userName");
            }

            MembershipUser user = membershipService.GetUser(UserName);

            try
            {
                user.Comment = Request["User.Comment"];
                user.Email = Request["User.Email"];

                membershipService.UpdateUser(user);
                TempData["SuccessMessage"] = "The user was updated successfully!";

            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "There was an error updating this user.";
            }

            return RedirectToAction("Update", new { userName = user.UserName });
        }
        
        #region Ajax methods for Updating the user

        [HttpPost]
        public ActionResult Unlock(string userName)
        {
            var response = new JsonResponse();

            MembershipUser user = membershipService.GetUser(userName);

            try
            {
                user.UnlockUser();
                response.Success = true;                
                response.Message = "User unlocked successfully!";
                response.Locked = false;
                response.LockedStatus = (response.Locked) ? "Locked" : "Unlocked";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "User unlocked failed.";
            }

            return Json(response);
        }

        [HttpPost]
        public ActionResult ApproveDeny(string userName)
        {
            var response = new JsonResponse();

            MembershipUser user = membershipService.GetUser(userName);

            try
            {
                user.IsApproved = !user.IsApproved;
                membershipService.UpdateUser(user);

                string approvedMsg = (user.IsApproved) ? "Approved" : "Denied";

                response.Success = true;
                response.Message = "User " + approvedMsg + " successfully!";
                response.Approved = user.IsApproved;
                response.ApprovedStatus = (user.IsApproved) ? "Approved" : "Not approved";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "User unlocked failed.";
            }

            return Json(response);
        }

        #endregion

        #endregion

        #region Cancel User Methods

        [HttpPost]
        [MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UserCancel")]
        public ActionResult Cancel()
        {
            return RedirectToAction("Index");
        }

        #endregion

        #region Grant Users with Roles Methods

        /// <summary>
        /// Return two lists:
        ///   1)  a list of Roles not granted to the user
        ///   2)  a list of Roles granted to the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public virtual ActionResult GrantRolesToUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index");
            }

            var model = new GrantRolesToUserViewModel
            {
                UserName = username,
                AvailableRoles =
                    (string.IsNullOrEmpty(username)
                         ? new SelectList(roleService.GetAllRoles())
                         : new SelectList(roleService.AvailableRolesForUser(username))),
                GrantedRoles =
                    (string.IsNullOrEmpty(username)
                         ? new SelectList(new string[] { })
                         : new SelectList(roleService.GetRolesForUser(username)))
            };

            return View(model);
        }



        /// <summary>
        /// Grant the selected roles to the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult GrantRolesToUser(string userName, string roles)
        {
            var response = new JsonResponse();

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            string[] roleNames = roles.Substring(0, roles.Length - 1).Split(',');

            if (roleNames.Length == 0)
            {
                response.Success = false;
                response.Message = "No roles have been granted to the user.";
                return Json(response);
            }

            try
            {
                roleService.AddUserToRoles(userName, roleNames);

                response.Success = true;
                response.Message = "The Role(s) has been GRANTED successfully to " + userName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem adding the user to the roles.";
            }

            return Json(response);
        }

        /// <summary>
        /// Revoke the selected roles for the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RevokeRolesForUser(string userName, string roles)
        {
            var response = new JsonResponse();

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            if (string.IsNullOrEmpty(roles))
            {
                response.Success = false;
                response.Message = "Roles is missing";
                return Json(response);
            }

            string[] roleNames = roles.Substring(0, roles.Length - 1).Split(',');

            if (roleNames.Length == 0)
            {
                response.Success = false;
                response.Message = "No roles are selected to be revoked.";
                return Json(response);
            }

            try
            {
                roleService.RemoveUserFromRoles(userName, roleNames);

                response.Success = true;
                response.Message = "The Role(s) has been REVOKED successfully for " + userName;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "There was a problem revoking roles for the user.";
            }

            return Json(response);
        }

        #endregion

        #region Grant Users with Company Code Methods

        /// <summary>
        /// Return two lists:
        ///   1)  a list of Company Code not granted to the user
        ///   2)  a list of Company Code granted to the user
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ActionResult CompanyCodeToUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "CompanyCode");
            }

            MembershipUser user = membershipService.GetUser(username);
            var AvailableCompanyCodeForUser = default(IList);
            var AllCompanyCodeForUser = default(IList);

            var isCorporate = User.IsInRole("Corporate");
            int[] assignedFranchises;


            using (var ctx = new MembershipConnection())
            {
                assignedFranchises = ctx.UserFranchise
                                        .Where(uf => uf.UserId == (Guid)user.ProviderUserKey)
                                        .Select(f => f.FranchiseID)
                                        .ToArray();
            }


            AvailableCompanyCodeForUser = (from a in memberShipContext.MembershipFranchise
                                           where !(from o in memberShipContext.UserFranchise
                                                   where o.UserId == (Guid)user.ProviderUserKey
                                                   select o.FranchiseID).Contains(a.FranchiseID)
                                           select a.FranchiseNumber).Distinct().ToList();


            var model = new GrantCompaniesToUser
            {
                UserName = username,
                AvailibleCompanies =
                    (string.IsNullOrEmpty(username)
                         ? new SelectList(AllCompanyCodeForUser)
                         : new SelectList(AvailableCompanyCodeForUser)),
                GrantedCompanyCode =
                     memberShipContext.MembershipFranchise
                               .Where(f => assignedFranchises.Contains(f.FranchiseID))
                               .OrderBy(f => f.FranchiseNumber)
                               .Select(d => new SelectListItem
                               {
                                   Text = d.FranchiseNumber,
                                   Value = d.FranchiseNumber
                               })
                               .ToList(),
            };

            return View(model);

        }
        public ActionResult CompanyCodeToWithoutUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GrantCompaniesToUser(string userName, string companies)
        {
            MembershipUser user = membershipService.GetUser(userName);
            Guid UserGUID = (Guid)user.ProviderUserKey;

            var response = new JsonResponse();

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            string[] CompanyCode = companies.Substring(0, companies.Length - 1).Split(',');

            if (CompanyCode.Length == 0)
            {
                response.Success = false;
                response.Message = "No company code have been granted to the user.";
                return Json(response);
            }

            int frenchID = 0;
            foreach (var companyCode in CompanyCode)
            {
                frenchID = (from s in memberShipContext.MembershipFranchise
                            where s.FranchiseNumber == companyCode
                            select s.FranchiseID).SingleOrDefault();
                UserFranchise_guard b = new UserFranchise_guard
                {
                    FranchiseID = frenchID,
                    UserId = UserGUID,
                };

                memberShipContext.UserFranchise.Add(b);
                memberShipContext.SaveChanges();
            }

            response.Success = true;
            response.Message = "The Company has been GRANTED successfully to " + userName;

            return Json(response);

        }

        /// <summary>
        /// Revoke the selected company code for the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="co"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RevokeCompanyCodeForUser(string userName, string companies)
        {
            var response = new JsonResponse();
            MembershipUser user = membershipService.GetUser(userName);
            Guid UserGUID = (Guid)user.ProviderUserKey;

            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "The userName is missing.";
                return Json(response);
            }

            if (string.IsNullOrEmpty(companies))
            {
                response.Success = false;
                response.Message = "Company Code is missing";
                return Json(response);
            }

            string[] CompanyCode = companies.Substring(0, companies.Length - 1).Split(',');

            if (CompanyCode.Length == 0)
            {
                response.Success = false;
                response.Message = "No company Code are selected to be revoked.";
                return Json(response);
            }

            int frenchID = 0;
            foreach (var companyCode in CompanyCode)
            {
                frenchID = (from s in memberShipContext.MembershipFranchise
                            where s.FranchiseNumber == companyCode
                            select s.FranchiseID).SingleOrDefault();
                int userFranchiseID = (from g in memberShipContext.UserFranchise
                                       where g.UserId == (Guid)user.ProviderUserKey && g.FranchiseID == frenchID
                                       select g.UserFranchiseID).SingleOrDefault();

                var t = memberShipContext.UserFranchise.Find(userFranchiseID);
                memberShipContext.UserFranchise.Remove(t);
                memberShipContext.SaveChanges();
            }

            response.Success = true;
            response.Message = "The Company has been REVOKED successfully for " + userName;

            return Json(response);

        }


        #endregion

        #region Reset password 

        public ActionResult ResetPassword()
        {
            var viewModel = new ResetPasswordViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel model, string userName)
        {
            var changePasswordSucceeded = false;

            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                try
                {

                    if (string.IsNullOrWhiteSpace(userName))
                        userName = User.Identity.Name;

                    var currentUser = Membership.GetUser(userName, true);

                    if (currentUser!=null)
                    {
                        var oldPassword = currentUser.ResetPassword();

                        changePasswordSucceeded = currentUser.ChangePassword(oldPassword, model.NewPassword);
                    }

                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }
            }

            // If we got this far, something failed, redisplay form
            return Json(new { success = changePasswordSucceeded });
        }
        
        #endregion
    }
}
