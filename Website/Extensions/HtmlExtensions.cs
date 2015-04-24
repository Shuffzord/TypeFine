using System;
using System.Web.Mvc;

namespace Website.Extensions
{
    public static class HtmlExtensions
    {
        public static string AbsoluteAction(this UrlHelper url, string action, string controller)
        {
            var requestUrl = url.RequestContext.HttpContext.Request.Url;

            
            var absoluteAction = string.Format("{0}://{1}{2}",
                requestUrl.Scheme,
                requestUrl.Authority,
                url.Action(action, controller, new { }));

            return absoluteAction;
        }
    }
}