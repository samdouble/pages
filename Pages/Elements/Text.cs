using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using System;
using System.Xml;

namespace Pages.Elements
{
    class Text : Element
    {
        protected Panel parent;
        protected string text;
        protected const int FONT_SIZE = 13;
        protected const int LINE_HEIGHT = 12;
        protected const int MARGIN = 5;
        protected PdfFont font;
        protected float left = 0;
        protected float top = 0;
        protected float? width;

        public Text(XmlNode element, Panel parent) : base(element)
        {
            this.parent = parent;
            this.text = element.Attributes["text"]?.InnerText;
            // Optional
            this.left = element.Attributes["left"] != null ? float.Parse(element.Attributes["left"].InnerText) : 0.0f;
            this.top = element.Attributes["top"] != null ? float.Parse(element.Attributes["top"].InnerText) : 0.0f;
            this.width = element.Attributes["width"] != null ? float.Parse(element.Attributes["width"].InnerText) : (float?)null;
            // Load Font
            this.font = PdfFontFactory.CreateFont(Properties.Resources.Comicsam_Bold, PdfEncodings.CP1252);
        }

        public override void Render(Document doc, PdfWriter writer)
        {
            float left = this.parent.getPosition().X + this.left + MARGIN;
            float right;
            if (this.width is float width)
            {
                right = this.parent.getPosition().X + this.left + Math.Min(width, this.parent.getLargeur() - this.left) - MARGIN;
            }
            else
            {
                right = this.parent.getPosition().X + this.parent.getLargeur() - MARGIN;
            }

            float top = this.parent.getPosition().Y - this.top;
            float bottom = this.parent.getPosition().Y - this.parent.getHauteur() + MARGIN;
            // PdfContentByte cb = writer.DirectContent;
            // ColumnText ct = new ColumnText(cb);
            // Paragraph phrase = new Paragraph(this.text, this.font);
            // ct.SetSimpleColumn(phrase, left, top, right, bottom, LINE_HEIGHT, 0);
            // ct.Go();
        }
    }
}
