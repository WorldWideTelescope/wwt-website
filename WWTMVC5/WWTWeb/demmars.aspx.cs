using System;
using System.Security.Cryptography;
using WWTWebservices;

namespace WWTMVC5.WWTWeb
{


    public partial class DemMars : System.Web.UI.Page
    {
        private static MD5 md5Hash = MD5.Create();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public UInt32 ComputeHash(int level, int x, int y)
        {
            return DirectoryEntry.ComputeHash(level + 128, x, y);
        }


    }

}