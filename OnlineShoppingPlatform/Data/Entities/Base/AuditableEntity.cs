using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Data.Entities.Base
{
    public abstract class AuditableEntity
    {
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }

        [StringLength(100)]
        public required string CreatedBy { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
