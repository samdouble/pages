using System;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Pages
{
    public class Image : IPositionable, IRenderable
    {
        private iTextSharp.text.Image image;

        public Image(string src)
        {
            this.image = iTextSharp.text.Image.GetInstance(src);
        }

        public Image(byte[] bytes)
        {
            this.image = iTextSharp.text.Image.GetInstance(bytes);
        }

        private void AddBorders()
        {
            this.image.Border = Rectangle.BOX;
            this.image.BorderColor = BaseColor.BLACK;
            this.image.BorderWidth = 2f;
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

        public float GetHeight()
        {
            return this.image.ScaledHeight;
        }

        public float GetWidth()
        {
            return this.image.ScaledWidth;
        }

        public void SetHeight(float height)
        {
            float pctScaling = height / this.image.Height;
            this.image.ScalePercent(100 * pctScaling);
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.image.SetAbsolutePosition(x, y - this.image.ScaledHeight);
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer, IRenderable parent)
        {
            this.AddBorders();
            doc.Add(this.image);
        }

        public static bool IsPrime(int candidate)
        {
            if (candidate == 1)
            {
                return false;
            }
            throw new NotImplementedException("Not fully implemented.");
        }
    }
}
