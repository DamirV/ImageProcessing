using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

struct Images
{
    private Bitmap first;
    private Bitmap second;
    private string str;

    public Images(Bitmap first, Bitmap second, string str)
    {
        this.first = first;
        this.second = second;
        this.str = str;
    }

    public Images(Images images)
    {
        this.first = images.getFirstBitmap();
        this.second = images.getSecondBitmap();
        this.str = images.getString();
    }

    public Bitmap getFirstBitmap()
    {
        return first;
    }

    public Bitmap getSecondBitmap()
    {
        return second;
    }

    public string getString()
    {
        return str;
    }
}

namespace ImageProcessing
{
    public partial class ImageProcessing : MetroFramework.Forms.MetroForm
    {
        Bitmap image = null;
        Bitmap resultImage = null;
        int size = 3;
        Filters filter;
        Stack<Images> stack;
        private string filterName;

        public ImageProcessing()
        {
            InitializeComponent();
            stack = new Stack<Images>();
            this.StyleManager = metroStyleManager1;
            filterName = "Фильтр";
        }

        private void MetroButton3_Click(object sender, EventArgs e)
        {
            metroContextMenu3.Show(metroButton3, 0, metroButton3.Height);
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filters)e.Argument).ProcessImage(image, backgroundWorker1);
            resultImage = null;

            if (backgroundWorker1.CancellationPending != true)
            {
                resultImage = newImage;
            }
        }

        private void MetroButton1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            metroProgressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox2.Image = resultImage;
                pictureBox2.Refresh();
            }

            metroProgressBar1.Value = 0;
        }

        private void MetroButton4_Click(object sender, EventArgs e)
        {
            metroContextMenu1.Show(metroButton4, 0, metroButton4.Height);
        }

        private void МедианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (image == null)
            {
                return;
            }
            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Медианный фильтр";
            filter = new Mediana(size);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ОбычнаяМаскаToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (image == null)
            {
                return;
            }
            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Линейный фильтр";

            filter = new LinearSmoothing(size, false);
            backgroundWorker1.RunWorkerAsync(filter);

        }

        private void РасширеннаяМаскаToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (image == null)
            {
                return;
            }
            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Линейный фильтр";

            filter = new LinearSmoothing(size, true);
            backgroundWorker1.RunWorkerAsync(filter);

        }

        private void ФильтрГауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            metroButton4.Text = "Фильтр Гаусса";

            if (image == null)
            {
                return;
            }

            try
            {
                int kernelSize = size;
                float weight = parseFloat(metroTextBox1.Text);
                stack.Push(new Images(image, resultImage, filterName));
                filterName = metroButton4.Text = "Фильтр Гаусса";
                filter = new GaussianFilter(kernelSize, weight);
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private float parseFloat(string str)
        {
            if (str.Contains("/"))
            {
                string[] numbers = str.Split(new char[] { '/' });
                float up = float.Parse(numbers[0]);
                float down = float.Parse(numbers[1]);

                return up / down;
            }
            else
            {
                return float.Parse(str);
            }

        }

        private void ОбычнаяМатрицаToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (image == null)
            {
                return;
            }

            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Маска Лапласа";
            filter = new Laplass(false, false);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void РасширеннаяМатрицаToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (image == null)
            {
                return;
            }

            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Маска Лапласа";
            filter = new Laplass(true,false);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ОбычнаяМатрицаToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (image == null)
            {
                return;
            }

            float[,] kernel = new float[3, 3] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            float k = parseFloat(metroTextBox3.Text);


            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Оператор Лапласа с восстановленным фоном";
            filter = new Laplass(false, true, k);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void РасширеннаяМатрицаToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (image == null)
            {
                return;
            }

            try
            {
                float[,] kernel = new float[3, 3] { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };
                float k = parseFloat(metroTextBox2.Text);

                stack.Push(new Images(image, resultImage, filterName));

                filterName = metroButton4.Text = "Оператор Лапласа с восстановленным фоном";
                filter = new Laplass(true, true, k);
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch
            {
                MessageBox.Show("Неверный коэффициент маски", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ФильтрацияСПодъемомВысокихЧастотToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (image == null)
            {
                return;
            }

            try
            {
                float k = parseFloat(metroTextBox2.Text);
                int kernelSize = size;
                float sigma = parseFloat(metroTextBox1.Text);


                stack.Push(new Images(image, resultImage, filterName));
                filterName = metroButton4.Text = "Подъем высоких частот";
                filter = new FrequencyIncrease(k, kernelSize, sigma);
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch
            {
                MessageBox.Show("Неверные коэффициенты", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ОператорCобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Оператор Собеля";
            filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void MetroButton5_Click(object sender, EventArgs e)
        {
            if (size != 7)
            {
                size += 2;
                metroLabel2.Text = "Размер ядра: " + Convert.ToString(size);
            }
        }

        private void MetroButton6_Click(object sender, EventArgs e)
        {
            if (size != 3)
            {
                size -= 2;
                metroLabel2.Text = "Размер ядра: " + Convert.ToString(size);
            }
        }

        private void ImageProcessing_Load(object sender, EventArgs e)
        {

        }

        private void MetroButton2_Click(object sender, EventArgs e)
        {
            metroContextMenu2.Show(metroButton2, 0, metroButton2.Height);
        }

        private void СольИПерецToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }
            stack.Push(new Images(image, resultImage, filterName));
            int saltPercent = (int)parseFloat(metroTextBox4.Text);
            int pepperPercent = (int)parseFloat(metroTextBox5.Text);
            filterName = metroButton4.Text = "Соль и перец";
            Filters filter = new SaltPepper(image.Width, image.Height, saltPercent,pepperPercent);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void MetroButton8_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                Bitmap temp = resultImage;
                resultImage = image;
                image = temp;

                pictureBox1.Image = image;
                pictureBox2.Image = resultImage;

                pictureBox1.Refresh();
                pictureBox2.Refresh();

            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Невозможно поменять изображения",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void MetroButton9_Click(object sender, EventArgs e)
        {

        }

        private void MetroButton2_Click_1(object sender, EventArgs e)
        {
            metroContextMenu2.Show(metroButton2, 0, metroButton2.Height);
        }

        private void ToolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            metroButton2.Text = "Медианный фильтр";
            metroLabel6.Text = "Размер ядра: " + size;
            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\median\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\median\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\median\" + size + "\\formula.gif"));
            pictureBox6.Image = null;

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
        }

        private void ОбычнаяМаскаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Линейный фильтр";
            metroLabel6.Text = "Размер ядра: " + size;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\common\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\common\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\common\" + size + "\\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\common\" + size + "\\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void РасширеннаяМаскаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Линейный фильтр расширенный";
            metroLabel6.Text = "Размер ядра: " + size;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\extended\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\extended\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\extended\" + size + "\\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\linear\extended\" + size + "\\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Фильтр Гаусса";
            metroLabel6.Text = "Размер ядра: " + size + "; Сигма: 3";
            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\gauss\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\gauss\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\smoothing\gauss\" + size + "\\formula.gif"));
            pictureBox6.Image = null;

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ToolStripMenuItem10_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Оператор Лапласа";
            metroLabel6.Text = "Размер ядра: " + 3;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\common\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\common\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\common\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\common\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ToolStripMenuItem11_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Фильтр Лапласа";
            metroLabel6.Text = "Размер ядра: " + 3;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\extended\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\extended\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\extended\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\mask\extended\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ToolStripMenuItem14_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Фильтр Лапласа с восстановленным фоном";
            metroLabel6.Text = "Размер ядра: " + 3 + "; k = 1";

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\common\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\common\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\common\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\common\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ToolStripMenuItem15_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Фильтр Лапласа с восстановленным фоном";
            metroLabel6.Text = "Размер ядра: " + 3 + "; k = 1";

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\extended\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\extended\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\extended\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\laplas\background\extended\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ToolStripMenuItem17_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Подъем высоких частот";
            metroLabel6.Text = "Размер ядра: " + size + "; k = 3";

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\frequency\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\frequency\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\frequency\" + size + "\\formula.gif"));
            pictureBox6.Image = null;

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ToolStripMenuItem18_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Оператор Собеля";
            metroLabel6.Text = "Размер ядра: " + 3;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\sobel\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\sobel\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\sobel\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\sharpness\sobel\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ImageProcessing_Resize(object sender, EventArgs e)
        {
            alignment();
        }

        private void ImageProcessing_Activated(object sender, EventArgs e)
        {
            alignment();
        }

        private void alignment()
        {
            pictureBox1.Width = (int)(metroTabPage1.Width / 2.07);
            pictureBox1.Height = (int)(metroTabPage1.Height / 1.4);

            pictureBox2.Location = new Point((int)(metroTabPage1.Width / 1.94), 3);
            pictureBox2.Height = (int)(metroTabPage1.Height / 1.4);
            pictureBox2.Width = (int)(metroTabPage1.Width / 2.07);

            pictureBox3.Width = (int)(metroTabPage3.Width / 2.07);
            pictureBox3.Height = (int)(metroTabPage3.Height / 1.4);

            pictureBox4.Location = new Point((int)(metroTabPage3.Width / 1.94), 3);
            pictureBox4.Height = (int)(metroTabPage3.Height / 1.4);
            pictureBox4.Width = (int)(metroTabPage3.Width / 2.07);
        }

        private void MetroTabControl1_Click(object sender, EventArgs e)
        {
            alignment();
        }

        private void ВЧерноБелоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Черно-белое";
            Filters filter = new BlackAndWhite();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void MetroLabel5_Click(object sender, EventArgs e)
        {

        }

        private void MetroTextBox2_Click(object sender, EventArgs e)
        {

        }

        private void ОткрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = " Image files|*.png; *.jpg; *.bmp| All Files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    image = new Bitmap(dialog.FileName);
                    pictureBox1.Image = image;
                    pictureBox1.Refresh();
                    pictureBox2.Image = null;
                }
                catch
                {
                    MetroFramework.MetroMessageBox.Show(this, "Error", "Невозможно открыть выбранный файл",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void СохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (resultImage == null)
            {
                return;
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPeg Image|.jpg|Bitmap Image|.bmp|Gif Image|*.gif";
            dialog.Title = "Save an Image File";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                System.IO.FileStream fs = (System.IO.FileStream)dialog.OpenFile();

                switch (dialog.FilterIndex)
                {
                    case 1:
                        pictureBox2.Image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        pictureBox2.Image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        pictureBox2.Image.Save(fs,
                           System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }

                fs.Close();
            }
        }

        private void НаращиваниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }


            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Дилатация";
            filter = new Dilation(size);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ЭрозияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }


            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Эрозия";
            filter = new Erosion(size);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void РазмыканиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Размыкание";
            filter = new Opening(size);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void ЗамыканиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = "Замыкание";
            filter = new Closing(size);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void СольToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                int percent = (int)parseFloat(metroTextBox4.Text);
                stack.Push(new Images(image, resultImage, filterName));
                filterName = metroButton4.Text = "Соль";
                Filters filter = new Salt(image.Width, image.Height, percent);
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Соль",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ПерецToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                int percent = (int)parseFloat(metroTextBox5.Text);
                stack.Push(new Images(image, resultImage, filterName));
                filterName = metroButton4.Text = "Перец";
                Filters filter = new Pepper(image.Width, image.Height, percent);
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Перец",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void MetroButton7_Click(object sender, EventArgs e)
        {
            if (stack.Count != 0)
            {
                Images images = stack.Pop();

                image = images.getFirstBitmap();
                resultImage = images.getSecondBitmap();
                metroButton4.Text = images.getString();

                pictureBox1.Image = image;
                pictureBox2.Image = resultImage;

                pictureBox1.Refresh();
                pictureBox2.Refresh();
            }
        }

        private void НаращиваниеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Дилатация";
            metroLabel6.Text = "Размер ядра: " + size;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Dilation\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Dilation\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Dilation\" + size + "\\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Dilation\" + size + "\\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ЭрозияToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Эрозия";
            metroLabel6.Text = "Размер ядра: " + size;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Erosion\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Erosion\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Erosion\" + size + "\\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Erosion\" + size + "\\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ОткрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Размыкание";
            metroLabel6.Text = "Размер ядра: " + size;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Opening\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Opening\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Opening\" + size + "\\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Opening\" + size + "\\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ЗакрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            metroButton2.Text = "Замыкание";
            metroLabel6.Text = "Размер ядра: " + size;

            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Closing\" + size + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Closing\" + size + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Closing\" + size + "\\formula.gif"));
            pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(@"Images\morphology\Closing\" + size + "\\matrix.gif"));

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();
        }

        private void ЧерныеДырыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                int percent = (int)parseFloat(metroTextBox6.Text);
                stack.Push(new Images(image, resultImage, filterName));
                filterName = metroButton4.Text = "Черные дыры";
                Filters filter = new BlackHoles(size, image.Width, image.Height, percent);
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Черные дыры",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void БелыеДырыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image == null)
            {
                return;
            }

            try
            {
                int percent = (int)parseFloat(metroTextBox7.Text);
                stack.Push(new Images(image, resultImage, filterName));
                filterName = metroButton4.Text = "Белые дыры";
                Filters filter = new WhiteHoles(size, image.Width, image.Height, percent);
                backgroundWorker1.RunWorkerAsync(filter);
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Черные дыры",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
