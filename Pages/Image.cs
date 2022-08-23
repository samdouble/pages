using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;

namespace Pages
{
    public class Image : IPositionable, IRenderable
    {
        private iText.Layout.Element.Image image;

        public Image(string src)
        {
            this.image = iText.Layout.Element.Image.GetInstance(src);
        }

        public Image(byte[] bytes)
        {
            this.image = iText.Layout.Element.Image.GetInstance(bytes);
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
            PdfContentByte cb = procEcriture.DirectContent;
            PdfTemplate t = cb.CreateTemplate(this.image.GetImageScaledWidth() - horizontalOffset, this.image.GetImageScaledHeight() - verticalOffset);
            float origWidth = this.image.GetImageScaledWidth();
            float origHeight = this.image.GetImageScaledHeight();
            t.AddImage(this.image, origWidth, 0, 0, origHeight, -decoupageGauche, -decoupageHaut);
            this.image = iTextSharp.text.Image.GetInstance(t);
        }

        private void AddBorders()
        {
            Border border = new SolidBorder(Black, 2f);
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
