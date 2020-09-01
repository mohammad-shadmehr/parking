using System;
using Parking.Application.Interfaces;

namespace Parking.Application.Validators.Implementations
{
    public class InputDateValidator : IValidator<string>
    {
        public Validation IsValid(string input)
        {
            var result = new Validation() { IsValid = true };

            try
            {
                if (Convert.ToDateTime(input) == default(DateTime))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Invalid date";
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}
