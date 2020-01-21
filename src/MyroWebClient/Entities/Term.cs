using System.Collections.Generic;

namespace MyroWebClient.Entities
{
    public class Term
    {
        public Term() {}

        public Term(List<Course> courses)
        {
            Courses = courses;
        }

        public string Name { get; set; }
        public decimal ObtainedScore { get; set; }
        public decimal AverageScore { get; set; }
        public List<Course> Courses { get; set; }
    }
}
