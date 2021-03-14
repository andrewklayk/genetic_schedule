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
        public List<ConstraintBase> constraints;
        public List<Instructor> Instructors;
        public List<Schedule> SchedulePopulation;
        private double crossoverProb = 0.5;
        private double mutationProb = 0.1;

        public GenAlgorithm()
        {
            Courses = new List<Course>();
            constraints = new List<ConstraintBase>();
            Instructors = new List<Instructor>();
            SchedulePopulation = new List<Schedule>();
        }
        public GenAlgorithm(ICollection<Course> cs, List<Room> _rooms, List<Instructor> _instructors)
        {
            Courses = cs.ToList();
            constraints = cs.SelectMany(x => x.Constraints).ToList();
            Instructors = _instructors;
            SchedulePopulation = new List<Schedule>();
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
                        var instructor = Instructors[rng.Next(Instructors.Count)];
                        var newEntry = new CourseScheduleEntry(courseToAdd, instructor)
                        {
                            groupNum = cl,
                            room = Rooms[rng.Next(Rooms.Count)],
                            instructor = instructor,
                            time = (byte)rng.Next(timeSlotsCount),
                        };
                        newSchedule.Entries.Add(newEntry);
                    }
                }
                SchedulePopulation.Add(newSchedule);
            }
        }

        private List<Schedule> FullSelectForReproduction()
        {
            double totalFitness = SchedulePopulation.Select(x => x.Fitness).Sum();
            var wheel = new List<Tuple<double, Schedule>>();
            foreach (var s in SchedulePopulation)
            {
                wheel.Add(new Tuple<double, Schedule>(s.Fitness / totalFitness, s));
            }
            var parents = new List<Schedule>();
            var rng = new Random();
            for(int i = 0; i < SchedulePopulation.Count * 2; i++)
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
            List<Schedule> children = new List<Schedule>();
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
                        child.Entries[i] = new CourseScheduleEntry(secondParent.Entries[i]);
                    }
                }
                children.Add(child);
            }
            return children;
        }

        private void Mutate(Schedule individual)
        {
            var rng = new Random();
            foreach (var entry in individual.Entries)
            {
                if (rng.NextDouble() < mutationProb)
                {
                    entry.instructor = Instructors[rng.Next(Instructors.Count)];
                    entry.time = (byte)rng.Next(4);
                    entry.room = Rooms[rng.Next(Rooms.Count)];
                }
            }
        }

        public long RunSimpleGA(int generationNum = 10000)
        {
            RandomInitialize();
            int i = 0;
            for (; i < generationNum; i++)
            {
                foreach (var s in SchedulePopulation)
                {
                    s.CalcFitness();
                }
                if (SchedulePopulation.Max().violationCount <= maxViolations)
                    return i;
                var selectedForReproduction = FullSelectForReproduction();
                var children = FullCrossover(selectedForReproduction);
                foreach(var child in children)
                {  
                    Mutate(child);
                }
                SchedulePopulation = children;
            }
            SchedulePopulation.Sort();
            return i;
        }
    }
}
