using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Borders;
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

        public void Crop(Document doc, float decoupageGauche, float horizontalOffset, float decoupageHaut = 0, float verticalOffset = 0)
        {
            this.image.SetFixedPosition(-decoupageGauche, -decoupageHaut);
            Rectangle rectangle = new Rectangle(this.image.GetImageScaledWidth() - horizontalOffset, this.image.GetImageScaledHeight() - verticalOffset);
            PdfFormXObject template = new PdfFormXObject(rectangle);
            Canvas canvas = new Canvas(template, doc.GetPdfDocument());
            canvas.Add(this.image);
            this.image = new iText.Layout.Element.Image(template);
        }

        // IPositionable
        public void SetPosition(int noPage, float x, float y)
        {
            this.image.SetFixedPosition(noPage, x, y - this.image.GetImageScaledHeight());
        }

        // IRenderable
        public void Render(Document doc)
        {
            this.image.SetBorder(new SolidBorder(ColorConstants.BLACK, 2f));
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
