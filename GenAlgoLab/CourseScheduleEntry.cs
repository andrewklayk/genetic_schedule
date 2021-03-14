using System;

namespace GenAlgoLab
{
    public class CourseScheduleEntry
    {
        public Course course;
        public bool[,] daysAndTimes;
        public Room room;
        public Instructor instructor;
        public CourseScheduleEntry(Course _course, Room _room, Instructor _instructor)
        {
            course = _course;
            room = _room;
            instructor = _instructor;
            daysAndTimes = new bool[5,4];
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j <4; j++)
                {
                    daysAndTimes[i, j] = false;
                }
            }
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
        public bool ExceedsCapacity()
        {
            return course.Capacity > room.Capacity;
        }
        public bool NotEnoughHours()
        {
            int hoursCount = 0;
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j <4; j++)
                {
                    if (daysAndTimes[i,j])
                        hoursCount += 1;
                }
            }
            return hoursCount < course.MinHours;
        }
        public bool HasUnqualifiedInstructor()
        {
            return !(instructor.CoursesQualifiesFor.Count == 0 || instructor.CoursesQualifiesFor.Contains(course));
        }
    }
}
