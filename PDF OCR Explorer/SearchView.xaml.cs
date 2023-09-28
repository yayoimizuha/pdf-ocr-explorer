using System.Diagnostics;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using Image = SixLabors.ImageSharp.Image;

#if IOS || ANDROID || MACCATALYST
#if IOS || MACCATALYST
using CoreGraphics;
#endif
using Microsoft.Maui.Graphics.Platform;
#elif WINDOWS
using Microsoft.Maui.Graphics.Win2D;
using Size = Windows.Foundation.Size;

#endif

namespace PDF_OCR_Explorer;

public partial class SearchView : ContentPage{
    private readonly Manager.PoeFile _poeFile;

    public SearchView(Lines lines, int pageNum, Manager.PoeFile poeFile, double angle, double width, double height) {
        _poeFile = poeFile;
        InitializeComponent();
        pageNum++;
        Header.Text = _poeFile.DispName + $" {pageNum}ページ目";
        string base64DataUri = null;
        if (Path.GetExtension(_poeFile.OrigFile)!.ToLower().Equals(".pdf")){
            var reader = new PdfReader(_poeFile.FilePath);
            reader.SetUnethicalReading(true);
            var pdfDocument = new PdfDocument(reader);
            var outputStream = new MemoryStream();
            var pdfWriter = new PdfWriter(outputStream);
            pdfWriter.SetCloseStream(false);
            var outputDocument = new PdfDocument(pdfWriter);
            pdfDocument.CopyPagesTo(pageNum, pageNum, outputDocument);
            //pdfDocument.GetPage(pageNum).SetRotation((int)Math.Round(angle / 90) * 90).CopyTo(outputDocument);

            var canvas = new PdfCanvas(outputDocument.GetPage(1));
            //canvas.SetStrokeColor(ColorConstants.BLUE)
            //    .SetLineWidth(outputDocument.GetPage(1).GetPageSize().GetX() / 100f)
            //    .MoveTo(lines.BoundingPolygon.First().X, lines.BoundingPolygon.First().Y)
            //    .LineTo(lines.BoundingPolygon[1].X, lines.BoundingPolygon[1].Y).ClosePathStroke();
            var pageSize = outputDocument.GetPage(1).GetPageSize();
            canvas.SetStrokeColor(ColorConstants.RED)
                //.SetLineWidth(1f)
                .MoveTo(pageSize.GetHeight() / height * lines.BoundingPolygon[0].X,
                    pageSize.GetHeight() - pageSize.GetWidth() / width * lines.BoundingPolygon[0].Y)
                .LineTo(pageSize.GetHeight() / height * lines.BoundingPolygon[1].X,
                    pageSize.GetHeight() - pageSize.GetWidth() / width * lines.BoundingPolygon[1].Y)
                .LineTo(pageSize.GetHeight() / height * lines.BoundingPolygon[2].X,
                    pageSize.GetHeight() - pageSize.GetWidth() / width * lines.BoundingPolygon[2].Y)
                .LineTo(pageSize.GetHeight() / height * lines.BoundingPolygon[3].X,
                    pageSize.GetHeight() - pageSize.GetWidth() / width * lines.BoundingPolygon[3].Y);
            canvas.ClosePathStroke();
            outputDocument.GetPage(1).SetRotation((int)(Math.Round(angle / 90) * -90));

            //foreach (var polygon in lines.BoundingPolygon[1..]) {
            //    canvas.LineTo(polygon.X, polygon.Y);
            //}

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
            var fileStream = new FileStream(poeFile.FilePath, FileMode.Open);
//#if IOS || ANDROID || MACCATALYST
//            var image = PlatformImage.FromStream(fileStream);
//#if IOS || MACCATALYST
//            var canvas = new PlatformCanvas(CGColorSpace.CreateDeviceRGB);
//#else
//            var canvas = new PlatformCanvas();
//#endif
//#elif WINDOWS
//            var image = new W2DImageLoadingService().FromStream(fileStream);
//            var canvas = new W2DCanvas{CanvasSize =new Size(){Height = image.Height,Width = image.Width}};
//#endif
//            canvas.DrawImage(image, 0, 0, image.Width, image.Height);
//            var path = new PathF();
//            path.MoveTo(x: (float)lines.BoundingPolygon[0].X, y: (float)lines.BoundingPolygon[0].Y);
//            foreach (var polygon in lines.BoundingPolygon){
//                path.LineTo(x: (float)polygon.X, y: (float)polygon.Y);
//            }
//
//            path.Close();
//            canvas.StrokeColor = Colors.Red;
//            canvas.DrawPath(path);
//            canvas.SaveState();
//            using var outputStream = new MemoryStream();
//            image.Save(outputStream);
            //outputStream.Flush();
            var image = Image.Load(_poeFile.FilePath);
            


            base64DataUri = $"data:image/{Path.GetExtension(_poeFile.FilePath)!.Replace(".", "")};base64," +
                            Convert.ToBase64String(outputStream.ToArray());
        }

        View.Add(new WebView {
            Source = new HtmlWebViewSource {
                Html = $"<!DOCTYPE html><body><embed src=\"{base64DataUri}\" type=\"application/pdf\" " +
                       $"style=\"width:100%;height:100%\"></body></html>"
            }
        });
    }
}