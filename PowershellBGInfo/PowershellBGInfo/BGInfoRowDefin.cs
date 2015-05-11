using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Management.Automation;
using System.Drawing;

namespace PowershellBGInfo
{
    [Cmdlet(VerbsCommon.New, "BGInfoRow")]
    public class BGInfoRowDefin : PSCmdlet
    {
        private object rowText;
        private string _fontFamily = "Arial";
        private int _fontSize = 12;
        string _fontStyle = "Regular";
      


        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "This text will be the BGInfo row")]
        public object RowText
        {
            get { return rowText; }
            set { rowText = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The text FontFamily, Default = Ariel")]
        public string FontFamily
        {
            get { return _fontFamily; }
            set { _fontFamily = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The text size, Default = 12")]
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The text FontStyle, Default = FontStyle.Regular")]
        [ValidateSet("Bold", "Regular","Italic","Underline","Strikeout")]
        public string FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }
            
        protected override void ProcessRecord()
        {
            //base.ProcessRecord();
            WriteVerbose("Start ProcessRecord");
            BGInfoRow row = new BGInfoRow();
            row.RowText = RowText;
            row.FontFamily = FontFamily;
            row.FontSize = FontSize;
            row.FontStyle = FontStyle;
            WriteObject(row);
            
        }
        protected override void EndProcessing()
        {
            base.EndProcessing();
        }
    }
   
}
