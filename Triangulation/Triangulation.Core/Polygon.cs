﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triangulation.Core
{
    public class Polygon: ICloneable
    {
        public List<Triangle> Triangles { get; set; }
        public List<Vertex> Tops { get;set; }
        public List<Vertex> TopsForMove { get; private set; }
        private int _moverPosition;

        public Polygon(List<Vertex> tops)
        {
            Tops = tops.Distinct().ToList();            
            Triangles = new List<Triangle>();
            ResetMover();            
        } 
        
        public void Reverse()
        {
            Tops.Reverse();
            TopsForMove.Reverse();
        }

        public virtual double GetSquare()
        {
            ResetMover();
            if (Tops.Count <= 2)
                return 0;
            double square = 0.0;
            for (int i = 0; i < Tops.Count; i++)
            {
                var current = GetCurrentTop();
                var next = GetNextTop();
                square += current.X * next.Y - next.X * current.Y;
                MoveNext();
            }
            return square / 2;
        }

        public double GetPerimeter()
        {
            ResetMover();
            if (Tops.Count < 2)
                return 0;
            double result = 0.0;
            for (int i = 0; i <= Tops.Count; i++)
            {
                result += Geometry.GetEuclidianDistance(GetCurrentTop(), GetNextTop());
                MoveNext();
            }
            return result;
        }
        
        public Vertex GetCurrentTop()
        {
            if (_moverPosition == TopsForMove.Count)
            {
                _moverPosition = 0;
                return TopsForMove[TopsForMove.Count - 1];
            }
            return TopsForMove[_moverPosition];
        }

        public Vertex GetNextTop()
        {
            if (_moverPosition == TopsForMove.Count - 1)
                return TopsForMove[0];
            return TopsForMove[_moverPosition + 1];
        }

        public Vertex GetPreviousTop()
        {
            if (_moverPosition == 0)
                return TopsForMove[TopsForMove.Count - 1];
            return TopsForMove[_moverPosition - 1];
        }

        public void RemoveCurrentTop()
        {
            TopsForMove.RemoveAt(_moverPosition);
            _moverPosition = _moverPosition == 0 ? _moverPosition : _moverPosition - 1;
        }

        public bool HasTriangles()
        {
            return TopsForMove.Count > 2;
        }
        
        public void MoveNext()
        {
            _moverPosition++; 
        }

        private void ResetMover()
        {
            _moverPosition = 0;
            TopsForMove = TopsForMove?.Count == Tops.Count ? TopsForMove : new List<Vertex>(Tops);
        }

        public int CurrentIndex()
        {
            return _moverPosition;
        }

        public object Clone()
        {
            var newPolygon = this.MemberwiseClone() as Polygon ?? new Polygon(new List<Vertex>());
            List<Triangle> triangles = new List<Triangle>();
            List<Vertex> tops = new List<Vertex>();
            foreach (var top in Tops)
            {
                tops.Add(top);
            }
            foreach (var triangle in Triangles)
            {
                triangles.Add((Triangle)triangle.Clone());
            }
            newPolygon.Triangles = triangles;
            newPolygon.Tops = tops;
            return newPolygon;
        }
    }
}
