using ExemploTenantIdPorUsuario.Web.Models;
using ExemploTenantIdPorUsuario.Web.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExemploTenantIdPorUsuario.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
            ICurrentUserService currentUserService
            )
            : base(options)
        {
            this.currentUserService = currentUserService;
        }

        public DbSet<Categoria> Categorias { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (currentUserService != null)
            {
                var appUserContext = currentUserService.GetUserContext();
                if (appUserContext != null && appUserContext.TenantId.HasValue)
                {
                    Guid tenantId = appUserContext.TenantId.Value;
                    builder.Entity<Categoria>().HasQueryFilter(x => x.TenantId == tenantId);
                }
                
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
