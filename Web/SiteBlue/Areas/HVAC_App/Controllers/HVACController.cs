using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SecurityGuard.Interfaces;
using SecurityGuard.Services;
using SiteBlue.Areas.HVAC_App.Models.Interfaces;
using SiteBlue.Controllers;
using SiteBlue.Data;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Areas.HVAC_App.Controllers
{
    public class HVACController : SiteBlueBaseController
    {
        protected IMembershipService membershipService;
        protected IAuthenticationService authenticationService;

        protected virtual IItemStore<int> ConfigId { get; set; }
        protected virtual IItemStore<int> FranchiseId { get; set; } 

        public HVACController()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            membershipService = new MembershipService(Membership.Provider);
            authenticationService = new AuthenticationService(membershipService, new FormsAuthenticationService());
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ConfigId = new ConfigStore();
            FranchiseId = new FranchiseStoreInCookies();
            base.OnActionExecuting(filterContext);
        }

        protected virtual int GetConfigID()
        {
            var httpCookie = Request.Cookies.Get("id_config");
            if (httpCookie != null)
                return int.Parse(httpCookie.Value.Replace("\"", ""));
            return -1;
        }

        protected virtual Guid GetUserID()
        {
            return UserInfo.UserKey;
        }

        protected int GetJobCode()
        {
            var httpCookie = Request.Cookies.Get("id_job");
            if (httpCookie != null)
                return int.Parse(httpCookie.Value.Replace("\"", ""));
            return -1;
        }

        protected virtual int GetFranchiseID()
        {
            var httpCookie = Request.Cookies.Get("franchise_id");
            if (httpCookie != null)
                return int.Parse(httpCookie.Value.Replace("\"", ""));
            return 51;
        }

        protected void SetCookies(Guid userID)
        {
            var franchiseId = GetCompanyCodeID(userID);
            var configId = CreateConfigIfNotExist(franchiseId);
            //TODO uncoment when config will filled
		    var cookieConf = new HttpCookie("id_config", configId.ToString()){Expires = DateTime.Now.AddDays(356)};
            Response.Cookies.Add(cookieConf);
            var cookieFr = new HttpCookie("franchise_id", franchiseId.ToString()) {Expires = DateTime.Now.AddDays(356)};
            Response.Cookies.Add(cookieFr);
        }

        private int GetCompanyCodeID(Guid userID)
        {
            if (UserInfo != null)
                if (UserInfo.UserKey == userID)
                    return UserInfo.CurrentFranchise.FranchiseID;

            var context800 = new EightHundredEntities();
            var useridstring = userID.ToString();
            if (context800.tbl_Employee.Any(item => item.UserKey == useridstring))
            {
                var usert = userID.ToString();
                return context800.tbl_Employee.First(item => item.UserKey == usert).FranchiseID;
            }

            var context = new MembershipEntities();
            return context.UserFranchise.Any(item => item.UserId == userID)
                       ? context.UserFranchise.First(item => item.UserId == userID).FranchiseID
                       : 51;

            //return 51;
        }

        private int CreateConfigIfNotExist(int franchiseID)
        {
            var context = new EightHundredEntities();
            if (context.tbl_HVAC_ConfigFranchise.Any(item => item.FranchiseID == franchiseID))
            {
                return context.tbl_HVAC_ConfigFranchise.First(item => item.FranchiseID == franchiseID).ConfigID;
            }
            var config = new tbl_HVAC_ConfigsApp { ConfigName = "Config for Franchise " + franchiseID.ToString() };
            context.tbl_HVAC_ConfigsApp.AddObject(config);
            context.SaveChanges();
            var configFranchise = new tbl_HVAC_ConfigFranchise { ConfigID = config.ConfigID, FranchiseID = franchiseID };
            context.tbl_HVAC_ConfigFranchise.AddObject(configFranchise);

            SetQuestionsForNewConfig(context, config);

            context.SaveChanges();
            return config.ConfigID;
        }

        private void SetQuestionsForNewConfig(EightHundredEntities context, tbl_HVAC_ConfigsApp configID)
        {
            var defaultConfig = context.tbl_HVAC_ConfigsApp.First(item => item.ConfigID == 1);
            foreach (var question in defaultConfig.tbl_HVAC_ConfigQuestions)
            {
                configID.tbl_HVAC_ConfigQuestions.Add(new tbl_HVAC_ConfigQuestions { QuestionID = question.QuestionID, OrderNum = question.OrderNum });
            }
        }


    }

    public class FakeMembershipPovider: MembershipProvider
    {
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class FakeMembershipService:IMembershipService
    {
        public MembershipUser CreateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public MembershipUser CreateUser(string username, string password, string email)
        {
            throw new NotImplementedException();
        }

        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByEmail(string emailToMatch)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByName(string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser()
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(object providerUserKey)
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public string GetUserNameByEmail(string emailToMatch)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public string ApplicationName { get; set; }
        public bool EnablePasswordReset { get; private set; }
        public bool EnablePasswordRetrieval { get; private set; }
        public string HashAlgorithmType { get; private set; }
        public int MaxInvalidPasswordAttempts { get; private set; }
        public int MinRequiredNonAlphanumericCharacters { get; private set; }
        public int MinRequiredPasswordLength { get; private set; }
        public int PasswordAttemptWindow { get; private set; }
        public string PasswordStrengthRegularExpression { get; private set; }
        public MembershipProvider Provider { get; private set; }
        public MembershipProviderCollection Providers { get; private set; }
        public bool RequiresQuestionAndAnswer { get; private set; }
        public int UserIsOnlineTimeWindow { get; private set; }
        public event MembershipValidatePasswordEventHandler ValidatingPassword;

        public void OnValidatingPassword(ValidatePasswordEventArgs e)
        {

            MembershipValidatePasswordEventHandler handler = ValidatingPassword;
            if (handler != null) handler(this, e);
        }
    }

    public class FakeFormAuthenticationService:IFormsAuthenticationService
    {
        public void SetAuthCookie(string userName, bool createPersistentCookie)
        {
            
        }

        public void SignOut()
        {
            
        }
    }


}
