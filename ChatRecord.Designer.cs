namespace Browserform
{
    partial class ChatRecord
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatRecord));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rtb_chatRecord = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lb_friend = new System.Windows.Forms.ListBox();
            this.button1_Up = new System.Windows.Forms.Button();
            this.button1_down = new System.Windows.Forms.Button();
            this.label2_PageShow = new System.Windows.Forms.Label();
            this.button1_top = new System.Windows.Forms.Button();
            this.button2_bot = new System.Windows.Forms.Button();
            this.pTitleBackground = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.PictureBox();
            this.btnMinimize = new System.Windows.Forms.PictureBox();
            this.pBackgroundBorder = new System.Windows.Forms.Panel();
            this.pBackground = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pTitleBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            this.pBackgroundBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.4163F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 77.58369F));
            this.tableLayoutPanel1.Controls.Add(this.rtb_chatRecord, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 31);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(676, 444);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // rtb_chatRecord
            // 
            this.rtb_chatRecord.BackColor = System.Drawing.Color.White;
            this.rtb_chatRecord.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtb_chatRecord.Location = new System.Drawing.Point(154, 3);
            this.rtb_chatRecord.Name = "rtb_chatRecord";
            this.rtb_chatRecord.ReadOnly = true;
            this.rtb_chatRecord.Size = new System.Drawing.Size(519, 438);
            this.rtb_chatRecord.TabIndex = 1;
            this.rtb_chatRecord.Text = "";
            this.rtb_chatRecord.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtb_chatRecord_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lb_friend);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(145, 438);
            this.panel1.TabIndex = 2;
            // 
            // lb_friend
            // 
            this.lb_friend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb_friend.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lb_friend.ForeColor = System.Drawing.Color.Blue;
            this.lb_friend.FormattingEnabled = true;
            this.lb_friend.ItemHeight = 20;
            this.lb_friend.Location = new System.Drawing.Point(0, 0);
            this.lb_friend.Margin = new System.Windows.Forms.Padding(0);
            this.lb_friend.Name = "lb_friend";
            this.lb_friend.Size = new System.Drawing.Size(145, 442);
            this.lb_friend.TabIndex = 1;
            this.lb_friend.SelectedIndexChanged += new System.EventHandler(this.lb_friend_SelectedIndexChanged);
            // 
            // button1_Up
            // 
            this.button1_Up.Location = new System.Drawing.Point(529, 491);
            this.button1_Up.Name = "button1_Up";
            this.button1_Up.Size = new System.Drawing.Size(33, 23);
            this.button1_Up.TabIndex = 1;
            this.button1_Up.Text = "<";
            this.button1_Up.UseVisualStyleBackColor = true;
            this.button1_Up.Click += new System.EventHandler(this.button1_Up_Click);
            // 
            // button1_down
            // 
            this.button1_down.Location = new System.Drawing.Point(568, 491);
            this.button1_down.Name = "button1_down";
            this.button1_down.Size = new System.Drawing.Size(29, 23);
            this.button1_down.TabIndex = 2;
            this.button1_down.Text = ">";
            this.button1_down.UseVisualStyleBackColor = true;
            this.button1_down.Click += new System.EventHandler(this.button1_down_Click);
            // 
            // label2_PageShow
            // 
            this.label2_PageShow.AutoSize = true;
            this.label2_PageShow.Location = new System.Drawing.Point(304, 496);
            this.label2_PageShow.Name = "label2_PageShow";
            this.label2_PageShow.Size = new System.Drawing.Size(59, 12);
            this.label2_PageShow.TabIndex = 4;
            this.label2_PageShow.Text = "共0条记录";
            // 
            // button1_top
            // 
            this.button1_top.Location = new System.Drawing.Point(491, 491);
            this.button1_top.Name = "button1_top";
            this.button1_top.Size = new System.Drawing.Size(32, 23);
            this.button1_top.TabIndex = 5;
            this.button1_top.Text = "<<";
            this.button1_top.UseVisualStyleBackColor = true;
            this.button1_top.Click += new System.EventHandler(this.button1_top_Click);
            // 
            // button2_bot
            // 
            this.button2_bot.Location = new System.Drawing.Point(603, 491);
            this.button2_bot.Name = "button2_bot";
            this.button2_bot.Size = new System.Drawing.Size(31, 23);
            this.button2_bot.TabIndex = 6;
            this.button2_bot.Text = ">>";
            this.button2_bot.UseVisualStyleBackColor = true;
            this.button2_bot.Click += new System.EventHandler(this.button2_bot_Click);
            // 
            // pTitleBackground
            // 
            this.pTitleBackground.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pTitleBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.pTitleBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pTitleBackground.Controls.Add(this.label1);
            this.pTitleBackground.Controls.Add(this.btnClose);
            this.pTitleBackground.Controls.Add(this.btnMinimize);
            this.pTitleBackground.Location = new System.Drawing.Point(1, 1);
            this.pTitleBackground.Name = "pTitleBackground";
            this.pTitleBackground.Size = new System.Drawing.Size(698, 25);
            this.pTitleBackground.TabIndex = 7;
            this.pTitleBackground.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 14);
            this.label1.TabIndex = 49;
            this.label1.Text = "聊天记录";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnClose.BackgroundImage = global::Browserform.Properties.Resources.btn_close;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnClose.Location = new System.Drawing.Point(675, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(24, 24);
            this.btnClose.TabIndex = 47;
            this.btnClose.TabStop = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnClose.MouseEnter += new System.EventHandler(this.btnClose_MouseEnter);
            this.btnClose.MouseLeave += new System.EventHandler(this.btnClose_MouseLeave);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnMinimize.BackgroundImage = global::Browserform.Properties.Resources.btn_minimize;
            this.btnMinimize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnMinimize.Location = new System.Drawing.Point(651, 0);
            this.btnMinimize.Margin = new System.Windows.Forms.Padding(0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(24, 24);
            this.btnMinimize.TabIndex = 48;
            this.btnMinimize.TabStop = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            this.btnMinimize.MouseEnter += new System.EventHandler(this.btnMinimize_MouseEnter);
            this.btnMinimize.MouseLeave += new System.EventHandler(this.btnMinimize_MouseLeave);
            // 
            // pBackgroundBorder
            // 
            this.pBackgroundBorder.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pBackgroundBorder.Controls.Add(this.pBackground);
            this.pBackgroundBorder.Location = new System.Drawing.Point(0, 0);
            this.pBackgroundBorder.Name = "pBackgroundBorder";
            this.pBackgroundBorder.Size = new System.Drawing.Size(700, 532);
            this.pBackgroundBorder.TabIndex = 8;
            // 
            // pBackground
            // 
            this.pBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.pBackground.Location = new System.Drawing.Point(1, 1);
            this.pBackground.Name = "pBackground";
            this.pBackground.Size = new System.Drawing.Size(698, 528);
            this.pBackground.TabIndex = 0;
            // 
            // ChatRecord
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(700, 530);
            this.Controls.Add(this.button2_bot);
            this.Controls.Add(this.button1_top);
            this.Controls.Add(this.label2_PageShow);
            this.Controls.Add(this.button1_down);
            this.Controls.Add(this.button1_Up);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.pTitleBackground);
            this.Controls.Add(this.pBackgroundBorder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChatRecord";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "聊天记录";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ChatRecord_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.pTitleBackground.ResumeLayout(false);
            this.pTitleBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).EndInit();
            this.pBackgroundBorder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RichTextBox rtb_chatRecord;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lb_friend;
        private System.Windows.Forms.Button button1_Up;
        private System.Windows.Forms.Button button1_down;
        private System.Windows.Forms.Label label2_PageShow;
        private System.Windows.Forms.Button button1_top;
        private System.Windows.Forms.Button button2_bot;
        private System.Windows.Forms.Panel pTitleBackground;
        private System.Windows.Forms.PictureBox btnClose;
        private System.Windows.Forms.PictureBox btnMinimize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pBackgroundBorder;
        private System.Windows.Forms.Panel pBackground;
    }
}