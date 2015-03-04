using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PicHexFileEditorLib;


namespace PicHexFileEditorDemo
{
    public partial class Form1 : Form
    {
        private PicHexHelper picHexHelper;
        
        #region Constructor
        public Form1()
        {
            InitializeComponent();
            customInitialization();
        } 
        #endregion

        #region customInitialization
        private void customInitialization()
        {
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Select a PIC hex file";
            openFileDialog1.Filter = "Hex File (.hex)|*.hex";
            openFileDialog1.FileName = "";
        } 
        #endregion

        #region button1_Click [Load Hex File]
        private async void button1_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                picHexHelper = new PicHexHelper();
                await picHexHelper.init(openFileDialog1.FileName);
                //Console.WriteLine(selectedHexFilePath);
            }
        } 
        #endregion
    }
}
