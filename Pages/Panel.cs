using iTextSharp.text;
using iTextSharp.text.pdf;
using Pages.Elements;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Pages
{
    public class Panel : IPositionable, IRenderable
    {
        [XmlAttribute]
        public string image = string.Empty;
        [XmlElement(ElementName = "description")]
        public List<Description> descriptions { get; set; } = new List<Description>();
        [XmlElement(ElementName = "text")]
        public List<Text> texts { get; set; } = new List<Text>();
        Image img;
        PointF position;

        public void Crop(PdfWriter procEcriture, float decoupageGauche, float horizontalOffset, float decoupageHaut = 0, float verticalOffset = 0)
        {
            this.img.Crop(procEcriture, decoupageGauche, horizontalOffset, decoupageHaut, verticalOffset);
            foreach (Description description in descriptions)
                description.Crop(procEcriture, decoupageGauche, horizontalOffset);
            foreach (Text text in texts)
                text.Crop(procEcriture, decoupageGauche, horizontalOffset);
        }

        public float GetHeight()
        {
            return this.img.GetHeight();
        }

        public PointF GetPosition()
        {
            return this.position;
        }

        public float GetWidth()
        {
            return this.img.GetWidth();
        }

        // IRenderable
        public void Render(Document doc, PdfWriter writer, IRenderable parent)
        {
            string imagePath = @"" + root.GetImagesFolderPath() + '/' + this.image;
            Console.WriteLine("Getting image at " + imagePath);
            if (File.Exists(imagePath))
            {
                this.img = new Image(imagePath);
            }
            else
            {
                this.img = new Image(Properties.Resources.temp);
            }
            this.img.Render(doc, writer, this);
            this.descriptions.ForEach(element => element.Render(doc, writer, this));
            this.texts.ForEach(element => element.Render(doc, writer, this));
        }

        public void SetHeight(float height)
        {
            this.img.SetHeight(height);
        }

        // IPositionable
        public void SetPosition(float x, float y)
        {
            this.position = new PointF(x, y);
            this.img.SetPosition(x, y);
            this.descriptions.ForEach(element => element.SetPosition(x, y));
            this.texts.ForEach(element => element.SetPosition(x, y));
        }
    }
}
