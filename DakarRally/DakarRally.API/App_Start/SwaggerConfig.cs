using System.Web.Http;
using WebActivatorEx;
using DakarRally.API;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace DakarRally.API
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            //GlobalConfiguration.Configuration
            //  .EnableSwagger(c => c.SingleApiVersion("v1", "A title for your API"))
            //  .EnableSwaggerUi();
            //
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "Dakar Rally API");
                        c.PrettyPrint();
                        c.DescribeAllEnumsAsStrings();
                        c.IncludeXmlComments(string.Format(@"{0}\bin\DakarRally.API.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("Dakar Rally");
                    });
        }
    }
}
