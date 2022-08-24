using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Layout;
using iText.Layout.Borders;
using SixLabors.ImageSharp.ColorSpaces.Conversion;

namespace Pages
{
    public class Image : IPositionable, IRenderable
    {
        private iText.Layout.Element.Image image;

        public Image(string src)
        {
            ImageData imageData = ImageDataFactory.Create(src);
            this.image = new iText.Layout.Element.Image(imageData);
        }

        public Image(byte[] bytes)
        {
            ImageData imageData = ImageDataFactory.Create(bytes);
            this.image = new iText.Layout.Element.Image(imageData);
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

        public void Crop(PdfWriter procEcriture, float decoupageGauche, float horizontalOffset, float decoupageHaut = 0, float verticalOffset = 0)
        {
            // TODO PdfContentByte cb = procEcriture.DirectContent;
            // TODO PdfTemplate t = cb.CreateTemplate(this.image.GetImageScaledWidth() - horizontalOffset, this.image.GetImageScaledHeight() - verticalOffset);
            float origWidth = this.image.GetImageScaledWidth();
            float origHeight = this.image.GetImageScaledHeight();
            // TODO t.AddImage(this.image, origWidth, 0, 0, origHeight, -decoupageGauche, -decoupageHaut);
            // TODO this.image = iText.Layout.Element.Image.GetInstance(t);
        }

        private void AddBorders()
        {
            Border border = new SolidBorder(2f);
            this.image.SetBorder(border);
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.image.SetFixedPosition(x, y - this.image.GetImageScaledHeight());
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            this.AddBorders();
            doc.Add(this.image);
        }
    }
}
