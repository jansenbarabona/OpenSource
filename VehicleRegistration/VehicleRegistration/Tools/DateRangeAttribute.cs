using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Models
{
    public class DateRangeAttribute : ValidationAttribute
    {
        public DateRangeAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ErrorMessage);

            var dt_now = (DateTime)value;

            if (dt_now >= DateTime.Now.AddYears(-100) && dt_now <= DateTime.Now.AddYears(+10))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}