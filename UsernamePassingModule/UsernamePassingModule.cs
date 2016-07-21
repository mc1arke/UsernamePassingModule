using System;
using System.Security.Principal;
using System.Web;

namespace UsernamePassingModule
{
/// <summary>
/// <para>IIS contains a URLRewirite module which allows requests to be modified before being passed to the final processing endpoint, such as adding parameters or headers to the request.
/// However, the rewrite content and target is created before the Authenticate step in the request pipeline, meaning any details obtained during authentication - such as the user's
/// username - cannot be passed to the target of the rewrite. This module runs between the authenticate step and the execution of the rewrite, adding an 'X-REMOTE-USER' header to the
/// rewritten request content, containing the principal name of the currently authenticated user (or null if no user is authenticated). This allows any downstream server to retrieve
/// the currently authenticated user from the request by reviewing the request headers, without having to understand Kerberos, NTLM or some other authentication mechanism details that
/// may be held in the forwarded request.</para>
/// 
/// <para>WARNING: This module does not provide any way for the downstream server to identify this header has originated from this module: someone who can run direct requests to the backend
/// server could inject their own HTTP 'X-Remote-User' header thereby spoofing an authenticated login. It is therefore adivseable that anyone using this module ensure that communication
/// with a backend server is restricted so the only source location is any IIS server running as a proxy with this module enabled.</para>
/// </summary>
    public class UsernamePassingModule : IHttpModule
    {

        public void Init(HttpApplication context)
        {
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
            
            // add the username (possibly null if user is unauthenticated) to the request under the 'X-Remote-User' header
            context.Request.Headers.Add("X-Remote-User", username);
        }


    }

}

