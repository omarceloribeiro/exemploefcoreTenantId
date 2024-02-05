namespace ExemploTenantIdPorUsuario.Web.Services
{
    public interface ICurrentUserService
    {
        AppUserContext? GetUserContext();
    }
}
