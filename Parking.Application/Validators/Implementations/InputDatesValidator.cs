using System;
using Parking.Application.DTOs;
using Parking.Application.Interfaces;

namespace Parking.Application.Validators.Implementations
{
    public class InputDatesValidator : IValidator<TimerDto>
    {
        public Validation IsValid(TimerDto input)
        {
            var result = new Validation() { IsValid = true };

            var start = Convert.ToDateTime(input.Entry);
            var end = Convert.ToDateTime(input.Exit);

            if (end <= start)
            {
                result.IsValid = false;
                result.ErrorMessage = "End date time must be greater than start date time";
            }

            return result;
        }
    }
}
