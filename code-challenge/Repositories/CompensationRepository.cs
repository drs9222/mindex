// unset

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Data;
using challenge.Models;
using Microsoft.Extensions.Logging;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public CompensationRepository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }
        
        public Compensation Add(Compensation compensation)
        {
            _employeeContext.Compensations.Add(compensation);

            return compensation;
        }
        public List<Compensation> GetCompensationsById(String id) 
        {
            return _employeeContext.Compensations.Where(c => c.Employee == id).ToList();
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
        
    }
}