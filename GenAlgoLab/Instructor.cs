using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    public class Instructor
    {
        readonly int InstructorID;
        public readonly string FullName;
        //Courses that this instructor can teach
        public HashSet<Course> CoursesQualifiesFor;
        public Instructor(int id)
        {
            InstructorID = id;
            FullName = id.ToString();
            CoursesQualifiesFor = new HashSet<Course>();
        }
        public Instructor(int id, string name) : this(id)
        {
            FullName = name;
        }
        public Instructor(int id, string name, IEnumerable<Course> courses) :this(id, name)
        {
            CoursesQualifiesFor = courses.ToHashSet();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, FullName: {1}", InstructorID, FullName);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Instructor objInst))
                return false;
            else
                return objInst.InstructorID == this.InstructorID && objInst.FullName == this.FullName;
        }
        public override int GetHashCode()
        {
            int hashCode = -230024762;
            hashCode = hashCode * -1521134295 + InstructorID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FullName);
            return hashCode;
        }
    }
}
