using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    public class Instructor
    {
        int InstructorID;
        public string FullName;
        public HashSet<Course> CoursesQualifiesFor;
        public Instructor(int id)
        {
            InstructorID = id;
            FullName = id.ToString();
            CoursesQualifiesFor = new HashSet<Course>();
        }
        public Instructor(int id, string name)
        {
            InstructorID = id;
            FullName = name;
            CoursesQualifiesFor = new HashSet<Course>();
        }
        public Instructor(int id, string name, HashSet<Course> courses)
        {
            InstructorID = id;
            FullName = name;
            CoursesQualifiesFor = courses;
        }

        public override string ToString()
        {
            return string.Format("(Id: {0}, FullName: {1})", InstructorID, FullName);
        }
    }
}
