using System.Web.Http;
using MusicApi.Formatters;
using Newtonsoft.Json.Serialization;
using PointW.WebApi.MediaTypeFormatters.CollectionJson;
using PointW.WebApi.MediaTypeFormatters.Hal;

namespace MusicApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            // Web API formatters, using HAL as the default
            config.Formatters.Clear();
            config.Formatters.Add(new HtmlMediaTypeFormatter() { Indent = true });
            config.Formatters.Add(new HalJsonMediaTypeFormatter(true) { Indent = true });
            config.Formatters.Add(new CollectionJsonMediaTypeFormatter());
            config.EnsureInitialized();

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
     
        }
    }
}
