﻿using ECommerceSite.SingleProjectSolution.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSite.SingleProjectSolution.Models
{
    [Table("Categories", Schema = "Store")]
    public class Category : EntityBase
    {
        [DataType(DataType.Text), MaxLength(50)]
        public string CategoryName { get; set; }

        [InverseProperty(nameof(Product.Category))]
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
