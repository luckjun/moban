using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing.Imaging;
using System.Drawing;
using System.Web;

namespace Moban.Utils
{
    /// <summary>
    /// ToThumbnailImage 的摘要说明。
    /// </summary>
    public class ToThumbnailImage 
    {
        /*  
        Create By lion  
        2003-05-20 19:00  
        Copyright (C) 2004 http://www.lionsky.net/. All rights reserved. 
        Web: http://www.lionsky.net/ ;
        Email: lion-a@sohu.com 
        */


        static Hashtable htmimes = new Hashtable();
        internal readonly string AllowExt = ".jpe|.jpeg|.jpg|.png|.tif|.tiff|.bmp";

        //#region Web 窗体设计器生成的代码
        //override protected void OnInit(EventArgs e)
        //{
        //    #region htmimes[".jpe"]="image/jpeg";
        //    htmimes[".jpeg"] = "image/jpeg";
        //    htmimes[".jpg"] = "image/jpeg";
        //    htmimes[".png"] = "image/png";
        //    htmimes[".tif"] = "image/tiff";
        //    htmimes[".tiff"] = "image/tiff";
        //    htmimes[".bmp"] = "image/bmp";
        //    #endregion
        //    //调用生成缩略图方法
        //    ToThumbnailImages("lionsky.jpg", "b.gif", 300);
        //}
        //#endregion

        #region Helper

        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }

        /// <summary>
        /// 检测扩展名的有效性
        /// </summary>
        /// <param name="sExt">文件名扩展名</param>
        /// <returns>如果扩展名有效,返回true,否则返回false.</returns>
        bool CheckValidExt(string sExt)
        {
            bool flag = false;
            string[] aExt = AllowExt.Split('|');
            foreach (string filetype in aExt)
            {
                if (filetype.ToLower() == sExt)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="ici">指定格式的编解码参数</param>
        void SaveImage(System.Drawing.Image image, string savePath, ImageCodecInfo ici)
        {
            //设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, ((long)90));
            image.Save(savePath, ici, parameters);
            parameters.Dispose();
        }
        #endregion

        #region Methods

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="sourceImagePath">原图片路径(相对路径)</param>
        /// <param name="thumbnailImagePath">生成的缩略图路径,如果为空则保存为原图片路径(相对路径)</param>
        /// <param name="thumbnailImageWidth">缩略图的宽度（高度与按源图片比例自动生成）</param>
        public void ToThumbnailImages(string sourceImagePath, string thumbnailImagePath, int thumbnailImageWidth)
        {
            string SourceImagePath = sourceImagePath;
            string ThumbnailImagePath = thumbnailImagePath;
            int ThumbnailImageWidth = thumbnailImageWidth;
            string sExt = SourceImagePath.Substring(SourceImagePath.LastIndexOf(".")).ToLower();
            if (SourceImagePath.ToString() == System.String.Empty) throw new NullReferenceException("SourceImagePath is null!");
            if (!CheckValidExt(sExt))
            {
                throw new ArgumentException("原图片文件格式不正确,支持的格式有[ " + AllowExt + " ]", "SourceImagePath");
            }
            //从 原图片 创建 Image 对象
            System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(SourceImagePath));
            int num = ((ThumbnailImageWidth / 4) * 3);
            int width = image.Width;
            int height = image.Height;
            //计算图片的比例
            if ((((double)width) / ((double)height)) >= 1.3333333333333333f)
            {
                num = ((height * ThumbnailImageWidth) / width);
            }
            else
            {
                ThumbnailImageWidth = ((width * num) / height);
            }
            if ((ThumbnailImageWidth < 1) || (num < 1))
            {
                return;
            }
            //用指定的大小和格式初始化 Bitmap 类的新实例
            Bitmap bitmap = new Bitmap(ThumbnailImageWidth, num, PixelFormat.Format32bppArgb);
            //从指定的 Image 对象创建新 Graphics 对象
            Graphics graphics = Graphics.FromImage(bitmap);
            //清除整个绘图面并以透明背景色填充
            graphics.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制 原图片 对象
            graphics.DrawImage(image, new Rectangle(0, 0, ThumbnailImageWidth, num));
            image.Dispose();
            try
            {
                //将此 原图片 以指定格式并用指定的编解码参数保存到指定文件 
                string savepath = (ThumbnailImagePath == null ? SourceImagePath : ThumbnailImagePath);
                SaveImage(bitmap, HttpContext.Current.Server.MapPath(savepath), GetCodecInfo((string)htmimes[sExt]));
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                bitmap.Dispose();
                graphics.Dispose();
            }
        }
        #endregion

    }
}