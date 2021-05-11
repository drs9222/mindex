using System;
using System.Collections.Generic;
using challenge.Models;

namespace challenge.Services
{
    public interface ICompensationService
    {
        List<Compensation> GetCompensationsById(String id);
        Compensation Create(Compensation compensation);
    }
}