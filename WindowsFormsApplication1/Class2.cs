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



    public class Kosmos
    {

        Form1 f;
        public Planet[] p = new Planet[1000];
        public int era;
        public int nObj;
        public Brush brfon;
        public Brush brsun;
        public double mE;
        public double rE;
        public double mP;
        public int bt;
        public int btk;
        public int ok; //old kill
        public long Cnt;
        public double vm;
        public int young;
        public int ap;
        public DateTime dt;
        public double diff;



        public Kosmos()
        {
            
        }

        public void InitializeKosmos(int TYP, Form1 i1, int cb1, int cb2, int cb3, int cb4, int cb9, int tb1, double tb2, double tb3, double tb5, double tb6)
        {

            if (TYP == 1)
            {
                f = i1;
                nObj = tb1;
                btk = 0;
                ok = 0;
                Cnt = 0;
                for (int j = 0; j < nObj; j++) p[j] = new Planet(f, this, j);
            }


            if (cb1 == 0) { brfon = Brushes.White; brsun = Brushes.Orange; }
            if (cb1 == 1) { brfon = Brushes.Black; brsun = Brushes.Orange; }

            Cls();

            mE = tb2;
            rE = tb5;
            mP = tb6;
            bt = cb9;
            era = cb2;
            young = cb3;
            ap = cb4;
            vm = tb3;   //max v


        }


        public void Cls()
        {
            f.g1.FillRectangle(brfon, 0, 0, f.pb.Width, f.pb.Height);
        }


        public void Step()
        {
            int j;
            if (era == 1)
            {
                f.g1.FillRectangle(brfon, 0, 0, f.pb.Width, f.pb.Height);
            }

            for (j = 0; j < nObj; j++) p[j].Show();  //bmp2 = bmp1;
            for (j = 0; j < nObj; j++) p[j].Move();
            for (j = 0; j < nObj; j++) p[j].Gravity();

            f.g1.FillEllipse(brsun, 270, 270, 60, 60);
            f.pb.Image = f.bmp1;


            if (++Cnt % 100 == 0)
            {
                int li = 0;
                for (j = 0; j < nObj; j++) if (p[j].l > 5000) li++;
                diff = (DateTime.Now - dt).TotalMilliseconds;
                f.label1.Text = "fps: " + (100 * 1000 / diff).ToString("#.##");
                f.label2.Text = "btk: " + btk + "-" + ok;
                f.label3.Text = "day:" + Cnt.ToString();
                f.label9.Text = "old:" + li;

                dt = DateTime.Now;
            }


        }

    }


}
