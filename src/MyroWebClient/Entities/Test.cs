using System;

namespace MyroWebClient.Entities
{
    public class Test
    {
        public Test() {}
        public Test(uint maxScore) 
        {

        }

        public string Name { get; set; }
        public string Comment { get; set; }
        public decimal ObtainedScore { get; set; }
        public decimal AverageScore { get; set; }
        public decimal MaxScore { get; set; }
        public DateTime Date { get; set; }
    }
}
