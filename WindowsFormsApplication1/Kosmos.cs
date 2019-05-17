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





namespace WindowsFormsApplication1
{

    public struct crd
    {
        public double x;
        public double y;
    }


    public class Space
    {
      


        public Form1 f;
        public Planet[] p = new Planet[10000];
        public List<int>[,] doska = new List<int>[40, 40];
        public List<int> doskaM1 = new List<int>();
        public Queue<crd> doskaShl = new Queue<crd>();

        public int nObj;        //tb1
        public double mS;       //tb2   масса солнца
        public double vP1;       //tb3   мин скор планеты
        public double rS;       //tb5   радиус солнца
        public double mP1;       //tb6   мин масса планеты
        public int cntvid;      //tb7   через сколько шагов отображение
        public double rP1;       //tb10  мин рад планеты
        public double rP2;       //tb4   макс рад планеты
        public double mP2;       //tb6   макс масса планеты
        public double vP2;       //tb8   макс скор планеты
        public int cs;          //tb11  частота вывода статуса
        public double mag1;         //tb21  magnification main display
        public double mag1speed;
        public double mag2;         //tb13  magnification mini display
        public int vmult;       //tb14  multipl v
        public int steptimer;    //tb15 step in timer 
        public int lorbold;    //tb16 old orbita 
        public int newgrrad;    //tb18 old orbita 
        public double rx1;    //tb20 rx1 
        public double rx2;    //tb19 rx2 
        public double stop;    //stop date
        public double accmag;     //tb24 acc mag
        public int cntfixnom;     //tb26

        public SolidBrush brfon;     //cb1
        public int era;         //cb2
        public int young;       //cb3
        public int ap;          //cb4
        public int bt;          //cb9
        public int ts;          //cb8   тип солнца
        public Color colsun;     //cb13
        public int rt;          //cb11  определение радиуса
        public int rst;         //cb14  определение радиуса
        public int vt;          //cb7   тип скорости
        public int stt;         //cb10
        public int vzt;         //cb16
        public int grt;         //cb6
        public int rsmt;         //cb15
        public int rbrn;         //cb17
        public int mt;         //cb5
        public int viztyp;      //cb18  что показывать - орбиты итд
        public double timecompress;


        public bool fixnomch;    //chb1  меченый
        public bool mini;        //chb4   mini display
        public bool orb1;         //chb5  orbita1
        public bool per1;         //chb10  perigelii1
        public bool orb2;         //chb12  orbita2
        public bool per2;         //chb14  perigelii2
        public bool display;     //chb7 display
        public bool shleif;     //chb8 shleif
        public bool autofps;     //chb9
        public bool pname;      //chb17
        public bool cntfix;     //chb18
        public bool stopdat;     //chb16
        public bool automag;     //chb19
        public bool parallel;    //chb27
        public bool jup;        //chb28

        //----------------------------------------------------
        public bool[] objviz = new bool[6];
        public int[] objsizmag = new int[6];
        public double[] objsizlim = new double[6];
        public double grsatsiz;
        public bool[] objorb = new bool[6];
        public bool[] objnam = new bool[6];
        public int asttyp;
        public string asttyptxt;

        public bool allsat;
        //----------------------------------------------------

        public bool vctspd;     //вектор скорости
        public bool vctacc;     //вектор ускорения
        public bool printreal;  //принт реал

        public int btk;         //счетчик all kill
        public int ok;          //счетчик old kill
        public long Cnt;        //счетчик шагов космоса
        public double CntHour, ch;        //счетчик часов космоса
        public int cntprint;


        public DateTime dt, dt2;     //для расчета fps
        public int fixnom;
        public crd tempcrd;
        public bool pause;
        public Rectangle rct;
        public SolidBrush br;
        public Pen pn;
        public int q1, q2, q3, q4;
        public double dx, dy;

        delegate double spfunc(double x, double y);

        private string namemini;
        spfunc hyp = (x, y) => Math.Sqrt(x * x + y * y);

      

        public bool PAUSE
        {
            get { return pause; }
            set
            {
                pause = value;

                if (pause)
                {
                    f.button8.Enabled = true; f.button9.Enabled = true; f.saveToXlsToolStripMenuItem.Enabled = true; f.loadFromXlsToolStripMenuItem.Enabled = false;
                    f.button8.Text = "resume";
                }
                else
                {
                    f.button8.Enabled = true; f.button9.Enabled = false; f.saveToXlsToolStripMenuItem.Enabled = false; f.loadFromXlsToolStripMenuItem.Enabled = false;
                    f.button8.Text = "pause";
                }
            }
        }



        public const double ae = 149597870700;//м  //астрономическая единица
        public const double G = 6.6740831313131313131313131E-11; //гравитациооная постоянная 6,67408(31)·10−11 м3·с−2·кг−1, или Н·м²·кг−2.
        
        public double Gg, diffM, diffR, diffT;


        public int[] vzryv = new int[20] { 4, 5, 7, 9, 10, 10, 10, 10, 10, 10, 10, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        public Space() {}

        public Space(Form1 i1)
        {
            int i, j;
            f = i1;
            colsun = Color.Red;
            brfon = new SolidBrush(Color.Red);
            br = new SolidBrush(Color.Red);
            pn = new Pen(Color.Red);

           
            diffM = 1E30;
            diffR = ae / 8;  //м    //100ae - 400 точек  перевод из метров в точки
            diffT = 3600;
            Gg = G / diffR / diffR / diffR * diffT * diffT * diffM;
            dt2 = new DateTime(2000, 1, 1, 0, 0, 0);

            PAUSE = true;
            f.button8.Enabled = false; f.button9.Enabled = true; f.saveToXlsToolStripMenuItem.Enabled = false; f.loadFromXlsToolStripMenuItem.Enabled = true;

            for (j = 0; j < 10000; j++) p[j] = new Planet(this, j);
            for (i = 0; i < 40; i++) for (j = 0; j < 40; j++) doska[i, j] = new List<int>();

            

        }
        public void InitializeKosmos(int TYP, Form1 i1)
        {

            timecompress = (double)f.numericUpDown1.Value;


            if (TYP == 1)
            {
                nObj = Int32.Parse(f.textBox1.Text);     //                                     
                f = i1;
                btk = 0;        //death counter
                ok = 0;         //death old counter
                Cnt = 0;        //kosmos counter
                CntHour = 0;
                ts = f.comboBox8.SelectedIndex;       //sun type                            
                mS = Double.Parse(f.textBox2.Text);       //sun mass                            
                rS = Double.Parse(f.textBox5.Text);       //sun radius                           
                rst = f.comboBox14.SelectedIndex;     //sun radius count type               
                rsmt = f.comboBox15.SelectedIndex;    //real sun move type  0-no 1-yes      
                fixnom = 1;     //fix
                mag1speed = 0;
            }

            vP1 = Double.Parse(f.textBox3.Text);          //max v pl
            vP2 = Double.Parse(f.textBox8.Text);          //max v pl
            mP1 = Double.Parse(f.textBox6.Text);          //min m pl
            mP2 = Double.Parse(f.textBox9.Text);          //max m pl
            rP1 = Double.Parse(f.textBox10.Text);         //min R pl
            rP2 = Double.Parse(f.textBox4.Text);          //max R pl

            cs = Int32.Parse(f.textBox11.Text);          //частота вывода статуса
            mag2 = Double.Parse(f.textBox13.Text);        //magnification mini display
            mag1 = Double.Parse(f.textBox21.Text);        //magnification main display
            vmult = Int32.Parse(f.textBox14.Text);       //multipl v умножитель рахмера планеты от массы
            steptimer = Int32.Parse(f.textBox15.Text);   //step in timer
            lorbold = Int32.Parse(f.textBox16.Text);     // old orbita l при котором показывается орбита при паузе 
            newgrrad = Int32.Parse(f.textBox18.Text);    //радиус новой гравитации
            rx1 = Double.Parse(f.textBox20.Text);         //радиус в котором появляютс яновые планеты
            rx2 = Double.Parse(f.textBox19.Text);         //радиус в котором появляютс яновые планеты
            cntvid = Int32.Parse(f.textBox7.Text);       //через сколько шагов отображение
            stop = (DateTime.Parse(f.textBox25.Text) - dt2).TotalHours;
            accmag = Double.Parse(f.textBox24.Text);      //увеличение вектора ускорения
            era = f.comboBox2.SelectedIndex;          //стирать или нет
            young = f.comboBox3.SelectedIndex;        //как отображать новые планеты
            ap = f.comboBox4.SelectedIndex;           //делать ли паузу при дис олд
            bt = f.comboBox9.SelectedIndex;           //реакция на бордюр
            rt = f.comboBox11.SelectedIndex;          //определение радиуса
            vt = f.comboBox7.SelectedIndex;           //тип скорости
            stt = f.comboBox10.SelectedIndex;         //тип столкновения
            vzt = f.comboBox16.SelectedIndex;         //тип взрыва
            grt = f.comboBox6.SelectedIndex;          //тип гравитации
            rbrn = f.comboBox17.SelectedIndex;        //возникновение заново
            mt = f.comboBox5.SelectedIndex;           //тип распределения масс
            viztyp = f.comboBox18.SelectedIndex;      // какие элементы показывать
            fixnomch = f.checkBox1.Checked;    //отображать метку
            mini = f.checkBox4.Checked;        //мини дисплэй активность
            orb1 = f.checkBox5.Checked;         //показывать орбиту
            per1 = f.checkBox10.Checked;
            orb2 = f.checkBox12.Checked;
            per2 = f.checkBox14.Checked;
            display = f.checkBox7.Checked;     //показывать основной дисплей
            shleif = f.checkBox8.Checked;      //показывать шлейф
            autofps = f.checkBox9.Checked;     //авто фпс
            vctspd = f.checkBox11.Checked;     //вектор скорости
            vctacc = f.checkBox13.Checked;     //вектор ускорения
            printreal = f.checkBox6.Checked;
            pname = f.checkBox17.Checked;
            cntfix = f.checkBox18.Checked;
            cntfixnom = Int32.Parse(f.textBox26.Text);
            stopdat = f.checkBox16.Checked;
            automag = f.checkBox19.Checked;
            parallel = f.checkBox27.Checked;
            jup = f.checkBox28.Checked;


            //------------------------------------------------

            objviz[0]=true;
            objviz[1]=true;
            objviz[2]=true;
            objviz[3]=f.checkBox25.Checked;
            objviz[4]=f.checkBox26.Checked;
            objviz[5]=true;


            objsizmag[0] = Int32.Parse(f.textBox23.Text);   //увеличение размера планет  1-sun 2-planet 3-sputnik 4-cometa 5-asteroid 6-kr planeta
            objsizmag[1] = Int32.Parse(f.textBox22.Text);
            objsizmag[2] = Int32.Parse(f.textBox27.Text);
            objsizmag[3] = Int32.Parse(f.textBox29.Text);
            objsizmag[4] = Int32.Parse(f.textBox30.Text);
            objsizmag[5] = Int32.Parse(f.textBox31.Text);

            objsizlim[0] = 0;   
            objsizlim[1] = 0;
            objsizlim[2] = Double.Parse(f.textBox28.Text);
            objsizlim[3] = Double.Parse(f.textBox33.Text);
            objsizlim[4] = Double.Parse(f.textBox34.Text);
            objsizlim[5] = 0;

            grsatsiz = double.Parse(f.textBox32.Text);

            objorb[0]=true;
            objorb[1]= f.checkBox15.Checked;
            objorb[2]= f.checkBox32.Checked;
            objorb[3]=f.checkBox24.Checked;
            objorb[4]=f.checkBox21.Checked;
            objorb[5]=true;

                    
            objnam[0]= f.checkBox31.Checked;
            objnam[1]= f.checkBox29.Checked;
            objnam[2]= f.checkBox30.Checked;
            objnam[3]=f.checkBox23.Checked;
            objnam[4]=f.checkBox22.Checked; 
            objnam[5]=true;

            asttyp= f.comboBox19.SelectedIndex;
            asttyptxt = f.comboBox19.Text;

            allsat = f.checkBox20.Checked;
            //------------------------------------------------



            switch (f.comboBox1.SelectedIndex)
            {
                case 0: brfon.Color = Color.White; break;
                case 1: brfon.Color = Color.Black; break;
            }
            switch (f.comboBox13.SelectedIndex)
            {
                case 0: colsun = Color.Yellow; break;
                case 1: colsun = Color.Orange; break;
                case 2: colsun = Color.Red; break;
                case 3: colsun = Color.Green; break;
            }

            Cls(true);
            if (TYP == 1) for (int j = 0; j < nObj; j++) p[j].Recreate();
            if (TYP == 3) if (PAUSE) ShowK();

        }
        public void ShowK()
        {
            Cls(true);
            ShowStart();
            for (int j = 0; j < nObj; j++) if (p[j].IsVisible()) p[j].Show();
            ShowEnd();
            if (PAUSE == true && mag1speed == 0) { f.ms7 = f.sw.ElapsedTicks; PrintStatusOnForm(); }
        }

        public void Move(int j)
        {
            p[j].Move();
        }
        public void Gravity6(int j)
        {
            p[j].Gravity6();
        }

        public void Step(int v)
        {
            int j;
            bool u;
            f.ms0 = f.ms1;
            f.ms1 = f.sw.ElapsedTicks;
            Cnt++;
            CntHour += timecompress;

            if (CntHour >= stop && stopdat) { PAUSE = true; ShowK(); }

            u = true;
            if (v > 0) u = false;
            else
                if (!display) u = false;
                else
                    if (f.WindowState == FormWindowState.Minimized) u = false;
                    else
                        if (cntvid != 1) if (Cnt % cntvid != 0) u = false;



            if (u) ShowStart();
            //==============================================
            if (u) for (j = 0; j < nObj; j++) if (p[j].IsVisible()) p[j].Show();
            f.ms2 = f.sw.ElapsedTicks;


            if (parallel) Parallel.For(0, nObj, Move); else for (j = 0; j < nObj; j++) p[j].Move();                                                 
            //
            f.ms3 = f.sw.ElapsedTicks;


            if (stt!=13) for (j = 0; j < nObj; j++) if (p[j].l >= 0) p[j].Collision();
            f.ms4 = f.sw.ElapsedTicks;


            //---------------------------------------------
            for (j = 0; j < nObj; j++) { p[j].ax = p[j].ay = 0; }
            if (ts == 0) for (j = 0; j < nObj; j++) if (p[j].l >= 0) p[j].GravityToCenter();
            f.ms5 = f.sw.ElapsedTicks;
            if (rsmt==1) if(ts == 1) p[0].GravityForSun();

            //sun   sun old     all     all new     satof       satof sat
            f.prob(0);
            switch (grt)
            {
                case 0: for (j = ts; j < nObj; j++) if (p[j].l >= 0) p[j].Gravity1(); break;
                case 1: for (j = ts; j < nObj; j++) if (p[j].l >= 0) p[j].Gravity2(); break;
                case 2: for (j = ts; j < nObj; j++) if (p[j].l >= 0) p[j].Gravity3(); break;
                case 3: for (j = ts; j < nObj; j++) if (p[j].l >= 0) p[j].Gravity4(); break;
                case 4: for (j = ts; j < nObj; j++) if (p[j].l >= 0) p[j].Gravity5(); break;
                case 5: if (parallel) Parallel.For(ts, nObj, Gravity6); else for (j = ts; j < nObj; j++) p[j].Gravity6(); break;
            }
            f.prob();
            f.ms6 = f.sw.ElapsedTicks;
            //==============================================
            if (u) ShowEnd();
        }
        public void ShowStart()
        {

            if (era == 1) Cls(false);


            if (automag && fixnom != 0)
            {
                dx = p[fixnom].x;
                dy = p[fixnom].y;
                if (cntfix) { dx -= p[cntfixnom].x; dy -= p[cntfixnom].y; }
                if (dx != 0 || dy != 0) mag1 = Math.Abs(350 / Math.Sqrt(dx * dx + dy * dy));
                //if (Math.Abs(dx) > Math.Abs(dy)) mag1 = Math.Abs(350 / dx); else mag1 = Math.Abs(350 / dy);
            }

            if (orb1 || orb2)
            switch (viztyp)  //None Fix Old All Vized
            {
                case 0: break;

                case 1: if (orb1) p[fixnom].Orbita(p[fixnom].col);
                        if (orb2) if (p[fixnom].dop != null) p[fixnom].Orbita2(Color.Green);
                        break;

                case 2: for (int j = 0; j < nObj; j++)
                        {
                            if (p[j].IsVisible())
                            {
                                if (orb1) if ((p[j].dop != null) ? objorb[p[j].dop.type - 1] : true)  p[j].Orbita(p[j].col);
                                if (orb2) if (p[j].dop != null)  p[j].Orbita2(Color.Green);
                            }
                        }
                        break;
            }
            if (pname) f.g1.DrawString("\u2648", new Font("Tahoma", 15), ((brfon.Color == Color.White) ? Brushes.Black : Brushes.White), 770, 390);
        }
        private void ShowEnd()
        {
            if (vctspd || vctacc)
            switch (viztyp)  //None Fix Old All
            {
                case 0: break;
                case 1: if (vctspd) p[fixnom].SpeedVector(); if (vctacc) p[fixnom].AccVector(); break;
                case 2:
                        for (int j = 0; j < nObj; j++)
                        {
                            if (p[j].IsVisible())
                            {
                                if (vctspd) p[j].SpeedVector();
                                if (vctacc) p[j].AccVector();
                            }
                        }
                        break;
            }



            if (ts == 0)
            {
                DE(f.g1, colsun, 0, 0, rS, mag1, cntfix, cntfixnom);
                if (mini) if (Math.Abs(p[fixnom].x) < 400 / mag2 / mag1 && Math.Abs(p[fixnom].y) < 400 / mag2 / mag1)
                    DE(f.g2, colsun, 0, 0, rS, mag2 * mag1, true, fixnom);

            }
            if (fixnomch) if (p[fixnom].old == false && young == 1) ShowFix(p[fixnom].x, -p[fixnom].y, 1, objsizmag[(p[fixnom].dop == null) ? 0 : p[fixnom].dop.type-1]); else ShowFix(p[fixnom].x, -p[fixnom].y, p[fixnom].r, objsizmag[(p[fixnom].dop == null) ? 0 : p[fixnom].dop.type-1]);
            if (shleif) p[fixnom].Shleif();
            if (mini) if (pname) if (p[fixnom].dop != null) f.g2.DrawString(p[fixnom].dop.name, new Font("Tahoma", 10), ((brfon.Color == Color.White) ? Brushes.Black : Brushes.White), 380, 500);


            //for (int j = 0; j < nObj; j++) if (p[j].old == true && p[j].l < 0 && p[j].k < 20) p[j].Orbita(Color.Red);
            f.pb.Image = f.bmp1;
            if (mini) { f.g3.DrawImage(f.bmp2, f.pb1.Width / 2 - 400, f.pb1.Height / 2 - 400); f.pb1.Image = f.bmp_crop; }


            if (!mini)
            {
                if (p[fixnom].dop != null && p[fixnom].dop.name != namemini)
                {
                    namemini = p[fixnom].dop.name;
                    //f.pb1.BackColor = Color.Black;
                    //f.pb1.SizeMode = PictureBoxSizeMode.CenterImage;
                    var image = ScaleImage(fixnom, f.pb1.Width, f.pb1.Height);
                    f.pb1.Image = image;
                }
            }
        }

        private Bitmap ScaleImage(int num, int maxWidth, int maxHeight)
        {
            var outputImage = new Bitmap(maxWidth, maxHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(outputImage)) graphics.Clear(Color.Black);


            var fileName = $@"PlanetImages\{num}_{p[num].dop?.name}.jpg";

            if (File.Exists(fileName))
            {
                var image = Image.FromFile(fileName);
                var ratioX = (double) maxWidth / image.Width;
                var ratioY = (double) maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);
                var newWidth = (int) (image.Width * ratio);
                var newHeight = (int) (image.Height * ratio);
                using (var graphics = Graphics.FromImage(outputImage))
                {
                    graphics.Clear(Color.Black);
                    graphics.DrawImage(image,
                        new Rectangle(new Point((maxWidth - newWidth) / 2, (maxHeight - newHeight) / 2),
                            new Size(newWidth, newHeight)),
                        new Rectangle(new Point(), image.Size), GraphicsUnit.Pixel);
                }
            }

            using (var graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawString(p[num].dop.name, new Font("Tahoma", 12), ((brfon.Color == Color.White) ? Brushes.Black : Brushes.White), 170, 220);
            }


            Bitmap bmp = new Bitmap(outputImage);

            return bmp;
        }

    

        public void PrintStatusOnForm()
        {
            int i, j, l_cnt, l_old_cnt, list_cnt, m1_cnt, sp;
            double m_cnt;
            double q, st, v;
            l_cnt = l_old_cnt = list_cnt = m1_cnt = 0;
            m_cnt = 0;

            for (i = 0; i < 40; i++) for (j = 0; j < 40; j++) list_cnt += doska[i, j].Count;

            for (j = 0; j < nObj; j++)
                if (p[j].l >= 0)
                {
                    l_cnt++;
                    if (p[j].old == true) l_old_cnt++;
                    m_cnt += p[j].M;
                    if (p[j].M >= 1) m1_cnt++;
                }


            q = (DateTime.Now - dt).TotalMilliseconds;
            f.label1.Text = "fps: " + ((cs * steptimer) * 1000 / q).ToString("#");
            f.label56.Text = "год = " + (365 * 24 * q / 1000 / (CntHour - ch)).ToString("0.###") + " с";
            ch = CntHour;
            dt = DateTime.Now;

            //f.textBox21.Text = mag1.ToString("N2");


            f.label2.Text = "btk: " + btk + "-" + ok;
            f.label3.Text = "STEP:" + Cnt.ToString();
            f.label95.Text = "Time:" + (CntHour / 24).ToString("N2") + " дн (" + (CntHour / 24 / 365).ToString("N2") + ") лет";



            //dt2 = DateTime(2000, 1, 1, 0, 0, 0);
            //dt2.AddDays(CntHour / 24);
            f.label50.Text = dt2.AddDays(CntHour / 24).ToShortDateString();



            f.label9.Text = "old:" + l_old_cnt;
            //f.label27.Text = f.WindowState.ToString();
            f.label30.Text = fixnom.ToString();
            //if (p[fixnom].dop != null) f.label55.Text = p[fixnom].dop.name + "  " + fixnom + "  " + p[fixnom].dop.info;
            f.label55.Text = p[fixnom].dop?.name + "  " + fixnom + "  " + p[fixnom].dop?.info;

            f.label32.Text = "life:" + p[fixnom].l.ToString();
            f.label33.Text = "r:" + p[fixnom].r.ToString("N6");
            f.label34.Text = "m:" + p[fixnom].M.ToString("N6");
            f.label35.Text = "x:" + p[fixnom].x.ToString("N2");
            f.label36.Text = "y:" + p[fixnom].y.ToString("N2");
            f.label37.Text = "vx:" + p[fixnom].Vx.ToString("N5");
            f.label38.Text = "vy:" + p[fixnom].Vy.ToString("N5");
            f.label39.Text = "ax:" + p[fixnom].ax.ToString("N5");
            f.label40.Text = "ay:" + p[fixnom].ay.ToString("N5");

            if (printreal)
            {

                f.label80.Text = "Радиус:" + (p[fixnom].r * diffR / 1000).ToString("N0") + " км";
                f.label89.Text = "Масса:" + (p[fixnom].M * diffM).ToString() + " кг";
                v = 4D / 3D * Math.PI * Math.Pow(p[fixnom].r * diffR,3);  //м3

                var density = p[fixnom].M * diffM / v / 1000;
                if (!double.IsInfinity(density) & !double.IsNaN(density))
                    f.label91.Text = "Плотность:" + density.ToString("N2") + " г/см3";
                sp = 0;
                //if (p[fixnom].dop != null) if (p[fixnom].dop.sat != null) sp = p[fixnom].dop.sat.Count;
                if (p[fixnom].dop?.sat != null) sp = p[fixnom].dop.sat.Count;
                f.label114.Text = "Спутников:" + sp;


                f.label88.Text = "x:" + (p[fixnom].x * diffR).ToString("N2") + " м";
                f.label87.Text = "y:" + (p[fixnom].y * diffR).ToString("N2") + " м";
                f.label86.Text = "vx:" + (p[fixnom].Vx * diffR / diffT).ToString("N2") + " м/c";
                f.label85.Text = "vy:" + (p[fixnom].Vy * diffR / diffT).ToString("N2") + " м/c";
                f.label83.Text = "ax:" + (p[fixnom].ax * diffR / diffT / diffT).ToString("N5") + " м/c2";
                f.label81.Text = "ay:" + (p[fixnom].ay * diffR / diffT / diffT).ToString("N5") + " м/c2";
                //f.label84.Text =
                //f.label82.Text =

                st = G * p[fixnom].M * diffM / p[fixnom].r / p[fixnom].r / diffR / diffR;
                if (!double.IsInfinity(st) & !double.IsNaN(st))
                    f.label59.Text = "Cила тяжести:" + st.ToString("N3") + " м/c2 (" + (st / 9.82026).ToString("0.##") + "G)";
                f.label124.Text = "Расст до Земли:" + (hyp(p[fixnom].x - p[3].x, p[fixnom].y - p[3].y) * diffR / 1000000000).ToString("N2") + " млн км";
            }



            long frequency = Stopwatch.Frequency;
            long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;

            //f.label16.Text = "ms2:  " + frequency;
            //f.label18.Text = "ms3:  " + nanosecPerTick;
            //f.label23.Text = "ms4:  " + TimeSpan.TicksPerMillisecond;


            f.textBox12.Text = p[fixnom].M.ToString();
            f.textBox17.Text = p[fixnom].r.ToString();

            Process currentProcess = Process.GetCurrentProcess();
            f.label79.Text = (currentProcess.PrivateMemorySize64 / 1024).ToString() + " Kb";

            f.label57.Text = "Real: " + l_cnt.ToString();
            f.label64.Text = "M: " + m_cnt.ToString("N2");
            f.label66.Text = "M1:" + m1_cnt + "-dM1:" + doskaM1.Count;
            f.label60.Text = "Doska: " + list_cnt;

            f.label16.Text = "timer step:  " + (f.ms1 - f.ms0) / f.TPM + "мкс";
            f.label18.Text = "show:  " + (f.ms2 - f.ms1) / f.TPM + "мкс";
            f.label23.Text = "move:  " + (f.ms3 - f.ms2) / f.TPM + "мкс";
            f.label27.Text = "collision:  " + (f.ms4 - f.ms3) / f.TPM + "мкс";
            f.label61.Text = "gravity sun:  " + (f.ms5 - f.ms4) / f.TPM + "мкс";
            f.label62.Text = "gravity:  " + (f.ms6 - f.ms5) / f.TPM + "мкс";
            f.label69.Text = "show end:  " + (f.ms7 - f.ms6) / f.TPM + "мкс";
            int all;
            all = (int)(((f.ms7 - f.ms1 != 0) ? (f.ms7 - f.ms1) : 1) / f.TPM);
            f.label70.Text = "all step:  " + all + "мкс";
            f.label71.Text = "=> " + 1000000 / ((all == 0) ? 1 : all);
            f.label97.Text = "all group:  " + (f.ms7 - f.ms8) / f.TPM + "мкс";

            f.label101.Text = "0-1  " + (f.ms[1] - f.ms[0]) / f.TPM + "мкс";
            f.label102.Text = "1-2  " + (f.ms[2] - f.ms[1]) / f.TPM + "мкс";
            f.label103.Text = "2-3  " + (f.ms[3] - f.ms[2]) / f.TPM + "мкс";
            f.label104.Text = "3-4  " + (f.ms[4] - f.ms[3]) / f.TPM + "мкс";
            f.label105.Text = "4-5  " + (f.ms[5] - f.ms[4]) / f.TPM + "мкс";
            f.label106.Text = "5-6  " + (f.ms[6] - f.ms[5]) / f.TPM + "мкс";
            f.label107.Text = "6-7  " + (f.ms[7] - f.ms[6]) / f.TPM + "мкс";
            f.label108.Text = "7-8  " + (f.ms[8] - f.ms[7]) / f.TPM + "мкс";
            f.label109.Text = "8-9  " + (f.ms[9] - f.ms[8]) / f.TPM + "мкс";
            f.label110.Text = "9-10  " + (f.ms[10] - f.ms[9]) / f.TPM + "мкс";


            if (autofps)
            {
                steptimer = 1000000/64 / all + 1;
                if (steptimer > 5000) steptimer = 5000;
                f.textBox15.Text = steptimer.ToString();

            }
        }
        public void FixNomChange(int i)
        {
            int t;

            t = fixnom;
            do
            {
                t += i;
                if (t < 0) t = nObj - 1;
                if (t == nObj) t = 0;
            }
            while (p[t].l < 0 && t != fixnom && young > 0 || p[t].old != true && t != fixnom && young == 0 || p[t].IsVisible()==false);

            fixnom = t;

        }
        public void ShowFix(double i1, double i2, double i3, double mag)
        {
            //int o =4, t=3;
            //if (i3 < 3) i3 = 3;

            double o = 4, t = 3;
            //if (i3 < 3) i3 = 3;
            o /= mag1; t /= mag1;
            i3 *= mag;

            Color col = Color.Black;
            if (brfon.Color == Color.Black) col = Color.White;
            DL(f.g1, col, 2, i1 - i3 - t, -i2 + i3 + t, i1 - i3 - t,-i2 - i3 - t, mag1, cntfix, cntfixnom);
            DL(f.g1, col, 2, i1 + i3 + t, -i2 + i3 + t, i1 + i3 + t,-i2 - i3 - t, mag1, cntfix, cntfixnom);
            DL(f.g1, col, 2, i1 - i3 - t, -i2 +i3 + t, i1 - i3 - t + o,-i2 + i3 + t, mag1, cntfix, cntfixnom);
            DL(f.g1, col, 2, i1 - i3 - t, -i2 - i3 - t, i1 - i3 - t + o,-i2 - i3 - t, mag1, cntfix, cntfixnom);
            DL(f.g1, col, 2, i1 + i3 + t, -i2 + i3 + t, i1 + i3 + t - o,-i2 + i3 + t, mag1, cntfix, cntfixnom);
            DL(f.g1, col, 2, i1 + i3 + t, -i2 - i3 - t, i1 + i3 + t - o,-i2 - i3 - t, mag1, cntfix, cntfixnom);

        }
        public void Cls(bool i1)
        {
            //pb = pictureBox1;     bmp1 = new Bitmap(pb.Width, pb.Height);           g1 = Graphics.FromImage(bmp1);              
            //                      bmp2 = new Bitmap(800, 800);                      g2 = Graphics.FromImage(bmp2);
            //pb1 = pictureBox2;    bmp_crop = new Bitmap(pb1.Width, pb1.Height);     g3 = Graphics.FromImage(bmp_crop);          
            //--------------------------------------------------------------------------------------------------------------
            //f.g1.FillRectangle(brfon, 0, 0, f.pb.Width, f.pb.Height);
            //f.g2.FillRectangle(brfon, 0, 0, 800, 800);

            f.g1.Clear(brfon.Color);
            f.g2.Clear(brfon.Color);

            if (i1)
            {
                f.pb.Image = f.bmp1;
                //f.g2.Clear(brfon.Color);
                f.g3.DrawImage(f.bmp2, f.pb1.Width / 2 - 400, f.pb1.Height / 2 - 400);
                f.pb1.Image = f.bmp_crop;
            }

            namemini = "";
        }
        public void DE(Graphics g, Color c, double i1, double i2, double i3, double mag, bool cf, int ncf)
        {
            br.Color = c;
            if (cf) { i1 -= p[ncf].x; i2 -= p[ncf].y; }
            i2 = -i2;
            if (mag == 1)   { q1 = 400 + (int)(i1 - i3);                            q2 = 400 + (int)(i2 - i3);                             q3 = (int)(i3 + i3); }
            else            { q1 = 400 + (int)(mag * (i1 - i3));                    q2 = 400 + (int)(mag * (i2 - i3));                     q3 = (int)(2 * mag * i3); }

            if (q3 < 2) q3 = 2;

            //if (q1 >= 0 && q2 >= 0 && q1 < 810 && q2 < 810)
                g.FillEllipse(br, q1, q2, q3, q3);
        }
        public void DC(Graphics g, Color c, int width, double i1, double i2, double i3, double mag, bool cf, int ncf)
        {
            pn.Color = c;
            pn.Width = width;

            if (cf) { i1 -= p[ncf].x; i2 -= p[ncf].y; }
            i2 = -i2;
            if (mag == 1) { q1 = 400 + (int)(i1 - i3); q2 = 400 + (int)(i2 - i3); q3 = (int)(i3 + i3); }
            else { q1 = 400 + (int)(mag * (i1 - i3)); q2 = 400 + (int)(mag * (i2 - i3)); q3 = (int)(2 * mag * i3); }

            if (q1 >= 0 && q2 >= 0 && q1 < 810 && q2 < 810)
                g.DrawEllipse(pn, q1, q2, q3, q3);

        }
        public void DR(Graphics g, Color c, double i1, double i2, double i3, double mag, bool cf, int ncf)
        {
            br.Color = c;

            if (cf) { i1 -= p[ncf].x; i2 -= p[ncf].y; }
            i2 = -i2;
            if (mag == 1) { q1 = 400 + (int)(i1 - i3); q2 = 400 + (int)(i2 - i3);}
            else { q1 = 400 + (int)(mag*i1 - i3); q2 = 400 + (int)(mag*i2 - i3);}
            q3 = (int)(i3 + i3);

            if (q1 >= 0 && q2 >= 0 && q1 < 810 && q2 < 810)
                g.FillRectangle(br, q1, q2, q3, q3);
        }
        public void DL(Graphics g, Color c, int width, double i1, double i2, double i3, double i4, double mag, bool cf, int ncf)
        {
            pn.Color = c;
            pn.Width = width;


            if (cf) { i1 -= p[ncf].x; i2 -= p[ncf].y; i3 -= p[ncf].x; i4 -= p[ncf].y; }
            i2 = -i2; i4 = -i4;
            if (mag == 1) { q1 = 400 + (int)i1; q2 = 400 + (int)i2; q3 = 400 + (int)i3; q4 = 400 + (int)i4;}
            else { q1 = 400 + (int)(i1 * mag); q2 = 400 + (int)(i2 * mag); q3 = 400 + (int)(i3 * mag); q4 = 400 + (int)(i4 * mag); }
            if (q1>=0 && q2>=0 && q1<810 && q2<810)
                g.DrawLine(pn, q1, q2, q3, q4);
        }
        public void DS(Graphics g, Color c, double i1, double i2,                                           string name, double mag, bool cf, int ncf)
        {
            if (cf) { i1 -= p[ncf].x; i2 -= p[ncf].y; }
            i2 = -i2;
            if (mag == 1) { q1 = 380 + (int)i1; q2 = 400 + (int)i2; }
            else { q1 = 380 + (int)(mag * i1); q2 = 400 + (int)(mag * i2); }

            g.DrawString(name, new Font("Tahoma", 8), ((brfon.Color == Color.White) ? Brushes.Black : Brushes.White), q1, q2);
        }
    }
}
