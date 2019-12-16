/********************************************************
 * Author: Chukwudi Ikem
 * Email: godofcollege43@csu.fullerton.edu
 * Course: CPSC 223N
 * Semester: Fall 2019
 * Assignment #: 6
 * Program name: Flower Assignment
*****************************************************/
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Timers;
using System.Drawing.Drawing2D;

namespace FlowerAssignment
{
    public class FlowerUI : Form
    {
        Flower_Algorithm flowerdraw = new Flower_Algorithm(); // OOP - my class, encapsulating the flower algorithm
        protected Label showminus, showplus;
        protected Button go, pause, exit;
        protected Pen thickness = new Pen(Color.Black, 1);
        protected Graphics referencetographics;
        protected Bitmap referencetobitmap;
        protected const int formwidth = 1920;
        protected const int formheight = 1080;
        protected const int spiral_width = 2;

        protected double x, y, t, incrementvalue;
        protected double x_scaled_doubled, y_scaled_doubled;
        protected int x_scaled_int, y_scaled_int;

        protected static System.Timers.Timer GraphicClock = new System.Timers.Timer();
        protected static System.Timers.Timer FlowerClock = new System.Timers.Timer();
        protected static double xOrigin = formwidth / 2.0;
        protected static double yOrigin = formheight / 2.0;
        protected const double scalefactor = 65.0; // this is to compound our labels and give accurate spacing
        protected bool isOn;

        public FlowerUI()
        {
            //DoubleBuffered = true;
            CenterToScreen();
            Size = new Size(formwidth, formheight);
            Text = "Flower Drawing";
            referencetobitmap = new Bitmap(formwidth, formheight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            referencetographics = Graphics.FromImage(referencetobitmap);
            incrementvalue = 10000.0; // TODO: Change this value to influence drawing!
            StartGraphicClock(); // will initiate the graphics refresh and enables it
            StartFlowerClock(); //will initiate the flower refresh interval and enable it
            BitMapGraph(); // to create the static bitmaps for the background and the apple. Wanted to avoid invalidation on these paintings
            RenderButtons();
        }
        protected void RenderButtons()
        {
            showminus = new Label { Text = "(-)", Location = new Point(25, 500), BackColor = Color.LightBlue, AutoSize = true };
            showplus = new Label { Text = "(+)", Location = new Point(1895, 500), BackColor = Color.LightBlue, AutoSize = true };
            go = new Button { Text = "Go", Location = new Point(25, 25), AutoSize = true, BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat };
            pause = new Button { Text = "Pause", Location = new Point(425, 25), AutoSize = true, BackColor = Color.WhiteSmoke, FlatStyle = FlatStyle.Flat };
            exit = new Button { Text = "Exit", Location = new Point(1850, 950), Size = new Size(50, 50), BackColor = Color.IndianRed, ForeColor = Color.GhostWhite };

            exit.Click += ExitCall;
            go.Click += GoCall;
            pause.Click += PauseCall;

            Controls.Add(showminus);
            Controls.Add(showplus);
            Controls.Add(go);
            Controls.Add(pause);
            Controls.Add(exit);

        }
        protected void BitMapGraph() // THIS FUNCTION IS WHAT DRAWS THE X AND Y AXIS, DO NOT GET CONFUSED IN IMPLEMENTATION
        {
            Font numericalvaluefont = new Font("Times New Roman", 12, FontStyle.Regular);
            SolidBrush numberbrush = new SolidBrush(Color.Black);
            double numberlabel = 0.0;
            referencetographics.Clear(Color.LightBlue); // if not this, the color will remain black for some reason.. ?. This sets the color!
            referencetographics.DrawLine(thickness, 0, formheight / 2, formwidth, formheight / 2); //draws the x axis
            referencetographics.DrawLine(thickness, formwidth / 2, 0, formwidth / 2, formheight); //draws the y axis

            while (numberlabel * scalefactor * 2.0 < (float)formwidth) //this is to darw the coordinates for the x_axis
            {
                numberlabel = numberlabel + 1.0;
                referencetographics.DrawString(String.Format("{0:0.0}", numberlabel), numericalvaluefont, numberbrush, new PointF((float)(xOrigin + (int)Math.Round(numberlabel * scalefactor - 10.0)), (float)yOrigin + 2));
                referencetographics.DrawString(String.Format("{0:0.0}", numberlabel), numericalvaluefont, numberbrush, new PointF((float)(xOrigin - (int)Math.Round(numberlabel * scalefactor + 10.0)), (float)yOrigin + 2));
                if (numberlabel.Equals(14.0)) { break; }
            }
            numberlabel = 0.0;
            while (numberlabel * scalefactor * 2.0 < (float)formheight) //this is to draw the coordinates for the y_axis
            {
                numberlabel = numberlabel + 1.0;
                referencetographics.DrawString(String.Format("{0:0.0}", numberlabel), numericalvaluefont, numberbrush, new PointF((float)(xOrigin - 10.0), (float)(yOrigin - (int)Math.Round(numberlabel * scalefactor + 6.0))));
                referencetographics.DrawString(String.Format("{0:0.0}", numberlabel), numericalvaluefont, numberbrush, new PointF((float)(xOrigin - 10.0), (float)(yOrigin + (int)Math.Round(numberlabel * scalefactor - 6.0))));
                if (numberlabel.Equals(7.0)) { break; }
            }

        }
        protected override void OnPaint(PaintEventArgs e) // Create a Graphics object to Draw the Bitmap onto the form.
        {
            Graphics graph = e.Graphics;
            graph.DrawImage(referencetobitmap, 0, 0, formwidth, formheight); // this is going to draw the image from (0,0) to (1920,1080), fullscreen
            base.OnPaint(e);
        }

        protected void UpdatePosition(object sender, ElapsedEventArgs events) // Updates the position of x and y with respect to the scale factor.
        {
            flowerdraw.GetNextCoordinate(incrementvalue, ref t, out x, out y);
            x_scaled_doubled = 200 * x;
            y_scaled_doubled = 200 * y;
        }

        protected void UpdateTheGraphicArea(object sender, ElapsedEventArgs events) // Includes the functions such as x(t) = cos(2t)*cos(t) that will draw the actual flower
        {
            x_scaled_int = (int)Math.Round(x_scaled_doubled);
            y_scaled_int = (int)Math.Round(y_scaled_doubled);
            referencetographics.FillEllipse(Brushes.Red, (int)(xOrigin + x_scaled_int - spiral_width / 2), (int)(yOrigin - y_scaled_int - spiral_width / 2), spiral_width, spiral_width);
            Invalidate();
        }

        protected void StartGraphicClock() //will be called in the constructor, sets up the interval at which the form will be refreshed at
        {
            GraphicClock.Elapsed += new ElapsedEventHandler(UpdateTheGraphicArea);
            GraphicClock.Interval = 40.0;
        }

        protected void StartFlowerClock() // call in constructor ^^
        {
            FlowerClock.Elapsed += new ElapsedEventHandler(UpdatePosition);
            FlowerClock.Interval = 40.0;
        }

        protected void GoCall(object sender, EventArgs e)
        {
            isOn = true;
            GraphicClock.Enabled = true;
            FlowerClock.Enabled = true;
        }
        protected void PauseCall(object sender, EventArgs e)
        {
            isOn = false;
            GraphicClock.Enabled = false;
            FlowerClock.Enabled = false;
        }
        protected void ExitCall(object sender, EventArgs e) => Close();
    }

    public class Flower_Algorithm
    {
        public void GetNextCoordinate(double incrementvalue, ref double t, out double Xreturn, out double Yreturn) // Includes the functions such as x(t) = cos(2t)*cos(t) that will draw the actual flower
        {
            t = t + incrementvalue;
            Xreturn = Math.Sin(4 * t) * Math.Cos(t);
            Yreturn = Math.Sin(4 * t) * Math.Sin(t);
        }
    }
}
// 4.47213595 