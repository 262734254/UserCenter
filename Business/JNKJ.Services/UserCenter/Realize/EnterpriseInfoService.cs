using Gcyg.Domain.Dto.Customers;
using Gcyg.Domain.Dto.Customers.Enums;
using Gcyg.Domain.Enums;
using JNKJ.Core;
using JNKJ.Core.Data;
using JNKJ.Core.Infrastructure;
using JNKJ.Domain;
using JNKJ.Domain.UserCenter;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.General;
using JNKJ.Services.UserCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JNKJ.Services.UserCenter.Realize
{
    public class EnterpriseInfoService : IEnterpriseInfoService
    {
        private readonly IRepository<EnterpriseInfo> _enterpriseInfoRepository;
        private readonly IRepository<UserInfo> _enterpriseUserInfo;
        private readonly IRepository<UserLogin> _userPreRegisterRecord;
        private readonly IRepository<Relationship> _relationship;

        public EnterpriseInfoService(
           IRepository<EnterpriseInfo> enterpriseInfoRepository,
            IRepository<UserLogin> userPreRegisterRecord,
           IRepository<UserInfo> enterpriseUserInfo,
           IRepository<Relationship> relationship)
        {
            _userPreRegisterRecord = userPreRegisterRecord;
            _enterpriseInfoRepository = enterpriseInfoRepository;
            _enterpriseUserInfo = enterpriseUserInfo;
            _relationship = relationship;
        }
        public JsonResponse AddEnterpriseInfo(EnterpriseInfoRequest enterprise)
        {
            try
            {
                if (string.IsNullOrEmpty(enterprise?.Name) || string.IsNullOrEmpty(enterprise.BusinessLicenseNum))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的条件");
                }
                //根据企业名称、营业执照注册号 综合判断是否存在该项目资料
                if (_enterpriseInfoRepository.Table.FirstOrDefault(s => s.Name == enterprise.Name) != null)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "已经存在该企业信息");
                }
                ////根据法定代表电话判断是否存在该用户
                //if (_enterpriseUserInfo.Table.FirstOrDefault(s => s.Phone == enterprise.RepresentativeTel && s.DeletedState != (int)DeletedStates.Deleted) != null)
                //{
                //    return new JsonResponse(OperatingState.CheckDataFail, "该法定代表电话已被注册");
                //}
                #region 新的EnterpriseInfo
                var enterId = Guid.NewGuid();
                var enterpriseInfo = new EnterpriseInfo()
                {
                    Id = enterId,
                    DeletedState = null,
                    DeletedTime = null,
                    CreatedTime = DateTime.Now,
                    State = (int)EnterpriseState.New,
                    Name = enterprise.Name,
                    LocationProvinceCode = enterprise.LocationProvinceCode,
                    LocationCityCode = enterprise.LocationCityCode,
                    LocationDistrictCode = enterprise.LocationDistrictCode,
                    Address = enterprise.Address,
                    Tel = enterprise.Tel,
                    TypeName = enterprise.TypeName,
                    Safety = enterprise.Safety,
                    SafetyToAudit = null,
                    RepresentativePerson = enterprise.RepresentativePerson,
                    RepresentativeTel = enterprise.RepresentativeTel,
                    CreditCode = enterprise.CreditCode,
                    CreditCodeToAudit = null,
                    BusinessLicenseNum = enterprise.BusinessLicenseNum,
                    BusinessLicenseNumToAudit = null,
                    Email = enterprise.Email,
                    RegisteredCapital = enterprise.RegisteredCapital,
                    RegisteredCapitalToAudit = 0,
                    OpenAnAccount = enterprise.OpenAnAccount,
                    BankAccount = enterprise.BankAccount,
                    AptitudeCertificate = enterprise.AptitudeCertificate,
                    AptitudeCertificateToAudit = null,
                    Nature = enterprise.Nature,
                    RegisterUserID = enterprise.RegisterUserID,
                    Remark = enterprise.Remark,
                    RepresentativeIDCardType = enterprise.RepresentativeIDCardType == null?0: enterprise.RepresentativeIDCardType
                };
                #endregion


                var relationship = _relationship.Table.FirstOrDefault(t => t.UserId == enterprise.RegisterUserID && t.EnterpriseID == enterId);


                if (relationship != null)
                {
                    relationship.Id = relationship.Id;
                    relationship.UserId = enterprise.RegisterUserID;
                    relationship.EnterpriseID = enterId;
                    relationship.RoleId = Guid.Empty;
                    relationship.State = 0;
                    relationship.IsEnterprise = true;
                    _relationship.Update(relationship);
                }
                else
                {
                    relationship = new Relationship()
                    {
                        Id = Guid.NewGuid(),
                        UserId = enterprise.RegisterUserID,
                        DeptInfoId = Guid.Empty,
                        EnterpriseID = enterId,
                        RoleId = Guid.Empty,
                        State = 0,
                        IsEnterprise = true,
                        CreateTime = DateTime.Now

                    };
                    _relationship.PreInsert(relationship);
                }

                _enterpriseInfoRepository.PreInsert(enterpriseInfo);
                _enterpriseInfoRepository.SaveChanges();

                return new JsonResponse(OperatingState.Success, "添加数据成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "添加数据失败", e.Message);
            }
        }

        public JsonResponse DeleteEnterpriseInfo(Guid? enterpriseInfoId)
        {
            var enterpriseInfo = _enterpriseInfoRepository.GetById(enterpriseInfoId);

            if (enterpriseInfo == null || enterpriseInfo.DeletedState == (int)DeletedStates.Deleted)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未发现该企业或已被删除");
            }

            try
            {
                enterpriseInfo.DeletedState = (int)DeletedStates.Deleted;
                enterpriseInfo.DeletedTime = DateTime.Now;
                _enterpriseInfoRepository.SaveChanges();
                return new JsonResponse(OperatingState.Success, "删除成功");

            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "删除时出现异常！");
            }
        }

        /// <summary>
        /// 获取系统内所有企业列表
        /// </summary>
        /// <returns></returns>
        public IPagedList<EnterpriseInfo> GetAllEnterpriseInfo(int pageIndex = 1, int pageSize = 100)
        {
            if (pageIndex <= ConstKeys.DEFAULT_PAGEINDEX) { pageIndex = ConstKeys.DEFAULT_PAGEINDEX; }

            if (pageSize >= ConstKeys.DEFAULT_MAX_PAGESIZE || pageSize <= ConstKeys.ZERO_INT) { pageSize = ConstKeys.DEFAULT_PAGESIZE; }

            var query = _enterpriseInfoRepository.Table;


            query = _enterpriseInfoRepository.Table.OrderByDescending(s => s.CreatedTime);

            return new PagedList<EnterpriseInfo>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取系统内所有企业列表
        /// </summary>
        /// <returns></returns>
        public List<EnterpriseInfo> GetAllEnterpriseInfo()
        {
            var list = _enterpriseInfoRepository.Table.Where(s => s.DeletedState != (int)DeletedStates.Deleted).OrderByDescending(s => s.CreatedTime).ToList();

            return list;
        }


        public IPagedList<EnterpriseInfo> GetEnterpriseInfoByRns(bool isAdmin, Guid? subContractorId, DateTime? establishDate, string companyName, string organizationCode, string businessStatus, int pageIndex, int pageSize)
        {
            if (pageIndex <= ConstKeys.DEFAULT_PAGEINDEX) { pageIndex = ConstKeys.DEFAULT_PAGEINDEX; }
            if (pageSize >= ConstKeys.DEFAULT_MAX_PAGESIZE || pageSize <= ConstKeys.ZERO_INT) { pageSize = ConstKeys.DEFAULT_PAGESIZE; }

            var query = _enterpriseInfoRepository.Table;

            if (!isAdmin)
            {
                query = query.Where(s => s.Id == subContractorId);
            }

            if (establishDate.HasValue)
            {
                query = query.Where(c => establishDate.Value == c.EstablishDate);
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                query = query.Where(c => c.CompanyName.Contains(companyName));
            }
            if (!string.IsNullOrWhiteSpace(organizationCode))
            {
                query = query.Where(c => c.OrganizationCode.Contains(organizationCode));
            }
            if (!string.IsNullOrWhiteSpace(businessStatus))
            {
                query = query.Where(c => c.BusinessStatus.Contains(businessStatus));
            }

            query = query.OrderByDescending(c => c.EstablishDate);


            var list = new PagedList<EnterpriseInfo>(query, pageIndex, pageSize);
            return list;
        }



        /// <summary>
        /// 根据条件获取企业信息
        /// PC
        /// </summary>
        /// <returns></returns>
        public IPagedList<EnterpriseInfo> GetEnterpriseInfo(string keyValue, int pageIndex, int pageSize)
        {
            if (pageIndex <= ConstKeys.DEFAULT_PAGEINDEX) { pageIndex = ConstKeys.DEFAULT_PAGEINDEX; }

            if (pageSize >= ConstKeys.DEFAULT_MAX_PAGESIZE || pageSize <= ConstKeys.ZERO_INT) { pageSize = ConstKeys.DEFAULT_PAGESIZE; }

            var query = _enterpriseInfoRepository.Table;

            //根据不同参数拼接筛选条件
            //Expression<Func<EnterpriseInfo, bool>> wheres = c => c.DeletedState != (int)DeletedStates.Deleted;

            query = query.Where(c => c.DeletedState != (int)DeletedStates.Deleted);

            if (!string.IsNullOrEmpty(keyValue))
            {
                query = query.Where(c => c.Name.Contains(keyValue));
            }

            query = query.OrderByDescending(c => c.CreatedTime).AsQueryable();

            return new PagedList<EnterpriseInfo>(query, pageIndex - 1, pageSize);
        }

        /// <summary>
        /// 根据多条件获取企业信息
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        public IPagedList<EnterpriseInfo> GetEnterpriseInfo(EnterpriseInfoGetRequest enterprise)
        {
            if (enterprise.pageIndex <= ConstKeys.DEFAULT_PAGEINDEX) { enterprise.pageIndex = ConstKeys.DEFAULT_PAGEINDEX; }

            if (enterprise.pageSize >= ConstKeys.DEFAULT_MAX_PAGESIZE || enterprise.pageSize <= ConstKeys.ZERO_INT) { enterprise.pageSize = ConstKeys.DEFAULT_PAGESIZE; }

            var query = _enterpriseInfoRepository.Table;

            query = query.Where(c => c.DeletedState != (int)DeletedStates.Deleted);

            if (!string.IsNullOrEmpty(enterprise.Name))
            {
                query = query.Where(c => c.Name.Contains(enterprise.Name));
            }
            if (enterprise.State != null)
            {
                query = query.Where(c => c.State == enterprise.State);
                //query = query.Where(c => c.State == 50);
            }
            if (enterprise.StartDate != null && enterprise.EndDate != null)
            {
                var start = enterprise.StartDate;
                var end = Convert.ToDateTime(enterprise.EndDate).AddDays(1).AddSeconds(-1);
                query = query.Where(c => c.CreatedTime > start && c.CreatedTime < end);
            }
            if (enterprise.Id != null)
            {
                query = query.Where(c => c.Id == enterprise.Id);
            }

            //query = query.OrderByDescending(c => c.CreatedTime).AsQueryable();
            var QueryList = query.ToList();

            if (enterprise.State == 2 && enterprise.State != null)
            {
                var QueryState_50 = _enterpriseInfoRepository.Table.Where(t => t.State == 50).ToList();
                QueryList.AddRange(QueryState_50);
            }

            query = QueryList.AsQueryable();
            query = query.OrderByDescending(c => c.CreatedTime).AsQueryable();

            return new PagedList<EnterpriseInfo>(query, enterprise.pageIndex-1, enterprise.pageSize);
        }

        /// <summary>
        /// 根据企业ID获取企业信息
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        public JsonResponse GetEnterpriseInfo(Guid EnterpriseID)
        {
            return new JsonResponse
            {
                DataModel = _enterpriseInfoRepository.GetById(EnterpriseID),
                State = OperatingState.Success,
                Message = "获取信息成功"
            };
        }

        /// <summary>
        /// 修改企业信息
        /// </summary>
        /// <param name="enterprise"></param>
        /// <returns></returns>
        public JsonResponse ModifyEnterpriseInfo(EnterpriseInfoRequest enterprise)
        {
            if (enterprise?.Id == null || string.IsNullOrEmpty(enterprise.Name))
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的数据");
            }
            try
            {
                var enterpriseInfo = _enterpriseInfoRepository.GetById(enterprise.Id);
                if (enterpriseInfo == null || enterpriseInfo.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "未发现该企业或已被删除");
                }

                Modity(enterprise, enterpriseInfo);

                _enterpriseInfoRepository.SaveChanges();
                return new JsonResponse(OperatingState.Success, "编辑成功，信息已提交！");

            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "修改时出现异常！");
            }
        }

        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResponse ModifyEnterpriseStatus(EnterpriseInfoAuditRequest request)
        {
            var msg = string.Empty;
            var enterprise = _enterpriseInfoRepository.GetById(request.EnterpriseID);
            if (enterprise == null)
            {
                return new JsonResponse(OperatingState.CheckDataFail, "未发现该企业或已被删除");
            }
            #region 
            switch (enterprise.State)
            {
                case (int)EnterpriseState.New:
                    if (request.IsAuditPass)
                    {
                        enterprise.State = (int)EnterpriseState.Normal;
                        msg = "审核通过，数据生效";
                    }
                    else
                    {
                        enterprise.State = (int)EnterpriseState.Invalid;
                        msg = "审核不通过，数据不生效";
                    }
                    break;
                case (int)EnterpriseState.Modity:

                    if (request.IsAuditPass)
                    {
                        enterprise.State = (int)EnterpriseState.Normal;
                        enterprise.Safety = enterprise.SafetyToAudit;
                        enterprise.CreditCode = enterprise.CreditCodeToAudit;
                        enterprise.AptitudeCertificate = enterprise.AptitudeCertificateToAudit;
                        enterprise.RegisteredCapital = enterprise.RegisteredCapitalToAudit;
                        enterprise.BusinessLicenseNum = enterprise.BusinessLicenseNumToAudit;
                        enterprise.SafetyToAudit = null;
                        enterprise.CreditCodeToAudit = null;
                        enterprise.AptitudeCertificateToAudit = null;
                        enterprise.RegisteredCapitalToAudit = 0;
                        enterprise.BusinessLicenseNumToAudit = null;
                        msg = "审核通过，新的数据已生效";
                    }
                    else
                    {
                        enterprise.State = (int)EnterpriseState.Unapprove;
                        enterprise.SafetyToAudit = null;
                        enterprise.CreditCodeToAudit = null;
                        enterprise.AptitudeCertificateToAudit = null;
                        enterprise.RegisteredCapitalToAudit = 0;
                        enterprise.BusinessLicenseNumToAudit = null;
                        msg = "审核不通过，数据不做变更";
                    }
                    break;

            }
            #endregion

            _enterpriseInfoRepository.SaveChanges();

            return new JsonResponse(OperatingState.Success, msg);
        }

        /// <summary>
        /// 验证验证码是否正确，正确后注册企业帐户
        /// </summary>
        /// <param name="requestEntity"></param>
        /// <returns></returns>
        public JsonResponse ValidateRegister(UserPreRegisterValidationSMSRequest requestEntity)
        {
            try
            {
                if (requestEntity == null)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "未传入需要的数据");
                }
                if (string.IsNullOrWhiteSpace(requestEntity.PhoneNo))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "手机号不能为空！");
                }
                if (string.IsNullOrWhiteSpace(requestEntity.ValidationCode))
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "验证码不能为空！");
                }
                var entity = _userPreRegisterRecord.Table.FirstOrDefault(c => c.PhoneNo == requestEntity.PhoneNo);
                if (entity == null || entity.DeletedState == (int)DeletedStates.Deleted)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "该手机号还没有获取验证码！");
                }
                if (DateTime.Now > entity.ValidationCodeExpiredEndTime)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "验证码已经过期，请重新获取！");
                }
                if (entity.ValidationCode != requestEntity.ValidationCode)
                {
                    return new JsonResponse(OperatingState.CheckDataFail, "验证码不正确！");
                }
                //验证码正确，添加用户信息
                var enteUserEntity = _enterpriseUserInfo.Table.FirstOrDefault(c => c.UserName == requestEntity.PhoneNo);
                if (enteUserEntity == null || enteUserEntity.DeletedState == (int)DeletedStates.Deleted)
                {
                    _enterpriseUserInfo.Insert(new UserInfo()
                    {
                        UserName = requestEntity.PhoneNo,
                        Phone = requestEntity.PhoneNo,
                        SubTime = DateTime.Now,
                        NickName = "用户" + requestEntity.PhoneNo,
                        UserTypeEnum = 1, //用户类别。1=管理员，2=其它用户人员。
                        AccountState = (int)UserInfo_AccountState.Normal,
                        DeletedState = null,
                        DeletedTime = null
                    });
                    _enterpriseUserInfo.SaveChanges();

                    return new JsonResponse(OperatingState.Success, "用户注册成功");
                }
                return new JsonResponse(OperatingState.Failure, "系统中已经存在该用户！");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "添加数据失败", e.Message);
            }
        }

        /// <summary>
        /// 一一匹配修改项
        /// </summary>
        /// <param name="enterprise"></param>
        /// <param name="enterpriseInfo"></param>
        private static void Modity(EnterpriseInfoRequest enterprise, EnterpriseInfo enterpriseInfo)
        {
            enterpriseInfo.State = enterprise.State;
            enterpriseInfo.Name = enterprise.Name;
            enterpriseInfo.LocationProvinceCode = enterprise.LocationProvinceCode;
            enterpriseInfo.LocationCityCode = enterprise.LocationCityCode;
            enterpriseInfo.LocationDistrictCode = enterprise.LocationDistrictCode;
            enterpriseInfo.Address = enterprise.Address;
            enterpriseInfo.Tel = enterprise.Tel;
            enterpriseInfo.TypeName = enterprise.TypeName;
            enterpriseInfo.RepresentativePerson = enterprise.RepresentativePerson;
            enterpriseInfo.RepresentativeTel = enterprise.RepresentativeTel;
            enterpriseInfo.Email = enterprise.Email;
            enterpriseInfo.OpenAnAccount = enterprise.OpenAnAccount;
            enterpriseInfo.BankAccount = enterprise.BankAccount;
            enterpriseInfo.Nature = enterprise.Nature;
            enterpriseInfo.RegisterUserID = enterprise.RegisterUserID;
            enterpriseInfo.Remark = enterprise.Remark;
            enterpriseInfo.Safety = enterprise.Safety;
            enterpriseInfo.CreditCode = enterprise.CreditCode;
            enterpriseInfo.AptitudeCertificate = enterprise.AptitudeCertificate;
            enterpriseInfo.RegisteredCapital = enterprise.RegisteredCapital;
            enterpriseInfo.BusinessLicenseNum = enterprise.BusinessLicenseNum;
        }

      
    }
}
