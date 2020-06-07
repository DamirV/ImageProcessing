using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroFramework;

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

        private int LoadTheory(string filterName, string info, string path, int diameter)
        {
            try
            {
                showTheoryButton.Text = filterName + "; " + info;
                pictureBox3.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\source.jpg"));
                pictureBox4.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\result.jpg"));
                pictureBox5.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\formula.jpg"));
                pictureBox6.Image = new Bitmap((Bitmap)Image.FromFile(path + diameter + "\\matrix.jpg"));

                pictureBox3.Refresh();
                pictureBox4.Refresh();
                pictureBox5.Refresh();
                pictureBox6.Refresh();
            }
            catch (Exception exception)
            {
                MetroMessageBox.Show(this, exception.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

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
            try
            {
                double k = Convert.ToDouble(metroTextBox3.Text);
                RunProcessing("Маска Лапласа", new Laplace(k));
            }
            catch

            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ExtendedLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                double k = Convert.ToDouble(metroTextBox3.Text);
                RunProcessing("Маска Лапласа", new ExtendedLaplace(k));
            }
            catch

            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            try
            {
                int saltPercent = (int)Convert.ToDouble(metroTextBox4.Text);
                int pepperPercent = (int)Convert.ToDouble(metroTextBox5.Text);
                RunProcessing("Соль и перец", new SaltPepper(saltPercent, pepperPercent));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SwapButton_Click(object sender, EventArgs e)
        {
            if (sourceImage == null || resultImage == null)
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
            LoadTheory("Медианный фильтр", "Размер ядра: " + diameter, @"Images\smoothing\median\", diameter);
        }

        private void LoadTheoryLinearSmoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Линейный фильтр", "Размер ядра: " + diameter, @"Images\smoothing\linear\common\", diameter);
        }

        private void LoadTheoryExtendedLinearSmoothingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Линейный фильтр расширенный", "Размер ядра: " + diameter, @"Images\smoothing\linear\extended\", diameter);
        }

        private void LoadTheoryGaussianStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Фильтр Гаусса", "Размер ядра: " + diameter + "; Сигма: 3", @"Images\smoothing\gauss\", diameter);
        }

        private void LoadTheoryLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа", "Размер ядра: " + 3 + "; Вес маски: = 1", @"Images\sharpness\laplace\notrestored\common\", 3);
        }

        private void LoadTheoryExtendedLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа", "Размер ядра: " + 3 + "; Вес маски = 1", @"Images\sharpness\laplace\notrestored\extended\", 3);
        }

        private void LoadTheoryRestoredLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа с восстановленным фоном", "Размер ядра: " + 3 + "; Вес маски = 1", @"Images\sharpness\laplace\restored\common\", 3);
        }

        private void LoadTheoryRestoredExtendedLaplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Лапласа с восстановленным фоном", "Размер ядра: " + 3 + "; Вес маски = 1", @"Images\sharpness\laplace\restored\extended\", 3);
        }

        private void LoadTheoryFrequencyIncreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Подъем высоких частот", "Размер ядра: " + diameter + "; k = 3; Сигма = 3", @"Images\sharpness\frequencyincrease\", diameter);
        }

        private void LoadTheorySobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Собеля", "Размер ядра: " + 3, @"Images\sharpness\sobel\", 3);
        }
        private void LoadTheoryDilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Дилатация", "Размер ядра: " + diameter, @"Images\morphology\dilation\", diameter);
        }

        private void LoadTheoryErosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Эрозия", "Размер ядра: " + diameter, @"Images\morphology\erosion\", diameter);
        }

        private void LoadTheoryOpeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Размыкание", "Размер ядра: " + diameter, @"Images\morphology\opening\", diameter);
        }

        private void LoadTheoryClosingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Замыкание", "Размер ядра: " + diameter, @"Images\morphology\closing\", diameter);
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

            showTheoryButton.Location = new Point(pictureBox3.Location.X, pictureBox3.Location.Y + pictureBox3.Height + 5);
            showTheoryButton.Width = pictureBox3.Width;

            pictureBox6.Location = new Point(showTheoryButton.Location.X, showTheoryButton.Location.Y + showTheoryButton.Height + 5);
            pictureBox6.Width = showTheoryButton.Width;
            pictureBox6.Height = metroTabPage3.Height - pictureBox6.Location.Y;

            pictureBox5.Location = new Point(pictureBox4.Location.X, pictureBox4.Location.Y + pictureBox4.Height + 5);
            pictureBox5.Width = pictureBox4.Width;
            pictureBox5.Height = metroTabPage3.Height - pictureBox5.Location.Y;
        }

        private void MetroTabControl1_Click(object sender, EventArgs e)
        {
            Alignment();
        }

        private void DilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Дилатация", new Dilation(diameter));
        }

        private void ErosionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Эрозия", new Erosion(diameter));
        }

        private void OpeningToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Размыкание", new Opening(diameter));
        }

        private void ClosingToolStripMenuItem_Click(object sender, EventArgs e)
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
            try
            {
                double sigma = Convert.ToDouble(metroTextBox12.Text);
                int middle = Convert.ToInt32(metroTextBox11.Text);
                RunProcessing("Гауссовский шум", new GaussianNoise(sigma, middle));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            try
            {
                double order = Convert.ToDouble(metroTextBox8.Text);
                RunProcessing("Среднее контргармоническое", new CounterHarmonicMean(diameter, order));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
            try
            {
                int a = Convert.ToInt32(metroTextBox9.Text);
                int b = Convert.ToInt32(metroTextBox10.Text);
                RunProcessing("Равномерный шум", new UniformNoise(a, b));
            }
            catch
            {
                MetroFramework.MetroMessageBox.Show(this, "Warning", "Неверные коэффициенты",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SobelMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunProcessing("Оператор Собеля для цветного изображения", new SobelMean());
        }

        private void LoadTheoryDiZenzoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Метод Ди Зензо", "Размер ядра: 3", @"Images\sharpness\dizenzo\", 3);
        }

        private void LoadTheorySobelMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Оператор Собеля для цветного изображения", "Размер ядра: 3", @"Images\sharpness\sobelmean\", 3);
        }

        private void LoadTheoryGeometricMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Среднее геометрическое", "Размер ядра: " + diameter, @"Images\smoothing\geometric\", diameter);
        }

        private void LoadTheoryHarmonicMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Среднее гармоническое", "Размер ядра: " + diameter, @"Images\averaging\harmonic\", diameter);
        }

        private void LoadTheoryCounterHarmonicMeanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Среднее контргармоническое", "Размер ядра: " + diameter + "; " + "Порядок фильтра: 1", @"Images\averaging\counterharmonic\", diameter);
        }

        private void LoadTheoryMaximumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Фильтр максимума", "Размер ядра: " + diameter, @"Images\averaging\maximum\", diameter);
        }

        private void LoadTheoryMinimumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Фильтр минимума", "Размер ядра: " + diameter, @"Images\averaging\minimum\", diameter);
        }

        private void LoadTheoryMidPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadTheory("Фильтр срединной точки", "Размер ядра: " + diameter, @"Images\averaging\midpoint\", diameter);
        }
    }
}