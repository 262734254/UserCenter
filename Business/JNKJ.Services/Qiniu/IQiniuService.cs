using JNKJ.Dto.Results;
namespace JNKJ.Services.Qiniu
{
    public interface IQiniuService
    {
        /// <summary>
        /// 断点续传
        /// </summary>
        /// <param name="localFile"></param>
        /// <returns></returns>
        string ResumeUploadFile(string localFile);
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="localFile"></param>
        /// <returns></returns>
        string UploadFile(string localFile);
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetFileInfo(string fileName);
        /// <summary>
        /// 下载文件_私有空间
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string DownLoadPrivateUrl(string fileName);
        /// <summary>
        /// 直接下载文件，不在浏览器中打开
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string DownLoadPrivateUrlPath(string fileName);
        /// <summary>
        /// 下载文件_公开空间
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string DownLoadPublishUrl(string fileName);
        /// <summary>
        /// 获取TOKEN
        /// </summary>
        /// <returns></returns>
        JsonResponse GetToken();
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <returns></returns>
        string GetDomain();
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <returns></returns>
        JsonResponse GetDomainToWeb();
    }
}
