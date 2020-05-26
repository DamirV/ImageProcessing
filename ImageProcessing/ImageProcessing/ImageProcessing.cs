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
    private int width;

    private int height;
    public Images(Bitmap first, Bitmap second, string str)
    {
        this.first = first;
        this.second = second;
        this.str = str;
        this.width = this.first.Width;
        this.height = this.first.Height;
    }

    public Images(Images images)
    {
        this.first = images.getFirstBitmap();
        this.second = images.getSecondBitmap();
        this.str = images.getString();
        this.width = images.getWidth();
        this.height = images.getHeight();
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

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }
}

namespace ImageProcessing
{
    public partial class ImageProcessing : MetroFramework.Forms.MetroForm
    {
        Bitmap image = null;
        Bitmap resultImage = null;
        private int width;
        private int height;
        int diameter = 3;
        Filter filter;
        Stack<Images> stack;
        private string filterName;

        public ImageProcessing()
        {
            InitializeComponent();
            stack = new Stack<Images>();
            this.StyleManager = metroStyleManager1;
            filterName = "Фильтр";
        }

        private int RunProcessing(string _filterName, Filter _filter)
        {
            if (image == null)
            {
                return 0;
            }

            stack.Push(new Images(image, resultImage, filterName));
            filterName = metroButton4.Text = _filterName;
            filter = _filter;
            backgroundWorker1.RunWorkerAsync(filter);

            return 1;
        }

        private int LoadTheory(string _filterName, string info, string path, int _diameter, bool loadMatrix = false)
        {
            metroButton2.Text = _filterName;
            metroLabel6.Text = info;
            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(path + _diameter + "\\source.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(path + _diameter + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(path + _diameter + "\\formula.gif"));

            if (loadMatrix)
            {
                pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\matrix.gif"));
            }
            else
            {
                pictureBox6.Image = null;
            }

            pictureBox3.Refresh();
            pictureBox4.Refresh();
            pictureBox5.Refresh();
            pictureBox6.Refresh();

            return 1;
        }
        private float ParseFloat(string str)
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

        private void MetroButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = " Image files|*.png; *.jpg; *.bmp| All Files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    stack.Clear();

                    image = new Bitmap(dialog.FileName);
                    resultImage = null;

                    pictureBox1.Image = image;
                    pictureBox1.Refresh();

                    pictureBox2.Image = null;
                    pictureBox2.Refresh();

                    width = image.Width;
                    height = image.Height;

                    metroLabel14.Text = "Ширина: " + width + "; Высота: " + height + ";";

                    stack.Push(new Images(image, resultImage, filterName));
                }
                catch
                {
                    MetroFramework.MetroMessageBox.Show(this, "Error", "Невозможно открыть выбранный файл",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Bitmap newImage = ((Filter)e.Argument).ProcessImage(image, backgroundWorker1);
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
            RunProcessing("Медианный фильтр", new Mediana(diameter));
        }

        
        private void ОбычнаяМаскаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Линейный фильтр", new LinearSmoothing(diameter, false));
        }

        private void РасширеннаяМаскаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Линейный фильтр", new LinearSmoothing(diameter, true));
        }

        private void ФильтрГауссаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                float weight = ParseFloat(metroTextBox1.Text);
                RunProcessing("Фильтр Гаусса", new GaussianFilter(diameter, weight));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ОбычнаяМатрицаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Маска Лапласа", new Laplass(false, false));
        }

        private void РасширеннаяМатрицаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Маска Лапласа", new Laplass(true, false));
        }

        private void ОбычнаяМатрицаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                float k = ParseFloat(metroTextBox3.Text);
                RunProcessing("Оператор Лапласа с восстановленным фоном", new Laplass(false, true, k));
            }
            catch
            {
                MessageBox.Show("Неверный коэффициент маски", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void РасширеннаяМатрицаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                float k = ParseFloat(metroTextBox3.Text);
                RunProcessing("Оператор Лапласа с восстановленным фоном", new Laplass(true, true, k));
            }
            catch
            {
                MessageBox.Show("Неверный коэффициент маски", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ФильтрацияСПодъемомВысокихЧастотToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                float k = ParseFloat(metroTextBox2.Text);
                float sigma = ParseFloat(metroTextBox1.Text);
                RunProcessing("Подъем высоких частот", new FrequencyIncrease(k, diameter, sigma));
            }
            catch
            {
                MessageBox.Show("Неверные коэффициенты", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ОператорCобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Оператор Собеля", new SobelFilter());
        }

        private void MetroButton5_Click(object sender, EventArgs e)
        {
            if (diameter != 7)
            {
                diameter += 2;
                metroLabel2.Text = "Размер ядра: " + Convert.ToString(diameter);
            }
        }

        private void MetroButton6_Click(object sender, EventArgs e)
        {
            if (diameter != 3)
            {
                diameter -= 2;
                metroLabel2.Text = "Размер ядра: " + Convert.ToString(diameter);
            }
        }

        private void MetroButton2_Click(object sender, EventArgs e)
        {
            metroContextMenu2.Show(metroButton2, 0, metroButton2.Height);
        }

        private void СольИПерецToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int saltPercent = (int)ParseFloat(metroTextBox4.Text);
            int pepperPercent = (int)ParseFloat(metroTextBox5.Text);
            RunProcessing("Соль и перец", new SaltPepper(width, height, saltPercent, pepperPercent));
        }

        private void MetroButton8_Click(object sender, EventArgs e)
        {
            if (image == null|| resultImage == null)
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

        private void MetroButton2_Click_1(object sender, EventArgs e)
        {
            metroContextMenu2.Show(metroButton2, 0, metroButton2.Height);
        }

        private void ToolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            LoadTheory("Медианный фильтр", "Размер ядра: " + diameter,@"Images\smoothing\median\", diameter);
        }

        private void ОбычнаяМаскаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LoadTheory("Линейный фильтр", "Размер ядра: " + diameter, @"Images\smoothing\linear\common\", diameter, true);
        }

        private void РасширеннаяМаскаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LoadTheory("Линейный фильтр расширенный", "Размер ядра: " + diameter, @"Images\smoothing\linear\extended\", diameter, true);
        }

        private void ToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            LoadTheory("Фильтр Гаусса", "Размер ядра: " + diameter + "; Сигма: 3", @"Images\smoothing\gauss\", diameter);
        }

        private void ToolStripMenuItem10_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа", "Размер ядра: " + 3, @"Images\sharpness\laplas\mask\common\", 3, true);
        }

        private void ToolStripMenuItem11_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа", "Размер ядра: " + 3, @"Images\sharpness\laplas\mask\extended\", 3, true);
        }

        private void ToolStripMenuItem14_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа с восстановленным фоном", "Размер ядра: " + 3 + "; k = 1", @"Images\sharpness\laplas\background\common\", 3, true);
        }

        private void ToolStripMenuItem15_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа с восстановленным фоном", "Размер ядра: " + 3 + "; k = 1", @"Images\sharpness\laplas\background\extended\", 3, true);
        }

        private void ToolStripMenuItem17_Click(object sender, EventArgs e)
        {
            LoadTheory("Подъем высоких частот", "Размер ядра: " + diameter + "; k = 3", @"Images\sharpness\frequency\", diameter);
        }

        private void ToolStripMenuItem18_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Собеля", "Размер ядра: " + 3, @"Images\sharpness\sobel\", 3, true);
        }
        private void НаращиваниеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LoadTheory("Дилатация", "Размер ядра: " + diameter, @"Images\morphology\Dilation\", diameter, true);
        }

        private void ЭрозияToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LoadTheory("Эрозия", "Размер ядра: " + diameter, @"Images\morphology\Erosion\", diameter, true);
        }

        private void ОткрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Размыкание", "Размер ядра: " + diameter, @"Images\morphology\Opening\", diameter, true);
        }

        private void ЗакрытиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Замыкание", "Размер ядра: " + diameter, @"Images\morphology\Closing\", diameter, true);
        }
        private void ImageProcessing_Rediameter(object sender, EventArgs e)
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
           
        }

        private void НаращиваниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Дилатация", new Dilation(diameter));
        }

        private void ЭрозияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Эрозия", new Erosion(diameter));
        }

        private void РазмыканиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Размыкание", new Opening(diameter));
        }

        private void ЗамыканиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Замыкание", new Closing(diameter));
        }

        private void СольToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int percent = (int)ParseFloat(metroTextBox4.Text);
                RunProcessing("Соль", new Salt(width, height, percent));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Соль",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ПерецToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int percent = (int)ParseFloat(metroTextBox5.Text);
                RunProcessing("Перец", new Pepper(width, height, percent));
             
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

                width = images.getWidth();
                height = images.getHeight();

                pictureBox1.Image = image;
                pictureBox2.Image = resultImage;

                metroLabel14.Text = "Ширина: " + width + "; Высота: " + height + ";";
                
                pictureBox1.Refresh();
                pictureBox2.Refresh();
            }
        }

        private void ЧерныеДырыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int percent = (int)ParseFloat(metroTextBox6.Text);
                RunProcessing("Черные дыры", new BlackHoles(diameter, width, height, percent));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Черные дыры",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void БелыеДырыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int percent = (int)ParseFloat(metroTextBox7.Text);
                RunProcessing("Белые дыры", new WhiteHoles(diameter, width, height, percent));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Черные дыры",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MetroButton9_Click_1(object sender, EventArgs e)
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

        private void ImageProcessing_Resize(object sender, EventArgs e)
        {
            alignment();
        }

        private void ВЧерноБелоеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RunProcessing("Черно-белое", new BlackAndWhite());
        }

        private void КраснаяКомпонентаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Красная компонента", new ComponentR());
        }

        private void ЗеленаяКомпонентаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Зеленая компонента", new ComponentG());
        }

        private void СиняяКомпонентаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Синяя компонента", new ComponentB());
        }

        private void ОператорСобеляДляЦветногоИзображенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Оператор Собеля для цветного изображения", new SobelFilterColored());
        }

        private void ГауссовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float sigma = ParseFloat(metroTextBox1.Text);
            RunProcessing("Гауссовский шум", new GaussianNoise(sigma));
        }

        private void среднееГеометрическоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Среднее геометрическое", new GeometricMean(diameter));
        }

        private void среднееГармоническоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Среднее гармоническое", new HarmonicMean(diameter));
        }

        private void среднееКонтргармоническоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int order = Convert.ToInt32(metroTextBox8.Text);
            RunProcessing("Среднее контргармоническое", new CounterHarmonicMean(diameter, order));
        }

        private void ФильтрМаксимумаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Фильтр максимума", new MaximumFilter(diameter));
        }

        private void ФильтрМинимумаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Фильтр минимума", new MinimumFilter(diameter));
        }

        private void ФильтрСреднейТочкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Фильтр срединной точки", new MidpointFilter(diameter));
        }
    }
}
