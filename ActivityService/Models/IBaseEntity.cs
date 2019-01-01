using System;

namespace ActivityService.Models
{
    public interface IBaseEntity
    {
        string Id { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}