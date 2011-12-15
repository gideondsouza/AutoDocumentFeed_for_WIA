using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ADFScanner;
using System.Drawing.Imaging;

namespace ADFScannerTest
{
    public partial class Form1 : Form
    {
        ADFScan _scanner;
        int[] _colors = { 1,2,4};
        int count = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            _scanner = new ADFScan();
            _scanner.Scanning += new EventHandler<WiaImageEventArgs>(_scanner_Scanning);
            _scanner.ScanComplete += new EventHandler(_scanner_ScanComplete);
            ScanColor selectedColor = (ScanColor)_colors[comboBox1.SelectedIndex];
            int dpi = (int) numericUpDown1.Value;
            _scanner.BeginScan(selectedColor,dpi );
        }
        void _scanner_ScanComplete(object sender, EventArgs e)
        {
            MessageBox.Show("Scan Complete");
        }
        void _scanner_Scanning(object sender, WiaImageEventArgs e)
        {
            string filename = textBox1.Text + "image" + (count++).ToString() + ".jpg";
            listBox1.Items.Add(filename);
            e.ScannedImage.Save(filename, ImageFormat.Jpeg);//FILES ARE RETURNED AS BITMAPS
        }        
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fld = new FolderBrowserDialog();
            if (fld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = fld.SelectedPath;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                pictureBox1.Image = Image.FromFile(listBox1.SelectedItem.ToString());
            }
        }
    }
}
