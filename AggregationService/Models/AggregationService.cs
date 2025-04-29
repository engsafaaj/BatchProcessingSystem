using System;

namespace AggregationService.Models
{
    public class AggregatedResult
    {
        public int Id { get; set; }
        public int PULocationID { get; set; }
        public string Quarter { get; set; }
        public int Year { get; set; }
        public int TripCount { get; set; }
        public decimal TotalFareAmount { get; set; }
        public double AverageTripDistance { get; set; }
        public DateTime AggregationTime { get; set; }
    }
}
