using iText.Kernel.Pdf;
using iText.Layout;
using Pages.Elements;
using System.Xml;

namespace Pages
{
    class Description : Text
    {
        protected bool visible = true;

        public Description(XmlNode element, Panel parent) : base(element, parent)
        {
            this.visible = element.Attributes["visible"] != null
                ? bool.Parse(element.Attributes["visible"].InnerText)
                : true;
            // this.font.SetColor(255, 0, 0);
        }

        public override void Render(Document doc, PdfWriter writer)
        {
            if (this.visible)
            {
                base.Render(doc, writer);
            }
        }
    }
}
