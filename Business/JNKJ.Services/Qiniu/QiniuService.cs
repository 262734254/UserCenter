using System;
using Qiniu.Storage;
using Qiniu.Util;
using System.Configuration;
using System.IO;
using JNKJ.Dto.Enums;
using JNKJ.Dto.Results;
using Qiniu.Http;
namespace JNKJ.Services.Qiniu
{
    public class QiniuService : IQiniuService
    {
        public string AccessKey = ConfigurationManager.AppSettings["QiniuAccessKey"];
        public string SecretKey = ConfigurationManager.AppSettings["QiniuSecretKey"];
        public string Bucket = ConfigurationManager.AppSettings["QiniuBucket"];
        public string Domain = ConfigurationManager.AppSettings["QiniuDomain"];
        /// <summary>
        /// 断点续传
        /// </summary>
        /// <param name="localFile"></param>
        /// <returns></returns>
        public string ResumeUploadFile(string localFile)
        {
            if (!File.Exists(localFile))
            {
                return "文件不存在";
            }
            var filename = Path.GetFileName(localFile);//文件名
            var extension = Path.GetExtension(localFile);//扩展名
            var mac = new Mac(AccessKey, SecretKey);
            var rand = new Random();
            string key = $"jnkj_{rand.Next()}{extension}";
            Stream fs = File.OpenRead(localFile);
            var putPolicy = new PutPolicy { Scope = Bucket + ":" + key };
            putPolicy.SetExpires(3600);
            putPolicy.DeleteAfterDays = 1;
            var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            var config = new Config
            {
                UseHttps = true,
                Zone = Zone.ZONE_CN_East,
                UseCdnDomains = true,
                ChunkSize = ChunkUnit.U512K
            };
            var target = new ResumableUploader(config);
            var extra = new PutExtra { ResumeRecordFile = ResumeHelper.GetDefaultRecordKey(localFile, key) };
            //设置断点续传进度记录文件
            Console.WriteLine("record file:" + extra.ResumeRecordFile);
            extra.ResumeRecordFile = "test.progress";
            var result = target.UploadStream(fs, key, token, extra);
            Console.WriteLine("resume upload: " + result);
            return result.ToString();
        }
        /// <summary>
        /// 简单上传
        /// </summary>
        public string UploadFile(string localFile)
        {
            if (!File.Exists(localFile))
            {
                return "文件不存在";
            }
            var filename = Path.GetFileName(localFile);//文件名
            var extension = Path.GetExtension(localFile);//扩展名
            var mac = new Mac(AccessKey, SecretKey);
            var rand = new Random();
            string key = filename;
            //string key = $"jnkj_{rand.Next()}{extension}";
            var putPolicy = new PutPolicy { Scope = Bucket };
            putPolicy.SetExpires(31536000);
            //putPolicy.DeleteAfterDays = 1;
            var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            var config = new Config
            {
                Zone = Zone.ZONE_CN_South,
                UseHttps = true,
                UseCdnDomains = true,
                ChunkSize = ChunkUnit.U512K
            };
            FormUploader target = new FormUploader(config);
            var result = target.UploadFile(localFile, key, token, null);
            return result.Code.ToString();
        }
        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetFileInfo(string fileName)
        {
            // 设置存储区域
            Config config = new Config();
            config.Zone = Zone.ZONE_CN_East;
            Mac mac = new Mac(AccessKey, SecretKey);
            BucketManager bucketManager = new BucketManager(mac, config);
            //待查询文件名
            string key = fileName;
            StatResult statRet = bucketManager.Stat(Bucket, key);
            if (statRet.Code != (int)HttpCode.OK)
            {
                Console.WriteLine("stat error: " + statRet.ToString());
            }
            Console.WriteLine(statRet.Result.Hash);
            Console.WriteLine(statRet.Result.MimeType);
            Console.WriteLine(statRet.Result.Fsize);
            Console.WriteLine(statRet.Result.MimeType);
            Console.WriteLine(statRet.Result.FileType);
            return statRet.ToString();
        }
        /// <summary>
        /// 获取TOKEN
        /// </summary>
        /// <returns></returns>
        public JsonResponse GetToken()
        {
            var mac = new Mac(AccessKey, SecretKey);
            string key = $"jnkj_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            var putPolicy = new PutPolicy { Scope = Bucket };
            putPolicy.SetExpires(31536000);
            //putPolicy.DeleteAfterDays = 1;
            var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            return new JsonResponse(OperatingState.Success, "获取Token成功", token);
        }
        /// <summary>
        /// 下载文件_私有空间
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string DownLoadPrivateUrl(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Mac mac = new Mac(AccessKey, SecretKey);
                string privateUrl = DownloadManager.CreatePrivateUrl(mac, Domain, fileName);
                return privateUrl;
            }
            return "";
        }
        /// <summary>
        /// 直接下载文件，不在浏览器中打开
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string DownLoadPrivateUrlPath(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Mac mac = new Mac(AccessKey, SecretKey);
                string privateUrl = DownloadManager.CreatePrivateUrl(mac, Domain, fileName + "?attname=" + fileName);
                return privateUrl;
            }
            return "";
        }
        /// <summary>
        /// 下载文件_公开空间
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string DownLoadPublishUrl(string fileName)
        {
            string publicUrl = DownloadManager.CreatePublishUrl(Domain, fileName);
            return publicUrl;
        }
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <returns></returns>
        public string GetDomain()
        {
            return Domain;
        }
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <returns></returns>
        public JsonResponse GetDomainToWeb()
        {
            return new JsonResponse(OperatingState.Success, "获取域名成功", Domain);
        }
    }
}
