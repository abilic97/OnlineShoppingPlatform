using OnlineShoppingPlatform.Data.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Data.Entities
{
    public class User : AuditableEntity
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }
        public string OAuthProvider { get; set; }
        public string OAuthId { get; set; }
        // Foreign key to UserRole
        public int UserRoleId { get; set; }

        // Navigation property
        public UserRole UserRole { get; set; }
    }
}
