using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using challenge.Models;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        List<Compensation> GetCompensationsById(String id);
        Compensation Add(Compensation compensation);
        Task SaveAsync();
    }
}