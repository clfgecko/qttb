﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class TreeModel
    {
        public string id { get; set; }
        public string pId { get; set; }
        public string name { get; set; }
        public string name_control { get; set; }
        public string pace_id { get; set; }
        public bool open { get; set; }
        public string file { get; set; }
        public bool flag { get; set; }
        public bool @checked { get; set; }
    }
}
