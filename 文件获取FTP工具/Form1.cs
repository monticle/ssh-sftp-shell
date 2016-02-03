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
using Renci;
using Renci.SshNet;
using Renci.SshNet.NetConf;
using Renci.SshNet.Common;
using Renci.SshNet.Channels;
using Renci.SshNet.Compression;
using Renci.SshNet.Sftp;
using Renci.SshNet.Security;
using Renci.SshNet.Messages;


namespace 文件获取FTP工具
{
    public partial class Form1 : Form
    {
        string LHftpserverIP;//下载服务器IP
        string LHftpname;//下载服务器的用户名
        string LHftppassword;//下载服务器的密码
        string savepath;//本地保存位置
        string KKfilename;
        string JYfilename;
        string FtpUrl;
        private string hostip;//第一个linux服务器IP
        private string user;//第一个linux服务器用户名
        private string psw;
        private string re = null;
        private SshClient sshtest;
        private string cmd;
        //ssh后的linux命令
        private string cmd2;
        //ssh后的linux命令
        string folder;//服务器目标文件夹
        string UPserverIP;//上传服务器IP
        string UPusername;//上传服务器的用户名
        string UPpassword;//上传服务器的密码
        string KKUPpath;//文件上传目标文件夹
        string JYUpth;
        string FHupth;
        string errortext = "wearenogetthenewfileintheserver";
        delegate void SetTextCallback(string text);
        public Form1()
        {
            InitializeComponent();
        }

        private void close_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
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
        #region 获取本地文件夹中最新修改的文件的文件名
        private string[] getlastfilename(string foldername) 
        {
            SetText("正在寻找本地"+foldername+"文件夹中最新文件\r\n");
            string path = savepath+"/"+foldername;
            var query = (from f in Directory.GetFiles(path)
                         let fi = new FileInfo(f)
                         orderby fi.LastWriteTime descending
                         select fi.Name).Take(3);
            return query.ToArray();
        }
        #endregion

        #region 获取服务器中最新文件的文件名
        private string FileName(string firstname)
        //private void FileName(string firstname) 
        {
            
            string FileName1 = getlastfilename(firstname)[0];
            SetText("找到本地最新文件" + FileName1 + "\r\n");
            string localname = FileName1.Substring(0, 10);
            string getnameurl = "ftp://"+LHftpserverIP+"/"+folder;
            try
            {
                FtpWebRequest getname = (FtpWebRequest)FtpWebRequest.Create(getnameurl);
                getname.UseBinary = true;
                getname.Credentials = new NetworkCredential(LHftpname, LHftppassword);
                //getname.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                getname.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse test = (FtpWebResponse)getname.GetResponse();
                StreamReader stream = new StreamReader(test.GetResponseStream());
                SetText("连接服务器成功,尝试寻找更新的文件\r\n");
                string line = stream.ReadLine();
                while (line != null)
                {
                    string compare = line.Substring(8, 10);
                    string Fname = line.Substring(8, 2);
                    string year = line.Substring(10, 4);
                    string localyear = localname.Substring(2, 4);
                    if (Fname == firstname)
                    {
                        if (localyear == year) 
                        {
                            int result = string.Compare(compare, localname);
                            if (result > 0)
                            {
                                stream.Close();
                                test.Close();
                                return line;
                            }
                            else
                            {
                                line = stream.ReadLine();
                            }
                        }
                        else
                        {
                            line = stream.ReadLine();
                        }
                    }
                    else
                    {
                        line = stream.ReadLine();
                    }
                }
                return errortext;
            }
            catch (Exception ex)
            {
                SetText("出现错误" + ex.ToString() + "\r\n");
            }  
            return null;
        }
        #endregion
        private void start_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(Downloadoperation);
            th.Start();
        }
        #region ssh连接第一个服务器后用命令进行ftp操作
        private string givemefive()//获取服务器最新的FH文件文件名
        {
            sshtest = new SshClient(hostip, user, psw);
            sshtest.Connect();
            var cmdd = sshtest.RunCommand(cmd);
            re = cmdd.Execute();
            string[] filename = re.Split('\n');
            int i = filename.Length - 1;
            ArrayList T = new ArrayList();
            for (int a = 0; a < i; a++)
            {
                if (filename[a].Length != 0 && filename[a].Substring(0, 3) == "-rw")
                {
                    T.Add(filename[a].Substring(54, 20));
                }
            }
            string[] filelist = (string[])T.ToArray(typeof(string));
            string url = cmd2 + filelist[filelist.Length - 1] + "\nbye\nEOF\n";
            if (sshtest.IsConnected)
            {
                var cmde = sshtest.RunCommand(url);
                return filelist[filelist.Length - 1];
            }
            else
            {
                sshtest.Connect();
                var cmde = sshtest.RunCommand(url);
                return filelist[filelist.Length - 1];
            }
        }
        private void getFHfile(string fhname)//将FH取回第一个服务器 
        {
            sshtest = new SshClient(hostip, user, psw);
            sshtest.Connect();
            string url = cmd2 + fhname + "\nbye\nEOF\n";
            if (sshtest.IsConnected)
            {
                var cmde = sshtest.RunCommand(url);
            }
            else
            {
                sshtest.Connect();
                var cmde = sshtest.RunCommand(url);
            }
        }
        #endregion
        #region 文件下载
        private void downfile(string filename, string foldername1) 
        {
            FtpUrl = "ftp://" + LHftpserverIP + "/" + folder + "/" + filename/*+"/"*/;
            FtpWebRequest LHftp;
            LHftp = (FtpWebRequest)FtpWebRequest.Create(FtpUrl);
            LHftp.Credentials = new NetworkCredential(LHftpname, LHftppassword);
            LHftp.Method = WebRequestMethods.Ftp.DownloadFile;//设置连接模式
            LHftp.UseBinary = true;
            LHftp.UsePassive = true;
            try
            {
                using (FtpWebResponse LHftp1 = (FtpWebResponse)LHftp.GetResponse())
                {
                    string filepath = Path.Combine(savepath, foldername1, filename);
                    FileStream LH = new FileStream(filepath, FileMode.Create, FileAccess.Write);
                    int buf = 1024;
                    byte[] a = new byte[buf];
                    int i = 0;
                    Stream LHstream = LHftp1.GetResponseStream();
                    SetText("连接服务器成功，"+filename + "下载中\r\n");
                    while ((i = LHstream.Read(a, 0, buf)) > 0)
                    {
                        LH.Write(a, 0, buf);
                    }
                    LHstream.Close();
                    LH.Close();
                    SetText(filename+"下载成功\r\n");
                }
            }
            catch (Exception ex)
            {
                SetText("出现错误:" + ex.ToString()+"\r\n");
            }   
        }
        #endregion
        #region FH,KK,JY文件的下载操作
        private void Downloadoperation() 
        {
            try
            {
                KKfilename = FileName("KK").Substring(8, 20);//截取需要下载的文件的文件名
                if (KKfilename != errortext.Substring(8, 20))
                {
                    SetText("找到更新的文件" + KKfilename + "尝试下载中\r\n");
                    downfile(KKfilename, "KK");
                }
                else
                { SetText("尚未发现比本地文件新的文件！\r\n"); }
                JYfilename = FileName("JY").Substring(8, 20);//截取需要下载的文件的文件名
                if (JYfilename != errortext.Substring(8, 20))
                {
                    SetText("找到更新的文件" + JYfilename + "尝试下载中\r\n");
                    downfile(JYfilename, "JY");
                }
                else
                { SetText("尚未发现比本地文件新的文件！\r\n"); }
                string fhname = givemefive();
                if (getlastfilename("FH")[0] == fhname) { SetText("本地文件" + fhname + "就是最新的文件！\r\n"); }
                else
                {
                    getFHfile(fhname);
                    FHdownload(fhname);
                }

            }
            catch (Exception ex)
            {
                SetText("出现错误:" + ex.ToString() + "\r\n");
            }
            
        }
        #endregion
        private void update_Click(object sender, EventArgs e)
        {
            Thread th2 = new Thread(uploaddoperation);
            th2.Start();
        }
        private void uploaddoperation() 
        {
            string KKname = getlastfilename("KK")[0];
            SftpUpload(KKname, "KK",KKUPpath);
            string JYname = getlastfilename("JY")[0];
            SftpUpload(JYname, "JY",JYUpth);
            string FHname = givemefive();
            SftpUpload(FHname, "FH", FHupth);
        }
        private void SftpUpload(string filename, string foldername3,string uppath) 
        {
            string localpath=savepath+"\\"+foldername3+"\\"+filename;
            SFTPHelper SFTP = new SFTPHelper(UPserverIP, UPusername, UPpassword);
            try 
            {
                SetText("成功连接服务器，尝试上传" + filename + "\r\n");
                if (SFTP.Put(localpath, uppath+filename))
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
        private void FHdownload(string FHname)//FH下载操作
        {
            SFTPHelper FHdl = new SFTPHelper(hostip, user, psw);
            try 
            {
                FHdl.Connect();
                if (FHdl.Connected) 
                {
                    SetText("成功连接服务器，尝试下载" + FHname + "\r\n");
                    if (FHdl.Get("./balfile_temp/" + FHname, savepath+"/"+"FH/" + FHname))
                    { SetText("下载成功\r\n"); }
                    else { SetText("下载失败\r\n"); }
                    FHdl.Disconnect();
                }
            }
            catch (Exception ex)
            {
                SetText("出现错误:" + ex.ToString() + "\r\n");
            }
        }
    }
}
