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
            Course ms_l = new Course(8, "MSP Lec", 30, 1, ClassTypes.Lec);
            Course ks_l = new Course(9, "KSSQL Lec", 30, 2, ClassTypes.Lec);
            Course mn_p = new Course(10, "Manag Lab", 30, 1, ClassTypes.Lab);
            Course mn_l = new Course(11, "Manag Lec", 90, 1, ClassTypes.Lec);
            Course rf_l = new Course(12, "Refactoring Lab", 30, 1, ClassTypes.Lab);
            Course rf_p = new Course(13, "Refactoring Lec", 30, 1, ClassTypes.Lec);
            Course pa_l = new Course(14, "Parallel Lec", 30, 2, ClassTypes.Lec);
            Course pa_p = new Course(14, "Parallel Lab", 30, 2, ClassTypes.Lab);
            Course qn_l = new Course(15, "Quant Lec", 30, 2, ClassTypes.Lec);
            sc = new GenAlgorithm(
                _groups: new List<Group> {
                    new Group(0, "TTP", new HashSet<Course>() { is_p, kp_l, bi_l, ms_l, ks_l, is_l, kp_p, bi_p, mn_p }),
                    new Group(1, "MI", new HashSet<Course>() { pr_l, pr_p, mn_l, mn_p, rf_l, rf_p, pa_l, pa_p, qn_l })
                },
               _courses: new List<Course> { is_p, kp_l, bi_l, ms_l, ks_l, is_l, kp_p, bi_p, mn_p },
               _rooms: new List<Room>
               {
                   new Room(0, 90), new Room(1, 30), new Room(2, 30), new Room(3, 90), new Room(4, 90), new Room(5, 30), new Room(6, 30),
                   new Room(7, 90), new Room(8,30), new Room(9, 30), new Room(10, 90), new Room(11, 30), new Room(12, 30), new Room(13, 30), new Room(14, 90),
                   new Room(15, 90), new Room(16, 90)
               },
               _instructors: new List<Instructor> {
                   new Instructor(9, "Glybovecj", new HashSet<Course>(){is_l}),
                   new Instructor(0, "Fedorus", new HashSet<Course>{is_p}),
                   new Instructor(1,"Bohdan", new HashSet<Course>{ pr_p, pr_l}),
                   new Instructor(2,"Tkachenko", new HashSet<Course>{ kp_l,ks_l, kp_p}),
                   new Instructor(3, "Panch", new HashSet<Course>{bi_l, bi_p}),
                   new Instructor(4,"Vergun", new HashSet<Course>{mn_l, mn_p}),
                   new Instructor(5, "Shishka", new HashSet<Course>{ms_l}),
                   new Instructor(6, "Kuliabko", new HashSet<Course>{rf_l, rf_p }),
                   new Instructor(7, "Brevno", new HashSet<Course>{pa_l, pa_p }),
                   new Instructor(8, "Riabokonj", new HashSet<Course>{ qn_l }),
               }
           );
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (first)
            {
                sc.RandomInitialize();
                first = false;
                button1.Visible = true;
                btn_start.Text = "Заново";
            }
            sc.RunGA(stopOnFirstDecision: true);
            PopulateGrids(sc.schedules.Max());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sc.RunGA(minGenerations: 100);
            PopulateGrids(sc.schedules.Max());
        }

        private void PopulateGrids(Schedule schedule)
        {
            var s = schedule.Entries.OrderBy(x => x.time).ThenBy(x => x.group);
            monDataGridView.Rows.Clear();
            tueDataGridView.Rows.Clear();
            wedDataGridView.Rows.Clear();
            thuDataGridView.Rows.Clear();
            friDataGridView.Rows.Clear();
            foreach (var item in s)
            {
                switch (item.day)
                {
                    case 0:
                        monDataGridView.Rows.Add(item.time + 1, item.group.groupName, item.course.name, item.room.RoomID, item.instructor.FullName);
                        break;
                    case 1:
                        tueDataGridView.Rows.Add(item.time + 1, item.group.groupName, item.course.name, item.room.RoomID, item.instructor.FullName);
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
            critErrorsLabel.Visible = true;
            critErrorsLabel.Text = String.Format("{0} порушень жорстких обмежень, \n{1} порушень м'яких обмежень", schedule.violationCount, schedule.softCapCount);
        }
    }
}
