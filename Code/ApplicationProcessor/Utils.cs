using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace ApplicationProcessor
{
    static class Utils
    {
        public static string ReadXMLElementValue(XElement xml, string element, string defaultValue)
        {
            string elementValue = defaultValue;
            try
            {
                elementValue = xml.Element(element).Value;
            }
            catch(Exception)
            { 
            }
            
            // if no value is specified in xml revert to default                        
            if (elementValue == "")
            {
                elementValue = defaultValue;
            }

            return elementValue;
        }

        public static int ExecuteSQLQuery(string connectionString, string query)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);

            command.Connection.Open();
            int rowsAffected = command.ExecuteNonQuery();
            command.Connection.Close();

            return rowsAffected;
        }

    }
}
