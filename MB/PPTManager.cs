using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moban.DAL;
using Microsoft.Office.Interop.PowerPoint;
using System.IO;
using System.Collections;
using Moban.Model;
using Microsoft.Office.Core;
using Moban.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace MB
{
    public class PPTManager
    {
        public volatile bool shouldStop;
        private MainForm form;
        private int iStart;
        private int iEnd;
        private bool synchronous;
        private string fileFolders;
        //SqlHelp sqlHelp = new SqlHelp();

        Moban.DAL.PPTDAL dal = new PPTDAL();

        public PPTManager(MainForm frm, int iS, int iE)
        {
            this.form = frm;
            this.iEnd = iE;
            this.iStart = iS;
            this.synchronous = true;
        }
        public PPTManager(MainForm frm, string folders)
        {
            this.form = frm;
            this.fileFolders = folders;
        }


        public void PPTArrangeWithFile()
        {
            try
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxC);
                ApplicationClass pptApplication = new ApplicationClass();
                DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
                Hashtable ht = new Hashtable();
                string mainPath = "E:\\MYPPT\\";
                int i = TheFolder.GetDirectories().Count();
                //遍历文件夹
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    try
                    {
                        //根据子文件夹名称获取是哪条记录，根据记录，生成新的文件夹。
                        PPTInfo pptinfo = dal.GetModelByFileName(NextFolder.Name + ".rar");
                        if (pptinfo == null) //如果找打不到，继续
                        {
                            form.AddMessage(NextFolder.Name + "不存在", MainForm.richTextBoxLog);
                            continue;
                        }

                        foreach (FileInfo file in NextFolder.GetFiles())
                        {

                            #region 去掉HTML文件
                            //去掉HTML文件 
                            //因为会把ppt复制到另外的文件夹内，html文件可以不删除
                            //if (file.Extension == ".html")
                            //{
                            //    file.Delete();
                            //    continue;
                            //}
                            #endregion

                            if (file.Extension == ".ppt" || file.Extension == ".pptx")
                            {
                                //var app = new Microsoft.Office.Interop.PowerPoint.Application();
                                //Presentation ppt=  app.Presentations.Open2007(file.FullName, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoTrue, MsoTriState.msoFalse);
                                //app.ActivePresentation.SaveAs(@"R:\LiChao\Temp\test\CCB_F4I;09_Field Work - Part 1 Field Work.pptx", PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoFalse);
                                //app.ActivePresentation.Close();

                                Presentation pptPresentation = pptApplication.Presentations.Open2007(file.FullName, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
                                //int master = pptPresentation.Designs.Count;
                                //string s =  pptPresentation.SlideMaster.HeadersFooters.Header.Text;
                                //Master master = pptPresentation.SlideMaster;
                                //var doc = pptPresentation.BuiltInDocumentProperties;
                                 
                                foreach (Microsoft.Office.Interop.PowerPoint.Slide slide in pptPresentation.Slides)
                                {
                                    //Master masters = slide.Master;
                                    //Microsoft.Office.Interop.PowerPoint.Slide sd = slide;
                                    //string ss = sd.Master.HeadersFooters.Header.Text;
                                    foreach (Microsoft.Office.Interop.PowerPoint.Shape shape in slide.Shapes)
                                    {
                                        if (MsoTriState.msoTrue == shape.HasTextFrame)
                                        {
                                            shape.TextFrame.TextRange.Replace("第一PPT模板网", "PPT中国网-PPTCN", 0, MsoTriState.msoFalse, MsoTriState.msoFalse);
                                            shape.TextFrame.TextRange.Replace("www.1ppt.com", "www.pptcn.cn", 0, MsoTriState.msoFalse, MsoTriState.msoFalse);
                                            if (shape.TextFrame.TextRange.Text.IndexOf("ppt") > -1)
                                            {
                                                pptinfo.IsHavePPT = 1;
                                            }
                                        }
                                    }

                                    ExportImages(mainPath + "images\\" + pptinfo.Id.ToString() + "\\n\\", slide, slide.SlideIndex, 0);
                                    ExportImages(mainPath + "images\\" + pptinfo.Id.ToString() + "\\220\\", slide, slide.SlideIndex, 220);
                                    ExportImages(mainPath + "images\\" + pptinfo.Id.ToString() + "\\130\\", slide, slide.SlideIndex, 130);
                                    ExportImages(mainPath + "images\\" + pptinfo.Id.ToString() + "\\80\\", slide, slide.SlideIndex, 80);
                                }

                                //CreateIndexImg(mainPath + "images\\" + pptinfo.Id.ToString() + "\\n\\", pptinfo.Id);

                                pptinfo.Pages = pptPresentation.Slides.Count;
                                pptinfo.Keywords = StringHelper.GetRegValue(pptinfo.Body, "<meta name=\"keywords\" content=\"(.*?)\" />");
                                pptinfo.Description = StringHelper.GetRegValue(pptinfo.Body, "<meta name=\"description\" content=\"(.*?)\" />");

                                dal.PPTArrangeWithFile(pptinfo);

                                if (!Directory.Exists(mainPath + "ppt\\" + pptinfo.Id + "\\"))
                                {
                                    Directory.CreateDirectory(mainPath + "ppt\\" + pptinfo.Id + "\\");
                                }
                               
                                pptPresentation.SaveAs(mainPath + "ppt\\" + pptinfo.Id + "\\" + pptPresentation.Name, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoFalse);
                                pptApplication.ActivePresentation.Close();
                            }
                        }
                        NextFolder.Delete(true);
                        form.AddMessage(i + "   " + NextFolder.Name, MainForm.richTextBoxC);
                        i--;
                    }
                    catch (Exception ex)
                    {
                        form.AddMessage(ex.Message, MainForm.richTextBoxLog);
                        pptApplication.ActivePresentation.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                form.AddMessage(DateTime.Now + "...............抓取循环开始.............", MainForm.richTextBoxLog);
            }
        }

        //生成图片
        public void ExportImages(string filePath, Microsoft.Office.Interop.PowerPoint.Slide slide, int index, int width)
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            slide.Export(filePath + "\\" + index + ".jpg", "jpg", width, (int)(slide.Application.Height * width / slide.Application.Width));
        }

        public void CreateIndexImg(string fileFolders, int id)
        {


            DirectoryInfo TheFolder = new DirectoryInfo(fileFolders);
            var imgFiles = from file in
                               Directory.EnumerateFiles(fileFolders)
                           where file.ToLower().EndsWith(".jpg")
                           select file;
            List<string> imgList = new List<string>();
            int mod = imgFiles.Count<string>() % 3;
            imgList = imgFiles.ToList();
            switch (mod)
            {
                case 1:
                    imgList.Remove("1.jpg");
                    break;
                case 2:
                    imgList.Add("D:\\Personal\\1.jpg");
                    break;
                default:
                    break;
            }

            if (imgFiles.Count<string>() == 1)
            {
                Bitmap b1 = new Bitmap(fileFolders + "1.jpg");
                Size size = new System.Drawing.Size();
                size.Width = 650;
                size.Height = (int)b1.Height * 660 / b1.Width;
                Bitmap aImage = GetImageThumb(b1, size);
                SaveAsJPEG(aImage, fileFolders.Replace("n\\", "") + id + ".jpg", 50);
            }
            else
            {
                int row = imgList.Count / 5 + 1;
                if (row > 3)
                {
                    row = 3;
                }
                Bitmap b1 = new Bitmap(fileFolders + "1.jpg");
                int height = (int)b1.Height * 660 / b1.Width;
                int rh = (int)b1.Height * 126 / b1.Width;

                Size size = new System.Drawing.Size();
                size.Width = 660;
                size.Height = height;
                Bitmap indexImage = GetImageThumb(b1, size);

                Bitmap bImage = new Bitmap(660, height + (rh + 5) * row + 10);
                Graphics g = Graphics.FromImage(bImage);
                g.Clear(System.Drawing.Color.White);
                g.DrawImage(indexImage, 0, 0, 660, height);

                int i = 0;
                foreach (string item in imgList)
                {
                    Bitmap b = new Bitmap(item);
                    int left = 10 + (i % 5) * 10 + (i % 5) * 120;
                    int top = 5 + height + (i / 5) * 5 + (i / 5) * rh;
                    g.DrawImage(b, left, top, 120, rh);
                    i++;
                    if (i == 15)
                    {
                        break;
                    }
                    //rtb_C.AppendText(i + " : " + left + "  " + top + "\r\n");
                }

                
                SaveAsJPEG(bImage, fileFolders.Replace("n\\", "") + id + ".jpg", 50);

                //aImage.Save(fileFolders.Replace("n\\", "") + "index2.jpg", System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        //压缩图片
        public Bitmap GetImageThumb(Bitmap mg, Size newSize)
        {
            double ratio = 0d;
            double myThumbWidth = 0d;
            double myThumbHeight = 0d;
            int x = 0;
            int y = 0;

            Bitmap bp;

            if ((mg.Width / Convert.ToDouble(newSize.Width)) > (mg.Height /
            Convert.ToDouble(newSize.Height)))
                ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
            else
                ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
            myThumbHeight = Math.Ceiling(mg.Height / ratio);
            myThumbWidth = Math.Ceiling(mg.Width / ratio);

            Size thumbSize = new Size((int)newSize.Width, (int)newSize.Height);
            bp = new Bitmap(newSize.Width, newSize.Height);
            x = (newSize.Width - thumbSize.Width) / 2;
            y = (newSize.Height - thumbSize.Height);
            System.Drawing.Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
            g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);

            return bp;
        }

        public static bool GetPicThumbnail(string sFile, string outPath, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;

            //以下代码为保存图片时，设置压缩质量  
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    iSource.Save(outPath, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    iSource.Save(outPath, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                iSource.Dispose();
            }
        }

        /// <summary>
        /// 保存为JPEG格式，支持压缩质量选项
        /// </summary>
        /// <param name="bmp">原始位图</param>
        /// <param name="FileName">新文件地址</param>
        /// <param name="Qty">压缩质量，越大越好，文件也越大(0-100)</param>
        /// <returns>成功标志</returns>
        public static bool SaveAsJPEG(Bitmap bmp, string FileName, int Qty)
        {
            try
            {
                EncoderParameter p;
                EncoderParameters ps;

                ps = new EncoderParameters(1);

                p = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Qty);
                ps.Param[0] = p;

                bmp.Save(FileName, GetCodecInfo("image/jpeg"), ps);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存JPG时用
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns>得到指定mimeType的ImageCodecInfo</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }

    }
}
