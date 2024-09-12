using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Xml;

namespace Pages.Elements
{
    class Text : Element
    {
        protected Panel parent;
        protected string text;
        protected Color color = ColorConstants.BLACK;
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

        public override void Render(Document doc)
        {
            float left = this.parent.getPosition().X + this.left + MARGIN;
            float right;
            if (this.width is float width)
            {
                right = this.parent.getPosition().X + this.left + Math.Min(width, this.parent.GetWidth() - this.left) - MARGIN;
            }
            else
            {
                right = this.parent.getPosition().X + this.parent.GetWidth() - MARGIN;
            }

            float top = this.parent.getPosition().Y - this.top - 3;
            float bottom = this.parent.getPosition().Y - this.parent.GetHeight() + MARGIN;
            float phraseWidth = right - left;
            Paragraph phrase = new Paragraph(this.text);
            phrase.SetFixedLeading(LINE_HEIGHT);
            phrase.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.TOP);
            phrase.SetHeight(top - bottom);
            phrase.SetFont(this.font);
            phrase.SetFixedPosition(this.noPage, left, bottom, phraseWidth);
            phrase.SetFontColor(this.color);
            doc.Add(phrase);
        }
    }
}
