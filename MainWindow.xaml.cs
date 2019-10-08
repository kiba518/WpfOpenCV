using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.Extensions;
namespace WpfOpenCV
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //引用了OpenCvSharp BitmapImage和Bitmap对象会增加扩展方法ToMat,用于处理已经在内存中的图片
            //BitmapImage bmp = new BitmapImage();
            //Mat mat = bmp.ToMat();
        }
        private void SetSource(string url)
        { 
            using (var src = new Mat(url, ImreadModes.Unchanged))
            {
                var bmp = src.ToBitmap();//需要引用OpenCvSharp.Extensions，才能使用ToBitmap 
                var returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                imgOrignal.Source = returnSource;
            }
        }

        //正常
        private void BtnChange0_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.Unchanged))
            {
                var bmp = src.ToBitmap();//需要引用OpenCvSharp.Extensions，才能使用ToBitmap 
                var returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()); 
                imgOutput.Source = returnSource;
            }
        }

        //蓝红颜色互换
        private void BtnChange1_Click(object sender, RoutedEventArgs e)
        {
            Mat mat = new Mat(@"..\..\Images\ocv01.jpg", ImreadModes.Unchanged);
            //Mat mat = new Mat(new OpenCvSharp.Size(128, 128), MatType.CV_8U, Scalar.All(255));
            for (var y = 0; y < mat.Height; y++)
            {
                for (var x = 0; x < mat.Width; x++)
                {
                    Vec3b color = mat.Get<Vec3b>(y, x);
                    var temp = color.Item0;
                    color.Item0 = color.Item2; // B <- R
                    color.Item2 = temp;        // R <- B
                    mat.Set(y, x, color);
                }
            }
            var mem = mat.ToMemoryStream();
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = mem;
            bmp.EndInit();
            imgOutput.Source = bmp;
            mat.Dispose();/// 该方法在mat里被重写了，可以释放资源，可以放心调用
            SetSource(@"..\..\Images\ocv01.jpg");
        }

        //图片灰显
        private void BtnChange2_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv03.jpg", ImreadModes.Grayscale))
            {
                var mem = src.ToMemoryStream();
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = mem;
                bmp.EndInit();
                imgOutput.Source = bmp; 
            }
            SetSource(@"..\..\Images\ocv03.jpg");
        }

        //2倍缩小读取图像
        private void BtnChange3_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\images\ocv02.jpg", ImreadModes.ReducedGrayscale2))
            {
                var mem = src.ToMemoryStream();
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = mem;
                bmp.EndInit();
                imgOutput.Source = bmp;
                //using (var srcCopy = new Mat())
                //{
                //    src.CopyTo(srcCopy);
                //    Cv2.CvtColor(srcCopy, srcCopy, ColorConversionCodes.BayerGR2GRAY);//copy成灰色，二倍缩小
                //    var mem2 = srcCopy.ToMemoryStream();
                //    BitmapImage bmp2 = new BitmapImage();
                //    bmp2.BeginInit();
                //    bmp2.StreamSource = mem2;
                //    bmp2.EndInit();
                //    imgOutput.Source = bmp2;
                //}
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }

        //腐蚀
        private void BtnChange4_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                Cv2.Erode(src, src, new Mat());
                var mem = src.ToMemoryStream();
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = mem;
                bmp.EndInit();
                imgOutput.Source = bmp; 
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }

        //膨胀
        private void BtnChange5_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var dst = new Mat())//复制以后处理
                {
                    Cv2.Dilate(src, dst, new Mat());
                    var mem = dst.ToMemoryStream();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = mem;
                    bmp.EndInit();
                    imgOutput.Source = bmp;
                }
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }
  


        //反转
        private void BtnChange6_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var dst = new Mat())//复制以后处理
                {
                    Cv2.BitwiseNot(src, dst, new Mat());
                    var mem = dst.ToMemoryStream();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = mem;
                    bmp.EndInit();
                    imgOutput.Source = bmp;
                }
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }

        //变换顶点
        private void BtnChange7_Click(object sender, RoutedEventArgs e)
        { 
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var dst = new Mat())//复制以后处理
                {   
                    //设置原图变换顶点
                   List< Point2f> AffinePoints0  =new List<Point2f>() { new Point2f(100, 50), new Point2f(100, 390), new Point2f(600, 50) };
                    //设置目标图像变换顶点
                    List<Point2f> AffinePoints1 = new List<Point2f>() { new Point2f(200, 100), new Point2f(200, 330), new Point2f(500, 50) };
                    //计算变换矩阵
                    Mat Trans =Cv2.GetAffineTransform(AffinePoints0, AffinePoints1);
                    //矩阵仿射变换
                    Cv2.WarpAffine(src, dst, Trans,new OpenCvSharp.Size() { Height= src.Cols, Width= src.Rows }); 
                    var mem = dst.ToMemoryStream();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = mem;
                    bmp.EndInit();
                    imgOutput.Source = bmp;
                }
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }
        //     blur(edges, edges, Size(7, 7));//模糊化  
        //Canny(edges, edges, 0, 30, 3);//边缘化  
        //模糊
        private void BtnChange8_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var dst = new Mat())//复制以后处理
                {
                    Cv2.Blur(src, dst, new OpenCvSharp.Size(7, 7));
                    var mem = dst.ToMemoryStream();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = mem;
                    bmp.EndInit();
                    imgOutput.Source = bmp;
                }
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }
        //边缘化  
        private void BtnChange9_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var dst = new Mat())//复制以后处理
                {
                    Cv2.Canny(src, dst, 10, 400, 3);
                    var mem = dst.ToMemoryStream();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = mem;
                    bmp.EndInit();
                    imgOutput.Source = bmp;
                }
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }
        //亮度
        private void BtnChange10_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bmpSource = new BitmapImage(new Uri("pack://application:,,,/images/ocv02.jpg" )); 
            Mat mat = bmpSource.ToMat();
            for (var y = 0; y < mat.Height; y++)
            {
                for (var x = 0; x < mat.Width; x++)
                {
                    Vec3b color = mat.Get<Vec3b>(y, x);
                    int item0 = color.Item0;
                    int item1 = color.Item1;
                    int item2 = color.Item2;
                    #region  变暗
                    item0 -= 60;
                    item1 -= 60;
                    item2 -= 60;
                    if (item0 < 0)
                        item0 = 0;
                    if (item1 < 0)
                        item1 = 0;
                    if (item2 < 0)
                        item2 = 0;
                    #endregion
                    #region  变亮
                    //item0 += 80;
                    //item1 += 80;
                    //item2 += 80;
                    //if (item0 > 255)
                    //    item0 = 255;
                    //if (item1 > 255)
                    //    item1 = 255;
                    //if (item2 > 255)
                    //    item2 = 255;
                    #endregion

                    color.Item0 = (byte)item0;
                    color.Item1 = (byte)item1;
                    color.Item2 = (byte)item2;
                    mat.Set(y, x, color);
                }
            }
            var mem = mat.ToMemoryStream();
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = mem;
            bmp.EndInit();
            imgOutput.Source = bmp;
            mat.Dispose();/// 该方法在mat里被重写了，可以释放资源，可以放心调用
            SetSource(@"..\..\Images\ocv02.jpg");
        }
        //高斯模糊
        private void BtnChange11_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bmpSource = new BitmapImage(new Uri("pack://application:,,,/images/ocv02.jpg"));
            Mat src = bmpSource.ToMat(); 
            using (var dst = new Mat())
            {
                Cv2.GaussianBlur(src, dst,new OpenCvSharp.Size(5, 5), 1.5);
                var mem = dst.ToMemoryStream();
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = mem;
                bmp.EndInit();
                imgOutput.Source = bmp;
            } 
            src.Dispose();
            SetSource(@"..\..\Images\ocv02.jpg");
        }
        //美颜磨皮 双边滤波 
        private void BtnChange12_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var dst = new Mat())//复制以后处理
                {
                    Cv2.BilateralFilter(src, dst, 15, 35d, 35d);
                    var mem = dst.ToMemoryStream();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = mem;
                    bmp.EndInit();
                    imgOutput.Source = bmp;
                }
            }
            SetSource(@"..\..\Images\ocv02.jpg");
        }
       
    }
}
