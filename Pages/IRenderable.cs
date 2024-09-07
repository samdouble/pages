using SixLabors.ImageSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Pages
{
    public interface IRenderable
    {
        float GetHeight();

        string? GetImagesFolderPath();

        PointF GetPosition();

        float GetVerticalPanelSpacing();

        float GetWidth();

        void Render(Document doc, PdfWriter writer, IRenderable parent);
    }
}
