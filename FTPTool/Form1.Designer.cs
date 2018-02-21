namespace FTPTool
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.start = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.update = new System.Windows.Forms.Button();
            this.list = new System.Windows.Forms.Button();
            this.textfile = new System.Windows.Forms.TextBox();
            this.textRfolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.textLfolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(444, 718);
            this.start.Margin = new System.Windows.Forms.Padding(6);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(137, 46);
            this.start.TabIndex = 0;
            this.start.Text = "开始下载";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(24, 24);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(6);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(704, 572);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(593, 718);
            this.update.Margin = new System.Windows.Forms.Padding(6);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(135, 46);
            this.update.TabIndex = 3;
            this.update.Text = "开始上传";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.update_Click);
            // 
            // list
            // 
            this.list.Location = new System.Drawing.Point(444, 612);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(284, 46);
            this.list.TabIndex = 4;
            this.list.Text = "列出目录下文件";
            this.list.UseVisualStyleBackColor = true;
            this.list.Click += new System.EventHandler(this.list_Click);
            // 
            // textfile
            // 
            this.textfile.Location = new System.Drawing.Point(189, 726);
            this.textfile.Name = "textfile";
            this.textfile.Size = new System.Drawing.Size(236, 35);
            this.textfile.TabIndex = 5;
            // 
            // textRfolder
            // 
            this.textRfolder.Location = new System.Drawing.Point(189, 620);
            this.textRfolder.Name = "textRfolder";
            this.textRfolder.Size = new System.Drawing.Size(236, 35);
            this.textRfolder.TabIndex = 5;
            this.textRfolder.Text = "/";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 623);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "远程目录：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 729);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "文件/文件夹：";
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(593, 788);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(135, 46);
            this.close.TabIndex = 4;
            this.close.Text = "关闭";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // textLfolder
            // 
            this.textLfolder.Location = new System.Drawing.Point(189, 674);
            this.textLfolder.Name = "textLfolder";
            this.textLfolder.Size = new System.Drawing.Size(236, 35);
            this.textLfolder.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 677);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 24);
            this.label3.TabIndex = 6;
            this.label3.Text = "本地目录：";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 872);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textLfolder);
            this.Controls.Add(this.textRfolder);
            this.Controls.Add(this.textfile);
            this.Controls.Add(this.close);
            this.Controls.Add(this.list);
            this.Controls.Add(this.update);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.start);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "文件自动获取工具";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button start;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button update;
        private System.Windows.Forms.Button list;
        private System.Windows.Forms.TextBox textfile;
        private System.Windows.Forms.TextBox textRfolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.TextBox textLfolder;
        private System.Windows.Forms.Label label3;
    }
}

