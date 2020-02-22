using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Pages.Elements
{
    class Text : Element
    {
        protected string text;
        protected const int FONT_SIZE = 15;
        protected const int LINE_HEIGHT = 16;
        protected Font font;

        public Text(XmlNode element) : base(element)
        {
            this.text = element.Attributes["text"]?.InnerText;
            // Load Font
            BaseFont customfont = BaseFont.CreateFont(@"../../Comicsam-Bold.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);
            this.font = new Font(customfont, FONT_SIZE);
        }

        public override void Render(Document doc, PdfWriter writer)
        {
            PdfContentByte cb = writer.DirectContent;
            ColumnText ct = new ColumnText(cb);
            Paragraph phrase = new Paragraph(this.text, this.font);
            ct.SetSimpleColumn(phrase, 30, 780, 250, 600, LINE_HEIGHT, 0);
            ct.Go();
        }
    }
}
