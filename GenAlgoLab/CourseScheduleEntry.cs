using System;
using System.Collections.Generic;

namespace GenAlgoLab
{
    public class CourseScheduleEntry
    {
        public Course course;
        public Dictionary<Tuple<byte, byte>, Room> dayTimeRoom;
        public Instructor instructor;
        public CourseScheduleEntry(Course _course, Instructor _instructor)
        {
            course = _course;
            instructor = _instructor;
            dayTimeRoom = new Dictionary<Tuple<byte, byte>, Room>();
        }
        public CourseScheduleEntry(CourseScheduleEntry x)
        {
            course = x.course;
            instructor = x.instructor;
            dayTimeRoom = new Dictionary<Tuple<byte, byte>, Room>();
            foreach(var entry in x.dayTimeRoom)
            {
                dayTimeRoom.Add(entry.Key, entry.Value);
            }
        }
        public void AddTimeRoomAssignment(byte day, byte time, Room room)
        {
            var key = new Tuple<byte, byte>(day, time);
            dayTimeRoom[key] = room;
        }
        public Room GetTimeRoomAssignmentOrNull(byte day, byte time)
        {
            var key = new Tuple<byte, byte>(day, time);
            if (dayTimeRoom.ContainsKey(key))
                return dayTimeRoom[key];
            else
                return null;
        }
        /*public bool AddDayTime(byte day, byte time)
        {
            return daysAndTimes.Add(new Tuple<byte, byte>(day, time));
        }*/
        /*public void Mutate()
        {
            var rng = new Random();
            var sw = rng.Next(5);
            switch(sw)
            {
                case 0:
                    course = 
            }
        }*/
        public uint CountExceedsCapacity()
        {
            uint count = 0;
            foreach(var entry in dayTimeRoom)
            {
                if (entry.Value.Capacity < course.Capacity)
                    count++;
            }
            return count++;
        }
        public bool NotEnoughHours()
        {
            return dayTimeRoom.Count < course.MinHours;
        }
        public bool HasUnqualifiedInstructor()
        {
            return !(instructor.CoursesQualifiesFor.Count == 0 || instructor.CoursesQualifiesFor.Contains(course));
        }
    }
}
