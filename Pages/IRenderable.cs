using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pages
{
    interface IRenderable
    {
        void Render(Document doc, PdfWriter writer);
    }
}
