using System.Collections.Generic;

namespace MyroWebClient.Entities
{
    public class Course
    {
        public string Name { get; set; }
        public string Teacher { get; set; }
        public decimal ObtainedScore { get; set; }
        public decimal AverageScore { get; set; }
        public List<Test> Tests { get; set; }
    }
}
