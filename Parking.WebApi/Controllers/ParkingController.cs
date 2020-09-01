using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parking.Application.DTOs;
using Parking.Application.Interfaces;

namespace Parking.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : ControllerBase
    {
        private readonly IApplicationService _parkingService;
        private readonly ILogger<ParkingController> _logger;

        public ParkingController(ILogger<ParkingController> logger, IApplicationService parkingService)
        {
            _logger = logger;
            _parkingService = parkingService;
        }


        /// <summary>
        /// Calculate parking fee
        /// </summary>
        /// <remarks>Gets Enter and Exit date/time and calculate the payable amount</remarks>
        /// <response code="200">Successful response - returns payable amount and rate name</response>
        /// <param name="entryDate">in format of dd/MM/yyyy hh:mm</param>
        /// <param name="exitDate">in format of dd/MM/yyyy hh:mm</param>
        [HttpGet]
        [Route("Calculate")]
        public string Calculate(DateTime entryDate, DateTime exitDate)
        {
            var input = new List<string>() { entryDate.ToString("dd/MM/yyyy hh:mm"), exitDate.ToString("dd/MM/yyyy hh:mm") };
            var timer = new TimerDto();

            var valid = _parkingService.ValidateInput(input, out timer);
            if (!valid.IsValid)
            {
                throw new Exception(valid.ErrorMessage);
            }

            try
            {
                var response = _parkingService.CalculateAsync(timer).Result;

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
