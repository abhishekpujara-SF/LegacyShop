using System;
using System.Diagnostics;
using System.Web;

namespace LegacyShop.Web.Infrastructure
{
    /// <summary>Classic IHttpModule that stamps each response with its server time.</summary>
    public class RequestTimingModule : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.BeginRequest += (s, e) => HttpContext.Current.Items["sw"] = Stopwatch.StartNew();
            app.PreSendRequestHeaders += (s, e) =>
            {
                var ctx = ((HttpApplication)s).Context;
                var sw = ctx.Items["sw"] as Stopwatch;
                if (sw != null) ctx.Response.Headers["X-Elapsed-Ms"] = sw.ElapsedMilliseconds.ToString();
            };
        }

        public void Dispose() { }
    }
}
