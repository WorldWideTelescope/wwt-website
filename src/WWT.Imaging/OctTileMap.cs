#nullable disable

using System;
using System.Collections.Generic;

namespace WWT.Imaging
{
    public class OctTileMap
    {
        protected bool backslash;

        protected Vector3d[,] Bounds;

        public double decMax;

        public double decMin;

        public int Level;

        protected static Vector3d[,] masterBounds;

        private Vector2d[,] raDecMap;

        public double raMax;

        public double raMin;

        private int subDivisions = 5;

        private float subDivSize = 1f / (float)Math.Pow(2.0, 5.0);

        public int X;

        public int Y;

        static OctTileMap()
        {
            masterBounds = new Vector3d[3, 3];
            masterBounds[0, 0] = new Vector3d(0.0, -1.0, 0.0);
            masterBounds[1, 0] = new Vector3d(0.0, 0.0, -1.0);
            masterBounds[2, 0] = new Vector3d(0.0, -1.0, 0.0);
            masterBounds[0, 1] = new Vector3d(1.0, 0.0, 0.0);
            masterBounds[1, 1] = new Vector3d(0.0, 1.0, 0.0);
            masterBounds[2, 1] = new Vector3d(-1.0, 0.0, 0.0);
            masterBounds[0, 2] = new Vector3d(0.0, -1.0, 0.0);
            masterBounds[1, 2] = new Vector3d(0.0, 0.0, 1.0);
            masterBounds[2, 2] = new Vector3d(0.0, -1.0, 0.0);
        }

        public OctTileMap(int level, int x, int y)
        {
            Level = level;
            Y = y;
            X = x;
            int num = 0;
            Vector3d[,] masterBounds = null;
            backslash = false;
            for (; num <= level; num++)
            {
                if (num == 0)
                {
                    masterBounds = OctTileMap.masterBounds;
                    continue;
                }
                Vector3d[,] vectordArray2 = new Vector3d[3, 3];
                int num2 = (int)((double)x / Math.Pow(2.0, level - num));
                int num5 = (int)((double)y / Math.Pow(2.0, level - num));
                int num3 = num2 % 2;
                int num4 = num5 % 2;
                if (num == 1)
                {
                    backslash = ((num3 == 1) ^ (num4 == 1));
                }
                vectordArray2[0, 0] = masterBounds[num3, num4];
                vectordArray2[1, 0] = Vector3d.MidPoint(masterBounds[num3, num4], masterBounds[num3 + 1, num4]);
                vectordArray2[2, 0] = masterBounds[num3 + 1, num4];
                vectordArray2[0, 1] = Vector3d.MidPoint(masterBounds[num3, num4], masterBounds[num3, num4 + 1]);
                if (backslash)
                {
                    vectordArray2[1, 1] = Vector3d.MidPoint(masterBounds[num3, num4], masterBounds[num3 + 1, num4 + 1]);
                }
                else
                {
                    vectordArray2[1, 1] = Vector3d.MidPoint(masterBounds[num3 + 1, num4], masterBounds[num3, num4 + 1]);
                }
                vectordArray2[2, 1] = Vector3d.MidPoint(masterBounds[num3 + 1, num4], masterBounds[num3 + 1, num4 + 1]);
                vectordArray2[0, 2] = masterBounds[num3, num4 + 1];
                vectordArray2[1, 2] = Vector3d.MidPoint(masterBounds[num3, num4 + 1], masterBounds[num3 + 1, num4 + 1]);
                vectordArray2[2, 2] = masterBounds[num3 + 1, num4 + 1];
                masterBounds = vectordArray2;
            }
            Bounds = masterBounds;
            InitGrid();
        }

        private void InitGrid()
        {
            List<PositionTexture> vertexList = null;
            List<Triangle> list2 = null;
            vertexList = new List<PositionTexture>();
            list2 = new List<Triangle>();
            vertexList.Add(new PositionTexture(Bounds[0, 0], 0.0, 0.0));
            vertexList.Add(new PositionTexture(Bounds[1, 0], 0.5, 0.0));
            vertexList.Add(new PositionTexture(Bounds[2, 0], 1.0, 0.0));
            vertexList.Add(new PositionTexture(Bounds[0, 1], 0.0, 0.5));
            vertexList.Add(new PositionTexture(Bounds[1, 1], 0.5, 0.5));
            vertexList.Add(new PositionTexture(Bounds[2, 1], 1.0, 0.5));
            vertexList.Add(new PositionTexture(Bounds[0, 2], 0.0, 1.0));
            vertexList.Add(new PositionTexture(Bounds[1, 2], 0.5, 1.0));
            vertexList.Add(new PositionTexture(Bounds[2, 2], 1.0, 1.0));
            if (Level == 0)
            {
                list2.Add(new Triangle(3, 7, 4));
                list2.Add(new Triangle(3, 6, 7));
                list2.Add(new Triangle(7, 5, 4));
                list2.Add(new Triangle(7, 8, 5));
                list2.Add(new Triangle(5, 1, 4));
                list2.Add(new Triangle(5, 2, 1));
                list2.Add(new Triangle(1, 3, 4));
                list2.Add(new Triangle(1, 0, 3));
            }
            else if (backslash)
            {
                list2.Add(new Triangle(4, 0, 3));
                list2.Add(new Triangle(4, 1, 0));
                list2.Add(new Triangle(5, 1, 4));
                list2.Add(new Triangle(5, 2, 1));
                list2.Add(new Triangle(3, 7, 4));
                list2.Add(new Triangle(3, 6, 7));
                list2.Add(new Triangle(8, 4, 7));
                list2.Add(new Triangle(8, 5, 4));
            }
            else
            {
                list2.Add(new Triangle(1, 0, 3));
                list2.Add(new Triangle(1, 3, 4));
                list2.Add(new Triangle(2, 1, 4));
                list2.Add(new Triangle(2, 4, 5));
                list2.Add(new Triangle(6, 4, 3));
                list2.Add(new Triangle(6, 7, 4));
                list2.Add(new Triangle(7, 5, 4));
                list2.Add(new Triangle(8, 5, 7));
            }
            int subDivisions = this.subDivisions;
            subDivSize = 1f / (float)Math.Pow(2.0, this.subDivisions);
            while (subDivisions-- > 1)
            {
                List<Triangle> triList = new List<Triangle>();
                foreach (Triangle item in list2)
                {
                    item.SubDivide(triList, vertexList);
                }
                list2 = triList;
            }
            int num2 = 1 + (int)Math.Pow(2.0, this.subDivisions);
            int num3 = 1 + (int)Math.Pow(2.0, this.subDivisions);
            PositionTexture[,] textureArray = new PositionTexture[num2, num3];
            raDecMap = new Vector2d[num2, num3];
            foreach (PositionTexture texture in vertexList)
            {
                int num4 = (int)(texture.Tu / (double)subDivSize + 0.1);
                int num5 = (int)(texture.Tv / (double)subDivSize + 0.1);
                textureArray[num4, num5] = texture;
            }
            for (int i = 0; i < num3; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    raDecMap[j, i] = textureArray[j, i].Position.ToRaDec();
                }
            }
            if (Level == 0)
            {
                raMin = 0.0;
                raMax = 360.0;
                decMin = -90.0;
                decMax = 90.0;
                return;
            }
            raMin = Math.Min(Math.Min(raDecMap[0, 0].X, raDecMap[0, num3 - 1].X), Math.Min(raDecMap[num2 - 1, 0].X, raDecMap[num2 - 1, num3 - 1].X));
            raMax = Math.Max(Math.Max(raDecMap[0, 0].X, raDecMap[0, num3 - 1].X), Math.Max(raDecMap[num2 - 1, 0].X, raDecMap[num2 - 1, num3 - 1].X));
            decMin = Math.Min(Math.Min(raDecMap[0, 0].Y, raDecMap[0, num3 - 1].Y), Math.Min(raDecMap[num2 - 1, 0].Y, raDecMap[num2 - 1, num3 - 1].Y));
            decMax = Math.Max(Math.Max(raDecMap[0, 0].Y, raDecMap[0, num3 - 1].Y), Math.Max(raDecMap[num2 - 1, 0].Y, raDecMap[num2 - 1, num3 - 1].Y));
            if (Math.Abs(raMax - raMin) > 180.0)
            {
                raMin = raMax;
                raMax = 360.0;
            }
        }

        public Vector2d PointToRaDec(Vector2d point)
        {
            int num = (int)(point.X / (double)subDivSize);
            int num2 = (int)(point.Y / (double)subDivSize);
            if (num > (int)Math.Pow(2.0, subDivisions) - 1)
            {
                num = (int)Math.Pow(2.0, subDivisions) - 1;
            }
            if (num2 > (int)Math.Pow(2.0, subDivisions) - 1)
            {
                num2 = (int)Math.Pow(2.0, subDivisions) - 1;
            }
            double interpolater = (point.X - (double)((float)num * subDivSize)) / (double)subDivSize;
            double num3 = (point.Y - (double)((float)num2 * subDivSize)) / (double)subDivSize;
            Vector2d left = Vector2d.Lerp(raDecMap[num, num2], raDecMap[num + 1, num2], interpolater);
            Vector2d right = Vector2d.Lerp(raDecMap[num, num2 + 1], raDecMap[num + 1, num2 + 1], interpolater);
            return Vector2d.Lerp(left, right, num3);
        }
    }
}
