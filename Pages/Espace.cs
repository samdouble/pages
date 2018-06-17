using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pages
{
    class Espace
    {
        private List<Case> cases = new List<Case>();
        public float paddingMaxGauchePct { get; set; }
        public float paddingMaxDroitePct { get; set; }
        public float paddingGauche { get; set; }
        public float paddingDroite { get; set; }

        public Espace(List<Case> cases)
        {
            this.cases = cases;
            this.paddingMaxGauchePct = 0;
            this.paddingMaxDroitePct = 0;
            this.paddingGauche = 0;
            this.paddingDroite = 0;
        }

        // On prend cases[0] puisque toutes les cases d'un même espace
        // doivent avoir la même largeur
        public float getLargeur()
        {
            return cases[0].getLargeur();
        }
        public float getHauteur()
        {
            return cases[0].getHauteur();
        }

        public void Decouper(PdfWriter procEcriture)
        {
            float decoupageGauche = (this.paddingMaxGauchePct * this.getLargeur() / 100) - this.paddingGauche;
            float decoupageDroite = (this.paddingMaxDroitePct * this.getLargeur() / 100) - this.paddingDroite;
            float offset = decoupageGauche + decoupageDroite;
            foreach (Case casex in cases)
                casex.Decouper(procEcriture, decoupageGauche, offset);
        }

        public void Positionner(Document doc, float x, float y, float margeHaut, float margeGauche)
        {
            foreach (Case casex in cases)
                casex.Positionner(doc, x, y, margeHaut, margeGauche);
        }

        public void AjouterBordures()
        {
            foreach (Case casex in cases)
                casex.AjouterBordures();
        }

        public List<Element> getElements()
        {
            List<Element> elements = new List<Element>();
            foreach (Case casex in cases)
                elements.AddRange(casex.getElements());
            return elements;
        }
    }
}
