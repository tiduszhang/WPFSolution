using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Common
{
    /// <summary>
    /// 自定义Web客户端
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    [System.ComponentModel.DesignerCategory("Code")]
    [System.ComponentModel.Designer("Code")]
    class StrongWebClient : System.Net.WebClient
    {
        /// <summary>
        /// 设置超时时间(毫秒)
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            if (Timeout <= 0)
            {
                Timeout = 5000;
            }
            try
            {
                WebRequest request = base.GetWebRequest(address);
                if (request is HttpWebRequest)
                {
                    HttpWebRequest httpRequest = request as HttpWebRequest;
                    httpRequest.Timeout = Timeout;
                    httpRequest.ReadWriteTimeout = Timeout * 5;
                    return httpRequest;
                }
                else if (request is FtpWebRequest)
                {
                    FtpWebRequest ftpRequest = request as FtpWebRequest;
                    ftpRequest.Timeout = Timeout;
                    ftpRequest.ReadWriteTimeout = Timeout * 5;
                    return ftpRequest;
                }
                else
                {
                    request.Timeout = Timeout;
                }
                return request;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return base.GetWebRequest(address);
        } 
    }
}
