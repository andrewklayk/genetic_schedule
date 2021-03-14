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
        public string Name;
        //Student count
        public int Capacity;
        //Lowest amount of hours the course should have for each group
        public int MinHours;
        public List<ConstraintBase> Constraints { get; }
        public Course(ushort _id, string name, int cap, int minHours)
        {
            CourseID = _id;
            Name = name;
            Capacity = cap;
            MinHours = minHours;
            Constraints = new List<ConstraintBase>
            {
                new RoomCapacityConstraint(this, cap)
            };
        }
        public void AddConstraint(ConstraintBase c)
        {
            Constraints.Add(c);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, Cap: {2}", CourseID, Name, Capacity);
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            return ((Course)obj).CourseID == CourseID;
        }
        public override int GetHashCode()
        {
            return CourseID.GetHashCode() ^ Name.GetHashCode();
            //return CourseID ^ Name.ToCharArray().Sum(x=>x.GetHashCode());
        }
    }
}
