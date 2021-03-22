using System;
using System.Collections.Generic;
using System.Linq;

namespace GenAlgoLab
{
    public class GenAlgorithm
    {
        //Number of classes
        private byte daysCount = 5;
        private int toNextGen = 2;
        //Number of timeslots (courses per day)
        // 0 - 8:40 - 10:15
        // 1 - 10:35 - 12:10
        // 3 - 12:20 - 13:55
        // 4 - 14:05 - 15:40
        static int timeSlotsCount = 4;
        //Number of schedules (population size)
        public int maxHardViolations = 0;
        static int popSize = 40;
        //Chromosome population
        public List<Schedule> schedules = new List<Schedule>();
        public HashSet<Course> courses = new HashSet<Course>();
        public HashSet<Group> groups = new HashSet<Group>();
        public List<Room> rooms = new List<Room>();
        public List<Instructor> instructors = new List<Instructor>();
        private double crossoverProb = 0.5;
        private double mutationProb = 0.1;
        public GenAlgorithm(ICollection<Group> _groups, ICollection<Course> _courses, ICollection<Room> _rooms, ICollection<Instructor> _instructors)
        {
            groups = _groups.ToHashSet();
            courses = _courses.ToHashSet();
            instructors = _instructors.ToList();
            rooms = _rooms.ToList();
        }
        private List<Room> GetFreeRooms(Schedule s, byte day, byte time, int cap)
        {
            return rooms.Where(x => x.Capacity >= cap && !s.Entries.Exists(
                                        e => e.day == day && e.time == time && e.room == x
                                    )).ToList();
        }
        private List<Instructor> GetFreeQualifiedInstructors(Schedule s, Course c, byte day, byte time)
        {
            return instructors.Where(x => (x.CoursesQualifiesFor.Count == 0 || x.CoursesQualifiesFor.Contains(c)) && !s.Entries.Exists(
                                        e => e.day == day && e.time == time && e.instructor == x
                                    )).ToList();
        }
        public void RandomInitialize()
        {
            Random rng = new Random();
            for (int i = 0; i < popSize; i++)
            {
                Schedule newSchedule = new Schedule(i);
                foreach (var cl in groups)
                {
                    foreach (var course in cl.courses)
                    {
                        for (var j = 0; j < course.MinHours; j++)
                        {
                            byte _day = 0;
                            byte _time = 0;
                            var freeInstructors = new List<Instructor>();
                            var freeRooms = new List<Room>();
                            while (_day < daysCount)
                            {
                                freeRooms = GetFreeRooms(newSchedule, _day, _time, course.capacity);
                                freeInstructors = GetFreeQualifiedInstructors(newSchedule, course, _day, _time);
                                if (freeRooms.Count > 0 && freeInstructors.Count > 0)
                                    break;
                                else
                                {
                                    _time++;
                                    if (_time == timeSlotsCount)
                                    {
                                        _day++;
                                        _time = 0;
                                    }
                                }
                            }
                            var newEntry = new CourseScheduleEntry(cl, course)
                            {
                                group = cl,
                                room = freeRooms[rng.Next(freeRooms.Count)],
                                instructor = freeInstructors[rng.Next(freeInstructors.Count)],
                                day = _day,
                                time = _time,
                            };
                            newSchedule.Entries.Add(newEntry);
                        }
                    }
                }
                schedules.Add(newSchedule);
            }
        }

        private List<Schedule> FullSelectForReproduction()
        {
            double sumFitness = schedules.Select(x => x.Fitness).Sum();
            var wheel = new List<Tuple<double, Schedule>>();
            foreach (var s in schedules)
            {
                wheel.Add(new Tuple<double, Schedule>(s.Fitness / sumFitness, s));
            }
            var parents = new List<Schedule>();
            var rng = new Random();
            //Generate N-toNextGen pairs of parents
            for(int i = 0; i < (schedules.Count - toNextGen)*2; i++)
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
                var parOne = parents[pairIndex];
                var parTwo = parents[pairIndex + 1];
                var child = new Schedule(id: 0);
                if (parOne.Entries.Count != parTwo.Entries.Count)
                    throw new ArgumentException("Different chromosome count in individuals!");
                for (int i = 0; i < parOne.Entries.Count; i++)
                {
                    child.Entries.Add(new CourseScheduleEntry(parOne.Entries[i]));
                    var crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child.Entries[i].instructor = parTwo.Entries[i].instructor;
                    }
                    crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child.Entries[i].room = parTwo.Entries[i].room;
                    }
                    crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child.Entries[i].day = parTwo.Entries[i].day;
                        child.Entries[i].time = parTwo.Entries[i].time;
                    }
                }
                children.Add(child);
            }
            return children;
        }

        private void MutateAndFix(Schedule schedule)
        {
            var rng = new Random();
            foreach (var entry in schedule.Entries)
            {
                if (rng.NextDouble() < mutationProb || schedule.busyGroupCount > 0)
                {
                    byte d = 0;
                    byte t = 0;
                    //Change time and day to one when this group is free
                    while (d < daysCount && t < timeSlotsCount && schedule.Entries.Exists(x => x.group == entry.group && x.day == d && x.time == t))
                    {
                        t++;
                        if (t == timeSlotsCount)
                        {
                            d++;
                            t = 0;
                        }
                    }
                    entry.day = d;
                    entry.time = t;
                    schedule.EvaluateFitness();
                }
                if (rng.NextDouble() < mutationProb || schedule.unqualCount > 0 || schedule.occTeacherCount > 0)
                {
                    byte d = 0;
                    byte t = 0;
                    var freeInstructors = GetFreeQualifiedInstructors(schedule, entry.course, d, t);
                    while (freeInstructors.Count == 0)
                    {
                        t++;
                        if (t == timeSlotsCount)
                        {
                            d++;
                            t = 0;
                        }
                        freeInstructors = GetFreeQualifiedInstructors(schedule, entry.course, d, t);
                    }
                    entry.time = t;
                    entry.day = d;
                    entry.instructor = freeInstructors[rng.Next(freeInstructors.Count)];
                    schedule.EvaluateFitness();
                }
                if (rng.NextDouble() < mutationProb || schedule.occRoomCount > 0 || schedule.capCount > 0)
                {
                    byte d = 0;
                    byte t = 0;
                    var freeRooms = GetFreeRooms(schedule, d, t, entry.course.capacity);
                    while (freeRooms.Count == 0)
                    {
                        t++;
                        if (t == timeSlotsCount)
                        {
                            d++;
                            t = 0;
                        }
                        freeRooms = GetFreeRooms(schedule, d, t, entry.course.capacity);
                    }
                    entry.time = t;
                    entry.day = d;
                    entry.room = freeRooms[rng.Next(freeRooms.Count)];
                }
            }
        }

        public Schedule RunGA(int minGenerations = -1, int maxGenerations = 10000)
        {
            for(int i = 0; i < maxGenerations; i++)
            {
                //Calculate fitness
                foreach (var s in schedules)
                {
                    s.EvaluateFitness();
                }
                //Determine if a good enough solution exists already
                schedules.Sort();
                if (schedules[popSize - 1].hardViolationCount <= maxHardViolations && (minGenerations == -1 || i >= minGenerations))
                    return schedules[popSize - 1];
                //Perform selection
                var selectedForReproduction = FullSelectForReproduction();
                //Perform crossover
                var children = FullCrossover(selectedForReproduction);
                //Replace old chromosomes with new ones (except toNextGen best ones) and fix and mutate
                for(int j = 0; j < popSize - toNextGen; j++)
                {
                    schedules[j] = children[j];
                    schedules[j].EvaluateFitness();
                    MutateAndFix(schedules[j]);
                }
            }
            return null;
        }
    }
}
