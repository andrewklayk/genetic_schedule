using System;
using System.Collections.Generic;
using System.Linq;

namespace GenAlgoLab
{
    public class GenAlgorithm
    {
        //Number of classes
        static byte classes = 3;
        private byte daysCount = 5;
        private int toNextGen = 2;
        private enum Days
        {
            Mon = 0,
            Tue = 1,
            Wed = 2,
            Thu = 3,
            Fri = 4
        }
        //Number of timeslots (courses per day)
        // 0 - 8:40 - 10:15
        // 1 - 10:35 - 12:10
        // 3 - 12:20 - 13:55
        // 4 - 14:05 - 15:40
        static int timeSlotsCount = 6;
        //Number of schedules (population size)
        static int popSize = 40;
        public int maxViolations = 0;
        public List<Room> Rooms;
        public List<Course> Courses;
        public List<Instructor> Instructors;
        public List<Schedule> Schedules;
        private double crossoverProb = 0.5;
        private double mutationProb = 0.1 ;

        public GenAlgorithm()
        {
            Courses = new List<Course>();
            Instructors = new List<Instructor>();
            Schedules = new List<Schedule>();
        }
        public GenAlgorithm(ICollection<Course> cs, List<Room> _rooms, List<Instructor> _instructors)
        {
            Courses = cs.ToList();
            Instructors = _instructors;
            Schedules = new List<Schedule>();
            Rooms = _rooms;
        }
        public void RandomInitialize()
        {
            Random rng = new Random();
            for (int i = 0; i < popSize; i++)
            {
                Schedule newSchedule = new Schedule(i);
                foreach (var courseToAdd in Courses)
                {
                    for (int cl = 0; cl < classes; cl++)
                    {
                        byte _time;
                        var freeInstructors = new List<Instructor>();
                        var freeRooms = new List<Room>();
                        for (_time = 0; _time < timeSlotsCount && freeInstructors.Count == 0 && freeRooms.Count == 0; _time++)
                        {
                            freeInstructors = Instructors.Where(
                                x => (x.CoursesQualifiesFor.Count == 0 || x.CoursesQualifiesFor.Contains(courseToAdd)) && !newSchedule.Entries.Exists(
                                    e => e.time == _time && e.instructor == x
                                )).ToList();
                            freeRooms = Rooms.Where(
                                x => !newSchedule.Entries.Exists(
                                    e => e.time == _time && e.room == x
                                )).ToList();
                        }
                        var newEntry = new CourseScheduleEntry(courseToAdd)
                        {
                            groupNum = cl,
                            room = freeRooms[rng.Next(freeRooms.Count)],
                            instructor = freeInstructors[rng.Next(freeInstructors.Count)],
                            time = _time,
                        };
                        newSchedule.Entries.Add(newEntry);
                    }
                }
                Schedules.Add(newSchedule);
            }
        }

        private List<Schedule> FullSelectForReproduction()
        {
            double sumFitness = Schedules.Select(x => x.Fitness).Sum();
            var wheel = new List<Tuple<double, Schedule>>();
            foreach (var s in Schedules)
            {
                wheel.Add(new Tuple<double, Schedule>(s.Fitness / sumFitness, s));
            }
            var parents = new List<Schedule>();
            var rng = new Random();
            //Generate N-toNextGen pairs of parents
            for(int i = 0; i < (Schedules.Count - toNextGen)*2; i++)
            {
                var d = rng.NextDouble();
                var upper = 0.0;
                for (int j = 0; j < wheel.Count; j++)
                {
                    upper += wheel[j].Item1;
                    if (d < upper)
                    {
                        parents.Add(wheel[j].Item2);
                        break;
                    }
                }
            }
            return parents;
        }

        private List<Schedule> FullCrossover(List<Schedule> parents)
        {
            var rng = new Random();
            var children = new List<Schedule>();
            for(int pairIndex = 0; pairIndex < parents.Count - 1; pairIndex += 2)
            {
                var firstParent = parents[pairIndex];
                var secondParent = parents[pairIndex + 1];
                var child = new Schedule(id: 0);
                if (firstParent.Entries.Count != secondParent.Entries.Count)
                    throw new ArgumentException("Different chromosome count in individuals!");
                for (int i = 0; i < firstParent.Entries.Count; i++)
                {
                    child.Entries.Add(new CourseScheduleEntry(firstParent.Entries[i]));
                    var crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child.Entries[i].instructor = secondParent.Entries[i].instructor;
                    }
                    crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child.Entries[i].room = secondParent.Entries[i].room;
                    }
                    crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child.Entries[i].time = secondParent.Entries[i].time;
                    }
                }
                children.Add(child);
            }
            return children;
        }

        private void FixAndMutate(Schedule schedule)
        {
            var rng = new Random();
            foreach (var entry in schedule.Entries)
            {
                if (rng.NextDouble() < mutationProb)
                {
                    if(schedule.busyGroupCount > 0)
                    {
                        //Change timeslot to one when this group is free
                        for (byte t = 0; t < timeSlotsCount; t++) {
                            if (schedule.Entries.Exists(x => x.groupNum == entry.groupNum && x.time == t))
                                continue;
                            entry.time = t;
                        }
                    }
                    schedule.EvaluateFitness();
                    if(schedule.unqualCount > 0 || schedule.occTeacherCount > 0)
                    {
                        //Change the instructor to one that is free at that time and is qualified for this course
                        for (byte time = 0; time < timeSlotsCount; time++)
                        {
                            var availableInstructors = Instructors.Where(r => r.CoursesQualifiesFor.Contains(entry.course) && 
                            !schedule.Entries.Exists(e => e != entry && e.time == time && e.instructor == entry.instructor)).ToList();
                            if (availableInstructors.Count == 0)
                                continue;
                            else
                            {
                                entry.time = time;
                                entry.instructor = availableInstructors[rng.Next(availableInstructors.Count)];
                            }
                        }
                    }
                    schedule.EvaluateFitness();
                    if (schedule.occRoomCount > 0 || schedule.capCount > 0)
                    {
                        //Change room to one that is free and supports the course
                        for(byte time = 0; time < timeSlotsCount; time++)
                        {
                            var viableRooms = Rooms.Where(r => r.Capacity >= entry.course.Capacity && 
                            !schedule.Entries.Exists(e => e!= entry && e.time == time && e.room == entry.room)).ToList();
                            if (viableRooms.Count == 0)
                                continue;
                            else
                            {
                                entry.time = time;
                                entry.room = viableRooms[rng.Next(viableRooms.Count)];
                            }
                        }
                    }
                }
            }
        }

        public long RunSimpleGA(int generationNum = 1000)
        {
            RandomInitialize();
            int i = 0;
            for (; i < generationNum; i++)
            {
                foreach (var s in Schedules)
                {
                    s.EvaluateFitness();
                }
                Schedules.Sort();
                if (Schedules[popSize-1].violationCount <= maxViolations)
                    return i;
                var selectedForReproduction = FullSelectForReproduction();
                var children = FullCrossover(selectedForReproduction);
                for(int j = 0; j < popSize - toNextGen; j++)
                {
                    Schedules[j] = children[j];
                }
                foreach(var s in Schedules)
                {
                    s.EvaluateFitness();
                    FixAndMutate(s);
                }
            }
            Schedules.Sort();
            return i;
        }
    }
}
