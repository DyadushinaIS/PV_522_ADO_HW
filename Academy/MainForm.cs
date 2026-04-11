using DBtools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Academy
{
	public partial class MainForm : Form
	{
		Connector connector;
		DataGridView[] tables;
		Query[] queries =
			{
				new Query
				(
					"stud_id,last_name,first_name,middle_name,birth_date,group_name,direction_name",
					"Students,Groups,Directions",
					"[group]=group_id AND direction=direction_id"
				),
				
			/*start_date,start_time,learning_days,*/
			new Query
				(
					"group_id,group_name,    direction_name",
					"Groups,Directions",
					"direction=direction_id"
				),
				new Query("*", "Directions"),
				new Query("*", "Disciplines"),
				new Query("*", "Teachers")                
            };
		public MainForm()
		{
			InitializeComponent();

			tables = new DataGridView[] { dgvStudents, dgvGroups, dgvDirections, dgvDisciplines, dgvTeachers };

			connector = new Connector(ConfigurationManager.ConnectionStrings["PV_522_Import"].ConnectionString);
			//dgvDirections.DataSource = connector.Select("SELECT * FROM Directions");
			tabControl_SelectedIndexChanged(tabControl, null);

			cbGroupsDirection.DataSource = connector.Load("SELECT direction_id, direction_name FROM Directions");
			cbGroupsDirection.DisplayMember = "direction_name";
			cbGroupsDirection.ValueMember = "direction_id";


            cbStudentGroup.DisplayMember = "group_name";
            cbStudentGroup.ValueMember = "group_id";
            cbStudentGroup.DataSource = connector.Load(queries[1].ToString());
            //SELECT group_name, group_id FROM Groups

            cbStudentDirection.DisplayMember = "direction_name";
            cbStudentDirection.ValueMember = "direction_id";
            cbStudentDirection.DataSource = connector.Load("SELECT direction_id, direction_name FROM Directions");
        }

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = (sender as TabControl).SelectedIndex;   //Получаем номер выбранной вкладки
			tables[i].DataSource = connector.Load(queries[i].ToString());
			//tables[i].DataSource = connector.Select("*", tabControl.SelectedTab.Text);
			toolStripStatusLabel.Text = $"Количество записей: {tables[i].RowCount-1}";

			cbStudentGroup.DataSource=connector.Load($"SELECT * FROM Groups");
		}

		private void cbGroupsDirection_SelectionChangeCommitted(object sender, EventArgs e)
		{
			dgvGroups.DataSource = connector.Load
				(
				queries[1].ToString() + $" AND direction={cbGroupsDirection.SelectedValue}"
				);
			toolStripStatusLabel.Text = $"Количество записей: {dgvGroups.RowCount - 1}";
		}

        // Для фильтрации по группе
       
        private void cbStudentGroup_SelectionChangeCommitted_1(object sender, EventArgs e)
        {
            dgvStudents.DataSource = connector.Load
        (
                    queries[0].ToString() + $" AND group_id={cbStudentGroup.SelectedValue}"
        );            
               toolStripStatusLabel.Text = $"Количество записей: {dgvStudents.RowCount - 1}";
        }

        private void cbStudentDirection_SelectionChangeCommitted_1(object sender, EventArgs e)
        {
            dgvStudents.DataSource = connector.Load
                (
                queries[0].ToString() + $" AND direction_id={cbStudentDirection.SelectedValue}"
                );
			cbStudentGroup.DataSource = connector.Load($"SELECT * FROM Groups WHERE direction={cbStudentDirection.SelectedValue}");
            toolStripStatusLabel.Text = $"Количество записей: {dgvStudents.RowCount - 1}";
        }
    }
}
