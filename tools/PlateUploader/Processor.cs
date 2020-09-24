using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WWTWebservices;
using WWT.Azure;

namespace PlateUploader
{
    public class Processor
    {
        private readonly AzurePlateTilePyramid _pyramid;
        private readonly string _baseUrl;

        public Processor(AzurePlateTilePyramid pyramid, string baseUrl)
        {
            _pyramid = pyramid;
            _baseUrl = baseUrl;
        }

        public IEnumerable<Action> GetActions(IEnumerable<string> plateFiles)
        {
            // Copy it as the source of the input may end up being cleared before the items are fully created
            var copiedPlateFiles = plateFiles.ToList();

            foreach (string plateFile in copiedPlateFiles)
            {
                foreach (var action in CreateWorkItems(plateFile, _pyramid))
                {
                    yield return action;
                }
            }
        }

        private IEnumerable<Action> CreateWorkItems(string plateFile, AzurePlateTilePyramid tileUploader)
        {
            var filepart = Path.GetFileNameWithoutExtension(plateFile);
            var azureContainer = Path.GetFileName(plateFile).ToLowerInvariant();

            bool hasLevels = PlateTilePyramid.GetLevelCount(plateFile, out int levels);
            string thumbnail = plateFile.Replace(".plate", ".jpg").ToLower().Replace("-", "_");
            string wtmlfile = plateFile.Replace(".plate", ".wtml").ToLower();

            if (File.Exists(thumbnail))
            {
                void UploadThumbnail()
                {
                    tileUploader.SaveStream(GetFileStream(thumbnail), azureContainer, filepart.ToLower().Replace("-", "_") + "_thumb.jpg");
                }

                yield return UploadThumbnail;
            }

            if (File.Exists(wtmlfile))
            {
                string wtmlFileOut = wtmlfile.Replace(".wtml", ".azure.wtml");
                string wtmldata = File.ReadAllText(wtmlfile);
                wtmldata = wtmldata.Replace(filepart + "/{1}/{3}/{3}_{2}.png", _baseUrl + azureContainer + "/" + filepart + "L{1}X{2}Y{3}.png");
                wtmldata = wtmldata.Replace(filepart.ToLower().Replace("-", "_") + ".jpg", _baseUrl + azureContainer + "/" + filepart.ToLower().Replace("-", "_") + "_thumb.jpg");
                File.WriteAllText(wtmlFileOut, wtmldata);
            }

            if (hasLevels)
            {
                int maxX = 1;
                int maxY = 1;
                for (int level = 0; level < levels; level++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        for (int x = 0; x < maxX; x++)
                        {
                            void UploadItem()
                            {
                                ProcessPlateTile(tileUploader, azureContainer, plateFile, level, x, y);
                            }
                            yield return UploadItem;
                        }
                    }
                    maxX *= 2;
                    maxY *= 2;
                }
            }
        }

        private Stream GetFileStream(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);
            return new MemoryStream(data);
        }

        private void ProcessPlateTile(AzurePlateTilePyramid uploader, string container, string plateFile, int level, int x, int y)
        {
            using (var stream = PlateTilePyramid.GetFileStream(plateFile, level, x, y))
            {
                if (stream != null)
                {
                    uploader.SaveStream(stream, container, level, x, y);
                }
            }
        }


    }
}
