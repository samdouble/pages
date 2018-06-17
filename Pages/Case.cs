using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pages
{
    class Case
    {
        List<Element> elements = new List<Element>();

        public Case(List<Element> elements)
        {
            this.elements = elements;
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
