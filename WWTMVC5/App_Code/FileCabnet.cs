using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Xml;


    public class FileEntry
    {
        public string Filename;
        public int Size;
        public int Offset;
        public FileEntry(string filename, int size)
        {
            Filename = filename;
            Size = size;
        }
        public override string ToString()
        {
            return Filename;
        }
    }

    public class FileCabinet
    {
     
      
        public static void Extract(string cabfile, string filetarget, HttpResponse response)
        {

            //try
            {
                filetarget = filetarget.ToLower();

                while (filetarget.StartsWith(@"\"))
                {
                    filetarget = filetarget.Substring(1);
                }


                string data;
                XmlDocument doc = new XmlDocument();
                int headerSize = 0;
                using (FileStream fs = File.OpenRead(cabfile))
                {

                    byte[] buffer = new byte[256];
                    fs.Read(buffer, 0, 255);
                    data = Encoding.UTF8.GetString(buffer);

                    int start = data.IndexOf("0x");
                    if (start == -1)
                    {
                        throw new SystemException( "Invalid File Format");
                    }
                    headerSize = Convert.ToInt32(data.Substring(start, 10), 16);

                    fs.Seek(0, SeekOrigin.Begin);


                    buffer = new byte[headerSize];
                    fs.Read(buffer, 0, headerSize);
                    data = Encoding.UTF8.GetString(buffer);
                    doc.LoadXml(data);

                    XmlNode cab = doc["FileCabinet"];
                    XmlNode files = cab["Files"];

                    int offset = headerSize;
                  
                    foreach (XmlNode child in files.ChildNodes)
                    {
                        FileEntry entry = new FileEntry(child.Attributes["Name"].Value.ToLower(), Convert.ToInt32(child.Attributes["Size"].Value));
                        entry.Offset = offset;
                        offset += entry.Size;

                        if (entry.Filename.Contains(@"\"))
                        {
                            entry.Filename = entry.Filename.Substring(entry.Filename.LastIndexOf("\\") + 1);
                        }


                        if (entry.Filename == filetarget || filetarget == "master" )
                        {

                            buffer = new byte[entry.Size];
                            fs.Seek(entry.Offset, SeekOrigin.Begin);
                            if (fs.Read(buffer, 0, entry.Size) != entry.Size)
                            {
                                throw new SystemException("One of the files in the collection is missing, corrupt or inaccessable");
                            }
			
			    buffer = UnGzip(buffer);
				
                            response.ContentType = GetMimeTypoForFile(entry.Filename);
                            response.OutputStream.Write(buffer, 0, buffer.Length);
                            return;
                        }

                    }

                   
                    fs.Close();
                }
            }
            //catch
            {
                //  UiTools.ShowMessageBox("The data cabinet file was not found. WWT will now download all data from network.");
            }

        }
	
	 static byte[] UnGzip(byte[] buffer)
    {
        if (buffer[0] == 31 && buffer[1] == 139)
        {
            MemoryStream msIn = new MemoryStream(buffer);
            MemoryStream msOut = new MemoryStream();
            System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(msIn, System.IO.Compression.CompressionMode.Decompress);

            byte[] data = new byte[2048];

            while (true)
            {
                int count = gzip.Read(data, 0, 2048);
                msOut.Write(data, 0, count);

                if (count < 2048)
                {
                    break;
                }
            }
            gzip.Close();
            msOut.Close();
            return msOut.ToArray();

        }
        return buffer;
    }

        static string GetMimeTypoForFile(string filename)
        {
            if (filename.Contains("."))
            {
                string extention = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();

                switch (extention)
                {

                    case "jepg":
                    case "jpg":
                    case "jfif":
                        return "images/jepg";
                    case "png":
                        return "images/png";
                    case "wma":
                        return "audio/x-ms-wma";
                    case "mp3":
                        return "audio/mp3";
                    case "xml":
                        return "text/xml";
                    case "txt":
                        return "text/plain";
                    case "asf":
                        return "video/x-ms-asf";
                    case "asx":
                        return "video/x-ms-asf";
                    case "wmv":
                        return "audio/x-ms-wmv";
                    case "wvx":
                        return "video/x-ms-wm";
                    case "wmx":
                        return "video/x-ms-wmx";
                    case "wmz":
                        return "application/x-ms-wmz";
                    case "wmd":
                        return "application/x-ms-wmd";

                }

            }

            return "text/xml";
        }
    }
