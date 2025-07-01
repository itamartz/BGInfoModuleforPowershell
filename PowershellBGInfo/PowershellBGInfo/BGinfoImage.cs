using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

namespace PowershellBGInfo
{
    [Cmdlet(VerbsCommon.New, "BGinfoImage")]
    [OutputType(typeof(BGinfoImageOutput))]
    public class BGinfoImage : PSCmdlet
    {
        Screen[] allScreens = Screen.AllScreens.ToArray();
        Screen PrimaryScreen = Screen.PrimaryScreen;

        Brush textBrush;
        Color BackBrush;
        StringFormat sf;
        Graphics drawing;
        System.Drawing.Image img;

        private BGInfoRow[] rows;
        private string textColor;
        private string backgroundcolor;
        private string imagePath;
        private string background;
        private string wallpaperPath;
        private TextPosition position;

        List<FontWithText> listFont;


        public BGinfoImage()
        {
            sf = new StringFormat();
            //sf.LineAlignment = StringAlignment.Center;
            //sf.Alignment = StringAlignment.Center;
        }

        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "This Rows will be in the imgae")]
        public BGInfoRow[] BGInfoRows
        {
            get { return rows; }
            set { rows = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "This is the string color - Default White")]
        public string TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "This is the string color - Default Black", ParameterSetName = "Color")]
        public string BackgroundColor
        {
            get { return backgroundcolor; }
            set { backgroundcolor = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The path where the image will be saved.")]
        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The base image background.", ParameterSetName = "Background")]
        public string BackgroundImage
        {
            get { return background; }
            set { background = value; }
        }


        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The position of the text on the background.", ParameterSetName = "Background")]
        public TextPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        protected override void BeginProcessing()
        {
            if (string.IsNullOrEmpty(ImagePath))
                ImagePath = Path.GetTempPath();

            switch (this.ParameterSetName)
            {
                case "Background":

                    if (BackgroundImage != null)
                    {
                        wallpaperPath = background;
                    }
                    break;
                case "Color":

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

                    break;
                default:
                    break;
            }

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

            if (!string.IsNullOrEmpty(wallpaperPath) && File.Exists(wallpaperPath))
            {
                img = new Bitmap(wallpaperPath);
            }
            else
            {
                img = new Bitmap(width, height);
            }

            drawing = Graphics.FromImage(img);

            if (string.IsNullOrEmpty(wallpaperPath))
                //paint the background
                drawing.Clear(BackBrush);

            float x = 0;
            float y = 0;

            float xExtract = GetXPosionForLongestText(img, listFont);
            int yExtract = listFont.Count * 35;

            switch (Position)
            {
                #region TOP

                case TextPosition.TopLeft:
                    x = 150;
                    y = 150;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case TextPosition.TopCenter:
                    x = (drawing.VisibleClipBounds.Width / 2);
                    y = 150;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    break;
                case TextPosition.TopRight:
                    //int textLength = listFont.Max(l => l.Text.Length);

                    x = drawing.VisibleClipBounds.Width - 200 - xExtract;
                    y = 150;
                    sf.LineAlignment = StringAlignment.Near;
                    break;

                #endregion TOP

                #region CENTER

                case TextPosition.CenterLeft:
                    x = 150;
                    y = (drawing.VisibleClipBounds.Height / 2);
                    break;

                case TextPosition.Center:
                    x = (drawing.VisibleClipBounds.Width / 2);
                    y = (drawing.VisibleClipBounds.Height / 2);
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    break;

                case TextPosition.CenterRight:
                    x = drawing.VisibleClipBounds.Width - 200 - xExtract;
                    y = (drawing.VisibleClipBounds.Height / 2);
                    sf.LineAlignment = StringAlignment.Near;
                    break;

                #endregion CENTER

                case TextPosition.BottomLeft:
                    x = 150;
                    y = drawing.VisibleClipBounds.Height - 150 - yExtract;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case TextPosition.BottomCenter:
                    x = (drawing.VisibleClipBounds.Width / 2);
                    y = drawing.VisibleClipBounds.Height - 150 - yExtract;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    break;
                case TextPosition.BottomRight:
                    x = drawing.VisibleClipBounds.Width - 200 - xExtract;
                    y = drawing.VisibleClipBounds.Height - 150 - yExtract;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                default:
                    break;
            }

            //string strFormat = string.Empty;
            foreach (FontWithText item in listFont)
            {
                //x = x - (item.Text.Length / 2);
                drawing.DrawString(item.Text, item.Font, textBrush, x, y, sf);
                SizeF sizef = drawing.MeasureString(item.Text, item.Font);
                y = y + sizef.Height;
            }


        }

        private float GetXPosionForLongestText(System.Drawing.Image img, List<FontWithText> listFont)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                var longest = listFont.OrderByDescending(i => i.Text.Length).First();
                SizeF f = g.MeasureString(longest.Text, longest.Font);

                return f.Width;


            }
        }

        protected override void EndProcessing()
        {
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();

            //img.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "BGinfoImage.bmp"));
            img.Save(Path.Combine(ImagePath, "BGinfoImage.bmp"));
            WriteObject(new BGinfoImageOutput() { Image = img, ImageName = "BGinfoImage.bmp", ImagePath = this.ImagePath });
        }

    }

    public class BGinfoImageOutput
    {
        public System.Drawing.Image Image { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
    }

    public enum TextPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}
