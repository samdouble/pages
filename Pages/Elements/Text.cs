using System;
using System.Xml;
using System.Xml.Serialization;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Pages.Elements
{
    public class Text : Element
    {
        protected const int FONT_SIZE = 13;
        protected const int LINE_HEIGHT = 12;
        protected const int MARGIN = 5;
        [XmlAttribute]
        protected string text = string.Empty;
        [XmlAttribute]
        protected float left = 0;
        [XmlAttribute]
        protected float top = 0;
        [XmlAttribute]
        protected float? width;
        protected Font? font;

        public override void Render(Document doc, PdfWriter writer, IRenderable parent)
        {
            // Load Font
            byte[] baseFont = Properties.Resources.Comicsam_Bold;
            BaseFont customfont = BaseFont.CreateFont("Comicsam-Bold.ttf", BaseFont.CP1252, BaseFont.EMBEDDED, BaseFont.CACHED, baseFont, null);
            this.font = new Font(customfont, FONT_SIZE);

            float left = parent.GetPosition().X + this.left + MARGIN;
            float right;
            if (this.width is float width)
            {
                right = parent.GetPosition().X + this.left + Math.Min(width, parent.GetWidth() - this.left) - MARGIN;
            }
            else
            {
                right = parent.GetPosition().X + parent.GetWidth() - MARGIN;
            }

            float top = parent.GetPosition().Y - this.top;
            float bottom = parent.GetPosition().Y - parent.GetHeight() + MARGIN;
            PdfContentByte cb = writer.DirectContent;
            ColumnText ct = new ColumnText(cb);
            Paragraph phrase = new Paragraph(this.text, this.font);
            ct.SetSimpleColumn(phrase, left, top, right, bottom, LINE_HEIGHT, 0);
            ct.Go();
        }
    }
}
