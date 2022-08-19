using iTextSharp.text;
using iTextSharp.text.pdf;
using Pages.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        PointF position;

        public Panel(XmlNode xmlPanel)
        {
            if (xmlPanel.Attributes["image"] == null)
                throw new Exception("A panel must have an image attribute");

            string imageSrc = xmlPanel.Attributes["image"].InnerText;
            if (File.Exists(@"..\..\..\..\..\BD\BD1\" + imageSrc))
            {
                this.image = new Image(@"..\..\..\..\..\BD\BD1\" + imageSrc);
            } else {
                this.image = new Image(Properties.Resources.temp);
            }

            foreach (XmlNode xmlElement in xmlPanel.ChildNodes)
            {
                Element element = null;
                if (xmlElement.Name == "description")
                    element = new Description(xmlElement, this);
                else if (xmlElement.Name == "text")
                    element = new Text(xmlElement, this);

                if (element != null)
                {
                    this.elements.Add(element);
                }
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

        public PointF getPosition()
        {
            return this.position;
        }

        public void Crop(PdfWriter procEcriture, float decoupageGauche, float horizontalOffset, float decoupageHaut = 0, float verticalOffset = 0)
        {
            this.image.Crop(procEcriture, decoupageGauche, horizontalOffset, decoupageHaut, verticalOffset);
            foreach (Element element in elements)
                element.Crop(procEcriture, decoupageGauche, horizontalOffset);
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.position = new PointF(x, y);
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
