using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Numerics;

namespace kg6
{
    public partial class Form1 : Form
    {

        public List<Line> values;
        public List<string> mnog;
        float xMin, xMax, yMin, yMax;
        int k=0;
       

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
           
            for (int i = 0; i < values.Count; i++)
            {
                middle(values[i]);
            }

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            label1.Text = "Time "+elapsedTime;
            label2.Text = "" + k;
            
        }

        int l = 1;
        public void middle(Line line)
        {
            k++;
            
            Graphics g = CreateGraphics();
           // if (l == 1) { g.Clear(Color.Black); l++; }
           
           // g.DrawLine(Pens.White, new Point((line.getX1() + line.getX2()) / 2, (line.getY1() + line.getY2()) / 2), new Point((line.getX1() + line.getX2()) / 2, (line.getY1() + line.getY2()) / 2));

            if (Math.Sqrt((line.getX2() - line.getX1()) * (line.getX2() - line.getX1()) + (line.getY2() - line.getY1()) * (line.getY2() - line.getY1())) < 1)
                return;

            if((((line.getX1() >= xMin) && (line.getX1() <= xMax)) && ((line.getY1() >= yMin) && (line.getY1()) <= yMax)) && (((line.getX2() >= xMin) && (line.getX2() <= xMax)) && ((line.getY2() >= yMin) && (line.getY2()) <= yMax)))
            {
                g.DrawLine(Pens.LightGreen, line.getX1(), line.getY1(), line.getX2(), line.getY2());
                //Console.WriteLine("X1 "+ line.getX1()+" y1 "+ line.getY1()+" x2 "+ line.getX2()+ " y2 "+ line.getY2());
                return;
            }
            else
            {
               // return;
            }

            middle(new Line(line.getX1(), line.getY1(), (line.getX1() + line.getX2()) / 2, (line.getY1() + line.getY2()) / 2));
            middle(new Line((line.getX1() + line.getX2()) / 2, (line.getY1() + line.getY2()) / 2, line.getX2(), line.getY2()));
            //g.DrawRectangle(Pens.LightYellow, line.getX1(), line.getY1(), 1, 1);
            //g.DrawRectangle(Pens.LightSkyBlue, line.getX2(), line.getY2(), 1, 1);
            //g.DrawRectangle(Pens.LightSkyBlue, line.getX1(), line.getY1(), line.getX2(), line.getY2());

        }

           public void LBalg(Line line)
           {
               Graphics g = CreateGraphics();

               int  xx1, yy1, xx2, yy2, dx, dy, i;
               int[] p = new int[4];
               int[] q = new int[4];
               float[] t = new float[4];
               float t1, t2;

               dx = (int)line.getX2() - (int)line.getX1();
               dy = (int)line.getY2() - (int)line.getY1();
               p[0] = -dx;
               p[1] = dx;
               p[2] = -dy;
               p[3] = dy;
               q[0] = (int)line.getX1() - (int)xMin;
               q[1] = (int)xMax - (int)line.getX1();
               q[2] = (int)line.getY1() - (int)yMin;
               q[3] = (int)yMax - (int)line.getY1();

               for (i = 0; i < 4; i++)
               {
                   if (p[i] == 0)
                   {
                       Console.WriteLine("line is parallel to one of the clipping boundary");
                       if (q[i] >= 0)
                       {
                           if (i < 2)
                           {
                               if (line.getY1() < yMin)
                               {
                                   line.sety1(yMin); //нижняя гор.
                               }

                               if (line.getY2() > yMax) 
                               {
                                   line.sety2(yMax); //верхняя гор.
                               }

                               //g.DrawLine(Pens.White, line.getX1(), line.getY1(), line.getX2(), line.getY2());
                           }

                           if (i > 1)
                           {
                               if (line.getX1() < xMin)
                               {
                                   line.setX1(xMin); //левая вер.
                               }

                               if (line.getX2() > xMax)
                               {
                                   line.setX2(xMax); //правая вер.
                               }

                               //g.DrawLine(Pens.White, line.getX1(), line.getY1(), line.getX2(), line.getY2());
                           }
                       }
                   }
               }

               t1 = 0;
               t2 = 1;

               for (i = 0; i < 4; i++)
               {
                   Console.WriteLine("qi " +q[i]+ "     pi "+p[i]);

                       t[i] = (float) q[i] / p[i];

                   if (p[i] < 0)
                   {
                       if (t1 <= t[i])
                           t1 = t[i];
                   }
                   else
                   {
                       if (t2 > t[i])
                           t2 = t[i];
                   }
               }

               if (t1 < t2)
               {
                   xx1 = (int)line.getX1() + (int)t1 * p[1];
                   xx2 = (int)line.getX1() + (int)t2 * p[1];
                   yy1 = (int)line.getY1() + (int)t1 * p[3];
                   yy2 = (int)line.getY1() + (int)t2 * p[3];
                   g.DrawLine(Pens.Blue, xx1, yy1, xx2, yy2);

               }
           }

        float t0, t1;
        bool checkPQ(float p, float q)
        {
            t0 = 0;
            t1 = 1;
            if (p == 0) // прямая параллельна отсекающей стороне окна
            {
                if (q < 0) return false;            // отрезок не виден
                Console.WriteLine("paralel");
            }
            else
            {
                float r = (float)q / p; // r - точка пересечения с гранью окна
                Console.WriteLine("peresecg");
                if (p < 0)
                {
                    if (r > t1) return false;
                    else if (r > t0) t0 = r;
                }
                else
                {
                    if (r < t0) return false;
                    else if (r < t1) t1 = r;
                }
            }
            return true;
        }

        private bool Clip(Line line, Line window) // двумерное отсечение линии line окном window
        {
            float dx = line.x2 - line.x1, dy = line.y2 - line.y1;
            t0 = 0; t1 = 1;
            Console.WriteLine("1 " + (line.x1 - window.x1) + " 2 " + (window.x2 - line.x1) + " 3 " + (line.y1 - window.y1) + " 4 " + (window.y1 - line.y1));
            Console.WriteLine("1 " + checkPQ(-dx, line.x1 - window.x1) + " 2 " + checkPQ(dx, window.x2 - line.x1) + " 3 " + checkPQ(-dy, line.y1 - window.y1) + " 4 " + checkPQ(dy, window.y1 - line.y1));
            if (checkPQ(-dx, line.x1 - window.x1) &&
                checkPQ(dx, window.x2 - line.x1) &&
                checkPQ(-dy, line.y1 - window.y1) &&
                checkPQ(dy, window.y1 - line.y1))
            {
                Console.WriteLine("x " + line.getX1() + " y " + line.getY1() + " x2 " + line.getX2() + " y2 " + line.getY2());
                if (t1 < 1)
                {
                    line.x2 = (int)(line.x1 + t1 * dx);
                    line.y1 = (int)(line.y1 + t1 * dy);
                }
                if (t0 > 0)
                {
                    line.x1 = (int)(line.x1 + t0 * dx);
                    line.y1 = (int)(line.y1 + t0 * dy);
                }
                return true;
            }
            return false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch stopWatch2 = new Stopwatch();
            stopWatch2.Start();
            Graphics g = CreateGraphics();
            Console.WriteLine("" + values.Count);
            for (int i = 0; i < values.Count; i++)
            {
                LBalg(values[i]);
               //  g.DrawLine(Pens.LightBlue, new Point((int)line.getX1(), (int)line.getY1()), new Point((int)line.getX2(), (int)line.getY2()));
                // Console.WriteLine("x "+ values[i].getX1() + " y "+ values[i].getY1()+ " x2 " + values[i].getX2() + " y2 " + values[i].getY2());
              /*  if (!Clip(values[i], new Line(Convert.ToInt32(xMin),
                                        Convert.ToInt32(yMin),
                                        Convert.ToInt32(xMax),
                                        Convert.ToInt32(yMax)))) MessageBox.Show("Линия невидима!");
                else g.DrawLine(Pens.LightBlue, new Point((int)values[i].getX1(), (int)values[i].getY1()), new Point((int)values[i].getX2(), (int)values[i].getY2()));*/

            }
            //Thread.Sleep(60);
            stopWatch2.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch2.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            label3.Text = "Time " + elapsedTime;
            
        }

      

        public Form1()
        {
            InitializeComponent();

            values = new List<Line>();
            mnog = new List<string>();
            ReadFile(new StreamReader("input.txt"));

            xMin = values[values.Count-1].getX1();
            xMax = values[values.Count-1].getX2();
            yMin = values[values.Count-1].getY1();
            yMax = values[values.Count-1].getY2();

            values.RemoveAt(values.Count-1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
           
          
            for(int i =0; i< values.Count; i++)
            {
                e.Graphics.DrawLine(Pens.White, values[i].getX1(), values[i].getY1(), values[i].getX2(), values[i].getY2());
            }

             GraphicsPath path = new GraphicsPath();

             for (int i = 0; i < mnog.Count-1; i++){
                 var parts = mnog[i].Split(' ');
                 var parts2 = mnog[i+1].Split(' ');
                 path.AddLine(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts2[0]), Convert.ToInt32(parts2[1]));
               // Console.WriteLine("" + parts[0] + "    " + parts[1] + "    "+ parts2[0] + "    " + parts2[1]);
                //Console.WriteLine("SIZE " + (mnog.Count - 1));
            }
             e.Graphics.DrawPath(Pens.LightCoral, path);


            e.Graphics.DrawLine(Pens.Green, 20, 350, 20, 20);
            e.Graphics.DrawLine(Pens.Green, 20, 20, 10, 40);
            e.Graphics.DrawLine(Pens.Green, 20, 20, 30, 40);

            e.Graphics.DrawLine(Pens.Red, 20, 350, 400, 350);
            e.Graphics.DrawLine(Pens.Red, 400, 350, 380, 360);
            e.Graphics.DrawLine(Pens.Red, 400, 350, 380, 340);
            e.Graphics.DrawRectangle(Pens.Red, new Rectangle((int)xMin, (int)yMin, (int)xMax-50, (int)yMax-50));
        }

        int xn = 1000;
        int yn = 1000;
        public void ReadFile(TextReader tr)
        {
            
            string line;
            string str = tr.ReadLine();

            int n = Convert.ToInt32(str);

            while ((line = tr.ReadLine()) != null)
            {
                var parts = line.Split(' ');
                if (parts.Length > 4)
                {
                    for (int i=0; i < parts.Length-1; i+=2){
                        mnog.Add(parts[i] + " " + parts[i+1]);
                        //Console.WriteLine("" + parts[i] + "    " + parts[i+1] );
                    }
                    
                }
                values.Add(new Line(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3])));

            }
        }

      

        public void SH(List<Vector2> vec)
        {
            int j;
            Graphics g = CreateGraphics();
            //L - граница полуплоскости
            for (int i = 0; i <= vec.Count-1; i++ )
            {
              /*j = i+1;
              if( j == vec.Count )
                     j = 1;*/
              //верхняя грань
              if( (((vec[i].X >= xMin) && (vec[i].X <= xMax)) && ((vec[i].Y <= yMin) && (vec[i].Y <= yMax))) && (((vec[i+1].X >= xMin) && (vec[i+1].X <= xMax)) && ((vec[i+1].Y >= yMin) && (vec[i+1].Y <= yMax)))) 
                {
                    for(int k=(int)xMin; k<(int)(xMax); k++)
                    {
                        for(int l=(int)vec[i].Y; l<(int)(vec[i+1].Y); l++)
                        {
                            if (k == l)
                            {
                                Vector2 sovp = new Vector2();
                                sovp.X = k;
                                sovp.Y = l;
                                // return sovp;
                                g.DrawRectangle(Pens.Yellow,sovp.X+xn,sovp.Y+yn,3,3);
                            }
                        }
                    }
                } //нижняя
                else if ((((vec[i].X >= xMin) && (vec[i].X <= xMax)) && ((vec[i].Y >= yMin) && (vec[i].Y >= yMax))) && (((vec[i + 1].X >= xMin) && (vec[i + 1].X <= xMax)) && ((vec[i + 1].Y >= yMin) && (vec[i + 1].Y <= yMax))))
                {
                    for (int k = (int)xMin; k < (int)(xMax); k++)
                    {
                        for (int l = (int)vec[i].Y; l < (int)(vec[i + 1].Y); l++)
                        {
                            if (k == l)
                            {
                                Vector2 sovp = new Vector2();
                                sovp.X = k;
                                sovp.Y = l;
                                //return sovp;
                                g.DrawRectangle(Pens.Yellow, sovp.X+xn, sovp.Y+yn, 3, 3);
                            }
                        }
                    }
                } //левая
                else if ((((vec[i].X <= xMin) && (vec[i].X <= xMax)) && ((vec[i].Y >= yMin) && (vec[i].Y <= yMax))) && (((vec[i + 1].X >= xMin) && (vec[i + 1].X <= xMax)) && ((vec[i + 1].Y >= yMin) && (vec[i + 1].Y <= yMax))))
                {
                    for (int k = (int)yMin; k < (int)(yMax); k++)
                    {
                        for (int l = (int)vec[i].X; l < (int)(vec[i + 1].X); l++)
                        {
                            if (k == l)
                            {
                                Vector2 sovp = new Vector2();
                                sovp.X = k;
                                sovp.Y = l;
                                //  return sovp;
                                g.DrawRectangle(Pens.Yellow, sovp.X+xn, sovp.Y+yn, 3, 3);
                            }
                        }
                    }
                }//правая
                else if ((((vec[i].X >= xMin) && (vec[i].X >= xMax)) && ((vec[i].Y >= yMin) && (vec[i].Y <= yMax))) && (((vec[i + 1].X >= xMin) && (vec[i + 1].X <= xMax)) && ((vec[i + 1].Y >= yMin) && (vec[i + 1].Y <= yMax))))
                {
                    for (int k = (int)yMin; k < (int)(yMax); k++)
                    {
                        for (int l = (int)vec[i].X; l < (int)(vec[i + 1].X); l++)
                        {
                            if (k == l)
                            {
                                Vector2 sovp = new Vector2();
                                sovp.X = k;
                                sovp.Y = l;
                               // return sovp;
                                 g.DrawRectangle(Pens.Yellow,sovp.X+xn,sovp.Y+yn,3,3);
                            }
                        }
                    }
                }
                else
                {
                 //   return vec[i];
                }
            }
            // return vec[1];
         
            g.DrawRectangle(Pens.Yellow, new Rectangle(50, 50, 50, 50));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Vector2> vec = new List<Vector2>();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < mnog.Count - 1; i++)
            {
                var parts = mnog[i].Split(' ');
                var parts2 = mnog[i + 1].Split(' ');
                vec.Add(new Vector2(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1])));
                vec.Add(new Vector2(Convert.ToInt32(parts2[0]), Convert.ToInt32(parts2[1])));
            }
             
                SH(vec);
            
            //Thread.Sleep(60);
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            label6.Text = "Time " + elapsedTime;
        }

    }
    public class Line
    {
        public float x1, x2, y1, y2;

        public Line(float x1, float y1, float x2, float y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }

        public Line() { }

        public void setX1(float x1) { this.x1 = x1; }

        public float getX1()
        {
            return x1;
        }

        public void setX2(float x2) { this.x2 = x2; }
        public float getX2()
        {
            return x2;
        }

        public void sety1(float y1) { this.y1 = y1; }
        public float getY1()
        {
            return y1;
        }

        public void sety2(float y2) { this.y2 = y2; }
        public float getY2()
        {
            return y2;
        }
    }
}
