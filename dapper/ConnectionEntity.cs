using System;
using System.Collections.Generic;
using System.Text;

namespace CitizenBudget.Common.Dapper
{
    public class MssqlConnectionEntity
    {
        public string ServerName { get; set; }
        public string DataBaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
