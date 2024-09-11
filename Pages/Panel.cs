using iText.Kernel.Pdf;
using iText.Layout;
using Pages.Elements;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Pages
{
    class Panel : IPositionable, IRenderable
    {
        private Comic parent;
        Image image;
        List<Element> elements = new List<Element>();
        PointF position;

        public Panel(Comic parent, XmlNode xmlPanel)
        {
            this.parent = parent;
            if (xmlPanel.Attributes["image"] == null)
                throw new Exception("A panel must have an image attribute");

            string imageSrc = xmlPanel.Attributes["image"].InnerText;
            string imagesFolderPath = parent.GetImagesFolderPath();

            Console.WriteLine("Getting image at " + (@"" + imagesFolderPath + '/' + imageSrc));
            if (File.Exists(@"" + imagesFolderPath + '/' + imageSrc))
            {
                this.image = new Image(@"" + imagesFolderPath + '/' + imageSrc);
            }
            else
            {
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

        public void Crop(Document doc, PdfWriter procEcriture, float decoupageGauche, float horizontalOffset, float decoupageHaut = 0, float verticalOffset = 0)
        {
            this.image.Crop(doc, procEcriture, decoupageGauche, horizontalOffset, decoupageHaut, verticalOffset);
            foreach (Element element in elements)
                element.Crop(doc, procEcriture, decoupageGauche, horizontalOffset);
        }

        // IPositionable
        public void SetPosition(int noPage, float x, float y)
        {
            this.position = new PointF(x, y);
            this.image.SetPosition(noPage, x, y);
            this.elements.ForEach(element => element.SetPosition(noPage, x, y));
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer)
        {
            this.image.Render(doc, writer);
            this.elements.ForEach(element => element.Render(doc, writer));
        }
    }
}
