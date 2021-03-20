using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    public enum ClassTypes
    {
        Lec,
        Lab
    }
    public class Course
    {
        private ushort CourseID;
        //Subject name
        public readonly string name;
        //Type: lecture or lab
        public readonly ClassTypes classType;
        //Student count
        public readonly int capacity;
        //Lowest amount of hours the course should have for each group
        public readonly int MinHours;
        public Course(ushort _id, string _name, int _capacity, int minHours, ClassTypes _type = ClassTypes.Lab)
        {
            CourseID = _id;
            name = _name;
            capacity = _capacity;
            MinHours = minHours;
            classType = _type;
        }
        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, Cap: {2}", CourseID, name, capacity);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Course objCourse))
                return false;
            else
                return objCourse.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hashCode = -1506781142;
            hashCode = hashCode * -1521134295 + CourseID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + capacity.GetHashCode();
            hashCode = hashCode * -1521134295 + classType.GetHashCode();
            hashCode = hashCode * -1521134295 + MinHours.GetHashCode();
            return hashCode;
        }
    }
}
