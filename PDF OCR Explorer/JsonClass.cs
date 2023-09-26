namespace PDF_OCR_Explorer;

public class OcrResultJson{
    public string ServiceVersion { get; set; }
    public string ModelId { get; set; }
    public string Content { get; set; }
    public Pages[] Pages { get; set; }
    public Paragraphs[] Paragraphs { get; set; }
    public object[] Tables { get; set; }
    public object[] KeyValuePairs { get; set; }
    public Styles[] Styles { get; set; }
    public object[] Languages { get; set; }
    public object[] Documents { get; set; }
}

public class Pages{
    public int Unit { get; set; }
    public int PageNumber { get; set; }
    public double Angle { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public Spans[] Spans { get; set; }
    public Words[] Words { get; set; }
    public object[] SelectionMarks { get; set; }
    public Lines[] Lines { get; set; }
    public object[] Barcodes { get; set; }
    public object[] Formulas { get; set; }
}

public class Spans{
    public int Index { get; set; }
    public int Length { get; set; }
}

public class Words{
    public BoundingPolygon[] BoundingPolygon { get; set; }
    public string Content { get; set; }
    public _Span Span { get; set; }
    public double Confidence { get; set; }
}

public class BoundingPolygon{
    public bool IsEmpty { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class _Span{
    public int Index { get; set; }
    public int Length { get; set; }
}

public class Lines{
    public BoundingPolygon1[] BoundingPolygon { get; set; }
    public string Content { get; set; }
    public Spans1[] Spans { get; set; }
}

public class BoundingPolygon1{
    public bool IsEmpty { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class Spans1{
    public int Index { get; set; }
    public int Length { get; set; }
}

public class Paragraphs{
    public object Role { get; set; }
    public string Content { get; set; }
    public BoundingRegions[] BoundingRegions { get; set; }
    public Spans2[] Spans { get; set; }
}

public class BoundingRegions{
    public int PageNumber { get; set; }
    public BoundingPolygon2[] BoundingPolygon { get; set; }
}

public class BoundingPolygon2{
    public bool IsEmpty { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
}

public class Spans2{
    public int Index { get; set; }
    public int Length { get; set; }
}

public class Styles{
    public bool IsHandwritten { get; set; }
    public object SimilarFontFamily { get; set; }
    public object FontStyle { get; set; }
    public object FontWeight { get; set; }
    public object Color { get; set; }
    public object BackgroundColor { get; set; }
    public Spans3[] Spans { get; set; }
    public double Confidence { get; set; }
}

public class Spans3{
    public int Index { get; set; }
    public int Length { get; set; }
}