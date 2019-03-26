using Newtonsoft.Json;
using ProductGroup.Models;
using ProductGroup.Models.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace WebApplication1.Controllers
{
    public class SharedController : Controller
    {
        // GET: Shared
        public string Upload()
        {
            HttpPostedFileBase file = Request.Files[0];
            var filename= DateTime.Now.ToString("yyyyMMddHHmmss")+file.FileName.Substring(file.FileName.LastIndexOf("."));
            string savePath = AppDomain.CurrentDomain.BaseDirectory + "Upload/Product/" + filename;
            file.SaveAs(savePath);
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            r.data = new img { src = "/Product/" + filename };
            return JsonConvert.SerializeObject(r);
        }
        public ProductGroupEntities pg = new ProductGroupEntities();
        public string SaveGroup(int groupcount)
        {
            new GRandom().Group(groupcount);
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public string SaveOneGroup()
        {
            DateTime dtToday = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));//今天
            DateTime dtNexDay = Convert.ToDateTime(DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"));//明天
            pg.Groups.RemoveRange(pg.Groups.Where(w => w.Date > dtToday && w.Date < dtNexDay));

            var toady = DateTime.Now.AddDays(1);
            var proList = pg.Products.Where(w => w.CreateDate.Value.Month == toady.Month && w.CreateDate.Value.Day == toady.Day).ToList();
            var count = 0;            
            try
            {
                foreach (var pro in proList)
                {
                    string name = "";
                    count++;
                    name += "" + DateTime.Now.AddDays(1).ToString("MMdd") + "Q";
                    var l = 4 - count.ToString().Length;
                    for (var p = 0; p < l; p++)
                    {
                        name += "0";
                    }
                    name += count;
                    pg.Groups.Add(new Group { TaskID = name, ProId = pro.Id.ToString(), Date = DateTime.Now.AddDays(1) });
                }
                pg.SaveChanges();
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { code = -1, msg = ex.Message });
            }
            return JsonConvert.SerializeObject(new { code = 0, msg = "" });
        }
        public string exportExcel()
        {
            ExportExcel ee = new ExportExcel();
            List <GroupItem> gi= GetGroupData();
            foreach (var g in gi)
            {
                ee.exportExcel(Server.MapPath("/"),g);
            }
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public string ExcelHand(string date)
        {
            ExportExcel ee = new ExportExcel();
            //List<Group> gi = pg.Groups.Where(w=>w.TaskID!="0").ToList();
            var gl=GrouplistAll2(date);
            foreach (var g in gl)
            {
                ee.ExcelHand(Server.MapPath("/"), g);
            }
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public string ExcelTask(string date)
        {
            ExportExcel ee = new ExportExcel();
            List<Group> gi = pg.Groups.Where(w => w.TaskID != "0").ToList();
            ee.ExcelTask(Server.MapPath("/"), GrouplistAll(date));
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public List<GroupItem> GetGroupData()
        {
            DateTime dtToday = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));//今天
            DateTime dtNexDay = Convert.ToDateTime(DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"));//明天
            List<GroupItem> gitem = new List<GroupItem>();
            var gro = pg.Groups.Where(w => w.TaskID != "0"&& w.Date > dtToday && w.Date < dtNexDay).ToList();
            foreach (var g in gro)
            {
                GroupItem gi = new GroupItem();
                gi.proList = new List<Product>();
                gi.group = g;
                var ids = g.ProId.Split('|');
                foreach (var id in ids)
                {
                    if (id != "")
                    {
                        var id2 = Convert.ToInt32(id);
                        gi.proList.Add(pg.Products.FirstOrDefault(f => f.Id == id2));
                    }

                }
                gitem.Add(gi);
            }
            return gitem;
        }
        public string ClearData()
        {
            var toady = DateTime.Now.AddDays(1);
            var proarr = pg.Products.Where(w=>w.CreateDate.Value.Month==toady.Month&&w.CreateDate.Value.Day==toady.Day);
            //foreach(var p in proarr)
            //{
            //    p.IsDelete = -1;
            //}
            pg.Products.RemoveRange(proarr);
            pg.SaveChanges();
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public string DataImport()
        {
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            List<Product> pros= Utility.DataImport.ImpotrExcel(Server.MapPath("/"));
            if (pros.Count > 0 && pros[0].Shop == "-1")
            {
                r.code = "-1";
                r.msg = "店铺："+ pros[0].ShopName+"一栏数据格式有误！";
            }
            else
            {
                foreach (var pro in pros)
                {
                    pro.CreateDate = DateTime.Now.AddDays(1);
                    pg.Products.Add(pro);
                }
                pg.SaveChanges();
            }
            return JsonConvert.SerializeObject(r);
        }
        public string HandOut(List<Group> handList)
        {
            foreach (var hl in handList)
            {
                Group g=pg.Groups.FirstOrDefault(f => f.Id == hl.Id);
                if (g.WWId != hl.WWId)
                {
                    g.WWId = hl.WWId;
                    g.WWInputDate = DateTime.Now;
                }                
            }
            pg.SaveChanges();
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public List<GroupItem> GrouplistAll(string date)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            if (date != null)
            {
                today = date;
            }
            DateTime dtToday = Convert.ToDateTime(today);//今天
            DateTime dtNexDay = Convert.ToDateTime(today).AddDays(1);//明天
            List<GroupItem> gitem = new List<GroupItem>();
            var gro = pg.Groups.Where(w => w.TaskID != "0" && w.Date > dtToday && w.Date < dtNexDay).ToList();
            foreach (var g in gro)
            {
                GroupItem gi = new GroupItem();
                gi.proList = new List<Product>();
                gi.group = g;
                var ids = g.ProId.Split('|');
                foreach (var id in ids)
                {
                    if (id != "")
                    {
                        var id2 = Convert.ToInt32(id);
                        gi.proList.Add(pg.Products.FirstOrDefault(f => f.Id == id2));
                    }

                }
                gitem.Add(gi);
            }
            return gitem;
        }
        public List<List<ExcelhandModel>> GrouplistAll2(string date)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            if (date != null)
            {
                today = date;
            }
            DateTime dtToday = Convert.ToDateTime(today);//今天
            DateTime dtNexDay = Convert.ToDateTime(today).AddDays(1);//明天
            List<ExcelhandModel> ehs = new List<ExcelhandModel>();
            var gro = pg.Groups.Where(w => w.TaskID != "0" && w.Date > dtToday && w.Date < dtNexDay).ToList();
            foreach (var g in gro)
            {
                var ids = g.ProId.Split('|');
                foreach (var id in ids)
                {
                    if (id != "")
                    {
                        ExcelhandModel gi = new ExcelhandModel();
                        var id2 = Convert.ToInt32(id);
                        var p=pg.Products.FirstOrDefault(f => f.Id == id2);
                        gi.ShopName = p.ShopName;
                        gi.Shop = p.Shop;
                        gi.group = g;
                        ehs.Add(gi);
                    }

                }
            }
            List<List<ExcelhandModel>> ll = new List<List<ExcelhandModel>>();
            var gb = ehs.GroupBy(o => o.Shop).ToList();
            foreach (var gbitem in gb)
            {
                ll.Add(gbitem.OrderBy(o=>o.group.WWInputDate).ToList());
            }
            return ll;
        }
        public string ShopList(string words)
        {
            List<Product> proarr=pg.Products.Where(w => w.ShopName.Contains(words)).ToList();
            proarr=proarr.Where((x, i) => proarr.FindIndex(z => z.ShopName == x.ShopName) == i).ToList();
            JsonResult js = new JsonResult();
            return JsonConvert.SerializeObject(proarr);
        }
        public string ImportHistory()
        {
            JsonResult js = new JsonResult();
            result r = new result();
            r.code = "0";
            r.msg = "";
            List<CollectHistory> pros = Utility.DataImport.ImportCollectHistory(Server.MapPath("/"));
            foreach (var pro in pros)
            {
                pro.CreatDate = DateTime.Now;
                pg.CollectHistories.Add(pro);
            }
            pg.SaveChanges();
            return JsonConvert.SerializeObject(r);
        }
        public class result
        {
            public string code { get; set; }
            public string msg { get; set; }
            public img data { get; set; }
        }
        public class img
        {
            public string src { get; set; }
        }
    }
}