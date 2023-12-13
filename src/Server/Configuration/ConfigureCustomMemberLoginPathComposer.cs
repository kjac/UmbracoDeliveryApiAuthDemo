using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;

namespace Server.Configuration;

/// <summary>
/// This composer replaces the default ASP.NET Core login path ("/Account/Login") with a custom path ("/login").
/// It is purely a cosmetic thing - authorization is functionally indifferent to the login path. 
/// </summary>
public class ConfigureCustomMemberLoginPathComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
        => builder.Services.ConfigureOptions<ConfigureCustomMemberLoginPath>();
    
    private class ConfigureCustomMemberLoginPath : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        public void Configure(string? name, CookieAuthenticationOptions options)
        {
            if (name != IdentityConstants.ApplicationScheme)
            {
                return;
            }

            Configure(options);
        }

        public void Configure(CookieAuthenticationOptions options)
            => options.LoginPath = "/login";
    }
}