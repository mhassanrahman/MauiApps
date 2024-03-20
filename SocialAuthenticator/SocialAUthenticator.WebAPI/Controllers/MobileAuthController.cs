using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace aspnetcore_web_api.Controllers
{
    [Route("mobileauth")]
    [ApiController]
    public class MobileAuthController : ControllerBase
    {
        const string callbackScheme = "xamarinessentials";

        [HttpGet("{scheme}")]
        public async Task Get([FromRoute] string scheme)
        {
            var auth = await Request.HttpContext.AuthenticateAsync(scheme);

            if (!auth.Succeeded || auth?.Principal == null || !auth.Principal.Identities.Any(id => id.IsAuthenticated) 
                                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
            {
                // Not authenticated, challenge
                await Request.HttpContext.ChallengeAsync(scheme);
            }
            else
            {
                var claims = auth.Principal.Identities.FirstOrDefault()?.Claims;
                var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
                var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

                // Get parameters to send back to the callback
                var qs = new Dictionary<string, string>
                {
                    { "name", name },
                    { "email", email },
                    { "access_token", auth.Properties.GetTokenValue("access_token") },
                    { "refresh_token", auth.Properties.GetTokenValue("refresh_token") ?? string.Empty },
                    { "expires_in", auth.Properties.ExpiresUtc.ToString() }
                };

                // Build the result url
                var url = callbackScheme + "://#" + string.Join("&", qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                                                                       .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                // Redirect to final url
                Request.HttpContext.Response.Redirect(url);
            }
        }
    }
}
