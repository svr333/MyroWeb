using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MyroWebClient.Entities;

namespace MyroWebClient
{
    public class DataParser
    {
        private List<TempCourse> tempCourses = new List<TempCourse>();
        private List<TempTerm> tempTerms = new List<TempTerm>();

        public Grades ConvertDataToObject(string rawData)
        {
            var test = rawData.Split(new string[] { "", ""}, StringSplitOptions.RemoveEmptyEntries );
            var regex = new Regex("<tr class='h3 courseName'>");
            var splits = regex.Split(rawData);

            for (int i = 1; i < splits.Length; i++)
            {
                // textInLines => 1 course with all terms
                var column = new Regex("</tr>\n");
                var textInLines = column.Split(splits[i]);

                var courseAndTeacher = GetCourseName(textInLines);

                tempCourses.Add(new TempCourse
                {
                    Name = courseAndTeacher[0],
                    Teacher = courseAndTeacher[1],
                    Terms = GetTerms(textInLines)
                });
            }

            return CreateGradesObjectFromTempCourses(tempCourses);
        }

        private Grades CreateGradesObjectFromTempCourses(List<TempCourse> tempCourses)
        {
            List<Term> terms = new List<Term>(); 

            for (int i = 0; i < tempCourses.Count; i++)
            {
                var course = tempCourses[i];

                // ADD ALL TERMS THAT EXIST (WITHOUT COURSES)
                for (int j = 0; j < course.Terms.Count; j++)
                {
                    var term = course.Terms[j];

                    if (terms.Find(x => x.Name == term.Name) is null)
                    {
                        terms.Add(new Term()
                        {
                            Name = term.Name,
                            Courses = new List<Course>()
                        });
                    }
                }
            
                // ADD CURRENT COURSE TO EACH TERM (WITHOUT TESTS)
                for (int j = 0; j < terms.Count; j++)
                {
                    terms[j].Courses.Add(new Course()
                    {
                        Name = course.Name,
                        Teacher = course.Teacher,
                        Tests = new List<Test>()
                    });
                }

                // ADD SCORES TO COURSE FOR EACH TERM
                for (int j = 0; j < course.Terms.Count; j++)
                {
                    var currentTerm = course.Terms[j];

                    // ADD EACH TEST TO THE CORRECT COURSE FOR EACH TERM
                    for (int k = 0; k < currentTerm.Tests.Count; k++)
                    {
                        var currentTest = currentTerm.Tests[k];

                        var termCourse = terms.Find(x => x.Name == currentTerm.Name).Courses.Find(x => x.Name == course.Name);
                        termCourse.ObtainedScore = currentTerm.Score;
                        termCourse.AverageScore = currentTerm.Average;              

                        // ADD EACH TEST TO THE CORRECT COURSE FOR EACH TERM
                        termCourse.Tests.Add(new Test()
                        {
                            Name = currentTest.Name,
                            Comment = currentTest.Comment,
                            ObtainedScore = currentTest.ObtainedScore,
                            AverageScore = currentTest.AverageScore,
                            MaxScore = currentTest.MaxScore,
                            Date = currentTest.Date
                        });
                    }   
                }
            }

            return new Grades() { Terms = terms };
        }

        private List<TempTerm> GetTerms(string[] textInLines)
        {
            tempTerms = new List<TempTerm>();

            int currentTerm = -1;

            for (int i = 0; i < textInLines.Length; i++)
            {
                if (textInLines[i].StartsWith("<tr class=\"point reportPoint\">"))
                {
                    currentTerm++;
                    var chunks = textInLines[i].Split(new [] { "<", ">" }, StringSplitOptions.RemoveEmptyEntries);

                    AddTempTerm(chunks);
                }

                else if (textInLines[i].StartsWith("<tr class=\"point\">"))
                {
                    // very specific, example below:
                    // <tr class="point"><td class="title" style="padding-left:40px;">TASK: Creative writing: " For sale: tiny blue baby boots "</td><td class="point" title="70%">14</td><td class="max" title="70%">/20</td><td class="average" title="Gemiddelde">13,5</td><td class="comment">Original story, a pity you only handed it in at the end of the week.</td><td class="date">2019-11-18</td></tr>
                    var chunks = textInLines[i].Split(new [] { "<", ">" }, StringSplitOptions.RemoveEmptyEntries);

                    var testName = chunks[2];
                    Test test;

                    // if it contains an extra class, numbers will shift.
                    if (chunks[3] == "span class=\"Weight\"") test = MakeTestWithWeight(chunks);
                    else test = MakeTestWithoutWeight(chunks);

                    tempTerms[currentTerm].Tests.Add(test);
                    
                }
            }
            
            return tempTerms;
        }

        #region Helper Methods

        private Test MakeTestWithWeight(string[] chunks)
        => new Test()
        {
            Name = chunks[2],
            Comment = chunks[21],
            Date = DateTime.Parse(chunks[24]),
            ObtainedScore = decimal.Parse(FilterChunk(chunks[12])),
            AverageScore = decimal.Parse(FilterChunk(chunks[18])),
            MaxScore = decimal.Parse(FilterChunk(chunks[15]))
        };

        private Test MakeTestWithoutWeight(string[] chunks)
        => new Test()
        {
            Name = chunks[2],
            Comment = chunks[14],
            Date = DateTime.Parse(chunks[17]),
            ObtainedScore = decimal.Parse(FilterChunk(chunks[5])),
            AverageScore = decimal.Parse(FilterChunk(chunks[11])),
            MaxScore = decimal.Parse(FilterChunk(chunks[8]))
        };

        private void AddTempTerm(string[] chunks)
        {
            var termName = chunks[2];      
            var obtainedScore = decimal.Parse(FilterChunk(chunks[5]));                    
            var averageScore = (obtainedScore == -1) ? decimal.Parse(FilterChunk(chunks[10])) : decimal.Parse(FilterChunk(chunks[11]));

            var date = (obtainedScore == -1) ? DateTime.Parse(chunks[16]) : DateTime.Parse(chunks[17]);
                    
            tempTerms.Add(new TempTerm()
            {
                Name = termName,
                Date = date,
                Score = obtainedScore,
                Average = averageScore,
                Tests = new List<Test>()
            });
        }

        private string FilterChunk(string chunk)
        {
            var filteredChunk = chunk.Replace(',', '.').Replace("/", "");
            if (filteredChunk == "&nbsp;") filteredChunk = "-1";
            return filteredChunk;
        }

        private string[] GetCourseName(string[] textInLines)
        {
            // course name is located on the first line, example below:
            // <td colspan='3' class='h3 line courseName'>Biologie (VAN PRAET J.)</td><td class='averageTitle line' colspan="3">Gem.</td></tr>
            var elements = textInLines[0].Split(new string[] { "<", ">" }, options: StringSplitOptions.RemoveEmptyEntries);
            return elements[1].Split(new string[] { "(", ")" }, options: StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region Temp structs

        private struct TempCourse
        {
            public string Name { get; set; }
            public string Teacher { get; set; }
            public List<TempTerm> Terms { get; set; }
        }

        private struct TempTerm
        {
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public decimal Score { get; set; }
            public decimal Average { get; set; }
            public List<Test> Tests { get; set; }
        }
        
        #endregion
    }
}
