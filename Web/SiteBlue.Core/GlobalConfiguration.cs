using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SiteBlue.Core
{
    public static class GlobalConfiguration
    {
        private const string TestModeKey = "TestMode";
        private const string PlumberInvoicesFromKey = "PlumberInvoicesFrom";
        private const string ConnectUsProInvoicesFromKey = "ConnectUsProInvoicesFrom";
        private const string DocumentGenerationUserKey = "DocumentGenerationUser";
        private const string DocumentGenerationPasswordKey = "DocumentGenerationPassword";
        private const string InvoiceTermsUriKey = "InvoiceTermsUri";
        private const string InvoiceSendBccKey = "InvoiceSendBcc";
        private const string InitTabletDropPathKey = "InitTabletDropPath";
        private const string SendToTabletDropPathKey = "SendToTabletDropPath";

        public static bool TestMode
        {
            get { return string.Compare(ConfigurationManager.AppSettings[TestModeKey], bool.TrueString, true) == 0; }
        }

        public static string PlumberInvoiceFromAddress
        {
            get { return ConfigurationManager.AppSettings[PlumberInvoicesFromKey]; }
        }

        public static string ConnectUsProInvoiceFromAddress
        {
            get { return ConfigurationManager.AppSettings[ConnectUsProInvoicesFromKey]; }
        }

        public static string DocumentGenerationUser
        {
            get { return ConfigurationManager.AppSettings[DocumentGenerationUserKey]; }
        }

        public static string DocumentGenerationPassword
        {
            get { return ConfigurationManager.AppSettings[DocumentGenerationPasswordKey]; }
        }

        public static string InvoiceTermsUri
        {
            get { return ConfigurationManager.AppSettings[InvoiceTermsUriKey]; }
        }

        public static string InvoiceSendBcc
        {
            get { return ConfigurationManager.AppSettings[InvoiceSendBccKey]; }
        }

        public static string InitTabletDropPath
        {
            get { return ConfigurationManager.AppSettings[InitTabletDropPathKey]; }
        }

        public static string SendToTabletDropPath
        {
            get { return ConfigurationManager.AppSettings[SendToTabletDropPathKey]; }
        }
    }
}
