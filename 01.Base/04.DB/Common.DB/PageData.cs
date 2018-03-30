/*----------------------------------------------------------------

// Copyright (C) 2016 南京思创信息技术有限公司 版权所有。
//
// 文件名：.cs
// 文件功能描述：
//
/// @author zhangsx
/// @date 2016/12/05 11:18:19
//
//----------------------------------------------------------------*/
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 分页实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T> : EntityBase
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        private int _PageIndex = 0;

        /// <summary>
        /// 每页显示条数
        /// </summary>
        private int _PageSize = 0;

        /// <summary>
        /// 总页数
        /// </summary>
        private int _TotlePage = 0;

        /// <summary>
        /// 总条数
        /// </summary>
        private int _TotleCount = 0;

        /// <summary>
        /// 当前页数据
        /// </summary>
        private List<T> _Data = null;

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex
        {
            get
            {
                return _PageIndex;
            }

            set
            {
                _PageIndex = value;
            }
        }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return _PageSize;
            }

            set
            {
                _PageSize = value;
            }
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotlePage
        {
            get
            {
                return _TotlePage;
            }

            set
            {
                _TotlePage = value;
            }
        }

        /// <summary>
        /// 总条数
        /// </summary>
        public int TotleCount
        {
            get
            {
                return _TotleCount;
            }

            set
            {
                _TotleCount = value;
            }
        }

        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> Data
        {
            get
            {
                return _Data;
            }

            set
            {
                _Data = value;
            }
        }
    }
}