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
    class Panel
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
                else if (xmlElement.Name == "texte")
                    element = new Texte(xmlElement);

                this.elements.Add(element);
            }
        }

        public void SetHeight(float height)
        {
            this.image.SetHeight(height);
            this.elements.ForEach(element => element.SetHeight(height));
        }

        // REQUIS: elements[0] doit être l'image principale
        // (celle qui dicte ultimement la grosseur de l'espace)
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

        public void Positionner(Document doc, float x, float y, float margeHaut, float margeGauche)
        {
            this.image.Positionner(doc, x, y, margeHaut, margeGauche);
            foreach (Element element in elements)
                element.Positionner(doc, x, y, margeHaut, margeGauche);
        }

        public void AjouterBordures()
        {
            this.image.AjouterBordures();
        }

        public void Render(Document doc)
        {
            doc.Add(this.image.GetImage());
            this.elements.ForEach(element => doc.Add(element.getImage()));
        }
    }
}
