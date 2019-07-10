using Browserform.common;
using MyDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browserform.Frm
{
    public partial class FrmGroupUser : Form
    {
        //注解 构造函数 执行form1的方法 给当前窗口赋值 在Load中加载出来

        public FrmGroupUser()
        {
            InitializeComponent();
            Form1.Instance.GroupUserAdd1();
        }
        /// <summary>
        /// 群组 名称 集合 
        /// </summary>
        public static List<common.GroupUserAndNickName> List_GroupName = new List<common.GroupUserAndNickName>();
        /// <summary>
        /// 群成员 集合 
        /// </summary>
        public static List<common.MemberList> Group_MemberList = new List<common.MemberList>();

        /// <summary>
        /// 群组加人 信息
        /// </summary>
        public static  common.AddGroupUser info = new common.AddGroupUser();

        public static List<common.AddGroupUser> listinfo = new List<AddGroupUser>();

        /// <summary>
        /// Cookie
        /// </summary>
        public static CookieContainer myCookieContainer = new CookieContainer();

        /// <summary>
        /// 新老微信标识 
        /// </summary>
        public static int WxorWx2 = 1;
        /// <summary>
        /// 登录信息 
        /// </summary>
        public static LoginRedirectResult loginRedirectResult=new LoginRedirectResult();

        public FrmGroupUser(List<common.GroupUserAndNickName> list_GroupName1,List<common.MemberList> group_MemberList1, AddGroupUser info1, CookieContainer mycookieContainer1,int wxorWx21)
        {
            InitializeComponent();
            List_GroupName = list_GroupName1;
            Group_MemberList = group_MemberList1;
            info = info1;
            myCookieContainer = mycookieContainer1;
            WxorWx2 = wxorWx21;
        }

        private void FrmGroupUser_Load(object sender, EventArgs e)
        {
            try
            {
                FrmGroupUser.CheckForIllegalCrossThreadCalls = false;
                this.comboBox1_groupName.SelectedIndexChanged -= comboBox1_groupName_SelectedIndexChanged;
                init(Group_MemberList);
               // AddFullSelect(dataGridView1);
                InitGroup();
                this.comboBox1_groupName.SelectedIndexChanged += comboBox1_groupName_SelectedIndexChanged;

               // this.dataGridView1.RowHeadersVisible = false;
                comboBox1_groupName_SelectedIndexChanged(null,null);

                this.button1_Add.Click += button1_Add_Click;
                this.button1_stop.Click +=button1_stop_Click;
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.ToString());
            }

        }



        /// <summary>
        /// 初始化群组 
        /// </summary>
        private void InitGroup()
        {
            comboBox1_groupName.DataSource = List_GroupName;
            comboBox1_groupName.ValueMember = "UserName";
            comboBox1_groupName.DisplayMember = "NickName";
        }

        /// <summary>
        /// 初始化数据表 
        /// </summary>
        private void init(List<common.MemberList> Group_MemberList)
        {
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            DataGridViewCheckBoxColumn f = new DataGridViewCheckBoxColumn();
            f.Name = "选择";
            dataGridView1.Columns.Add(f);
            dataGridView1.Columns.Add("NickName", "昵称");
            dataGridView1.Columns.Add("GroupName", "群组");
            dataGridView1.Columns.Add("UserName", "序号");
            dataGridView1.Columns.Add("State","状态");
            if (Group_MemberList.Count == 0)
                return;
            for (int i = 0; i < Group_MemberList.Count; i++)
            {
                this.dataGridView1.Rows.Add(0, Group_MemberList[i].NickName, GetNickNameByUserName(Group_MemberList[i].GroupUserName), Group_MemberList[i].UserName,"");
            }

            //for (int i = 0; i < 100; i++)
            //{

            //    this.dataGridView1.Rows.Add(0, "NickName:"+i, "GroupName:" + i, "UserName:" + i);
            //}
            dataGridView1.Columns["UserName"].Visible = false;
        }
        private string GetNickNameByUserName(string groupUserName)
        {
            foreach (var item in List_GroupName)
            {
                if (item.UserName == groupUserName)
                    return item.NickName;
            }
            return "";
        }
        /// <summary>
        /// 添加 选中的群组里的人。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Add_Click(object sender, EventArgs e)
        {

            //if (CheCountGroup() > 5)
            //{
            //    MessageBox.Show("加人每次不允许大于五个！");
            //    return;
            //}
            this.button1_Add.Enabled = false;
            ///初始化 要加的群好友数据
            AddUserList();
            addIndex = 0;
            TimerAddUser();

        }
        /// <summary>
        /// 加群好友 
        /// </summary>
        private void  AddUserList()
        {
            listinfo.Clear();
            int count = dataGridView1.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true)
                {
                    common.AddGroupUser info1 = new AddGroupUser();
                    string groupname = dataGridView1.Rows[checkCell.RowIndex].Cells["GroupName"].FormattedValue.ToString();
                    string nickname = dataGridView1.Rows[checkCell.RowIndex].Cells["NickName"].FormattedValue.ToString();
                    string username = dataGridView1.Rows[checkCell.RowIndex].Cells["UserName"].FormattedValue.ToString();

                    info1.NickName = nickname;
                    info1.Value = username;
                    if (info1.Value == "")
                        return;

                    listinfo.Add(info1);
                }
            }
        }
        System.Timers.Timer t;
        /// <summary>
        /// 时间 间隔加人
        /// </summary>
        private void TimerAddUser()
        {
            int num = Convert.ToInt32(numericUpDown1.Value)*1000;
            t = new System.Timers.Timer(num);//实例化Timer类，设置间隔时间为10000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler(theout);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            t.Start();
        }
        int addIndex = 0;
        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            if (addIndex > listinfo.Count)
            {
                t.Stop();
            }
            else
            {
                //加群友
                AddGroupUser(addIndex);
                addIndex++;
            }
            if (addIndex - 1 == listinfo.Count)
            {
                try
                {
                    this.button1_Add.Invoke(new Action(() =>
                    {
                        this.button1_Add.Enabled = true;
                    }));
                }
                catch{ }
            }

        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="addIndex"></param>
        private void AddGroupUser(int addIndex)
        {
            for (int i = 0; i < listinfo.Count; i++)
            {
                if (i == addIndex)
                {

                    info.Value = listinfo[i].Value;
                    info.NickName = listinfo[i].NickName;
                    info.VerifyContent = this.textBox1_VerifyContent.Text.ToString().Trim();


                    JObject job = new common.WeChatGroup().AddGroupUser(info, myCookieContainer, WxorWx2);
                    string ret = job["BaseResponse"]["Ret"].ToString();
                    //string ret = "0";
                    if (ret == "0")
                    {
                        RefDataShow(true,info.Value);
                    }
                    else
                    {
                        RefDataShow(false, info.Value);
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 更新显示数据
        /// </summary>
        private void RefDataShow(bool flag,string group_username)
        {
            dataGridView1.Invoke(new Action(()=>{
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                   // Console.WriteLine(item.Cells["UserName"].Value.ToString()+"\r\n");
                    if (item.Cells["UserName"].Value.ToString()==group_username)
                    {
                        if (flag == true)
                        {
                            item.Cells["State"].Value = "已申请加好友";
                        }
                        else {
                            item.Cells["State"].Value = "申请加好友失败";
                        }
                    }
                }
            }));
        }

            /// <summary>
            /// 检查勾选数目     
            /// </summary>
            /// <returns></returns>
            private int CheCountGroup()
        {
            int count = 0;
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell che = (DataGridViewCheckBoxCell)dataGridView1.Rows[i].Cells[0];
                if (che == null)
                    return 0;
                if (Convert.ToBoolean(che.Value)==true)
                {
                    count++;
                }
            }
            return count;
        }

        #region 全选的复选框 设置
        private DataGridView DGV;
        public  void AddFullSelect(DataGridView  dgv)
        {
            DGV = dgv;
            //if (dgv.Rows.Count< 1)  
            //{  
            //    return;  
            //}  
            System.Windows.Forms.CheckBox ckBox = new System.Windows.Forms.CheckBox();  
            ckBox.Text = "全选";  
            ckBox.Checked = false;  
            System.Drawing.Rectangle rect =
            dgv.GetCellDisplayRectangle(0, -1,true);  
            ckBox.Size = new System.Drawing.Size(dgv.Columns[0].Width, 18);  
            ckBox.Location = rect.Location;  
            ckBox.CheckedChanged += new EventHandler(ckBox_CheckedChanged);  
            dgv.Controls.Add(ckBox);  
        }  
        /// <summary>
        /// 全选事件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ckBox_CheckedChanged(object sender, EventArgs e)
        {  
            for (int i = 0; i< DGV.Rows.Count; i++)  
            {
                DGV.Rows[i].Cells[0].Value = ((System.Windows.Forms.CheckBox) sender).Checked;  
            }
            DGV.EndEdit();  
        }
        /// <summary>
        /// 群组 改变选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_groupName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1_groupName.SelectedItem == null)
            {
                MessageBox.Show("未在通讯录内找到任何群组,请在群中留言或将群加入到通讯录","通讯录未发现群组",MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.Invoke(new Action(()=> {
                string GroupUserName = ((Browserform.common.GroupUserAndNickName)comboBox1_groupName.SelectedItem).UserName;
                common.Noumenon_GetGroupUser info = new common.WeChatGroup().GetGroupInfo(GroupUserName, myCookieContainer, WxorWx2, loginRedirectResult);
                List<common.MemberList> list = new common.WeChatGroup().GetALLUser(info, WxorWx2, myCookieContainer);
                init(list);

            }));
            
            return;
           
            //if (GroupUserName != "")
            //{
            //    List<common.MemberList> NewGroup_MemberList = new List<MemberList>();
            //    for (int j = 0; j < Group_MemberList.Count; j++)
            //    {
            //        if (Group_MemberList[j].GroupUserName == GroupUserName)
            //        {
            //            NewGroup_MemberList.Add(Group_MemberList[j]);
            //        }
            //        init(NewGroup_MemberList);
            //    }

            //}
            //else {

            //}
        }
        #endregion




        private void btn_close_Click(object sender, EventArgs e)
        {
            if (this.button1_Add.Enabled == false)
            {
                if (DialogResult.OK == MessageBox.Show("正在加人中是否退出？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    //保存缓存数据到数据库
                    // wordOperation.UpdateAll(wordCache);
                    //保存
                    t.Stop();
                    this.Close();
                }
            }
            else {
                this.Close();
            }

         
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);//*********************调用移动无窗体控件函数  

        }

        //定义无边框窗体Form  
        [DllImport("user32.dll")]//*********************拖动无窗体的控件  
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;

        private void button1_stop_Click(object sender, EventArgs e)
        {
            try
            {
                t.Stop();
                this.button1_Add.Enabled = true;
            }catch{ }
        }
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chebox_ALL_CheckedChanged(object sender, EventArgs e)
        {
            if (chebox_ALL.Checked)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = ((System.Windows.Forms.CheckBox)sender).Checked;
                }
            }
            else {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = false;
                }

            }
        }
    }
}
