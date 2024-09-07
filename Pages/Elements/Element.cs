using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Pages
{
    public abstract class Element : IPositionable, IRenderable
    {
        protected float x;
        protected float y;

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
        public abstract void Render(Document doc, PdfWriter writer, IRenderable parent);
    }
}
