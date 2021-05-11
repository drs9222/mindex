using System;
using System.ComponentModel.DataAnnotations;

namespace challenge.Models
{
    public class SalaryAttribute : ValidationAttribute
    {
        public SalaryAttribute() : base("{0} must be a non-negative decimal value.") { }

        private Boolean IsValid(Decimal value)
        {
            // Only allow non-negative salaries
            return value >= 0;
        }

        /// <inheritdoc />
        public override Boolean IsValid(Object value)
        {
            return TryConvertToDecimal(value, out Decimal decimalValue) && IsValid(decimalValue);
        }

        private Boolean TryConvertToDecimal(Object value, out Decimal decimalValue)
        {
            decimalValue = default;

            if (null != value)
            {
                try
                {
                    decimalValue = Convert.ToDecimal(value);

                    return true;
                }
                catch (FormatException) { }
                catch (InvalidCastException) { }
                catch (NotSupportedException) { }
            }

            return false;
        }
    }
}