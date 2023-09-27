using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_OCR_Explorer;

public partial class SearchView : ContentPage{
    private readonly Manager.PoeFile _poeFile;

    public SearchView(Lines lines, int pageNum, Manager.PoeFile poeFile) {
        _poeFile = poeFile;
        InitializeComponent();
        Header.Text = _poeFile.DispName;
        View.Add(new WebView() {
            Source = "https://example.com"
        });
    }
}