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
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
namespace FTPTool
{
    public class SFTPHelper
    {
       
            private Session m_session;
            private Channel m_channel;
            private ChannelSftp m_sftp;

            //host:sftp地址   user：用户名   pwd：密码        
            public SFTPHelper(string host, string user, string pwd)
            {
                string[] arr = host.Split(':');
                string ip = arr[0];
                int port = 22;
                if (arr.Length > 1) port = Int32.Parse(arr[1]);

                JSch jsch = new JSch();
                m_session = jsch.getSession(user, ip, port);
                MyUserInfo ui = new MyUserInfo();
                ui.setPassword(pwd);
                m_session.setUserInfo(ui);

            }

            //SFTP连接状态        
            public bool Connected { get { return m_session.isConnected(); } }

            //连接SFTP        
            public bool Connect()
            {
                try
                {
                    if (!Connected)
                    {
                        m_session.connect();
                        m_channel = m_session.openChannel("sftp");
                        m_channel.connect();
                        m_sftp = (ChannelSftp)m_channel;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //断开SFTP        
            public void Disconnect()
            {
                if (Connected)
                {
                    m_channel.disconnect();
                    m_session.disconnect();
                }
            }

            //新建目录
            public bool Mkdir(string path)
            {
                try
                {
                    string root = "/";
                    string[] folders = path.Split('/');
                    foreach (string folder in folders) {
                        if (folder.Length > 0 && (!folder.Equals("/"))) {
                            Tamir.SharpSsh.java.String dir = new Tamir.SharpSsh.java.String(root + folder);
                            bool isExists = false;
                            SftpATTRS attrs;
                            try
                            {
                                attrs = m_sftp.stat(dir);
                                isExists = true;
                            }
                            catch (Exception e)
                            {
                                isExists = false;
                            }
                            if (isExists == false)
                            {
                                m_sftp.mkdir(dir);
                            }
                            root += folder + "/"; 
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //SFTP上传文件        
            public bool UploadFile(string localPath, string remotePath)
            {
                try
                {
                    if (remotePath.EndsWith("/") || localPath.EndsWith("/")) { return false; }
                    Connect();
                    if (this.Connected)
                    {
                        //m_sftp.cd("./../../app/");//进入文件存放目录
                        Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(localPath);
                        Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(remotePath);
                        string remotefolder = remotePath.Substring(0, remotePath.LastIndexOf("/"));

                        SftpATTRS attrs;
                        try
                        {
                            attrs = m_sftp.stat(remotefolder);
                        }
                        catch (Exception e)
                        {
                            Mkdir(remotefolder);
                        }
                        m_sftp.put(src, dst);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                    return false;
                }
                return false;
            }

            //SFTP下载文件        
            public bool DownloadFile(string remotePath, string localPath)
            {
                try
                {
                    if (remotePath.EndsWith("/") || localPath.EndsWith("/")) { return false; }
                    Tamir.SharpSsh.java.String src = new Tamir.SharpSsh.java.String(remotePath);
                    Tamir.SharpSsh.java.String dst = new Tamir.SharpSsh.java.String(localPath);
                    string localfolder = localPath.Substring(0, localPath.LastIndexOf("/"));
                    DirectoryInfo dir = new DirectoryInfo(localfolder);
                    if (!dir.Exists) { dir.Create(); }
                    m_sftp.get(src, dst);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            //删除SFTP文件
            public bool Delete(string remoteFile)
            {
                try
                {
                    m_sftp.rm(remoteFile);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //SFTP上传目录下所有文件，
            //ref ArrayList 为操作失败文件列表       
            public bool UploadFolder(string localPath, string remotePath, ref ArrayList failedlist)
            {
                ArrayList list = new ArrayList();
                GetAllFileFullLocalPath(localPath, "", ref list);
                foreach (string file in list)
                {
                    DirectoryInfo dir = new DirectoryInfo(localPath);
                    string relativename = file.Substring(dir.FullName.Length).Replace("\\", "/");
                    bool res = UploadFile(file, remotePath + relativename);
                    if (!res)
                    {
                        failedlist.Add(file);
                    }
                }
                return failedlist.Count == 0;
            }
            //SFTP下载目录下所有文件，
            //ref ArrayList 为操作失败文件列表       
            public bool DownloadFolder(string remotePath, string localPath, ref ArrayList failedlist)
            {
                ArrayList list = new ArrayList();
                GetAllFileFullRemotePath(remotePath, "", ref list);
                foreach (string file in list)
                {
                    string relativename = file.Substring(remotePath.Length).Replace("\\", "/");
                    bool res = DownloadFile(file, localPath + relativename);
                    if (!res)
                    {
                        failedlist.Add(file);
                    }
                }
                return failedlist.Count == 0;
            }

            //删除SFTP目录
            public bool DeleteFolder(string remoteFolder)
            {
                try
                {
                    m_sftp.rmdir(remoteFolder);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            //获取SFTP文件/目录列表,fileType为空时列出所有文件
            public ArrayList GetFolderContents(string remotePath, string fileType)
            {
                try
                {
                    if (remotePath.Length == 0) {
                        remotePath = "/";
                    }
                    Tamir.SharpSsh.java.util.Vector vvv = m_sftp.ls(remotePath);
                    ArrayList objList = new ArrayList();
                    foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry qqq in vvv)
                    {
                        string sss = qqq.getFilename();
                        if (sss.Equals(".") || sss.Equals("..") || qqq.getAttrs().isLink() ) { continue; }

                        bool isdir = qqq.getAttrs().isDir();
                        if (isdir) { sss += "/"; }
                        if (fileType.Length == 0 || (sss.Length > (fileType.Length + 1) && fileType == sss.Substring(sss.Length - fileType.Length)))
                        { objList.Add(sss); }
                        else { continue; }
                    }

                    return objList;
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            //获取SFTP所有文件列表，不包括目录, fileType为空时列出所有文件
            public bool GetAllFileFullRemotePath(string remotePath, string fileType, ref ArrayList list)
            {
                try
                {
                    if (remotePath.Length == 0)
                    {
                        remotePath = "/";
                    }
                    Tamir.SharpSsh.java.util.Vector vvv = m_sftp.ls(remotePath);

                    foreach (Tamir.SharpSsh.jsch.ChannelSftp.LsEntry qqq in vvv)
                    {
                        string sss = qqq.getFilename();
                        if (sss.Equals(".") || sss.Equals("..")) { continue; }

                        bool isdir = qqq.getAttrs().isDir();
                        if (isdir) {
                            GetAllFileFullRemotePath(remotePath + "/" + sss, fileType,ref list); 
                        }
                        else if (fileType.Length == 0 || (sss.Length > (fileType.Length + 1) && fileType == sss.Substring(sss.Length - fileType.Length)))
                        { list.Add(remotePath + "/" + sss); }
                        else { continue; }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            /// <summary>
            /// 查找指定文件夹下指定后缀名的文件
            /// </summary>
            /// <param name="directory">文件夹</param>
            /// <param name="pattern">后缀名</param>
            /// <returns>文件路径</returns>
            public void GetAllFileFullLocalPath(string path, string pattern, ref ArrayList fileList)
            {
                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory.Exists || pattern.Trim() != string.Empty)
                {
                    try
                    {
                        FileInfo[] files;
                        if (pattern.Trim().Length == 0)
                        {
                            files = directory.GetFiles();
                        }
                        else {
                            files = directory.GetFiles(pattern);
                        }
                        foreach (FileInfo info in files)
                        {
                            fileList.Add(info.FullName.ToString());
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    foreach (DirectoryInfo info in directory.GetDirectories())//获取文件夹下的子文件夹
                    {
                        GetAllFileFullLocalPath(info.FullName, pattern, ref fileList);//递归调用该函数，获取子文件夹下的文件
                    }
                }
            }


            //登录验证信息        
            public class MyUserInfo : UserInfo
            {
                String passwd;
                public String getPassword() { return passwd; }
                public void setPassword(String passwd) { this.passwd = passwd; }

                public String getPassphrase() { return null; }
                public bool promptPassphrase(String message) { return true; }

                public bool promptPassword(String message) { return true; }
                public bool promptYesNo(String message) { return true; }
                public void showMessage(String message) { }
            }
        }
    }
