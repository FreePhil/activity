using System;

namespace ActivityService.Models
{
    public interface IServiceEntity
    {
        string Id { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}