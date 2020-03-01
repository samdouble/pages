﻿using iTextSharp.text;
using iTextSharp.text.pdf;
using Pages.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            this.image = new Image(@"..\..\..\..\BD\BD1\" + imageSrc);

            foreach (XmlNode xmlElement in xmlPanel.ChildNodes)
            {
                Element element = null;
                if (xmlElement.Name == "description")
                    element = new Description(xmlElement, this);
                else if (xmlElement.Name == "text")
                    element = new Text(xmlElement, this);

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

        public PointF getPosition()
        {
            return this.position;
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
