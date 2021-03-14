using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenAlgoLab
{
    public partial class MainPage : Form
    {
        static Scheduler sc;
        public MainPage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Course c1 = new Course(0, "OGKG", 90, 1);
            Course c2 = new Course(1, "Algo", 90,1);
            Course c3 = new Course(2, "IntSys", 90,1);
            Course c4 = new Course(3, "BI", 30,1);
            Course c5 = new Course(4, "Web", 100, 1);
            Course c6 = new Course(5, "UNIX", 90, 1);
            Course c7 = new Course(6, "Cloud", 80, 1);
            Course c8 = new Course(7, "PARCS", 30, 1);
            Course c9 = new Course(8, "Parallel", 30, 1);
            Course c10 = new Course(9, "Poker", 50, 1);
            Course c11 = new Course(10, "Cloud 2", 100, 1);
            Course c12 = new Course(11, "Econ", 30, 1);
            Course c13 = new Course(12, "Manag", 80, 1);
            Course c14 = new Course(13, "MathMod", 90, 1);
            sc = new Scheduler(
               new List<Course> { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14 },
               new List<Room>
               { 
                   new Room(0, 100), new Room(1, 30), new Room(2, 40), new Room(3, 90), new Room(4, 100), new Room(5, 90), new Room(6, 30),
                   new Room(7, 100), new Room(8,120), new Room(9, 100), new Room(10, 60), new Room(11, 30), new Room(12, 100), new Room(13, 100), new Room(14, 100),
                   new Room(15, 100), new Room(16, 100)
               },
               new List<Instructor> { 
                   new Instructor(0, "Fedorus", new HashSet<Course>{ c1, c2, c3}),
                   new Instructor(1,"Panch", new HashSet<Course>{ c4, c5, c6}),
                   new Instructor(2, "Brevno", new HashSet<Course>{ c7, c8, c9, c10, c11}), 
                   new Instructor(3,"Vergun", new HashSet<Course>{ c12, c13, c14 })
               }
           );
           //c1.AddConstraint(new SimultCourseConstraint(c1, c2));
           //c1.AddConstraint(new SimultCourseConstraint(c1, c3));
           //c1.AddConstraint(new SimultCourseConstraint(c1, c4));
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            sc.RunSimpleGA();
            /*for(int i = 0; i < sc.SchedulePopulation[39].Entries.Count; i++)
            {
                var item = sc.SchedulePopulation[39].Entries[i];
                switch (item.day)
                {
                    case 0:
                        monDataGridView.Rows.Add(item.timeSlot, item.classNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 1:
                        tueDataGridView.Rows.Add(item.timeSlot, item.classNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 2:
                        wedDataGridView.Rows.Add(item.timeSlot, item.classNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 3:
                        thuDataGridView.Rows.Add(item.timeSlot, item.classNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 4:
                        friDataGridView.Rows.Add(item.timeSlot, item.classNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    default:
                        throw new ArgumentException("day > 4");
                }
            }*/
        }
    }
}
