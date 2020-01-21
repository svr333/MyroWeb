using System.Collections.Generic;

namespace MyroWebClient.Entities
{
    public class Grades
    {
        public Grades() {}

        public Grades(List<Term> terms)
        {
            Terms = terms;
        }

        public List<Term> Terms { get; set; }
    }    
}
