using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStartTest.Vectors
{
    public class Vector3
    {
        float x;
        float y;
        float z;

        public float X
        {
            get {return x;}
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }

        public Vector3()
        :this(0,0,0){}

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
