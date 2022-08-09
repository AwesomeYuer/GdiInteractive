using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GdiInteractive
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CreateBackgroundImg();
            //SetStyle(ControlStyles.OptimizedDoubleBuffer
            //    | ControlStyles.AllPaintingInWmPaint
            //    | ControlStyles.UserPaint, true);
        }

        //当前矩形、圆形的位置和大小
        Rectangle rectShape = Rectangle.Empty;
        //上次鼠标所在位置
        Point pointLast = Point.Empty;

        //背景图形
        Bitmap bmpBackGround = null!;

        void CreateBackgroundImg()
        {
            bmpBackGround = new Bitmap(panel1.Width, panel1.Height);
            var g = Graphics.FromImage(bmpBackGround);
            g.FillRectangle(new SolidBrush(panel1.BackColor), 0, 0, panel1.Width, panel1.Height);
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    g.FillRectangle((((i + j) % 2 == 0 ? Brushes.White : Brushes.DimGray)), new Rectangle(i * 20, j * 20, 20, 20));
                }
            }
            g.Dispose();
        }

        void DrawRect_ClearAndFill()
        {
            var g = panel1.CreateGraphics();
            g.Clear(panel1.BackColor);
            g.FillRectangle(Brushes.CornflowerBlue, rectShape);
            g.Dispose();
        }

        void DrawRect_FillOldFillNew(Rectangle rectOld)
        {
            var g = panel1.CreateGraphics();
            g.FillRectangle(new SolidBrush(panel1.BackColor), rectOld);
            g.FillRectangle(Brushes.CornflowerBlue, rectShape);
            g.Dispose();
        }

        void DrawEllipse_ClearAndFill()
        {
            var g = panel1.CreateGraphics();
            g.Clear(panel1.BackColor);
            g.FillEllipse(Brushes.CornflowerBlue, rectShape);
            g.Dispose();
        }

        void DrawEllipse_FillOldFillNew(Rectangle rectOld)
        {
            var g = panel1.CreateGraphics();
            g.FillEllipse(new SolidBrush(panel1.BackColor), rectOld);
            g.FillEllipse(Brushes.CornflowerBlue, rectShape);
            g.Dispose();
        }

        void DrawEllipse_ClearAndFill_WithBorder()
        {
            var g = panel1.CreateGraphics();
            g.Clear(panel1.BackColor);
            g.FillEllipse(Brushes.CornflowerBlue, rectShape);
            g.DrawRectangle(new Pen(Color.Red), rectShape);
            g.Dispose();
        }

        void DrawEllipse_ClearAndFill_WithBG()
        {
            var g = panel1.CreateGraphics();
            g.DrawImage(bmpBackGround, new Point(0, 0));
            g.FillEllipse(Brushes.CornflowerBlue, rectShape);
            g.Dispose();
        }


        void DrawEllipse_FillOldFillNew_WithBG(Rectangle rectOld)
        {
            var g = panel1.CreateGraphics();
            g.DrawImage(bmpBackGround, rectOld, rectOld, GraphicsUnit.Pixel);
            g.FillEllipse(Brushes.CornflowerBlue, rectShape);
            g.Dispose();
        }

        void DrawEllipse_FillOldFillNew_WithNoVisible(Rectangle rectOld)
        {
            var g = panel1.CreateGraphics();
            g.FillRectangle(new SolidBrush(panel1.BackColor), rectOld);
            g.FillRectangle(Brushes.Red, rectShape);
            g.FillEllipse(Brushes.CornflowerBlue, rectShape);
            g.Dispose();
        }

        void DrawEllipse_Region(Rectangle rectOld)
        {
            //计算剪辑区域
            var x = Math.Min(rectOld.X, rectShape.X);
            var y = Math.Min(rectOld.Y, rectShape.Y);
            var width = Math.Max(rectOld.Right, rectShape.Right) - x;
            var height = Math.Max(rectOld.Bottom, rectShape.Bottom) - y;
            var rectRegion = new Rectangle(x, y, width, height);

            //创建内存位图，将背景及圆形绘制在此内存位图上
            var bmp = new Bitmap(width, height);
            var g = Graphics.FromImage(bmp);
            //从背景图片中取剪辑区域对应的位置
            g.DrawImage(bmpBackGround, 0, 0, rectRegion, GraphicsUnit.Pixel);
            //将圆形绘制到剪辑区域中，所以需要重新计算位置
            var rectDraw = rectShape;
            rectDraw.Offset(-x, -y);
            g.FillEllipse(Brushes.CornflowerBlue, rectDraw);
            g.Dispose();

            g = panel1.CreateGraphics();
            //设置剪辑区域
            g.SetClip(rectRegion);
            //绘制内存位图
            g.DrawImage(bmp, new Point(x, y));
            g.Dispose();
            bmp.Dispose();
        }

        private void btnRect_Click(object sender, EventArgs e)
        {
            rectShape = new Rectangle(10, 10, 200, 200);
            DrawRect_ClearAndFill();
        }

        private void btnEllipse_Click(object sender, EventArgs e)
        {
            rectShape = new Rectangle(10, 10, 200, 200);
            DrawEllipse_ClearAndFill_WithBG();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            var path = new GraphicsPath();
            path.AddEllipse(rectShape);
            path.CloseFigure();
            var region = new Region(path);
            if (region.IsVisible(e.Location))
            {
                pointLast = e.Location;
            }
            else
            {
                pointLast = Point.Empty;
            }
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pointLast != Point.Empty)
            {
                var offset = new Point(e.X - pointLast.X, e.Y - pointLast.Y);
                pointLast = e.Location;
                var old = rectShape;
                rectShape.Offset(offset);
                DrawEllipse_Region(old);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            pointLast = Point.Empty;
        }

    }
}
