
using ExemploTenantIdPorUsuario.Web.Data;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;
using System.Security.Claims;

namespace ExemploTenantIdPorUsuario.Web.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Guid? TenantId
        {
            get
            {
                string? tenantId = httpContextAccessor?.HttpContext?.User?.FindFirstValue("TenantId");
                if (tenantId != null)
                {
                    return Guid.Parse(tenantId);
                }

                return null;
            }
        }

        public AppUserContext? GetUserContext()
        {
            if (httpContextAccessor?.HttpContext?.User?.Identity == null)
                return null;

            var user = httpContextAccessor.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
                return null;

            AppUserContext result = new AppUserContext()
            {
                Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
                TenantId = user.FindFirstValue("TenantId") != null ? Guid.Parse(user.FindFirstValue("TenantId")!) : null,
                Name = user.FindFirstValue(ClaimTypes.Name),
                Etc = "etc values"

            };

            return result;
        }
    }

    public class AppUserContext
    {
        public string? Id { get; set; }
        public Guid? TenantId { get; set; }
        public string? Name { get; set; }
        public string? Etc { get; set; }
    }
}
