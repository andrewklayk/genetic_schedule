using System;
using System.Collections.Generic;
using System.Linq;

namespace GenAlgoLab
{
    public class Scheduler
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
        static int timeSlotsCount = 4;
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

        public Scheduler()
        {
            Courses = new List<Course>();
            constraints = new List<ConstraintBase>();
            Instructors = new List<Instructor>();
            SchedulePopulation = new List<Schedule>();
        }
        public Scheduler(ICollection<Course> cs, List<Room> _rooms, List<Instructor> _instructors)
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
                /*foreach (var courseToAdd in courses)
                {
                    var room = Rooms[rng.Next(Rooms.Count)];
                    var instructor = Instructors[rng.Next(Instructors.Count)];
                    var newEntry = new CourseScheduleEntry(courseToAdd, room, instructor);
                    for(byte day = 0; day<daysCount; day++)
                    {
                        for(byte time = 0; time<timeSlotsCount; time++)
                        {
                            newEntry.daysAndTimes[day, time] = 1 == rng.Next(2);
                            room.busyDayTime[day, time] = true;
                        }
                    }
                }*/
                for (byte day = 0; day < daysCount; day++)
                {
                    for (byte time = 0; time < timeSlotsCount; time++)
                    {
                        var newEntry = new ScheduleEntry(0, time, day, 0)
                        {
                            course = Courses[rng.Next(Courses.Count)],
                            room = Rooms[rng.Next(Rooms.Count)],
                            instructor = Instructors[rng.Next(Instructors.Count)]
                        };
                        newSchedule.Entries.Add(newEntry);
                    }
                }
                SchedulePopulation.Add(newSchedule);
            }
        }
        //Select 2 schedules at random with probabilities proportional to their fitness
        public Schedule[] SelectForReproduction()
        {
            double totalFitness = SchedulePopulation.Select(x => x.Fitness).Sum();
            var wheel = new List<Tuple<double, Schedule>>();
            foreach (var s in SchedulePopulation)
            {
                wheel.Add(new Tuple<double, Schedule>(s.Fitness / totalFitness, s));
            }
            
            Random r = new Random();
            byte sizeOfRepr = 2;
            var mates = new Schedule[sizeOfRepr];
            for (int k = 0; k < sizeOfRepr; k++)
            {
                var d = r.NextDouble();
                var upper = 0.0;
                for (int i=0; i < wheel.Count; i++)
                {
                    upper += wheel[i].Item1;
                    if (d < upper)
                    {
                        mates[k] = wheel[i].Item2;
                        break;
                    }
                }
            }
            return mates;
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
            for(int i = 0; i < SchedulePopulation.Count; i++)
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
                var firstParent = parents[rng.Next(0, parents.Count)];
                var secondParent = parents[rng.Next(0, parents.Count)];
                var child_1 = new Schedule(id: 0);
                var child_2 = new Schedule(id: 0);
                if (firstParent.Entries.Count != secondParent.Entries.Count)
                    throw new ArgumentException("Different chromosome count in individuals!");
                for (int i = 0; i < firstParent.Entries.Count; i++)
                {
                    var crVal = rng.NextDouble();
                    child_1.Entries.Add(new ScheduleEntry(firstParent.Entries[i]));
                    child_2.Entries.Add(new ScheduleEntry(secondParent.Entries[i]));
                    if (crVal < crossoverProb)
                    {
                        child_1.Entries[i].course = secondParent.Entries[i].course;
                        child_2.Entries[i].course = firstParent.Entries[i].course;
                    }
                    crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child_1.Entries[i].instructor = secondParent.Entries[i].instructor;
                        child_2.Entries[i].instructor = firstParent.Entries[i].instructor;
                    }
                    crVal = rng.NextDouble();
                    if (crVal < crossoverProb)
                    {
                        child_1.Entries[i].room = secondParent.Entries[i].room;
                        child_2.Entries[i].room = firstParent.Entries[i].room;
                    }
                }
                children.Add(child_1);
                children.Add(child_2);
            }
            return children;
        }

        //Exchange genes at random
        private void Crossover(Schedule[] Mates)
        {
            var rng = new Random();
            List<Schedule> children = new List<Schedule>();
            //Randomly select a crossover point
            //var crossoverPoint = rng.Next(0, Mates[0].Entries.Count);
            for (int pairInd = 0; pairInd < Mates.Length - 1; pairInd += 2)
            {
                //Child's genes before the CP are inherited from the 1st parent, after - from the 2nd parent
                var child_1 = new Schedule(id: 0);
                var child_2 = new Schedule(id: 0);
                for (int i = 0; i < Mates[pairInd].Entries.Count; i++)
                {
                    var crVal = rng.NextDouble();
                    if (crVal > crossoverProb)
                    {
                        child_1.Entries.Add(Mates[pairInd].Entries[i]);
                        child_2.Entries.Add(Mates[pairInd + 1].Entries[i]);
                    }
                    else
                    {
                        child_1.Entries.Add(Mates[pairInd + 1].Entries[i]);
                        child_2.Entries.Add(Mates[pairInd].Entries[i]);
                    }
                }
                children.Add(child_1);
                children.Add(child_2);
            }
            //Replace *Number of Children* least fit choromosomes with the new children
            SchedulePopulation.Sort();
            for(int i = 0; i < children.Count; i ++)
            {
                if (rng.NextDouble() < mutationProb)
                {
                    Mutate(children[i]);
                }
                SchedulePopulation[i] = children[i];
            }
        }

        private void Mutate(Schedule individual)
        {
            var rng = new Random();
            //for(int i = 0; i < individual.Entries)
            foreach (var entry in individual.Entries)
            {
                if (rng.NextDouble() < mutationProb)
                {
                    var sw = rng.Next(3);
                    switch (sw)
                    {
                        case 0:
                            entry.course = Courses[rng.Next(Courses.Count)];
                            break;
                        case 1:
                            entry.instructor = Instructors[rng.Next(Instructors.Count)];
                            break;
                        case 2:
                            entry.room = Rooms[rng.Next(Rooms.Count)];
                            entry.room.busyDayTime[entry.day, entry.timeSlot] = true;
                            break;
                    }
                }
            }
        }

        public long RunSimpleGA(int generationNum = 10000)
        {
            RandomInitialize();
            int i = 0;
            for (; i < generationNum; i++)
            {
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

        public long RunSteadyStateGA(int generationNum = 10000)
        {
            RandomInitialize();
            for (int i = 0; i < generationNum; i++) {
                var selectedForReproduction = SelectForReproduction();
                Crossover(selectedForReproduction);
            }
            SchedulePopulation.Sort();
            return generationNum;
        }
    }
}
