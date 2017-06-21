using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this Uri value, object data = null, string head = "", bool fast = false)
        {
            string result = "";
            ("请求地址：" + value.ToString()).WriteToLog(log4net.Core.Level.Debug);
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
                if (fast)
                {
                    webClient.Timeout = 1000;
                }
                if (!String.IsNullOrWhiteSpace(head))
                {
                    webClient.Headers["x-auth-token"] = head;//todo:用户Appkey 
                }
                webClient.Encoding = Encoding.UTF8;
                if (data == null || String.IsNullOrWhiteSpace(data.ToString()))
                {
                    webClient.Headers["Content-type"] = "application/json";
                    result = webClient.DownloadString(value);
                }
                else if (data != null && data is string)
                {
                    webClient.Headers["Content-type"] = "application/json";
                    result = webClient.UploadString(value, data.ToString());
                }
                else if (data != null && data is NameValueCollection)
                {
                    result = webClient.UploadValues(value, (data as NameValueCollection)).ConvertToUTF8String();
                }
                else if (data != null)
                {
                    result = webClient.UploadData(value, data.ConvertToBytes()).ConvertToUTF8String();
                }
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                if (data == null || String.IsNullOrWhiteSpace(data.ToString()))
                {
                    ("请求地址：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
                }
                else
                {
                    ("请求地址：" + value.ToString() + "出现异常！提交数据：" + data + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
                }
                result.Trim().WriteToLog(log4net.Core.Level.Debug);
                result = "";
                //throw ex;
            }
            //GC.Collect();
            ("服务器返回：" + result.Trim()).WriteToLog(log4net.Core.Level.Debug);
            return result.Trim();
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="head"></param>
        public static string UploadFile(this Uri value, string fileName, string head = "")
        {
            string result = "";
            ("请求上传文件：" + value.ToString()).WriteToLog(log4net.Core.Level.Debug);
            try
            {
                StrongWebClient webClient = new StrongWebClient();
                webClient.Headers["Content-type"] = "application/octet-stream";
                if (!String.IsNullOrWhiteSpace(head))
                {
                    webClient.Headers["x-auth-token"] = head;//todo:用户Appkey 
                }
                byte[] uploadResult = webClient.UploadFile(value, fileName);
                webClient.Dispose();
                result = Encoding.UTF8.GetString(uploadResult);
            }
            catch (Exception ex)
            {
                ("上传过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
            }
            //GC.Collect();
            ("服务器返回：" + result.Trim()).WriteToLog(log4net.Core.Level.Debug);
            return result;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="head"></param>
        public static string UploadFile(this string value, string fileName, string head = "")
        {
            Uri url = new Uri(value);
            return url.UploadFile(fileName, head);
        }


        /// <summary>
        /// 上传文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="head"></param>
        public static WebClient UploadFileAsync(this Uri value, string fileName, string head = "")
        {
            ("请求上传文件地址：" + value.ToString()).WriteToLog(log4net.Core.Level.Debug);
            StrongWebClient webClient = new StrongWebClient();
            try
            {
                webClient.Headers["Content-type"] = "application/octet-stream";
                if (!String.IsNullOrWhiteSpace(head))
                {
                    webClient.Headers["x-auth-token"] = head;//todo:用户Appkey 
                }
                webClient.UploadFileAsync(value, fileName);
                webClient.UploadFileCompleted += (sender, e) =>
                {
                    try
                    {
                        if (e.Error != null)
                        {
                            ("上传过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + e.Error.ToString()).WriteToLog(log4net.Core.Level.Error);
                        }
                        webClient.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                };
            }
            catch (Exception ex)
            {
                ("请求上传文件地址：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
            }
            return webClient;
        }

        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="head"></param>
        public static WebClient UploadFileAsync(this string value, string fileName, string head = "")
        {
            Uri url = new Uri(value);
            return url.UploadFileAsync(fileName, head);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="head"></param>
        public static void DownloadFile(this Uri value, string fileName, string head = "")
        {
            ("请求下载文件地址：" + value.ToString()).WriteToLog(log4net.Core.Level.Debug);
            try
            {
                StrongWebClient webClient = new StrongWebClient();
                webClient.Headers["Content-type"] = "application/octet-stream";
                if (!String.IsNullOrWhiteSpace(head))
                {
                    webClient.Headers["x-auth-token"] = head;//todo:用户Appkey 
                }
                webClient.DownloadFile(value, fileName);
                webClient.Dispose();
            }
            catch (Exception ex)
            {
                ("下载过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fileName"></param>
        /// <param name="head"></param>
        public static void DownloadFile(this string value, string fileName, string head = "")
        {
            Uri url = new Uri(value);
            url.DownloadFile(fileName, head);
        }

        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="head"></param>
        public static WebClient DownloadFileAsync(this Uri value, string fileName, string head = "")
        {
            ("请求下载文件地址：" + value.ToString()).WriteToLog(log4net.Core.Level.Debug);
            StrongWebClient webClient = new StrongWebClient();
            try
            {
                webClient.Headers["Content-type"] = "application/octet-stream";
                if (!String.IsNullOrWhiteSpace(head))
                {
                    webClient.Headers["x-auth-token"] = head;//todo:用户Appkey 
                }
                webClient.DownloadFileAsync(value, fileName);
                webClient.DownloadFileCompleted += (sender, e) =>
                {
                    try
                    {
                        if (e.Error != null)
                        {
                            ("下载过程中出现异常：" + value.ToString() + "出现异常！" + System.Environment.NewLine + e.Error.ToString()).WriteToLog(log4net.Core.Level.Error); 
                        }
                        webClient.Dispose();
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                };
            }
            catch (Exception ex)
            {
                ("请求下载文件地址：" + value.ToString() + "出现异常！" + System.Environment.NewLine + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
            }
            return webClient;
        }

        /// <summary>
        /// 下载文件
        /// </summary>  
        /// <param name="value"></param> 
        /// <param name="fileName"></param> 
        /// <param name="head"></param>
        public static WebClient DownloadFileAsync(this string value, string fileName, string head = "")
        {
            Uri url = new Uri(value);
            return url.DownloadFileAsync(fileName, head);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <param name="value">URL对象</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this Uri value, string head = "", bool fast = false)
        {
            return value.DownloadString("", head, fast);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL对象</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this Uri value, string head = "", bool fast = false) where T : class
        {
            return value.DownloadObject<T>("", head, fast);
        }

        /// <summary>
        /// 根据URL得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL对象</param>
        /// <param name="data">需要提交的数据</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this Uri value, object data = null, string head = "", bool fast = false) where T : class
        {
            string strObject = value.DownloadString(data, head, fast);
            T Tobject = strObject.JsParse<T>();
            return Tobject;
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this string value, string head = "", bool fast = false)
        {
            return value.DownloadString("", head, fast);
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数据</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this string value, object data = null, string head = "", bool fast = false)
        {
            Uri url = new Uri(value);
            string strData = url.DownloadString(data, head, fast);
            return strData;
        }

        /// <summary>
        /// 批量提交内容
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数组</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString(this string value, List<string> data, string head = "", bool fast = false)
        {
            Uri url = new Uri(value);
            string strData = url.DownloadString(String.Join("&", data.ToArray()), head, fast);
            return strData;
        }

        /// <summary>
        /// 批量提交内容
        /// </summary>
        /// <param name="value">URL字符串</param>
        /// <param name="key">需要提交的数组的Key</param>
        /// <param name="data">需要提交的数组</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static string DownloadString<T>(this string value, string key, List<T> data, string head = "", bool fast = false)
        {
            Uri url = new Uri(value);
            var lstData = data.ConvertAll<string>(o => "key=" + o.JsStringify());
            string strData = url.DownloadString(String.Join("&", lstData.ToArray()), head, fast);
            return strData;
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL字符串</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this string value, string head = "", bool fast = false) where T : class
        {
            return value.DownloadObject<T>("", head, fast);
        }

        /// <summary>
        /// 根据URL字符串得到数据对象
        /// </summary>
        /// <typeparam name="T">泛型实体类</typeparam>
        /// <param name="value">URL字符串</param>
        /// <param name="data">需要提交的数据</param>
        /// <param name="head">头部信息</param>
        /// <param name="fast">快速访问</param>
        /// <returns></returns>
        public static T DownloadObject<T>(this string value, object data = null, string head = "", bool fast = false) where T : class
        {
            string strObject = value.DownloadString(data, head, fast);
            T Tobject = strObject.JsParse<T>();
            return Tobject;
        }
    }
}
