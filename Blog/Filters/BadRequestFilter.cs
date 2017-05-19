using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Filters
{
    /// <summary>
    /// A7  Security This filter checks every response, if the response code is greater than 400 
    /// it increments attempts allowed. Upon reaching the BadAttempts count it will start
    /// rejecting all requests.
    /// </summary>
    public class BadRequestFilter : ActionFilterAttribute
    {
        private const int DefaultMaxAttemptsAllowed = 10000;
        public int MaxAttemptsAllowed { get; set; }

        private static ConcurrentDictionary<string, int> BadAttempts { get; }

        static BadRequestFilter()
        {
            BadAttempts = new ConcurrentDictionary<string, int>();
        }

        public BadRequestFilter()
        {
            if (MaxAttemptsAllowed == 0)
            {
                MaxAttemptsAllowed = MaxAttemptsAllowed;
            }
        }


        /// <summary>
        /// A7 SECURITY this checks every incoming request, if the action exists in our bad request filter
        /// AND it exceeds our max attempts allowed we set the request to a bad result and return it to the 
        /// user.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var action = context.RouteData.Values["action"].ToString();
            int attempts;
            BadAttempts.TryGetValue(action, out attempts);
            if (attempts > MaxAttemptsAllowed)
            {
                context.Result = new ContentResult()
                {
                    Content = "max requests exceeded",
                    StatusCode = 400
                };
            }
            base.OnActionExecuting(context);
        }


        /// <summary>
        /// SECURITY A7 Checks to see if our response status code is greater than399. if it is, we update our bad attempts.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {

            int result;
            int.TryParse(context.HttpContext?.Response?.StatusCode.ToString(), out result);

            if (result > 399)
            {
                var action = context.RouteData.Values["action"].ToString();
                BadAttempts.AddOrUpdate(action, 1, (s, i) => i + 1);
            }
            base.OnActionExecuted(context);
        }
    }
}