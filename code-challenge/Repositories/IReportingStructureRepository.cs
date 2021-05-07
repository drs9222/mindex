using System;
using challenge.Models;

namespace challenge.Repositories
{
    public interface IReportingStructureRepository
    {
        Employee GetEmployeeWithReportById(String id);
    }
}
