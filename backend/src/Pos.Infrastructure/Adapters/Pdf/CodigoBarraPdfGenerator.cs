using System.Globalization;
using System.Text;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Etiquetas;

namespace Pos.Infrastructure.Adapters.Pdf;

public sealed class CodigoBarraPdfGenerator : ICodigoBarraPdfGenerator
{
    private const double PageWidth = 595d;
    private const double PageHeight = 842d;
    private const double Margin = 28d;
    private const double HeaderHeight = 36d;
    private const double FooterHeight = 18d;
    private const double ColumnGap = 8d;
    private const double RowGap = 8d;
    private const double LabelHeight = 90d;

    private static readonly IReadOnlyDictionary<char, string> Code39Patterns = new Dictionary<char, string>
    {
        ['0'] = "nnnwwnwnn",
        ['1'] = "wnnwnnnnw",
        ['2'] = "nnwwnnnnw",
        ['3'] = "wnwwnnnnn",
        ['4'] = "nnnwwnnnw",
        ['5'] = "wnnwwnnnn",
        ['6'] = "nnwwwnnnn",
        ['7'] = "nnnwnnwnw",
        ['8'] = "wnnwnnwnn",
        ['9'] = "nnwwnnwnn",
        ['A'] = "wnnnnwnnw",
        ['B'] = "nnwnnwnnw",
        ['C'] = "wnwnnwnnn",
        ['D'] = "nnnnwwnnw",
        ['E'] = "wnnnwwnnn",
        ['F'] = "nnwnwwnnn",
        ['G'] = "nnnnnwwnw",
        ['H'] = "wnnnnwwnn",
        ['I'] = "nnwnnwwnn",
        ['J'] = "nnnnwwwnn",
        ['K'] = "wnnnnnnww",
        ['L'] = "nnwnnnnww",
        ['M'] = "wnwnnnnwn",
        ['N'] = "nnnnwnnww",
        ['O'] = "wnnnwnnwn",
        ['P'] = "nnwnwnnwn",
        ['Q'] = "nnnnnnwww",
        ['R'] = "wnnnnnwwn",
        ['S'] = "nnwnnnwwn",
        ['T'] = "nnnnwnwwn",
        ['U'] = "wwnnnnnnw",
        ['V'] = "nwwnnnnnw",
        ['W'] = "wwwnnnnnn",
        ['X'] = "nwnnwnnnw",
        ['Y'] = "wwnnwnnnn",
        ['Z'] = "nwwnwnnnn",
        ['-'] = "nwnnnnwnw",
        ['.'] = "wwnnnnwnn",
        [' '] = "nwwnnnwnn",
        ['$'] = "nwnwnwnnn",
        ['/'] = "nwnwnnwnn",
        ['+'] = "nwnnnwnwn",
        ['%'] = "nnnwnwnwn",
        ['*'] = "nwnnwnwnn"
    };

    public byte[] Generate(CodigoBarraPdfDataDto data)
    {
        var proveedores = data.Proveedores.Count == 0
            ? new[] { new CodigoBarraProveedorPdfDto("SIN PROVEEDOR", Array.Empty<CodigoBarraItemPdfDto>()) }
            : data.Proveedores;

        var pageContents = new List<string>();
        var cellWidth = (PageWidth - (Margin * 2) - (ColumnGap * 2)) / 3d;
        var contentTop = PageHeight - Margin - HeaderHeight;
        var contentBottom = Margin + FooterHeight;
        var rowsPerPage = Math.Max(1, (int)Math.Floor((contentTop - contentBottom + RowGap) / (LabelHeight + RowGap)));
        var itemsPerPage = rowsPerPage * 3;

        foreach (var proveedor in proveedores)
        {
            var items = proveedor.Items.Count == 0
                ? new List<CodigoBarraItemPdfDto> { new("Sin productos", "-") }
                : proveedor.Items.ToList();

            for (var offset = 0; offset < items.Count; offset += itemsPerPage)
            {
                var page = StartPage();
                WriteHeader(page, proveedor.Proveedor);

                var slice = items.Skip(offset).Take(itemsPerPage).ToList();
                for (var i = 0; i < slice.Count; i++)
                {
                    var row = i / 3;
                    var column = i % 3;
                    var x = Margin + (column * (cellWidth + ColumnGap));
                    var topY = contentTop - (row * (LabelHeight + RowGap));
                    DrawLabel(page, slice[i], x, topY, cellWidth);
                }

                pageContents.Add(page.ToString());
            }
        }

        for (var i = 0; i < pageContents.Count; i++)
        {
            var sb = new StringBuilder(pageContents[i]);
            WritePageNumber(sb, i + 1, pageContents.Count);
            pageContents[i] = sb.ToString();
        }

        return SimplePdfBuilder.Build(pageContents);
    }

    private static StringBuilder StartPage()
    {
        var sb = new StringBuilder();
        sb.AppendLine("0 0 0 rg");
        sb.AppendLine("0 0 0 RG");
        sb.AppendLine("0.8 w");
        return sb;
    }

    private static void WriteHeader(StringBuilder sb, string proveedor)
    {
        var providerName = string.IsNullOrWhiteSpace(proveedor) ? "SIN PROVEEDOR" : proveedor.Trim();
        WriteText(sb, Margin, PageHeight - Margin - 10d, 14, providerName, true);
        sb.AppendLine($"{Fmt(Margin)} {Fmt(PageHeight - Margin - HeaderHeight + 2d)} {Fmt(PageWidth - (Margin * 2))} 0.5 re f");
    }

    private static void DrawLabel(StringBuilder sb, CodigoBarraItemPdfDto item, double x, double topY, double width)
    {
        var y = topY - LabelHeight;
        var name = string.IsNullOrWhiteSpace(item.Nombre) ? "SIN NOMBRE" : item.Nombre.Trim();
        var sku = NormalizeSku(item.Sku);

        sb.AppendLine($"{Fmt(x)} {Fmt(y)} {Fmt(width)} {Fmt(LabelHeight)} re S");
        WriteText(sb, x + 6d, topY - 16d, 10, name, true);

        var barcodeX = x + 8d;
        var barcodeY = y + 24d;
        var barcodeWidth = width - 16d;
        DrawCode39Barcode(sb, sku, barcodeX, barcodeY, barcodeWidth, 28d);

        WriteText(sb, x + 6d, y + 8d, 9, sku);
    }

    private static void WritePageNumber(StringBuilder sb, int page, int total)
    {
        var text = $"Pagina {page}/{total}";
        var x = PageWidth - Margin - 70d;
        var y = Margin - 2d;
        WriteText(sb, x, y, 9, text);
    }

    private static void DrawCode39Barcode(
        StringBuilder sb,
        string value,
        double x,
        double y,
        double maxWidth,
        double barHeight)
    {
        var encoded = $"*{value}*";
        const double narrow = 1d;
        const double wide = 3d;
        const double charGap = 1d;

        var totalWidth = 0d;
        foreach (var ch in encoded)
        {
            var pattern = Code39Patterns[ch];
            foreach (var unit in pattern)
            {
                totalWidth += unit == 'w' ? wide : narrow;
            }

            totalWidth += charGap;
        }

        var scale = totalWidth > 0d ? Math.Min(1d, maxWidth / totalWidth) : 1d;
        var cursorX = x;

        sb.AppendLine("0 0 0 rg");
        foreach (var ch in encoded)
        {
            var pattern = Code39Patterns[ch];
            for (var i = 0; i < pattern.Length; i++)
            {
                var elementWidth = (pattern[i] == 'w' ? wide : narrow) * scale;
                var isBar = i % 2 == 0;
                if (isBar)
                {
                    sb.AppendLine($"{Fmt(cursorX)} {Fmt(y)} {Fmt(elementWidth)} {Fmt(barHeight)} re f");
                }

                cursorX += elementWidth;
            }

            cursorX += charGap * scale;
        }
    }

    private static void WriteText(StringBuilder sb, double x, double y, int size, string text, bool bold = false)
    {
        sb.AppendLine("BT");
        sb.AppendLine($"/{(bold ? "F2" : "F1")} {size} Tf");
        sb.AppendLine($"{Fmt(x)} {Fmt(y)} Td");
        sb.AppendLine($"({Escape(text)}) Tj");
        sb.AppendLine("ET");
    }

    private static string NormalizeSku(string? sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return "-";
        }

        var normalized = new StringBuilder();
        foreach (var ch in sku.Trim().ToUpperInvariant())
        {
            normalized.Append(Code39Patterns.ContainsKey(ch) ? ch : '-');
        }

        return normalized.Length == 0 ? "-" : normalized.ToString();
    }

    private static string Escape(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        return text
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal);
    }

    private static string Fmt(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private static class SimplePdfBuilder
    {
        public static byte[] Build(IReadOnlyList<string> pages)
        {
            var encoding = Encoding.Latin1;
            using var stream = new MemoryStream();
            var offsets = new List<long>();

            void Write(string value)
            {
                var bytes = encoding.GetBytes(value);
                stream.Write(bytes, 0, bytes.Length);
            }

            void WriteObject(int id, string body)
            {
                offsets.Add(stream.Position);
                Write($"{id} 0 obj\n{body}\nendobj\n");
            }

            Write("%PDF-1.4\n");

            var pageCount = pages.Count;
            var firstPageObjectId = 3;
            var fontObjectId = firstPageObjectId + (pageCount * 2);
            var fontBoldObjectId = fontObjectId + 1;

            WriteObject(1, "<< /Type /Catalog /Pages 2 0 R >>");

            var kids = string.Join(" ", Enumerable.Range(0, pageCount).Select(i => $"{firstPageObjectId + (i * 2)} 0 R"));
            WriteObject(2, $"<< /Type /Pages /Kids [{kids}] /Count {pageCount} >>");

            for (var i = 0; i < pageCount; i++)
            {
                var pageId = firstPageObjectId + (i * 2);
                var contentId = pageId + 1;

                WriteObject(
                    pageId,
                    $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PageWidth.ToString(CultureInfo.InvariantCulture)} {PageHeight.ToString(CultureInfo.InvariantCulture)}] /Resources << /Font << /F1 {fontObjectId} 0 R /F2 {fontBoldObjectId} 0 R >> >> /Contents {contentId} 0 R >>");

                var content = pages[i];
                var contentBytes = encoding.GetBytes(content);
                WriteObject(contentId, $"<< /Length {contentBytes.Length} >>\nstream\n{content}\nendstream");
            }

            WriteObject(fontObjectId, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            WriteObject(fontBoldObjectId, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");

            var xrefPosition = stream.Position;
            Write($"xref\n0 {offsets.Count + 1}\n");
            Write("0000000000 65535 f \n");
            foreach (var offset in offsets)
            {
                Write($"{offset:D10} 00000 n \n");
            }

            Write($"trailer\n<< /Size {offsets.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPosition}\n%%EOF");
            return stream.ToArray();
        }
    }
}
