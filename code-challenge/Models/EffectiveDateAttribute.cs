using System;
using System.ComponentModel.DataAnnotations;

namespace challenge.Models
{
    public class EffectiveDateAttribute : ValidationAttribute
    {
        public EffectiveDateAttribute() : base($"{{0}} must be after {_minDate:d}.") { }
        private static readonly DateTimeOffset _minDate = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private Boolean IsValid(DateTimeOffset value)
        {
            return value >= _minDate;
        }

        /// <inheritdoc />
        public override Boolean IsValid(Object value)
        {
            return TryConvertToDateTimeOffset(value, out DateTimeOffset dateTimeOffset) && IsValid(dateTimeOffset);
        }

        private Boolean TryConvertToDateTimeOffset(Object value, out DateTimeOffset dtoValue)
        {
            switch (value)
            {
                case DateTimeOffset dto:
                    dtoValue = dto;
                    return true;
                case DateTime dt:
                    dtoValue = dt;
                    return true;
                case String s when DateTimeOffset.TryParse(s, out dtoValue):
                    return true;
                default:
                    dtoValue = default;
                    return false;
            }
        }
    }
}