using System;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{

    public class ReportingStructureService : IReportingStructureService
    {
        private readonly IReportingStructureRepository _reportingStructureRepository;
        private readonly ILogger<ReportingStructureService> _logger;

        public ReportingStructureService(ILogger<ReportingStructureService> logger, IReportingStructureRepository reportingStructureRepository)
        {
            _reportingStructureRepository = reportingStructureRepository;
            _logger = logger;
        }
        
        public ReportingStructure GetById(String id) 
        { 
            if(!String.IsNullOrEmpty(id))
            {
                var employee = _reportingStructureRepository.GetEmployeeWithReportById(id);

                if (null != employee)
                {
                    Int32 numberOfReports = CountReports(employee);
                    // Uncomment this line to exclude the direct reports in the returned employee
                    //employee.DirectReports = null;
                    return new ReportingStructure { Employee = employee, NumberOfReports = numberOfReports};
                }
            }

            return null;
        }

        private Int32 CountReports(Employee employee)
        {
            Int32 count = 0;

            if (null != employee?.DirectReports)
            {
                foreach (var directReport in employee.DirectReports)
                {
                    count++;
                    count += CountReports(directReport);
                }
            }

            return count;
        }
    }
}
