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
    }
}
