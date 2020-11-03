using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
 
        private ImageClass img;
        public MainWindow()
        {

            img = new ImageClass();
            InitializeComponent();         
        }
  

        public void loadBackgroud_action(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri backgrounduri = null;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Images only. | *.jpg; *.jpeg; *.png; *.bmp; *.rgb";

                if (openFileDialog.ShowDialog() == true)
                {
                    backgrounduri = new Uri(openFileDialog.FileName);
                }
                if (backgrounduri != null)
                {
                    img.background = background.Source;
                    FileInfo file = new FileInfo(openFileDialog.FileName);
                    Bitmap bmpImp = new Bitmap(openFileDialog.FileName);
                    ///Validator 
                    background.Source = new BitmapImage(backgrounduri);
                    img.bmpBackground = bmpImp;
                    if (img.bmpSource != null)
                    {
                        img.setSize();
                        var validator = new ImageClassValidator();
                        //validator.Validate(img);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error");
            }
        }
        public void loadSource_action(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri sourceUri = null;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Images only. | *.jpg; *.jpeg; *.png; *.bmp; *.rgb";

                if (openFileDialog.ShowDialog() == true)
                {
                    sourceUri = new Uri(openFileDialog.FileName);
                }
                if (sourceUri != null)
                {
                    img.source = Source.Source;
                    FileInfo file = new FileInfo(openFileDialog.FileName);
                    Bitmap bmpImp = new Bitmap(openFileDialog.FileName);
                    ///Validator 
                    Source.Source = new BitmapImage(sourceUri);
                    img.bmpSource = bmpImp;
                    img.setSize();
                    var validator = new ImageClassValidator();
                   // validator.Validate(img);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error");
            }
        }


        public void Slider_ValueChangedSource(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = sender as Slider;
            var value = (float)slider.Value;
            this.Title = "Value: " + value.ToString("0") + "/" + slider.Maximum;
            img.alpha = Convert.ToByte(255 * value);
        }

        public void Button_Aply_Action(object sender, RoutedEventArgs e)
        {
            img.createRGB_Background();
            img.createRGB_source();
            //img.Brighten(10, false, 1);       
            img.Blend_C();
            var result_img = img.BitmapToImageSource(img.AfterImageFromRGB());
            Result.Source = result_img;

        }

    }
}
