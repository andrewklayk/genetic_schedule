using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{   
    // Penalizes having rooms with capacity lower then the course's
    class RoomCapacityConstraint : ConstraintBase
    {
        int capacity;
        public Course Course { get; set; }
        public double Penalty { get; set; }
        public override double PenalizeIfViolated(Schedule s)
        {
            double cumPenalty = 0;
            var EntriesWithCourse = s.Entries.Where(x => x.course == Course);
            foreach (var entry in EntriesWithCourse)
            {
                //if (capacity > entry.room.Capacity)
                //    cumPenalty += Penalty;
            }
            return cumPenalty;
        }
        public RoomCapacityConstraint(Course c, int _capacity, double p = 20)
        {
            Course = c;
            capacity = _capacity;
            Penalty = p;
        }
    }
}
