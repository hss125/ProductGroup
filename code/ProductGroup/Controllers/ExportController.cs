using Newtonsoft.Json;
using ProductGroup.Models;
using ProductGroup.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace ProductGroup.Controllers
{
    public class ExportController : Controller
    {
        // GET: Export
        public ProductGroupEntities pg = new ProductGroupEntities();
        public string exportSource()
        {
            ExportExcel ee = new ExportExcel();
            List<Product> gi = pg.Products.Where(w=>w.IsDelete!=-1).ToList();
            ee.exportSource(Server.MapPath("/"), gi);
            JsonResult js = new JsonResult();
            Result r = new Result();
            r.succ = true;
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public string exportToayPro()
        {
            var pros=pg.Products.Where(w=>w.IsDelete!=-1).GroupBy(g => g.Shop).OrderBy(o=>o.FirstOrDefault().ShopKeeper).ToList();
            ExportPro ee = new ExportPro();
            ee.exportToayProData(Server.MapPath("/"), pros);
            JsonResult js = new JsonResult();
            Result r = new Result();
            r.succ = true;
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        public string exportSamePro()
        {
            DateTime now = DateTime.Now;
            DateTime d1 = new DateTime(now.Year, now.Month, 1);
            DateTime d2 = d1.AddMonths(1).AddMilliseconds(-1);
            var pros = pg.Products.Where(w => w.CreateDate>=d1&&w.CreateDate<=d2).GroupBy(g => g.ShopKeeper).ToList();
            ExportPro ee = new ExportPro();
            for (int i = 0; i < pros.Count; i++)
            {
                //var s1=pros[i].GroupBy(g => new { g.Shop,g.CreateDate }).ToList();
                ee.exportSameProData(Server.MapPath("/"), pros[i]);
            }
            JsonResult js = new JsonResult();
            Result r = new Result();
            r.succ = true;
            r.msg = "";
            return JsonConvert.SerializeObject(r);
        }
        //public string exportOneExcel()
        //{
        //    DateTime dtToday = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));//今天
        //    DateTime dtNexDay = Convert.ToDateTime(DateTime.Now.AddDays(2).ToString("yyyy-MM-dd"));//明天
        //    var proarr = pg.Products.Where(w=>w.CreateDate > dtToday && w.CreateDate < dtNexDay).ToList();
        //    try
        //    {
        //        foreach (var pro in proarr)
        //        {
        //            new ExportExcel().exportOneExcel(Server.MapPath("/"), pro);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonConvert.SerializeObject(new { code = -1, msg = ex.Message });
        //    }
        //    return JsonConvert.SerializeObject(new { code=0, msg =""});
        //}
    }
}