using System;
using System.Globalization;
using System.Web;
using System.Web.Http;

namespace ResxDescriptionFilters.UsageExample
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            string[] userLanguages = ((HttpApplication)sender).Request.UserLanguages;

            if (userLanguages != null && userLanguages.Length > 0)
            {
                CultureInfo.CurrentUICulture = new CultureInfo(userLanguages[0]);
            }
        }
    }
}
