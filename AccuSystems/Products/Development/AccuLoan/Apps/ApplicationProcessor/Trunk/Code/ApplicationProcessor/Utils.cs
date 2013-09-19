//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="AccuSystems LLC">
//     Copyright (c) AccuSystems.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ApplicationProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Data.SqlClient;

    /// <summary>
    /// utilities for data processing
    /// </summary>
    static class Utils
    {
        /// <summary>
        /// Reads the Value of an XML Element
        /// </summary>
        /// <param name="xml">The XElement block to retreive the Element value from</param>
        /// <param name="element">The element to lookup</param>
        /// <param name="defaultValue">The value to assign if no element value is found</param>
        /// <returns>The Element value</returns>
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

        /// <summary>
        /// Executes an SQL command and returns the number of rows affected
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="query">The database query</param>
        /// <returns>The number of rows affected</returns>
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
