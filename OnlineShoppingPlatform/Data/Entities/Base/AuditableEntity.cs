using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Data.Entities.Base
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string CreatedBy { get; set; } = "System";

        [StringLength(100)]
        public string? UpdatedBy { get; set; } = "Sytem";
    }
}
