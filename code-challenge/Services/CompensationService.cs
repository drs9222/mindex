using System;
using System.Collections.Generic;
using challenge.Models;
using challenge.Repositories;
using Microsoft.Extensions.Logging;

namespace challenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;
        private readonly IEmployeeRepository _employeeRepository;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _compensationRepository = compensationRepository;
            _logger = logger;
        }
        public List<Compensation> GetCompensationsById(String id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _compensationRepository.GetCompensationsById(id);
            }

            return new List<Compensation>();
        }
        
        public Compensation Create(Compensation compensation) {
            if (compensation != null)
            {
                // Make sure the employee exists since the in memory provider does not enforce a required foreign key
                var employee = _employeeRepository.GetById(compensation.Employee);
                if (employee == null) return null;

                _compensationRepository.Add(compensation);
                _compensationRepository.SaveAsync().Wait();
            }

            return compensation;
        }
    }
}