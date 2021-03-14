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
        public CourseScheduleEntry(Course _course, Instructor _instructor)
        {
            course = _course;
            instructor = _instructor;
        }
        public CourseScheduleEntry(CourseScheduleEntry x)
        {
            groupNum = x.groupNum;
            course = x.course;
            instructor = x.instructor;
            time = x.time;
            room = x.room;
        }
        public double GetExceedsRoomCapacityMultiplier()
        {
            return course.Capacity > room.Capacity ? course.Capacity / room.Capacity : 0;
        }
        public bool HasUnqualifiedInstructor()
        {
            return !(instructor.CoursesQualifiesFor.Count == 0 || instructor.CoursesQualifiesFor.Contains(course));
        }
    }
}
