using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace WpfOpenCV
{
    public static class ImageHelper
    {
        public static byte[] GetImage(string path)
        {
            byte[] ret = null;
            var mem = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(path)); 
            ret = mem.ToArray();
            mem.Close();
            return ret;
        }

        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }


        public static BitmapImage BytesToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;

            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            } 
            return bmp;
        }
        public static byte[] BitmapImageToBytes(BitmapImage bmp)
        {
            byte[] byteArray = null;

            try
            {
                Stream sMarket = bmp.StreamSource;

                if (sMarket != null && sMarket.Length > 0)
                {
                    //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。 
                    sMarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(sMarket))
                    {
                        byteArray = br.ReadBytes((int)sMarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling 
            }

            return byteArray;
        }
        /// <summary>
        /// Convert Byte[] to a picture and Store it in file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }
         

        private static Size NewSize(int maxWidth, int maxHeight, int width, int height)
        {
            double w = 0.0;
            double h = 0.0;
            double sw = Convert.ToDouble(width);
            double sh = Convert.ToDouble(height); double mw = Convert.ToDouble(maxWidth);
            double mh = Convert.ToDouble(maxHeight); if (sw < mw && sh < mh)
            {
                w = sw;
                h = sh;
            }
            else if ((sw / sh) > (mw / mh))
            {
                w = maxWidth;
                h = (w * sh) / sw;
            }
            else
            {
                h = maxHeight;
                w = (h * sw) / sh;
            }
            return new Size(Convert.ToInt32(w), Convert.ToInt32(h));
        }
        /// <summary>
        /// 缩略图方法 调用方法 ImageHelper.CreateSmallImage(@"F:\bot\4.png",@"F:\bot\11111.jpg",100,100); 会替换已存的图片
        /// </summary>
        /// <param name="fileName">源路径</param>
        /// <param name="newFile">新图片路径</param>
        /// <param name="maxHeight">最大高度</param>
        /// <param name="maxWidth">最大宽度</param> 
        public static void CreateSmallImage(string fileName, string newFile, int maxHeight, int maxWidth)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(fileName);
            System.Drawing.Imaging.ImageFormat thisFormat = img.RawFormat;
            Size newSize = NewSize(maxWidth, maxHeight, img.Width, img.Height);
            Bitmap outBmp = new Bitmap(newSize.Width, newSize.Height);
            Graphics g = Graphics.FromImage(outBmp);
            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, new Rectangle(0, 0, newSize.Width, newSize.Height),
            0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时,设置压缩质量
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            //获得包含有关内置图像编码解码器的信息的ImageCodecInfo 对象.
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo jpegICI = null;
            for (int x = 0;
            x < arrayICI.Length;
            x++)
            {
                if (arrayICI[x].FormatDescription.Equals("JPEG"))
                {
                    jpegICI = arrayICI[x];
                    //设置JPEG编码
                    break;
                }
            }
            if (jpegICI != null)
            {
                outBmp.Save(newFile, jpegICI, encoderParams);
            }
            else
            {
                outBmp.Save(newFile,
                thisFormat);
            }
            img.Dispose();
            outBmp.Dispose();
        }
        /// <summary>
        /// 图片加水印文字
        /// </summary>
        /// <param name="oldpath">旧图片地址</param>
        /// <param name="text">水印文字</param>
        /// <param name="newpath">新图片地址</param>
        /// <param name="Alpha">透明度0-255 透明度越大越淡</param>
        /// <param name="fontsize">字体大小</param>
        public static void AddWaterText(string oldpath, string text, string newpath, int Alpha, int fontsize)
        {
            try
            {
                text = text + "版权所有";
                FileStream fs = new FileStream(oldpath, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                MemoryStream ms = new MemoryStream(bytes);

                System.Drawing.Image imgPhoto = System.Drawing.Image.FromStream(ms);
                int imgPhotoWidth = imgPhoto.Width;
                int imgPhotoHeight = imgPhoto.Height;

                Bitmap bmPhoto = new Bitmap(imgPhotoWidth, imgPhotoHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                bmPhoto.SetResolution(72, 72);
                Graphics gbmPhoto = Graphics.FromImage(bmPhoto);
                //gif背景色
                gbmPhoto.Clear(Color.FromName("white"));
                gbmPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                gbmPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gbmPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, imgPhotoWidth, imgPhotoHeight), 0, 0, imgPhotoWidth, imgPhotoHeight, GraphicsUnit.Pixel);
                System.Drawing.Font font = null;
                System.Drawing.SizeF crSize = new SizeF();
                font = new Font("宋体", fontsize, FontStyle.Bold);
                //测量指定区域
                crSize = gbmPhoto.MeasureString(text, font);
                float y = imgPhotoHeight - crSize.Height;
                float x = imgPhotoWidth - crSize.Width;
                System.Drawing.StringFormat StrFormat = new System.Drawing.StringFormat();
                StrFormat.Alignment = System.Drawing.StringAlignment.Center;

                //画两次制造透明效果
                System.Drawing.SolidBrush semiTransBrush2 = new System.Drawing.SolidBrush(Color.FromArgb(Alpha, 56, 56, 56));
                gbmPhoto.DrawString(text, font, semiTransBrush2, x + 1, y + 1);

                System.Drawing.SolidBrush semiTransBrush = new System.Drawing.SolidBrush(Color.FromArgb(Alpha, 176, 176, 176));
                gbmPhoto.DrawString(text, font, semiTransBrush, x, y);
                bmPhoto.Save(newpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                gbmPhoto.Dispose();
                imgPhoto.Dispose();
                bmPhoto.Dispose();
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 图片加水印文字
        /// </summary>
        /// <param name="oldpath">旧图片地址</param>
        /// <param name="text">水印文字</param>
        /// <param name="newpath">新图片地址</param>
        /// <param name="Alpha">透明度0-255 透明度越大越淡</param>
        /// <param name="fontsize">字体大小</param>
        public static void AddWaterImg(string oldpath, string newpath, string waterImgpath)
        {
            try
            {

                FileStream fs = new FileStream(oldpath, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                MemoryStream ms = new MemoryStream(bytes);

                System.Drawing.Image imgPhoto = System.Drawing.Image.FromStream(ms);
                int imgPhotoWidth = imgPhoto.Width;
                int imgPhotoHeight = imgPhoto.Height;

                Bitmap bmPhoto = new Bitmap(imgPhotoWidth, imgPhotoHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                bmPhoto.SetResolution(72, 72);
                Graphics gbmPhoto = Graphics.FromImage(bmPhoto);
                //gif背景色
                gbmPhoto.Clear(Color.FromName("white"));
                gbmPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                gbmPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gbmPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, imgPhotoWidth, imgPhotoHeight), 0, 0, imgPhotoWidth, imgPhotoHeight, GraphicsUnit.Pixel);


                FileStream fswaterImg = new FileStream(waterImgpath, FileMode.Open);
                BinaryReader brwaterImg = new BinaryReader(fswaterImg);
                byte[] byteswaterImg = brwaterImg.ReadBytes((int)fswaterImg.Length);
                brwaterImg.Close();
                fswaterImg.Close();
                MemoryStream mswaterImg = new MemoryStream(byteswaterImg);

                System.Drawing.Image imgPhotowaterImg = System.Drawing.Image.FromStream(mswaterImg);
                PointF pf = new PointF();
                pf.X = imgPhotoWidth - imgPhotowaterImg.Width;
                pf.Y = imgPhotoHeight - imgPhotowaterImg.Height;
                ;
                gbmPhoto.DrawImage(imgPhotowaterImg, pf);

                bmPhoto.Save(newpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                gbmPhoto.Dispose();
                imgPhoto.Dispose();
                bmPhoto.Dispose();
                imgPhotowaterImg.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        /// 图片裁剪，生成新图，保存在同一目录下,名字加_new，格式1.png  新图1_new.png
        /// </summary>
        /// <param name="picPath">要修改图片完整路径</param>
        /// <param name="x">修改起点x坐标</param>
        /// <param name="y">修改起点y坐标</param>
        /// <param name="width">新图宽度</param>
        /// <param name="height">新图高度</param>
        public static void CutPicture(string oldpath, string newpath)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(oldpath);
            double rate = 180.00 / 250.00;
            int width = (int)(rate * img.Height);
            int x = (img.Width - width) / 2;
            int y = 0;
            //System.Drawing.Image img = System.Drawing.Image.FromStream(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(oldpath)));
            //定义截取矩形
            System.Drawing.Rectangle cropArea = new System.Drawing.Rectangle(x, y, width, img.Height);
            //要截取的区域大小
            //加载图片
           
            //判断超出的位置否
            if ((img.Width < x + width))
            {
                img.Dispose();
                throw new  Exception("裁剪尺寸超出原有尺寸！");  
            }
            //定义Bitmap对象
            System.Drawing.Bitmap bmpImage = new System.Drawing.Bitmap(img);
            //进行裁剪
            System.Drawing.Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            //保存成新文件
            bmpCrop.Save(newpath);
            //释放对象
            img.Dispose();
            bmpCrop.Dispose();
        }
    }

}
