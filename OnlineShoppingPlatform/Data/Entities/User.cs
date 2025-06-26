using OnlineShoppingPlatform.Data.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatform.Data.Entities
{
    // Minimal example just to demonstrate the authentication and authorization requirements of the task.
    // Expanding this with values like shipping address, phone number etc. can be added 
    // so shipping information gets autofilled automatically when trying to order. Ofcourse,
    // these automatic values can be oveerriden (to be handeled on FE)
    public class User : AuditableEntity
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }
        public string OAuthProvider { get; set; }
        public string OAuthId { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}
