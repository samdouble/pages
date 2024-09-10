using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using System;

namespace Pages
{
    public class Image : IPositionable, IRenderable
    {
        private iText.Layout.Element.Image image;

        public Image(string src)
        {
            this.image = new iText.Layout.Element.Image(ImageDataFactory.Create(src));
        }

        public Image(byte[] bytes)
        {
            this.image = new iText.Layout.Element.Image(ImageDataFactory.Create(bytes));
        }

        public void SetHeight(float height)
        {
            float pctScaling = height / this.image.GetImageHeight();
            this.image.Scale(pctScaling, pctScaling);
        }

        public float getHauteur()
        {
            return this.image.GetImageScaledHeight();
        }

        public float getLargeur()
        {
            return this.image.GetImageScaledWidth();
        }

        private void AddBorders()
        {
            // this.image.AddStyle()
            // this.image.Border = Rectangle.BOX;
            // this.image.BorderColor = BaseColor.BLACK;
            // this.image.BorderWidth = 2f;
        }

        public void Crop(PdfWriter procEcriture, float decoupageGauche, float horizontalOffset, float decoupageHaut = 0, float verticalOffset = 0)
        {
            // PdfContentByte cb = procEcriture.DirectContent;
            // PdfTemplate t = cb.CreateTemplate(this.image.ScaledWidth - horizontalOffset, this.image.ScaledHeight - verticalOffset);
            // float origWidth = this.image.ScaledWidth;
            // float origHeight = this.image.ScaledHeight;
            // t.AddImage(this.image, origWidth, 0, 0, origHeight, -decoupageGauche, -decoupageHaut);
            // this.image = new iText.Layout.Element.Image(t);
        }

        // IPositionable
        public void SetPosition(int noPage, float x, float y)
        {
            this.image.SetFixedPosition(noPage, x, y - this.image.GetImageScaledHeight());
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            this.AddBorders();
            Console.WriteLine("HELLO");
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
