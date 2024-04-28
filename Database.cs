using System;
using System.IO;
using JollyWrapper;

namespace DB
{
    class DatabaseConnection
    {
        public void InsertConversion(string input)
        {
            // Allow for paramterised queries
            QueryParms parms = new QueryParms(){
                {"@input", input}
            };
            Database.ExecuteNonQuery("INSERT INTO `Coversions`(`CoversionID`, `Text`, `TIMESTAMP`) VALUES (null,@input,null)" , parms); // SQL query to insert the conversion into the database
        }
        public DatabaseConnection() // Connects to databse when created
        {
            Database.Init("plesk.remote.ac",
              "ws328700_OOSDD",
              "~7138zmfI",
              "ws328700_OOSDD",
              "SSLMODE=None");
        }
    }
}