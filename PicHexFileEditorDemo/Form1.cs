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
using PicHexFileEditorLib.DataType;
using System.Runtime.InteropServices;
using PicHexFileEditorLib.StaticUtility;


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
                Boolean result = await picHexHelper.init(openFileDialog1.FileName);
                if(result)
                {
                    List<DataHexFileLine> dataHexFileLineList = picHexHelper.dataHexFileLineList;
                    string lineAttributeString = "";
                    string lineContentString = "";
                    string lineChecksumString = "";
                    for(int i=0; i<dataHexFileLineList.Count; i++)
                    {
                        DataHexFileLine dataHexFileLine = dataHexFileLineList.ElementAt(i);
                        //Line      DataCount Address Type
                        lineAttributeString += String.Format("{0,-6}:{1, 13:x2} {2, 7:x4} {3, 4:x2}\n\n", 
                            dataHexFileLine.lineIndex,
                            dataHexFileLine.lineDataCount,
                            dataHexFileLine.lineDataAddress,
                            dataHexFileLine.lineDataType);
                        //D0   D1   D2   D3   D4   D5   D6   D7   D8   D9   D10  D11  D12  D13  D14  D15
                        lineContentString += dataHexFileLine.lineDatasString + "\n\n";
                        lineChecksumString += String.Format("{0:x2}\n\n", dataHexFileLine.lineChecksum);
                    }
                    richTextBox1.Text = lineAttributeString;
                    richTextBox2.Text = lineContentString;
                    richTextBox3.Text = lineChecksumString;

                }
            }
        } 
        #endregion

        #region RichTextBox Scroll synchronization
        private void richTextBox2_Scrolling(object sender, EventArgs e)
        {
            try
            {
                ScrollingEventArgs scrollingEventArgs = (ScrollingEventArgs)e;
                scrollingEventArgs.m.HWnd = richTextBox1.Handle;
                richTextBox1.PublicWndProc(ref scrollingEventArgs.m);
                scrollingEventArgs.m.HWnd = richTextBox3.Handle;
                richTextBox3.PublicWndProc(ref scrollingEventArgs.m);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }  
        }
	    #endregion

        #region textBox1_TextChanged
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox2.Text = StaticUtilityClass.convertStringToHexString(textBox1.Text);
            }
        } 
        #endregion

        #region button2_Click
        private void button2_Click(object sender, EventArgs e)
        {
            if(picHexHelper != null)
            {
                List<DataFoundStringInfo> dataFoundStringInfoList = picHexHelper.searchForString(textBox2.Text);
                foreach(DataFoundStringInfo dataFoundStringInfo in dataFoundStringInfoList)
                {
                    Console.WriteLine("Line Index: {0} Char Index: {1} LineData: {2}",
                        dataFoundStringInfo.lineIndex,
                        dataFoundStringInfo.charIndex,
                        picHexHelper.dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex).lineDatasString);
                }
                if(dataFoundStringInfoList.Count() == 0)
                {
                    Console.WriteLine("Can not find "+ textBox2.Text);
                }
            }
        } 
        #endregion
    }
}
