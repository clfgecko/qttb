using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class TreeItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool nodrop { get; set; }
        public List<TreeItem> nodes { get; set; }
    }
}
