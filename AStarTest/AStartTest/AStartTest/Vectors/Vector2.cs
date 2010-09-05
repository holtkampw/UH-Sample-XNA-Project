using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStartTest.Vectors
{
    public class Vector2
    {
        float x;
        float y;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public Vector2()
            : this(0, 0) { }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 Copy(Vector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
    }
}
