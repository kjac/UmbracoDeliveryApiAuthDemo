using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Server.Configuration;

/// <summary>
/// Sets up a CORS policy that allows the client application to utilize the Delivery API.
/// </summary>
public class ConfigureCorsComposer : IComposer
{
    private const string ClientOrigin = "http://localhost:3000";
    
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddCors(opt =>
            opt.AddPolicy("AllowClient", options => options
                .WithOrigins(ClientOrigin)
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
            )
        );

        builder.Services.Configure<UmbracoPipelineOptions>(options => options.AddFilter(new CorsAllowAllPipelineFilter()));
    }

    private class CorsAllowAllPipelineFilter : UmbracoPipelineFilter
    {
        public CorsAllowAllPipelineFilter()
            : base(nameof(CorsAllowAllPipelineFilter)) =>
            PostRouting = app => app.UseCors("AllowClient");
    }
}