using ProductGroup.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductGroup.Models
{
    public class GroupPage:PageModel
    {
        public List<GroupItem> groupList { get; set; }
        public List<SurplusItem> surplusList { get; set; }
    }
    public class GroupItem
    {
        //public string TaskId { get; set; }
        //public string WWid { get; set; }
        public Group group { get; set; }
        public List<Product> proList { get; set; }
    }
    public class SurplusItem
    {
        public int? Surplus { get; set; }
        public Product pro { get; set; }
    }
    public class ExcelhandModel
    {
        public Group group { get; set; }
        public string ShopName { get; set; }
        public string Shop { get; set; }
    }
}