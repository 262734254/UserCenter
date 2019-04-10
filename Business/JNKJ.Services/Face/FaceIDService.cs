using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using JNKJ.Core.Infrastructure;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using JNKJ.Services.Qiniu;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace JNKJ.Services.Face
{
    public class FaceIdService : IFaceIdService
    {
        //文字识别 API Key
        private const string ClientId = "cLl0DDTul35hHrU5anKBBEe2";
        //文字识别 Secret Key
        private const string ClientSecret = "MpTkLGRya2GhPNLwyzFajIz0eH120QA9";
        //人脸识别 API Key
        private const string ApiKey = "dUmvdasDcZQ1LpUQBsycbnFh";
        //人脸识别 Secret Key
        private const string SecretKey = "HfPoa0cjBrHGBzSuhAZU0aNOms8zKnRY";
        /// <summary>
        /// 图片类型 
        /// BASE64:         图片的base64值，base64编码后的图片数据，需urlencode，编码后的图片大小不超过2M；
        /// URL:            图片的 URL地址( 可能由于网络等原因导致下载图片时间过长)；
        /// FACE_TOKEN:     人脸图片的唯一标识，调用人脸检测接口时，会为每个人脸图片赋予一个唯一的FACE_TOKEN，同一张图片多次检测得到的FACE_TOKEN是同一个
        /// </summary>
        private const string ImageType = "URL";
        /// <summary>
        /// 用户组id
        /// （由数字、字母、下划线组成），长度限制128B
        /// </summary>
        private const string GroupId = "jnkj";
        private Baidu.Aip.Face.Face Client { get; } = new Baidu.Aip.Face.Face(ApiKey, SecretKey) { Timeout = 60000 };
        /// <summary>
        /// 人脸对比
        /// 优先注册一张照片到人脸库
        /// 拍照获取照片生成网络路径调用此接口
        /// 对比结果大于50或60表示认证成功
        /// </summary>
        /// <returns>是否对比成功</returns>
        public bool Contrast(FaceRequest request)
        {
            try
            {
                //获取当前用户 最近登记的打卡图片的faceToken
                var userFaceToken = "";
                var faceResult = FaceGetlist(request.userId);
                if (faceResult == null || faceResult.face_list.Count <= 0) return false;
                var userFace = faceResult.face_list.FirstOrDefault(s => s.face_token == userFaceToken);
                var faces = new JArray
                {
                    new JObject
                    {
                        {"image", request.image},
                        {"image_type", ImageType},
                        {"face_type", "LIVE"},      //人脸的类型  LIVE  表示生活照：通常为手机、相机拍摄的人像图片、或从网络获取的人像图片等
                        {"quality_control", "LOW"}, //图片质量控制 LOW 较低的质量要求 
                    },
                    new JObject
                    {
                        {"image", userFace?.face_token},
                        {"image_type", "FACE_TOKEN"},
                        {"face_type", "LIVE"},
                        {"quality_control", "LOW"},
                    }
                };
                var result = Client.Match(faces);
                var faceList = result.SelectToken("result");
                var score = DeserializeToModel<FaceList>(faceList.ToString());
                if (score == null) { return false; }
                return Convert.ToDouble(score.score) >= 50;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 人脸检测
        /// 考勤打卡使用
        /// </summary>
        /// <param name="image"></param>
        /// <param name="userId"></param>
        public bool IsDetect(string image, string userId)
        {
            var qiniu = EngineContext.Current.Resolve<IQiniuService>();
            var imgUrl = qiniu.DownLoadPrivateUrl(image);
            var options = new Dictionary<string, object>{
                {"face_field", "quality"},//人脸质量信息
                {"max_face_num", 10},//最多处理人脸的数目，默认值为1，仅检测图片中面积最大的那个人脸；最大值10，检测图片中面积最大的几张人脸。
                {"face_type", "LIVE"}//人脸的类型 LIVE表示生活照：通常为手机、相机拍摄的人像图片、或从网络获取的人像图片等
             };
            var result = false;
            var isPass = false;
            try
            {
                var obj = Client.Detect(imgUrl, ImageType, options);
                if (!obj.SelectToken("error_code").ToString().Equals("0")) return false;
                var faceList = obj.SelectToken("result");
                var detectResult = DeserializeToModel<DetectClass>(faceList.ToString());
                if (detectResult.face_num != 1) return false;
                foreach (var item in detectResult.face_list.Where(item => item.face_probability > 0.5))
                {
                    var occlusion = item.quality.occlusion;
                    if (occlusion.left_cheek < 0.5 && occlusion.right_cheek < 0.5
                        && occlusion.left_eye < 0.5 && occlusion.right_eye < 0.5
                        && occlusion.nose < 0.5 && occlusion.mouth < 0.5
                        && occlusion.chin < 0.5)
                    {
                        result = true;
                    }
                }
                //检测完毕
                if (result)//表示检查通过，可以进行人脸对比
                {
                    isPass = Contrast(new FaceRequest() { image = imgUrl, userId = userId });
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return isPass;
        }
        /// <summary>
        /// 人脸注册
        /// 注册前需要检测人脸图像是否合格
        /// 暂时不做分组，整个项目同属一个分组内
        /// 使用登录用户的手机号码作为userId
        /// </summary>
        public JsonResponse UserAdd(FaceRequest request)
        {
            try
            {
                var qiniu = EngineContext.Current.Resolve<IQiniuService>();
                var result = Client.UserAdd(qiniu.DownLoadPrivateUrl(request.image), ImageType, GroupId, request.userId);
                var faceList = result.SelectToken("result");
                if (!faceList.Any())
                {
                    return new JsonResponse(OperatingState.Failure, "人脸导入失败，原因：" + GetError_msg(result.SelectToken("error_code").ToString()));
                }
                var resultStr = DeserializeToModel<FaceListItem>(faceList.ToString());
                //注册成功，修改用户的FaceImg和FaceToken
                //var userInfo = _enterpriseUserInfo.Table.FirstOrDefault(s => s.UserName == request.userId);
                //if (userInfo == null) return new JsonResponse(OperatingState.Failure, "人脸导入失败，原因：系统中未找到对应的用户");
                //userInfo.FaceImg = request.image;
                //userInfo.FaceToken = resultStr.face_token;
                //_enterpriseUserInfo.SaveChanges();
                return new JsonResponse(OperatingState.Success, "人脸导入成功");
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "人脸导入成功，原因：", e.Message);
            }
        }
        /// <summary>
        /// 错误码
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public string GetError_msg(string errorCode)
        {
            switch (errorCode)
            {
                case "222207":
                    return "未找到匹配的用户";
                case "222208":
                    return "图片的数量错误";
                case "222209":
                    return "faceToken不存在";
                case "222300":
                    return "人脸图片添加失败";
                case "222302":
                    return "服务端请求失败";
                case "223102":
                    return "该用户已存在";
                case "223103":
                    return "找不到该用户";
                case "223105":
                    return "该人脸已存在";
                case "223106":
                    return "该人脸不存在";
                case "222204":
                    return "从图片的url下载图片失败";
            }
            return "系统繁忙";
        }
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="image"></param>
        public JsonResponse Detect(string image, string userId)
        {
            var qiniu = EngineContext.Current.Resolve<IQiniuService>();
            var imgUrl = qiniu.DownLoadPrivateUrl(image);
            var options = new Dictionary<string, object>{
                {"face_field", "quality"},//人脸质量信息
                {"max_face_num", 10},//最多处理人脸的数目，默认值为1，仅检测图片中面积最大的那个人脸；最大值10，检测图片中面积最大的几张人脸。
                {"face_type", "LIVE"}//人脸的类型 LIVE表示生活照：通常为手机、相机拍摄的人像图片、或从网络获取的人像图片等
             };
            var result = false;
            var isPass = false;
            try
            {
                var obj = Client.Detect(imgUrl, ImageType, options);
                if (!obj.SelectToken("error_code").ToString().Equals("0")) return new JsonResponse(OperatingState.Failure, "人脸对比失败");
                var faceList = obj.SelectToken("result");
                var detectResult = DeserializeToModel<DetectClass>(faceList.ToString());
                if (detectResult.face_num != 1) return new JsonResponse(OperatingState.Failure, "人脸对比失败");
                foreach (var item in detectResult.face_list.Where(item => item.face_probability > 0.5))
                {
                    var occlusion = item.quality.occlusion;
                    if (occlusion.left_cheek < 0.5 && occlusion.right_cheek < 0.5
                        && occlusion.left_eye < 0.5 && occlusion.right_eye < 0.5
                        && occlusion.nose < 0.5 && occlusion.mouth < 0.5
                        && occlusion.chin < 0.5)
                    {
                        result = true;
                    }
                }
                //检测完毕
                if (result)//表示检查通过，可以进行人脸对比
                {
                    isPass = Contrast(new FaceRequest() { image = imgUrl, userId = userId });
                }
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Failure, "人脸对比失败，原因：", e.Message);
            }
            return new JsonResponse(OperatingState.Success, result ? (isPass ? "人脸对比通过" : "人脸对比不通过") : "人脸检测不通过", new { IsDetect = result, IsContrast = isPass });
        }
        /// <summary>
        /// 人脸删除
        /// </summary>
        public bool FaceDelete(string userId)
        {
            var resultStr = string.Empty;
            var faceResult = FaceGetlist(userId);
            resultStr = faceResult?.face_list.Select(item => Client.FaceDelete(userId, GroupId, item.face_token)).Aggregate(resultStr, (current, result) => current + result);
            //foreach (var result in faceResult.face_list.Select(item => Client.FaceDelete(userId, GroupId, item.face_token)))
            //{
            //    resultStr += result;
            //}
            return true;
        }
        /// <summary>
        /// 获取用户人脸列表
        /// </summary>
        private FaceList FaceGetlist(string userId)
        {
            try
            {
                var result = Client.FaceGetlist(userId, GroupId);
                var faceList = result.SelectToken("result");
                return DeserializeToModel<FaceList>(faceList.ToString());
            }
            catch (Exception)
            {
                return new FaceList();
            }
        }
        /// <summary>
        /// 获取用户人脸列表
        /// </summary>
        public JsonResponse GetFaceList(string userId)
        {
            try
            {
                var result = Client.FaceGetlist(userId, GroupId);
                var faceList = result.SelectToken("result");
                var fList = DeserializeToModel<FaceList>(faceList.ToString());
                return new JsonResponse(OperatingState.Success, "获取人脸列表成功", fList);
            }
            catch (Exception)
            {
                return new JsonResponse(OperatingState.Failure, "获取人脸列表失败");
            }
        }
        /// <summary>
        /// 序列化成对象
        /// </summary>
        public static T DeserializeToModel<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public JsonResponse GetAccessToken()
        {
            try
            {
                const string authHost = "https://aip.baidubce.com/oauth/2.0/token";
                var client = new HttpClient();
                var paraList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret)
            };
                var response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                var tokenObj = DeserializeToModel<AccessTokenResponse>(result);
                return new JsonResponse(OperatingState.Success, "获取token成功", tokenObj.access_token);
            }
            catch (Exception e)
            {
                return new JsonResponse(OperatingState.Success, "获取token失败", e);
            }
        }
    }
}
