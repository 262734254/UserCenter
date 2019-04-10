using JNKJ.Dto.Results;
namespace JNKJ.Services.Face
{
    /// <summary>
    /// 人脸识别接口
    /// </summary>
    public interface IFaceIdService
    {
        /// <summary>
        /// 人脸对比
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool Contrast(FaceRequest request);
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="image"></param>
        JsonResponse Detect(string image, string userId);
        /// <summary>
        /// 人脸检测
        /// </summary>
        /// <param name="image"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool IsDetect(string image, string userId);
        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        JsonResponse UserAdd(FaceRequest request);
        /// <summary>
        /// 人脸删除
        /// </summary>
        /// <param name="userId"></param>
        bool FaceDelete(string userId);
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        JsonResponse GetAccessToken();
        /// <summary>
        /// 获取人脸列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        JsonResponse GetFaceList(string userId);
    }
}
