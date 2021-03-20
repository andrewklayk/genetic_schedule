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
        bool first = true;
        private void Form1_Load(object sender, EventArgs e)
        {
            Course is_p = new Course(0, "IS Lab", 30, 2, ClassTypes.Lab);
            Course is_l = new Course(1, "IS Lec", 90, 1, ClassTypes.Lec);
            Course pr_p = new Course(2, "Pravo Lab", 30, 2,ClassTypes.Lab);
            Course pr_l = new Course(3, "Pravo Lec", 30, 2,ClassTypes.Lec);
            Course kp_p = new Course(4, "KPtaLP Lab", 30, 1, ClassTypes.Lab);
            Course kp_l = new Course(5, "KPtaLP Lec", 30, 1, ClassTypes.Lec);
            Course bi_p = new Course(6, "BI Lab", 30, 1, ClassTypes.Lab);
            Course bi_l = new Course(7, "BI Lec", 30, 1, ClassTypes.Lec);
            Course ms_l = new Course(9, "MSP Lec", 30, 1, ClassTypes.Lec);
            Course ks_l = new Course(9, "KSSQL Lec", 30, 2, ClassTypes.Lec);
            Course mn_p = new Course(10, "Manag Lab", 30, 1, ClassTypes.Lab);
            Course mn_l = new Course(10, "Manag Lec", 90, 1, ClassTypes.Lec);
            //Course rf_l = new Course(11, "Refactoring", 30, 2, ClassTypes.Lec);
            //Course pr_l = new Course(12, "Parallel", 30, 2, ClassTypes.Lec);
            //Course qn_l = new Course(13, "Quant", 30, 2, ClassTypes.Lec);
            sc = new GenAlgorithm(
                _groups: new List<Group> { new Group(0, "TTP", new HashSet<Course>() { is_p, kp_l, bi_l, ms_l, ks_l, is_l, kp_p, bi_p, mn_p })/*, new Group(1, "MI", new HashSet<Course>() { rf_l,pr_l,qn_l})*/ },
               _courses: new List<Course> { is_p, kp_l, bi_l, ms_l, ks_l, is_l, kp_p, bi_p, mn_p },
               _rooms: new List<Room>
               {
                   new Room(0, 100), new Room(1, 30), new Room(2, 40), new Room(3, 90), new Room(4, 100), new Room(5, 90), new Room(6, 30),
                   new Room(7, 100), new Room(8,120), new Room(9, 100), new Room(10, 60), new Room(11, 30), new Room(12, 100), new Room(13, 100), new Room(14, 100),
                   new Room(15, 100), new Room(16, 100)
               },
               _instructors: new List<Instructor> {
                   new Instructor(6, "Glybovecj", new HashSet<Course>(){is_l}),
                   new Instructor(0, "Fedorus", new HashSet<Course>{is_p}),
                   new Instructor(1,"Bohdan", new HashSet<Course>{ pr_p, pr_l}),
                   new Instructor(2,"Tkachenko", new HashSet<Course>{ kp_l,ks_l, kp_p}),
                   new Instructor(3, "Panch", new HashSet<Course>{bi_l, bi_p}),
                   new Instructor(4,"Vergun", new HashSet<Course>{mn_l, mn_p}),
                   new Instructor(5, "Shishka", new HashSet<Course>{ms_l}),
                   //new Instructor(6, "Kuliabko", new HashSet<Course>{rf_l }),
                   //new Instructor(6, "Brevno", new HashSet<Course>{pr_l }),
                   //new Instructor(6, "Riabokonj", new HashSet<Course>{qn_l }),
               }
           );
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (first)
            {
                sc.RandomInitialize();
                first = false;
            }
            sc.RunGA(generationNum: 1000, stopOnFirstDecision: true);
            monDataGridView.Rows.Clear();
            tueDataGridView.Rows.Clear();
            wedDataGridView.Rows.Clear();
            thuDataGridView.Rows.Clear();
            friDataGridView.Rows.Clear();

            var res = sc.schedules.Max().Entries.OrderBy(x => x.time).ThenBy(x=>x.group).ToList();
            foreach (var item in res)
            {
                switch (item.day)
                {
                    case 0:
                        monDataGridView.Rows.Add(item.time + 1, item.group.groupName, item.course.name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 1:
                        tueDataGridView.Rows.Add(item.time +1, item.group.groupName, item.course.name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 2:
                        wedDataGridView.Rows.Add(item.time + 1, item.group.groupName, item.course.name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 3:
                        thuDataGridView.Rows.Add(item.time + 1, item.group.groupName, item.course.name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 4:
                        friDataGridView.Rows.Add(item.time + 1, item.group.groupName, item.course.name, item.room.RoomID, item.instructor.FullName);
                        break;
                    default:
                        throw new ArgumentException("day > 4");
                }
            }
        }
    }
}
