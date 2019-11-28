
#if NETCOREAPP3_0

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

#else

using System.Web.Mvc;
using System.Web;

#endif

namespace Mvc.Mailer {
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Produces the tag for inline image
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="contentId">e.g. logo</param>
        /// <param name="alt">e.g. Company Logo</param>
        /// <returns><img src="cid:logo" alt="Company Logo"/></returns>
        public static
#if NETCOREAPP3_0
            IHtmlContent InlineImage(this IHtmlHelper htmlHelper, string contentId, string alt = "")
#else
            IHtmlString InlineImage(this HtmlHelper htmlHelper, string contentId, string alt = "")
#endif

        {
            return htmlHelper.Raw($"<img src=\"cid:{contentId}\" alt=\"{alt}\"/>");
        }
    }
}
