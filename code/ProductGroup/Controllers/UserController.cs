using Newtonsoft.Json;
using ProductGroup.LoginFilter;
using ProductGroup.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductGroup.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        ProductGroupEntities pg = new ProductGroupEntities();
        public ActionResult Login(User user)
        {
            if (user.UserName != null)
            {
                var u=pg.Users.FirstOrDefault(f=>f.UserName==user.UserName&&f.PassWord==user.PassWord);
                if (u != null)
                {
                    HttpCookie hcUserName = new HttpCookie("user", JsonConvert.SerializeObject(u));
                    System.Web.HttpContext.Current.Response.SetCookie(hcUserName);
                    return RedirectToAction("Index","Home");
                }
                else {
                    ViewBag.Error = "1";
                }
                
            }
            return View();
        }
        [CheckLogin]
        public ActionResult List()
        {            
            return View(pg.Users.ToList());
        }
        public ActionResult Edit(int id)
        {
            var p = new User();
            if (id != 0)
            {
                p = pg.Users.FirstOrDefault(f => f.Id == id);
            }
            return View(p);
        }
        public ActionResult SaveEdit(User user)
        {
            ViewBag.save = 0;
            if (user.Id == 0)
            {
                user.CreatTime = DateTime.Now;
                pg.Users.Add(user);                
            }
            else
            {
                var u = pg.Users.FirstOrDefault(f=>f.Id==user.Id);
                u.UserName = user.UserName;
                u.PassWord = user.PassWord;
                u.Type = user.Type;
            }
            pg.SaveChanges();
            var list = pg.Users.ToList();
            return View("~/Views/User/List.cshtml", list);
        }
        public string Del(int id)
        {
            var p = pg.Users.FirstOrDefault(f => f.Id == id);
            pg.Users.Remove(p);
            pg.SaveChanges();
            result res = new result();
            res.success = true;
            return JsonConvert.SerializeObject(res);
        }
        public class result
        {
            public bool success { get; set; }
            public string msg { get; set; }
        }
    }
}