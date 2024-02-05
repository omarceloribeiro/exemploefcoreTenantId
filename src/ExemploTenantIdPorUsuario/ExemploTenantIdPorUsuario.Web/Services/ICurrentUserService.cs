namespace ExemploTenantIdPorUsuario.Web.Services
{
    public interface ICurrentUserService
    {
        Guid? TenantId { get; }
        AppUserContext? GetUserContext();
    }
}
