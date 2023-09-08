using System;

namespace ChangePassword
{
    /// <summary>  
    /// 给DataGridView添加全选  
    /// </summary>  
    public class AddCheckBoxToDataGridView1
    {
        public static System.Windows.Forms.DataGridView dgv;
        public static void AddFullSelect()
        {
            //if (dgv.Rows.Count < 1)
            //{
            //    return;
            //}
            System.Windows.Forms.CheckBox ckBox = new System.Windows.Forms.CheckBox();
            //ckBox.Text = "全选";
            //ckBox.Checked = true;
            System.Drawing.Rectangle rect =
                dgv.GetCellDisplayRectangle(0, -1, true);
            ckBox.Size = new System.Drawing.Size(15, 15);
            //ckBox.Location = rect.Location;
            ckBox.Location = new System.Drawing.Point(rect.Location.X + 60, rect.Location.Y + 8);
            ckBox.CheckedChanged += new EventHandler(ckBox_CheckedChanged);
            dgv.Controls.Add(ckBox);
        }
        static void ckBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows[i].Cells[0].Value = ((System.Windows.Forms.CheckBox)sender).Checked;
            }
            dgv.EndEdit();
        }
    }
    public class AddCheckBoxToDataGridView2
    {
        public static System.Windows.Forms.DataGridView dgv;
        public static void AddFullSelect()
        {
            //if (dgv.Rows.Count < 1)
            //{
            //    return;
            //}
            System.Windows.Forms.CheckBox ckBox = new System.Windows.Forms.CheckBox();
            //ckBox.Text = "全选";
            //ckBox.Checked = true;
            System.Drawing.Rectangle rect =
                dgv.GetCellDisplayRectangle(0, -1, true);
            ckBox.Size = new System.Drawing.Size(15, 15);
            //ckBox.Location = rect.Location;
            ckBox.Location = new System.Drawing.Point(rect.Location.X + 60, rect.Location.Y + 8);
            ckBox.CheckedChanged += new EventHandler(ckBox_CheckedChanged);
            dgv.Controls.Add(ckBox);
        }
        static void ckBox_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows[i].Cells[0].Value = ((System.Windows.Forms.CheckBox)sender).Checked;
            }
            dgv.EndEdit();
        }
    }
}
