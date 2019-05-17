using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Planet
    {
        double x, y, x1, y1;
        double vx, vy, vx1, vy1;
        double ax, ay, ax1, ay1;
        int r;
        int m;
        SolidBrush br;
        Pen p;
        int nom;
        public int l;
        int k;
        Kosmos K;
        Form1 f;


        //double R, a, ax, ay;

        public Planet(Form1 i1, Kosmos i2, int i3)
        {
            f = i1;
            K = i2;
            nom = i3;
            x = y = 10;
            Recreate();

        }

        public void Recreate()
        {
            Color col;
            x = f.rand.Next(-150, 150);
            y = f.rand.Next(-150, 150);
            vx = K.vm * (f.rand.NextDouble() - 0.5);
            vy = K.vm * (f.rand.NextDouble() - 0.5);
            ax = 0;
            ay = 0;
            r = f.rand.Next(2, 4);
            m = (int)(K.mP * f.rand.NextDouble());
            //if (nom == 0) { x = 1; y = 1; vx = 0;vy = 0;r = 10;m = 10; }
            //br = new SolidBrush(Color.FromArgb(f.rand.Next(10, 250), f.rand.Next(10, 250), f.rand.Next(10, 250)));

            col = Color.FromArgb(f.rand.Next(10, 250), f.rand.Next(10, 250), f.rand.Next(10, 250));
            br = new SolidBrush(col);
            p = new Pen(col, 2);
            x1 = x; y1 = y; vx1 = vx; vy1 = vy; ax1 = ax; ay1 = ay;
            l = 0;
            k = 0;
        }


        public void Move()
        {
            if (k > 0) return;

            x1 = x; y1 = y; vx1 = vx; vy1 = vy; ax1 = ax; ay1 = ay;

            vx += ax;
            vy += ay;
            x += vx;
            y += vy;
            l++;

            if (x >= 300 - r || x <= -300 + r || y >= 300 - r || y <= -300 + r) // если ударилась в край
            {
                switch (K.bt)
                {
                    case 0:
                        Kill(20);
                        break;

                    case 1:
                        if (x >= 300 - r) { vx = -vx; x = 300 - r; }
                        if (x <= -300 + r) { vx = -vx; x = -300 + r; }
                        if (y >= 300 - r) { vy = -vy; y = 300 - r; }
                        if (y <= -300 + r) { vy = -vy; y = -300 + r; }
                        break;

                    
                }
            }

        }

        public void Gravity()
        {
            if (k > 0) return;

            double R, a;

            ax = 0;
            ay = 0;

            //---------------------притяжение к центру
            R = Math.Sqrt(y * y + x * x);  //расстояние между центрами объектов  - делается по 2 раза
            a = K.mE / R / R;  //расчет ускорения - делается по 2 раза
            ax -= a * x / R;
            ay -= a * y / R;
            if (R < K.rE) Kill(20);   //удар в Землю
            //-------------------------------------------------

            //----------------------притяжение к другим объектам
            //if (l<=5000)
                for (int j = 0; j < K.nObj; j++)
                    if (j != nom && K.p[j].k == 0)
                    {
                        R = Math.Sqrt(Math.Pow(y - K.p[j].y, 2) + Math.Pow(x - K.p[j].x, 2));  //расстояние между центрами объектов
                        a = K.p[j].m / R / R;  //расчет ускорения
                        ax -= a * (x - K.p[j].x) / R;
                        ay -= a * (y - K.p[j].y) / R;
                        if (R < r + K.p[j].r - 2) //удар между объектами
                            if (l < K.p[j].l) Kill(20); else K.p[j].Kill(20);
                    }
            //-------------------------------------------------
        }


        public void Show()
        {
            if (k == 0)
                if (l > 5000)
                    f.g1.FillEllipse(br, 300 + (int)x - r, 300 + (int)y - r, 2 * r, 2 * r);
                else
                    switch (K.young)
                    {
                        case 0: break;
                        case 1: f.g1.FillRectangle(Brushes.Black, 300 + (int)x - 1, 300 + (int)y - 1, 2, 2); break;
                        case 2: f.g1.DrawEllipse(p, 300 + (int)x - r, 300 + (int)y - r, 2 * r, 2 * r); break;
                        case 3: f.g1.FillEllipse(br, 300 + (int)x - r, 300 + (int)y - r, 2 * r, 2 * r); break;
                    }
            if (k == 1) Recreate();

            if (k > 1)
            {
                if (l > 5000) f.g1.FillEllipse(new SolidBrush(Color.Red), 300 + (int)x - 20, 300 + (int)y - 20, 40, 40);
                else f.g1.FillEllipse(new SolidBrush(Color.Red), 300 + (int)x - k / 2, 300 + (int)y - k / 2, k, k);

                k--;
            }

        }




        private void Kill(int i1)
        {
            k = i1;
            K.btk++;
            if (l > 5000) { K.ok++; if (K.ap == 1) f.t.Enabled = false; }
            vx = 0;
            vy = 0;
        }

        public void Step()
        {
            Show();
            Move();
        }  // for  BackgroundWorker
        public void ShowT()
        {
            if (K.era == 1) f.g.FillEllipse(K.brfon, 200 + (int)x1 - r, 200 + (int)y1 - r, 2 * r, 2 * r);
            f.g.FillEllipse(br, 200 + (int)x - r, 200 + (int)y - r, 2 * r, 2 * r);
        }   // for  BackgroundWorker
        public void Move1()
        {
            double t;
            x1 = x;
            y1 = y;
            ax1 = ax;
            ay1 = ay;

            x += vx;
            y += vy;

            t = K.mE / Math.Pow(y * y + x * x, 1.5);
            ax = -t * x;
            ay = -t * y;

            vx += ax;
            vy += ay;


            if (x >= 200 - r || x <= -200 + r || y >= 200 - r || y <= -200 + r)
            {
                switch (K.bt)
                {
                    case 1:
                        if (x >= 200 - r) { vx = -vx; x = 200 - r; }
                        if (x <= -200 + r) { vx = -vx; x = -200 + r; }
                        if (y >= 200 - r) { vy = -vy; y = 200 - r; }
                        if (y <= -200 + r) { vy = -vy; y = -200 + r; }
                        break;

                    case 2:
                        K.btk++;
                        Recreate();
                        break;
                }
            }

            if (K.mE / t < 50000)
            {
                K.btk++;
                Recreate();
            }
        }   //старый вариант

    }
}



