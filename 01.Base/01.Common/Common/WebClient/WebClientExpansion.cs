using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Common
{
    /// <summary>
    /// Web客户端
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public static class WebClientExpansion
    {
        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <param name="value">URL对象</param>
        /// <param name="data">需要提交的数据</param>
        /// <returns></returns>
        public static string DownloadString(this Uri value, string data)
        {
            string result = "";
            //value.ToString().WriteToLog(log4net.Core.Level.Debug);
            try
            {
                //WebRequest webRequest = WebRequest.Create(value);
                //webRequest.Proxy = null;
                //webRequest.Timeout = 1500;
                //if (!String.IsNullOrEmpty(data) && !String.IsNullOrEmpty(data.Trim()))
                //{
                //    webRequest.Method = "POST";
                //    byte[] bData = data.ConvertToBytes();
                //    webRequest.ContentType = "application/x-www-form-urlencoded";
                //    webRequest.ContentLength = bData.Length;
                //    using (Stream streamRequest = webRequest.GetRequestStream())//获得请求数据流
                //    {
                //        streamRequest.Write(bData, 0, bData.Length);
                //    }
                //}
                //using (WebResponse webResponse = webRequest.GetResponse())
                //{
                //    using (Stream streamResponse = webResponse.GetResponseStream())//获得回应的数据流
                //    {
                //        using (StreamReader readResponse = new StreamReader(streamResponse, System.Text.Encoding.UTF8))
                //        {
                //            result = readResponse.ReadToEnd();
                //        }
                //    }
                //}

                StrongWebClient webClient = new StrongWebClient();
                //webClient.Timeout = 1000;
                webClient.Headers["Content-type"] = "application/x-www-form-urlencoded";
                webClient.Encoding = Encoding.UTF8;
                if (String.IsNullOrWhiteSpace(data))
                {
                    result = webClient.DownloadString(value);
                }
                else
                {
                    result = webClient.UploadString(value, data);
                    //result = webClient.UploadData(value, data.ConvertToBytes()).ConvertToString();
                }
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                if (String.IsNullOrWhiteSpace(data))
                {
                    ("请求地址：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
                }
                else
                {
                    ("请求地址：" + value.ToString() + "出现异常！提交数据：" + data + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
                }
                //throw ex;
            }
            //GC.Collect();
            //result.Trim().WriteToLog(log4net.Core.Level.Debug);
            return result.Trim();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        public static string UploadFile(this Uri value, string fileName)
        {
            string result = "";
            //value.ToString().WriteToLog(log4net.Core.Level.Debug);
            try
            {
                StrongWebClient webClient = new StrongWebClient();
                webClient.Headers["Content-type"] = "application/octet-stream";
                byte[] uploadResult = webClient.UploadFile(value, fileName);
                webClient.Dispose();
                result = Encoding.UTF8.GetString(uploadResult);
            }
            catch (Exception ex)
            {
                (value.ToString() + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Debug);
                //throw ex;
            }
            GC.Collect();
            return result;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        public static string UploadFile(this string value, string fileName)
        {
            Uri url = new Uri(value);
            return url.UploadFile(fileName);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        public static void DownloadFile(this Uri value, string fileName)
        {
            //value.ToString().WriteToLog(log4net.Core.Level.Debug);
            try
            {
                StrongWebClient webClient = new StrongWebClient();
                webClient.Headers["Content-type"] = "application/octet-stream";
                webClient.DownloadFile(value, fileName);
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                (value.ToString() + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Debug);
                //throw ex;
            }
            GC.Collect();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        public static void DownloadFile(this string value, string fileName)
        {
            Uri url = new Uri(value);
            url.DownloadFile(fileName);
        }
        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param>
        /// <param name="progressChanged"></param>
        /// <param name="completed"></param>
        public static void DownloadFileAsync(this Uri value, string fileName, Action<System.Net.DownloadProgressChangedEventArgs> progressChanged, Action completed)
        {
            //value.ToString().WriteToLog(log4net.Core.Level.Debug);
            try
            {
                StrongWebClient webClient = new StrongWebClient();
                webClient.DownloadProgressChanged += (sender, e) =>
                {
                    if (progressChanged != null)
                    {
                        progressChanged(e);
                    }
                };
                webClient.DownloadFileCompleted += (sender, e) =>
                {
                    if (completed != null)
                    {
                        completed();
                    }
                    webClient.Dispose();
                };
                webClient.DownloadFileAsync(value, fileName);
            }
            catch (Exception ex)
            {
                (value.ToString() + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Debug);
                //throw ex;
            }
            GC.Collect();
        }

        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param>
        /// <param name="progressChanged"></param>
        /// <param name="completed"></param>
        public static void DownloadFileAsync(this string value, string fileName, Action<System.Net.DownloadProgressChangedEventArgs> progressChanged, Action completed)
        {
            Uri url = new Uri(value);
            url.DownloadFileAsync(fileName, progressChanged, completed);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <param name="value">URL对象</param>
        /// <returns></returns>
        public static string DownloadString(this Uri value)
        {
            return value.DownloadString("");
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL对象</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this Uri value) where T : class
        {
            return value.DownloadObject<T>("");
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL对象</param>
        /// <param name="data">需要提交的数据</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this Uri value, string data) where T : class
        {
            string strObject = value.DownloadString(data);
            T Tobject = strObject.JsParse<T>();
            return Tobject;
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <returns></returns>
        public static string DownloadString(this string value)
        {
            return value.DownloadString("");
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数据</param>
        /// <returns></returns>
        public static string DownloadString(this string value, string data)
        {
            Uri url = new Uri(value);
            string strData = url.DownloadString(data);
            return strData;
        }

        /// <summary>
        /// 批量提交内容
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数组</param>
        /// <returns></returns>
        public static string DownloadString(this string value, List<string> data)
        {
            Uri url = new Uri(value);
            string strData = url.DownloadString(String.Join("&", data.ToArray()));
            return strData;
        }

        /// <summary>
        /// 批量提交内容
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="key">需要提交的数组的Key</param>
        /// <param name="data">需要提交的数组</param>
        /// <returns></returns>
        public static string DownloadString<T>(this string value, string key, List<T> data)
        {
            Uri url = new Uri(value);
            var lstData = data.ConvertAll<string>(o => "key=" + o.JsStringify());
            string strData = url.DownloadString(String.Join("&", lstData.ToArray()));
            return strData;
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL字符串</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this string value) where T : class
        {
            return value.DownloadObject<T>("");
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数据</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this string value, string data) where T : class
        {
            string strObject = value.DownloadString(data);
            T Tobject = strObject.JsParse<T>();
            return Tobject;
        }
    }
}
