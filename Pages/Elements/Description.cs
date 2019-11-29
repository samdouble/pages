using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pages
{
    class Description : Element
    {
        private string text;

        public Description(XmlNode element) : base(element)
        {
            this.text = element.Attributes["text"]?.InnerText;
        }

        public override void SetHeight(float height)
        {
            float pctScaling = height / this.image.Height;
            this.image.ScalePercent(100 * pctScaling);
        }

        public override void AjouterBordures()
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
    }
}
