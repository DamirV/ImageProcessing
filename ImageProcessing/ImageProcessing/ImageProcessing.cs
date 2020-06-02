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
    private Bitmap sourceImage;
    private Bitmap resultImage;
    private string filterName;
    private int width;
    private int height;

    public Images(Bitmap sourceImage, Bitmap resultImage, string filterName)
    {
        this.sourceImage = sourceImage;
        this.resultImage = resultImage;
        this.filterName = filterName;
        this.width = this.sourceImage.Width;
        this.height = this.sourceImage.Height;
    }

    public Images(Images images)
    {
        this.sourceImage = images.getsourceImageBitmap();
        this.resultImage = images.getresultImageBitmap();
        this.filterName = images.getFilterName();
        this.width = images.getWidth();
        this.height = images.getHeight();
    }

    public Bitmap getsourceImageBitmap()
    {
        return sourceImage;
    }

    public Bitmap getresultImageBitmap()
    {
        return resultImage;
    }

    public string getFilterName()
    {
        return filterName;
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

namespace ResearchWork
{
    public partial class ImageProcessing : MetroFramework.Forms.MetroForm
    {
        private Bitmap sourceImage;
        private Bitmap resultImage;
        private int width;
        private int height;
        int diameter = 3;
        Stack<Images> stateStack;
        private string filterName;

        public ImageProcessing()
        {
            InitializeComponent();
            this.StyleManager = metroStyleManager1;

            this.stateStack = new Stack<Images>();
            this.filterName = "Фильтр";
            this.sourceImage = null;
            this.resultImage = null;
        }

        private int RunProcessing(string filterName, Filter currentFilter)
        {
            if (sourceImage == null)
            {
                return 0;
            }

            stateStack.Push(new Images(sourceImage, resultImage, this.filterName));
            this.filterName = showFiltersButton.Text = filterName;
            backgroundWorker1.RunWorkerAsync(currentFilter);

            return 1;
        }

        private int LoadTheory(string filterName, string info, string path, int diameter, bool loadMatrix = false)
        {
            showTheoryButton.Text = filterName;
            metroLabel6.Text = info;
            pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\sourceImage.jpg"));
            pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\result.jpg"));
            pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\formula.gif"));

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

        private void LoadImageButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = " Image files|*.png; *.jpg; *.bmp| All Files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    stateStack.Clear();

                    sourceImage = new Bitmap(dialog.FileName);
                    resultImage = null;

                    pictureBox1.Image = sourceImage;
                    pictureBox1.Refresh();

                    pictureBox2.Image = null;
                    pictureBox2.Refresh();

                    width = sourceImage.Width;
                    height = sourceImage.Height;

                    metroLabel14.Text = "Ширина: " + width + "; Высота: " + height + ";";

                    stateStack.Push(new Images(sourceImage, resultImage, filterName));
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
            Bitmap newImage = ((Filter)e.Argument).ProcessImage(sourceImage, backgroundWorker1);
            resultImage = null;

            if (backgroundWorker1.CancellationPending != true)
            {
                resultImage = newImage;
            }
        }

        private void CancelProcessing(object sender, EventArgs e)
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

        private void ShowFiltersButton_Click(object sender, EventArgs e)
        {
            metroContextMenu1.Show(showFiltersButton, 0, showFiltersButton.Height);
        }

        private void MedianaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Медианный фильтр", new Mediana(diameter));
        }

        private void LinearSmoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Линейный фильтр", new LinearSmoothing(diameter));
        }

        private void ExtendedLinearSmoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Линейный фильтр", new ExtendedLinearSmoothing(diameter));
        }

        private void GaussianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                double weight = Convert.ToDouble(metroTextBox1.Text);
                RunProcessing("Фильтр Гаусса", new Gaussian(diameter, weight));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double k = Convert.ToDouble(metroTextBox3.Text);
            RunProcessing("Маска Лапласа", new Laplace(k));
        }

        private void ExtendedLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double k = Convert.ToDouble(metroTextBox3.Text);
            RunProcessing("Маска Лапласа", new ExtendedLaplace(k));
        }

        private void RestoredLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                double k = Convert.ToDouble(metroTextBox3.Text);
                RunProcessing("Оператор Лапласа с восстановленным фоном", new RestoredLaplace(k));
            }
            catch
            {
                MessageBox.Show("Неверный коэффициент маски", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestoredExtendedLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                double k = Convert.ToDouble(metroTextBox3.Text);
                RunProcessing("Оператор Лапласа с восстановленным фоном", new RestoredExtendedLaplace(k));
            }
            catch
            {
                MessageBox.Show("Неверный коэффициент маски", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrequencyIncreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                double k = Convert.ToDouble(metroTextBox2.Text);
                double sigma = Convert.ToDouble(metroTextBox13.Text);
                RunProcessing("Подъем высоких частот", new FrequencyIncrease(k, diameter, sigma));
            }
            catch
            {
                MessageBox.Show("Неверные коэффициенты", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Оператор Собеля", new Sobel());
        }

        private void UpDiameterButton_Click(object sender, EventArgs e)
        {
            if (diameter != 7)
            {
                diameter += 2;
                metroLabel2.Text = "Размер ядра: " + Convert.ToString(diameter);
            }
        }

        private void DownDiameterButton_Click(object sender, EventArgs e)
        {
            if (diameter != 3)
            {
                diameter -= 2;
                metroLabel2.Text = "Размер ядра: " + Convert.ToString(diameter);
            }
        }

        private void SaltPepperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int saltPercent = (int)Convert.ToDouble(metroTextBox4.Text);
            int pepperPercent = (int)Convert.ToDouble(metroTextBox5.Text);
            RunProcessing("Соль и перец", new SaltPepper(saltPercent, pepperPercent));
        }

        private void MetroButton8_Click(object sender, EventArgs e)
        {
            if (sourceImage == null|| resultImage == null)
            {
                return;
            }

            try
            {
                Bitmap temp = resultImage;
                resultImage = sourceImage;
                sourceImage = temp;

                pictureBox1.Image = sourceImage;
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

        private void ShowTheoryButton_Click(object sender, EventArgs e)
        {
            metroContextMenu2.Show(showTheoryButton, 0, showTheoryButton.Height);
        }

        private void LoadTheoryMedianaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Медианный фильтр", "Размер ядра: " + diameter,@"Images\smoothing\median\", diameter);
        }

        private void LoadTheoryLinearSmoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Линейный фильтр", "Размер ядра: " + diameter, @"Images\smoothing\linear\common\", diameter, true);
        }

        private void LoadTheoryExtendedLinearSmoothingToolStripMenuItem_Click(object sender, EventArgs e)
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
            Alignment();
        }

        private void ImageProcessing_Activated(object sender, EventArgs e)
        {
            Alignment();
        }

        private void Alignment()
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
            Alignment();
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

        private void SaltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int saltPercent = (int)Convert.ToDouble(metroTextBox4.Text);
                RunProcessing("Соль", new Salt(saltPercent));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Соль",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void PepperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int pepperPercent = (int)Convert.ToDouble(metroTextBox5.Text);
                RunProcessing("Перец", new Pepper(pepperPercent));
             
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Перец",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (stateStack.Count != 0)
            {
                Images images = stateStack.Pop();

                sourceImage = images.getsourceImageBitmap();
                resultImage = images.getresultImageBitmap();
                showFiltersButton.Text = images.getFilterName();

                width = images.getWidth();
                height = images.getHeight();

                pictureBox1.Image = sourceImage;
                pictureBox2.Image = resultImage;

                metroLabel14.Text = "Ширина: " + width + "; Высота: " + height + ";";
                
                pictureBox1.Refresh();
                pictureBox2.Refresh();
            }
        }

        private void BlackHolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int percent = (int)Convert.ToDouble(metroTextBox6.Text);
                RunProcessing("Черные дыры", new BlackHoles(diameter, percent));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Черные дыры",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WhiteHolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int percent = (int)Convert.ToDouble(metroTextBox7.Text);
                RunProcessing("Белые дыры", new WhiteHoles(diameter, percent));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Error", "Неверный процент шума Черные дыры",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveImageButton_Click(object sender, EventArgs e)
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
            Alignment();
        }

        private void BlackAndWhiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Черно-белое", new BlackAndWhite());
        }

        private void RedComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Красная компонента", new RedComponent());
        }

        private void GreenComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Зеленая компонента", new GreenComponent());
        }

        private void BlueComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Синяя компонента", new BlueComponent());
        }

        private void DiZenzoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Метод Ди Зензо", new DiZenzo());
        }

        private void GaussianNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sigma = Convert.ToDouble(metroTextBox12.Text);
            int middle = Convert.ToInt32(metroTextBox11.Text);
            RunProcessing("Гауссовский шум", new GaussianNoise(sigma, middle));
        }

        private void GeometricMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Среднее геометрическое", new GeometricMean(diameter));
        }

        private void HarmonicMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Среднее гармоническое", new HarmonicMean(diameter));
        }

        private void CounterHarmonicMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int order = Convert.ToInt32(metroTextBox8.Text);
            RunProcessing("Среднее контргармоническое", new CounterHarmonicMean(diameter, order));
        }

        private void MaximumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Фильтр максимума", new Maximum(diameter));
        }

        private void MinimumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Фильтр минимума", new Minimum(diameter));
        }

        private void MidPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Фильтр срединной точки", new MidPoint(diameter));
        }

        private void UniformNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int a = Convert.ToInt32(metroTextBox9.Text);
            int b = Convert.ToInt32(metroTextBox10.Text);
            RunProcessing("Равномерный шум", new UniformNoise(a,b));
        }

        private void SobelMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Оператор Собеля для цветного изображения", new SobelMean());
        }

    }
}
