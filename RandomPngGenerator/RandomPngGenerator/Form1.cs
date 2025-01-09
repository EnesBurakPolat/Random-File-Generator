using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomPngGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtFileSize.Text, out double fileSizeInGB) || fileSizeInGB <= 0)
            {
                MessageBox.Show("Geçerli bir dosya boyutu girin (GB cinsinden).", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileName = txtFileName.Text;
            string extension = cmbExtension.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(extension))
            {
                MessageBox.Show("Geçerli bir dosya adı ve uzantısı girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderBrowser.SelectedPath;
                    string fullPath = Path.Combine(folderPath, $"{fileName}.{extension}");

                    try
                    {
                        progressBar.Value = 0;
                        progressBar.Visible = true;

                        await Task.Run(() =>
                        {
                            GenerateRandomBinaryFile(fullPath, (long)(fileSizeInGB * 1024 * 1024 * 1024));
                        });

                        MessageBox.Show($"Dosya başarıyla oluşturuldu: {fullPath}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Dosya oluşturulurken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        progressBar.Visible = false;
                    }
                }
            }
        }

        private void GenerateRandomBinaryFile(string filePath, long sizeInBytes)
        {
            Random random = new Random();
            byte[] buffer = new byte[1024 * 1024]; // 1 MB buffer

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                long writtenBytes = 0;

                while (writtenBytes < sizeInBytes)
                {
                    random.NextBytes(buffer);
                    long remainingBytes = sizeInBytes - writtenBytes;
                    int bytesToWrite = (int)Math.Min(buffer.Length, remainingBytes);

                    fs.Write(buffer, 0, bytesToWrite);
                    writtenBytes += bytesToWrite;

                    // Update progress bar
                    this.Invoke(new Action(() =>
                    {
                        progressBar.Value = (int)((writtenBytes * 100) / sizeInBytes);
                    }));
                }
            }
        }
    }
}

