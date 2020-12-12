#nullable disable

using System;

namespace WWT.Imaging
{
    // Summary:
    //     Describes a custom vertex format structure that contains position and one
    //     set of texture coordinates.

    // Summary:
    //     Describes and manipulates a vector in three-dimensional (3-D) space.
    [Serializable]
    public struct Vector3d
    {
        // Summary:
        //     Retrieves or sets the x component of a 3-D vector.
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of a 3-D vector.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of a 3-D vector.
        public double Z;

        //
        // Summary:
        //     Initializes a new instance of the Microsoft.DirectX.Vector3d class.
        //
        // Parameters:
        //   valueX:
        //     Initial Microsoft.DirectX.Vector3d.X value.
        //
        //   valueY:
        //     Initial Microsoft.DirectX.Vector3d.Y value.
        //
        //   valueZ:
        //     Initial Microsoft.DirectX.Vector3d.Z value.
        public Vector3d(double valueX, double valueY, double valueZ)
        {
            X = valueX;
            Y = valueY;
            Z = valueZ;
        }
        public Vector3d(Vector3d value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }
        // Summary:
        //     Negates the vector.
        //
        // Parameters:
        //   vec:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     The Microsoft.DirectX.Vector3d structure that is the result of the operation.
        public static Vector3d operator -(Vector3d vec)
        {
            Vector3d result;
            result.X = -vec.X;
            result.Y = -vec.Y;
            result.Z = -vec.Z;
            return result;
        }
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     The Microsoft.DirectX.Vector3d structure to the left of the subtraction operator.
        //
        //   right:
        //     The Microsoft.DirectX.Vector3d structure to the right of the subtraction operator.
        //
        // Returns:
        //     Resulting Microsoft.DirectX.Vector3d structure.
        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X - right.X, left.Y - right.Y, left.Z - left.Z);
        }
        //
        // Summary:
        //     Compares the current instance of a class to another instance to determine
        //     whether they are different.
        //
        // Parameters:
        //   left:
        //     The Microsoft.DirectX.Vector3d structure to the left of the inequality operator.
        //
        //   right:
        //     The Microsoft.DirectX.Vector3d structure to the right of the inequality operator.
        //
        // Returns:
        //     Value that is true if the objects are different, or false if they are the
        //     same.
        public static bool operator !=(Vector3d left, Vector3d right)
        {
            return (left.X != right.X || left.Y != right.Y || left.Z != right.Z);
        }
        //
        // Summary:
        //     Determines the product of a single value and a 3-D vector.
        //
        // Parameters:
        //   right:
        //     Source System.Single structure.
        //
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the product of the Microsoft.DirectX.Vector3d.op_Multiply()
        //     and Microsoft.DirectX.Vector3d.op_Multiply() parameters.
        //public static Vector3d operator *(double right, Vector3d left);
        //
        // Summary:
        //     Determines the product of a single value and a 3-D vector.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source System.Single structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the product of the Microsoft.DirectX.Vector3d.op_Multiply()
        //     and Microsoft.DirectX.Vector3d.op_Multiply() parameters.
        //public static Vector3d operator *(Vector3d left, double right);
        //
        // Summary:
        //     Adds two vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that contains the sum of the parameters.
        //public static Vector3d operator +(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Compares the current instance of a class to another instance to determine
        //     whether they are the same.
        //
        // Parameters:
        //   left:
        //     The Microsoft.DirectX.Vector3d structure to the left of the equality operator.
        //
        //   right:
        //     The Microsoft.DirectX.Vector3d structure to the right of the equality operator.
        //
        // Returns:
        //     Value that is true if the objects are the same, or false if they are different.
        public static bool operator ==(Vector3d left, Vector3d right)
        {
            return (left.X == right.X || left.Y == right.Y || left.Z == right.Z);
        }
        public static Vector3d MidPoint(Vector3d left, Vector3d right)
        {
            Vector3d result = new Vector3d((left.X + right.X) / 2, (left.Y + right.Y) / 2, (left.Z + right.Z) / 2);
            result.Normalize();
            return result;
        }
        // Summary:
        //     Retrieves an empty 3-D vector.
        public static Vector3d Empty
        {
            get
            {
                return new Vector3d(0, 0, 0);
            }
        }

        // Summary:
        //     Adds two 3-D vectors.
        //
        // Parameters:
        //   source:
        public void Add(Vector3d source)
        {
            X += source.X;
            Y += source.Y;
            Z += source.Z;
        }

        //
        // Summary:
        //     Adds two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d.
        //
        // Returns:
        //     Sum of the two Microsoft.DirectX.Vector3d structures.
        public static Vector3d Add(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        //
        // Summary:
        //     Returns a point in barycentric coordinates, using specified 3-D vectors.
        //
        // Parameters:
        //   v1:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   v2:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   v3:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   f:
        //     Weighting factor. See Remarks.
        //
        //   g:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure in barycentric coordinates.
        //public static Vector3d BaryCentric(Vector3d v1, Vector3d v2, Vector3d v3, double f, double g);
        //
        // Summary:
        //     Performs a Catmull-Rom interpolation using specified 3-D vectors.
        //
        // Parameters:
        //   position1:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   position2:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   position3:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   position4:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   weightingFactor:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the Catmull-Rom
        //     interpolation.
        //public static Vector3d CatmullRom(Vector3d position1, Vector3d position2, Vector3d position3, Vector3d position4, double weightingFactor)
        //{
        //}
        //
        // Summary:
        //     Determines the cross product of two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the cross product of two 3-D
        //     vectors.
        public static Vector3d Cross(Vector3d left, Vector3d right)
        {
            return new Vector3d(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);

        }
        //
        // Summary:
        //     Determines the dot product of two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A System.Single value that is the dot product.
        public static double Dot(Vector3d left, Vector3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }
        //
        // Summary:
        //     Returns a value that indicates whether the current instance is equal to a
        //     specified object.
        //
        // Parameters:
        //   compare:
        //     Object with which to make the comparison.
        //
        // Returns:
        //     Value that is true if the current instance is equal to the specified object,
        //     or false if it is not.
        public override bool Equals(object compare)
        {
            Vector3d comp = (Vector3d)compare;
            return this.X == comp.X && this.Y == comp.Y && this.Z == comp.Z;
        }
        //
        // Summary:
        //     Returns the hash code for the current instance.
        //
        // Returns:
        //     Hash code for the instance.
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
        //
        // Summary:
        //     Performs a Hermite spline interpolation using the specified 3-D vectors.
        //
        // Parameters:
        //   position:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   tangent:
        //     Source Microsoft.DirectX.Vector3d structure that is a tangent vector.
        //
        //   position2:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   tangent2:
        //     Source Microsoft.DirectX.Vector3d structure that is a tangent vector.
        //
        //   weightingFactor:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the Hermite spline
        //     interpolation.
        //public static Vector3d Hermite(Vector3d position, Vector3d tangent, Vector3d position2, Vector3d tangent2, double weightingFactor);
        //
        // Summary:
        //     Returns the length of a 3-D vector.
        //
        // Returns:
        //     A System.Single value that contains the vector's length.
        public double Length()
        {
            return System.Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        //
        // Summary:
        //     Returns the length of a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A System.Single value that contains the vector's length.
        public static double Length(Vector3d source)
        {
            return System.Math.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z);

        }
        //
        // Summary:
        //     Returns the square of the length of a 3-D vector.
        //
        // Returns:
        //     A System.Single value that contains the vector's squared length.
        public double LengthSq()
        {
            return X * X + Y * Y + Z * Z;
        }
        //
        // Summary:
        //     Returns the square of the length of a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A System.Single value that contains the vector's squared length.
        public static double LengthSq(Vector3d source)
        {
            return source.X * source.X + source.Y * source.Y + source.Z * source.Z;
        }

        //
        // Summary:
        //     Performs a linear interpolation between two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   interpolater:
        //     Parameter that linearly interpolates between the vectors.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the linear interpolation.
        public static Vector3d Lerp(Vector3d left, Vector3d right, double interpolater)
        {
            return new Vector3d(
                left.X * (1.0 - interpolater) + right.X * interpolater,
                left.Y * (1.0 - interpolater) + right.Y * interpolater,
                left.Z * (1.0 - interpolater) + right.Z * interpolater);

        }
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the largest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //public void Maximize(Vector3d source);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the largest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is made up of the largest components
        //     of the two vectors.
        //public static Vector3d Maximize(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the smallest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //public void Minimize(Vector3d source);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the smallest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is made up of the smallest components
        //     of the two vectors.
        //public static Vector3d Minimize(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Multiplies a 3-D vector by a System.Single value.
        //
        // Parameters:
        //   s:
        //     Source System.Single value used as a multiplier.
        public void Multiply(double s)
        {
            X *= s;
            Y *= s;
            Z *= s;
        }
        //
        // Summary:
        //     Multiplies a 3-D vector by a System.Single value.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   f:
        //     Source System.Single value used as a multiplier.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is multiplied by the System.Single
        //     value.
        public static Vector3d Multiply(Vector3d source, double f)
        {
            Vector3d result = new Vector3d(source);
            result.Multiply(f);
            return result;
        }
        //
        // Summary:
        //     Returns the normalized version of a 3-D vector.
        public void Normalize()
        {
            // Vector3.Length property is under length section
            double length = this.Length();
            if (length != 0)
            {
                X /= length;
                Y /= length;
                Z /= length;
            }
        }

        //
        // Summary:
        //     Scales a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   scalingFactor:
        //     Scaling value.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the scaled vector.
        public static Vector3d Scale(Vector3d source, double scalingFactor)
        {
            Vector3d result = source;
            result.Multiply(scalingFactor);
            return result;
        }
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure to subtract from the current instance.
        public void Subtract(Vector3d source)
        {
            this.X -= source.X;
            this.Y -= source.Y;
            this.Z -= source.Z;

        }
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure to the left of the subtraction
        //     operator.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure to the right of the subtraction
        //     operator.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the operation.
        public static Vector3d Subtract(Vector3d left, Vector3d right)
        {
            Vector3d result = left;
            result.Subtract(right);
            return result;
        }
        //
        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", X, Y, Z);
        }
        public Vector2d ToSpherical()
        {

            double ascention;
            double declination;

            double radius = Math.Sqrt(X * X + Y * Y + Z * Z);
            double XZ = Math.Sqrt(X * X + Z * Z);
            declination = Math.Asin(Y / radius);
            if (XZ == 0)
            {
                ascention = 0;
            }
            else if (0 <= X)
            {
                ascention = Math.Asin(Z / XZ);
            }
            else
            {
                ascention = Math.PI - Math.Asin(Z / XZ);
            }

            //if (vector.Z < 0)
            //{
            //    ascention = ascention - Math.PI;
            //}
            // 0 -1.0         return new Vector2d((((ascention + Math.PI) / (2.0 * Math.PI)) % 1.0f), ((declination + (Math.PI / 2.0)) / (Math.PI)));
            return new Vector2d((((ascention + Math.PI) % (2.0 * Math.PI))), ((declination + (Math.PI / 2.0))));

        }
        public Vector2d ToRaDec(bool edge)
        {
            Vector2d point = ToSpherical();
            point.X = point.X / Math.PI * 180;
            if (edge && point.X == 0)
            {
                point.X = 360;
            }
            point.Y = (point.Y / Math.PI * 180) - 90;
            return point;
        }
        public Vector2d ToRaDec()
        {
            Vector2d vectord = ToSpherical();
            vectord.X = vectord.X / Math.PI * 180.0;
            vectord.Y = vectord.Y / Math.PI * 180.0 - 90.0;
            return vectord;
        }
    }
}
