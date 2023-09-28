using System.Diagnostics;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using Image = Microsoft.Maui.Controls.Image;
using Path = System.IO.Path;
using PathBuilder = SixLabors.ImageSharp.Drawing.PathBuilder;
using PointF = SixLabors.ImageSharp.PointF;


namespace PDF_OCR_Explorer;

public partial class SearchView : ContentPage{
    public SearchView(Lines lines, int pageNum, Manager.PoeFile poeFile, double angle, double width, double height) {
        InitializeComponent();
        pageNum++;
        Header.Text = poeFile.DispName + $" {pageNum}ページ目";
        if (Path.GetExtension(poeFile.OrigFile)!.ToLower().Equals(".pdf")){
            var reader = new PdfReader(poeFile.FilePath);
            reader.SetUnethicalReading(true);
            var pdfDocument = new PdfDocument(reader);
            using var outputStream = new MemoryStream();
            var pdfWriter = new PdfWriter(outputStream);
            pdfWriter.SetCloseStream(false);
            var outputDocument = new PdfDocument(pdfWriter);
            pdfDocument.CopyPagesTo(pageNum, pageNum, outputDocument);

            var canvas = new PdfCanvas(outputDocument.GetPage(1));

            var pageSize = outputDocument.GetPage(1).GetPageSize();
            canvas.SetStrokeColor(ColorConstants.RED)
                .MoveTo(pageSize.GetHeight() / height * lines.BoundingPolygon[0].X,
                    pageSize.GetHeight() - pageSize.GetWidth() / width * lines.BoundingPolygon[0].Y);
            foreach (var polygon in lines.BoundingPolygon[1..]){
                canvas.LineTo(pageSize.GetHeight() / height * polygon.X,
                    pageSize.GetHeight() - pageSize.GetWidth() / width * polygon.Y);
            }

            canvas.ClosePathStroke();
            outputDocument.GetPage(1).SetRotation((int)(Math.Round(angle / 90) * -90));


            outputDocument.Close();
            pdfWriter.Flush();
            Debug.Write(outputStream.Length);
            Debug.Write(angle);
            View.Add(new WebView {
                Source = new HtmlWebViewSource {
                    Html =
                        $"<!DOCTYPE html><body><embed src=\"data:application/pdf;base64,{
                            Convert.ToBase64String(outputStream.ToArray())}\" type=\"application/pdf\" " +
                        $"style=\"width:100%;height:100%\"></body></html>"
                }
            });
        }
        else{
            using var fileStream = new FileStream(poeFile.FilePath, FileMode.Open);
            using var rgbImageStream = new MemoryStream();
            SixLabors.ImageSharp.Image.Load(fileStream)
                .Save(rgbImageStream, new PngEncoder { ColorType = PngColorType.Rgb });

            var image = SixLabors.ImageSharp.Image.Load(rgbImageStream.ToArray());
            var pathBuilder = new PathBuilder();
            pathBuilder.ResetOrigin();
            for (var i = 1; i < lines.BoundingPolygon.Length; i++){
                var first = lines.BoundingPolygon[i - 1];
                var second = lines.BoundingPolygon[i];
                pathBuilder.AddLine(new PointF { X = (float)first.X, Y = (float)first.Y },
                    new PointF { X = (float)second.X, Y = (float)second.Y });
            }

            var lastItem = lines.BoundingPolygon.Last();
            var firstItem = lines.BoundingPolygon.First();

            pathBuilder.AddLine(new PointF { X = (float)lastItem.X, Y = (float)lastItem.Y },
                new PointF { X = (float)firstItem.X, Y = (float)firstItem.Y });
            var path = pathBuilder.Build();
            image.Mutate(ctx => ctx.Draw(SixLabors.ImageSharp.Color.Red, 10, path));
            using var outputStream = new MemoryStream();
            image.SaveAsPng(outputStream);
            var imageBytes = outputStream.ToArray();
            View.Add(
                new Image {
                    Source = ImageSource.FromStream(() => new MemoryStream(imageBytes))
                });
        }
    }
}