using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace JNKJ.Dto.Results
{
    public interface IResponseBase<TEnum> where TEnum : struct
    {
        /// <summary>
        /// 操作状态
        /// </summary>
        TEnum State { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        string Message { get; set; }
    }
}
