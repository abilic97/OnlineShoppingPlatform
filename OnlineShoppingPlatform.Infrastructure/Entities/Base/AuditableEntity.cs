using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Infrastructure.Entities.Base
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string CreatedBy { get; set; } = "System Admin";

        [StringLength(100)]
        public string? UpdatedBy { get; set; } = "Sytem Admin";
    }
}
