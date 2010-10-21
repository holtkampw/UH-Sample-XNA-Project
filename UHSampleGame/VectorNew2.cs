using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UHSampleGame
{
    public class VectorNew2
    {
        int X = 0;
        int Y = 0;

        public VectorNew2 Zero
        {
            get { return new VectorNew2(0, 0); }
        }

        public VectorNew2(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static VectorNew2 operator +(VectorNew2 v1, VectorNew2 v2)
        {
            return new VectorNew2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static VectorNew2 operator -(VectorNew2 v1, VectorNew2 v2)
        {
            return new VectorNew2(v1.X - v2.X, v1.Y - v2.Y);
        }


    }
}
