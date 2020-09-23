using Azure.Core;
using Azure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WWTWebservices.Azure;

namespace PlateUploader
{
    public partial class PlateUploader : Form
    {
        public PlateUploader()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Plate Files (*.plate)|*.plate"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                listBox1.Items.AddRange(ofd.FileNames);
            }
        }

        private void Upload_Click(object sender, EventArgs e)
        {
            Browse.Enabled = false;
            Upload.Enabled = false;

            var credential = checkBox1.Checked
                ? new InteractiveBrowserCredential()
                : (TokenCredential)new DefaultAzureCredential();

            var tileUploader = new AzurePlateTilePyramid(new AzurePlateTilePyramidOptions
            {
                StorageUri = textBox1.Text,
                CreateContainer = true,
                OverwriteExisting = true
            }, credential);

            var processor = new Processor(tileUploader, textBox2.Text);
            var workItems = processor.GetActions(listBox1.Items.Cast<string>()).ToList();

            ItemCount.Text = string.Format("Item Count: {0}", workItems.Count);

            backgroundWorker1.RunWorkerAsync(workItems);
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var workItems = (IReadOnlyCollection<Action>)e.Argument;
            var itemCount = workItems.Count;
            var itemsProcessed = 0;

            Parallel.ForEach(workItems, workitem =>
            {
                workitem();
                var current = Interlocked.Increment(ref itemsProcessed);
                backgroundWorker1.ReportProgress(current * 100 / itemCount, new Progress
                {
                    ItemCount = itemCount,
                    ItemsProcessed = current,
                });
            });
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progress = (Progress)e.UserState;
            ItemCount.Text = string.Format("Item {0} of {1}", progress.ItemsProcessed, progress.ItemCount);
            progressBar1.Value = Math.Min(100, e.ProgressPercentage);
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Browse.Enabled = true;
            Upload.Enabled = true;
        }

        private class Progress
        {
            public int ItemsProcessed { get; set; }

            public int ItemCount { get; set; }
        }
    }
}
