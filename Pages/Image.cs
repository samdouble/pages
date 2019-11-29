using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Pages
{
    public class Image : IPositionable, IRenderable
    {
        private iTextSharp.text.Image image;

        public Image(string src)
        {
            this.image = iTextSharp.text.Image.GetInstance(src);
        }

        public void SetHeight(float height)
        {
            float pctScaling = height / this.image.Height;
            this.image.ScalePercent(100 * pctScaling);
        }

        public float getHauteur()
        {
            return this.image.ScaledHeight;
        }

        public float getLargeur()
        {
            return this.image.ScaledWidth;
        }

        public void Decouper(PdfWriter procEcriture, float decoupageGauche, float offset)
        {
            this.image = Crop(this.image, procEcriture, decoupageGauche, 0, this.image.ScaledWidth - offset, this.image.ScaledHeight);
        }

        public void AjouterBordures()
        {
            this.image.Border = Rectangle.BOX;
            this.image.BorderColor = BaseColor.BLACK;
            this.image.BorderWidth = 2f;
        }

        private static iTextSharp.text.Image Crop(iTextSharp.text.Image image, PdfWriter writer, float x, float y, float width, float height)
        {
            PdfContentByte cb = writer.DirectContent;
            PdfTemplate t = cb.CreateTemplate(width, height);
            float origWidth = image.ScaledWidth;
            float origHeight = image.ScaledHeight;
            t.AddImage(image, origWidth, 0, 0, origHeight, -x, -y);
            return iTextSharp.text.Image.GetInstance(t);
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.image.SetAbsolutePosition(x, y - this.image.ScaledHeight);
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            doc.Add(this.image);
        }
    }
}
