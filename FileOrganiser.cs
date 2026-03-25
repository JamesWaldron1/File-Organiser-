using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace FileOrganizer
{
    public class FileOrganizerForm : Form
    {
        private TextBox csvPathBox;
        private TextBox sourcePathBox;
        private TextBox destPathBox;
        private TextBox resultsBox;
        private Label statusLabel;

        public FileOrganizerForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "File Organizer";
            this.Size = new Size(650, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Title
            Label titleLabel = new Label();
            titleLabel.Text = "File Organizer";
            titleLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            titleLabel.Location = new Point(20, 20);
            titleLabel.Size = new Size(600, 25);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(titleLabel);

            Label subtitleLabel = new Label();
            subtitleLabel.Text = "Organize files based on a CSV with 'name' and 'output' columns";
            subtitleLabel.Location = new Point(20, 50);
            subtitleLabel.Size = new Size(600, 20);
            this.Controls.Add(subtitleLabel);

            // CSV File Selection
            Label csvLabel = new Label();
            csvLabel.Text = "1. CSV File:";
            csvLabel.Location = new Point(20, 90);
            csvLabel.Size = new Size(200, 20);
            this.Controls.Add(csvLabel);

            csvPathBox = new TextBox();
            csvPathBox.Location = new Point(20, 110);
            csvPathBox.Size = new Size(500, 20);
            csvPathBox.ReadOnly = true;
            this.Controls.Add(csvPathBox);

            Button csvBrowseBtn = new Button();
            csvBrowseBtn.Text = "Browse...";
            csvBrowseBtn.Location = new Point(530, 110);
            csvBrowseBtn.Size = new Size(80, 25);
            csvBrowseBtn.Click += BrowseCsv;
            this.Controls.Add(csvBrowseBtn);

            // Source Folder Selection
            Label sourceLabel = new Label();
            sourceLabel.Text = "2. Files Location:";
            sourceLabel.Location = new Point(20, 150);
            sourceLabel.Size = new Size(200, 20);
            this.Controls.Add(sourceLabel);

            sourcePathBox = new TextBox();
            sourcePathBox.Location = new Point(20, 170);
            sourcePathBox.Size = new Size(500, 20);
            sourcePathBox.ReadOnly = true;
            this.Controls.Add(sourcePathBox);

            Button sourceBrowseBtn = new Button();
            sourceBrowseBtn.Text = "Browse...";
            sourceBrowseBtn.Location = new Point(530, 170);
            sourceBrowseBtn.Size = new Size(80, 25);
            sourceBrowseBtn.Click += BrowseSource;
            this.Controls.Add(sourceBrowseBtn);

            // Destination Folder Selection
            Label destLabel = new Label();
            destLabel.Text = "3. Output Root:";
            destLabel.Location = new Point(20, 210);
            destLabel.Size = new Size(200, 20);
            this.Controls.Add(destLabel);

            destPathBox = new TextBox();
            destPathBox.Location = new Point(20, 230);
            destPathBox.Size = new Size(500, 20);
            destPathBox.ReadOnly = true;
            this.Controls.Add(destPathBox);

            Button destBrowseBtn = new Button();
            destBrowseBtn.Text = "Browse...";
            destBrowseBtn.Location = new Point(530, 230);
            destBrowseBtn.Size = new Size(80, 25);
            destBrowseBtn.Click += BrowseDest;
            this.Controls.Add(destBrowseBtn);

            // Organize Button
            Button organizeBtn = new Button();
            organizeBtn.Text = "Organize Files";
            organizeBtn.Location = new Point(250, 280);
            organizeBtn.Size = new Size(120, 35);
            organizeBtn.BackColor = Color.FromArgb(76, 175, 80);
            organizeBtn.ForeColor = Color.White;
            organizeBtn.FlatStyle = FlatStyle.Flat;
            organizeBtn.Font = new Font("Arial", 10, FontStyle.Bold);
            organizeBtn.Click += OrganizeFiles;
            this.Controls.Add(organizeBtn);

            // Results Log
            Label resultsLabel = new Label();
            resultsLabel.Text = "Results:";
            resultsLabel.Location = new Point(20, 330);
            resultsLabel.Size = new Size(200, 20);
            this.Controls.Add(resultsLabel);

            resultsBox = new TextBox();
            resultsBox.Location = new Point(20, 350);
            resultsBox.Size = new Size(590, 150);
            resultsBox.Multiline = true;
            resultsBox.ScrollBars = ScrollBars.Vertical;
            resultsBox.ReadOnly = true;
            resultsBox.Font = new Font("Consolas", 9);
            this.Controls.Add(resultsBox);

            // Status Bar
            statusLabel = new Label();
            statusLabel.Text = "Ready";
            statusLabel.Location = new Point(0, 510);
            statusLabel.Size = new Size(650, 25);
            statusLabel.BorderStyle = BorderStyle.Fixed3D;
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            statusLabel.Padding = new Padding(5, 0, 0, 0);
            this.Controls.Add(statusLabel);
        }

        private void BrowseCsv(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                dialog.Title = "Select CSV File";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    csvPathBox.Text = dialog.FileName;
                    AppendLog("Selected CSV: " + dialog.FileName);
                }
            }
        }

        private void BrowseSource(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select Folder Containing Files to Organize";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    sourcePathBox.Text = dialog.SelectedPath;
                    AppendLog("Selected source folder: " + dialog.SelectedPath);
                }
            }
        }

        private void BrowseDest(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select Root Folder for Organized Files";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    destPathBox.Text = dialog.SelectedPath;
                    AppendLog("Selected destination folder: " + dialog.SelectedPath);
                }
            }
        }

        private void OrganizeFiles(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(csvPathBox.Text))
            {
                MessageBox.Show("Please select a CSV file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(sourcePathBox.Text))
            {
                MessageBox.Show("Please select the source folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(destPathBox.Text))
            {
                MessageBox.Show("Please select the destination folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Clear log
            resultsBox.Clear();

            statusLabel.Text = "Processing files...";
            AppendLog("============================================================");
            AppendLog("Starting file organization...");
            AppendLog("============================================================");

            int successCount = 0;
            int notFoundCount = 0;
            int errorCount = 0;
            int totalCount = 0;

            try
            {
                // Read CSV file
                string[] lines = File.ReadAllLines(csvPathBox.Text);
                if (lines.Length == 0)
                {
                    MessageBox.Show("CSV file is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Parse header
                string[] headers = lines[0].Split(',');
                int nameCol = -1;
                int outputCol = -1;

                for (int i = 0; i < headers.Length; i++)
                {
                    string header = headers[i].Trim().Trim('"');
                    if (header.Equals("name", StringComparison.OrdinalIgnoreCase))
                        nameCol = i;
                    if (header.Equals("output", StringComparison.OrdinalIgnoreCase))
                        outputCol = i;
                }

                if (nameCol == -1 || outputCol == -1)
                {
                    MessageBox.Show("CSV must have 'name' and 'output' columns", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                AppendLog("Found " + (lines.Length - 1).ToString() + " entries in CSV");

                // Process each row
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] columns = lines[i].Split(',');
                    if (columns.Length <= Math.Max(nameCol, outputCol))
                        continue;

                    string fileName = columns[nameCol].Trim().Trim('"');
                    string outputPath = columns[outputCol].Trim().Trim('"');

                    if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(outputPath))
                        continue;

                    totalCount++;

                    string sourceFile = Path.Combine(sourcePathBox.Text, fileName);
                    string destFolder = Path.Combine(destPathBox.Text, outputPath);
                    string destFile = Path.Combine(destFolder, fileName);

                    // Check if source file exists
                    if (!File.Exists(sourceFile))
                    {
                        AppendLog("✗ Not found: " + fileName);
                        notFoundCount++;
                        continue;
                    }

                    // Create destination folder
                    try
                    {
                        Directory.CreateDirectory(destFolder);
                    }
                    catch
                    {
                        AppendLog("✗ Error creating folder for: " + fileName);
                        errorCount++;
                        continue;
                    }

                    // Copy file
                    try
                    {
                        File.Copy(sourceFile, destFile, true);
                        AppendLog("✓ " + fileName + " → " + outputPath);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        AppendLog("✗ Error copying: " + fileName);
                        errorCount++;
                    }

                    statusLabel.Text = "Processing " + i.ToString() + "/" + totalCount.ToString() + "...";
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error";
                return;
            }

            // Show summary
            AppendLog("");
            AppendLog("============================================================");
            AppendLog("SUMMARY");
            AppendLog("============================================================");
            AppendLog("✓ Successfully organized: " + successCount.ToString());
            AppendLog("✗ Not found: " + notFoundCount.ToString());
            AppendLog("✗ Errors: " + errorCount.ToString());
            AppendLog("============================================================");

            statusLabel.Text = "Complete!";

            MessageBox.Show("Successfully organized: " + successCount.ToString() + "\nNot found: " + notFoundCount.ToString() + "\nErrors: " + errorCount.ToString(),
                "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AppendLog(string message)
        {
            if (resultsBox.Text.Length > 0)
                resultsBox.AppendText(Environment.NewLine);
            resultsBox.AppendText(message);
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FileOrganizerForm());
        }
    }
}
