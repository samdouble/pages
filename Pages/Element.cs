using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows;
using System.Xml;

namespace Pages
{
    public class Element
    {
        protected iTextSharp.text.Image image;
        protected float offsetHaut;
        protected float offsetGauche;

        public Element()
        {

        }

        public Element(XmlNode element)
        {
            this.image = iTextSharp.text.Image.GetInstance(@"..\..\..\..\bd1\" + element.Attributes["src"]?.InnerText);
            this.offsetHaut = element.Attributes["haut"] != null ? float.Parse(element.Attributes["haut"].InnerText) : 0f;
            this.offsetGauche = element.Attributes["gauche"] != null ? float.Parse(element.Attributes["gauche"].InnerText) : 0f;
        }

        public virtual void Redimensionner(float hauteurCase)
        {

        }

        public virtual void Decouper(PdfWriter procEcriture, float decoupageGauche, float offset)
        {

        }

        public virtual void Positionner(Document doc, float x, float y, float margeHaut, float margeGauche)
        {
            Point position = new Point(margeGauche + x, doc.PageSize.Height - margeHaut - y - this.image.ScaledHeight);
            position.X += this.offsetGauche;
            position.Y -= this.offsetHaut;

            this.image.SetAbsolutePosition((float) position.X, (float) position.Y);
        }

        public virtual void AjouterBordures()
        {

        }

        public iTextSharp.text.Image getImage()
        {
            return this.image;
        }

        public float getLargeur()
        {
            return this.image.ScaledWidth;
        }

        public float getHauteur()
        {
            return this.image.ScaledHeight;
        }
    }
}
