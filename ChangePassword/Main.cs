using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TradingAPI.MT4Server;

namespace ChangePassword
{
    public partial class Main : Form
    {
        static BindingList<ListSer> serlist = new BindingList<ListSer>();
        static BindingList<ListData1> list1 = new BindingList<ListData1>();
        static BindingList<ListData2> list2 = new BindingList<ListData2>();

        static string SrvPath = AppDomain.CurrentDomain.BaseDirectory + "\\srv\\";
        public Main()
        {
            InitializeComponent();
            AddCheckBoxToDataGridView1.dgv = dataGridView1;
            AddCheckBoxToDataGridView1.AddFullSelect();

            AddCheckBoxToDataGridView2.dgv = dataGridView2;
            AddCheckBoxToDataGridView2.AddFullSelect();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(SrvPath))
            {
                var files = Directory.GetFiles(SrvPath, "*.srv");
                foreach (var file in files)
                {
                    serlist.Add(new ListSer
                    {
                        SerSrv = Path.GetFileNameWithoutExtension(file)
                    });
                }
            }
            else
            {
                Directory.CreateDirectory(SrvPath);
            }
            GridSer.DataSource = serlist;
            dataGridView1.DataSource = list1;
            dataGridView2.DataSource = list2;

            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now.AddDays(1);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        #region 平台添加/删除
        private void AddSrv_Click(object sender, EventArgs e)
        {
            if (oFD_srv.ShowDialog() == DialogResult.OK)
            {
                string[] srvNames = oFD_srv.FileNames;
                for (int i = 0; i < srvNames.Length; i++)
                {
                    string fName = Path.GetFileNameWithoutExtension(srvNames[i]);//取文件名 
                    string userSrvPath = SrvPath + fName + ".srv";
                    if (File.Exists(userSrvPath))
                    {
                        if ((int)MessageBox.Show(fName + "平台已存在，是否覆盖", "提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != 6) { continue; }
                    }
                    else
                    {
                        serlist.Add(new ListSer
                        {
                            SerSrv = fName,
                        });
                    }
                    File.Copy(srvNames[i], userSrvPath, true);
                }
            }
        }
        private void GridSer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (GridSer.Columns[e.ColumnIndex].Name == "cz")
            {
                string ser = GridSer.Rows[e.RowIndex].Cells["pt"].Value.ToString();

                //1.删除文件
                string userSrvPath = SrvPath + ser + ".srv";
                if (File.Exists(userSrvPath))
                {
                    //File.SetAttributes(userSrvPath, FileAttributes.Normal);
                    File.Delete(userSrvPath);
                }

                //2.删除serlist里的平台信息
                ListSer si = serlist.Where(u => u.SerSrv == ser).FirstOrDefault();
                serlist.Remove(si);
            }
        }
        #endregion

        #region  绑定datagridview的序号列 添加平台页面
        public static void GridRowPostPaint(DataGridView dgv, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dgv.RowHeadersWidth - 4,
                e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgv.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dgv.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        private void GridSer_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            GridRowPostPaint(GridSer, e);
        }
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            GridRowPostPaint(dataGridView1, e);
        }
        private void dataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            GridRowPostPaint(dataGridView2, e);
        }
        #endregion

        private void ChkFile_Click(object sender, EventArgs e)
        {
            if (oFD_txt.ShowDialog() == DialogResult.OK)
            {   
                string srvNames = oFD_txt.FileName;
                switch(tabControl1.SelectedIndex)
                {
                    case 0:
                        list1.Clear();
                        ReadTxt(srvNames, true);
                        break;
                    case 1:
                        list2.Clear();
                        ReadTxt(srvNames, false);
                        break;
                }                  
            }
        }
        void ReadTxt(string path, bool mark)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadLine();//这行就是第一行的内容
                while (sr.Peek() != -1)
                {
                    line = sr.ReadLine();
                    string[] str = line.Split(',');
                    if (mark)//修改密码
                    {
                        if (str.Length < 5)
                        {
                            MessageBox.Show("导入文件格式有误，请核对");
                        }
                        else
                        {
                            list1.Add(new ListData1
                            {
                                Srv = str[0].Trim(),
                                login = str[1].Trim(),
                                pwd = str[2].Trim(),
                                new1 = str[3].Trim(),
                                new2 = str[4].Trim()
                            });
                        }
                    }
                    else//查询历史
                    {
                        if (str.Length < 3)
                        {
                            MessageBox.Show("导入文件格式有误，请核对");
                        }
                        else
                        {
                            list2.Add(new ListData2
                            {
                                Srv = str[0].Trim(),
                                login = str[1].Trim(),
                                pwd = str[2].Trim()
                            });
                        }
                    }
                }               
            }
        }

        private void ChangePwd_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false && checkBox2.Checked == false) return;
            LoadingHelper.ShowLoadingScreen();//显示
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((bool)dataGridView1.Rows[i].Cells[0].EditedFormattedValue == true)
                {
                    string Srv = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    string _login = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    var l = list1.Where(o => o.Srv == Srv && o.login == _login).FirstOrDefault();
                    if (l == null)
                    {
                        dataGridView1.Rows[i].Cells[7].Value = "失败:账号未查到";
                        continue;
                    }

                    if (int.TryParse(_login, out int login))
                    {
                        if (l.new1 == null)
                        {
                            l.result = "失败:新密码不能为空";
                            continue;
                        }
                        if (l.new2 == null)
                        {
                            l.result = "失败:新密码不能为空";
                            continue;
                        }
                        if (l.new1.Trim() == "")
                        {
                            l.result = "失败:新密码不能为空";
                            continue;
                            //if (t1 == "")
                            //{

                            //}
                            //l.new1 = t1;
                        }
                        if (l.new2.Trim() == "")
                        {
                            l.result = "失败:新密码不能为空";
                            continue;
                            //if (t2 == "")
                            //{

                            //}
                            //l.new2 = t2;
                        }

                        CP cp = new CP
                        {
                            ld = l,
                            changeTrade = checkBox1.Checked,
                            changeInvestor = checkBox2.Checked
                        };
                        ChangePassword(cp);
                    }
                    else
                    {
                        l.result = "失败:账号不是数字";
                    }
                }
            }          
            LoadingHelper.CloseForm();//关闭
        }
        void ChangePassword(object obj)
        {
            CP cp = (CP)obj;
            string result;
            string log;
            try
            {
                MainServer primary = QuoteClient.LoadSrv($"{SrvPath}{cp.ld.Srv}.srv", out Server[] slaves);
                QuoteClient qc = Connect(primary, slaves, int.Parse(cp.ld.login), cp.ld.pwd);

                if (cp.changeInvestor)//true 改观摩密码
                {
                    qc.ChangePassword(cp.ld.new2, true);
                }
                if (cp.changeTrade)//false 改交易密码
                {
                    qc.ChangePassword(cp.ld.new1, false);
                }

                qc.Disconnect();
                result = "成功";
                log = $"{cp.ld.login}ChangePassword:成功(pwd:{cp.ld.pwd}-new1:{cp.ld.new1}-new2:{cp.ld.new2})";
            }
            catch (Exception ex)
            {
                result = $"失败:{ex.Message}";
                log = $"{cp.ld.login}ChangePassword:失败:{ex.Message}(pwd:{cp.ld.pwd}-new1:{cp.ld.new1}-new2:{cp.ld.new2})";
            }
            this.Invoke((Action)(() =>
            {
                cp.ld.result = result;
            }));
            MyLog.LogInfo(log);
        }
        private void ExportPwd_Click(object sender, EventArgs e)
        {
            if (list1.Count > 0)
            {
                if (sFD_txt.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sFD_txt.FileName))
                    {
                        sw.WriteLine($"平台,账号,新交易密码,新观摩密码,结果");
                        foreach (var s in list1)
                        {
                            if (s.result == "成功")
                                sw.WriteLine($"{s.Srv},{s.login},{s.new1},{s.new2},{s.result}");
                        }
                        MessageBox.Show("导出成功");
                    }
                }
            }
        }

        private void CheckProfit_Click(object sender, EventArgs e)
        {
            LoadingHelper.ShowLoadingScreen();//显示
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                if ((bool)dataGridView2.Rows[i].Cells[0].EditedFormattedValue == true)
                {
                    string Srv = dataGridView2.Rows[i].Cells[1].Value.ToString();
                    string _login =  dataGridView2.Rows[i].Cells[2].Value.ToString();
                    string pwd = dataGridView2.Rows[i].Cells[3].Value.ToString();

                    var l = list2.Where(o => o.Srv == Srv && o.login == _login).FirstOrDefault();
                    if (l == null)
                    {
                        dataGridView2.Rows[i].Cells[9].Value = "失败:账号未查到";
                        continue;
                    }
                    if (int.TryParse(_login, out int login))
                    {
                        SumProfit(l);
                    }
                    else
                    {
                        l.result = "失败:账号不是数字";
                    }
                }
            }
            LoadingHelper.CloseForm();//关闭
        }
        void SumProfit(object obj)
        {
            ListData2 ld = (ListData2)obj;
            string result;
            string log;
            double balance = 0, commission = 0, taxes = 0, swap = 0, profit = 0,pl=0;
            try
            {
                MainServer primary = QuoteClient.LoadSrv($"{SrvPath}{ld.Srv}.srv", out Server[] slaves);
                QuoteClient qc = Connect(primary, slaves, int.Parse(ld.login), ld.pwd);
                Order[] item = qc.DownloadOrderHistory(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date).Where(o=>o.Type==Op.Buy|| o.Type ==Op.Sell).ToArray();

                balance = Math.Round(qc.AccountBalance, 2);
                commission = Math.Round(item.Sum(o => o.Commission), 2); 
                taxes= Math.Round(item.Sum(o => o.Ex.taxes), 2);
                swap = Math.Round(item.Sum(o => o.Swap), 2);
                profit = Math.Round(item.Sum(o => o.Profit), 2);
                pl = Math.Round(commission + taxes+ swap+ profit, 2);
                qc.Disconnect();

                result = "成功";
                log=$"{ld.login}SumProfit:成功";
            }
            catch (Exception ex)
            {
                result = $"失败:{ex.Message}";
                log = $"{ld.login}SumProfit:失败:{ex.Message}";
            }
            this.Invoke((Action)(() =>
            {
                ld.balance = balance;
                ld.pl = pl;
                ld.commission = commission;
                ld.taxes = taxes;
                ld.swap = swap;
                ld.profit = profit;
                ld.result = result;
            }));
            MyLog.LogInfo(log);
        }
        private void ExportProfit_Click(object sender, EventArgs e)
        {
            if (list2.Count > 0)
            {
                if (sFD_txt0.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sFD_txt0.FileName))
                    {
                        sw.WriteLine($"平台,账号,余额,盈亏,手续费,税金,库存费,获利,结果");
                        foreach (var s in list2)
                        {
                            if (s.result == "成功")
                                sw.WriteLine($"{s.Srv},{s.login},{s.balance},{s.pl},{s.commission},{s.taxes},{s.swap},{s.profit},{s.result}");
                        }
                        MessageBox.Show("导出成功");
                    }
                }
            }
        }

        QuoteClient Connect(MainServer primary, Server[] slaves, int user, string password)
        {
            QuoteClient qc = new QuoteClient(user, password, primary.Host, primary.Port);
            try
            {
                qc.Connect();
                return qc;
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "Invalid account":
                        throw new Exception("Invalid account");
                    case "Account disabled":
                        throw new Exception("Account disabled");
                }
                return ConnectSlaves(slaves, user, password);
            }
        }
        QuoteClient ConnectSlaves(Server[] slaves, int user, string password)
        {
            foreach (var server in slaves)
            {
                QuoteClient qc = new QuoteClient(user, password, server.Host, server.Port);
                try
                {
                    qc.Connect();
                    return qc;
                }
                catch (Exception)
                {
                }
            }
            throw new Exception("Cannot connect to slaves");
        }
    }

    class ListSer
    {
        public string SerSrv { get; set; }
    }
    class ListData1 : INotifyPropertyChanged
    {
        public string Srv { get; set; }
        public string login { get; set; }
        public string pwd { get; set; }

        private string _new1;
        public string new1
        {
            get { return _new1; }
            set { _new1 = value; NotifyPropertyChanged("new1"); }
        }

        private string _new2;
        public string new2
        {
            get { return _new2; }
            set { _new2 = value; NotifyPropertyChanged("new2"); }
        }

        private string _result;
        public string result
        {
            get { return _result; }
            set { _result = value; NotifyPropertyChanged("result"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            PropertyChangedEventHandler property_changed = PropertyChanged;
            if (property_changed != null)
            {
                property_changed(this, new PropertyChangedEventArgs(info));
            }
        }
    }
    class ListData2 : INotifyPropertyChanged
    {
        public string Srv { get; set; }
        public string login { get; set; }
        public string pwd { get; set; }

        private double _balance;
        public double balance
        {
            get { return _balance; }
            set { _balance = value; NotifyPropertyChanged("balance"); }
        }

        private double _pl;
        public double pl
        {
            get { return _pl; }
            set { _pl = value; NotifyPropertyChanged("pl"); }
        }

        private double _commission;
        public double commission
        {
            get { return _commission; }
            set { _commission = value; NotifyPropertyChanged("commission"); }
        }

        private double _taxes;
        public double taxes
        {
            get { return _taxes; }
            set { _taxes = value; NotifyPropertyChanged("taxes"); }
        }

        private double _swap;
        public double swap
        {
            get { return _swap; }
            set { _swap = value; NotifyPropertyChanged("swap"); }
        }

        private double _profit;
        public double profit
        {
            get { return _profit; }
            set { _profit = value; NotifyPropertyChanged("profit"); }
        }

        private string _result;
        public string result
        {
            get { return _result; }
            set { _result = value; NotifyPropertyChanged("result"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            PropertyChangedEventHandler property_changed = PropertyChanged;
            if (property_changed != null)
            {
                property_changed(this, new PropertyChangedEventArgs(info));
            }
        }

    }
    class CP//ChangePassword
    {
        public ListData1 ld { get; set; }
        public bool changeTrade { get; set; }
        public bool changeInvestor { get; set; }
    }  
}
