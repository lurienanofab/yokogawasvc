using System;

namespace YokogawaService.Models
{
    public class DataQueryCriteria
    {
        public string HeaderPattern { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
