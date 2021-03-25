using Alturos.Yolo;
using Alturos.Yolo.Model;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstMachineLearningApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string ImageFileName = string.Empty;
        Image<Bgr,byte> ImageInput = null;
        private void donwloadPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                if (openFile.ShowDialog()==DialogResult.OK)
                {
                    ImageFileName = openFile.FileName;
                    pictureBox1.Image = Image.FromFile(ImageFileName);
                }
            }
        }

        private void findingThingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var configurationDetector = new ConfigurationDetector();
            var config = configurationDetector.Detect();
            using (var yoloWraper = new YoloWrapper(config))
            {
                var items = yoloWraper.Detect(ImagetoByte(pictureBox1.Image));
                if (items.Count()>0)
                {
                    foreach (YoloItem item in items)
                    {
                        Bitmap bitmap = new Bitmap(pictureBox1.Image);
                        Rectangle rectangle = new Rectangle(item.X,item.Y,item.Width,item.Height);
                        ImageInput = new Image<Bgr, byte>(bitmap);
                        ImageInput.Draw(rectangle, new Bgr(0, 255, 0),3);
                        ImageInput.Draw($"{item.Type} ({string.Format( "{0:0.0}",item.Confidence)})",
                            new Point(item.X-3,item.Y-3),Emgu.CV.CvEnum.FontFace.HersheyComplex,0.5,
                            new Bgr(Color.DarkBlue) );
                        pictureBox1.Image = ImageInput.Bitmap;
                    }
                }
            }
        }
        private byte[] ImagetoByte(Image image)
        {
            using (var stream =new MemoryStream())
            {
                image.Save(stream,ImageFormat.Jpeg);
                return stream.ToArray();
            }
           
        }
    }
}
