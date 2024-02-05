using System.ComponentModel.DataAnnotations;

namespace ExemploTenantIdPorUsuario.Web.Models
{
    public class Categoria
    {
        public required int Id { get; set; }
        [StringLength(50)]
        public required string Nome { get; set; }
        public required Guid TenantId { get; set; }
    }
}
