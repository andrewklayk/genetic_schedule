using System;
using System.Collections.Generic;

namespace GenAlgoLab
{
    public class CourseScheduleEntry
    {
        public int groupNum;
        public Course course;
        public byte time;
        public Room room;
        public Instructor instructor;
        public CourseScheduleEntry(Course _course)
        {
            course = _course;
        }
        public CourseScheduleEntry(CourseScheduleEntry x)
        {
            groupNum = x.groupNum;
            course = x.course;
            instructor = x.instructor;
            time = x.time;
            room = x.room;
        }
        public double GetCapacityPenaltyMultiplier()
        {
            if (course.Capacity == room.Capacity)
                return 0;
            if (course.Capacity > room.Capacity)
                return course.Capacity / room.Capacity;
            else
                return (float)room.Capacity / course.Capacity;
        }
        public bool HasUnqualifiedInstructor()
        {
            return !(instructor.CoursesQualifiesFor.Count == 0 || instructor.CoursesQualifiesFor.Contains(course));
        }
    }
}
