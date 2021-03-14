using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    //Chromosome
    public class ScheduleEntry
    {
        private int EntryID;
        public Course course;
        public byte timeSlot;
        public byte classNum;
        public byte day;
        public Room room;
        public Instructor instructor;
        public ScheduleEntry(ScheduleEntry _e)
        {
            course = _e.course;
            timeSlot = _e.timeSlot;
            classNum = _e.classNum;
            day = _e.day;
            room = _e.room;
            instructor = _e.instructor;
        }
        public ScheduleEntry(int id, byte _timeSlot, byte _day, byte _classNum)
        {
            EntryID = id;
            timeSlot = _timeSlot;
            day = _day;
            classNum = _classNum;
        }

        public bool ExceedsCapacity()
        {
            return course.Capacity > room.Capacity;
        }

        public bool HasUnqualifiedInstructor()
        {
            return instructor.CoursesQualifiesFor.Count != 0 && !instructor.CoursesQualifiesFor.Contains(course);
        }
    }
}
