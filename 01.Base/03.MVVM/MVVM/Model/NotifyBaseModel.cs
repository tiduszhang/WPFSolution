using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;
using Common;

namespace MVVM.Model
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class NotifyBaseModel : NotifyPropertyBase
    {
        ///// <summary>
        ///// ID
        ///// </summary>
        //public virtual string ID
        //{
        //    get
        //    {
        //        return this.GetValue(o => o.ID);
        //    }
        //    set
        //    {
        //        this.SetValue(o => o.ID, value);
        //    }
        //}

        ///// <summary>
        ///// 存储路径
        ///// </summary>
        //[ScriptIgnore]
        //public virtual string Path
        //{
        //    get
        //    {
        //        return this.GetValue(o => o.ID);
        //    }
        //    set
        //    {
        //        this.SetValue(o => o.ID, value);
        //    }
        //}

        ///// <summary>
        ///// 文件名
        ///// </summary>
        //[ScriptIgnore]
        //public virtual string FileName
        //{
        //    get
        //    {
        //        return this.GetValue(o => o.ID);
        //    }
        //    set
        //    {
        //        this.SetValue(o => o.ID, value);
        //    }
        //}

        /// <summary>
        /// 构造函数
        /// </summary>
        public NotifyBaseModel()
        {
            this.ErrorData = new ValidationDataErrorInfo();
            this.ErrorData.NotifyProperty = this;
            this.PropertyNameData = new PropertyNameDataInfo();
            this.PropertyNameData.NotifyProperty = this;
        }

        /// <summary>
        /// 错误信息，通过IDataErrorInfo实现
        /// </summary>
        [ScriptIgnore]
        public ValidationDataErrorInfo ErrorData
        {
            get
            {
                return this.GetValue(o => o.ErrorData);
            }
            set
            {
                this.SetValue(o => o.ErrorData, value);
            }
        }

        /// <summary>
        /// 属性名称，可以通过DisplayAttribute标注属性名称
        /// </summary>
        [ScriptIgnore]
        public PropertyNameDataInfo PropertyNameData
        {
            get
            {
                return this.GetValue(o => o.PropertyNameData);
            }
            set
            {
                this.SetValue(o => o.PropertyNameData, value);
            }
        }

        /// <summary>
        /// 消息内容，所有错误消息内容。
        /// </summary>
        [ScriptIgnore]
        public virtual string Message
        {
            get
            {
                return this.GetValue(o => o.Message);
            }
            set
            {
                this.SetValue(o => o.Message, value);
            }
        }

        /// <summary>
        /// 是否验证通过
        /// </summary>
        [ScriptIgnore]
        public virtual bool IsValid
        {
            get
            {
                return this.GetValue(o => o.IsValid);
            }
            set
            {
                this.SetValue(o => o.IsValid, value);
            }
        }


        /// <summary>
        /// 验证属性
        /// </summary>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public virtual bool Valid()
        {
            this.Message = "";
            Type tp = this.GetType();
            PropertyInfo[] pis = tp.GetProperties();
            IsValid = true;
            foreach (PropertyInfo pi in pis)
            {
                try
                {
                    if (pi.Name.ToUpper() != "ITEM")
                    {
                        string strError = this.ErrorData.Valid(pi.Name);
                        if (!String.IsNullOrWhiteSpace(strError))
                        {
                            IsValid = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            if (!IsValid)
            {
                this.Message = this.ErrorData.Error;
            }
            return IsValid;
        }

        /// <summary>
        /// 验证属性
        /// </summary>
        /// <param name="Valided"> 先执行属性特性验证。再执行相关验证方法。 </param>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public virtual bool Valid(Func<object, string> Valided)
        {
            return Valid<NotifyBaseModel>(Valided);
        }

        /// <summary>
        /// 验证属性
        /// </summary>
        /// <typeparam name="T"> 当前对象泛型 </typeparam>
        /// <param name="Valided"> 先执行属性特性验证。再执行相关验证方法。 </param>
        /// <returns> 验证是否通过，若通过则返回 true，否则为false。 </returns>
        public bool Valid<T>(Func<T, string> Valided) where T : class
        {
            if (!Valid())
            {
                return false;
            }
            string message = Valided.Invoke(this as T);
            if (!String.IsNullOrWhiteSpace(message))
            {
                this.Message = message;
                return false;
            }
            return true;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (ErrorData != null)
            {
                ErrorData.Dispose();
            }
            if (PropertyNameData != null)
            {
                PropertyNameData.Dispose();
            }
            base.Dispose();
        }


        ///// <summary>
        ///// 保存
        ///// </summary>
        //public void Save()
        //{
        //    if (String.IsNullOrWhiteSpace(this.ID))
        //    {
        //        this.ID = Guid.NewGuid().ToString("N");
        //    }

        //    if (String.IsNullOrWhiteSpace(Path))
        //    {
        //        Path = WorkPath.ApplicationWorkPath +@"\" + this.GetType().Name + @"\";
        //    }

        //    if (String.IsNullOrWhiteSpace(FileName))
        //    {
        //        FileName = "{0}.json";
        //        FileName = String.Format(FileName, this.ID);
        //    }

        //    this.SaveJsonFile(Path, FileName);
        //} 
    }
}