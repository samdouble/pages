using iText.Kernel.Pdf;
using iText.Layout;

namespace Pages
{
    interface IRenderable
    {
        void Render(Document doc, PdfWriter writer);
    }
}
