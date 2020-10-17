using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbDumpClassLibrary
{
    public class exportDB
    {
        string constring, file;
        public exportDB(string connectionString, string filePath)
        {
            constring = connectionString;
            file = filePath;
        }
        
        public void doThat()
        {
            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportInfo.TablesToBeExportedList = new List<string> {
                                "aspnetusers",
                                "msgs"
                                };
                        mb.ExportToFile(file);
                        conn.Close();
                    }
                }
            }
        }
    }
}
