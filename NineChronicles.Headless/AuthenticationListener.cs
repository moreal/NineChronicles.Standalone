using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace NineChronicles.Headless
{
    public class AuthenticationListener : IOperationMessageListener
    {
        public static readonly string PRINCIPALKEY = "User";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationListener(IHttpContextAccessor contextAccessor)
        {
            this._httpContextAccessor = contextAccessor;
        }

        private ClaimsPrincipal? BuildClaimsPrincipal()
        {
            // Your code here
            // A user context builder can be included via constructor injection,
            //  and possibly the Authorization payload can be set (hacked)
            //  to the Authorization header on the http context
            return null;
        }

        public Task BeforeHandleAsync(MessageHandlingContext context)
        {
            if (context.Message.Type == MessageType.GQL_CONNECTION_INIT)
            {
                var payload = context.Message.Payload as JObject;

                if (payload != null && payload.ContainsKey("Authorization"))
                {
                    var auth = payload.Value<string>("Authorization");

                    // Save the user to the http context
                    _httpContextAccessor.HttpContext.User = BuildClaimsPrincipal();
                }
            }

            // Always insert the http context user into the message handling context properties
            // Note: any IDisposable item inside the properties bag will be disposed after this message is handled!
            //  So do not insert such items here, but use something like 'context[PRINCIPAL_KEY] = [...]'
            context.Properties[PRINCIPALKEY] = _httpContextAccessor.HttpContext.User;

            return Task.CompletedTask;
        }

        public Task HandleAsync(MessageHandlingContext context) => Task.CompletedTask;
        public Task AfterHandleAsync(MessageHandlingContext context) => Task.CompletedTask;
    }
}
