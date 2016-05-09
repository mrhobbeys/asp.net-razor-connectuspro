using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Common;
namespace QBEngine
{
    interface IInvoice
    {

        Invoice searchbyInvoiceNo(string No);
        //DataSet searchbyAmount(double Fromamount,double Toamount);
    }
}
