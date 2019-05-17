using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;




namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        public System.Windows.Forms.Timer timer;
        public PictureBox pb, pb1;
        public Graphics g, g1, g2, g3;
        public Bitmap bmp1,bmp2,bmp_crop;
    
        public Space K;
        public Random rand;
        private DateTime ksp;

        public Stopwatch sw;
        public long TPM, ms0, ms1, ms2, ms3, ms4, ms5, ms6, ms7, ms8; int pi;
        public long[] ms = new long [11];
        DataTable tbl;
        int kt;

        public Form1()
        {
            InitializeComponent();
            rand = new Random();
            K = new Space(this);
            timer = new System.Windows.Forms.Timer();
            timer.Tick += new System.EventHandler(timer_Tick);
            timer.Interval = 1;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.KeyPreview = true;
            

            Height = 845;
            Width = 1245;

            pb = pictureBox1; pb.Width = 800; pb.Height = 800;
            pb1 = pictureBox2; 

            bmp1 = new Bitmap(pb.Width, pb.Height);
            bmp2 = new Bitmap(800, 800);
            bmp_crop = new Bitmap(pb1.Width, pb1.Height);
            g1 = Graphics.FromImage(bmp1);
            g2 = Graphics.FromImage(bmp2);
            g3 = Graphics.FromImage(bmp_crop);
            pi = 0;


            TPM = Stopwatch.Frequency/ (1000L * 1000L);

            sw = Stopwatch.StartNew();
            sw.Stop();

            textBox25.Text = DateTime.Today.ToShortDateString(); 


            //g = pb.CreateGraphics();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
            comboBox3.SelectedIndex = 3;
            comboBox4.SelectedIndex = 0;
            comboBox9.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBox13.SelectedIndex = 1;
            comboBox11.SelectedIndex = 2;
            comboBox14.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox10.SelectedIndex = 0;
            comboBox16.SelectedIndex = 1;
            comboBox6.SelectedIndex = 0;
            comboBox15.SelectedIndex = 0;       //тип солнца
            comboBox17.SelectedIndex = 0;       //reborn
            comboBox5.SelectedIndex = 0;       //m
            comboBox18.SelectedIndex = 0;       //m



            pb.MouseWheel += new MouseEventHandler(pb_MouseWheel);
            pb1.MouseWheel += new MouseEventHandler(pb1_MouseWheel);

            pb.Focus();
        }
        private void KosmosInit(int i)
        {
            int tmp; double tmp2;
            if (Double.TryParse(textBox13.Text, out tmp2) == false) return;
            if (Double.TryParse(textBox21.Text, out tmp2) == false) return;
            if (Int32.TryParse(textBox22.Text, out tmp) == false) return;
            if (Int32.TryParse(textBox23.Text, out tmp) == false) return;
            if (Double.TryParse(textBox24.Text, out tmp2) == false) return;
            if (Int32.TryParse(textBox26.Text, out tmp) == false) return;

            if (Int32.TryParse(textBox28.Text, out tmp) == false) return;
            if (Int32.TryParse(textBox32.Text, out tmp) == false) return;



            K.InitializeKosmos(i, this);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            
            //t.Stop();
            if (K.mag1speed != 0) { K.mag1 += K.mag1 * K.mag1speed * 0.002; textBox21.Text = K.mag1.ToString("N2"); }
            if (K.automag) textBox21.Text = K.mag1.ToString("N2");

            if (!K.PAUSE)
            {
                ms8 = sw.ElapsedTicks;
                for (int i = 0; i < K.steptimer; i++) K.Step(i);
                if (++K.cntprint >= K.cs) { K.cntprint = 0; ms7 = sw.ElapsedTicks; K.PrintStatusOnForm(); }
            }


            if (K.PAUSE && K.mag1speed != 0) K.ShowK();
            //t.Start();



            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Top = -15;
        }


        //start pause step save load
        private void button4_Click(object sender, EventArgs e)
        {
            if (K.Cnt == 0)  //start
            {
                if (timer.Enabled == false)
                {
                    sw = Stopwatch.StartNew();
                    KosmosInit(1);

                    timer.Enabled = true; K.PAUSE = false;
                    button8.Enabled = true; button9.Enabled = false; saveToXlsToolStripMenuItem.Enabled = false; loadFromXlsToolStripMenuItem.Enabled = false;
                    button4.Text = "end";
                }
            }
            else  //end
            {
                timer.Enabled = false; K.PAUSE = true; K.ShowK();
                button8.Enabled = false; button9.Enabled = true; saveToXlsToolStripMenuItem.Enabled = false; loadFromXlsToolStripMenuItem.Enabled = false;
                button4.Text = "start"; button8.Text = "pause";

                sw.Stop();
                for (int j = 0; j < K.nObj; j++) { K.p[j].newborn = true; K.p[j].M = 0; if (K.p[j].dop != null) { K.p[j].dop.sat = null; K.p[j].dop = null; } }
                for (int i = 0; i < 40; i++) for (int j = 0; j < 40; j++) K.doska[i, j].Clear();
                K.doskaM1.Clear();
                K.Cnt = 0;
            }
        }    //start
        public void button8_Click(object sender, EventArgs e)
        {
            if (K.Cnt>=0)
                if (K.PAUSE == false)
                {
                    K.PAUSE = true;
                    K.ShowK();
                }
                else
                {
                    KosmosInit(2);
                    K.p[K.fixnom].M = Double.Parse(textBox12.Text);
                    K.p[K.fixnom].r = Double.Parse(textBox17.Text);
                    K.PAUSE = false;
                }

        }    //pause
        private void button9_Click(object sender, EventArgs e)
        {
            //button4.Enabled = false; button6.Enabled = true; button8.Enabled = true; button9.Enabled = true;
            button8.Enabled = true; button9.Enabled = true; saveToXlsToolStripMenuItem.Enabled = true; loadFromXlsToolStripMenuItem.Enabled = false;
            if (K.Cnt == 0)
            {
                sw = Stopwatch.StartNew();
                sw.Stop();
                timer.Enabled = true;
                K.PAUSE = true;
                KosmosInit(1);
                button4.Text = "end";
            }
            K.Step(0);
            K.PrintStatusOnForm();
        }    //step
        private void loadFromXlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFromXLS();
        }
        private void loadFromDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFromDB();
        }
        private void loadSettingFromXlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSettingFromXLS();
        }
        private void loadSettingFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSettingFromDB();
        }
        private void saveToXlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToXLS();
        }
        private void saveSettingToXlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettingToXLS();
        }
        private void convertXlsToDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertPlanetXlsToDB();
        }
        private void convertSettingXlsToDbToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertSettingXlsToDB();
        }
        //-------------------------------------------------------------
        private void button7_Click(object sender, EventArgs e)
        {
            K.Cls(true);
        }    //cls
        private void button1_Click_1(object sender, EventArgs e)
        {
            K.vt = 2;
        }  //sputnik
        //-------------------------------------------------------------
        public void LoadFromXLS()
        {
            int i, j;

            //[по вертикали, по горизонтали]
            string path = Directory.GetCurrentDirectory() + "\\Test.xlsx";
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open XLS File";
            theDialog.Filter = "XLS files|*.xlsx";
            theDialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (theDialog.ShowDialog() == DialogResult.OK) path = theDialog.FileName;

            //try
            {

                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workBook;
                Excel.Worksheet workSheet;
                workBook = excelApp.Workbooks.Open(path, 0, true, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(2);


                LoadSetting(workSheet);

                workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);

                i = 0;
                do { i++; }
                while (workSheet.Cells[i + 3, 1].Value2 != null);
                textBox1.Text = i.ToString();

                PB_Init(i);



                sw = Stopwatch.StartNew();
                KosmosInit(1);

                K.Cnt = (long)workSheet.Cells[1, 1].Value;
                K.btk = (int)workSheet.Cells[1, 2].Value;
                K.ok = (int)workSheet.Cells[1, 3].Value;
                K.CntHour = (double)workSheet.Cells[1, 4].Value;


                i = 0;
                DateTime T0;
                double omega, lambda, w, a, eks, inakl;
                double M0, E, r, f, v, nu123, lambda_ist, cosfi, angv, fi, xt, yt, vxt, vyt, at, axt, ayt, n, JD;
                //double r2, r3, tgf2, cosf, sinfi, fi1;
                int kn;
                //int satof;


                do
                {
                    K.p[i].old = false;
                    //p[i].nom = (int)workSheet.Cells[i + 3, 1].Value;
                    if (workSheet.Cells[i + 3, 2].Value2 != null)
                    {
                        K.p[i].SetXY(workSheet.Cells[i + 3, 2].Value, workSheet.Cells[i + 3, 3].Value);
                        K.p[i].Vx = workSheet.Cells[i + 3, 4].Value;
                        K.p[i].Vy = workSheet.Cells[i + 3, 5].Value;
                        K.p[i].M = workSheet.Cells[i + 3, 6].Value;
                        K.p[i].r = workSheet.Cells[i + 3, 7].Value;
                        K.p[i].col = Color.FromArgb((int)workSheet.Cells[i + 3, 9].Value);
                        K.p[i].l = (int)workSheet.Cells[i + 3, 10].Value;
                        if (K.p[i].l > 5000) K.p[i].old = true;
                        K.p[i].k = 0;
                        K.p[i].ax = 0;
                        K.p[i].ay = 0;
                    }
                    else    //===========================================================================================================
                    {
                        if (i == 0)  //========== СОЛНЦЕ ==================-----поле satof пустое - солнце
                        {
                            K.p[i].SetXY(0, 0);
                            K.p[i].Vx = 0;
                            K.p[i].Vy = 0;
                            K.p[i].M = workSheet.Cells[i + 3, 31].Value / K.diffM;
                            //temp = workSheet.Cells[i + 3, 33].Value;
                            K.p[i].r = workSheet.Cells[i + 3, 33].Value * 1000 / K.diffR;
                            K.p[i].col = Color.Red;
                            K.p[i].l = 10000;
                            K.p[i].old = true;
                            K.p[i].k = 0;
                            K.p[i].ax = 0;
                            K.p[i].ay = 0;

                            K.p[i].dop = new Planet_dop();
                            K.p[i].dop.name = workSheet.Cells[i + 3, 11].Value;
                            K.p[i].dop.type = (int)workSheet.Cells[i + 3, 8].Value;
                            if (workSheet.Cells[i + 3, 9].Value != null) K.p[i].col = Color.FromArgb((int)workSheet.Cells[i + 3, 9].Value);

                        }
                        else
                        {


 
                            K.p[i].satof = (int)workSheet.Cells[i + 3, 13].Value;



                            nu123 = (workSheet.Cells[K.p[i].satof + 3, 31].Value != null) ? (K.G * workSheet.Cells[K.p[i].satof + 3, 31].Value) : (workSheet.Cells[K.p[i].satof + 3, 32].Value * 1000000000) +
                                (workSheet.Cells[i + 3, 31].Value != null) ? (K.G * workSheet.Cells[i + 3, 31].Value) : (workSheet.Cells[i + 3, 32].Value * 1000000000);






                            //==============================================================================================================
                            //Большая полуось
                            if (workSheet.Cells[i + 3, 19].Value2 != null) a = (double)workSheet.Cells[i + 3, 19].Value * 1000;
                            else a = workSheet.Cells[i + 3, 20].Value * K.ae; //м
                            n = Math.Pow(nu123, 1 / 2D) * Math.Pow(a, -3 / 2D);


                            //Эксцентриситет
                            eks = workSheet.Cells[i + 3, 21].Value; //rad


                            //Восходящий узел 
                            if (workSheet.Cells[i + 3, 15].Value != null) omega = workSheet.Cells[i + 3, 15].Value * (Math.PI / 180.0); else omega = rand.NextDouble() * 2 * Math.PI; //rad


                            //Наклонение
                            if (workSheet.Cells[i + 3, 16].Value != null) inakl = workSheet.Cells[i + 3, 16].Value * (Math.PI / 180.0); else inakl = rand.NextDouble() * Math.PI; //rad
                            kn = ((inakl <= Math.PI / 2) ? 1 : -1);


                            //Перицентр
                            if (workSheet.Cells[i + 3, 17].Value != null) w = omega + kn * workSheet.Cells[i + 3, 17].Value * (Math.PI / 180.0);    //rad
                            else
                            if (workSheet.Cells[i + 3, 18].Value != null) w = workSheet.Cells[i + 3, 18].Value * (Math.PI / 180.0);
                            else w = rand.NextDouble() * 2 * Math.PI; //rad


                            //Средняя аномалия
                            // 1/1/2000 - 2451545
                            // 4/1/2010 - 2455201   Epoch 2010 Jan. 4.0 TT . . . = JDT 2455200.5
                            // 1/1/1950 - 2433283   1950 January 0.9235 TT       JD 2433282.4235


                            //T0 = new DateTime(2000, 1, 1);
                            //JD = JDf(T0);
                            //T0 = new DateTime(2010, 1, 4);
                            //JD = JDf(T0);
                            //T0 = new DateTime(1950, 1, 1);
                            //JD = JDf(T0);


                            if (workSheet.Cells[i + 3, 22].Value != null) { lambda = workSheet.Cells[i + 3, 22].Value * (Math.PI / 180.0); M0 = lambda - w; } //rad
                            else
                                if (workSheet.Cells[i + 3, 23].Value != null) M0 = workSheet.Cells[i + 3, 23].Value * (Math.PI / 180.0);   //rad
                            else
                                    if (workSheet.Cells[i + 3, 26].Value != null)   //T0 rad
                            {
                                T0 = workSheet.Cells[i + 3, 26].Value;
                                JD = JDf(T0);
                                M0 = 0;
                                M0 = M0 + kn * n * 24 * 3600 * (JD - 2451545);
                            }
                            else { lambda = rand.NextDouble() * 2 * Math.PI; M0 = lambda - w; } //rad


                            if (workSheet.Cells[i + 3, 24].Value != null)   //JD
                            {
                                JD = workSheet.Cells[i + 3, 24].Value;
                                M0 = M0 + kn * n * 24 * 3600 * (JD - 2451545);
                            }
                            else
                                if (workSheet.Cells[i + 3, 25].Value != null)   //JD
                            {
                                T0 = workSheet.Cells[i + 3, 25].Value;
                                JD = JDf(T0);
                                M0 = M0 + kn * n * 24 * 3600 * (JD - 2451545);
                            }



                            //==============================================================================================================


                            K.p[i].dop = new Planet_dop();
                            K.p[i].dop.omega = omega;
                            K.p[i].dop.w = w;
                            K.p[i].dop.M0 = M0;
                            K.p[i].dop.inakl = inakl;
                            K.p[i].dop.a = a;
                            K.p[i].dop.eks = eks;
                            K.p[i].dop.name = workSheet.Cells[i + 3, 11].Value;
                            K.p[i].dop.type = (int)workSheet.Cells[i + 3, 8].Value;


                            E = M0; //rad
                            for (j = 0; j < 100; j++) E = M0 + eks * Math.Sin(E); //rad       //проверить для комет

                            r = a * (1 - eks * Math.Cos(E));  //м
                            //tgf2 = Math.Sqrt((1 + eks) / (1 - eks)) * Math.Tan(E / 2);
                            f = 2 * Math.Atan(Math.Sqrt((1 + eks) / (1 - eks)) * Math.Tan(E / 2)); //rad  //расчет истинной аномалии(f) через E
                            //cosf = (1 - tgf2 * tgf2) / (1 + tgf2 * tgf2);
                            //r2 = a * (1 - eks * eks) / (1 + eks * Math.Cos(f));  //м
                            //r3 = a * (1 - eks * eks) / (1 + eks * cosf);  //м

                            v = Math.Sqrt(nu123 * (2 / r - 1 / a));  //м с
                            //sinfi = Math.Sqrt(a*a*(1-eks*eks)/r/(2*a-r));
                            //fi1 = Math.Asin(sinfi);
                            cosfi = -eks * Math.Sin(E) / Math.Sqrt(1 - eks * eks * Math.Cos(E) * Math.Cos(E));
                            fi = Math.Acos(cosfi);

                            lambda_ist = w + kn * f;  //rad
                            angv = lambda_ist + kn * (Math.PI - fi);


                            xt = r * Math.Cos(lambda_ist);      //для спутников солнца
                            yt = r * Math.Sin(lambda_ist);      //для спутников солнца

                            vxt = v * Math.Cos(angv);
                            vyt = v * Math.Sin(angv);

                            at = (workSheet.Cells[K.p[i].satof + 3, 31].Value != null) ? (K.G * workSheet.Cells[K.p[i].satof + 3, 31].Value) : (workSheet.Cells[K.p[i].satof + 3, 32].Value * 1000000000) / r / r;
                            axt = -at * xt / r;                 //для спутников солнца
                            ayt = -at * yt / r;                 //для спутников солнца


                            //if (kn == 1) angv = Math.PI + lambda_ist - fi2; else angv = lambda_ist + fi2 - Math.PI;
                            //angv = lambda_ist - fi2;
                            //vt = Math.Sqrt(K.G * workSheet.Cells[K.p[i].satof + 3, 23].Value/r);


                            //теперь надо перевести координаты скорости ускорения в другие единицы измерения 
                            //расстояние -  метры   в точки     1 ае  - 3 точки  (diffR)
                            //время -       секунды в шаги      заранее устанавливать по умолчанию 1 реальный час - 1 шаг  (diffT)
                            //масса -       кг      в игровые единицы массы     1Е30 - 1 (diffM)
                            //пересчитывается гравитационная постоянная  6,67408(31)·10−11 м3·с−2·кг−1, или Н·м²·кг−2.

                            //G = 6.6740831313131313131313131E-11; //гравитациооная постоянная 6,67408(31)·10−11 м3·с−2·кг−1, или Н·м²·кг−2.



                            if (workSheet.Cells[i + 3, 31].Value != null) K.p[i].M = workSheet.Cells[i + 3, 31].Value / K.diffM;
                            else K.p[i].M = workSheet.Cells[i + 3, 32].Value * 1000000000 / K.G / K.diffM;
                            K.p[i].r = workSheet.Cells[i + 3, 33].Value * 1000 / K.diffR;


                            if (K.p[i].satof == 0)
                            {
                                K.p[i].SetXY(xt / K.diffR, yt / K.diffR);
                                K.p[i].Vx = vxt / K.diffR * K.diffT;
                                K.p[i].Vy = vyt / K.diffR * K.diffT;
                                K.p[i].ax = axt / K.diffR * K.diffT * K.diffT;
                                K.p[i].ay = ayt / K.diffR * K.diffT * K.diffT;
                            }
                            else
                            {
                                K.p[i].SetXY(xt / K.diffR + K.p[K.p[i].satof].x, yt / K.diffR + K.p[K.p[i].satof].y);
                                K.p[i].Vx = vxt / K.diffR * K.diffT + K.p[K.p[i].satof].Vx;
                                K.p[i].Vy = vyt / K.diffR * K.diffT + K.p[K.p[i].satof].Vy;
                                K.p[i].ax = axt / K.diffR * K.diffT * K.diffT + K.p[K.p[i].satof].ax;
                                K.p[i].ay = ayt / K.diffR * K.diffT * K.diffT + K.p[K.p[i].satof].ay;


                                if (K.p[i].r >= 0)
                                {
                                    if (K.p[K.p[i].satof].dop.sat == null) K.p[K.p[i].satof].dop.sat = new List<int>();
                                    K.p[K.p[i].satof].dop.sat.Add(i);
                                }
                            }


                            //vt = Math.Sqrt(K.Gg * workSheet.Cells[K.p[i].satof + 3, 23].Value / K.diffM / Math.Sqrt(K.p[i].x* K.p[i].x + K.p[i].y * K.p[i].y));

                            K.p[i].col = Color.Black;
                            K.p[i].l = 10000;
                            K.p[i].old = true;
                            K.p[i].k = 0;

                            if (workSheet.Cells[i + 3, 9].Value != null) K.p[i].col = Color.FromArgb((int)workSheet.Cells[i + 3, 9].Value);

                        }

                    }
                    i++;
                    PB_Step();
                    K.ShowK();

                }
                while (workSheet.Cells[i + 3, 1].Value2 != null);
                workBook.Close();
                pb.Focus();
            }
            //catch (Exception e1)
            {
                //MessageBox.Show("Exception: " + e1.Message);
            }
            //finally
            {
                //MessageBox.Show("Executing finally block.");
            }

            GravRecalc();

            button8.Enabled = true; button9.Enabled = true; saveToXlsToolStripMenuItem.Enabled = true; loadFromXlsToolStripMenuItem.Enabled = false;
            //sw.Stop();
            timer.Enabled = true;
            K.PAUSE = true;
            button4.Text = "end";
            K.ShowK();
        }
        public void LoadFromDB()
        {
            int i, j;
            ft = 2;


            using (SqlConnection connection = new SqlConnection(@"Data Source =.\SQLEXPRESS; Initial Catalog = ssystem; Integrated Security=false; User ID=sa; Password=dima; "))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.ToString());
                }


                SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Planets", connection);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                i = Convert.ToInt32(dr.GetValue(0).ToString()) - 1;
                textBox1.Text = i.ToString();
                dr.Close();

                PB_Init(i);

                //LoadSettingFromDB();

                sw = Stopwatch.StartNew();
                KosmosInit(1);


                cmd.CommandText = "SELECT * FROM Counters";
                dr = cmd.ExecuteReader();
                dr.Read();

                K.Cnt = (long)Convert.ToInt32(dr.GetValue(0).ToString());
                K.btk = Convert.ToInt32(dr.GetValue(1).ToString());
                K.ok = Convert.ToInt32(dr.GetValue(2).ToString());
                K.CntHour = Convert.ToDouble(dr.GetValue(3).ToString());
                dr.Close();


                tbl = new DataTable();
                cmd.CommandText = "SELECT * FROM Planets";
                dr = cmd.ExecuteReader();
                tbl.Load(dr);

                i = 0;
                DateTime T0;
                double omega, lambda, w, a, eks, inakl;
                double M0, E, r, f, v, nu, lambda_ist, cosfi, angv, fi, xt, yt, vxt, vyt, at, axt, ayt, n, JD;
                double massa, GM, rad, GMsatof;
                int kn;




                for (i = 0; i < tbl.Rows.Count - 1; i++)
                {
                    K.p[i].old = false;
                    if (t("x", i) != null)
                    {

                        K.p[i].SetXY(t("x", i), t("y", i));
                        K.p[i].Vx = t("vx", i);
                        K.p[i].Vy = t("vy", i);
                        K.p[i].M = t("mp", i);
                        K.p[i].r = t("radp", i);
                        K.p[i].col = Color.FromArgb(t("col", i));
                        K.p[i].l = t("l", i);
                        if (K.p[i].l > 5000) K.p[i].old = true;
                        K.p[i].k = 0;
                        K.p[i].ax = 0;
                        K.p[i].ay = 0;

                    }
                    else    //===========================================================================================================
                    {

                        if (i == 0)  //========== СОЛНЦЕ ==================-----поле satof пустое - солнце
                        {
                            K.p[i].SetXY(0, 0);
                            K.p[i].Vx = 0;
                            K.p[i].Vy = 0;
                            K.p[i].M = t("mas", i) / K.diffM;
                            K.p[i].r = t("radkm", i) * 1000 / K.diffR;
                            K.p[i].col = Color.Red;
                            K.p[i].l = 10000;
                            K.p[i].old = true;
                            K.p[i].k = 0;
                            K.p[i].ax = 0;
                            K.p[i].ay = 0;
                            K.p[i].dop = new Planet_dop();
                            K.p[i].dop.name = t("name", i);
                            K.p[i].dop.type = t("type", i);
                            if (t("col", i) != null) K.p[i].col = Color.FromArgb(t("col", i));
                        }
                        else
                        {


                            //радиус, масса, satof, nu ============================================================================================



                            if (t("radkm", i) != null) rad = t("radkm", i) * 1000;
                            else rad = t("diamkm", i) * 500;




                            if (t("mas", i) != null)
                            { massa = t("mas", i); GM = K.G * massa; }
                            else
                                if (t("gm", i) != null)
                            { GM = t("gm", i) * 1000000000; massa = GM / K.G; }
                            else
                            {
                                v = 4D / 3D * Math.PI * Math.Pow(rad, 3);  //м3
                                massa = 2 * v * 1000;       //г/см3            2 г/см3 - лед с пылью
                                GM = K.G * massa;
                            }

                            K.p[i].satof = t("satof", i);


                            GMsatof = K.p[K.p[i].satof].Gm * K.diffR * K.diffR * K.diffR / K.diffT / K.diffT;

                            nu = GMsatof + GM;


                            //Орбитальные элементы================================================================================================
                            //1Большая полуось a
                            if (t("akm", i) != null) a = t("akm", i) * 1000;
                            else a = t("aae", i) * K.ae; //м
                            n = Math.Pow(nu, 1 / 2D) * Math.Pow(a, -3 / 2D);


                            //2Эксцентриситет eks
                            eks = t("e", i); //rad

                            //3Восходящий узел  omega
                            if (t("om", i) != null) omega = t("om", i) * (Math.PI / 180.0); else omega = rand.NextDouble() * 2 * Math.PI; //rad


                            //4Наклонение inakl
                            if (t("i", i) != null) inakl = t("i", i) * (Math.PI / 180.0); else inakl = rand.NextDouble() * Math.PI; //rad
                            kn = ((inakl <= Math.PI / 2) ? 1 : -1);


                            //5Перицентр w
                            if (t("w", i) != null) w = omega + kn * t("w", i) * (Math.PI / 180.0);    //rad
                            else
                            if (t("w1", i) != null) w = t("w1", i) * (Math.PI / 180.0);
                            else w = rand.NextDouble() * 2 * Math.PI; //rad

                            //6Средняя аномалия M0
                            if (t("lambda", i) != null) { lambda = t("lambda", i) * (Math.PI / 180.0); M0 = lambda - w; } //rad
                            else
                                if (t("m0", i) != null) M0 = t("m0", i) * (Math.PI / 180.0);   //rad
                            else
                                    if (t("t02", i) != null)   //T0 rad
                            {
                                T0 = t("t02", i);
                                JD = JDf(T0);
                                M0 = 0;
                                M0 = M0 + kn * n * 24 * 3600 * (JD - 2451545);
                            }
                            else { lambda = rand.NextDouble() * 2 * Math.PI; M0 = lambda - w; } //rad


                            if (t("jd", i) != null)   //JD
                            {
                                JD = t("jd", i);
                                M0 = M0 + kn * n * 24 * 3600 * (JD - 2451545);
                            }
                            else
                                if (t("t01", i) != null)   //JD
                            {
                                T0 = t("t01", i);
                                JD = JDf(T0);
                                M0 = M0 + kn * n * 24 * 3600 * (JD - 2451545);
                            }

                            //==============================================================================================================

                            K.p[i].dop = new Planet_dop();
                            K.p[i].dop.omega = omega;
                            K.p[i].dop.w = w;
                            K.p[i].dop.M0 = M0;
                            K.p[i].dop.inakl = inakl;
                            K.p[i].dop.a = a;
                            K.p[i].dop.eks = eks;
                            K.p[i].dop.name = t("name", i);
                            K.p[i].dop.type = t("type", i);


                            E = M0; //rad
                            for (j = 0; j < 100; j++) E = M0 + eks * Math.Sin(E); //rad       //проверить для комет

                            r = a * (1 - eks * Math.Cos(E));  //м
                            //tgf2 = Math.Sqrt((1 + eks) / (1 - eks)) * Math.Tan(E / 2);
                            f = 2 * Math.Atan(Math.Sqrt((1 + eks) / (1 - eks)) * Math.Tan(E / 2)); //rad  //расчет истинной аномалии(f) через E
                            //cosf = (1 - tgf2 * tgf2) / (1 + tgf2 * tgf2);
                            //r2 = a * (1 - eks * eks) / (1 + eks * Math.Cos(f));  //м
                            //r3 = a * (1 - eks * eks) / (1 + eks * cosf);  //м

                            v = Math.Sqrt(nu * (2 / r - 1 / a));  //м с
                            //sinfi = Math.Sqrt(a*a*(1-eks*eks)/r/(2*a-r));
                            //fi1 = Math.Asin(sinfi);
                            cosfi = -eks * Math.Sin(E) / Math.Sqrt(1 - eks * eks * Math.Cos(E) * Math.Cos(E));
                            fi = Math.Acos(cosfi);

                            lambda_ist = w + kn * f;  //rad
                            angv = lambda_ist + kn * (Math.PI - fi);


                            xt = r * Math.Cos(lambda_ist);      //для спутников солнца
                            yt = r * Math.Sin(lambda_ist);      //для спутников солнца

                            vxt = v * Math.Cos(angv);
                            vyt = v * Math.Sin(angv);

                            at = GMsatof / r / r;
                            axt = -at * xt / r;                 //для спутников солнца
                            ayt = -at * yt / r;                 //для спутников солнца



                            K.p[i].M = massa / K.diffM;

                            K.p[i].r = rad / K.diffR;


                            if (K.p[i].satof == 0)
                            {
                                K.p[i].SetXY(xt / K.diffR, yt / K.diffR);
                                K.p[i].Vx = vxt / K.diffR * K.diffT;
                                K.p[i].Vy = vyt / K.diffR * K.diffT;
                                K.p[i].ax = axt / K.diffR * K.diffT * K.diffT;
                                K.p[i].ay = ayt / K.diffR * K.diffT * K.diffT;
                            }
                            else
                            {
                                K.p[i].SetXY(xt / K.diffR + K.p[K.p[i].satof].x, yt / K.diffR + K.p[K.p[i].satof].y);
                                K.p[i].Vx = vxt / K.diffR * K.diffT + K.p[K.p[i].satof].Vx;
                                K.p[i].Vy = vyt / K.diffR * K.diffT + K.p[K.p[i].satof].Vy;
                                K.p[i].ax = axt / K.diffR * K.diffT * K.diffT + K.p[K.p[i].satof].ax;
                                K.p[i].ay = ayt / K.diffR * K.diffT * K.diffT + K.p[K.p[i].satof].ay;


                                if (K.p[i].r >= 0)
                                {
                                    if (K.p[K.p[i].satof].dop.sat == null) K.p[K.p[i].satof].dop.sat = new List<int>();
                                    K.p[K.p[i].satof].dop.sat.Add(i);
                                }
                            }


                            K.p[i].col = Color.Black;
                            K.p[i].l = 10000;
                            K.p[i].old = true;
                            K.p[i].k = 0;

                            if (t("col", i) != null) K.p[i].col = Color.FromArgb(t("col", i));
                        }

                    }

                    PB_Step();

                }
                GravRecalc();
                dr.Close();
            }



            button8.Enabled = true; button9.Enabled = true; saveToXlsToolStripMenuItem.Enabled = true; loadFromXlsToolStripMenuItem.Enabled = false;
            //sw.Stop();
            timer.Enabled = true;
            K.PAUSE = true;
            button4.Text = "end";
            K.ShowK();
        }
        public void LoadSettingFromXLS()
        {
            string path = Directory.GetCurrentDirectory() + "\\Settings.xlsx";
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;
            workBook = excelApp.Workbooks.Open(path, 0, true, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);
            LoadSetting(workSheet);
            workBook.Close();
        }
        public void LoadSettingFromDB()
        {
            string t, n, d;
            Control ctrl;

            using (SqlConnection connection = new SqlConnection(@"Data Source =.\SQLEXPRESS; Initial Catalog = ssystem; Integrated Security=false; User ID=sa; Password=dima; "))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    try
                    {
                        connection.Open();

                        SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Settings", connection);
                        SqlDataReader dr = cmd.ExecuteReader();
                        dr.Read();
                        PB_Init(Convert.ToInt32(dr.GetValue(0).ToString()));
                        dr.Close();

                        cmd.CommandText = "SELECT * FROM Settings";
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.GetValue(3).ToString() != "")
                            {
                                t = dr.GetValue(1).ToString();
                                n = dr.GetValue(2).ToString();
                                d = dr.GetValue(3).ToString();
                                ctrl = this.Controls.Find(n, true).FirstOrDefault();

                                switch (t)
                                {
                                    case "System.Windows.Forms.TextBox": (ctrl as TextBox).Text = d; break;
                                    case "System.Windows.Forms.ComboBox": (ctrl as ComboBox).SelectedIndex = Convert.ToInt32(d); break;
                                    case "System.Windows.Forms.CheckBox": (ctrl as CheckBox).Checked = Convert.ToBoolean(d); break;
                                    case "System.Windows.Forms.NumericUpDown": (ctrl as NumericUpDown).Value = (decimal)Convert.ToDouble(d); break;
                                }
                            }
                            PB_Step();
                        }
                        GravRecalc();
                        dr.Close();
                        connection.Close();
                    }
                    catch (Exception e1) { MessageBox.Show(e1.Message); }
                }
            }
        }
        public void SaveToXLS()
        {
            string path;
            path = Directory.GetCurrentDirectory() + "\\Test.xlsx";

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;
            workBook = excelApp.Workbooks.Add();
            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(2);

            //[по вертикали, по горизонтали]

            SaveSetting(workSheet);

            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);

            workSheet.Cells[1, 1] = K.Cnt.ToString();
            workSheet.Cells[1, 2] = K.btk.ToString();
            workSheet.Cells[1, 3] = K.ok.ToString();
            workSheet.Cells[1, 4] = K.CntHour.ToString();

            workSheet.Cells[2, 1] = "nom";
            workSheet.Cells[2, 2] = "x";
            workSheet.Cells[2, 3] = "y";
            workSheet.Cells[2, 4] = "Vx";
            workSheet.Cells[2, 5] = "Vy";
            workSheet.Cells[2, 6] = "mass";
            workSheet.Cells[2, 7] = "r";
            workSheet.Cells[2, 8] = "type";
            workSheet.Cells[2, 9] = "Color";
            workSheet.Cells[2, 10] = "l";
            workSheet.Cells[2, 11] = "-";
            workSheet.Cells[2, 12] = "name";
            workSheet.Cells[2, 13] = "satof";
            workSheet.Cells[2, 14] = "-";
            workSheet.Cells[2, 15] = "\u2126"; workSheet.Cells[2, 15].AddComment("долгота восходящего узла");
            workSheet.Cells[2, 16] = "i"; workSheet.Cells[2, 16].AddComment("наклонение");
            workSheet.Cells[2, 17] = "\u03c9"; workSheet.Cells[2, 17].AddComment("аргумент перицентра");
            workSheet.Cells[2, 18] = "\u03d6"; workSheet.Cells[2, 18].AddComment("долгота перицентра");
            workSheet.Cells[2, 19] = "akm"; workSheet.Cells[2, 19].AddComment("большая полуось");
            workSheet.Cells[2, 20] = "aae";
            workSheet.Cells[2, 21] = "e"; workSheet.Cells[2, 21].AddComment("эксцентриситет");
            workSheet.Cells[2, 22] = "\u03BB"; workSheet.Cells[2, 22].AddComment("средняя долгота");
            workSheet.Cells[2, 23] = "M"; workSheet.Cells[2, 23].AddComment("средняя аномалия");
            workSheet.Cells[2, 24] = "JD"; workSheet.Cells[2, 24].AddComment("эпоха");
            workSheet.Cells[2, 25] = "JD"; workSheet.Cells[2, 25].AddComment("эпоха");
            workSheet.Cells[2, 26] = "T0"; workSheet.Cells[2, 26].AddComment("время перигелия");
            workSheet.Cells[2, 27] = "T0next";
            workSheet.Cells[2, 28] = "-";
            workSheet.Cells[2, 29] = "-";
            workSheet.Cells[2, 30] = "-";
            workSheet.Cells[2, 31] = "masskg";
            workSheet.Cells[2, 32] = "GMkm3sec2";
            workSheet.Cells[2, 33] = "radiuskm";


            PB_Init(K.nObj);

            for (int i = 0; i < K.nObj; i++)
            {
                workSheet.Cells[i + 3, 1] = i;
                workSheet.Cells[i + 3, 2] = K.p[i].x;
                workSheet.Cells[i + 3, 3] = K.p[i].y;
                workSheet.Cells[i + 3, 4] = K.p[i].Vx;
                workSheet.Cells[i + 3, 5] = K.p[i].Vy;
                workSheet.Cells[i + 3, 6] = K.p[i].M;
                workSheet.Cells[i + 3, 7] = K.p[i].r;
                workSheet.Cells[i + 3, 9] = K.p[i].col.ToArgb();
                workSheet.Cells[i + 3, 10] = K.p[i].l;
                PB_Step();
            }

            excelApp.Application.ActiveWorkbook.SaveAs(path, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            workBook.Close();

        }
        public void SaveSettingToXLS()
        {
            string path;
            path = Directory.GetCurrentDirectory() + "\\Settings.xlsx";

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;
            workBook = excelApp.Workbooks.Add();
            workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);
            SaveSetting(workSheet);

            excelApp.Application.ActiveWorkbook.SaveAs(path, Type.Missing,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange,
            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            workBook.Close();
        }
        public void ConvertSettingXlsToDB()
        {
            int i;
            string path = Directory.GetCurrentDirectory() + "\\Settings.xlsx";
            Excel.Application excelApp;
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;


            using (SqlConnection connection = new SqlConnection(@"Data Source =.\SQLEXPRESS; Initial Catalog = ssystem; Integrated Security=false; User ID=sa; Password=dima; "))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    try
                    {
                        excelApp = new Excel.Application();
                        workBook = excelApp.Workbooks.Open(path, 0, true, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                        workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);



                        i = workSheet.UsedRange.Rows.Count;
                        PB_Init(i);


                        connection.Open();

                        command.CommandText =
                        "IF OBJECT_ID(N'dbo.Settings', N'U') IS NOT NULL " +
                        "BEGIN " +
                            "DROP TABLE Settings " +
                        "END " +
                        "CREATE TABLE Settings" +
                        "(" +
                        "Id INT PRIMARY KEY IDENTITY," +
                        "ControlType NVARCHAR(50)," +
                        "ControlName NVARCHAR(50)," +
                        "ControlData NVARCHAR(50)" +
                        ")";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT into Settings (ControlType, ControlName, ControlData) VALUES (@type, @name, @data)";
                        command.Parameters.AddWithValue("@type", "");
                        command.Parameters.AddWithValue("@name", "");
                        command.Parameters.AddWithValue("@data", "");

                        i = 0;
                        do
                        {
                            i++;
                            command.Parameters["@type"].Value = workSheet.Cells[i, 1].Value.ToString(); ;
                            command.Parameters["@name"].Value = (workSheet.Cells[i, 2].Value2 != null) ? workSheet.Cells[i, 2].Value.ToString() : "";
                            command.Parameters["@data"].Value = (workSheet.Cells[i, 3].Value2 != null) ? workSheet.Cells[i, 3].Value.ToString() : "";
                            command.ExecuteNonQuery();
                            PB_Step();
                        }
                        while (workSheet.Cells[i + 1, 1].Value2 != null);

                        workBook.Close();
                        connection.Close();
                    }
                    catch (Exception e1) { MessageBox.Show(e1.Message); }
                }
            }
        }
        public void ConvertPlanetXlsToDB()
        {
            int i,j,k;
            string h;
            Excel.Application excelApp;
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;

            string path = Directory.GetCurrentDirectory() + "\\Test.xlsx";
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open XLS File";
            theDialog.Filter = "XLS files|*.xlsx";
            theDialog.InitialDirectory = Directory.GetCurrentDirectory();
            if (theDialog.ShowDialog() == DialogResult.OK) path = theDialog.FileName;



            using (SqlConnection connection = new SqlConnection(@"Data Source =.\SQLEXPRESS; Initial Catalog = ssystem; Integrated Security=false; User ID=sa; Password=dima; "))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;

                    //try
                    {
                        excelApp = new Excel.Application();
                        workBook = excelApp.Workbooks.Open(path, 0, true, 5, "", "", false, Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
                        workSheet = (Excel.Worksheet)workBook.Worksheets.get_Item(1);

                        i = workSheet.UsedRange.Rows.Count;
                        PB_Init(i);

                        connection.Open();


                        command.CommandText =
                        "IF OBJECT_ID(N'dbo.Counters', N'U') IS NOT NULL " +
                        "BEGIN " +
                            "DROP TABLE Counters " +
                        "END " +
                        "CREATE TABLE Counters" +
                        "(" +
                        "Cnt NVARCHAR(50)," +
                        "btk NVARCHAR(50)," +
                        "ok NVARCHAR(50)," +
                        "CntHour NVARCHAR(50)" +
                        ")";
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT into Counters (Cnt, btk, ok, CntHour) VALUES (@p1, @p2, @p3, @p4)";
                        command.Parameters.AddWithValue("@p1", (workSheet.Cells[1, 1].Value2 != null) ? workSheet.Cells[1, 1].Value.ToString() : "");
                        command.Parameters.AddWithValue("@p2", (workSheet.Cells[1, 2].Value2 != null) ? workSheet.Cells[1, 2].Value.ToString() : "");
                        command.Parameters.AddWithValue("@p3", (workSheet.Cells[1, 3].Value2 != null) ? workSheet.Cells[1, 3].Value.ToString() : "");
                        command.Parameters.AddWithValue("@p4", (workSheet.Cells[1, 4].Value2 != null) ? workSheet.Cells[1, 4].Value.ToString() : "");
                        command.ExecuteNonQuery();


                        k = 1;
                        h = "";
                        do
                        {
                            if (k > 1) h += ", ";
                            h += "COL" + k.ToString() + " NVARCHAR(50)";
                            k++;
                        }
                        while (workSheet.Cells[2, k].Value2 != null);



                        command.CommandText =
                        "IF OBJECT_ID(N'dbo.Planets', N'U') IS NOT NULL " +
                        "BEGIN " +
                            "DROP TABLE Planets " +
                        "END " +
                        "CREATE TABLE Planets" +
                        "(" +
                        h +
                        ")";
                        command.ExecuteNonQuery();

                        j = 2;
                        do
                        {
                            h = "";
                            for (i = 1; i < k; i++)
                            {
                                if (i>1) h += ", ";
                                h += "'";
                                h += (workSheet.Cells[j, i].Value2 != null) ? workSheet.Cells[j, i].Value.ToString() : "";
                                h += "'";
                            }
                            command.CommandText = "INSERT into Planets VALUES (" + h + ")";
                            command.ExecuteNonQuery();
                            j++;
                            PB_Step();
                        }
                        while (workSheet.Cells[j, 1].Value2 != null);
                        
                        
                        /*
                        K.Cnt = (long)workSheet.Cells[1, 1].Value;
                        K.btk = (int)workSheet.Cells[1, 2].Value;
                        K.ok = (int)workSheet.Cells[1, 3].Value;
                        K.CntHour = (double)workSheet.Cells[1, 4].Value;
                        */

                        workBook.Close();
                        connection.Close();
                    }
                    //catch (Exception e1) { MessageBox.Show(e1.Message); }
                }
            }
        }
        //-------------------------------------------------------------
        public void LoadSetting(Excel.Worksheet w)
        {
            int i;
            string t, n;
            Control ctrl;

            i = w.UsedRange.Rows.Count;
            PB_Init(i);

            i = 0;
            do
            {
                i++;
                if (w.Cells[i, 3].Value != null)
                {
                    t = w.Cells[i, 1].Value.ToString();
                    n = w.Cells[i, 2].Value.ToString();
                    ctrl = this.Controls.Find(n, true).FirstOrDefault();

                    switch (t)
                    {
                        case "System.Windows.Forms.TextBox": (ctrl as TextBox).Text = w.Cells[i, 3].Value.ToString(); break;
                        case "System.Windows.Forms.ComboBox": (ctrl as ComboBox).SelectedIndex = (int)w.Cells[i, 3].Value; break;
                        case "System.Windows.Forms.CheckBox": (ctrl as CheckBox).Checked = (bool)w.Cells[i, 3].Value; break;
                        case "System.Windows.Forms.NumericUpDown": (ctrl as NumericUpDown).Value = (decimal)w.Cells[i, 3].Value; break;
                    }
                }
                PB_Step();
            }
            while (w.Cells[i + 1, 1].Value2 != null);
        }
        public void SaveSetting(Excel.Worksheet w)
        {
            kt = 0;
            PereborControls(this.Controls, w);
        }
        public void PereborControls(Control.ControlCollection controls, Excel.Worksheet w)
        {
            foreach (Control ctrl in controls)
            {
                kt++;
                w.Cells[kt, 1] = ctrl.GetType().ToString();
                w.Cells[kt, 2] = ctrl.Name;

                switch (ctrl.GetType().ToString())
                {
                    case "System.Windows.Forms.TextBox": w.Cells[kt, 3] = ctrl.Text; break;
                    case "System.Windows.Forms.ComboBox": w.Cells[kt, 3] = (ctrl as ComboBox).SelectedIndex; break;
                    case "System.Windows.Forms.CheckBox": w.Cells[kt, 3] = (ctrl as CheckBox).Checked; break;
                    case "System.Windows.Forms.NumericUpDown": w.Cells[kt, 3] = (ctrl as NumericUpDown).Value; break;
                }

                PereborControls(ctrl.Controls, w);
            }
        }
        public void GravRecalc()
        {
            int j;

            for (j = 0; j < K.nObj; j++)
            {
                if (K.p[j].dop != null) if (K.p[j].dop.sat != null) { K.p[j].dop.sat.Clear(); K.p[j].dop.sat = null; }
            }


            for (j = 0; j < K.nObj; j++)
            {

                if (K.p[j].dop != null) if (K.p[j].satof > 0) if (K.p[j].r >= K.grsatsiz * 1000 / K.diffR)
                        {
                            if (K.p[K.p[j].satof].dop.sat == null) K.p[K.p[j].satof].dop.sat = new List<int>();
                            K.p[K.p[j].satof].dop.sat.Add(j);
                        }
            }
        }
        public void PB_Init(int i)
        {
            progressBar2.Visible = true;
            progressBar2.Minimum = 1;
            progressBar2.Value = 1;
            progressBar2.Step = 1;
            progressBar2.Maximum = i;
        }
        public void PB_Step()
        {
            progressBar2.PerformStep();
        }
        public double JDf(DateTime dt)
        {
            int y, mnth, d, a, m, jdn;
            double jd;
            y = dt.Year;
            mnth = dt.Month;
            d = dt.Day;

            int hr, mn, sc;
            hr = dt.Hour;
            mn = dt.Minute;
            sc = dt.Second;



            a = (14 - mnth) / 12;
            y = y + 4800 - a;
            m = mnth + 12 * a - 3;

            jdn = d + (153 * m + 2) / 5 + 365 * y + y / 4 - y / 100 + y / 400 - 32045;
            jd = jdn + (hr - 12) / 24 + mn / 1440 + sc / 86400;
            return (jd);



        }
        public dynamic t(string typper, int row)
        {
            int i = 1, p = 1;  //1-double 2-int 3-string 4-datetime
            dynamic ret = "";

            row++;
            switch (typper)
            {
                case "x": i = 1; p = 1; break;
                case "y": i = 2; p = 1; break;
                case "vx": i = 3; p = 1; break;
                case "vy": i = 4; p = 1; break;
                case "mp": i = 5; p = 1; break;
                case "radp": i = 6; p = 1; break;
                case "om": i = 14; p = 1; break;
                case "i": i = 15; p = 1; break;
                case "w": i = 16; p = 1; break;
                case "w1": i = 17; p = 1; break;
                case "akm": i = 18; p = 1; break;
                case "aae": i = 19; p = 1; break;
                case "e": i = 20; p = 1; break;
                case "lambda": i = 21; p = 1; break;
                case "m0": i = 22; p = 1; break;
                case "jd": i = 23; p = 1; break;
                case "mas": i = 30; p = 1; break;
                case "gm": i = 31; p = 1; break;
                case "radkm": i = 32; p = 1; break;
                case "diamkm": i = 33; p = 1; break;
                case "type": i = 7; p = 2; break;
                case "col": i = 8; p = 2; break;
                case "l": i = 9; p = 2; break;
                case "satof": i = 12; p = 2; break;
                case "name": i = 10; p = 3; break;
                case "t01": i = 24; p = 4; break;
                case "t02": i = 25; p = 4; break;
            }

            if (tbl.Rows[row][i].ToString() == "") return null;

            switch (p)
            {
                case 1: ret = Convert.ToDouble(tbl.Rows[row][i]); break;
                case 2: ret = Convert.ToInt32(tbl.Rows[row][i]); break;
                case 3: ret = tbl.Rows[row][i].ToString(); break;
                case 4: ret = Convert.ToDateTime(tbl.Rows[row][i]); break;
            }
            return ret;
        }
        public void prob(int i = 1)
        {
            if (i == 0) pi = 0;
            ms[pi++] = sw.ElapsedTicks;
        }
        //-------------------------------------------------------------

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.OemOpenBrackets || e.KeyCode == Keys.Oem6) { textBox13.Text = K.mag2.ToString(); if (K.PAUSE) K.ShowK(); }
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract) { textBox21.Text = K.mag1.ToString("N2"); if (K.PAUSE) K.ShowK(); }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //Q W E 
            label72.Text = e.KeyCode.ToString();
            if (e.KeyCode == Keys.Subtract && (DateTime.Now - ksp).TotalMilliseconds > 100)
            {
                ksp = DateTime.Now;

                K.mag1speed--;
                if (Math.Sign(K.mag1speed)>0) K.mag1speed = 0;
                label84.Text = K.mag1speed.ToString();

                pb.Focus();
            }
            if (e.KeyCode == Keys.Add && (DateTime.Now - ksp).TotalMilliseconds > 100)
            {
                ksp = DateTime.Now;

                K.mag1speed++;
                if (Math.Sign(K.mag1speed) < 0) K.mag1speed = 0;
                label84.Text = K.mag1speed.ToString();

                pb.Focus();
            }
            if (e.KeyCode == Keys.OemOpenBrackets && (DateTime.Now - ksp).TotalMilliseconds > 100)
            {
                ksp = DateTime.Now;
                K.mag2 -= 50;
                if (K.PAUSE) K.ShowK();
                pb.Focus();
            }
            if (e.KeyCode == Keys.Oem6 && (DateTime.Now - ksp).TotalMilliseconds > 100)
            {
                ksp = DateTime.Now;
                K.mag2 += 50;
                if (K.PAUSE) K.ShowK();
                pb.Focus();
            }
            if (e.KeyCode == Keys.P && (DateTime.Now - ksp).TotalMilliseconds > 100)
            {
                ksp = DateTime.Now;
                button8_Click(sender, e);
                pb.Focus();
            }
            if (e.KeyCode == Keys.W)
            {
                K.p[K.fixnom].ChangeV(1);
                K.ShowK();
                pb.Focus();
            }
            if (e.KeyCode == Keys.S)
            {
                K.p[K.fixnom].ChangeV(-1);
                K.ShowK();
                pb.Focus();
            }
            if (e.KeyCode == Keys.A)
            {
                K.p[K.fixnom].RotateV(-1);
                K.ShowK();
                pb.Focus();
            }
            if (e.KeyCode == Keys.D)
            {
                K.p[K.fixnom].RotateV(1);
                K.ShowK();
                pb.Focus();
            }
            if (e.KeyCode == Keys.Oemcomma)
            {
                K.FixNomChange(-1);
                if (K.PAUSE) K.ShowK();
            }
            if (e.KeyCode == Keys.OemPeriod)
            {
                K.FixNomChange(1);
                if (K.PAUSE) K.ShowK();
            }



        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            /*
            if (keyData == Keys.Left && (DateTime.Now - ksp).TotalMilliseconds > 50)
            {
                ksp = DateTime.Now;
                K.FixNomChange(-1);
                if (K.PAUSE) K.ShowK();
                pb.Focus();
            }
            if (keyData == Keys.Right && (DateTime.Now - ksp).TotalMilliseconds > 50)
            {
                ksp = DateTime.Now;
                K.FixNomChange(1);
                if (K.PAUSE) K.ShowK();
                pb.Focus();
            }
            */
            if (keyData == Keys.Up && (DateTime.Now - ksp).TotalMilliseconds > 50)
            {
                ksp = DateTime.Now;
                int t = 0;
                for (int i = 0; i < K.nObj; i++)
                {
                    if (K.p[i].l > 0 && K.p[i].M > K.p[t].M) t = i;
                }
                K.fixnom = t;
                if (K.PAUSE) K.ShowK();
                pb.Focus();
            }
            if (keyData == Keys.Down && (DateTime.Now - ksp).TotalMilliseconds > 50)
            {
                ksp = DateTime.Now;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox13_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox7_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox8_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox5_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox16_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox16_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox7_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox15_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox11_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox20_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox19_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox6_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox9_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox9_CheckedChanged(object sender, EventArgs e) { if (checkBox9.Checked == false) { textBox15.Text = "1";K.steptimer = 1; } KosmosInit(3); }
        private void textBox21_TextChanged(object sender, EventArgs e) { KosmosInit(3);  }
        private void textBox22_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox23_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox11_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox10_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox4_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox11_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox13_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            decimal prev = (decimal)Double.Parse(((UpDownBase)sender).Text);


            if (Math.Abs(numericUpDown1.Value - prev) == numericUpDown1.Increment)
            {
                if (numericUpDown1.Value - prev < 0)    //уменьшение
                {
                    if (prev == (decimal)1) { numericUpDown1.Value = (decimal)0.9; numericUpDown1.Increment = (decimal)0.1; }
                    if (prev == (decimal)0.1) { numericUpDown1.Value = (decimal)0.09; numericUpDown1.Increment = (decimal)0.01; }
                    if (prev == (decimal)0.01) { numericUpDown1.Value = (decimal)0.009; numericUpDown1.Increment = (decimal)0.001; }
                    if (numericUpDown1.Value == (decimal)0) { numericUpDown1.Value = (decimal)-0.001; }
                    if (prev == (decimal)-0.01) { numericUpDown1.Value = (decimal)-0.02; numericUpDown1.Increment = (decimal)0.01; }
                    if (prev == (decimal)-0.1) { numericUpDown1.Value = (decimal)-0.2; numericUpDown1.Increment = (decimal)0.1; }
                    if (prev == (decimal)-1) { numericUpDown1.Value = (decimal)-2; numericUpDown1.Increment = (decimal)1; }
                }
                else                                    //увеличение
                {
                    if (prev == (decimal)0.01) { numericUpDown1.Value = (decimal)0.02; numericUpDown1.Increment = (decimal)0.01; }
                    if (prev == (decimal)0.1) { numericUpDown1.Value = (decimal)0.2; numericUpDown1.Increment = (decimal)0.1; }
                    if (prev == (decimal)1) { numericUpDown1.Value = (decimal)2; numericUpDown1.Increment = (decimal)1; }
                    if (numericUpDown1.Value == (decimal)0) { numericUpDown1.Value = (decimal)0.001; }
                    if (prev == (decimal)-1) { numericUpDown1.Value = (decimal)-0.9; numericUpDown1.Increment = (decimal)0.1; }
                    if (prev == (decimal)-0.1) { numericUpDown1.Value = (decimal)-0.09; numericUpDown1.Increment = (decimal)0.01; }
                    if (prev == (decimal)-0.01) { numericUpDown1.Value = (decimal)-0.009; numericUpDown1.Increment = (decimal)0.001; }
                }
                //if (numericUpDown1.Value >= (decimal)1 || numericUpDown1.Value <= (decimal)-1) numericUpDown1.DecimalPlaces = 0;
                numericUpDown1.DecimalPlaces = 0;
                if (numericUpDown1.Value > (decimal)-1 && numericUpDown1.Value < (decimal)1) numericUpDown1.DecimalPlaces = 1;
                if (numericUpDown1.Value > (decimal)-0.1 && numericUpDown1.Value < (decimal)0.1) numericUpDown1.DecimalPlaces = 2;
                if (numericUpDown1.Value > (decimal)-0.01 && numericUpDown1.Value < (decimal)0.01) numericUpDown1.DecimalPlaces = 3;
                KosmosInit(3);
            }
            else
            {
                numericUpDown1.DecimalPlaces = 0; numericUpDown1.Increment = (decimal)1;
                if (numericUpDown1.Value > (decimal)-1 && numericUpDown1.Value < (decimal)1) { numericUpDown1.DecimalPlaces = 1; numericUpDown1.Increment = (decimal)0.1; }
                if (numericUpDown1.Value > (decimal)-0.1 && numericUpDown1.Value < (decimal)0.1) { numericUpDown1.DecimalPlaces = 2; numericUpDown1.Increment = (decimal)0.01; }
                if (numericUpDown1.Value > (decimal)-0.01 && numericUpDown1.Value < (decimal)0.01) { numericUpDown1.DecimalPlaces = 3; numericUpDown1.Increment = (decimal)0.001; }
            }

        } 
        private void checkBox6_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox10_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox12_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox14_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox24_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox25_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox17_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox18_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox16_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox24_TextChanged_1(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox26_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox19_CheckedChanged(object sender, EventArgs e) { K.mag1speed = 0; KosmosInit(3); }
        private void button10_Click(object sender, EventArgs e) {textBox26.Text = K.fixnom.ToString(); KosmosInit(3);}
        private void textBox13_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox14_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox26_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox15_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }

        }
        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры, клавиша BackSpace и запятая
            {
                e.Handled = true;
            }
        }
        private void textBox28_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры, клавиша BackSpace и запятая
            {
                e.Handled = true;
            }

        }
        private void textBox28_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void checkBox20_CheckedChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры, клавиша BackSpace и запятая
            {
                e.Handled = true;
            }
        }
        private void textBox27_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox29_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox30_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox31_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox23_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox27_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox32_TextChanged(object sender, EventArgs e) {
            KosmosInit(3);
            GravRecalc();
            K.PrintStatusOnForm();
           }
        private void textBox32_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox29_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox30_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void textBox31_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
            {
                e.Handled = true;
            }
        }
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox3_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox8_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void textBox18_TextChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox17_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e) { KosmosInit(3); }
        private void comboBox18_SelectedIndexChanged_1(object sender, EventArgs e) { KosmosInit(3);  }
        void pb_MouseWheel(object sender, MouseEventArgs e)
        {
            if (K.automag) K.mag1speed = 0;
            else
            {
                K.mag1speed += Math.Sign(e.Delta);
                if (Math.Sign(e.Delta) != Math.Sign(K.mag1speed)) K.mag1speed = 0;
            }
            label84.Text = K.mag1speed.ToString();
        }
        void pb1_MouseWheel(object sender, MouseEventArgs e)
        {
            K.mag2 += Math.Sign(e.Delta); 
            textBox13.Text = K.mag2.ToString();
            if (K.PAUSE) K.ShowK();
        }
    }
}


/*

    KnownColor[] names;


Planet(x, y, vx, vy, m, r, color, form)
p[j] = new Planet(rand.Next(-100, 100), rand.Next(-100, 100), 2 * rand.NextDouble() - 1, 2 * rand.NextDouble() - 1, 100, rand.Next(3, 10), Color.FromName(names[rand.Next(names.Length)].ToString()), this);
p[j] = new Planet(rand.Next(-100, 100), rand.Next(-100, 100), 2 * rand.NextDouble() - 1, 2 * rand.NextDouble() - 1, 100, rand.Next(3, 10), Color.FromKnownColor(names[rand.Next(names.Length)]), this);

p[j] = new Planet(rand.Next(-100, 100), rand.Next(-100, 100),
    4 * rand.NextDouble() - 2, 4 * rand.NextDouble() - 2, 100, rand.Next(3, 10),
   Color.FromArgb(rand.Next(100, 200), rand.Next(100, 200), rand.Next(100, 200)), this);
p[j] = new Planet(rand.Next(-100, 100), rand.Next(-100, 100),
   0, 0, 100, rand.Next(3, 10),
   Color.FromArgb(rand.Next(100, 200), rand.Next(100, 200), rand.Next(100, 200)), this);

 public double getx() { return x; }
 public double gety() { return y; }
 public double getvx() { return vx; }
 public double getvy() { return vy; }

 public double getR() { return R; }

 public double geta() { return a; }
 public double getax() { return ax; }
 public double getay() { return ay; }
 

label2.Text = "x: " + p[0].getx().ToString("#.##");
label3.Text = "y: " + p[0].gety().ToString("#.##");
label4.Text = "vx: " + p[0].getvx().ToString("#.##");
label5.Text = "vy: " + p[0].getvy().ToString("#.##");
label6.Text = "R: " + p[0].getR().ToString("#.##");
label7.Text = "a: " + p[0].geta().ToString("#.####");
label8.Text = "ax: " + p[0].getax().ToString("#.####");
label9.Text = "ay: " + p[0].getay().ToString("#.####");

SolidBrush br = new SolidBrush(col);
f.g.DrawEllipse(p, 200 + (int)x - r, 200 + (int)y - r, 2 * r, 2 * r);

SolidBrush br = new SolidBrush(col);
f.g1.FillEllipse(Brushes.White, 200 + (int)x1 - r - 1, 200 + (int)y1 - r - 1, 2 * r + 2, 2 * r + 2);
f.g.DrawEllipse(p, 200 + (int)x - r, 200 + (int)y - r, 2 * r, 2 * r);
f.g1.FillEllipse(br, 200 + (int)x - r, 200 + (int)y - r, 2 * r, 2 * r);
f.g1.DrawEllipse(new Pen(Color.FromArgb((l > 25000) ? 0 : 255 - l / 100, 0, 0), 2), 200 + (int)x - r, 200 + (int)y - r, 2 * r, 2 * r);
new Pen(Color.Black, 3);
f.g1.FillEllipse(new SolidBrush(Color.FromArgb((l>25000)?0:255-l/100, 0, 0)), 200 + (int)x - r, 200 + (int)y - r, 2 * r, 2 * r);
names = (KnownColor[])Enum.GetValues(typeof(KnownColor));


pb.Image = bmp;
worker.ReportProgress(1);
System.Threading.Thread.Sleep(50);



          private void button5_Click(object sender, EventArgs e)
        {
            //InitializeSpace();
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
                button4.Enabled = false;
                button5.Enabled = false;
            }
        }   //back


          if (bw != null && bw.IsBusy && !bw.CancellationPending)
            {
                bw.CancelAsync();
                button4.Enabled = true;
                //button5.Enabled = true;
            }



                string path, path1;
            StreamWriter fl;
            try
            {
                path = Directory.GetCurrentDirectory() + "\\Test.txt";
                //MessageBox.Show(path);
                fl = new StreamWriter(path);
                for (int i = 0; i < K.nObj; i++)
                {
                    fl.WriteLine(i + " " + K.p[i].x + " " + K.p[i].y + " " + K.p[i].Vx + " " + K.p[i].Vy + " " + K.p[i].M + " " + K.p[i].r + " " + K.p[i].br.Color);
                }
                fl.Close();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Exception: " + e1.Message);
            }
            finally
            {
                //MessageBox.Show("Executing finally block.");
            }



            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);



        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                for (int i = 0; i < 1000000 && !worker.CancellationPending; i++)
                {
                    for (int j = 0; j < K.nObj; j++) K.p[j].Step();
                    for (int j = 0; j < K.nObj; j++) K.p[j].Gravity();

                    if (++K.Cnt % 100 == 0)
                    {
                        //K.diff = (DateTime.Now - K.dt).TotalMilliseconds;
                        worker.ReportProgress(1);
                        K.dt = DateTime.Now;
                    }

                }
            }
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //label1.Text = "fps: " + (100 * 1000 / K.diff).ToString("#.##");
            label2.Text = "btk: " + K.btk + "-" + K.ok;
            label3.Text = K.Cnt.ToString();
            //resultLabel.Text = (e.ProgressPercentage.ToString() + "%");
            //p1.Show(g, Color.Red);
            //p2.Show(g, Color.Black);
            //p1.Move();
            //p2.Move();
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }


                    cmd.CommandText = "SELECT OBJECT_ID(N'dbo.Settings', N'U')";


                CREATE TABLE Customers
                (
                Id INT,
                Age INT,
                FirstName NVARCHAR(20),
                LastName NVARCHAR(20),
                Email VARCHAR(30),
                Phone VARCHAR(20)
                )


                cmd.ExecuteNonQuery

IF OBJECT_ID(N'dbo.Settings', N'U') IS NOT NULL BEGIN
    DROP TABLE Settings
END

            IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES   
    WHERE TABLE_CATALOG = @Catalog 
      AND TABLE_SCHEMA = @Schema 
      AND TABLE_NAME = @Table))
BEGIN
   --do stuff
END


            IF OBJECT_ID('*objectName*', 'U') IS NOT NULL 
                    
                IF OBJECT_ID (N'dbo.AWBuildVersion', N'U') IS NOT NULL
                SELECT OBJECT_ID(N'dbo.MyTable', N'U')

                //drop table if exists mytablename

                //(new SqlCommand("DELETE Table_report1", table_report1TableAdapter.Connection)).ExecuteNonQuery();


                cmd.CommandText = "SELECT * FROM Cap";
                dr = cmd.ExecuteReader();
                dr.Read();
            }
                dr.Close();

                //SqlConnection sqlConnection = new SqlConnection();
            //sqlConnection = new SqlConnection(@"Data Source =.\SQLEXPRESS; Initial Catalog = ssystem; Integrated Security=false; User ID=sa; Password=dima; ");
            //sqlConnection = new SqlConnection("Data Source=localhost; Integrated Security=SSPI; Initial Catalog=testdb;");
            //sqlConnection = new SqlConnection("Data Source=.\SQLEXPRESS; Integrated Security=false; User ID=sa; Password=dima; Initial Catalog=testdb;");
            //sqlConnection.Open();
            //MyClass.scriptExecute(sqlConnection, "database.sql");
            //sqlConnection.Close();
            //sqlConnection.Dispose();


 */
