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

        public void Crop(PdfWriter procEcriture, float decoupageGauche, float horizontalOffset, float decoupageHaut = 0, float verticalOffset = 0)
        {
            PdfContentByte cb = procEcriture.DirectContent;
            PdfTemplate t = cb.CreateTemplate(this.image.ScaledWidth - horizontalOffset, this.image.ScaledHeight - verticalOffset);
            float origWidth = this.image.ScaledWidth;
            float origHeight = this.image.ScaledHeight;
            t.AddImage(this.image, origWidth, 0, 0, origHeight, -decoupageGauche, -decoupageHaut);
            this.image = iTextSharp.text.Image.GetInstance(t);
        }

        private void AddBorders()
        {
            this.image.Border = Rectangle.BOX;
            this.image.BorderColor = BaseColor.BLACK;
            this.image.BorderWidth = 2f;
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.image.SetAbsolutePosition(x, y - this.image.ScaledHeight);
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            this.AddBorders();
            doc.Add(this.image);
        }
    }
}
