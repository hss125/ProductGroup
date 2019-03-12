using System.Web;
using System.Web.Mvc;

namespace ProductGroup.LoginFilter
{
    public class CheckLogin : ActionFilterAttribute
    {
        //在Action执行之前　乱了点，其实只是判断Cookie用户名密码正不正确而已而已。
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookieCollection CookieCollect = HttpContext.Current.Request.Cookies;
            if (CookieCollect["user"] == null)
            {
                filterContext.Result = new RedirectResult("/User/Login");
            }
        }
    }
}
