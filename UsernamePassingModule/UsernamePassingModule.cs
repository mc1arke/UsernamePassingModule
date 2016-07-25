using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Security.Principal;
using System.Web;

namespace UsernamePassingModule
{
/// <summary>
/// <para>IIS contains a URLRewirite module which allows requests to be modified before being passed to the final processing endpoint, such as adding parameters or headers to the request.
/// However, the rewrite content and target is created before the Authenticate step in the request pipeline, meaning any details obtained during authentication - such as the user's
/// username - cannot be passed to the target of the rewrite. This module runs between the authenticate step and the execution of the rewrite, adding an HTTP header to the
/// rewritten request content, containing the principal name of the currently authenticated user (or null if no user is authenticated). This allows any downstream server to retrieve
/// the currently authenticated user from the request by reviewing the request headers, without having to understand Kerberos, NTLM or some other authentication mechanism details that
/// may be held in the forwarded request. The name of the request header is configurable in Web.config (or App.config) under the property 'headerName' within the 'usernamePassingModule',
/// with the name defaulting to 'X-Remote-User' if the configuration doesn't exist or is invalid.</para>
/// 
/// <para>WARNING: This module does not provide any way for the downstream server to identify this header has originated from this module: someone who can run direct requests to the backend
/// server could inject their own HTTP header matching the name being passed by this module thereby spoofing an authenticated login. It is therefore adivseable that anyone using this module
/// ensure that communication with a backend server is restricted so the only source location is any IIS server running as a proxy with this module enabled.</para>
/// </summary>
    public class UsernamePassingModule : IHttpModule
    {

        private string headerName = "X-Remote-User";

        public void Init(HttpApplication context)
        {
            // check Web.config (or App.config if you've managed to run this as a Standalone App!) for a section within the 'config' XML section named 'UsernamePassingModule'
            object configurationSection = ConfigurationManager.GetSection("usernamePassingModule");

            // check if the section exists, and is a NameValueCollection (users can configure it as something else, but we don't support that)
            if (null != configurationSection && configurationSection is NameValueCollection) {

                // retrieve the value of the property named 'headerName' from the configuration section
                string potentialHeaderName = ((NameValueCollection) configurationSection)["headerName"];

                // check the property value exists and isn't empty, then set it to be used as the header name in all forwarded requests
                if (!string.IsNullOrEmpty(potentialHeaderName)) {
                    headerName = potentialHeaderName;
                }
            }

            // add a call to ProcessRequest into the pipeline for each HTTP request after the authentication setp
            context.PostAuthenticateRequest += ProcessRequest;
        }

        public void Dispose()
        {
            //no-op
        }

        private void ProcessRequest(object source, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            IPrincipal user = context.User;

            /*
             * Default to a null username. If user is not authenticated then 'user' will be null so we want
             * to make it clear we have no username available. Otherwise, use the name from the principal the
             * authentication process has assigned to this request.
             */
            string username = null;
            if (null != user)
            {
                username = user.Identity.Name;
            }
            
            // add the username (possibly null if user is unauthenticated) to the request under the currently configured headername:
            // either set by the user in Web.config, or defaulting to X-Remote-User if the settings are invalid or undefined
            context.Request.Headers.Add(headerName, username);
        }


    }

}

