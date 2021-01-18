using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface IJsonLoader
    { 
        Task<IList<EducationLevel>> Load();
    }
}