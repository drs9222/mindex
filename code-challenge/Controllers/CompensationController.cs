using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{

    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _logger.LogDebug($"Received compensation create request for '{compensation.Employee}'");


            compensation = _compensationService.Create(compensation);
            if (compensation == null)  return NotFound();

            return CreatedAtRoute("getCompensationsById", new { id = compensation.Employee }, compensation);
        }

        [HttpGet("{id}", Name = "getCompensationsById")]
        public IActionResult GetCompensationsById(String id)
        {
            _logger.LogDebug($"Received compensations get request for '{id}'");

            var compensations = _compensationService.GetCompensationsById(id);

            if (compensations == null) return NotFound();

            return Ok(compensations);
        }
    }
}
