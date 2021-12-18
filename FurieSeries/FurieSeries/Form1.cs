using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FurieSeries
{
    public partial class Form1 : Form
    {
        List<PointF> values = new List<PointF>();
        List<PointF> furieValues = new List<PointF>();
        private bool isMouseDown = false;
        private PointF last = Point.Empty;
        private bool drawFurie = false;

        public float integral()
        {
            var sum = 0.0f;
            var delta = 3.1428f / (float)pictureBox1.Width;

            foreach (var item in values)
            {
                sum += item.Y * delta;
            }

            return sum;
        }

        public float integralSin(int n)
        {
            var sum = 0.0f;
            var delta = 3.1428f / (float)pictureBox1.Width;

            foreach (var item in values)
            {
                sum += item.Y * (float)Math.Sin(n * item.X * delta) * delta;
            }

            return sum;
        }

        public float integralCos(int n)
        {
            var sum = 0.0f;
            var delta = 3.1428f / (float)pictureBox1.Width;

            foreach (var item in values)
            {
                sum += item.Y * (float)Math.Cos(n * item.X * delta) * delta;
            }

            return sum;
        }

        public void calculateFurie(int of)
        {
            drawFurie = true;
            furieValues.Clear();


            var a0 = integral() * 1 / 3.1428f;
            var delta = 3.1428 / (pictureBox1.Width);

            for (int j = 0; j < pictureBox1.Width; j++) {
                var sum = 0.0f;
                sum += a0 / 2.0f;


                for(int k = 1; k < of; k++)
                {
                    var a = 1 / 3.1428f * integralSin(n: k);
                    var b = 1 / 3.1428f * integralCos(n: k);

                    sum += a * (float)Math.Sin(j * delta * k) + b * (float)Math.Cos(j * delta * k);
                }

                furieValues.Add(new PointF(x: j, y: sum));
            }

            drawFunc();
    }

        public Form1()
        {
            InitializeComponent();

            values.Clear();
            for (int i = 0; i < pictureBox1.Width; i++)
            {
                values.Add(new PointF(i, pictureBox1.Height));
            }
        }


        void drawFunc()
        {
            var bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.Clear(Color.White);

            if(drawFurie)
            {
                var pen2 = new Pen(Color.Red);
                pen2.Width = 2;
                pen2.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen2.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                for (int i = 1; i < furieValues.Count; i++)
                {
                    graphics.DrawLine(pen2, furieValues[i - 1], furieValues[i]);
                }
            }


            var pen = new Pen(Color.Black);
            pen.Width = 1;
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            for (int i = 1; i < values.Count; i++)
            {
                graphics.DrawLine(pen, values[i - 1], values[i]);
            }
        
            pictureBox1.Image = bitmap;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.X >= pictureBox1.Width) return;

            if (isMouseDown)
            {
                values[e.X] = e.Location;

                var deltaX = (float)e.X - last.X;
                var deltaY = (float)e.Y - last.Y;
                var startVal = (e.X > last.X ? last.Y : e.Y);

                for(int i = Math.Min(e.X,(int)last.X); i < Math.Max(e.X, (int)last.X); i++)
                {
                    values[i] = new PointF(i, startVal + deltaY / deltaX * (float)(i - Math.Min(e.X, last.X)));
                }

                drawFunc();
            }

            last = e.Location;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            drawFurie = false;
            drawFunc();
            isMouseDown = true;
            last = e.Location;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int value = 0;

            try
            {
                value = Convert.ToInt32(textBox1.Text);
            } catch { }

            calculateFurie(of: value);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
