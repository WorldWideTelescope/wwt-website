using System;
using System.Collections.Generic;

namespace WWTWebservices
{
    public class ToastTileMap
    {
        public int X;
        public int Y;
        public int Level;
        public double raMin;
        public double raMax;
        public double decMin;
        public double decMax;

        public ToastTileMap(int level, int x, int y)
        {
            Level = level;
            Y = y;
            X = x;

            int levels = 0;
            Vector3d[,] oldBounds = null;
            backslash = false;
            while (levels <= level)
            {

                if (levels == 0)
                {
                    oldBounds = masterBounds;
                }
                else
                {
                    Vector3d[,] bounds = new Vector3d[3, 3];
                    // equiv : xTemp = (int) (x * Mat.Pow(2,levels-level)) ; note that levels-level < 0

                    int xTemp = (int)(x / Math.Pow(2, level - levels));
                    int yTemp = (int)(y / Math.Pow(2, level - levels));
                    int xIndex = xTemp % 2;
                    int yIndex = yTemp % 2;

                    if (levels == 1)
                    {
                        backslash = xIndex == 1 ^ yIndex == 1;
                    }


                    bounds[0, 0] = oldBounds[xIndex, yIndex];
                    bounds[1, 0] = Vector3d.MidPoint(oldBounds[xIndex, yIndex], oldBounds[xIndex + 1, yIndex]);
                    bounds[2, 0] = oldBounds[xIndex + 1, yIndex];
                    bounds[0, 1] = Vector3d.MidPoint(oldBounds[xIndex, yIndex], oldBounds[xIndex, yIndex + 1]);

                    if (backslash)
                    {
                        bounds[1, 1] = Vector3d.MidPoint(oldBounds[xIndex, yIndex], oldBounds[xIndex + 1, yIndex + 1]);
                    }
                    else
                    {
                        bounds[1, 1] = Vector3d.MidPoint(oldBounds[xIndex + 1, yIndex], oldBounds[xIndex, yIndex + 1]);
                    }

                    bounds[2, 1] = Vector3d.MidPoint(oldBounds[xIndex + 1, yIndex], oldBounds[xIndex + 1, yIndex + 1]);
                    bounds[0, 2] = oldBounds[xIndex, yIndex + 1];
                    bounds[1, 2] = Vector3d.MidPoint(oldBounds[xIndex, yIndex + 1], oldBounds[xIndex + 1, yIndex + 1]);
                    bounds[2, 2] = oldBounds[xIndex + 1, yIndex + 1];
                    oldBounds = bounds;

                }
                levels++;
            }

            Bounds = oldBounds;
            InitGrid();
        }

        static ToastTileMap()
        {
            masterBounds[0, 0] = new Vector3d(0, -1, 0);
            masterBounds[1, 0] = new Vector3d(0, 0, -1);
            masterBounds[2, 0] = new Vector3d(0, -1, 0);
            masterBounds[0, 1] = new Vector3d(1, 0, 0);
            masterBounds[1, 1] = new Vector3d(0, 1, 0);
            masterBounds[2, 1] = new Vector3d(-1, 0, 0);
            masterBounds[0, 2] = new Vector3d(0, -1, 0);
            masterBounds[1, 2] = new Vector3d(0, 0, 1);
            masterBounds[2, 2] = new Vector3d(0, -1, 0);

        }

        protected Vector3d[,] Bounds;
        protected bool backslash = false;
        static protected Vector3d[,] masterBounds = new Vector3d[3, 3];


        Vector2d[,] raDecMap = null;
        int subDivisions = 5;
        float subDivSize = 1.0f / (float)Math.Pow(2, 5);


        void InitGrid()
        {
            List<PositionTexture> vertexList = null;
            List<Triangle> triangleList = null;
            vertexList = new List<PositionTexture>();
            triangleList = new List<Triangle>();

            vertexList.Add(new PositionTexture(Bounds[0, 0], 0, 0));
            vertexList.Add(new PositionTexture(Bounds[1, 0], .5f, 0));
            vertexList.Add(new PositionTexture(Bounds[2, 0], 1, 0));
            vertexList.Add(new PositionTexture(Bounds[0, 1], 0, .5f));
            vertexList.Add(new PositionTexture(Bounds[1, 1], .5f, .5f));
            vertexList.Add(new PositionTexture(Bounds[2, 1], 1, .5f));
            vertexList.Add(new PositionTexture(Bounds[0, 2], 0, 1));
            vertexList.Add(new PositionTexture(Bounds[1, 2], .5f, 1));
            vertexList.Add(new PositionTexture(Bounds[2, 2], 1, 1));

            if (Level == 0)
            {
                triangleList.Add(new Triangle(3, 7, 4));
                triangleList.Add(new Triangle(3, 6, 7));
                triangleList.Add(new Triangle(7, 5, 4));
                triangleList.Add(new Triangle(7, 8, 5));
                triangleList.Add(new Triangle(5, 1, 4));
                triangleList.Add(new Triangle(5, 2, 1));
                triangleList.Add(new Triangle(1, 3, 4));
                triangleList.Add(new Triangle(1, 0, 3));
            }
            else
            {
                if (backslash)
                {
                    triangleList.Add(new Triangle(4, 0, 3));
                    triangleList.Add(new Triangle(4, 1, 0));
                    triangleList.Add(new Triangle(5, 1, 4));
                    triangleList.Add(new Triangle(5, 2, 1));
                    triangleList.Add(new Triangle(3, 7, 4));
                    triangleList.Add(new Triangle(3, 6, 7));
                    triangleList.Add(new Triangle(7, 4, 8));
                    triangleList.Add(new Triangle(4, 7, 8));
                    triangleList.Add(new Triangle(8, 5, 4));


                }
                else
                {

                    triangleList.Add(new Triangle(1, 0, 3));
                    triangleList.Add(new Triangle(1, 3, 4));
                    triangleList.Add(new Triangle(2, 1, 4));
                    triangleList.Add(new Triangle(2, 4, 5));
                    triangleList.Add(new Triangle(6, 4, 3));
                    triangleList.Add(new Triangle(6, 7, 4));
                    triangleList.Add(new Triangle(7, 5, 4));
                    triangleList.Add(new Triangle(8, 5, 7));
                }

            }

            int count = subDivisions;
            subDivSize = 1.0f / (float)Math.Pow(2, subDivisions);
            while (count-- > 1)
            {
                List<Triangle> newList = new List<Triangle>();
                foreach (Triangle tri in triangleList)
                {
                    tri.SubDivide(newList, vertexList);
                }
                triangleList = newList;

            }

            int xCount = 1 + (int)Math.Pow(2, subDivisions);
            int yCount = 1 + (int)Math.Pow(2, subDivisions);

            PositionTexture[,] points = new PositionTexture[xCount, yCount];
            raDecMap = new Vector2d[xCount, yCount];
            foreach (PositionTexture vertex in vertexList)
            {
                int indexX = (int)((vertex.Tu / subDivSize) + .1);
                int indexY = (int)((vertex.Tv / subDivSize) + .1);

                points[indexX, indexY] = vertex;
            }
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    raDecMap[x, y] = points[x, y].Position.ToRaDec((y == 0 || x == 32) & !backslash);
                }
            }

            if (Level == 0)
            {
                raMin = 0;
                raMax = 360;
                decMin = -90;
                decMax = 90;
            }
            else
            {
                raMin = Math.Min(Math.Min((double)raDecMap[0, 0].X, raDecMap[0, yCount - 1].X), Math.Min((double)raDecMap[xCount - 1, 0].X, raDecMap[xCount - 1, yCount - 1].X));
                raMax = Math.Max(Math.Max((double)raDecMap[0, 0].X, raDecMap[0, yCount - 1].X), Math.Max((double)raDecMap[xCount - 1, 0].X, raDecMap[xCount - 1, yCount - 1].X));
                decMin = Math.Min(Math.Min((double)raDecMap[0, 0].Y, raDecMap[0, yCount - 1].Y), Math.Min((double)raDecMap[xCount - 1, 0].Y, raDecMap[xCount - 1, yCount - 1].Y));
                decMax = Math.Max(Math.Max((double)raDecMap[0, 0].Y, raDecMap[0, yCount - 1].Y), Math.Max((double)raDecMap[xCount - 1, 0].Y, raDecMap[xCount - 1, yCount - 1].Y));
                if (Math.Abs((double)(this.raMax - this.raMin)) > 180.0)
                {
                    this.raMin = this.raMax;
                    this.raMax = 360.0;
                }

            }
        }
        public Vector2d PointToRaDec(double x, double y) // point is between 0 and 1 inclusive
        {
            Vector2d point = new Vector2d(x, y);
            int indexX = (int)(point.X / subDivSize);
            int indexY = (int)(point.Y / subDivSize);

            if (indexX > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexX = ((int)Math.Pow(2, subDivisions) - 1);
            }
            if (indexY > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexY = ((int)Math.Pow(2, subDivisions) - 1);
            }
            double xDist = (point.X - ((double)indexX * subDivSize)) / subDivSize;
            double yDist = (point.Y - ((double)indexY * subDivSize)) / subDivSize;

            Vector2d interpolatedTop = Vector2d.Lerp(raDecMap[indexX, indexY], raDecMap[indexX + 1, indexY], xDist);
            Vector2d interpolatedBottom = Vector2d.Lerp(raDecMap[indexX, indexY + 1], raDecMap[indexX + 1, indexY + 1], xDist);
            Vector2d result = Vector2d.Lerp(interpolatedTop, interpolatedBottom, yDist);


            return result;
        }
        public Vector2d PointToRaDec(Vector2d point) // point is between 0 and 1 inclusive
        {

            int indexX = (int)(point.X / subDivSize);
            int indexY = (int)(point.Y / subDivSize);

            if (indexX > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexX = ((int)Math.Pow(2, subDivisions) - 1);
            }
            if (indexY > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexY = ((int)Math.Pow(2, subDivisions) - 1);
            }
            double xDist = (point.X - ((double)indexX * subDivSize)) / subDivSize;
            double yDist = (point.Y - ((double)indexY * subDivSize)) / subDivSize;

            Vector2d interpolatedTop = Vector2d.Lerp(raDecMap[indexX, indexY], raDecMap[indexX + 1, indexY], xDist);
            Vector2d interpolatedBottom = Vector2d.Lerp(raDecMap[indexX, indexY + 1], raDecMap[indexX + 1, indexY + 1], xDist);
            Vector2d result = Vector2d.Lerp(interpolatedTop, interpolatedBottom, yDist);


            return result;
        }

    }
}