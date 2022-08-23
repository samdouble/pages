using iText.Kernel.Pdf;
using iText.Layout;
using System.Xml;

namespace Pages
{
    public abstract class Element : IPositionable, IRenderable
    {
        protected float x;
        protected float y;

        public Element(XmlNode element)
        {
            
        }

        public virtual void Crop(PdfWriter procEcriture, float decoupageGauche, float offset)
        {

        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // IRenderable
        public abstract void Render(Document doc, PdfWriter writer);
    }
}
