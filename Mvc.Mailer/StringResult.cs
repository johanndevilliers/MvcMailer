using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
#else
using System.Web.Mvc;
#endif

namespace Mvc.Mailer
{
    /// <summary>
    /// Mimics the ViewResult with an important difference - the view is only storted in the Output property insted of written to a
    /// browser stream!
    /// </summary>
    public class StringResult : ViewResult
    {
        public StringResult()
        {
        }
        
        public StringResult(string viewName)
        {
            ViewName = viewName;
        }

        /// <summary>
        /// Contains the output after compiling the view and putting in the values
        /// </summary>
        public string Output { get; private set; }

        public virtual void ExecuteResult(ControllerContext context, string mailerName)
        {


            //remember the controller name
            var controllerName = context.RouteData.Values["controller"];

            //temporarily change it to the mailer name so that FindView works
            context.RouteData.Values["controller"] = mailerName;
            try
            {
                ExecuteResult(context);
            }
            finally
            {
                //restore the controller name
                context.RouteData.Values["controller"] = controllerName;
            }
        }

#if !NETCOREAPP
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (String.IsNullOrEmpty(ViewName))
            {
                throw new ArgumentNullException("ViewName of StringResult cannot be null or empty");
            }

            if (View == null)
            {
                var result = FindView(context);
                View = result.View;
            }

            var stringBuilder = new StringBuilder();
            TextWriter writer = new StringWriter(stringBuilder);

            var viewContext = new ViewContext(context, View, ViewData, TempData, writer);
            View.Render(viewContext, writer);

            Output = stringBuilder.ToString();
        }
#else
        public override void ExecuteResult(ActionContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (String.IsNullOrEmpty(ViewName))
            {
                throw new ArgumentNullException("ViewName of StringResult cannot be null or empty");
            }

            var viewEngine = context.HttpContext.RequestServices.GetService<ICompositeViewEngine>();

            var view = viewEngine.FindView(context, ViewName, false).View;

            var stringBuilder = new StringBuilder();
            TextWriter writer = new StringWriter(stringBuilder);

            var viewContext = new ViewContext(context, view, ViewData, TempData, writer, new HtmlHelperOptions());

            view.RenderAsync(viewContext).Wait();

            Output = stringBuilder.ToString();

        }

#endif

    }
}