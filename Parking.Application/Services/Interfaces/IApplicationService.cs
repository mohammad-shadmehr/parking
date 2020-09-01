using System.Collections.Generic;
using System.Threading.Tasks;
using Parking.Application.DTOs;
using Parking.Application.Validators;

namespace Parking.Application.Interfaces
{
    public interface IApplicationService
    {
        Validation ValidateInput(IList<string> input, out TimerDto timer);
        Task<string> CalculateAsync(TimerDto input);
    }
}
