using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;
//using Renci;
//using Renci.SshNet;
//using Renci.SshNet.NetConf;
//using Renci.SshNet.Common;
//using Renci.SshNet.Channels;
//using Renci.SshNet.Compression;
//using Renci.SshNet.Sftp;
//using Renci.SshNet.Security;
//using Renci.SshNet.Messages;


namespace FTPTool
{
    public partial class Form1 : Form
    {
        string ftpserverURL = "192.168.11.104";//服务器URL
        string ftpname = "tester";//服务器的用户名
        string ftppassword = "testpass";//服务器的密码
        string ftproot = "/";//服务器目录
        string localroot = "local/";//本地保存位置
        string FtpUrl;
      
        delegate void SetTextCallback(string text);
        public Form1()
        {
            InitializeComponent();
        }

        private void SetText(string text)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.richTextBox1.Text += text;
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(DownloadOperation);
            th.Start();
        }

        #region 文件的下载操作
        private void DownloadOperation() 
        {
            SftpDownload(textfile.Text, textLfolder.Text, textRfolder.Text);
        }
        #endregion
        private void update_Click(object sender, EventArgs e)
        {
            Thread th2 = new Thread(UploadOperation);
            th2.Start();
        }
        private void UploadOperation() 
        {
            SftpUpload(textfile.Text, textLfolder.Text, textRfolder.Text);
        }
        private void SftpUpload(string filename, string localfolder,string ftppath) 
        {
            SFTPHelper SFTP = new SFTPHelper(ftpserverURL, ftpname, ftppassword);
            try 
            {
                SetText("成功连接服务器，尝试上传" + filename + "\r\n");
                bool res = true;
                ArrayList result = new ArrayList() ;
                if (filename.EndsWith("/"))
                {
                    res = SFTP.UploadFolder(Environment.CurrentDirectory + "/" +localroot + "/" + localfolder + "/" + filename, ftproot + "/" + ftppath + "/" + filename, ref result);
                }
                else {
                    res = SFTP.UploadFile(Environment.CurrentDirectory + "/" + localroot + "/" + localfolder + "/" + filename, ftproot + "/" + ftppath + "/" + filename);
                }
                if (res)
                {
                    SetText(filename + "上传成功\r\n");
                    SFTP.Disconnect();
                }
                else
                {
                    SetText(filename + "上传失败\r\n");
                }
            }
            catch (Exception ex)
            {
                SetText("出现错误:" + ex.ToString() + "\r\n");
            }
        }
        private void SftpDownload(string filename, string localpath, string ftppath)//下载操作
        {
            SFTPHelper SFTP = new SFTPHelper(ftpserverURL, ftpname, ftppassword);
            try 
            {
                SFTP.Connect();
                if (SFTP.Connected) 
                {
                    SetText("成功连接服务器，尝试下载" + filename + "\r\n");
                    bool res = true;
                    ArrayList result = new ArrayList();
                    if (filename.EndsWith("/"))
                    {
                        res = SFTP.DownloadFolder(ftproot + "/" + ftppath + "/" + filename, Environment.CurrentDirectory + "/" + localroot + "/" + localpath + "/" + filename, ref result);
                    }
                    else
                    {
                        res = SFTP.DownloadFile(ftproot + "/" + ftppath + "/" + filename, Environment.CurrentDirectory + "/" + localroot + "/" + localpath + "/" + filename);
                    }
                    if (res)
                    { SetText("下载成功\r\n"); }
                    else { SetText("下载失败\r\n"); }
                    SFTP.Disconnect();
                }
            }
            catch (Exception ex)
            {
                SetText("出现错误:" + ex.ToString() + "\r\n");
            }
        }

        private void list_Click(object sender, EventArgs e)
        {
            Thread th3 = new Thread(ListOperation);
            th3.Start();
        }

        private void ListOperation()
        {
            SFTPHelper SFTP = new SFTPHelper(ftpserverURL, ftpname, ftppassword);
            try
            {
                SFTP.Connect();
                if (SFTP.Connected)
                {
                    SetText("成功连接服务器，尝试列出所有文件\r\n");
                    ArrayList filelist = SFTP.GetFolderContents(textRfolder.Text, "");
                    if (filelist == null)
                    {
                        SetText("Exception. 请确保目录填写正确，如根目录需填入\"/\"\r\n");
                    }
                    else 
                    {
                        foreach (string file in filelist)
                        {
                            SetText(">>" + file + "\r\n");
                        }
                    }
                    SFTP.Disconnect();
                }
            }
            catch (Exception ex)
            {
                SetText("出现错误:" + ex.ToString() + "\r\n");
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

    }
}
