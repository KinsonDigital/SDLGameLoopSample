using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoopSample
{
    public class Vector
    {
        public Vector() { }


        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }


        public float X { get; set; }

        public float Y { get; set; }


        public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
    }
}
