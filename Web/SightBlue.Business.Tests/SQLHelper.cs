using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;

namespace SightBlue.Business.Tests
{
    /// <summary>
    /// Used for helping with the SQL Setup of the automated integration test
    /// </summary>
    internal class SQLHelper
    {
        internal static void RunSetupScript(string storedProcedureHelper, string commandParameter_ScenarioNumber)
        {
            string connectionStringUnitTests = ConfigurationManager.ConnectionStrings["EightHundred_UnitTests"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionStringUnitTests))
                {
                    conn.Open();
                    SqlCommand commandRunUnitTestProc = new SqlCommand(storedProcedureHelper, conn) { CommandType = System.Data.CommandType.StoredProcedure };
                    commandRunUnitTestProc.Parameters.Add(new SqlParameter("@scenarioNumber", commandParameter_ScenarioNumber));
                    commandRunUnitTestProc.ExecuteScalar();
                }
            }
            catch (SqlException sqlEx)
            {
                // Faiure assertion or tak on message
                throw sqlEx;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
