﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Common
{
    /// <summary>
    /// TCP服务端
    /// @author zhangsx
    /// @date 2017/04/12 11:18:19
    /// </summary>
    public class TcpService
    {
        /// <summary>
        /// 隐藏构造
        /// </summary>
        private TcpService()
        {

        }

        /// <summary>
        /// TCP监听服务
        /// </summary>
        private static TcpListener tcpListener = null;

        /// <summary>
        /// 接收到消息
        /// </summary>
        public event Action<Message> ReceiveMessage;

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 开启TCP监听
        /// </summary>
        public void Acceptor()
        {
            if (tcpListener == null)
            {
                tcpListener = new TcpListener(IPAddress.Any, Port);
                tcpListener.Start(200);
                IAsyncResult result = tcpListener.BeginAcceptTcpClient(new AsyncCallback(Acceptor), tcpListener);
            }
        }

        /// <summary>
        /// 开始监听，启用监听时使用，用于设置端口号开启服务。
        /// </summary>
        /// <param name="port"></param>
        public static void Start(int port = 12333)
        {
            var tcpService = GetInstence();
            tcpService.Port = port;
            tcpService.Acceptor();
        }

        /// <summary>
        /// 
        /// </summary>
        private static TcpService tcpService = null;

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static TcpService GetInstence()
        {
            if (tcpService == null)
            {
                tcpService = new TcpService();
            }
            return tcpService;
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        private void Disposable()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }
            tcpListener = null;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static void Stop()
        {
            var tcpService = GetInstence();
            tcpService.Disposable();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        private void Acceptor(IAsyncResult o)
        {
            TcpListener server = o.AsyncState as TcpListener;
            try
            {
                TcpClient client = server.EndAcceptTcpClient(o);
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    NetworkStream ns = client.GetStream();
                    MemoryStream ms = new MemoryStream();
                    try
                    {
                        byte[] bData = new byte[1024];
                        int length = 0;
                        while ((length = ns.Read(bData, 0, bData.Length)) > 0)
                        {
                            ms.Write(bData, 0, length);
                        }
                        byte[] bValue = ms.ToArray();
                        Message message = bValue.ConvertToObject<Message>();
                        if (message != null)
                        {
                            message.IP = (client.Client.RemoteEndPoint as IPEndPoint).Address.ToString();
                            message.Port = (client.Client.RemoteEndPoint as IPEndPoint).Port;
                        }
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (ReceiveMessage != null)
                            {
                                ReceiveMessage(message);
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                    finally
                    {
                        ms.Close();
                        ms.Dispose();
                        ms = null;

                        ns.Close();
                        ns.Dispose();
                        ns = null;
                        Close(client);
                    }
                });
            }
            catch (Exception ex)
            { 
                ex.ToString().WriteToLog(log4net.Core.Level.Error);
            }
            finally
            { 
                IAsyncResult result = server.BeginAcceptTcpClient(new AsyncCallback(Acceptor), server);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="client"></param>
        private void Close(TcpClient client)
        {
            if (client.Connected)
            {
                client.Client.Shutdown(SocketShutdown.Both);
            }
            client.Client.Close();
            client.Close();
            client = null;
        }
    }
}
