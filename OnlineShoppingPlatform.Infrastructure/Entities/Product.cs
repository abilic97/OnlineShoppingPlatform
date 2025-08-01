﻿using OnlineShoppingPlatform.Infrastructure.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingPlatform.Infrastructure.Entities
{
    public class Product : AuditableEntity
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsInStock { get; set; }
    }
}
