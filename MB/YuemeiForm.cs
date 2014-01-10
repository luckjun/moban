using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;

namespace MB
{
    public partial class YuemeiForm : Form
    {
        public YuemeiForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str1 = @"D:\Personal\aaa.jpg";
            string str = @"D:\Personal\aaa.ppt";
            string strKeyWord = "1ppt";
            ApplicationClass pptApplication = new ApplicationClass();
            Presentation pptPresentation = pptApplication.Presentations.Open(str, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
            //pptPresentation.Slides.FindBySlideID(0).Export(str1, "jpg", 320, 240);
            foreach (Microsoft.Office.Interop.PowerPoint.Slide slide in pptPresentation.Slides)
            {
                //slide.Export(str1, "jpg", 320, 240);
                foreach (Microsoft.Office.Interop.PowerPoint.Shape shape in slide.Shapes)
                {
                    if (MsoTriState.msoTrue == shape.HasTextFrame)
                    {
                        TextRange oText = null;
                        oText = shape.TextFrame.TextRange.Find(strKeyWord, 0, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue);
                        if (oText != null)
                        {
                            MessageBox.Show("文档中包含指定的关键字  ！", "搜索结果", MessageBoxButtons.OK);
                            slide.Delete();
                        }
                    }
                    
                    //shape.Delete();
                    //pptPresentation.SaveAs(@"D:\Personal\bb.ppt", Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsPresentation, MsoTriState.msoTrue);
                    //pptPresentation.SlideShowSettings.Run();
                }
            }
            pptPresentation.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        
    }
}
