using System.Diagnostics;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Svg.Renderers.Path.Impl;


namespace PDF_OCR_Explorer;

public partial class SearchView : ContentPage{
    private readonly Manager.PoeFile _poeFile;

    public SearchView(Lines lines, int pageNum, Manager.PoeFile poeFile, double angle) {
        _poeFile = poeFile;
        InitializeComponent();
        pageNum++;
        Header.Text = _poeFile.DispName + $" {pageNum}ページ目";
        string base64DataUri = null;
        if (Path.GetExtension(_poeFile.OrigFile)!.ToLower().Equals(".pdf")){
            var pdfDocument = new PdfDocument(new PdfReader(_poeFile.FilePath));
            var outputStream = new MemoryStream();
            var pdfWriter = new PdfWriter(outputStream);
            pdfWriter.SetCloseStream(false);
            var outputDocument = new PdfDocument(pdfWriter);
            pdfDocument.CopyPagesTo(pageNum, pageNum, outputDocument);
            //pdfDocument.GetPage(pageNum).SetRotation((int)Math.Round(angle / 90) * 90).CopyTo(outputDocument);
            outputDocument.GetPage(1).SetRotation((int)(Math.Round(angle / 90) * -90));
            var canvas = new PdfCanvas(outputDocument.GetPage(1));
            canvas.SetStrokeColor(ColorConstants.BLUE)
                .MoveTo(0,0).LineTo(50,50).Stroke();
            // foreach (var polygon in lines.BoundingPolygon[1..]){
            //     canvas.LineTo(polygon.X, polygon.Y);
            // }
            // canvas.Stroke();
                

            outputDocument.Close();
            pdfWriter.Flush();
            // pdfWriter.Close();
            Debug.Write(outputStream.Length);
            Debug.Write(angle);
            base64DataUri = "data:application/pdf;base64," + Convert.ToBase64String(outputStream.ToArray());
            //Debug.Write(base64DataUri);
        }
        else{
            base64DataUri = $"data:image/{Path.GetExtension(_poeFile.FilePath)!.Replace(".", "")};base64," +
                            Convert.ToBase64String(
                                File.ReadAllBytes(_poeFile.FilePath ?? throw new FileNotFoundException()));
        }

        View.Add(new WebView {
            Source = new HtmlWebViewSource {
                Html = $"<!DOCTYPE html><body><embed src=\"{base64DataUri}\" type=\"application/pdf\" " +
                       $"style=\"width:100%;height:100%\"></body></html>"
            }
        });
    }
}