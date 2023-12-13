using Umbraco.Cms.Api.Delivery.Configuration;
using Umbraco.Cms.Core.Composing;

namespace Server.Configuration;

/// <summary>
/// Enables member authorization support in the Swagger document.
/// Note that this also requires the Swagger login redirect URL configured in appsettings.json (see LoginRedirectUrls).
/// </summary>
public class ConfigureSwaggerComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
        => builder
			.Services
			.ConfigureOptions<ConfigureUmbracoMemberAuthenticationDeliveryApiSwaggerGenOptions>();
}