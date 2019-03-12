using Newtonsoft.Json;
using ProductGroup.LoginFilter;
using ProductGroup.Models;
using ProductGroup.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    [CheckLogin]
    public class HomeController : Controller
    {
        public ProductGroupEntities pg = new ProductGroupEntities();
        public ActionResult Index(string date)
        {
            var toady =date==null? DateTime.Now.AddDays(1):Convert.ToDateTime(date);
            ViewBag.Date = toady;
            var prolist = pg.Products.Where(w => w.CreateDate.Value.Month == toady.Month && w.CreateDate.Value.Day == toady.Day).ToList();
            ViewBag.TotalCount = prolist.Count;
            int orderCount = 0;
            foreach (var p in prolist)
            {
                orderCount += (int)p.OrderCount;
            }
            ViewBag.OrderCount = orderCount;
            return View();
        }
        public ActionResult Datalist(int curr,string date)
        {
            var toady = Convert.ToDateTime(date);
            List<Product> prolist = pg.Products.Where(w => w.CreateDate.Value.Month == toady.Month && w.CreateDate.Value.Day == toady.Day).OrderBy(o=>o.Id).Skip((curr - 1) * 10).Take(10).ToList();
            return View("~/Views/Home/PvProductTable.cshtml", prolist);
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult ProductAdd()
        {
            return View();
        }
        public ActionResult Add(Product pro)
        {
            if (pro.Shop != null)
            {
                var keys=JsonConvert.DeserializeObject<List<keyword>>(pro.KeyWord);
                foreach (var k in keys)
                {
                    Product p = new Product();
                    p.Shop = pro.Shop;
                    p.ShopName = pro.ShopName;
                    p.TaskId = pro.TaskId;
                    p.Price = pro.Price;
                    p.PriceMark = pro.PriceMark;
                    p.ImgUrl = pro.ImgUrl;
                    p.Screen = pro.Screen;

                    p.KeyWord = k.key;
                    p.OrderCount = k.count;
                    p.CreateDate = DateTime.Now.AddDays(1);
                    pg.Products.Add(p);
                }               
                pg.SaveChanges();
                ViewBag.save = 0;
            }
            return View("~/Views/Home/ProductAdd.cshtml");
        }
        public string Del(int id)
        {
            var p = pg.Products.FirstOrDefault(f=>f.Id== id);
            pg.Products.Remove(p);
            pg.SaveChanges();
            result res = new result();
            res.success = true;
            return JsonConvert.SerializeObject(res);
        }
        public ActionResult Edit(int id)
        {
            var p = pg.Products.FirstOrDefault(f => f.Id == id);
            return View(p);
        }
        public ActionResult SaveEdit(Product pro)
        {
            ViewBag.save = 0;
            var pros=pg.Products.Where(w => w.Shop == pro.Shop && w.IsDelete!=-1&&w.Id!=pro.Id);
            var thispro = pg.Products.FirstOrDefault(f => f.Id == pro.Id);
            foreach (var p in pros)
            {
                p.ImgUrl = pro.ImgUrl;
            }
            thispro.ShopName = pro.ShopName;
            thispro.Shop = pro.Shop;
            thispro.Price = pro.Price;
            thispro.PriceMark = pro.PriceMark;
            thispro.KeyWord = pro.KeyWord;
            thispro.OrderCount = pro.OrderCount;
            thispro.Screen = pro.Screen;
            thispro.ImgUrl = pro.ImgUrl;
            thispro.ShopKeeper = pro.ShopKeeper;
            pg.SaveChanges();
            var list = pg.Products.Where(w=>w.IsDelete!=-1).ToList();
            return View("~/Views/Home/Index.cshtml", list);
        }
        public ActionResult Group(string date)
        {
            var today = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            if (date != null)
            {
                today = date;
            }
            ViewBag.Date = today;           
            DateTime dtToday = Convert.ToDateTime(today);//今天
            DateTime dtNexDay = Convert.ToDateTime(today).AddDays(1);//明天
            GroupPage model = new GroupPage();
            model.TotalCount = pg.Groups.Where(w => w.TaskID != "0" && w.Date>dtToday&&w.Date<dtNexDay).Count();
            List<GroupItem> gitem = new List<GroupItem>();
            var surplusList = new List<SurplusItem>();
            var surp = pg.Groups.Where(w => w.TaskID == "0" && w.Date > dtToday && w.Date < dtNexDay).ToList();
            foreach (var s in surp)
            {
                SurplusItem sur = new SurplusItem();
                sur.Surplus = s.SurplusCount;
                var id2 = Convert.ToInt32(s.ProId);
                sur.pro = pg.Products.FirstOrDefault(f => f.Id == id2);
                surplusList.Add(sur);
            }
            model.groupList = gitem;
            model.surplusList = surplusList;
            return View(model);
        }
        public ActionResult Grouplist(int curr,string date)
        {
            var today = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            if (date != null)
            {
                today = date;
            }
            DateTime dtToday = Convert.ToDateTime(today);//今天
            DateTime dtNexDay = Convert.ToDateTime(today).AddDays(1);//明天
            List<GroupItem> gitem = new List<GroupItem>();
            var gro = pg.Groups.Where(w => w.TaskID != "0" && w.Date > dtToday && w.Date < dtNexDay).OrderBy(o=>o.Id).Skip((curr-1) * 10).Take(10).ToList();
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
            return View("~/Views/Home/PvGroupTable.cshtml", gitem);
        }
        public class keyword
        {
            public string key { get; set; }
            public int count { get; set; }
        }
        public class result
        {
            public bool success { get; set; }
            public string msg { get; set; }
        }
    }
}