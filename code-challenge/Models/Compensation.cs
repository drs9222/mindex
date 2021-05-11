using System;
using System.ComponentModel.DataAnnotations;

namespace challenge.Models
{
    public class Compensation
    {
        [Required]
        public String Employee { get; set; }
        
        [Required, Salary]
        public Decimal? Salary { get; set; }
        
        [Required, EffectiveDate]
        public DateTimeOffset? EffectiveDate { get; set; }
    }
}