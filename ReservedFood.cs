using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerProd.Models
{
    public class ReservedFood
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FoodId { get; set; }

        public long StudentNumber { get; set; }

        public DateTime ReserveDate { get; set; }

        public bool Status { get; set; }
    }
}
