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
        private float horizontalPadding = 8f;
        private float verticalPadding = 4f;

        public Description(XmlNode element) : base(element)
        {
            this.text = element.Attributes["text"]?.InnerText;
        }

        public void AjouterBordures()
        {
            /*this.image.Border = Rectangle.BOX;
            this.image.BorderColor = BaseColor.BLACK;
            this.image.BorderWidth = 2f;*/
        }

        private Tuple<float, float> GetRenderedSize(PdfWriter writer, float width)
        {
            float top;
            float bottom;
            Paragraph paragraph = new Paragraph(this.text, FontFactory.GetFont("dax-black"));
            ColumnText ct = new ColumnText(writer.DirectContent);
            ct.SetSimpleColumn(new Rectangle(0, 0, width, 1000f));
            ct.AddElement(paragraph);
            top = ct.YLine;
            ct.Go();
            bottom = ct.YLine;
            return new Tuple<float, float>(ct.LastX, top - bottom);
        }

        // IRenderable
        public override void Render(Document doc, PdfWriter writer)
        {
            Tuple<float, float> textSize = this.GetRenderedSize(writer, 500f); // TODO 500f should be panel width

            PdfContentByte cb = writer.DirectContent;
            cb.Rectangle(
                this.position.X,
                this.position.Y - textSize.Item2 - this.verticalPadding,
                textSize.Item1 + 2 * this.horizontalPadding,
                textSize.Item2 + this.verticalPadding
            );
            cb.SetColorFill(BaseColor.WHITE);
            cb.SetColorStroke(BaseColor.BLACK);
            cb.SetLineWidth(2f);
            cb.FillStroke();

            Paragraph paragraph = new Paragraph(this.text, FontFactory.GetFont("dax-black"));
            ColumnText ct = new ColumnText(writer.DirectContent);
            cb.SetColorFill(BaseColor.BLACK);
            Console.WriteLine(textSize.Item1);
            ct.SetSimpleColumn(
                new Rectangle(
                    (float) this.position.X + this.horizontalPadding,
                    (float) this.position.Y + this.verticalPadding,
                    (float) this.position.X + textSize.Item1 + 2 * this.horizontalPadding,
                    (float) this.position.Y - textSize.Item2 + this.verticalPadding
                )
            );
            ct.AddElement(paragraph);
            ct.Go();
        }
    }
}
