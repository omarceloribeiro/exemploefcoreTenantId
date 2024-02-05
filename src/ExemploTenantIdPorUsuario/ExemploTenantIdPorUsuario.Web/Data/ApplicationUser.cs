using Microsoft.AspNetCore.Identity;

namespace ExemploTenantIdPorUsuario.Web.Data
{
    public class ApplicationUser : IdentityUser
    {
        public required Guid TenantId { get; set; }
    }
}
