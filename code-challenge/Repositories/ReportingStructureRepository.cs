using System;
using System.Collections.Generic;
using System.Linq;
using challenge.Data;
using challenge.Models;
using Microsoft.Extensions.Logging;

namespace challenge.Repositories
{
    public class ReportingStructureRepository : IReportingStructureRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IReportingStructureRepository> _logger;

        public ReportingStructureRepository(ILogger<IReportingStructureRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee GetEmployeeWithReportById(String id)
        {
            var employee = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
            if (null == employee) return null;

            LoadDirectReports(employee, new List<Employee>());
            return employee;
        }

        private void LoadDirectReports(Employee employee, List<Employee> accessedEmployees)
        {
            if (accessedEmployees.Contains(employee))
            {
                throw new InvalidOperationException($"Circular reporting structure found when loading reports for {accessedEmployees.First().EmployeeId}. Duplicate encountered with {employee.EmployeeId}.");
            }

            accessedEmployees.Add(employee);

            employee.DirectReports = _employeeContext.Employees
                .Where(e => e.EmployeeId == employee.EmployeeId)
                .Select(e => e.DirectReports)
                .SingleOrDefault()?.ToList();

            if (null != employee.DirectReports)
            {
                foreach (var directReport in employee.DirectReports)
                {
                    LoadDirectReports(directReport, accessedEmployees);
                }
            }
        }
    }
}
