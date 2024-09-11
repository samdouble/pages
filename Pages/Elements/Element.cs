using iText.Kernel.Pdf;
using iText.Layout;
using System.Xml;

namespace Pages
{
    public abstract class Element : IPositionable, IRenderable
    {
        protected int noPage;
        protected float x;
        protected float y;

        public Element(XmlNode element)
        {

        }

        public virtual void Crop(Document doc, float decoupageGauche, float offset)
        {

        }

        // IPositionable
        public void SetPosition(int noPage, float x, float y)
        {
            this.noPage = noPage;
            this.x = x;
            this.y = y;
        }

        // IRenderable
        public abstract void Render(Document doc);
    }
}
