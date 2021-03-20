using System;
using System.Collections.Generic;
using System.Linq;

namespace GenAlgoLab
{
    public class Group : IComparable
    {
        private readonly int GroupID;
        public readonly string groupName;
        //Courses assigned to this group
        public HashSet<Course> courses;
        public Group(int id, string name)
        {
            GroupID = id;
            groupName = name;
            courses = new HashSet<Course>();
        }
        public Group(int id, string name, IEnumerable<Course> courses) : this(id, name)
        {
            this.courses = courses.ToHashSet();
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            if (!(obj is Group otherGroup))
                throw new ArgumentException("Object is not Group!");
            return GroupID.CompareTo(otherGroup.GroupID);
        }
        public override bool Equals(object obj)
        {
            return obj is Group group &&
                   GroupID == group.GroupID &&
                   groupName == group.groupName;
        }
        public override int GetHashCode()
        {
            int hashCode = -1062870623;
            hashCode = hashCode * -1521134295 + GroupID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(groupName);
            return hashCode;
        }
    }
}
