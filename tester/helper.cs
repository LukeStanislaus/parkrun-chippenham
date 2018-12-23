using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 

namespace tester
{
    public static class Helper
    {

        /// <summary>
        /// Returns the SQL connection string.
        /// </summary>
        /// <returns>The SQL connection string.</returns>
        public static string CnnVal()
        {
            return @"Server=95.146.92.110;Port=3306;Database=parkrun;Uid=root;Pwd=abc123;";

        }

    }
}