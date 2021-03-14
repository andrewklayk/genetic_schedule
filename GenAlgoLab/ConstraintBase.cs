using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    public abstract class ConstraintBase
    {
        private double Penalty;
        protected Course course;
        public abstract double PenalizeIfViolated(Schedule se);
    }
}
