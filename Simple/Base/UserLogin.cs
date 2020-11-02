﻿ using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Base
{
    [Serializable]
    public class UserLogin
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public string Avartar { get; set; }
        public int EmployeeId { get; set; }
        public string GroupId { get; set; }
        public bool ViewHome { get; set; }
    }
}
