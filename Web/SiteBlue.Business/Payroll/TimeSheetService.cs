using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using SiteBlue.Core;
using SiteBlue.Core.Email;
using SiteBlue.Data.EightHundred;

using InvoiceGen = SiteBlue.Business.Enterprise.InvoiceGeneration;

namespace SiteBlue.Business.Payroll
{
    public class TimeSheetService
    {
        // Will add OR update a timesheet for the employee
        public static void SaveTimeSheet(int employeeID
                                        , DateTime datetimeWeekOf
                                        , decimal sundayHours
                                        , decimal mondayHours
                                        , decimal tuesdayHours
                                        , decimal wednesdayHours
                                        , decimal thursdayHours
                                        , decimal fridayHours
                                        , decimal saturdayHours
                                        )
        {
            // TODO: Validat the input

            EightHundredEntities db = new EightHundredEntities();
            var existingTimeSheetList = (from timeSheet in db.tbl_HR_TimeSheet
                                         where timeSheet.EmployeeID == employeeID
                                             && timeSheet.WeekEndingDateOn == datetimeWeekOf
                                         select timeSheet)
                                         .ToList();

            if (existingTimeSheetList.Count > 0)
            {
                // update the existing timesheet
                var existingTimeSheet = existingTimeSheetList.First();

                existingTimeSheet.SundayHours = sundayHours;
                existingTimeSheet.MondayHours = mondayHours;
                existingTimeSheet.TuesdayHours = tuesdayHours;
                existingTimeSheet.WednesdayHours = wednesdayHours;
                existingTimeSheet.ThursdayHours = thursdayHours;
                existingTimeSheet.FridayHours = fridayHours;
                existingTimeSheet.SaturdayHours = saturdayHours;
            }
            else
            {
                // save a new timesheet
                tbl_HR_TimeSheet newTimeSheet = new tbl_HR_TimeSheet()
                    {
                        EmployeeID = employeeID,
                        WeekEndingDateOn = datetimeWeekOf,
                        SundayHours = sundayHours,
                        MondayHours = mondayHours,
                        TuesdayHours = tuesdayHours,
                        WednesdayHours = wednesdayHours,
                        ThursdayHours = thursdayHours,
                        FridayHours = fridayHours,
                        SaturdayHours = saturdayHours
                    };

                db.tbl_HR_TimeSheet.AddObject(newTimeSheet);
            }

            db.SaveChanges();
        }
    }
}











