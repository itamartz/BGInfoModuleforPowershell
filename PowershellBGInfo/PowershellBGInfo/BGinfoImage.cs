using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowershellBGInfo
{
    [Cmdlet(VerbsCommon.New, "BGinfoImage")]
    public class BGinfoImage : PSCmdlet
    {
        Screen[] allScreens = Screen.AllScreens.ToArray();
        Screen PrimaryScreen = Screen.PrimaryScreen;

        Brush textBrush;
        Color BackBrush;
        StringFormat sf;
        Graphics drawing;
        Image img;

        private BGInfoRow[] rows;
        private string textColor;
        private string backgroundcolor;
        List<FontWithText> listFont;


        public BGinfoImage()
        {
            sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
        }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "This Rows will be in the imgae")]
        public BGInfoRow[] BGInfoRows
        {
            get { return rows; }
            set { rows = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "This is the string color - Default White")]
        public string TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "This is the string color - Default Black")]
        public string BackgroundColor
        {
            get { return backgroundcolor; }
            set { backgroundcolor = value; }
        }

        protected override void BeginProcessing()
        {
            //base.BeginProcessing();
            #region textBrush
            if (string.IsNullOrEmpty(TextColor))
            {
                TextColor = "White";
            }
            Color c;
            try
            {
                c = Color.FromName(TextColor);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                          ex,
                          "Bad TextColor - Set to Default White",
                          ErrorCategory.InvalidOperation,
                          TextColor));
                c = Color.White;

            }
            textBrush = new SolidBrush(c);
            #endregion

            #region BackgroundColor
            if (string.IsNullOrEmpty(BackgroundColor))
            {
                BackgroundColor = "Black";
            }
            Color back;
            try
            {
                back = Color.FromName(BackgroundColor);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                          ex,
                          "Bad TextColor - Set to Default Black",
                          ErrorCategory.InvalidOperation,
                          TextColor));
                back = Color.Black;

            }
            BackBrush = back;
            #endregion

            listFont = new List<FontWithText>();
            foreach (BGInfoRow item in BGInfoRows)
            {
                FontWithText fontwithtext = new FontWithText();

                FontFamily fontFamily = new FontFamily(item.FontFamily);
                FontStyle style = (FontStyle)Enum.Parse(typeof(FontStyle), item.FontStyle);

                Font font = new Font(fontFamily, item.FontSize, style, GraphicsUnit.Pixel);
                fontwithtext.Font = font;
                if (item.RowText is object[])
                {
                    string strobj = Environment.NewLine + Environment.NewLine;
                    object[] obj = item.RowText as object[];
                    if (obj != null)
                    {
                        foreach (object obbject in obj)
                        {
                            strobj += obbject.ToString() + Environment.NewLine;
                        }
                    }
                    fontwithtext.Text = strobj;

                }
                else
                    fontwithtext.Text = item.RowText.ToString();

                listFont.Add(fontwithtext);
            }

        }
        protected override void ProcessRecord()
        {
            //base.ProcessRecord();
            int height = (int)PrimaryScreen.WorkingArea.Height;
            int width = (int)PrimaryScreen.WorkingArea.Width;
            img = new Bitmap(width, height);
            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(BackBrush);

            //create a brush for the text
            //Brush textBrush = new SolidBrush(textColor);

            float x = (width / 2);
            //float x = (width / 2) - (text.Length / 2);
            float y = (height / 2);

            string strFormat = string.Empty;
            foreach (FontWithText item in listFont)
            {

                x = x - (item.Text.Length / 2);
                drawing.DrawString(item.Text, item.Font, textBrush, x, y, sf);
                SizeF sizef = drawing.MeasureString(item.Text, item.Font);
                y = y + sizef.Height;
            }


        }
        protected override void EndProcessing()
        {
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            img.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "BGinfoImage.bmp"));
            WriteObject(new BGinfoImageOutput() { Image = img, ImageName = "BGinfoImage.bmp", ImagePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) });
        }

    }

    public class BGinfoImageOutput
    {
        public Image Image { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
    }
}
