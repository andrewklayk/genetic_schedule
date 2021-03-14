using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{  
    // Course A shouldn't be taught in the same time slot as course B
    public class SimultCourseConstraint : ConstraintBase
    {
        public Course SecondCourse;
        public double Penalty;
        public override double PenalizeIfViolated(Schedule sc)
        {
            double totalPenalty = 0;
            var entriesForA = sc.Entries.Where(x=>x.course == course);
            var entriesForB = sc.Entries.Where(x=>x.course == SecondCourse);
            foreach(var entA in entriesForA)
            {
                //if (entriesForB.Any(x => x.timeSlot == entA.timeSlot))
                //    totalPenalty += Penalty;
            }
            return totalPenalty;
        }
        public SimultCourseConstraint(Course _a, Course _b, double penalty=10)
        {
            course = _a;
            SecondCourse = _b;
            Penalty = penalty;
        }
    }
}
