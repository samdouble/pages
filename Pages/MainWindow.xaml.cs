using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;

namespace Pages
{
    public partial class MainWindow : Window
    {
        private float margeGauche = 25f;
        private float margeDroite = 25f;
        private float margeHaut = 64f;
        private float margeBas = 64f;

        private float espaceHEntreCases = 10f;
        private float espaceVEntreCases = 10f;

        private int nbRangeesParPage = 3;
   
        public MainWindow()
        {
            InitializeComponent();

            Document doc = new Document();
            try
            {
                PdfWriter procEcriture = PdfWriter.GetInstance(doc, new FileStream(@"..\..\..\..\bd1\BD1\Images.pdf", FileMode.Create));
                doc.Open();

                float hauteurCase = (doc.PageSize.Height - margeHaut - margeBas - (nbRangeesParPage - 1) * espaceVEntreCases) / nbRangeesParPage;
                float largeurRangee = doc.PageSize.Width - margeDroite - margeGauche;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(@"..\..\..\..\bd1\BD1\bd.xml");
                Comic comic = new Comic(xmlDocument.DocumentElement);

                float x = 0;
                float y = 0;
                float noRangee = 0;
                for (int i = 0; i < comic.GetSlotsCount(); )
                {
                    int nbCasesDansLaRangee = 0;

                    // On trouve le nombre de cases qu'on peut fitter dans la rangée
                    float largeurMin = 0;
                    float largeurMax = 0;
                    for ( ; i + nbCasesDansLaRangee < comic.GetSlotsCount() && largeurMin < largeurRangee; ++nbCasesDansLaRangee)
                    {
                        Slot slot = comic.GetNthSlot(i + nbCasesDansLaRangee);
                        slot.SetHeight(hauteurCase);
                        float largeurMinCase = slot.getLargeur() * (1 - ((slot.paddingMaxGauchePct + slot.paddingMaxDroitePct) / 100));
                        float largeurMaxCase = slot.getLargeur();
                        if (largeurMin + largeurMinCase + (nbCasesDansLaRangee >= 1 ? espaceHEntreCases : 0) > largeurRangee)
                            break;
                        largeurMin += largeurMinCase + (nbCasesDansLaRangee >= 1 ? espaceHEntreCases : 0);
                        largeurMax += largeurMaxCase + (nbCasesDansLaRangee >= 1 ? espaceHEntreCases : 0);
                    }

                    // =============================

                    float decoupage = 0;
                    float decoupageTotal = largeurRangee - largeurMin;
                    float decoupageAlloueParCase = decoupageTotal / nbCasesDansLaRangee;

                    List<Slot> espacesSurLaRangee = new List<Slot>();
                    for (int j = 0; j < nbCasesDansLaRangee; ++j)
                        espacesSurLaRangee.Add(comic.GetNthSlot(i + j));

                    // On essaie d'égaliser les côtés de chaque bord des images
                    foreach (Slot espace in espacesSurLaRangee)
                    {
                        float decoupageGauche, decoupageDroite;
                        float decoupagePossibleGauche = espace.getLargeur() * espace.paddingMaxGauchePct / 100;
                        float decoupagePossibleDroite = espace.getLargeur() * espace.paddingMaxDroitePct / 100;

                        if (decoupagePossibleGauche + decoupagePossibleDroite <= decoupageAlloueParCase)
                        {
                            float decoupageEquilibre = Math.Min(decoupagePossibleGauche, decoupagePossibleDroite);
                            decoupageGauche = Math.Min(decoupageEquilibre, decoupageAlloueParCase / 2);
                            decoupageDroite = Math.Min(decoupageEquilibre, decoupageAlloueParCase / 2);
                            espace.paddingGauche = decoupageGauche;
                            espace.paddingDroite = decoupageDroite;

                            decoupage += decoupageGauche;
                            decoupage += decoupageDroite;
                        }
                    }

                    // On ajoute le padding qu'il faut pour remplir la rangée le plus équitablement possible
                    List<Slot> espacesSurLaRangeeTries = espacesSurLaRangee.OrderBy(e => e.paddingMaxGauchePct + e.paddingMaxDroitePct).ToList();
                    while (decoupage < Math.Min(decoupageTotal, espacesSurLaRangee.Sum(e => (e.paddingMaxGauchePct + e.paddingMaxDroitePct) * e.getLargeur() / 100)))
                    {
                        foreach (Slot espace in espacesSurLaRangeeTries)
                        {
                            if (decoupage < decoupageTotal && espace.paddingGauche < (espace.paddingMaxGauchePct * espace.getLargeur() / 100))
                            {
                                espace.paddingGauche++;
                                decoupage++;
                            }

                            if (decoupage < decoupageTotal && espace.paddingDroite < (espace.paddingMaxDroitePct * espace.getLargeur() / 100))
                            {
                                espace.paddingDroite++;
                                decoupage++;
                            }
                        }
                    }

                    // On procède au découpage et positionnement de l'image
                    foreach (Slot espace in espacesSurLaRangee)
                    {
                        espace.Decouper(procEcriture);

                        espace.Positionner(doc, x, y, margeHaut, margeGauche);

                        espace.AjouterBordures();

                        List<Element> els = espace.getElements();
                        foreach (Element element in els)
                        {
                            doc.Add(element.getImage());

                            PdfContentByte cb = procEcriture.DirectContent;
                            cb.RoundRectangle(200f, 500f, 200f, 200f, 5f);
                            cb.SetColorFill(BaseColor.WHITE);
                            cb.SetColorStroke(BaseColor.BLACK);
                            cb.SetLineWidth(2f);
                            cb.FillStroke();

                            Paragraph paragraph = new Paragraph(
                                    "This could be a very long sentence that needs to be wrapped.",
                                    FontFactory.GetFont("dax-black")
                                );
                            ColumnText ct = new ColumnText(procEcriture.DirectContent);
                            cb.SetColorFill(BaseColor.BLACK);
                            ct.SetSimpleColumn(new Rectangle(206f, 700f, 400f, 600f));
                            ct.AddElement(paragraph);
                            ct.Go();
                            Console.WriteLine(ct.LastX);
                            Console.WriteLine(ct.YLine);
                        }
                            

                        x += espace.getLargeur() + espaceHEntreCases;
                    }
                    x = 0;
                    i += nbCasesDansLaRangee;
                    ++noRangee;
                    
                    if (noRangee % nbRangeesParPage == 0)
                    {
                        doc.NewPage();
                        y = 0;
                    }
                    else if (i < comic.GetSlotsCount())
                    {
                        y += comic.GetNthSlot(i).getHauteur() + espaceVEntreCases;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                doc.Close();
            }
            System.Diagnostics.Process.Start(@"..\..\..\..\bd1\BD1\Images.pdf");
        }
    }
}
