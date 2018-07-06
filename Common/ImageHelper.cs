using CodeCarvings.Piczard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IMS.Common
{
    public static class ImageHelper
    {
        public static bool SaveBase64(string file,out string res)
        {
            if(!file.Contains(";base64"))
            {
                res = "不是base64图片文件";
                return false;
            }
            string[] strs = file.Split(',');
            string[] formats = strs[0].Replace(";base64", "").Split(':');
            string img = strs[1];
            string format = formats[1];
            string[] imgFormats = { "image/png", "image/jpg", "image/jpeg", "image/bmp", "IMAGE/PNG", "IMAGE/JPG", "IMAGE/JPEG", "IMAGE/BMP" };

            if (!imgFormats.Contains(format))
            {
                res = "请选择正确的图片格式，支持png、jpg、jpeg、png格式";
                return false;
            }
            string ext = "." + format.Split('/')[1];
            byte[] imgBytes = null;
            try
            {
                imgBytes = Convert.FromBase64String(img);
            }
            catch (Exception ex)
            {
                res = "base64图片文件解码错误";
                return false;
            }

            string md5 = CommonHelper.GetMD5(imgBytes);
            string path = "/upload/" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + md5 + ext;
            string fullPath = HttpContext.Current.Server.MapPath("~" + path);
            new FileInfo(fullPath).Directory.Create();

            //Install-Package CodeCarvings.Piczard
            ImageProcessingJob jobNormal = new ImageProcessingJob();
            jobNormal.SaveProcessedImageToFileSystem(imgBytes, fullPath);
            res = path;
            return true;
        }
    }
}
