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
    class Panel : IPositionable, IRenderable
    {
        Image image;
        List<Element> elements = new List<Element>();

        public Panel(XmlNode xmlPanel)
        {
            if (xmlPanel.Attributes["image"] == null)
                throw new Exception("A panel must have an image attribute");

            string imageSrc = xmlPanel.Attributes["image"].InnerText;
            this.image = new Image(@"..\..\..\..\bd1\BD1\" + imageSrc);

            foreach (XmlNode xmlElement in xmlPanel.ChildNodes)
            {
                Element element = null;
                if (xmlElement.Name == "description")
                    element = new Description(xmlElement);

                this.elements.Add(element);
            }
        }

        public void SetHeight(float height)
        {
            this.image.SetHeight(height);
        }

        public float getLargeur()
        {
            return this.image.getLargeur();
        }
        public float getHauteur()
        {
            return this.image.getHauteur();
        }

        public void Decouper(PdfWriter procEcriture, float decoupageGauche, float offset)
        {
            this.image.Decouper(procEcriture, decoupageGauche, offset);
            foreach (Element element in elements)
                element.Decouper(procEcriture, decoupageGauche, offset);
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.image.SetPosition(x, y);
            this.elements.ForEach(element => element.SetPosition(x, y));
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            this.image.Render(doc, writer);
            this.elements.ForEach(element => element.Render(doc, writer));
        }
    }
}
