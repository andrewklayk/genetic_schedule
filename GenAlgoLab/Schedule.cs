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
        public uint unqualCount = 0;
        public uint occRoomCount = 0;
        public uint occTeacherCount = 0;
        public uint capCount = 0;
        public List<CourseScheduleEntry> Entries;
        public List<ConstraintBase> Constraints;
        public double Fitness {
            get
            {
                double totalPenalty = 0;
                violationCount = 0;
                unqualCount = 0;
                occRoomCount = 0;
                capCount = 0;
                occTeacherCount = 0;
                foreach (var entryX in Entries)
                {
                    var countExcCap = entryX.CountExceedsCapacity();
                    totalPenalty += OverRoomSizePenalty * countExcCap;
                    violationCount += countExcCap;
                    capCount += countExcCap;
                    if (entryX.HasUnqualifiedInstructor())
                    {
                        totalPenalty += NotQualifiedPenalty;
                        violationCount++;
                        unqualCount++;
                    }
                    foreach (var entryY in Entries)
                    {
                        /*for(int i = 0; i < 5; i++)
                        {
                            for(int j = 0; j < 4; j++)
                            {
                                if (anotherEntry.daysAndTimes[i, j] == entryX.daysAndTimes[i, j])
                                {
                                    if (anotherEntry.room == entryX.room)
                                        totalPenalty += BusyRoomPenalty;
                                    if (anotherEntry.instructor == entryX.instructor)
                                        totalPenalty += BusyTeacherPenalty;
                                }
                            }
                        }*/
                        for (byte day = 0; day < 5; day++)
                        {
                            for (byte time = 0; time < 4; time++)
                            {
                                if (entryX != entryY &&
                                    entryX.dayTimeRoom.ContainsKey(new Tuple<byte, byte>(day, time)) &&
                                    entryY.dayTimeRoom.ContainsKey(new Tuple<byte, byte>(day, time)))
                                {
                                    if (entryX.instructor == entryY.instructor)
                                    {
                                        totalPenalty += BusyTeacherPenalty;
                                        violationCount++;
                                        occTeacherCount++;
                                    }
                                    if (entryX.dayTimeRoom[new Tuple<byte, byte>(day, time)] == entryY.dayTimeRoom[new Tuple<byte, byte>(day, time)])
                                    {
                                        totalPenalty += BusyRoomPenalty;
                                        violationCount++;
                                        occRoomCount++;
                                    }
                                }
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
            Entries = new List<CourseScheduleEntry>();
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
