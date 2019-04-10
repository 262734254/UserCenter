using System.Collections.Generic;
namespace JNKJ.Services.Face
{
    /// <summary>
    /// 自定义返回类集合
    /// </summary>
    public class FaceList
    {
        public List<FaceListItem> face_list { get; set; }
        public string score { get; set; }
    }
    /// <summary>
    /// 自定义返回类实体
    /// </summary>
    public class FaceListItem
    {
        public string face_token { get; set; }
        public string ctime { get; set; }
    }
    /// <summary>
    /// 人脸识别_请求类
    /// </summary>
    public class FaceRequest
    {
        public string image { get; set; }
        public string userId { get; set; }
    }
    /// <summary>
    /// access_token_响应类
    /// </summary>
    public class AccessTokenResponse
    {
        public string access_token { get; set; }
        public string session_key { get; set; }
        public string refresh_token { get; set; }
        public string session_secret { get; set; }
        public string expires_in { get; set; }
    }
    #region 人脸检测相关自定义实体
    public class DetectClass
    {
        /// <summary>
        /// 检测到的图片中的人脸数量
        /// </summary>
        public int face_num { get; set; }
        /// <summary>
        /// 人脸信息列表，具体包含的参数参考下面的列表。
        /// </summary>
        public List<DetectItem> face_list { get; set; }
    }
    public class DetectItem
    {
        /// <summary>
        /// 人脸置信度，范围【0~1】，代表这是一张人脸的概率，0最小、1最大。
        /// </summary>
        public double face_probability { get; set; }
        public QualityItem quality { get; set; }
    }
    public class QualityItem
    {
        public OcclusionItem occlusion { get; set; }
        /// <summary>
        /// 人脸模糊程度，范围[0~1]，0表示清晰，1表示模糊
        /// </summary>
        public double blur { get; set; }
        /// <summary>
        /// 取值范围在[0~255], 表示脸部区域的光照程度 越大表示光照越好
        /// </summary>
        public double illumination { get; set; }
        /// <summary>
        /// 人脸完整度，0或1, 0为人脸溢出图像边界，1为人脸都在图像边界内
        /// </summary>
        public double completeness { get; set; }
    }
    /// <summary>
    /// 人脸各部分遮挡的概率，范围[0~1]，0表示完整，1表示不完整
    /// </summary>
    public class OcclusionItem
    {
        /// <summary>
        /// 左眼遮挡比例
        /// </summary>
        public double left_eye { get; set; }
        /// <summary>
        /// 右眼遮挡比例
        /// </summary>
        public double right_eye { get; set; }
        /// <summary>
        /// 鼻子遮挡比例
        /// </summary>
        public double nose { get; set; }
        /// <summary>
        /// 嘴巴遮挡比例
        /// </summary>
        public double mouth { get; set; }
        /// <summary>
        /// 左脸颊遮挡比例
        /// </summary>
        public double left_cheek { get; set; }
        /// <summary>
        /// 右脸颊遮挡比例
        /// </summary>
        public double right_cheek { get; set; }
        /// <summary>
        /// 下巴遮挡比例
        /// </summary>
        public double chin { get; set; }
    }
    #endregion
}
