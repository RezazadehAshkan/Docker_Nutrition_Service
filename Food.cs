using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContainerProd.Models
{
    public class Food
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int Number { get; set; }

        public string DayOfWeek { get; set; }

        public string Name { get; set; }
        
        public string Meal { get; set; }

        public int Price { get; set; }
    }
}
