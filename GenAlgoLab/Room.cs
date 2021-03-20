using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    public class Room
    {
        public int RoomID;
        public int Capacity;
        public bool[,] busyDayTime;
        public Room(int _id, int _cap)
        {
            RoomID = _id;
            Capacity = _cap;
            busyDayTime = new bool[5, 4];
        }
        public override string ToString()
        {
            return string.Format("Id: {0}, Cap: {1}", RoomID, Capacity);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return obj is Room roomObj && roomObj.RoomID == RoomID;
        }
        public override int GetHashCode()
        {
            int hashCode = -1758048040;
            hashCode = hashCode * -1521134295 + RoomID.GetHashCode();
            hashCode = hashCode * -1521134295 + Capacity.GetHashCode();
            return hashCode;
        }
    }
}
