using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace Common.DB
{
    /// <summary>
    /// NoSQLHelper
    /// </summary>
    public static class NoSQLHelper
    {
        /// <summary>
        /// 数据引擎列表
        /// </summary>
        private static Dictionary<string, LiteDatabase> Databases = new Dictionary<string, LiteDatabase>();

        /// <summary>
        /// 数据库存储位置
        /// </summary>
        public static string DBPath = "";

        /// <summary>
        /// 获取数据库存储路径
        /// </summary>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static string GetDB<T>(string MarkId = "") where T : EntityBase
        {
            string path = GetDBPath();

            return path + typeof(T).Name + MarkId + ".db";//目前一个实体一个库分开存储
        }

        /// <summary>
        /// 获取数据库存储路径
        /// </summary>
        /// <returns></returns>
        public static string GetDBPath()
        {
            string path = DBPath;
            if (String.IsNullOrWhiteSpace(path))
            {
                path = WorkPath.ApplicationWorkPath + @"\DB\";
            }
            DBPath = path;
            if (!System.IO.Directory.Exists(DBPath))
            {
                System.IO.Directory.CreateDirectory(DBPath);
            }
            return DBPath;//目前一个实体一个库分开存储
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        private static LiteDatabase GetLiteDatabase<T>(string MarkId = "") where T : EntityBase
        {
            string strKey = typeof(T).Name;

            LiteDatabase liteDatabase = null;

            lock (Databases)
            {
                var database = Databases.FirstOrDefault(o => o.Key == strKey);

                if (database.Value == null)
                {
                    var dbFile = GetDB<T>();
                    try
                    {
                        liteDatabase = new LiteDatabase(dbFile);
                    }
                    catch (Exception ex)
                    {
                        ("数据引擎初始化失败，异常信息：" + ex.ToString()).WriteToLog(log4net.Core.Level.Error);
                    }
                    Databases.Add(strKey, liteDatabase);
                }
                else
                {
                    liteDatabase = database.Value;
                }
            }
            return liteDatabase;
        }


        /// <summary>
        /// 加载所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static List<T> LoadALL<T>(string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            return table.FindAll().Select(o => o.JsParse<T>()).ToList();
        }

        /// <summary>
        /// 根据条件加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static List<T> LoadByCondition<T>(Func<T, bool> func, string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            return table.Find(o => func(o.JsParse<T>())).Select(o => o.JsParse<T>()).ToList();
        }


        /// <summary>
        /// 根据ID加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T LoadById<T>(string Id, string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            var objexist = table.FindOne(o => o.JsParse<T>().ID == Id);
            if (!String.IsNullOrWhiteSpace(objexist))
            {
                return objexist.JsParse<T>();
            }
            return null;
        }


        /// <summary>
        /// 根据条件分页加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static PageData<T> LoadByPage<T>(int PageIndex, int PageSize, Func<T, bool> func = null, string MarkId = "") where T : EntityBase
        {
            PageData<T> pagedata = new PageData<T>();
            pagedata.PageIndex = PageIndex;
            pagedata.PageSize = PageSize;

            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            if (func != null)
            {
                pagedata.TotalCount = (int)table.Find(o => func(o.JsParse<T>())).Count();
            }
            else
            {
                pagedata.TotalCount = (int)table.Count();
            }
            if (pagedata.TotalCount > 0)
            {
                if (pagedata.PageSize <= 0)
                {
                    pagedata.PageSize = 1;
                }
                pagedata.TotalPageCount = (int)Math.Ceiling((double)(pagedata.TotalCount / pagedata.PageSize));
                if (pagedata.PageIndex > pagedata.TotalPageCount)
                {
                    pagedata.PageIndex = pagedata.TotalPageCount;
                }
                else if (pagedata.PageIndex < 0)
                {
                    pagedata.PageIndex = 1;
                }
                if (func != null)
                {
                    pagedata.QueryData = table.Find(o => func(o.JsParse<T>()), (pagedata.PageIndex - 1) * pagedata.PageSize, pagedata.PageSize).Select(o => o.JsParse<T>()).ToList();
                }
                else
                {
                    pagedata.QueryData = table.Find(o => true, (pagedata.PageIndex - 1) * pagedata.PageSize, pagedata.PageSize).Select(o => o.JsParse<T>()).ToList();
                }
            }
            else
            {
                pagedata.TotalCount = 0;
                pagedata.TotalPageCount = 0;
                pagedata.PageIndex = 0;
                pagedata.QueryData = new List<T>();
            }
            return pagedata;
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MarkId"></param>
        public static void Delete<T>(this T value, string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            table.Delete(value.Sequence);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="MarkId"></param>
        public static void Delete<T>(this List<T> values, string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            foreach (T value in values)
            {
                table.Delete(value.Sequence);
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="MarkId"></param>
        public static void Save<T>(this T value, string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            int iMaxKey = 0;
            if (value.Sequence == 0)
            {
                if (table.Count() > 0)
                {
                    iMaxKey = table.Max(o => o.JsParse<T>().Sequence);
                }
                //新增
                iMaxKey++;
                value.Sequence = iMaxKey;
            }
            table.Upsert(value.Sequence, value.JsStringify());
        }


        /// <summary>
        /// 获取数据表中第一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T First<T>(Func<T, bool> func = null, string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            var iMinKey = table.Min(o => o.JsParse<T>().Sequence);
            if (iMinKey > 0)
            {
                return table.FindById(iMinKey).JsParse<T>();
            } 
            return default(T);
        }
         
        /// <summary>
        /// 获取数据表中最后一个数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="MarkId"></param>
        /// <returns></returns>
        public static T Last<T>(Func<T, bool> func = null, string MarkId = "") where T : EntityBase
        {
            var dataBase = GetLiteDatabase<T>(MarkId);
            var table = dataBase.GetCollection<string>(typeof(T).Name + MarkId);
            var iMaxKey = table.Max(o => o.JsParse<T>().Sequence);
            if (iMaxKey > 0)
            {
                return table.FindById(iMaxKey).JsParse<T>();
            }
            return default(T);
        }

    }
}
