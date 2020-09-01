using Parking.Application.Validators;

namespace Parking.Application.Interfaces
{
    public interface IValidator<in T>
    {
        Validation IsValid(T input);
    }
}