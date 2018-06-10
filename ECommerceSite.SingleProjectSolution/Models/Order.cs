using ECommerceSite.SingleProjectSolution.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceSite.SingleProjectSolution.Models
{
    [Table("Orders", Schema = "Store")]
    public class Order : EntityBase
    {
        public int CustomerId { get; set; }

        [DataType(DataType.Currency), Display(Name = "Total")]
        public decimal? OrderTotal { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Ordered")]
        public DateTime OrderDate { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Date Shipped")]
        public DateTime ShipDate { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public ApplicationUser Customer { get; set; }

        [InverseProperty(nameof(OrderDetail.Order))]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
