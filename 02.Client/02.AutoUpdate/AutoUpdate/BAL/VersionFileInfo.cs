using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdate
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class VersionFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public static string FileInfoName
        {
            get
            {
                return "version.json";
            }
        }
        /// <summary>
        /// 文件服务地址
        /// </summary>
        public static string FileService { get; set; }

        /// <summary>
        /// 程序名
        /// </summary>
        public static string ApplicationName { get; set; }

        /// <summary>
        /// 本地文件信息存储路径-保存到程序安装目录中
        /// </summary>
        public static readonly string LocalFileInfoPath = WorkPath.ExecPath;

        /// <summary>
        /// 服务器文件信息存储路径-保存到零时目录
        /// </summary>
        public static string ServiceFileInfoPath
        {
            get
            {
                return System.IO.Path.GetFullPath(WorkPath.ApplicationDataPath + @"\" + ApplicationName + @"\update");
            }
        }
        /// <summary>
        /// 解压缩路径
        /// </summary>
        public static string TempZipPath
        {
            get
            {
                return ServiceFileInfoPath + @"\GzTemp";
            }
        }
        /// <summary>
        /// 解压缩路径
        /// </summary>
        public static string TempBakPath
        {
            get
            {
                return LocalFileInfoPath + @"\Bak";
            }
        }

        /// <summary>
        /// 压缩解压缩
        /// </summary> 
        [System.Web.Script.Serialization.ScriptIgnore]
        private GZipCompress ZipCompress { get; set; }

        /// <summary>
        /// 程序标识
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 版本信息用于版本对比的信息
        /// </summary>
        [System.Web.Script.Serialization.ScriptIgnore]
        public Version VInfo
        {
            get
            {
                return System.Version.Parse(Version);
            }
        }

        /// <summary>
        /// 服务器中存储的更新文件名
        /// 注：比如为zip压缩包或exe安装包，参见IsZip属性
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// true表示zip压缩包，否则就是exe安装包
        /// </summary>
        public bool IsZip { get; set; }

        /// <summary>
        /// 消息信息
        /// </summary> 
        [System.Web.Script.Serialization.ScriptIgnore]
        public Action<string> Message { get; set; }

        /// <summary>
        /// 获取本地存储的文件信息内容
        /// </summary>
        /// <returns></returns>
        public static VersionFileInfo GetLocalFileInfo()
        {
            string fileName = LocalFileInfoPath + @"\" + FileInfoName;
            var file = new VersionFileInfo();
            if (!System.IO.File.Exists(fileName))
            {
                if (String.IsNullOrWhiteSpace(ApplicationName))
                {
                    ApplicationName = WorkPath.AssemblyName;
                }
                file.AppID = ApplicationName;
                file.Version = "1.0";
                file.FileName = "";
            }
            else
            {
                //读取本地文件序列化
                string fileInfo = System.IO.File.ReadAllText(fileName);
                System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
                file = jss.Deserialize<VersionFileInfo>(fileInfo);
                ApplicationName = file.AppID;
            }
            return file;
        }

        /// <summary>
        /// 获取服务器中存储的文件信息
        /// </summary>
        /// <returns></returns>
        public static VersionFileInfo GetServiceFileInfo()
        {
            if (!System.IO.Directory.Exists(ServiceFileInfoPath))
            {
                System.IO.Directory.CreateDirectory(ServiceFileInfoPath);
            }
            //从服务器中下载文件序列化
            string address = FileService + @"\" + FileInfoName;
            string fileName = ServiceFileInfoPath + @"\" + FileInfoName;
            System.Net.WebClient webClient = new System.Net.WebClient();
            try
            {
                webClient.DownloadFile(address, fileName);
            }
            catch
            {
                return null;
            }
            string fileInfo = System.IO.File.ReadAllText(fileName);
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jss.Deserialize<VersionFileInfo>(fileInfo);
        }

        /// <summary>
        /// 保存信息文件
        /// </summary>
        public void SaveAs()
        {
            string fileName = LocalFileInfoPath + @"\" + FileInfoName;
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            var fileInfo = jss.Serialize(this);
            System.IO.File.WriteAllText(fileName, fileInfo);
        }

        /// <summary>
        /// 获取本地缓存服务器版本
        /// </summary>
        public static VersionFileInfo LoadServiceFileInfo()
        {
            string fileName = ServiceFileInfoPath + @"\" + FileInfoName;
            string fileInfo = System.IO.File.ReadAllText(fileName);
            System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            return jss.Deserialize<VersionFileInfo>(fileInfo);
        }

        /// <summary>
        /// 检查服务器版本信息
        /// </summary>
        /// <returns>若返回不是null则表示需要更新</returns>
        public static VersionFileInfo Check()
        {
            var localFileInfo = GetLocalFileInfo();
            var serviceFileInfo = GetServiceFileInfo();
            if (localFileInfo != null
                    && serviceFileInfo != null
                    && localFileInfo.AppID.ToUpper() == serviceFileInfo.AppID.ToUpper()
                    && localFileInfo.VInfo < serviceFileInfo.VInfo)
            {
                return serviceFileInfo;
            }
            else
            {
                string fileName = ServiceFileInfoPath + @"\" + FileInfoName;
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
            }
            return null;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        public void DownloadFile()
        {
            var fileInfo = LoadServiceFileInfo();
            string address = FileService + @"\" + fileInfo.FileName;
            string fileName = ServiceFileInfoPath + @"\" + fileInfo.FileName;
            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFile(address, fileName);
        }

        /// <summary>
        /// 下载进度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 0)
            {
                Message?.Invoke("正在下载，已完成" + e.ProgressPercentage + "%。");
            }
            else
            {
                Message?.Invoke("正在下载...");
            }
        }

        /// <summary>
        /// 替换程序
        /// </summary>
        public void Replace()
        {
            string fileName = ServiceFileInfoPath + @"\" + this.FileName;
            //是压缩包
            if (this.IsZip)
            {
                if (!System.IO.Directory.Exists(TempZipPath))
                {
                    System.IO.Directory.CreateDirectory(TempZipPath);
                }
                if (!System.IO.Directory.Exists(TempBakPath))
                {
                    System.IO.Directory.CreateDirectory(TempBakPath);
                }
                //Zip包解压缩，并且替换目标文件 
                Message?.Invoke("正在解压...");
                if (ZipCompress == null)
                {
                    ZipCompress = new GZipCompress();
                }
                ZipCompress.DirPath = TempZipPath;
                ZipCompress.ZipFileName = fileName;
                ZipCompress.Message = Message;
                ZipCompress.DeCompress();
                System.Threading.Thread.Sleep(100);

                //复制文件到安装目录
                var tempFiles = System.IO.Directory.GetFiles(TempZipPath, "*", System.IO.SearchOption.AllDirectories);
                foreach (string f in tempFiles)
                {
                    Message?.Invoke("正在替换文件" + f + "...");
                    string workPath = System.IO.Path.GetFullPath(LocalFileInfoPath + @"\" + f.Substring(TempZipPath.Length, f.Length - TempZipPath.Length - System.IO.Path.GetFileName(f).Length));
                    if (!System.IO.Directory.Exists(workPath))
                    {
                        System.IO.Directory.CreateDirectory(workPath);
                    }

                    string strOldFilename = System.IO.Path.GetFullPath(workPath + @"\" + System.IO.Path.GetFileName(f));
                    string strOldBakFilename = System.IO.Path.GetFullPath(workPath.Replace(LocalFileInfoPath, TempBakPath) + @"\" + System.IO.Path.GetFileName(f));
                    if (System.IO.File.Exists(strOldFilename))
                    {
                        if (System.IO.File.Exists(strOldBakFilename + ".bak"))
                        {
                            System.IO.File.Delete(strOldBakFilename + ".bak");
                        }
                        try
                        {
                            System.IO.File.Move(strOldFilename, strOldBakFilename + ".bak");
                        }
                        catch (Exception ex)
                        {
                            ex.ToString();
                        }
                    }
                    try
                    {
                        System.IO.File.Copy(f, System.IO.Path.GetFullPath(workPath + @"\" + System.IO.Path.GetFileName(f)), true);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
                System.Threading.Thread.Sleep(100);

                Message?.Invoke("正在清理...");
                System.IO.File.Delete(fileName);
                System.IO.File.Delete(ServiceFileInfoPath + @"\" + FileInfoName);
                System.IO.Directory.Delete(TempZipPath, true);
                System.IO.Directory.Delete(ServiceFileInfoPath, true);

                //var fileBaks = System.IO.Directory.GetFiles(LocalFileInfoPath, "*.bak", System.IO.SearchOption.AllDirectories);
                var fileBaks = System.IO.Directory.GetFiles(TempBakPath, "*.bak", System.IO.SearchOption.AllDirectories);
                foreach (var fileBak in fileBaks)
                {
                    try
                    {
                        System.IO.File.Delete(fileBak);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }

                System.Threading.Thread.Sleep(100);

                //var dev = @"cmd.exe";
                //System.Diagnostics.Process.Start(dev);
            }
            else //是安装包
            {
                System.Diagnostics.Process.Start(fileName);
            }
        }

    }
}
