using System;
using System.Drawing;
using System.IO;

namespace WWTWebservices
{
    /* This contains dinoj's utilities for dealing with image pyramids and PlateTilePyramids
     * 
     * Things to add
     * 
     * create .plate file given any of the following
     *   2^N x 2^N image
     *   X x Y image (place inside a 2^N x 2^N image)
     *   an image pyramid
     *   a level of an image pyramid
     * 
     * given L X Y and (xfrac,yfrac) within the tile, output L+1 X1 Y1 for corresponding tile in level below
     * given L X Y, output L-1 X1 Y1 for parent tile
     * given L X Y and (xfrac,yfrac) within the tile, output L-1 X1 Y1 for parent tile (and corresponding xfrac1 yfrac1?)
     *
     * 
     * This file is very much in progress and should only be used as examples
     */

    public class ImagePyramid
    {
        /*
         * 
         * 
         */
        int maxLevel = -1; // if all levels are filled, this is one less than the number of levels
        int[] levels = null;
        string ext = ""; // type of image
        int side = -1; // length of image side
        string origPath = ""; // where image pyramid came from
        bool valid = false; // true iff all the above member data can be properly initialized.

        public string OrigPath
        {
            get { return origPath; }
            set { origPath = value; }
        }
        
        public int MaxLevel
        {
            get { return maxLevel; }
        }
        public string Ext
        {
            get { return ext; }
        }
        public int Side
        {
            get { return side; }
        }

        public ImagePyramid(string dir)
        {
            if (dir.Length > 0 && Directory.Exists(dir))
            {
                origPath = dir;
                if (! origPath[origPath.Length - 1].Equals('\\'))
                {
                    origPath += "\\";
                }
                // origPath now ends with a \ , emphasizing that this is a directory
                string[] tmp = Directory.GetDirectories(origPath);

                int count = 0;
                maxLevel = -1;
                for (int i = 0; i < tmp.Length; i++)
                {                    
                    if (tmp[i].Length > 0)
                    {
                        int k = Convert.ToInt32(Path.GetFileNameWithoutExtension(tmp[i]));
                        if (k >= 0)
                        {
                            maxLevel = (maxLevel > k ? maxLevel : k);
                            count++;
                        }
                    }
                }
                levels = new int[count];

                count = 0;
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].Length > 0)
                    {
                        int k = Convert.ToInt32(Path.GetFileNameWithoutExtension(tmp[i]));
                        if (k >= 0)
                        {
                            levels[count++] = k;
                        }
                    }

                }
                if (levels.Length > 0)
                {
                    string dire = origPath + string.Format("{0}\\0", levels[0]);
                    if (Directory.Exists(dire))
                    {
                        string[] files = Directory.GetFiles(dire);
                        if (files.Length > 0)
                        {
                            ext = Path.GetExtension(files[0]);
                            using (Bitmap b = new Bitmap(files[0]))
                            {
                                if (b.Height == b.Width && b.Height > 0)
                                {
                                    side = b.Height;
                                }
                            }
                        }
                    }
                }
            }
            if (side > 0)
            {
                valid = true;
            }
        }

        public bool BuildupPyramid(bool overwrite)
        {
            // creates more levels
            return false;
        }

        public bool FillPyramid()
        {
            // Fills up levels of pyramid in origPath
            // Returns true if pyramid is full by the time the function ends
            // not implemented yet
            if (maxLevel != levels.Length - 1)
            {
                // implement this
                // if new levels are added, add them to the array levels.
            }
            return (maxLevel == levels.Length - 1);            
        }

        public Bitmap createMosaic(int level)
        {
            // creates a large bitmap from a level
            bool levelExists = false;
            if (level <= maxLevel)
            {
                foreach (int L in levels)
                {
                    if (L.Equals(level))
                    {
                        levelExists = true;
                        break;
                    }
                }
            }
            if (!levelExists)
            {
                return null;
            }
            int L2 = (int)Math.Pow(2, level);
            int bSide = side * L2;
            Bitmap b = new Bitmap(bSide, bSide);
            Graphics g = Graphics.FromImage(b);
            string tileFileName;
            for (int y = 0; y < L2; y++)
            {
                for (int x = 0; x < L2; x++)
                {
                    tileFileName = string.Format("{0}{1}\\{3}\\{3}_{2}.png", origPath, level, x, y);
                    if (File.Exists(tileFileName))
                    {                        
                        using (Bitmap bTile = new Bitmap(tileFileName))
                        {
                            g.DrawImage(bTile, x * side, y * side);
                        }                        
                    }                   
                }
            }
            return b;
        }

        public PlateTilePyramid createPTP(string dotPlateFile)
        {
            /*
             * Creates a .plate file with the name dotPlateFile (which should include path info)
             *    from the current image pyramid. Adds ".plate" extension if not present.
             * Assumes that the current image pyramid is full i.e. has all levels
             * Returns the PlateTilePyramid object created, though this function will 
             *   often be called without any return value.  
             * 
             * 
             */
            if (!valid)
            {
                return null;
            }

            /*  CAUTION: IF this stuff is added, then an exception is thrown
             *  if it is unable to create a full pyramid to work from.
              
               if (maxLevel != levels.Length - 1)
               {
                   if (FillPyramid())
                   {
                       throw new System.ArgumentException("Need a full pyramid to work off ");
                   }
               }
            *
            */


            // dotPlateFile has name of .plate file, with or without extension
            if (Path.GetExtension(dotPlateFile).Length == 0 && dotPlateFile.IndexOf(".") < 0)
            {
                dotPlateFile = dotPlateFile + ".plate";
            }
            // dotPlateFile has name of .plate file, with extension

            string dotPlateDir = Path.GetDirectoryName(dotPlateFile);
            if (Directory.Exists(dotPlateDir))
            {
                Directory.CreateDirectory(dotPlateDir);
            }

            PlateTilePyramid ptp = new PlateTilePyramid(dotPlateFile, maxLevel+1);
            ptp.Create();
            for (int level = maxLevel; level >=0; level--)
            {
                int count = (int)Math.Pow(2, level);
                for (int y = 0; y < count; y++)
                {
                    for (int x = 0; x < count; x++)
                    {
                        string tileFileName = string.Format("{0}{1}\\{3}\\{3}_{2}.png", origPath, level, x, y);
                        if (File.Exists(tileFileName))
                        {
                            ptp.AddFile(tileFileName, level, x, y);
                        }
                    }
                }
            }
            ptp.UpdateHeaderAndClose();
            return ptp;
        }
    }

    
}
