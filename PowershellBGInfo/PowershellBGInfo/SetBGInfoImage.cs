using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PowershellBGInfo
{
    [Cmdlet(VerbsCommon.Set, "BGInfoImage")]
    public class SetBGInfoImage : PSCmdlet
    {
        //[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "This Is Image Object")]
        //public Image Image { get; set; }

        //[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The image name ")]
        //public string ImageName { get; set; }

        //[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The Image folder path")]
        //public string ImagePath { get; set; }


        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "BGinfoImage Object")]
        public BGinfoImageOutput BGinfoImageOutput { get; set; }

        protected override void BeginProcessing()
        {
            //base.BeginProcessing();
            #region  if (Image == null)
            //if (Image == null)
            //{
            //    try
            //    {
            //        string inputString = Path.Combine(ImagePath, ImageName);
            //        byte[] imageBytes = Encoding.Unicode.GetBytes(inputString);
            //        MemoryStream ms = new MemoryStream(imageBytes);
            //        Image = Image.FromStream(ms, true, true);

            //    }
            //    catch (Exception ex)
            //    {
            //        WriteError(new ErrorRecord(ex, "Bad ImageName Or bad ImagePath", ErrorCategory.InvalidOperation, null));
            //    }
            //}
            #endregion

        }
        protected override void ProcessRecord()
        {
            //base.ProcessRecord();
            Wallpaper.Set(BGinfoImageOutput.Image, Wallpaper.Style.Stretched);
        }

    }
}
