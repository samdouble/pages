using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Xml;
using System.Xml.Serialization;
using Pages.Elements;

namespace Pages
{
    public class Description : Text
    {
        [XmlAttribute]
        protected bool visible = true;

        public override void Render(Document doc, PdfWriter writer, IRenderable parent)
        {
            this.font.SetColor(255, 0, 0);
            if (this.visible)
            {
                base.Render(doc, writer, parent);
            }
        }
    }
}
