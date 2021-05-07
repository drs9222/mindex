using System;
using challenge.Models;

namespace challenge.Services
{
    public interface IReportingStructureService
    {
        ReportingStructure GetById(String id);
    }
}