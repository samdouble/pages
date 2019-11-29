using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Windows;
using System.Xml;

namespace Pages
{
    public abstract class Element : IRenderable
    {
        protected Point position;

        public Element(XmlNode element)
        {
            
        }

        public virtual void Decouper(PdfWriter procEcriture, float decoupageGauche, float offset)
        {

        }

        public virtual void Positionner(Document doc, float x, float y, float margeHaut, float margeGauche)
        {
            this.position = new Point(margeGauche + x, doc.PageSize.Height - margeHaut - y);
        }

        // IRenderable
        public abstract void Render(Document doc, PdfWriter writer);
    }
}
