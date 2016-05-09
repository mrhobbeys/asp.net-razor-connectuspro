using System.Linq;
using SiteBlue.Data.EightHundred;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SiteBlue.Test.Data.EightHundredBaseContext
{
    
    
    /// <summary>
    ///This is a test class for EightHundredBaseContextTest and is intended
    ///to contain all EightHundredBaseContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EightHundredBaseContextTest
    {
        private static SiteBlue.Data.EightHundred.EightHundredBaseContext testContextInstance;

        #region Additional test attributes
         
        //You can use the following additional attributes as you write your tests:
        
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext context)
        {
            testContextInstance = new EightHundredEntities();
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            testContextInstance.Dispose();
        }
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        internal virtual SiteBlue.Data.EightHundred.EightHundredBaseContext CreateEightHundredBaseContext()
        {
            // TODO: Instantiate an appropriate concrete class.
            SiteBlue.Data.EightHundred.EightHundredBaseContext target = new EightHundredEntities();
            return target;
        }

        /// <summary>
        ///A test for tbl_HVAC_Guarantees
        ///</summary>
        [TestMethod()]
        public void Test_tbl_HVAC_Guarantees()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_Guarantees.FirstOrDefault();
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigGuaranteeTexts()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigGuaranteeTexts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_Answers()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_Answers.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigFranchise()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigFranchise.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigHowWeAreUrls()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigHowWeAreUrls.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigLogoUrl()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigLogoUrl.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigPackages()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigPackages.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigQuestions()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigQuestions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigReliableInstallationsUrl()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigReliableInstallationsUrl.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigSystems()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigSystems.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigVideoUrls()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigVideoUrls.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigWhoWeAreTexts()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigWhoWeAreTexts.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_ConfigsApp()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_ConfigsApp.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_CustomersAnswers()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_CustomersAnswers.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_Questions()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_Questions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void Test_tbl_HVAC_SystemType()
        {
            try
            {
                var actual = testContextInstance.tbl_HVAC_SystemType.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Assert.Fail(ex.InnerException.Message);
                Assert.Fail(ex.Message);
            }
        }
    }
}
