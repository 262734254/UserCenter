using JNKJ.Core.Data;
using JNKJ.Core.Infrastructure;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Dto.UserCenter;
using JNKJ.Services.SMS;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JNKJ.Services.UserCenter.Realize
{
    public class UserLoginService : IUserLogin
    {
        #region Fields
        private readonly IRepository<UserLogin> _userLogin;
        private readonly IRepository<UserInfo> _userinfo;
        #endregion

        #region Ctor
        public UserLoginService(IRepository<UserLogin> userLogin,IRepository<UserInfo> userinfo)
        {
            _userLogin = userLogin;
            _userinfo = userinfo;
        }
        #endregion
        public JsonResponse UserLogin(LoginRequest loginRequest)
        {
            var model = new TokenInfo();
            try
            {
                if (loginRequest.UserName == null)
                {
                    return new JsonResponse() { DataModel = "", Message = "手机号码不能为空", State = OperatingState.CheckDataFail };
                }
                else
                {
                    var userinfo = _userinfo.Table.Where(t => t.UserName == loginRequest.UserName).FirstOrDefault();
                    if (userinfo != null && (userinfo.AccountState == (int)UserInfoState.Normal || userinfo.AccountState == null))
                    {
                        var userlogin = _userLogin.Table.Where(t => t.PhoneNo == loginRequest.UserName && t.DeletedState == 1).FirstOrDefault();
                        if (DateTime.Now > userlogin.RegisterTime.AddMinutes(2))
                        {
                            var ISmsService = EngineContext.Current.Resolve<ISmsService>();
                            var send = ISmsService.SendSms(loginRequest.UserName, "", SmsTemplateState.Login);
                            if (userlogin != null)
                            {
                                var userLogin = new UserLogin()
                                {
                                    Id = userlogin.Id,
                                    ValidationCode = send.Code,
                                    RegisterTime = userlogin.RegisterTime,
                                    ValidationCodeExpiredEndTime = userlogin.ValidationCodeExpiredEndTime,
                                    ValidationCodeExpiredMinutes = userlogin.ValidationCodeExpiredMinutes,
                                    DeletedState = userlogin.DeletedState,
                                    DeletedTime = userlogin.DeletedTime
                                };
                                _userLogin.Update(userLogin);
                            }
                            else
                            {
                                var userLogin = new UserLogin()
                                {
                                    PhoneNo = loginRequest.UserName,
                                    RegisterTime = DateTime.Now,
                                    ValidationCodeExpiredMinutes = 2,
                                    ValidationCodeExpiredEndTime = DateTime.Now.AddMinutes(2),
                                    ValidationCode = send.Code
                                };
                                _userLogin.Insert(userLogin);
                            }
                        }
                        else
                        {
                            return new JsonResponse() { DataModel = "", Message = "验证码过于频繁，两分钟后再发送", State = OperatingState.CheckDataFail };
                        }


                        var IsExists = _userLogin.Table.Any(t => t.ValidationCode == loginRequest.VerificationCode);
                        if (IsExists)
                        {
                            //调用token接口，存放cookie
                            var userid = _userinfo.Table.Where(t => t.UserName == loginRequest.UserName).Select(t => t.Id).FirstOrDefault();
                            var tokenService = EngineContext.Current.Resolve<ITokenService>();
                            var tokenInfo = tokenService.GenerateToken(new GenerateTokenRequest
                            {
                                UserId = userid,
                                IsApp = loginRequest.IsApp
                            });
                            var tokenInfoViewModel = new TokenInfoViewModel()
                            {
                                UserId = userid,
                                Message = tokenInfo.Message,
                                Token = tokenInfo.Token,
                                Success = tokenInfo.Success
                            };

                            return new JsonResponse() { DataModel = tokenInfoViewModel, State = OperatingState.Success, Message = "成功" };
                        }
                        else
                        {
                            return new JsonResponse() { DataModel = "", Message = "验证码错误", State = OperatingState.CheckDataFail };
                        }
                    }
                    else
                    {
                        return new JsonResponse() { DataModel = "", Message = "手机号异常或不存在", State = OperatingState.CheckDataFail };
                    }
                }
            }
            catch (Exception e)
            {
                return new JsonResponse() { DataModel = "", Message = "错误" + e.Message.ToString(), State = OperatingState.CheckDataFail };
            }
        }
    }
}
