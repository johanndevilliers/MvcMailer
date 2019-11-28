using System;


#if NETCOREAPP3_0
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
using System.Configuration;
#endif


namespace Mvc.Mailer {
    public static class UrlHelperExtensions {

#if !NETCOREAPP3_0
        public static readonly string BASE_URL_KEY = "MvcMailer.BaseUrl";
#endif

        /// <summary>
        /// This extension method will help generating Absolute Urls in the mailer or other views
        /// </summary>
        /// <param name="urlHelper">The object that gets the extended behavior</param>
        /// <param name="relativeOrAbsoluteUrl">A relative or absolute URL to convert to Absolute</param>
        /// <returns>An absolute Url. e.g. http://domain:port/controller/action from /controller/action</returns>

#if NETCOREAPP3_0
        public static string Abs(this IUrlHelper urlHelper, string relativeOrAbsoluteUrl)
#else
        public static string Abs(this UrlHelper urlHelper, string relativeOrAbsoluteUrl)
#endif
        {
            var uri = new Uri(relativeOrAbsoluteUrl, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri) {
                return relativeOrAbsoluteUrl;
            }

            Uri combinedUri;
            if (Uri.TryCreate(BaseUrl(urlHelper), relativeOrAbsoluteUrl, out combinedUri)) {
                return combinedUri.AbsoluteUri;
            }

            throw new Exception(string.Format("Could not create absolute url for {0} using baseUri{0}", relativeOrAbsoluteUrl, BaseUrl(urlHelper)));
        }


#if NETCOREAPP3_0
        private static Uri BaseUrl(IUrlHelper urlHelper)
#else
        private static Uri BaseUrl(UrlHelper urlHelper) 
#endif
        {

#if NETCOREAPP3_0

            var request = urlHelper.ActionContext.HttpContext.Request;

            return new Uri($"{request.Scheme}://{request.Host}{request.PathBase}");
#else

            var baseUrl = ConfigurationManager.AppSettings[BASE_URL_KEY];

            //No configuration given, so use the one from the context
            if (string.IsNullOrWhiteSpace(baseUrl)) {
                baseUrl = urlHelper.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            }

            return new Uri(baseUrl);

#endif


        }
    }
}