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
        static GenAlgorithm sc;
        public MainPage()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Course c1 = new Course(0, "OGKG", 30, 1);
            Course c2 = new Course(1, "IntSys", 30,1);
            Course c3 = new Course(2, "BI", 50,1);
            Course c8 = new Course(7, "Web", 60, 1);
            Course c9 = new Course(8, "Parallel", 30, 1);
            Course c10 = new Course(9, "Manag", 30, 1);
            sc = new GenAlgorithm(
               new List<Course> { c1, c2, c3, c8, c9, c10 },
               new List<Room>
               { 
                   new Room(0, 100), new Room(1, 30), new Room(2, 40), new Room(3, 90), new Room(4, 100), new Room(5, 90), new Room(6, 30),
                   new Room(7, 100), new Room(8,120), new Room(9, 100), new Room(10, 60), new Room(11, 30), new Room(12, 100), new Room(13, 100), new Room(14, 100),
                   new Room(15, 100), new Room(16, 100)
               },
               new List<Instructor> { 
                   new Instructor(0, "Fedorus", new HashSet<Course>{c1, c2, c3 }),
                   new Instructor(1,"Panch", new HashSet<Course>{ c8,c9}),
                   new Instructor(2, "Brevno", new HashSet<Course>{c9,c10 }), 
                   new Instructor(3,"Vergun", new HashSet<Course>{c10 }),
                   new Instructor(4, "Tereshch", new HashSet<Course>{c2,c3,c8 })
               }
           );
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            sc.RunSimpleGA();
            var res = sc.Schedules.Max().Entries.OrderBy(x => x.time).ThenBy(x=>x.groupNum).ToList();
            foreach (var item in res)
            {
                switch (0)
                {
                    case 0:
                        monDataGridView.Rows.Add(item.time, item.groupNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 1:
                        tueDataGridView.Rows.Add(item.time, item.groupNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 2:
                        wedDataGridView.Rows.Add(item.time, item.groupNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 3:
                        thuDataGridView.Rows.Add(item.time, item.groupNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 4:
                        friDataGridView.Rows.Add(item.time, item.groupNum, item.course.Name, item.room.RoomID, item.instructor.FullName);
                        break;
                    default:
                        throw new ArgumentException("day > 4");
                }
            }
        }
    }
}
