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
        private readonly int BusyGroupPenalty = 1000;
        private readonly int OverRoomSizePenalty = 20;
        private readonly int NotQualifiedPenalty = 20;
        public double violationCount = 0;
        public uint unqualCount = 0;
        public double occRoomCount = 0;
        public double occTeacherCount = 0;
        public uint capCount = 0;
        public double busyGroupCount = 0;
        public double Fitness = 0;
        public List<CourseScheduleEntry> Entries;
        public void EvaluateFitness()
        {
            double totalPenalty = 0;
            violationCount = 0;
            unqualCount = 0;
            occRoomCount = 0;
            capCount = 0;
            occTeacherCount = 0;
            busyGroupCount = 0;
            foreach (var entryX in Entries)
            {
                var excCap = entryX.GetCapacityPenaltyMultiplier();
                totalPenalty += OverRoomSizePenalty * excCap;
                if (entryX.room.Capacity < entryX.course.Capacity)
                {
                    violationCount++;
                    capCount++;
                }
                if (entryX.HasUnqualifiedInstructor())
                {
                    totalPenalty += NotQualifiedPenalty;
                    violationCount++;
                    unqualCount++;
                }
                foreach (var entryY in Entries)
                {
                    if (entryX != entryY && entryX.time == entryY.time)
                    {
                        if (entryX.instructor == entryY.instructor)
                        {
                            totalPenalty += BusyTeacherPenalty;
                            violationCount+=0.5;
                            occTeacherCount+=0.5;
                        }
                        if (entryX.room == entryY.room)
                        {
                            totalPenalty += BusyRoomPenalty;
                            violationCount+=0.5;
                            occRoomCount+=0.5;
                        }
                        if(entryX.groupNum == entryY.groupNum)
                        {
                            totalPenalty += BusyGroupPenalty;
                            violationCount+=0.5;
                            busyGroupCount+=0.5;
                        }
                    }
                }
            }
            Fitness = 1 / totalPenalty;
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
        public string listViolations()
        {
            return string.Format("Busy room: {0}, Busy teacher: {1}, Busy group {2}, Over cap: {3}, Unqualified teacher: {4}", occRoomCount, occTeacherCount, busyGroupCount, capCount, unqualCount);
        }
    }
}
