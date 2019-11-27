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
        List<Element> elements = new List<Element>();

        public Panel(XmlNode xmlPanel)
        {
            foreach (XmlNode xmlElement in xmlPanel.ChildNodes)
            {
                Element element = null;
                if (xmlElement.Name == "image")
                    element = new Image(xmlElement);
                else if (xmlElement.Name == "texte")
                    element = new Texte(xmlElement);

                this.elements.Add(element);
            }
        }

        public void SetHeight(float height)
        {
            this.elements.ForEach(element => element.SetHeight(height));
        }

        // REQUIS: elements[0] doit être l'image principale
        // (celle qui dicte ultimement la grosseur de l'espace)
        public float getLargeur()
        {
            return this.elements[0].getLargeur();
        }
        public float getHauteur()
        {
            return this.elements[0].getHauteur();
        }

        public void Decouper(PdfWriter procEcriture, float decoupageGauche, float offset)
        {
            foreach (Element element in elements)
                element.Decouper(procEcriture, decoupageGauche, offset);
        }

        public void Positionner(Document doc, float x, float y, float margeHaut, float margeGauche)
        {
            foreach (Element element in elements)
                element.Positionner(doc, x, y, margeHaut, margeGauche);
        }

        public void AjouterBordures()
        {
            foreach (Element element in elements)
                element.AjouterBordures();
        }

        public List<Element> getElements()
        {
            return this.elements;
        }
    }
}
