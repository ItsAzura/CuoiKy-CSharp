using Microsoft.AspNetCore.Authorization;

namespace CK_CSharp.Service
{
    public class AdminRequirements : IAuthorizationRequirement
    {
    }
    public class AdminRequirementHandler : AuthorizationHandler<AdminRequirements>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirements requirement)
        {
            var claims = context.User.Claims;
            return Task.CompletedTask;
        }
    }
}
