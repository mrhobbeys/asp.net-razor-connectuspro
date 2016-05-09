using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Alerts
{
    public enum AlertType
    {
        AppointmentBooked = 1,
        JobCompleted = 2,
        ServiceAgreementSold = 3,
        DrainMaintenanceSold = 4,
        CustomerCancellation = 5,
        CallRescheduled = 6,
        EstimateGiven = 7,
        JobHaltedWaitingParts = 8,
        HvacSalesAlert = 9,
        RecallBooked = 10

    }
}
