using OnlineShoppingPlatform.Data.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Data.Entities
{
    public class UserRole : AuditableEntity
    {
        [Key]
        public int UserRoleId { get; set; }

        [Required]
        [StringLength(50)]
        public required string RoleName { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
