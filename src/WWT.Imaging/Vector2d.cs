#nullable disable

using System;

namespace WWTWebservices
{
    public struct Vector2d
    {
        public double X;
        public double Y;
        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        public static Vector2d Lerp(Vector2d left, Vector2d right, double interpolater)
        {
            if (Math.Abs((double)(left.X - right.X)) > 180)
            {
                if (left.X > right.X)
                {
                    right.X += 360;
                }
                else
                {
                    left.X += 360;
                }
            }
            return new Vector2d(left.X * (1 - interpolater) + right.X * interpolater, left.Y * (1 - interpolater) + right.Y * interpolater);

        }
    }

}
