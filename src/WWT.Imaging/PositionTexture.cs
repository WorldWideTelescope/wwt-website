#nullable disable

using System;

namespace WWTWebservices
{
    // Summary:
    //     Describes a custom vertex format structure that contains position and one
    //     set of texture coordinates.
    public struct PositionTexture
    {
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        //
        // Summary:
        //     Retrieves or sets the x component of the position.
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public double Z;

        //
        // Summary:
        //     Initializes a new instance of the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3d object that contains the vertex position.
        //
        //   u:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        public PositionTexture(Vector3d pos, double u, double v)
        {
            Tu = u;
            Tv = v;
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }
        //
        // Summary:
        //     Initializes a new instance of the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   u:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        public PositionTexture(double xvalue, double yvalue, double zvalue, double u, double v)
        {
            Tu = u;
            Tv = v;
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
        }

        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position
        {
            get
            {
                return new Vector3d(X, Y, Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}", X, Y, Z, Tu, Tv);
        }



    }
}
