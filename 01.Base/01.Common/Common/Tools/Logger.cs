using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Logger
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 日志对象
        /// </summary>
        public class LogObject
        {
            /// <summary>
            /// ID
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 日志类
            /// </summary>
            public log4net.Repository.Hierarchy.Logger Log { get; set; }
        }

        /// <summary>
        /// 日志对象
        /// </summary>
        private static List<LogObject> logs = null;


        private static log4net.ILog log = null;

        /// <summary>
        /// 写入日志 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="level"></param>
        public static void WriteToLog(this string value, log4net.Core.Level level = null)
        {
            value.WriteToLog("", level);
        }

        /// <summary>
        /// 写入日志 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <param name="level"></param>
        public static void WriteToLog(this string value, string key, log4net.Core.Level level = null)
        {

            if (logs == null)
            {
                logs = new List<LogObject>();
            }

            LogObject logObject = null;

            lock (logs)
            {
                logObject = logs.FirstOrDefault(o => o.ID == key);
            }


            if (logObject == null)
            {
                string path = WorkPath.ExecPath + @"\Config\Log4net" + key + ".config";
                if (File.Exists(path))
                {
                    if (log == null)
                    {
                        log4net.GlobalContext.Properties["LogUrl"] = WorkPath.ApplicationWorkPath + @"\Logs\ALL\";
                        log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(path));
                        log = log4net.LogManager.GetLogger("lognet");
                    }
                }
                else
                {
                    log4net.Appender.RollingFileAppender rollingFileAppender = new log4net.Appender.RollingFileAppender();
                    rollingFileAppender.Name = "rollingFileAppender" + key;
                    rollingFileAppender.AppendToFile = true;
                    rollingFileAppender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
                    rollingFileAppender.File = WorkPath.ApplicationWorkPath + @"\Logs\" + key + @"\";
                    rollingFileAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
                    rollingFileAppender.DatePattern = "yyyyMMddHH'.log'";
                    rollingFileAppender.StaticLogFileName = false;
                    rollingFileAppender.MaxSizeRollBackups = 72;
                    rollingFileAppender.Encoding = Encoding.UTF8;

                    log4net.Layout.PatternLayout patternLayout = new log4net.Layout.PatternLayout();
                    patternLayout.ConversionPattern = "[ %date ] %-5level：%message%n";
                    patternLayout.ActivateOptions();
                    rollingFileAppender.Layout = patternLayout;
                    rollingFileAppender.ActivateOptions();

                    log4net.Repository.Hierarchy.Hierarchy repository = LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;

                    var logger = repository.LoggerFactory.CreateLogger(repository, "lognet" + key);
                    logger.Level = repository.Root.Level;// log4net.Core.Level.All;
                    logger.AddAppender(rollingFileAppender);
                    logger.Hierarchy = repository;

                    //log4net.Config.BasicConfigurator.Configure(repository, rollingFileAppender);

                    //log = log4net.LogManager.GetLogger("lognet" + key);

                    logObject = new LogObject()
                    {
                        ID = key,
                        Log = logger
                    };

                    lock (logs)
                    {
                        logs.Add(logObject);
                    }

                    if (level == null)
                    {
                        level = log4net.Core.Level.Info;
                    }
                    logger.Log(level, value, null);
                }
            }
            else
            {
                if (level == null)
                {
                    level = log4net.Core.Level.Info;
                }
                logObject.Log.Log(level, value, null);
            }

            if (log == null)
            {
                return;
            }

            if (level == log4net.Core.Level.Debug)
            {
                log.Debug(value);
            }
            else if (level == log4net.Core.Level.Info)
            {
                log.Info(value);
            }
            else if (level == log4net.Core.Level.Error)
            {
                log.Error(value);
            }
            else if (level == log4net.Core.Level.Fatal)
            {
                log.Fatal(value);
            }
            else if (level == log4net.Core.Level.Warn)
            {
                log.Warn(value);
            }
            else
            {
                log.Info(value);
            }
        }
    }
}
