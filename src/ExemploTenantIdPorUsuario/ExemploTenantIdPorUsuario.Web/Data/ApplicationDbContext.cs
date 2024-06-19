using ExemploTenantIdPorUsuario.Web.Models;
using ExemploTenantIdPorUsuario.Web.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExemploTenantIdPorUsuario.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService currentUserService;
        private Guid? tenantId;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService
            )
            : base(options)
        {
            this.currentUserService = currentUserService;
            this.tenantId = currentUserService.TenantId;
        }

        public DbSet<Categoria> Categorias { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // explicacao: se passar uma propriedade com get; para o valor de comparacao, o ef vai acionar o get no momento da chamada, mesmo que seja null incialmente, ele vai chamar o get novamente e pegar o valor no momento da query
            // ja se primeiro atribuir esse valor para uma variavel, e depois passar na condicao, dai o ef core vai deixar chumbado em toda vida da aplicacao

            int opcao = 4;

            if (opcao == 1)
            {
                // nao funciona
                Guid? teste = currentUserService.TenantId;
                builder.Entity<Categoria>().HasQueryFilter(x => x.TenantId == teste);
            }

            if (opcao == 2)
            {
                // funciona
                builder.Entity<Categoria>().HasQueryFilter(x => x.TenantId == currentUserService.TenantId);
            }

            if (opcao == 3)
            {
                // nao funciona
                var appUserContext = currentUserService.GetUserContext();
                if (appUserContext != null && appUserContext.TenantId.HasValue)
                {
                    Guid tenantId = appUserContext.TenantId.Value;
                    builder.Entity<Categoria>().HasQueryFilter(x => x.TenantId == tenantId);
                }
            }
            if (opcao == 4)
            {
                // nao funciona
                AppUserContext appUserContext = currentUserService.GetUserContext();
                builder.Entity<Categoria>().HasQueryFilter(x => x.TenantId == appUserContext.TenantId);
                
            }

            if (opcao == 5)
            {
                // tambem funciona chamando uma funcao direto e em seguinda chamando a propriedade.resumindo precisa ser uma funcao
                builder.Entity<Categoria>().HasQueryFilter(x => x.TenantId == currentUserService.GetUserContext().TenantId);
            }

            if (opcao == 6)
            {
                // variavel (sem get) no contexto da classe
                // nao funciona
                builder.Entity<Categoria>().HasQueryFilter(x => x.TenantId == tenantId);
            }

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var addedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added).ToList();

            addedEntities.ForEach(entityEntry =>
            {


                if (entityEntry.Entity is Categoria categoria)
                {
                    ArgumentNullException.ThrowIfNull(currentUserService);
                    var appUserContext = currentUserService.GetUserContext();
                    ArgumentNullException.ThrowIfNull(appUserContext);
                    ArgumentNullException.ThrowIfNull(appUserContext.TenantId);

                    categoria.TenantId = appUserContext.TenantId.Value;
                }
            });
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
