using System.Collections.Generic;
using System.Linq;
using System;

namespace GenAlgoLab
{
    public class Schedule : IComparable
    {
        private readonly int ScheduleID;
        private readonly int BusyTeacherPenalty = 1000;
        private readonly int BusyRoomPenalty = 1000;
        private readonly int OverRoomSizePenalty = 20;
        private readonly int NotQualifiedPenalty = 200;
        public uint violationCount = 0;
        public List<ScheduleEntry> Entries;
        public List<ConstraintBase> Constraints;
        public double Fitness {
            get 
            {
                double totalPenalty = 0;
                violationCount = 0;
                foreach (var scheduleEntry in Entries)
                {
                    if (scheduleEntry.ExceedsCapacity())
                    {
                        totalPenalty += OverRoomSizePenalty;
                        violationCount++;
                    }
                    if (scheduleEntry.HasUnqualifiedInstructor())
                    {
                        totalPenalty += NotQualifiedPenalty;
                        violationCount++;
                    }
                    foreach (var x in Entries)
                    {
                        /*for(int i = 0; i < 5; i++)
                        {
                            for(int j = 0; j < 4; j++)
                            {
                                if (anotherEntry.daysAndTimes[i, j] == scheduleEntry.daysAndTimes[i, j])
                                {
                                    if (anotherEntry.room == scheduleEntry.room)
                                        totalPenalty += BusyRoomPenalty;
                                    if (anotherEntry.instructor == scheduleEntry.instructor)
                                        totalPenalty += BusyTeacherPenalty;
                                }
                            }
                        }*/
                        if(x != scheduleEntry && x.day == scheduleEntry.day && x.timeSlot == scheduleEntry.timeSlot)
                        {
                            if (x.room == scheduleEntry.room)
                            {
                                totalPenalty += BusyRoomPenalty;
                                violationCount++;
                            }
                            if (x.instructor == scheduleEntry.instructor)
                            {
                                totalPenalty += BusyTeacherPenalty;
                                violationCount++;
                            }
                        }
                    }
                }
                return 1/totalPenalty;
            }
        }
        public Schedule(int id)
        {
            ScheduleID = id;
            Entries = new List<ScheduleEntry>();
        }
        //Compares two Schedules by their Fitness value
        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            var objSchedule = obj as Schedule;
            if (obj == null)
                throw new ArgumentException("obj is not a Schedule");
            return this.Fitness.CompareTo(objSchedule.Fitness);
        }
    }
}
