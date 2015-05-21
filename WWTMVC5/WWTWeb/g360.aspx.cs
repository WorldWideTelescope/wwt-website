using System;
using System.IO;
using WWTWebservices;

namespace WWTMVC5.WWTWeb
{



    public partial class G360 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        public Stream LoadImage(int level, int tileX, int tileY, int id)
        {
            UInt32 index = ComputeHash(level, tileX, tileY)%16;


            return
                PlateFile2.GetFileStream(
                    String.Format(@"\\wwt-mars\marsroot\hirise\hiriseV5_{0}.plate", index), id, level, tileX, tileY);


        }

        public UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }


    }
}