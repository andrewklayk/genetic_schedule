using System;
using System.Collections.Generic;

namespace GenAlgoLab
{
    public class CourseScheduleEntry
    {
        public Group group;
        public Course course;
        public byte day;
        public byte time;
        public Room room;
        public Instructor instructor;
        public CourseScheduleEntry(Group _group, Course _course)
        {
            group = _group;
            course = _course;
        }
        public CourseScheduleEntry(CourseScheduleEntry x)
        {
            group = x.group;
            course = x.course;
            instructor = x.instructor;
            time = x.time;
            day = x.day;
            room = x.room;
        }
        public bool HasUnqualifiedInstructor()
        {
            return !(instructor.CoursesQualifiesFor.Count == 0 || instructor.CoursesQualifiesFor.Contains(course));
        }
        public double GetCapacityPenaltyMultiplier()
        {
            if (course.capacity == room.Capacity)
                return 0;
            if (course.capacity > room.Capacity)
                return (float)course.capacity / room.Capacity;
            else
                return (float)room.Capacity / course.capacity;
        }
        public override bool Equals(object obj)
        {
            return obj is CourseScheduleEntry entry &&
                   group == entry.group &&
                   course.Equals(entry.course) &&
                   day == entry.day &&
                   time == entry.time &&
                   room.Equals(entry.room) &&
                   instructor.Equals(entry.instructor);
        }
        public override int GetHashCode()
        {
            int hashCode = -1715831775;
            hashCode = hashCode * -1521134295 + group.GetHashCode();
            hashCode = hashCode * -1521134295 + course.GetHashCode();
            hashCode = hashCode * -1521134295 + day.GetHashCode();
            hashCode = hashCode * -1521134295 + time.GetHashCode();
            hashCode = hashCode * -1521134295 + room.GetHashCode();
            hashCode = hashCode * -1521134295 + instructor.GetHashCode();
            return hashCode;
        }
    }
}
