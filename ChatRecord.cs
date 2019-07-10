using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browserform
{
    public partial class ChatRecord : Form
    {
        #region 
   

        /// <summary>
        /// 一个消息包含2行数据。
        /// </summary>
        /// <param name="my"></param>
        /// <param name="fri"></param>
        /// <param name="info"></param>
        public void AddMessage(string my, string fri, MessageInfo info, bool IsGroup)
        {
            try
            {
                if (info.MessageType == 0)  //我发送的消息
                {
                    rtb_chatRecord.SelectionColor = Color.Blue;
                    rtb_chatRecord.AppendText(my + "   " + info.time.ToString() + "\n");
                    rtb_chatRecord.SelectionColor = Color.Black;
                    rtb_chatRecord.AppendText(info.Content + "\n");
                }
                else if (info.MessageType == 10)//我发送的群消息
                {
                    string splitchat = ":\r\n";
                    string Group_nickname = info.Content.Substring(0, info.Content.IndexOf(splitchat)).ToString();
                    string Group_Content = info.Content.Substring(info.Content.IndexOf(splitchat), info.Content.Length - info.Content.IndexOf(splitchat)).Replace(splitchat, "");
                    rtb_chatRecord.SelectionColor = Color.Blue;
                    rtb_chatRecord.AppendText(Group_nickname + "   " + info.time.ToString() + "\n");
                    rtb_chatRecord.SelectionColor = Color.Black;
                    rtb_chatRecord.AppendText(Group_Content + "\n");
                }
                else if (info.MessageType == 1)//普通好友的消息记录
                {
                    rtb_chatRecord.SelectionColor = Color.Green;
                    rtb_chatRecord.AppendText(info.FriendNick + "   " + info.time.ToString() + "\n");
                    rtb_chatRecord.SelectionColor = Color.Black;
                    rtb_chatRecord.AppendText(info.Content + "\n");
                }
                else if (info.MessageType == 3)//群里的好友消息
                {

                    string splitchat = ":\r\n";
                    string Group_nickname = info.Content.Substring(0, info.Content.IndexOf(splitchat)).ToString();
                    string Group_Content = info.Content.Substring(info.Content.IndexOf(splitchat), info.Content.Length - info.Content.IndexOf(splitchat)).Replace(splitchat, "");
                    rtb_chatRecord.SelectionColor = Color.Green;
                    rtb_chatRecord.AppendText(Group_nickname + "   " + info.time.ToString() + "\n");
                    rtb_chatRecord.SelectionColor = Color.Black;
                    rtb_chatRecord.AppendText(Group_Content + "\n");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        #region
        string DBName = "";
        string dbpath = "";
        /// <summary>
        /// 根据数据库ID加载聊天记录
        /// </summary>
        /// <param name="Wuin"></param>
        public ChatRecord(string Wuin,string RootPath)
        {           
            InitializeComponent();
            DBName = Wuin;
            dbpath = RootPath + "/db/" + DBName + ".db";
        }
        /// <summary>
        /// 当前的表明
        /// </summary>
        string TableNameNow="";
        private void lb_friend_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.rtb_chatRecord.Text = "";
            if (this.lb_friend.SelectedItem == null)
                return;
            string tabname = this.lb_friend.SelectedItem.ToString();
            TableNameNow = tabname;
            GetInfoByTableName(1);
        }
        #region 分页聊天记录 
        //每页十条 
        int PageCount = 20;
        //当前页码
        int pageIndex = 1;
        //总共多少页
        int SumCountPage = 0;

        private void GetInfoByTableName(int pageIndex)
        {
            string tabname = TableNameNow;
            if (tabname != null && tabname != "")
            {
                try
                {
                    ShouPageInfo(tabname, pageIndex);


                    //开始位置
                    string sql = String.Empty;
                    int StartIndex = 1;
                    if (pageIndex == 1)
                    {
                        StartIndex = 1;
                    }
                    else {
                        StartIndex=PageCount* pageIndex;
                    }
                    if (StartIndex == 1) //解决一条时差不到
                    {
                        StartIndex--;
                        sql = String.Format("select * from '{0}' LIMIT {1},{2}; ", tabname, StartIndex, PageCount);
                    }
                    else {
                        sql = String.Format("select * from '{0}' LIMIT {1},{2}; ", tabname, PageCount ,StartIndex);
                    }

                    MyDB.SQLiteDBHelper db = new MyDB.SQLiteDBHelper(dbpath);

                    DataTable dt = db.ExecuteDataTable(sql, null);
                    rtb_chatRecord.Text = "";
                    bool IsGroup= IsGroupChat(dt);
                    foreach (DataRow item in dt.Rows)
                    {
                        MessageInfo info = new MessageInfo();
                        info.Content = item["Content"].ToString();
                        info.MessageType = Convert.ToInt32(item["Type"].ToString());
                        info.time = item["Time"].ToString();
                        info.FriendNick = tabname;
                        AddMessage("我", "", info, IsGroup);
                    }



                }
                catch (Exception ex)
                {
                    this.rtb_chatRecord.Text = "";
                }
            }
        }
        /// <summary>
        /// 表里包含群消息不 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private bool IsGroupChat(DataTable dt)
        {
            foreach (DataRow item in dt.Rows)
            {
                if (item["Type"].ToString().Trim() == "3") //是不不是来自群消息？
                {
                    return  true;
                }
            }
            return false;
        }
        /// <summary>
        /// 显示当前表的聊天记录信息 包含页数和条数 当前的页数。
        /// </summary>
        /// <param name="tablename"></param>
        private void ShouPageInfo(string tablename,int pageIndex)
        {
            string sql = String.Format("select Count(*) 'Count' from '{0}'", tablename);
            MyDB.SQLiteDBHelper db = new MyDB.SQLiteDBHelper(dbpath);
            DataTable dt = db.ExecuteDataTable(sql, null);

            string SumCount = dt.Rows[0]["Count"].ToString();
            if (Convert.ToInt32(SumCount) < PageCount) //不足一页的条目
            {
                SumCountPage = 1;
            }
            else {
                SumCountPage = Convert.ToInt32(SumCount) / PageCount+1;
            }
           

            label2_PageShow.Text = String.Format("共{0}条记录,当前第{1}页,共{2}页", SumCount, pageIndex,SumCountPage);
            
        }

        #endregion

        private void ChatRecord_Load(object sender, EventArgs e)
        {
            try
            {
                initDataLog(dbpath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载聊天记录"+ex.ToString());
            }
        }
        /// <summary>
        /// 根据数据库地址加载聊天记录
        /// </summary>
        /// <param name="dbapth"></param>
        private void initDataLog(string dbapth)
        {
            if (File.Exists(dbapth) == false)
                return;
            MyDB.SQLiteDBHelper db = new MyDB.SQLiteDBHelper(dbapth);

            DataTable dt = db.GetSchema();
            foreach (DataRow item in dt.Rows)
            {
                if (item["TABLE_TYPE"].ToString() == "table" && item["TABLE_NAME"].ToString() != "" && item["TABLE_NAME"].ToString() != "好友列表")
                    lb_friend.Items.Add(item["TABLE_NAME"].ToString());
            }
        }

     
        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Up_Click(object sender, EventArgs e)
        {
            pageIndex--;
            if (pageIndex == 0) {
                pageIndex = 1;
                return;
            }
               
            GetInfoByTableName(pageIndex);
        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_down_Click(object sender, EventArgs e)
        {
            pageIndex++;
            if (pageIndex > SumCountPage)
            {
                pageIndex = SumCountPage;
                return;
            }

            GetInfoByTableName(pageIndex);
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_top_Click(object sender, EventArgs e)
        {
            GetInfoByTableName(1);
        }
        /// <summary>
        /// 尾页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_bot_Click(object sender, EventArgs e)
        {
            GetInfoByTableName(SumCountPage);
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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


        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.DarkRed;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.RoyalBlue;
        }

        private void btnMinimize_MouseEnter(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.DarkRed;
        }

        private void btnMinimize_MouseLeave(object sender, EventArgs e)
        {
            btnMinimize.BackColor = Color.RoyalBlue;
        }

        private void rtb_chatRecord_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                int index = ((System.Windows.Forms.RichTextBox)sender).Text.ToString().IndexOf("file:");
                string filepath = ((System.Windows.Forms.RichTextBox)sender).Text.ToString().Substring(index);
                openimg(filepath.Replace("file:", ""));
            }
            catch (Exception ex)
            { }
        }
        private void openimg(string imgpath)
        {
            try
            {
                if (!File.Exists(imgpath))
                    return;
                Image img = Image.FromFile(imgpath);
                Form frm = new Form();
                frm.Text = imgpath;
                PictureBox pic = new PictureBox();
                pic.Dock = DockStyle.Fill;
                pic.Image = img;
                frm.Controls.Add(pic);
                frm.Width = 400;
                frm.Height= 400;
                frm.FormBorderStyle = FormBorderStyle.FixedDialog;
                frm.ShowDialog();
            }
            catch (Exception ex)
            { }
        }
    }
}
