using Newtonsoft.Json;
using ProductGroup.LoginFilter;
using ProductGroup.Models;
using ProductGroup.Models.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductGroup.Controllers
{
    public class InfoCollectController : Controller
    {
        // GET: InfoCollect
        public ProductGroupEntities pg = new ProductGroupEntities();
        [CheckLogin]
        public ActionResult Index(string date, Collect collect)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            if (date != null)
            {
                today = date;
            }
            ViewBag.Date = today;
            ViewBag.Search = collect;
            ViewBag.TotalCount = GetCollect(today, collect).Count;
            List<Collect> model = new List<Collect>();
            return View(model);
        }
        public ActionResult Collect(Collect group)
        {
            Collect model = new Collect();
            if (group.TaskId != null)
            {
                var g=pg.Groups.FirstOrDefault(f => f.TaskID == group.TaskId);
                if (g != null)
                {
                    var prevmonth = DateTime.Now.AddMonths(-1);
                    pg.Collects.RemoveRange(pg.Collects.Where(f => f.TaskId == group.TaskId));
                    pg.SaveChanges();
                    var nowmonth = pg.Collects.Where(w => w.WWId == group.WWId&&w.CreatDate> prevmonth).OrderByDescending(o=>o.CreatDate).ToList();
                    var inputdate = DateTime.Now;
                    g.WWId = group.WWId;
                    g.WWInputDate = inputdate;
                    group.CreatDate = inputdate;
                    if (nowmonth.Count > 0)
                    {
                        group.LastCollect = nowmonth[0].Id;
                        ViewBag.save = -2;
                    }
                    else
                    {

                    }
                    pg.Collects.Add(group);
                    pg.SaveChanges();
                    ViewBag.save = 0;              
                }
                else {
                    ViewBag.save = -1;
                }
            }
            return View(model);
        }
        public ActionResult CollectTransfer(Collect group)
        {
            return View();
        }
        public ActionResult Datalist(int curr, string date,Collect collect)
        {
            ViewBag.Curr = curr;
            var prolist = new List<Collect>();
            prolist = GetCollect(date,collect).OrderByDescending(o => o.Id).Skip((curr - 1) * 10).Take(10).ToList();
            return View("~/Views/InfoCollect/PvCollectTable.cshtml", prolist);
        }
        public List<Collect> GetCollect(string date, Collect collect)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            if (date != null)
            {
                today = date;
            }
            DateTime dtToday = Convert.ToDateTime(today);//今天
            DateTime dtNexDay = Convert.ToDateTime(today).AddDays(1);//明天
            var prolist = pg.Collects.Where(w => w.CreatDate > dtToday && w.CreatDate < dtNexDay);
            if (!string.IsNullOrEmpty(collect.TaskId) && prolist.Count() > 0)
            {
                prolist = prolist.Where(w => w.TaskId.Contains(collect.TaskId));
            }
            if (!string.IsNullOrEmpty(collect.WWId) && prolist.Count() > 0)
            {
                prolist = prolist.Where(w => w.WWId.Contains(collect.WWId));
            }
            if (!string.IsNullOrEmpty(collect.CityPartner) && prolist.Count() > 0)
            {
               prolist  = prolist.Where(w => w.CityPartner.Contains(collect.CityPartner));
            }
            if (!string.IsNullOrEmpty(collect.Clerk) && prolist.Count()>0)
            {
                prolist = prolist.Where(w => w.Clerk.Contains(collect.Clerk));
            }
            return prolist.ToList();
        }
        public string Upload()
        {
            HttpPostedFileBase file = Request.Files[0];
            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string save = AppDomain.CurrentDomain.BaseDirectory + "Upload/InfoCollect/";
            CreatFolder(save);
            string savePath = save + filename;
            file.SaveAs(savePath);
            JsonResult js = new JsonResult();
            Result r = new Result();
            r.succ = true;
            r.msg = "/Upload/InfoCollect/" + filename ;
            return JsonConvert.SerializeObject(r);
        }
        public string MultipleUpload()
        {
            HttpPostedFileBase file = Request.Files[0];
            var filename = DateTime.Now.ToString("yyyyMMddHHmmss")+ file.FileName.Substring(0,file.FileName.LastIndexOf(".")) + file.FileName.Substring(file.FileName.LastIndexOf("."));
            string save = AppDomain.CurrentDomain.BaseDirectory + "Upload/InfoCollect/";
            CreatFolder(save);
            string savePath = save + filename;
            file.SaveAs(savePath);
            JsonResult js = new JsonResult();
            Result r = new Result();
            r.succ = true;
            r.msg = "/Upload/InfoCollect/" + filename;
            return JsonConvert.SerializeObject(r);
        }
        public string GetLastCollect(int id)
        {
            var coll = pg.Collects.FirstOrDefault(f => f.Id == id);
            return JsonConvert.SerializeObject(coll);
        }
        public void CreatFolder(string url)
        {
            if (!Directory.Exists(url))
            {
                Directory.CreateDirectory(url);
            }
        }
        [CheckLogin]
        public ActionResult CollectHistory()
        {
            ViewBag.TotalCount = pg.CollectHistories.Count();
            List<CollectHistory> model = new List<CollectHistory>();
            return View(model);
        }
        public ActionResult HistoryData(int curr)
        {
            var prolist = new List<CollectHistory>();
            prolist = pg.CollectHistories.OrderBy(o=>o.MemberName).Skip((curr - 1) * 10).Take(10).ToList();
            return View("~/Views/InfoCollect/PvCollectHistoryTable.cshtml", prolist);
        }
        public string ClearHistory()
        {
            //pg.CollectHistories.RemoveRange(pg.CollectHistories.ToList());
            pg.Database.ExecuteSqlCommand("delete CollectHistory");
            pg.SaveChanges();
            JsonResult js = new JsonResult();
            Result r = new Result();
            r.succ = true;
            return JsonConvert.SerializeObject(r);
        }
        public ActionResult CollectSeach(string clerk)
        {
            ViewBag.clerk = clerk;
            ViewBag.TotalCount = pg.CollectHistories.Count();
            List<Collect> model = new List<Collect>();
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime dtToday = Convert.ToDateTime(today);//今天
            DateTime dtNexDay = Convert.ToDateTime(today).AddDays(1);//明天
            if (!string.IsNullOrEmpty(clerk))
            {
                model = pg.Collects.Where(w => w.Clerk.Contains(clerk)&w.CreatDate>dtToday&w.CreatDate<dtNexDay).ToList();
            }
            return View(model);
        }
    }
}