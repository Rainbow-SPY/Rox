using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PageHeader = AntdUI.PageHeader;

namespace Rox.Runtimes
{
    partial class Reporter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Reporter));
            this.pictureBox1 = new PictureBox();
            this.label1 = new Label();
            this.button1 = new Button();
            this.richTextBox1 = new RichTextBox();
            this.label2 = new Label();
            this._Crush_Path = new LinkLabel();
            this.label3 = new Label();
            this.pageHeader1 = new PageHeader();
            ((ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = Color.Transparent;
            this.pictureBox1.Image = ((Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new Point(23, 60);
            this.pictureBox1.Margin = new Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(96, 96);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new Font("微软雅黑", 15F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new Point(128, 56);
            this.label1.Margin = new Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(809, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "应用程序在执行时发生了未经处理的异常, 按\"确定\"退出程序";
            // 
            // button1
            // 
            this.button1.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.button1.Font = new Font("微软雅黑", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new Point(865, 600);
            this.button1.Margin = new Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new Size(146, 57);
            this.button1.TabIndex = 2;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = SystemColors.Control;
            this.richTextBox1.BorderStyle = BorderStyle.None;
            this.richTextBox1.Location = new Point(23, 195);
            this.richTextBox1.Margin = new Padding(4, 4, 4, 4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new Size(968, 382);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "text";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new Font("微软雅黑", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new Point(133, 154);
            this.label2.Margin = new Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new Size(177, 27);
            this.label2.TabIndex = 4;
            this.label2.Text = "崩溃文件已保存到:";
            // 
            // _Crush_Path
            // 
            this._Crush_Path.AutoEllipsis = true;
            this._Crush_Path.Font = new Font("微软雅黑", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this._Crush_Path.Location = new Point(315, 154);
            this._Crush_Path.Margin = new Padding(4, 0, 4, 0);
            this._Crush_Path.Name = "_Crush_Path";
            this._Crush_Path.Size = new Size(675, 28);
            this._Crush_Path.TabIndex = 4;
            this._Crush_Path.TabStop = true;
            this._Crush_Path.Text = "崩溃文件已保存到:";
            this._Crush_Path.Click += new EventHandler(this._Crush_Path_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new Font("微软雅黑", 14.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = Color.Red;
            this.label3.Location = new Point(128, 104);
            this.label3.Margin = new Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new Size(874, 38);
            this.label3.TabIndex = 4;
            this.label3.Text = "请将这个崩溃文件提交给技术处理人员, 而不是发送这个窗口的截图";
            // 
            // pageHeader1
            // 
            this.pageHeader1.DividerShow = true;
            this.pageHeader1.Dock = DockStyle.Top;
            this.pageHeader1.Font = new Font("微软雅黑", 9F);
            this.pageHeader1.Icon = ((Image)(resources.GetObject("pageHeader1.Icon")));
            this.pageHeader1.Location = new Point(0, 0);
            this.pageHeader1.Margin = new Padding(4);
            this.pageHeader1.Name = "pageHeader1";
            this.pageHeader1.ShowButton = true;
            this.pageHeader1.ShowIcon = true;
            this.pageHeader1.Size = new Size(1136, 50);
            this.pageHeader1.TabIndex = 32;
            this.pageHeader1.Text = "应用程序崩溃报告";
            // 
            // Reporter
            // 
            this.AutoScaleDimensions = new SizeF(9F, 18F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1024, 670);
            this.Controls.Add(this.pageHeader1);
            this.Controls.Add(this._Crush_Path);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new Padding(4, 4, 4, 4);
            this.Name = "Reporter";
            this.Text = "应用程序发生了未经处理的异常 - Rox Reporter";
            ((ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pictureBox1;
        private Label label1;
        private Button button1;
        private RichTextBox richTextBox1;
        private Label label2;
        private Label label3;
        private LinkLabel _Crush_Path;
        private PageHeader pageHeader1;
    }
}