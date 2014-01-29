using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace Moban.Utils
{
    public static class ImageHelper
    {

        /// <summary>
        /// 加图片水印
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="watermarkFilename">水印文件名</param>
        /// <param name="watermarkStatus">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中   9=右下</param>
        /// <param name="quality">附加图片质量，1是 0不是</param>
        /// <param name="watermarkTransparency">水印的透明度 1--10 10为不透明</param>
        public static void AddImageSignPic(Image img, string filename, string watermarkFilename, int watermarkStatus, int quality, int watermarkTransparency)
        {
            Graphics g = Graphics.FromImage(img);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkFilename);

            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
            {
                return;
            }

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
            {
                transparency = (watermarkTransparency / 10.0F);
            }

            float[][] colorMatrixElements = {
                                                new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                            };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);
            //g.DrawImage(watermark, new System.Drawing.Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, System.Drawing.GraphicsUnit.Pixel);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    ici = codec;
                }
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
            {
                quality = 80;
            }
            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
            {
                img.Save(filename, ici, encoderParams);
            }
            else
            {
                img.Save(filename);
            }

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 增加图片文字水印
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkStatus">图片水印位置</param>
        public static void AddImageSignText(Image img, string filename, string watermarkText, int watermarkStatus, int quality, string fontname, int fontsize)
        {
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(img);
            //    .FromFile(filename);
            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            //            System.Drawing.StringFormat StrFormat = new System.Drawing.StringFormat();
            //            StrFormat.Alignment = System.Drawing.StringAlignment.Center;
            //
            //            g.DrawString(watermarkText, drawFont, new System.Drawing.SolidBrush(System.Drawing.Color.White), xpos + 1, ypos + 1, StrFormat);
            //            g.DrawString(watermarkText, drawFont, new System.Drawing.SolidBrush(System.Drawing.Color.Black), xpos, ypos, StrFormat);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Black), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                {
                    ici = codec;
                }
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
            {
                quality = 80;
            }
            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
            {
                img.Save(filename, ici, encoderParams);
            }
            else
            {
                img.Save(filename);
            }
            g.Dispose();
            //bmp.Dispose();
            img.Dispose();
        }


        /// <summary>
        /// Creating a Watermarked Photograph with GDI+ for .NET
        /// </summary>
        /// <param name="rSrcImgPath">原始图片的物理路径</param>
        /// <param name="rMarkImgPath">水印图片的物理路径</param>
        /// <param name="rMarkText">水印文字（不显示水印文字设为空串）</param>
        /// <param name="rDstImgPath">输出合成后的图片的物理路径</param>
        /// @整理: anyrock@mending.cn
        public static void BuildWatermark(string rSrcImgPath, string rMarkImgPath, string rMarkText, string rDstImgPath)
        {
            //以下（代码）从一个指定文件创建了一个Image 对象，然后为它的 Width 和 Height定义变量。
            //这些长度待会被用来建立一个以24 bits 每像素的格式作为颜色数据的Bitmap对象。
            Image imgPhoto = Image.FromFile(rSrcImgPath);
            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            //这个代码载入水印图片，水印图片已经被保存为一个BMP文件，以绿色(A=0,R=0,G=255,B=0)作为背景颜色。
            //再一次，会为它的Width 和Height定义一个变量。
            Image imgWatermark = new Bitmap(rMarkImgPath);
            int wmWidth = imgWatermark.Width;
            int wmHeight = imgWatermark.Height;
            //这个代码以100%它的原始大小绘制imgPhoto 到Graphics 对象的（x=0,y=0）位置。
            //以后所有的绘图都将发生在原来照片的顶部。
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.DrawImage(
            imgPhoto,
            new Rectangle(0, 0, phWidth, phHeight),
            0,
            0,
            phWidth,
            phHeight,
            GraphicsUnit.Pixel);
            //为了最大化版权信息的大小，我们将测试7种不同的字体大小来决定我们能为我们的照片宽度使用的可能的最大大小。
            //为了有效地完成这个，我们将定义一个整型数组，接着遍历这些整型值测量不同大小的版权字符串。
            //一旦我们决定了可能的最大大小，我们就退出循环，绘制文本
            int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            Font crFont = null;
            SizeF crSize = new SizeF();
            for (int i = 0; i < 7; i++)
            {
                crFont = new Font("arial", sizes[i],
                FontStyle.Bold);
                crSize = grPhoto.MeasureString(rMarkText,
                crFont);
                if ((ushort)crSize.Width < (ushort)phWidth)
                    break;
            }
            //因为所有的照片都有各种各样的高度，所以就决定了从图象底部开始的5%的位置开始。
            //使用rMarkText字符串的高度来决定绘制字符串合适的Y坐标轴。
            //通过计算图像的中心来决定X轴，然后定义一个StringFormat 对象，设置StringAlignment 为Center。
            int yPixlesFromBottom = (int)(phHeight * .05);
            float yPosFromBottom = ((phHeight -
            yPixlesFromBottom) - (crSize.Height / 2));
            float xCenterOfImg = (phWidth / 2);
            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;
            //现在我们已经有了所有所需的位置坐标来使用60%黑色的一个Color(alpha值153)创建一个SolidBrush 。
            //在偏离右边1像素，底部1像素的合适位置绘制版权字符串。
            //这段偏离将用来创建阴影效果。使用Brush重复这样一个过程，在前一个绘制的文本顶部绘制同样的文本。
            SolidBrush semiTransBrush2 =
            new SolidBrush(Color.FromArgb(153, 0, 0, 0));
            grPhoto.DrawString(rMarkText,
            crFont,
            semiTransBrush2,
            new PointF(xCenterOfImg + 1, yPosFromBottom + 1),
            StrFormat);
            SolidBrush semiTransBrush = new SolidBrush(
            Color.FromArgb(153, 255, 255, 255));
            grPhoto.DrawString(rMarkText,
            crFont,
            semiTransBrush,
            new PointF(xCenterOfImg, yPosFromBottom),
            StrFormat);
            //根据前面修改后的照片创建一个Bitmap。把这个Bitmap载入到一个新的Graphic对象。
            Bitmap bmWatermark = new Bitmap(bmPhoto);
            bmWatermark.SetResolution(
            imgPhoto.HorizontalResolution,
            imgPhoto.VerticalResolution);
            Graphics grWatermark =
            Graphics.FromImage(bmWatermark);
            //通过定义一个ImageAttributes 对象并设置它的两个属性，我们就是实现了两个颜色的处理，以达到半透明的水印效果。
            //处理水印图象的第一步是把背景图案变为透明的(Alpha=0, R=0, G=0, B=0)。我们使用一个Colormap 和定义一个RemapTable来做这个。
            //就像前面展示的，我的水印被定义为100%绿色背景，我们将搜到这个颜色，然后取代为透明。
            ImageAttributes imageAttributes =
            new ImageAttributes();
            ColorMap colorMap = new ColorMap();
            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };
            //第二个颜色处理用来改变水印的不透明性。
            //通过应用包含提供了坐标的RGBA空间的5x5矩阵来做这个。
            //通过设定第三行、第三列为0.3f我们就达到了一个不透明的水平。结果是水印会轻微地显示在图象底下一些。
            imageAttributes.SetRemapTable(remapTable,
            ColorAdjustType.Bitmap);
            float[][] colorMatrixElements = {
　　new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
　　new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
　　new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
　　new float[] {0.0f,  0.0f,  0.0f,  0.3f, 0.0f},
　　new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
　　};
            ColorMatrix wmColorMatrix = new
            ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(wmColorMatrix,
            ColorMatrixFlag.Default,
            ColorAdjustType.Bitmap);
            //随着两个颜色处理加入到imageAttributes 对象，我们现在就能在照片右手边上绘制水印了。
            //我们会偏离10像素到底部，10像素到左边。
            int markWidth;
            int markHeight;
            //mark比原来的图宽
            if (phWidth <= wmWidth)
            {
                markWidth = phWidth - 10;
                markHeight = (markWidth * wmHeight) / wmWidth;
            }
            else if (phHeight <= wmHeight)
            {
                markHeight = phHeight - 10;
                markWidth = (markHeight * wmWidth) / wmHeight;
            }
            else
            {
                markWidth = wmWidth;
                markHeight = wmHeight;
            }
            int xPosOfWm = ((phWidth - markWidth) - 10);
            int yPosOfWm = 10;
            grWatermark.DrawImage(imgWatermark,
            new Rectangle(xPosOfWm, yPosOfWm, markWidth,
            markHeight),
            0,
            0,
            wmWidth,
            wmHeight,
            GraphicsUnit.Pixel,
            imageAttributes);
            //最后的步骤将是使用新的Bitmap取代原来的Image。 销毁两个Graphic对象，然后把Image 保存到文件系统。
            imgPhoto = bmWatermark;
            grPhoto.Dispose();
            grWatermark.Dispose();
            imgPhoto.Save(rDstImgPath, ImageFormat.Jpeg);
            imgPhoto.Dispose();
            imgWatermark.Dispose();
        }


    }
}
