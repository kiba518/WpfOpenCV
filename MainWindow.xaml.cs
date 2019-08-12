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
            Mat mat = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.Unchanged);
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
        }

        //图片灰显
        private void BtnChange2_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.Grayscale))
            {
                var mem = src.ToMemoryStream();
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = mem;
                bmp.EndInit();
                imgOutput.Source = bmp; 
            }
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
        }
        //边缘化  
        private void BtnChange9_Click(object sender, RoutedEventArgs e)
        {
            using (var src = new Mat(@"..\..\Images\ocv02.jpg", ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (var dst = new Mat())//复制以后处理
                {
                    Cv2.Canny(src, dst, 0, 30, 3);
                    var mem = dst.ToMemoryStream();
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = mem;
                    bmp.EndInit();
                    imgOutput.Source = bmp;
                }
            }
        }
    }
}
