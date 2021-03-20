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
        static int timeSlotsCount = 3;
        //Number of schedules (population size)
        static int popSize = 40;
        public int maxViolations = 0;
        public List<Room> rooms;
        public HashSet<Course> courses;
        public HashSet<Group> groups;
        public List<Instructor> instructors;
        public List<Schedule> schedules;
        private double crossoverProb = 0.5;
        private double mutationProb = 0.1 ;

        public GenAlgorithm()
        {
            courses = new HashSet<Course>();
            instructors = new List<Instructor>();
            schedules = new List<Schedule>();
        }
        public GenAlgorithm(ICollection<Group> _groups, ICollection<Course> _courses, List<Room> _rooms, List<Instructor> _instructors)
        {
            groups = _groups.ToHashSet();
            courses = _courses.ToHashSet();
            instructors = _instructors;
            schedules = new List<Schedule>();
            rooms = _rooms;
        }
        public List<Room> GetFreeRooms(Schedule s, byte day, byte time, int cap)
        {
            return rooms.Where(x => x.Capacity >= cap && !s.Entries.Exists(
                                        e => e.day == day && e.time == time && e.room == x
                                    )).ToList();
        }
        public List<Instructor> GetFreeQualifiedInstructors(Schedule s, Course c, byte day, byte time, int cap)
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
                                freeInstructors = GetFreeQualifiedInstructors(newSchedule, course, _day, _time, course.capacity);
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
                        child.Entries[i].day = secondParent.Entries[i].day;
                        child.Entries[i].time = secondParent.Entries[i].time;
                    }
                }
                children.Add(child);
            }
            return children;
        }

        private void Fix(Schedule schedule)
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
                    var freeInstructors = GetFreeQualifiedInstructors(schedule, entry.course, d, t, entry.course.capacity);
                    while (freeInstructors.Count == 0)
                    {
                        t++;
                        if (t == timeSlotsCount)
                        {
                            d++;
                            t = 0;
                        }
                        freeInstructors = GetFreeQualifiedInstructors(schedule, entry.course, d, t, entry.course.capacity);
                    }
                    entry.time = t;
                    entry.day = d;
                    entry.instructor = freeInstructors[rng.Next(freeInstructors.Count)];
                    //Change the instructor to one that is free at that time and is qualified for this course
                    /*for (byte time = 0, day = 0; time < timeSlotsCount && day < daysCount; time++)
                    {
                        var availableInstructors = Instructors.Where(r => r.CoursesQualifiesFor.Contains(entry.course) && 
                        !schedule.Entries.Exists(e => e != entry && e.day == day && e.time == time && e.instructor == entry.instructor)).ToList();
                        if (availableInstructors.Count > 0)
                        { 
                            entry.day = day;
                            entry.time = time;
                            entry.instructor = availableInstructors[rng.Next(availableInstructors.Count)];
                            break;
                        }
                        if (time == timeSlotsCount - 1)
                        {
                            day++;
                            time = 0;
                        }
                    }*/
                    schedule.EvaluateFitness();
                }
                if (rng.NextDouble() < mutationProb || schedule.occRoomCount > 0 || schedule.capCount > 0)
                {
                    //Change room to one that is free and supports the course
                    /*for(byte time = 0, day = 0; time < timeSlotsCount && day < daysCount; time++)
                    {
                        var viableRooms = Rooms.Where(r => r.capacity >= entry.course.capacity &&
                        !schedule.Entries.Exists(e => e != entry && e.day == day && e.time == time && e.room == entry.room)).ToList();
                        if(viableRooms.Count > 0)
                        {
                            entry.day = day;
                            entry.time = time;
                            entry.room = viableRooms[rng.Next(viableRooms.Count)];
                            break;
                        }
                        if (time == timeSlotsCount - 1)
                        {
                            day++;
                            time = 0;
                        }
                    }*/
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

        public long RunGA(int generationNum = 1000, bool stopOnFirstDecision = false)
        {
            int i = 0;
            for (; i < generationNum; i++)
            {
                foreach (var s in schedules)
                {
                    s.EvaluateFitness();
                }
                schedules.Sort();
                if (schedules[popSize-1].violationCount <= maxViolations && stopOnFirstDecision)
                    return i;
                var selectedForReproduction = FullSelectForReproduction();
                var children = FullCrossover(selectedForReproduction);
                for(int j = 0; j < popSize - toNextGen; j++)
                {
                    schedules[j] = children[j];
                }
                foreach(var s in schedules)
                {
                    s.EvaluateFitness();
                    Fix(s);
                }
            }
            return -1;
        }
    }
}
