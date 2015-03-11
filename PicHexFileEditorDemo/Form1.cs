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
        private List<DataFoundStringInfo> dataFoundStringInfoList;

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
            button2.Enabled = false;
            button3.Enabled = false;

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Select a PIC hex file";
            openFileDialog1.Filter = "Hex File (.hex)|*.hex";
            openFileDialog1.FileName = "";

            saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog1.Filter = "Hex File (.hex)|*.hex";
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
                    displayHexFileDetails(picHexHelper.dataHexFileLineList);
                    button2.Enabled = true;
                }
            }
        } 
        #endregion

        #region displayHexFileDetails
        private void displayHexFileDetails(List<DataHexFileLine> dataHexFileLineList)
        {
            string lineAttributeString = "";
            string lineContentString = "";
            string lineChecksumString = "";
            for (int i = 0; i < dataHexFileLineList.Count; i++)
            {
                DataHexFileLine dataHexFileLine = dataHexFileLineList.ElementAt(i);
                //Line      DataCount Address Type
                lineAttributeString += String.Format("{0,-6}:{1, 13:X2} {2, 7:X4} {3, 4:X2}\n",
                    dataHexFileLine.lineIndex,
                    dataHexFileLine.lineDataCount,
                    dataHexFileLine.lineDataAddress,
                    dataHexFileLine.lineDataType);
                //D0   D1   D2   D3   D4   D5   D6   D7   D8   D9   D10  D11  D12  D13  D14  D15
                lineContentString += dataHexFileLine.lineDatasString + "\n";
                lineChecksumString += String.Format("{0:X2}\n", dataHexFileLine.lineChecksum);
            }
            richTextBox1.Text = lineAttributeString;
            richTextBox2.Text = lineContentString;
            richTextBox3.Text = lineChecksumString;
        } 
        #endregion

        #region button2_Click [Search]
        private void button2_Click(object sender, EventArgs e)
        {
            if ((textBox2.Text.Length % 2) != 0)
                textBox2.Text = textBox2.Text + "0";
            List<String> comboBoxList = new List<string>();
            if (picHexHelper != null)
            {
                picHexHelper.searchForString(textBox2.Text);
                dataFoundStringInfoList = picHexHelper.dataFoundStringInfoList;
                foreach (DataFoundStringInfo dataFoundStringInfo in dataFoundStringInfoList)
                {
                    Console.WriteLine("Line Index: {0} Char Index: {1} LineData: {2}",
                        dataFoundStringInfo.lineIndex,
                        dataFoundStringInfo.charIndex,
                        picHexHelper.dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex).lineDatasString);
                    comboBoxList.Add("Line: " + dataFoundStringInfo.lineIndex);
                }
                if (dataFoundStringInfoList.Count() == 0)
                    button3.Enabled = false;
                else
                    button3.Enabled = true;
                
                if (comboBoxList.Count > 1)
                {
                    comboBoxList.Add("All");
                }
                comboBox1.DataSource = comboBoxList;
            }
            else
            {
                MessageBox.Show("Please load a HEX file", "No HEX file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region button3_Click [Replace]
        private void button3_Click(object sender, EventArgs e)
        {
            Boolean result = false;
            if(textBox2.Text.Length > textBox4.Text.Length)
            {
                DialogResult dialogResult = MessageBox.Show(this, "Replacing with text that shorter than original. Append with spaces?", "Warnning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if(dialogResult == DialogResult.OK)
                {
                    String replaceString = textBox4.Text;
                    for(int i = textBox4.Text.Length; i<textBox2.Text.Length; i+=2)
                    {
                        replaceString += "20";
                        result = picHexHelper.replaceWithStringAtFoundIndex(replaceString, comboBox1.SelectedIndex);
                    }
                }
            }
            else if(textBox2.Text.Length < textBox4.Text.Length)
            {
                DialogResult dialogResult = MessageBox.Show(this, "Replacing with text that longer than original. truncate text?", "Warnning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    String replaceString = textBox4.Text.Substring(0, textBox2.Text.Length);
                    result = picHexHelper.replaceWithStringAtFoundIndex(replaceString, comboBox1.SelectedIndex);
                }
            }
            else
            {
                result = picHexHelper.replaceWithStringAtFoundIndex(textBox4.Text, comboBox1.SelectedIndex);
                //DataFoundStringInfo dataFoundStringInfo = picHexHelper.dataFoundStringInfoList.ElementAt(comboBox1.SelectedIndex);
                //DataHexFileLine dataHexFileLine = picHexHelper.dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex);
                //Console.WriteLine(dataHexFileLine.lineString);
            }
            if(result)
            {
                button4.Enabled = true;
                displayHexFileDetails(picHexHelper.dataHexFileLineList);
                if(comboBox1.Text.StartsWith("Line"))
                {
                    //Console.WriteLine("RichTextBox2 refreshed - trying to re-select");
                    DataFoundStringInfo dataFoundStringInfo = dataFoundStringInfoList.ElementAt(comboBox1.SelectedIndex);
                    int richTextBoxLine = dataFoundStringInfo.lineIndex;
                    richTextBox2.SelectionStart = richTextBox2.Find(richTextBox2.Lines[richTextBoxLine]) + dataFoundStringInfo.charIndex;
                    int selectionLength = textBox2.Text.Length;
                    if (dataFoundStringInfo.charIndex + selectionLength > picHexHelper.dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex).lineDataCount * 2)
                        selectionLength++;
                    richTextBox2.SelectionLength = selectionLength;
                    richTextBox2.SelectionBackColor = Color.RoyalBlue;
                    richTextBox2.ScrollToCaret();
                }
            }
        } 
        #endregion

        #region button4_Click [Save] 
        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = String.Format("ModifiedHex_{0:yyyy}_{0:MM}_{0:dd}_{0:hh}_{0:mm}_{0:ss}", DateTime.Now);
            DialogResult dialogResult = saveFileDialog1.ShowDialog();
            if(dialogResult == DialogResult.OK)
            {
                String filePath = saveFileDialog1.FileName;
                //Console.WriteLine(filePath);
                if(picHexHelper.saveModifiedHexToFile(filePath))
                {
                    MessageBox.Show("File saved to: " + filePath, "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error occured", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        } 
        #endregion

        #region comboBox1_SelectedIndexChanged
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.WriteLine("ComboBox1_SelectionChangeCommitted");
            //Console.WriteLine(comboBox1.Text);
            if (comboBox1.Text.StartsWith("Line"))
            {
                //Console.WriteLine("Combo Box item start with line");
                DataFoundStringInfo dataFoundStringInfo = dataFoundStringInfoList.ElementAt(comboBox1.SelectedIndex);
                int richTextBoxLine = dataFoundStringInfo.lineIndex;

                richTextBox2.SelectAll();
                richTextBox2.SelectionBackColor = SystemColors.Control;

                richTextBox2.SelectionStart = richTextBox2.Find(richTextBox2.Lines[richTextBoxLine]) + dataFoundStringInfo.charIndex;
                int selectionLength = textBox2.Text.Length;
                if (dataFoundStringInfo.charIndex + selectionLength > picHexHelper.dataHexFileLineList.ElementAt(dataFoundStringInfo.lineIndex).lineDataCount * 2)
                    selectionLength++;
                richTextBox2.SelectionLength = selectionLength;
                richTextBox2.SelectionBackColor = Color.RoyalBlue;
                richTextBox2.ScrollToCaret();
                richTextBox1.SelectionStart = richTextBox1.Find(richTextBox1.Lines[richTextBoxLine]);
                richTextBox1.ScrollToCaret();
                richTextBox3.SelectionStart = richTextBox3.Find(richTextBox3.Lines[richTextBoxLine]);
                richTextBox3.ScrollToCaret();
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
            textBox2.Text = StaticUtilityClass.convertStringToHexString(textBox1.Text);
        } 
        #endregion
        
        #region textBox2_TextChanged
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!StaticUtilityClass.isOnlyHexInString(textBox2.Text) && textBox2.Text != "")
            {
                MessageBox.Show("Only 0-9 and A-F are valid", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region textBox3_TextChanged
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            textBox4.Text = StaticUtilityClass.convertStringToHexString(textBox3.Text);
        } 
        #endregion

        #region textBox4_TextChanged
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if(!StaticUtilityClass.isOnlyHexInString(textBox4.Text) && textBox4.Text != "")
            {
                MessageBox.Show("Only 0-9 and A-F are valid", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 
        #endregion

        #region radioButton1_CheckedChanged
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
            textBox1.Enabled = radioButton1.Checked;
            textBox2.Enabled = !radioButton1.Checked;
        } 
        #endregion

        #region radioButton2_CheckedChanged
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
            textBox1.Enabled = !radioButton2.Checked;
            textBox2.Enabled = radioButton2.Checked;
        } 
        #endregion

        #region radioButton3_CheckedChanged
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton4.Checked = !radioButton3.Checked;
            textBox3.Enabled = radioButton3.Checked;
            textBox4.Enabled = !radioButton3.Checked;
        } 
        #endregion

        #region radioButton4_CheckedChanged
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton3.Checked = !radioButton4.Checked;
            textBox3.Enabled = !radioButton4.Checked;
            textBox4.Enabled = radioButton4.Checked;
        } 
        #endregion

    }
}
