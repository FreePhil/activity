using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityService.Models;

namespace ActivityService.Services
{
    public interface ICacheFiller
    { 
        Task<IList<EducationLevel>> Load();
    }
}