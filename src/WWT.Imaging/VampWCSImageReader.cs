using System;
using System.Drawing;
using System.Xml;

namespace WWT.Providers
{
    internal class VampWCSImageReader : WcsImage
    {
        public static string ExtractXMPFromFile(string filename)
        {
            char contents;
            string xmpStartSigniture = "<rdf:RDF";
            string xmpEndSigniture = "</rdf:RDF>";
            string data = string.Empty;
            bool reading = false;
            bool grepping = false;
            int collectionCount = 0;

            using (System.IO.StreamReader sr = new System.IO.StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    contents = (char)sr.Read();

                    if (!grepping && !reading && contents == '<')
                    {
                        grepping = true;
                    }

                    if (grepping)
                    {
                        data += contents;

                        if (data.Contains(xmpStartSigniture))
                        {
                            //found the begin element we can stop matching and start collecting
                            grepping = false;
                            reading = true;
                        }
                        else if (contents == xmpStartSigniture[collectionCount++])
                        {
                            //we are still looking, but on track to start collecting
                            continue;
                        }
                        else
                        {
                            //false start reset everything
                            data = string.Empty;
                            grepping = false;
                            reading = false;
                            collectionCount = 0;
                        }

                    }
                    else if (reading)
                    {
                        data += contents;

                        if (data.Contains(xmpEndSigniture))
                        {
                            //we are finished found the end of the XMP data
                            break;
                        }
                    }
                }

            }
            return data;
        }

        private string imageFilename;

        public VampWCSImageReader(string filename)
        {
            this.imageFilename = filename;
            string data = VampWCSImageReader.ExtractXMPFromFile(filename);
            ValidWcs = ExtractXMPParameters(data);
        }




        public override Bitmap GetBitmap()
        {
            return new Bitmap(imageFilename);
        }

        private int Rating;

        public bool ExtractXMPParameters(string xmpXmlDoc)
        {
            XmlDocument doc = new XmlDocument();

            bool hasRotation = false;
            bool hasSize = false;
            bool hasScale = false;
            bool hasLocation = false;
            bool hasPixel = false;
            try
            {
                doc.LoadXml(xmpXmlDoc);
            }
            catch
            {
                return false;
            }

            try
            {

                XmlNamespaceManager NamespaceManager = new XmlNamespaceManager(doc.NameTable);
                NamespaceManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                NamespaceManager.AddNamespace("exif", "http://ns.adobe.com/exif/1.0/");
                NamespaceManager.AddNamespace("x", "adobe:ns:meta/");
                NamespaceManager.AddNamespace("xap", "http://ns.adobe.com/xap/1.0/");
                NamespaceManager.AddNamespace("tiff", "http://ns.adobe.com/tiff/1.0/");
                NamespaceManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                NamespaceManager.AddNamespace("avm", "http://www.communicatingastronomy.org/avm/1.0/");
                NamespaceManager.AddNamespace("ps", "http://ns.adobe.com/photoshop/1.0/");
                // get ratings
                XmlNode xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/xap:Rating", NamespaceManager);

                // Alternatively, there is a common form of RDF shorthand that writes simple properties as
                // attributes of the rdf:Description element.
                if (xmlNode == null)
                {
                    xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description", NamespaceManager);
                    xmlNode = xmlNode.Attributes["xap:Rating"];
                }

                if (xmlNode != null)
                {
                    this.Rating = Convert.ToInt32(xmlNode.InnerText);
                }

                // get keywords
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:subject/rdf:Bag", NamespaceManager);

                if (xmlNode != null)
                {

                    foreach (XmlNode li in xmlNode)
                    {
                        keywords.Add(li.InnerText);
                    }
                }

                // get description
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:description/rdf:Alt", NamespaceManager);

                if (xmlNode != null)
                {
                    this.description = xmlNode.ChildNodes[0].InnerText;
                }

                // get Credits
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/ps:Credit", NamespaceManager);

                if (xmlNode != null)
                {
                    this.copyright = xmlNode.ChildNodes[0].InnerText;
                }

                // get credut url
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:ReferenceURL", NamespaceManager);

                if (xmlNode != null)
                {
                    this.creditsUrl = xmlNode.ChildNodes[0].InnerText;
                }


                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.Rotation", NamespaceManager);
                if (xmlNode != null)
                {
                    rotation = -Convert.ToDouble(xmlNode.InnerText);
                    hasRotation = true;
                }
                else
                {
                    xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description", NamespaceManager);
                    if (xmlNode != null)
                    {
                        xmlNode = xmlNode.Attributes["avm:Spatial.Rotation"];
                        if (xmlNode != null)
                        {
                            rotation = -Convert.ToDouble(xmlNode.InnerText);
                            hasRotation = true;
                        }
                    }
                }

                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.Scale/rdf:Seq", NamespaceManager);
                if (xmlNode != null)
                {
                    xmlNode = xmlNode.FirstChild;
                    scaleX = Convert.ToDouble(xmlNode.InnerText);
                    scaleX = -Math.Abs(scaleX);
                    xmlNode = xmlNode.NextSibling;
                    scaleY = Convert.ToDouble(xmlNode.InnerText);
                    hasScale = true;
                }

                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.ReferencePixel/rdf:Seq",
                    NamespaceManager);
                if (xmlNode != null)
                {
                    xmlNode = xmlNode.FirstChild;
                    referenceX = Convert.ToDouble(xmlNode.InnerText);
                    xmlNode = xmlNode.NextSibling;
                    referenceY = Convert.ToDouble(xmlNode.InnerText);
                    hasPixel = true;
                }

                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.ReferenceDimension/rdf:Seq",
                    NamespaceManager);
                if (xmlNode != null)
                {
                    xmlNode = xmlNode.FirstChild;
                    sizeX = Convert.ToDouble(xmlNode.InnerText);
                    xmlNode = xmlNode.NextSibling;
                    sizeY = Convert.ToDouble(xmlNode.InnerText);
                    hasSize = true;
                }

                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.ReferenceValue/rdf:Seq",
                    NamespaceManager);
                if (xmlNode != null)
                {
                    xmlNode = xmlNode.FirstChild;
                    centerX = Convert.ToDouble(xmlNode.InnerText);
                    xmlNode = xmlNode.NextSibling;
                    centerY = Convert.ToDouble(xmlNode.InnerText);
                    hasLocation = true;
                }
                //- <avm:Spatial.CDMatrix>
                //- <rdf:Seq>
                //  <rdf:li>-2.39806701404E-08</rdf:li> 
                //  <rdf:li>-2.76656202414E-05</rdf:li> 
                //  <rdf:li>-2.76656202414E-05</rdf:li> 
                //  <rdf:li>2.39806701404E-08</rdf:li> 
                //  </rdf:Seq>

                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/avm:Spatial.CDMatrix/rdf:Seq", NamespaceManager);
                if (xmlNode != null)
                {
                    xmlNode = xmlNode.FirstChild;
                    cd1_1 = Convert.ToDouble(xmlNode.InnerText);
                    xmlNode = xmlNode.NextSibling;
                    cd1_2 = Convert.ToDouble(xmlNode.InnerText);
                    xmlNode = xmlNode.NextSibling;
                    cd2_1 = Convert.ToDouble(xmlNode.InnerText);
                    xmlNode = xmlNode.NextSibling;
                    cd2_2 = Convert.ToDouble(xmlNode.InnerText);

                    //TODO if Rotation was not found calculate it
                    if (!hasRotation)
                    {
                        CalculateRotationFromCD();
                    }
                    if (!hasScale)
                    {
                        CalculateScaleFromCD();
                    }
                    hasScale = true;
                    hasRotation = true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                doc = null;
            }

            return hasRotation && hasSize && hasScale && hasLocation && hasPixel;
        }
    }
}