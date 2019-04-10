using System;
namespace JNKJ.Dto.Enums
{
    public enum OperatingState : int
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 10,
        /// <summary>
        /// 鉴权失败
        /// </summary>
        AuthenticationFail = 20,
        /// <summary>
        /// 数据校验失败
        /// </summary>
        CheckDataFail = 30,
        /// <summary>
        /// 操作失败
        /// </summary>
        Failure = 90
    }
}

