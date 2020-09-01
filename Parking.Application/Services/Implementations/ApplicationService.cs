using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parking.Application.Interfaces;
using Parking.Domain.Models;
using Parking.Infrastructure.Interfaces;
using Parking.Application.Validators;
using Parking.Application.DTOs;

namespace Parking.Application.Implementations
{
    public class ApplicationService : IApplicationService
    {
        private readonly IEnumerable<IValidator<string>> _inputValidators;
        private readonly IEnumerable<IValidator<TimerDto>> _datesValidators;
        private readonly IRepository<Normal> _normalRepository;
        private readonly IRepository<Special> _specialRepository;

        public ApplicationService(
            IEnumerable<IValidator<string>> inputValidators,
            IEnumerable<IValidator<TimerDto>> datesValidators,
            ISpecialRepository specialRepository, 
            INormalRepository normalRepository
            )
        {
            _inputValidators = inputValidators;
            _datesValidators = datesValidators;
            _normalRepository = normalRepository;
            _specialRepository = specialRepository;
        }

        public async Task<string> CalculateAsync(TimerDto input)
        {
            var response = await calculate(input);
            var result = $"\nRate : {response.Name}\nPayable amount : {response.Price.ToString("C")}";

            return result;
        }
        
        public Validation ValidateInput(IList<string> input, out TimerDto timer)
        {
            timer = new TimerDto();

            foreach (var validator in _inputValidators)
            {
                foreach (var i in input)
                {
                    var r = validator.IsValid(i);
                    if (!r.IsValid)
                    {
                        return r;
                    }
                }
            }

            timer.Entry = Convert.ToDateTime(input.ElementAt(0));
            timer.Exit = Convert.ToDateTime(input.ElementAt(1));
            
            foreach (var validator in _datesValidators)
            {
                var r = validator.IsValid(timer);
                if (!r.IsValid)
                {
                    return r;
                }
            }
           
            return new Validation() {IsValid = true};
        }

        //Todo: transfer the calculation logic to the Domain project (ParkingManager)
        private async Task<ParkingRateDto> calculate(TimerDto input)
        {
            var normalRates = await _normalRepository.SelectAllAsync();
            var specialRates = await _specialRepository.SelectAllAsync();

            var result = new ParkingRateDto();

            var resultSpecial = calculateSpecial(specialRates, input.Entry, input.Exit);
            result = resultSpecial;

            var resultNormal = calculateNormal(normalRates, input.Entry, input.Exit);

            if (resultNormal.Price > 0 && (result.Price == 0 || result.Price > resultNormal.Price))
            {
                result.Name = resultNormal.Name;
                result.Price = resultNormal.Price;
            }

            return result;
        }

        private ParkingRateDto calculateSpecial(IEnumerable<Special> specialRates, DateTime start, DateTime end)
        {
            var result = new ParkingRateDto();

            foreach (var specialRate in specialRates)
            {
                var counter = 0;

                // Entry
                bool isSpecial = (specialRate.Entry.Start <= start.TimeOfDay && start.TimeOfDay <= specialRate.Entry.End) ||
                                 (specialRate.MaxDays > 0 &&
                                  (specialRate.Entry.Start <= start.TimeOfDay &&
                                   start.TimeOfDay <= specialRate.Entry.End.Add(TimeSpan.FromDays(1))) ||
                                  (specialRate.Entry.Start.Subtract(TimeSpan.FromDays(1)) <= start.TimeOfDay &&
                                   start.TimeOfDay <= specialRate.Entry.End));


                if (
                    !specialRate.Entry.Days.Any(
                        d => string.Equals(d, start.DayOfWeek.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    isSpecial = false;
                }

                var maxExitDay = start.AddDays(specialRate.MaxDays);
                var maxExit = new DateTime(maxExitDay.Year, maxExitDay.Month, maxExitDay.Day, specialRate.Exit.End.Hours,
                    specialRate.Exit.End.Minutes, 0);
                if (end > maxExit)
                {
                    isSpecial = false;
                }

                if (!specialRate.Exit.Days.Any(
                        d => string.Equals(d, end.DayOfWeek.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    isSpecial = false;
                }

                if ((end - start).Days > specialRate.MaxDays)
                {
                    isSpecial = false;
                }

                if (isSpecial)
                {
                    if (result.Price == 0 || result.Price > specialRate.TotalPrice)
                    {
                        result.Name = specialRate.Name;
                        result.Price = specialRate.TotalPrice;
                    }
                }
            }

            return result;

        }

        private ParkingRateDto calculateNormal(IEnumerable<Normal> normalRates, DateTime start, DateTime end)
        {
            var result = new ParkingRateDto() { Name = CONSTANTS.RATE.STANDARD };

            var resultNormalMacro = 0.0;
            var resultNormalMicro = 0.0;
            var isNormal = false;
            var duration = (end - start).TotalHours;
            var maxNormalRate = normalRates.OrderBy(nr => nr.MaxHours).LastOrDefault();

            // More than max duration
            if (duration >= maxNormalRate.MaxHours)
            {
                resultNormalMacro = Math.Floor(duration / maxNormalRate.MaxHours) * maxNormalRate.Rate;
                duration = duration % maxNormalRate.MaxHours;
            }
            if (duration > 0)
            {
                foreach (var normalRate in normalRates)
                {
                    if (!isNormal && duration <= normalRate.MaxHours)
                    {
                        isNormal = true;
                        resultNormalMicro = normalRate.Rate;
                    }
                }
            }

            result.Price = resultNormalMacro + resultNormalMicro;

            return result;
        }
    }
}
