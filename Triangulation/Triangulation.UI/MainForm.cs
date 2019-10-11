﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Triangulation.Core;

namespace Triangulation
{
    public partial class MainForm : Form
    {
        private List<PointF> _points;
        private List<Vertex> _polygonTops;
        private List<Color> _colorsForTriangles;

        public MainForm()
        {
            InitializeComponent();
            _polygonTops = new List<Vertex>();
            _points = new List<PointF>();
            _colorsForTriangles = new List<Color>
            {
                Color.Aqua,
                Color.Brown,
                Color.BlueViolet,
                Color.Yellow,
                Color.DarkBlue
            };
            TriangulateButton.Enabled = false;
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PolygonPanel_MouseDown(object sender, MouseEventArgs e)
        {
            var point = new Point(e.X, e.Y);
            var color = new SolidBrush(Color.Red);
            Graphics g = PolygonPanel.CreateGraphics();
            g.FillEllipse(color, point.X - 2.5f, point.Y - 2.5f, 5, 5);
            _polygonTops.Add(new Vertex(point.X, point.Y));
            Coordinates.Text += $"X: {e.X}, Y:{e.Y}\n";
            if (_points.Count > 0)
            {
                g.DrawLine(new Pen(new SolidBrush(Color.Black)), point, _points.Last());
            }
            _points.Add(e.Location);
            if (_points.Count >= 2)
                TriangulateButton.Enabled = true;
        }

        private void TriangulateButton_Click(object sender, EventArgs e)
        {
            Graphics g = PolygonPanel.CreateGraphics();
            var polygon = new Polygon(_polygonTops);
            var triangulator = new Triangulator(polygon);
            polygon = triangulator.Triangulate();
            int i = 0;
            foreach (var triangle in polygon.Triangles)
            {
                int j = 0;
                var points = new PointF[3];
                foreach (Vertex top in triangle.Tops)
                {
                    points[j] = new PointF((float)top.X, (float)top.Y);
                    j++;
                }
                g.DrawPolygon(new Pen(new SolidBrush(Color.Black), 2), points);
                g.FillPolygon(new SolidBrush(_colorsForTriangles[i]), points);
                i++;
                i = i % 3 == 0 ? 0 : i;
                foreach (var point in points)
                {
                    g.FillEllipse(new SolidBrush(Color.Yellow), point.X - 3, point.Y - 3, 6, 6);
                }
                //var triangleCentroid = triangle.GetCentroidForTops();
                //g.FillEllipse(new SolidBrush(Color.Black), (float)triangleCentroid.X - 3, (float)triangleCentroid.Y - 3, 6, 6);
            }
           // _polygonTops = new List<Vertex>();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            PolygonPanel.Invalidate();
            _points = new List<PointF>();
            _polygonTops = new List<Vertex>();
            Coordinates.Text = string.Empty;
        }
    }
}
