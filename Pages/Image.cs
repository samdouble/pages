using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using System;

namespace Pages
{
    public class Image : IPositionable, IRenderable
    {
        private iText.Layout.Element.Image image;
        protected int noPage;
        protected float x;
        protected float y;

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

        public float GetHeight()
        {
            return this.image.GetImageScaledHeight();
        }

        public float GetWidth()
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
            this.noPage = noPage;
            this.x = x;
            this.y = y;
            this.image.SetFixedPosition(this.noPage, this.x, this.y - this.image.GetImageScaledHeight());
        }

        // IRenderable
        public void Render(Document doc)
        {
            doc.Add(this.image);

            // Add borders
            iText.Kernel.Pdf.Canvas.PdfCanvas canvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(doc.GetPdfDocument().GetPage(this.noPage));
            canvas.SetStrokeColor(ColorConstants.BLACK);
            canvas.SetLineWidth(2f);
            canvas.Rectangle(this.x, this.y - image.GetImageScaledHeight(), image.GetImageScaledWidth(), image.GetImageScaledHeight());
            canvas.Stroke();
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
