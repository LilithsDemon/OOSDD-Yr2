using System;
using System.IO;
using JollyWrapper;

namespace DB
{
    class DatabaseConnection
    {
        public void InsertConversion(string input)
        {
            QueryParms parms = new QueryParms(){
                {"@input", input}
            };
            Database.ExecuteNonQuery("INSERT INTO `Coversions`(`CoversionID`, `Text`, `TIMESTAMP`) VALUES (null,@input,null)" , parms);
        }
        public DatabaseConnection()
        {
            Database.Init("plesk.remote.ac",
              "WS328700_OOSDD",
              "~7138zmfI",
              "WS328700_OOSDD",
              "SSLMODE=None");
        }
    }
}