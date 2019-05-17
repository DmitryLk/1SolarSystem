using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
//using System.ValueTuple;


namespace WindowsFormsApplication1
{
    public class Planet_dop
    {
        public double omega;
        public double w;
        public double M0;
        public double inakl;
        public double a;
        public double eks;
        public string name;
        public string info;
        public int type;
        public List<int> sat; // = new List<int>();

        //public double x, y; 
        //private double vx, vy;
        //public double ax, ay;
        //public double r;
        //private double m;
    }


    [Serializable()]
    public class Planet
    {
        Form1 f;
        Space K;
        public Planet_dop dop;


        

        //private (int, int) xyz = (5, 10);
        //(int, int) xyr = (5,10);

        public double x, y;
        //public (double, double) xy;
        public int q1,w1;
        private double vx, vy;
        public double ax, ay;
        public double axs, ays;

        public double r;
        private double m, gm;

        [XmlIgnore] public Color col;



        [XmlAttribute]
        public string ColorHtml
        {
            get { return ColorTranslator.ToHtml(this.col); }
            set { this.col = ColorTranslator.FromHtml(value); }
        }



        public int nom;
        public int l;
        public byte k;
        public bool old;
        public bool newborn;
        public int satof;



        public Planet() {}

        public Planet(Space i2, int i3)
        {
            K = i2;
            f = K.f;
            nom = i3;                       
            newborn = true;
            m = 0;
        }

        //[OnDeserializing()]
        //internal void OnDeserializingMethod(StreamingContext context) { Trace(); }


        public void Recreate()
        {
            
            if (nom == 0 && K.ts == 1)  //for real sun
            {
                l = 10000;                  
                old = true;                 

                SetXY(0, 0);                
                Vx = Vy = 0;                
                M = K.mS;                   
                r = K.rS;                   
                if (K.rst == 1) CalcR();    
                col = K.colsun;               
            }
            else
            {
                l = 0;                      
                old = false;                
                CalcXY();                   
                CalcV();                    
                CalcM();                    
                CalcR();                    
                col = Color.FromArgb(f.rand.Next(10, 250), f.rand.Next(10, 250), f.rand.Next(10, 250));     
            }
            k = 0;
            ax = 0;
            ay = 0;
         }

        //=========================================================

        public bool IsVisible()
        {
            double dx, dy;

            dx = x;
            dy = y;
            if (K.cntfix) { dx -= K.p[K.cntfixnom].x; dy -= K.p[K.cntfixnom].y; }
            if (Math.Abs(dx * K.mag1) > 400 || Math.Abs(dy * K.mag1) > 400) return false;

            if (dop != null)
            {
                if (!K.objviz[dop.type - 1]) return false;
                if (dop.type == 3)
                    if (K.allsat == false)
                    {
                        if (!K.cntfix) return false;
                        if (satof != K.cntfixnom && satof != K.p[K.cntfixnom].satof) return false;
                    }
                if (dop.type == 5)
                    if (K.asttyp != 0)
                        if (K.asttyptxt != dop.info) return false;
                if (K.objsizlim[dop.type - 1] != 0) if (r < K.objsizlim[dop.type - 1] * 1000 / K.diffR) return false;
            }

            return true;

        }

        public void Show()
        {
            Color c;

            if (l>=0)
            {
                if (old)
                {
                    c = col;
                    //if (dop != null)
                    {
                        if (dop?.type == 4) c = Color.Red;
                        if (dop?.type == 5) c = Color.LightBlue;
                    } 



                    K.DE(f.g1, c, x, y, r * K.objsizmag[(dop == null)?0:dop.type-1], K.mag1, K.cntfix, K.cntfixnom);

                    //g.SmoothingMode = SmoothingMode.AntiAlias;
                    //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    if (K.pname)
                        if (dop != null)
                                if (K.objnam[dop.type - 1] == true) K.DS(f.g1, col, x, y - 1.05 * r * K.objsizmag[(dop == null) ? 0 : dop.type - 1], dop.name, K.mag1, K.cntfix, K.cntfixnom); 
                }
                else
                    switch (K.young)
                    {
                        case 0: break;
                        case 1: K.DR(f.g1, Color.Black, x, y, 0.5, K.mag1, K.cntfix, K.cntfixnom); break;
                        case 2: K.DC(f.g1, col, 1, x,y, r * K.objsizmag[(dop == null) ? 0 : dop.type-1], K.mag1, K.cntfix, K.cntfixnom); break;
                        case 3: K.DE(f.g1, col, x,y, r * K.objsizmag[(dop == null) ? 0 : dop.type-1], K.mag1, K.cntfix, K.cntfixnom); break;
                    }

                if (K.mini)
                    if (Math.Abs(x - K.p[K.fixnom].x) < 400 / K.mag2 / K.mag1 && Math.Abs(y - K.p[K.fixnom].y) < 400 / K.mag2 / K.mag1)
                        K.DE(f.g2, col, x, y, r * K.objsizmag[(dop == null) ? 0 : dop.type-1], K.mag2 * K.mag1, true, K.fixnom);
            }
            else
            {
                if (k < 20)
                {
                    if (K.vzt == 0) K.DE(f.g1, Color.Red, x, y, K.vzryv[k], K.mag1, K.cntfix, K.cntfixnom);
                    if (K.mini) if (Math.Abs(x - K.p[K.fixnom].x) < 400 / K.mag2 / K.mag1 && Math.Abs(y - K.p[K.fixnom].y) < 400 / K.mag2 / K.mag1)
                        {
                            K.DE(f.g2, col, x, y, r * K.objsizmag[(dop == null) ? 0 : dop.type-1], K.mag2 * K.mag1,true, K.fixnom);
                            K.DC(f.g2, Color.Red, 2, x, y, r * K.objsizmag[(dop == null) ? 0 : dop.type-1], K.mag2 * K.mag1,true, K.fixnom);
                        }
                }
            }
        }

        public void SpeedVector()
        {
            double xj1, yj1;
            double am = Math.Pow(10, K.accmag - 3);

            xj1 = x + am * vx;
            yj1 = y + am * vy;
            K.DL(f.g1, Color.Green, 4, x, y, xj1, yj1, K.mag1, K.cntfix, K.cntfixnom);

            if (K.mini) if (Math.Abs(x - K.p[K.fixnom].x) < 400 / K.mag2 / K.mag1 && Math.Abs(y - K.p[K.fixnom].y) < 400 / K.mag2 / K.mag1)
                    K.DL(f.g2, Color.Green, 4, x, y, xj1, yj1, K.mag2 * K.mag1, true, K.fixnom);
        }
        public void AccVector()
        {
            double xj1, yj1, xjs, yjs;
            double am = Math.Pow(10, K.accmag);
            xj1 = x + am * (ax - axs);
            yj1 = y + am * (ay - ays);
            xjs = x + am * axs;
            yjs = y + am * ays;

            K.DL(f.g1, Color.Pink, 4, x, y, xj1, yj1, K.mag1, K.cntfix, K.cntfixnom);
            K.DL(f.g1, Color.Red, 4, x, y, xjs, yjs, K.mag1, K.cntfix, K.cntfixnom);

            if (K.mini) if (Math.Abs(x - K.p[K.fixnom].x) < 400 / K.mag2 / K.mag1 && Math.Abs(y - K.p[K.fixnom].y) < 400 / K.mag2 / K.mag1)
                {
                    K.DL(f.g2, Color.Pink, 4, x, y, xj1, yj1, K.mag2 * K.mag1, true, K.fixnom);
                    K.DL(f.g2, Color.Red, 4, x, y, xjs, yjs, K.mag2 * K.mag1, true, K.fixnom);
                }

        }
        public void Orbita(Color col)
        {
            Color c;
            double a, e, ft, ff, r, v, T, fi, cosf, cosfi, napr, nu, rj, marj, aj = 0;
            double xj1, yj1, xj2 = 0, yj2 = 0;
            double bord;

            double dx, dy, dvx, dvy;


            c = col;
            //if (dop != null)
            {
                if (dop?.type == 4) c = Color.Red;
                if (dop?.type == 5) c = Color.LightBlue;
            }

            dx = x - K.p[satof].x;
            dy = y - K.p[satof].y;
            dvx = vx - K.p[satof].vx;
            dvy = vy - K.p[satof].vy;

            nu = 0;

            r = hyp(dx, dy);
            v = hyp(dvx, dvy);


            if (satof == 0 && K.ts == 0) nu = K.Gg * (K.mS + m);
            else nu = K.Gg * (K.p[satof].m + m);




            a = 2 * nu / r - v * v;
            if (v == 0 || a == 0) return;
            ft = Math.Atan2(y - K.p[satof].y, x - K.p[satof].x);                   //реальный угол планеты
            a = nu / a;                                                           //большая полуось
            cosfi = -(dx * dvx + dy * dvy) / v / r;                                //косинус угла между радиус вектором и вектором скорости
            e = Math.Sqrt(1 - r / a / a * (2 * a - r) * (1 - cosfi * cosfi));       //эксцентриситет
            fi = Math.Acos(cosfi);                                                  //угол между радиус вектором и вектором скорости
            napr = dx * dvy - dy * dvx;                                                 //направление по часовой стрелке
            cosf = (a * (1 - e * e) / r - 1) / e;                                   //косинус истинной аномалиии
            ff = Math.Acos(cosf);                                                   //истинная аномалия
            if (napr * cosfi > 0) ff = -ff;                                         //истинная аномалия
            T = 2 * Math.PI * Math.Sqrt(a * a * a / nu);                          //период обращения
            //E = Math.Acos((1 - r / a) / e);                                       //эксцентричная аномалия
            // = 2 * Math.Atan(Math.Sqrt((1 + e) / (1 - e)) * Math.Tan(E / 2));   //расчет истинной аномалии(f) через E

            if (nom == K.fixnom) if (K.cntprint == K.cs - 1)
                {


                    f.label43.Text = "r:" + r.ToString("N2");
                    f.label44.Text = "v:" + v.ToString("N2");
                    f.label45.Text = "a:" + a.ToString("N2");
                    f.label46.Text = "T:" + T.ToString("N2");
                    f.label47.Text = "e:" + e.ToString("N2");
                    f.label49.Text = "f:" + ((ff >= 0 ? ff : ff + 2 * Math.PI) / Math.PI * 180).ToString("N2");
                    f.label51.Text = "fi:" + ((fi >= 0 ? fi : fi + 2 * Math.PI) / Math.PI * 180).ToString("N2");
                    f.label52.Text = "ft:" + ((ft >= 0 ? ft : ft + 2 * Math.PI) / Math.PI * 180).ToString("N2");
                    f.label53.Text = "cosf:" + cosf.ToString("N2");
                    f.label54.Text = "cosfi:" + cosfi.ToString("N2");


                    if (K.printreal)
                    {
                        f.label90.Text = "Расст до центра:" + (r * K.diffR / 1000).ToString("N2") + " км";
                        f.label92.Text = "Скорость:" + (v * K.diffR / K.diffT).ToString("N2") + " м/c";
                        f.label94.Text = "Большая полуось:" + (a * K.diffR / 1000).ToString("N2") + " км";



                        f.label93.Text = "Период обращения:" + (T * K.diffT / 3600 / 24).ToString("N2") + " дн (" + (T * K.diffT / 3600 / 24 / 365).ToString("N2") + ") лет";
                    }




                }
            //f.label48.Text = "E:" + E.ToString("N2");
            //f.label50.Text = "f2:" + (f2 / Math.PI * 180).ToString("N2");
            //f.label55.Text = "напр:" + ((napr < 0) ? "против час." : "по час.");
            //f.label56.Text = "половина:" + ((napr * cosfi < 0) ? "1" : "2");

            //a, e, ft, ff
            //SolidBrush bro = new SolidBrush(col);
            //Pen brp = new Pen(col);
            bord = 400 / K.mag1;
            do
            {
                //xj1 = xj2; yj1 = yj2;
                rj = a * (1 - e * e) / (1 + e * Math.Cos(aj + ff));
                xj2 = rj * Math.Cos(aj + ft);
                yj2 = rj * Math.Sin(aj + ft);
                //if (Math.Abs(xj2) < bord && Math.Abs(yj2) < bord)
                K.DR(f.g1, c, xj2 + K.p[satof].x, yj2 + K.p[satof].y, .5, K.mag1, K.cntfix, K.cntfixnom);
                ////if (Math.Abs(xj) < 400 && Math.Abs(yj) < 400) f.g1.FillRectangle(bro, 400 + (int)xj, 400 + (int)yj, 2, 2);
                ////if (xj1!=0&&yj1!=0)
                ////if (Math.Abs(xj1) < 400 && Math.Abs(yj1) < 400 && Math.Abs(xj2) < 400 && Math.Abs(yj2) < 400) K.DL(f.g1, col, 1, xj1, -yj1, xj2, -yj2, K.mag1);


                marj = Math.Abs(rj) * K.mag1; if (marj < 10) marj = 10; if (marj > 400) marj = 400;
                aj += 2 / marj; // Math.Acos(1 - 1 / 2D / rj / rj);
            }
            while (aj <= 2 * Math.PI + .03);


            if (K.per1)
            {
                rj = a * (1 - e * e) / (1 + e);
                xj1 = rj * Math.Cos(-ff + ft);
                yj1 = rj * Math.Sin(-ff + ft);
                K.DL(f.g1, col, 1, K.p[satof].x, K.p[satof].y, xj1 + K.p[satof].x, yj1 + K.p[satof].y, K.mag1, K.cntfix, K.cntfixnom);
            }

            /*
            aj = 0;
            rj = a * (1 - e * e) / (1 + e * Math.Cos(aj + ff));
            xj = rj * Math.Cos(aj + ft);
            yj = rj * Math.Sin(aj + ft);
            K.DL(f.g1, Color.Black,1 , 0, 0, xj, -yj);
            */
        }
        public void Orbita2(Color col)
        {
            double a, e, rj, marj, aj = 0, w;
            double xj1, yj1, xj2 = 0, yj2 = 0;
            double bord;

            a = dop.a / K.diffR;
            //if (satof != 0) return;
            //if (a * K.mag1 > 500) return;
            e = dop.eks;
            w = dop.w;

            bord = 400 / K.mag1;
            do
            {
                //xj1 = xj2; yj1 = yj2;
                rj = a * (1 - e * e) / (1 + e * Math.Cos(aj));
                xj2 = rj * Math.Cos(aj + w);
                yj2 = rj * Math.Sin(aj + w);
                //if (Math.Abs(xj2) < bord && Math.Abs(yj2) < bord)
                K.DR(f.g1, col, xj2 + K.p[satof].x, yj2 + K.p[satof].y, .5, K.mag1, K.cntfix, K.cntfixnom);
                ////if (Math.Abs(xj) < 400 && Math.Abs(yj) < 400) f.g1.FillRectangle(bro, 400 + (int)xj, 400 + (int)yj, 2, 2);
                ////if (xj1 != 0 && yj1 != 0)
                ////if (Math.Abs(xj2) < bord && Math.Abs(yj2) < bord) K.DL(f.g1, col, 1, xj1, -yj1, xj2, -yj2, K.mag1);

                marj = Math.Abs(rj) * K.mag1; if (marj < 10) marj = 10; if (marj > 400) marj = 400;
                aj += 2 / marj; // Math.Acos(1 - 1 / 2D / rj / rj);
            }
            while (aj <= 2 * Math.PI + .03);


            if (K.per2)
            {
                rj = a * (1 - e * e) / (1 + e);
                xj1 = rj * Math.Cos(w);
                yj1 = rj * Math.Sin(w);
                K.DL(f.g1, col, 1, K.p[satof].x, K.p[satof].y, xj1 + K.p[satof].x, yj1 + K.p[satof].y, K.mag1, K.cntfix, K.cntfixnom);
            }
        }
        public void Shleif()
        {
            foreach (crd j in K.doskaShl)
            {
                K.DR(f.g1, Color.Red, j.x, j.y, 0.5, K.mag1, K.cntfix, K.cntfixnom);
            }
        }


        public void Move()
        {
            if (l >= 0)
            {
                l++; if (old==false) if (l > 5000) old = true;
                vx += ax * K.timecompress;
                vy += ay * K.timecompress;
                SetXY(x + vx * K.timecompress, y + vy * K.timecompress);
            }
            else   //if (l<0)
            {
                if (k < 20) k++;
                else   //if(k==20)
                {
                    if (K.rbrn == 0) Recreate();
                    else  //if(K.rbrn == 1)
                    {
                        if (newborn==false)
                        {
                            K.doska[q1 + 20, w1 + 20].Remove(nom);
                            newborn = true;
                        }
                    }
                } 
            }
        }
        public void Collision()
        {
            int q, w, q1, w1;
            double dy, dx, dr, R;
            q1 = (int)x / 20 + 20;
            w1 = (int)y / 20 + 20;
            for (q = (q1 - 1 < 0 ? 0 : q1 - 1); q <= (q1 + 1 > 39 ? 39 : q1 + 1); q++)
                for (w = (w1 - 1 < 0 ? 0 : w1 - 1); w <= (w1 + 1 > 39 ? 39 : w1 + 1); w++)
                {
                    foreach (int j in K.doska[q, w])
                    {
                        if (j != nom && K.p[j].l >= 0)
                        {
                            //if (q1 - 1 <= q && q <= q1 + 1) if (w1 - 1 <= w && w <= w1 + 1)
                            dr = r + K.p[j].r;
                            dy = K.p[j].y - y;
                            if (-dr <= dy && dy <= dr)
                            {
                                dx = K.p[j].x - x;
                                if (-dr <= dx && dx <= dr)
                                {
                                    R = hyp(dx, dy); if (R <= dr) { СтолкновениеС(j); f.label98.Text = dop?.name + " - " + K.p[j].dop?.name; }
                                }
                            }
                        }
                    }
                }
        }


        public void GravityTo(int j)
        {
            double dx, dy, aR;
            dy = K.p[j].y - y; dx = K.p[j].x - x;
            aR = Math.Pow(dy * dy + dx * dx, 3 / 2D);
            if (aR > 0) { aR = K.Gg * K.p[j].m / aR; ax += aR * dx; ay += aR * dy; }
        }
        public void GravityToCenter()
        {
            double R, aR;
            //ax1 = ax; ay1 = ay;

            R = hyp(x, y);
            if (R > 0)
            {
                aR = K.Gg * K.mS / R / R / R;
                ax -= aR * x; axs = ax;
                ay -= aR * y; ays = ay;
            }
            if (R < K.rS + r && K.rS > 0) Death();
        }
        public void GravityForSun()
        {
            for (int j = 1; j < K.nObj; j++) if (K.p[j].l >= 0) GravityTo(j);
        }
        public void Gravity1()
        {
            if (K.ts == 1) if (K.p[0].l >= 0) { GravityTo(0); axs = ax; ays = ay; }
        }
        public void Gravity2()
        {
            if (K.ts == 1) if (K.p[0].l >= 0) { GravityTo(0); axs = ax; ays = ay; }
            if (old == false) for (int j = K.ts; j < K.nObj; j++) if (j != nom && K.p[j].l >= 0 && K.p[j].old == true) GravityTo(j);
        }
        public void Gravity3()
        {
            if (K.ts == 1) if (K.p[0].l >= 0) { GravityTo(0); axs = ax; ays = ay; }
            for (int j = K.ts; j < K.nObj; j++) if (j != nom && K.p[j].l >= 0) GravityTo(j);
        }
        public void Gravity4()
        {
            if (K.ts == 1) if (K.p[0].l >= 0) { GravityTo(0); axs = ax; ays = ay; }

            int q, w, q1, w1;
            int t = K.newgrrad;

            q1 = (int)x / 20 + 20;
            w1 = (int)y / 20 + 20;

            for (q = (q1 - t < 0 ? 0 : q1 - t); q <= (q1 + t > 39 ? 39 : q1 + t); q++)
                for (w = (w1 - t < 0 ? 0 : w1 - t); w <= (w1 + t > 39 ? 39 : w1 + t); w++)
                    foreach (int j in K.doska[q, w]) if (j != nom && K.p[j].l >= 0 && K.p[j].m < 1) GravityTo(j);

            foreach (int j in K.doskaM1) if (j != nom && K.p[j].l >= 0) GravityTo(j);
        }
        public void Gravity5()
        {
            if (K.ts == 1) if (K.p[0].l >= 0) { GravityTo(0); axs = ax; ays = ay; }
            if (satof > 0) GravityTo(satof);
        }
        public void Gravity6()
        {
            double dx, dy, aR;
            //if (satof == 6) f.prob(0);


            dy = K.p[0].y - y; dx = K.p[0].x - x;
            aR = K.p[0].gm / Math.Pow(dy * dy + dx * dx, 3 / 2D);
            ax = aR * dx; ay = aR * dy;
            axs = ax; ays = ay;

            if (satof > 0)
            {
                dy = K.p[satof].y - y; dx = K.p[satof].x - x;
                aR = K.p[satof].gm / Math.Pow(dy * dy + dx * dx, 3 / 2D);
                ax += aR * dx; ay += aR * dy;

                if (K.p[satof].dop.sat!=null)
                foreach (int j in K.p[satof].dop.sat)
                {
                        if (j != nom)
                        {
                            dy = K.p[j].y - y; dx = K.p[j].x - x;
                            aR = K.p[j].gm / Math.Pow(dy * dy + dx * dx, 3 / 2D);
                            ax += aR * dx; ay += aR * dy;
                        }
                }
                
            }

            if (K.jup)
            {
                if (dop.type == 5) if (dop.info == "TJN")
                    {
                        dy = K.p[5].y - y; dx = K.p[5].x - x;
                        aR = K.p[5].gm / Math.Pow(dy * dy + dx * dx, 3 / 2D);
                        ax += aR * dx; ay += aR * dy;
                    }
            }

            //if (satof == 6) f.prob();
        }
        //=============================================================================


        public double Vx
        {
            get { return vx; }
            set
            {
                vx = value;
            }
        }
        public double Vy
        {
            get { return vy; }
            set
            {
                vy = value;
            }
        }
        public double M
        {
            get { return m; }
            set
            {
                m = value;
                if (K!=null)
                {
                    if (m >= 1 && value < 1) K.doskaM1.Remove(nom);
                    if (m < 1 && value >= 1) K.doskaM1.Add(nom);
                    gm = m * K.Gg;
                }
            }
        }
        public double Gm
        {
            get { return gm; }
        }


        public void SetXY(double i1, double i2)
        {
            int q2, w2;
            if (nom == 0) if (K.ts == 1 && K.rsmt == 0 && newborn==false) return;  //солнце не двигается

            if (newborn==false)
            {
                q2 = R(i1 / 20);
                w2 = R(i2 / 20);
                if (q1 != q2 || w1 != w2)
                {
                    if (q2 < -20) if (K.bt == 1) { x = -399.999999999; q2 = -20; Vx = -vx; return; } else { Death(); return; }
                    if (q2 > 19) if (K.bt == 1) { x = 399.999999999; q2 = 19; Vx = -vx; return; } else { Death(); return; }
                    if (w2 < -20) if (K.bt == 1) { y = -399.999999999; w2 = -20; Vy = -vy; return; } else { Death(); return; }
                    if (w2 > 19) if (K.bt == 1) { y = 399.999999999; w2 = 19; Vy = -vy; return; } else { Death(); return; }

                    K.doska[q1+20, w1+20].Remove(nom);
                    q1 = q2;
                    w1 = w2;
                    K.doska[q1+20, w1+20].Add(nom);
                }
            }
            else
            {
                q1 = R(i1 / 20);
                w1 = R(i2 / 20);
                K.doska[q1+20, w1+20].Add(nom);
                newborn = false;
            }

            if (K.shleif) if (nom == K.fixnom) //if ((int)x != (int)i1 || (int)y != (int)i2)
            {
                K.tempcrd.x = i1;
                K.tempcrd.y = i2;
                K.doskaShl.Enqueue(K.tempcrd);
                if (K.doskaShl.Count >= 5000) K.doskaShl.Dequeue();
            }

            x = i1;
            y = i2;
        }
        private void CalcXY()
        {
            double x1, y1, h;
            //double ang, r;
            //ang = f.rand.NextDouble()*Math.PI*2;
            //r = f.rand.NextDouble() * 250 + 10;
            //x = r * Math.Sin(ang);
            //y = r * Math.Cos(ang);

            switch (K.vt)
            {
                case 0:
                case 1:
                        do
                        {
                            x1 = f.rand.Next(-400, 400);
                            y1 = f.rand.Next(-400, 400);
                            h = hyp(x1, y1);
                        }
                        while (h < K.rx1 || h >= K.rx2);
                        SetXY(x1,y1); break;
                case 2: SetXY(f.rand.Next(-10, 10) + K.p[K.fixnom].x, f.rand.Next(-10, 10) + K.p[K.fixnom].y); break;
            }
        }
        private void CalcV()
        {
            double v1,ang,v,dx,dy;
            
            //vy = vy;   

            switch (K.vt)
            {
                case 0:
                        //v2 = Math.Sqrt(2 * K.mS / hyp(x,y));
                        v = f.rand.NextDouble() * (K.vP2 - K.vP1) + K.vP1;
                        //v = (v > v2) ? v2 : v;              
                        ang = f.rand.NextDouble() * 2 * Math.PI;
                        Vx = v * Math.Sin(ang);
                        Vy = v * Math.Cos(ang);
                        break;
                case 1:
                        ang = Math.Atan2(x, y) + Math.PI / 2;
                        v1 = Math.Sqrt(K.Gg*K.mS / hyp(x, y));
                        Vx = v1 * Math.Sin(ang);
                        Vy = v1 * Math.Cos(ang);
                        break;
                case 2:
                        dx = x - K.p[K.fixnom].x;
                        dy = y - K.p[K.fixnom].y;
                        ang = Math.Atan2(dx, dy) + Math.PI / 2;
                        v1 = Math.Sqrt( K.p[K.fixnom].m   / hyp(dx, dy));
                        Vx = v1 * Math.Sin(ang)+ K.p[K.fixnom].vx;
                        Vy = v1 * Math.Cos(ang) + K.p[K.fixnom].vy;
                        //K.vt = 0;
                        break;
            }
        }
        public void RotateV(int i1)
        {
            double ang, v;

            ang = Math.Atan2(vx, vy);
            ang += (double)i1 / 100;
            v = hyp(vx, vy);
            Vx = v * Math.Sin(ang);
            Vy = v * Math.Cos(ang);

        }
        public void ChangeV(int i1)
        {
            Vx += (double)i1 / 100D * vx;
            Vy += (double)i1 / 100D * vy;
        }
        private void CalcM()
        {
            switch (K.mt)
            {
                case 0: M = f.rand.NextDouble() * (K.mP2 - K.mP1) + K.mP1; break;
                case 1: M = K.mP1 * Math.Pow(K.mP2 / K.mP1, f.rand.NextDouble()); break;
                case 2: M = K.mP1 * Math.Pow(K.mP2 / K.mP1, Math.Pow(f.rand.NextDouble(), 2)); break;
                case 3: M = K.mP1 * Math.Pow(K.mP2 / K.mP1, Math.Pow(f.rand.NextDouble(), 3)); break;
                case 4: M = K.mP1 * Math.Pow(K.mP2 / K.mP1, Math.Pow(f.rand.NextDouble(), 10)); break;
            }

        }
        public void CalcR()
        {
            switch (K.rt)
            {
                case 0: r = f.rand.NextDouble() * (K.rP2 - K.rP1) + K.rP1; break;
                case 1: r = K.vmult * m; break;
                case 2: r = K.vmult * Math.Pow(m, 1 / 3D); break;
            }
        }
        private double CalcEks()
        {
            double a, e, r, v, cosfi, nu;
            double dx, dy, dvx, dvy;

            dx = x - K.p[satof].x;
            dy = y - K.p[satof].y;
            dvx = vx - K.p[satof].vx;
            dvy = vy - K.p[satof].vy;

            nu = 0;

            r = hyp(dx, dy);
            v = hyp(dvx, dvy);

            if (satof == 0 && K.ts == 0) nu = K.Gg * (K.mS + m);
            else nu = K.Gg * (K.p[satof].m + m);

            a = 2 * nu / r - v * v;
            if (v == 0 || a == 0) return 5;
            a = nu / a;                                                           //большая полуось
            cosfi = -(dx * dvx + dy * dvy) / v / r;                                //косинус угла между радиус вектором и вектором скорости
            e = Math.Sqrt(1 - r / a / a * (2 * a - r) * (1 - cosfi * cosfi));       //эксцентриситет
            return (e);
        }
        private double CalcT()
        {
            double a, r, v, T, nu;
            double dx, dy, dvx, dvy;

            dx = x - K.p[satof].x;
            dy = y - K.p[satof].y;
            dvx = vx - K.p[satof].vx;
            dvy = vy - K.p[satof].vy;

            nu = 0;

            r = hyp(dx, dy);
            v = hyp(dvx, dvy);

            if (satof == 0 && K.ts == 0) nu = K.Gg * (K.mS + m);
            else nu = K.Gg * (K.p[satof].m + m);

            a = 2 * nu / r - v * v;
            if (v == 0 || a == 0) return 500000;
            a = nu / a;                                                           //большая полуось
            T = 2 * Math.PI * Math.Sqrt(a * a * a / nu);                          //период обращения
            return (T);
        }
        private void Death()
        {

            if (nom==K.fixnom)
            {
                if (K.ap == 1)
                {
                    K.PAUSE = true;
                    K.ShowK();
                    K.fixnom = nom;
                }
            }

            l = -1;
            M = 0;
            K.btk++;


            if (old)
            {
                K.ok++;
                if (K.ap == 1)
                {
                    K.PAUSE = true;
                    K.ShowK();
                    K.fixnom = nom;
                }
            }
        }
        private void СтолкновениеС(int i1)
        {
            switch (K.stt)
            {
                case 0: if (l < K.p[i1].l) Death(); else K.p[i1].Death(); break;
                case 1: if (m < K.p[i1].m) Death(); else K.p[i1].Death(); break;
                case 2:
                        //f.label50.ForeColor = br.Color;
                        //f.label56.ForeColor = br.Color;
                        //f.label55.ForeColor = K.p[i1].br.Color;
                        //f.label59.ForeColor = K.p[i1].br.Color;
                        //f.label50.Text = nom + "-" + l + "-" + k + "-" + old + "-" + CalcEks().ToString("N2") + "-" + (int)CalcT();
                        //f.label55.Text = K.p[i1].nom + "-" + K.p[i1].l + "-" + K.p[i1].k + "-" + K.p[i1].old + "-" + K.p[i1].CalcEks().ToString("N2") + "-" + (int)K.p[i1].CalcT();
                        if (CalcEks() > K.p[i1].CalcEks()) Death(); else K.p[i1].Death();
                        //f.label56.Text = nom + "-" + l + "-" + k + "-" + old + "-" + CalcEks().ToString("N2") + "-" + (int)CalcT();
                        //f.label59.Text = K.p[i1].nom + "-" + K.p[i1].l + "-" + K.p[i1].k + "-" + K.p[i1].old + "-" + K.p[i1].CalcEks().ToString("N2") + "-" + (int)K.p[i1].CalcT();
                        break;
                case 3:
                        //f.label50.ForeColor = br.Color;
                        //f.label56.ForeColor = br.Color;
                        //f.label55.ForeColor = K.p[i1].br.Color;
                        //f.label59.ForeColor = K.p[i1].br.Color;
                        //f.label50.Text = nom + "-" + l + "-" + k + "-" + old + "-" + CalcEks().ToString("N2") + "-" + (int)CalcT();
                        //f.label55.Text = K.p[i1].nom + "-" + K.p[i1].l + "-" + K.p[i1].k + "-" + K.p[i1].old + "-" + K.p[i1].CalcEks().ToString("N2") + "-" + (int)K.p[i1].CalcT();
                        //old убьется только если другая пролетела круг полный
                        if (CalcEks() > K.p[i1].CalcEks())
                            if (old)
                                if (K.p[i1].l >= K.p[i1].CalcT()) Death();
                                else K.p[i1].Death();
                            else Death();
                        else
                            if (K.p[i1].old)
                                if (l >= CalcT()) K.p[i1].Death();
                                else Death();
                            else K.p[i1].Death();
                        //f.label56.Text = nom + "-" + l + "-" + k + "-" + old + "-" + CalcEks().ToString("N2") + "-" + (int)CalcT();
                        //f.label59.Text = K.p[i1].nom + "-" + K.p[i1].l + "-" + K.p[i1].k + "-" + K.p[i1].old + "-" + K.p[i1].CalcEks().ToString("N2") + "-" + (int)K.p[i1].CalcT();
                        break;
                case 4: if (l < K.p[i1].l)
                        {
                            K.p[i1].M = m + K.p[i1].m;
                            K.p[i1].CalcR();
                            Death();
                        }
                        else
                        {
                            M = m + K.p[i1].m;
                            CalcR();
                            K.p[i1].Death();
                        }
                        break;                                                  
                case 5:
                        //f.label50.ForeColor = br.Color;
                        //f.label56.ForeColor = br.Color;
                        //f.label55.ForeColor = K.p[i1].br.Color;
                        //f.label59.ForeColor = K.p[i1].br.Color;
                        //f.label50.Text = nom + "-" + m + "-" + k + "-" + old + "-" + CalcEks().ToString("N2") + "-" + (int)CalcT();
                        //f.label55.Text = K.p[i1].nom + "-" + K.p[i1].m + "-" + K.p[i1].k + "-" + K.p[i1].old + "-" + K.p[i1].CalcEks().ToString("N2") + "-" + (int)K.p[i1].CalcT();
                    if (m < K.p[i1].m)
                        {

                            K.p[i1].M = m + K.p[i1].m;
                            K.p[i1].CalcR();
                        
                            Death();
                        }
                        else
                        {
                            M = m + K.p[i1].m;
                            CalcR();
                            K.p[i1].Death();
                        }
                        //f.label56.Text = nom + "-" + m + "-" + k + "-" + old + "-" + CalcEks().ToString("N2") + "-" + (int)CalcT();
                        //f.label59.Text = K.p[i1].nom + "-" + K.p[i1].m + "-" + K.p[i1].k + "-" + K.p[i1].old + "-" + K.p[i1].CalcEks().ToString("N2") + "-" + (int)K.p[i1].CalcT();
                        break;
                case 6: if (CalcEks() > K.p[i1].CalcEks())
                        {
                            K.p[i1].M = m + K.p[i1].m;
                            K.p[i1].CalcR();
                            Death();
                        }
                        else
                        {
                            M = m + K.p[i1].m;
                            CalcR();
                            K.p[i1].Death();
                        }
                        break;
                case 7: if (m < K.p[i1].m)
                        {
                            K.p[i1].Vx = (vx * m + K.p[i1].vx * K.p[i1].m) / (m + K.p[i1].m);
                            K.p[i1].Vy = (vy * m + K.p[i1].vy * K.p[i1].m) / (m + K.p[i1].m);
                            K.p[i1].M = m + K.p[i1].m;
                            K.p[i1].CalcR();
                            Death();
                        }
                        else
                        {
                            Vx = (vx * m + K.p[i1].vx * K.p[i1].m) / (m + K.p[i1].m);
                            Vy = (vy * m + K.p[i1].vy * K.p[i1].m) / (m + K.p[i1].m);
                            M = m + K.p[i1].m;
                            CalcR();
                            K.p[i1].Death();
                        }
                        break;
                case 8: break;
                case 9: break;                                             
                case 10: break;                                            
                case 11: break;                                            
                case 12: Death(); K.p[i1].Death(); break;                                            
                case 13: break;                                            
            }
            //0K1_young          0+
            //1K1_easy           1
            //2K1_ ^ eks         2+
            //3K1 y&e            3+
            //4Slip_young        4+
            //5Slip_easy         5+
            //6Slip_ ^ eks       6+
            //7Slip_2            7+
            //8Otskk_young       8
            //9Otskk_easy        9
            //10Otskk_ ^ eks      10
            //11Otskk_2           11
            //12K2                12
            //13None              13
        }
        public double hyp(double i1, double i2)
        {
            return Math.Sqrt(i1 * i1 + i2 * i2);
        }
        private int R(double d)
        {
            int i;
            i = (int)d;
            if (d < 0) i = i - 1;
            return i;
        }
        //========================================
    }
}



//========TO DO LIST=============
// =======ОСНОВНЫЕ===============

// - барицентр сделать
// - орбита харона реальная и начальная 
// - тритон неправильный перигелий
// - реальный fps
// - сжатие времени
// - шлейф спутников
// - кнопку плавного увеличения до размеров планеты
// - кнопку плавного увеличения до размеров орбиты фикс
// - сделать подвижное солнце
// - в таймере stop start делать, но надо замерить будет ли лаг перед следующим запуском таймера, оптимизировать ускорение?
// - Step Count-inue не работает

// - сделать предел количества циклов в таймере - 5000
// - неправильно расчитывается fps 65000/  а надо 1000000/64/
// - по движению мышки фиксироваться на планете
// - Cls ускорить
// - move ускорить
// - рисование ускорить
// - М предел по 1 переделать в gravity new ?
// - при сохранении если dop != null сохранять в файл и из допа
// - Solar system & graviti to all new - ошибка



// =======НЕ ОСНОВНОЕ============
// - длину шлейфа менять
// - реакция на бордюр None
// - get set  v
// - передний шлейф (какая будет траектория если все будет неподвижно)
// - корректно ли закрывается excel при save/load?
// - убрать двойное вычисление ускорения
// - менять цвет планеты
// - застревание в углах
// - error на доске больше 3000    
// - поубирать все new в повторяющихся функциях
// - столкновения обработать
// - мультимедиа таймер



// ======СДЕЛАНО=================
// + вставить комету
// + вставить спутники марса
// + возможность увеличения главного окна
// + TotalMemory
// + разлетаются планеты при V = orbit
// + в функции орбита калкт калкекс при вычисл а ню = G(m1+m2)
// + галочка стоп чтобы срабатывала
// + два вектора притяжения (к солнцу и остальным)
// + орбиты спутников
// + названия спутников убирать
// + вкладки
// + цвета планет, орбит при блэке
// + stop дату сделать
// + подпись солнца
// + в мини подпись вставить
// + подпись в зависимости от радиуса
// + подписи с большой буквы
// + поправить расчет орбитальной скорости при V=orbit
// + добавить за сколько секунд год пройдет
// + сэйвить счетчики?
// + убрать браш и пэн из планеты
// + запоминать траекторию отмеченного объекта и выводить его, может точек 500.
// + по кругу х дипапазон при орбитпльной скорости
// + get set  x y m
// + оптимизировать функцию гравитации
// + притяжение к массам более 1
// + экспоненциальный выбор масс
// + изменение радиуса
// + возраст показа орбит
// + display
// + общую массу системы
// + сделать тайминг в микросекундах
// + по эксцентриситету как то странно  
// + List<T>
// + при слипании неподвижное солнце двигается    
// + перемещение в паузе
// + в паузе менять направление с изменением орбиты
// + в паузе дорисовывать вектор скорости и ускорения
// + цветные орбиты    
// + new planet делать один раз
// + усовершестовать функцию орбиты
// + убрать движение реального солнца
// + чтоб при нажатиях стрелок сразу переключалось перекрестье
// + если young none то чтоб переключалось только по Old
// + gravity type натсроить
// + расчет эксцентриситета сделать
// + пробел - пауза





/*
 if (i1 >= 400 - r) if (K.bt == 1) { i1 = 400 - r; Vx = -vx; } else {Death(); return;}
 if (i1<= -400 + r)  if (K.bt == 1) { i1=-400+r; Vx = -vx; } else { Death(); return; }
 if (i2 >= 400 - r)  if (K.bt == 1) { i2 = 400 - r; Vy = -vy; } else { Death(); return; }
 if (i2 <= -400 + r) if (K.bt == 1) { i2 = -400 + r; Vy = -vy; } else { Death(); return; }
 
//br = new SolidBrush(Color.FromArgb(f.rand.Next(10, 250), f.rand.Next(10, 250), f.rand.Next(10, 250)));

            public void Step()
        {
            Show();
            Move();
        }  // for  BackgroundWorker
        public void ShowT()
        {
            //if (K.era == 1) f.g.FillEllipse(K.brfon, 200 + (int)x1 - (int)r, 200 + (int)y1 - (int)r, (int)(2 * r), (int)(2 * r));
            //f.g.FillEllipse(br, 200 + (int)x - (int)r, 200 + (int)y - (int)r, (int)(2 * r), (int)(2 * r));
        }   // for  BackgroundWorker
        public void Move1()
        {
            double t;
            //x1 = x; y1 = y; ax1 = ax; ay1 = ay;

            x += vx;
            y += vy;

            t = K.mS / Math.Pow(y * y + x * x, 1.5);
            ax = -t * x;
            ay = -t * y;

            vx += ax;
            vy += ay;


            if (x >= 200 - r || x <= -200 + r || y >= 200 - r || y <= -200 + r)
            {
                switch (K.bt)
                {
                    case 1:
                        if (x >= 200 - r) { Vx = -vx; x = 200 - r; }
                        if (x <= -200 + r) { Vx = -vx; x = -200 + r; }
                        if (y >= 200 - r) { Vy = -vy; y = 200 - r; }
                        if (y <= -200 + r) { Vy = -vy; y = -200 + r; }
                        break;

                    case 2:
                        K.btk++;
                        Recreate();
                        break;
                }
            }

            if (K.mS / t < 50000)
            {
                K.btk++;
                Recreate();
            }
        }   //старый вариант

*/
