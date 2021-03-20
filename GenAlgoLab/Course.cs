using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    public class Course
    {
        readonly ushort CourseID;
        //Subject name
        readonly public string Name;
        //Student count
        readonly public int Capacity;
        //Lowest amount of hours the course should have for each group
        readonly public int MinHours;
        public Course(ushort _id, string name, int cap, int minHours)
        {
            CourseID = _id;
            Name = name;
            Capacity = cap;
            MinHours = minHours;
        }
        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, Cap: {2}", CourseID, Name, Capacity);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Course objCourse))
                return false;
            else
                return objCourse.CourseID == this.CourseID;
        }

        public override int GetHashCode()
        {
            int hashCode = -1506781142;
            hashCode = hashCode * -1521134295 + CourseID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Capacity.GetHashCode();
            hashCode = hashCode * -1521134295 + MinHours.GetHashCode();
            return hashCode;
        }
    }
}
