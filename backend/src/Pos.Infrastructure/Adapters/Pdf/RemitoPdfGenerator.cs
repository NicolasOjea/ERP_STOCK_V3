using System.Globalization;
using System.Linq;
using System.Text;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Stock;

namespace Pos.Infrastructure.Adapters.Pdf;

public sealed class RemitoPdfGenerator : IRemitoPdfGenerator
{
    public byte[] Generate(StockRemitoPdfDataDto data)
    {
        var pages = new List<string>();

        var proveedorIndex = 1;
        foreach (var proveedor in data.Proveedores)
        {
            var items = proveedor.Items ?? Array.Empty<StockRemitoPdfItemDto>();
            const int maxRows = 20;
            var totalPages = (int)Math.Ceiling(items.Count / (double)maxRows);

            if (totalPages == 0)
            {
                pages.Add(BuildPage(data, proveedor, Array.Empty<StockRemitoPdfItemDto>(), 1, 1, proveedorIndex));
                proveedorIndex++;
                continue;
            }

            for (var pageIndex = 0; pageIndex < totalPages; pageIndex++)
            {
                var slice = items.Skip(pageIndex * maxRows).Take(maxRows).ToList();
                pages.Add(BuildPage(data, proveedor, slice, pageIndex + 1, totalPages, proveedorIndex));
            }

            proveedorIndex++;
        }

        if (pages.Count == 0)
        {
            pages.Add(BuildPage(
                data,
                new StockRemitoPdfProveedorDto("SIN PROVEEDOR", null, null, null, Array.Empty<StockRemitoPdfItemDto>()),
                Array.Empty<StockRemitoPdfItemDto>(),
                1,
                1,
                1));
        }

        return SimplePdfBuilder.Build(pages);
    }

    private static class SimplePdfBuilder
    {
        public static byte[] Build(IReadOnlyList<string> pages)
        {
            var buffer = new StringBuilder();
            var offsets = new List<int>();

            void Write(string text) => buffer.Append(text);

            Write("%PDF-1.4\n");

            offsets.Add(buffer.Length);
            Write("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

            var pageCount = pages.Count;
            var firstPageId = 3;
            var fontId = firstPageId + (pageCount * 2);
            var fontBoldId = fontId + 1;

            offsets.Add(buffer.Length);
            var kids = string.Join(" ", Enumerable.Range(0, pageCount).Select(i => $"{firstPageId + i * 2} 0 R"));
            Write($"2 0 obj\n<< /Type /Pages /Kids [{kids}] /Count {pageCount} >>\nendobj\n");

            for (var i = 0; i < pageCount; i++)
            {
                var pageId = firstPageId + i * 2;
                var contentId = pageId + 1;

                offsets.Add(buffer.Length);
                Write($"{pageId} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents {contentId} 0 R /Resources << /Font << /F1 {fontId} 0 R /F2 {fontBoldId} 0 R >> >> >>\nendobj\n");

                var contentStream = pages[i];
                offsets.Add(buffer.Length);
                Write($"{contentId} 0 obj\n<< /Length {contentStream.Length} >>\nstream\n{contentStream}\nendstream\nendobj\n");
            }

            offsets.Add(buffer.Length);
            Write($"{fontId} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");
            offsets.Add(buffer.Length);
            Write($"{fontBoldId} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>\nendobj\n");

            var xrefPosition = buffer.Length;
            Write($"xref\n0 {offsets.Count + 1}\n");
            Write("0000000000 65535 f \n");
            foreach (var offset in offsets)
            {
                Write(offset.ToString("D10"));
                Write(" 00000 n \n");
            }

            Write($"trailer\n<< /Size {offsets.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPosition}\n%%EOF");

            return Encoding.ASCII.GetBytes(buffer.ToString());
        }
    }

    private static string BuildPage(
        StockRemitoPdfDataDto data,
        StockRemitoPdfProveedorDto proveedor,
        IReadOnlyList<StockRemitoPdfItemDto> items,
        int pageIndex,
        int totalPages,
        int proveedorIndex)
    {
        const double pageWidth = 595;
        const double pageHeight = 842;
        const double margin = 25;
        const double gap = 10;

        const double topBoxHeight = 110;
        const double leftBoxWidth = 380;
        const double rightBoxWidth = pageWidth - (margin * 2) - leftBoxWidth - gap;
        const double topBoxY = pageHeight - margin - topBoxHeight;
        const double leftBoxX = margin;
        var rightBoxX = leftBoxX + leftBoxWidth + gap;

        const double providerBoxHeight = 85;
        var providerBoxY = topBoxY - gap - providerBoxHeight;

        const double tableX = margin;
        const double tableWidth = pageWidth - (margin * 2);
        const double headerHeight = 24;
        var headerY = providerBoxY - gap - headerHeight;

        var sb = new StringBuilder();
        sb.AppendLine("0.8 w");

        // Boxes
        sb.AppendLine($"{leftBoxX} {topBoxY} {leftBoxWidth} {topBoxHeight} re S");
        sb.AppendLine($"{rightBoxX} {topBoxY} {rightBoxWidth} {topBoxHeight} re S");
        sb.AppendLine($"{tableX} {providerBoxY} {tableWidth} {providerBoxHeight} re S");

        // Header boxes text
        WriteText(sb, leftBoxX + 10, topBoxY + 88, 12, "Razon Social", true);
        WriteText(sb, leftBoxX + 120, topBoxY + 88, 12, data.Header.EmpresaNombre);
        WriteText(sb, leftBoxX + 10, topBoxY + 70, 9, "Sucursal", true);
        WriteText(sb, leftBoxX + 120, topBoxY + 70, 9, data.Header.SucursalNombre);
        WriteText(sb, leftBoxX + 10, topBoxY + 54, 9, "CUIT", true);
        WriteText(sb, leftBoxX + 120, topBoxY + 54, 9, "-");
        WriteText(sb, leftBoxX + 10, topBoxY + 38, 9, "Direccion", true);
        WriteText(sb, leftBoxX + 120, topBoxY + 38, 9, "-");
        WriteText(sb, leftBoxX + 10, topBoxY + 22, 9, "Localidad", true);
        WriteText(sb, leftBoxX + 120, topBoxY + 22, 9, data.Header.SucursalNombre);
        WriteText(sb, leftBoxX + 10, topBoxY + 6, 9, "Telefono", true);
        WriteText(sb, leftBoxX + 120, topBoxY + 6, 9, "-");

        WriteText(sb, rightBoxX + 10, topBoxY + 80, 14, "REMITO", true);
        var numeroRemito = totalPages > 1 || proveedorIndex > 1
            ? $"{data.RemitoNumero}-{proveedorIndex:00}"
            : data.RemitoNumero;
        WriteText(sb, rightBoxX + 10, topBoxY + 55, 11, $"N\u00b0 {numeroRemito}", true);
        WriteText(sb, rightBoxX + 10, topBoxY + 35, 10, $"Fecha {data.Fecha:dd/MM/yyyy}");
        if (totalPages > 1)
        {
            WriteText(sb, rightBoxX + 10, topBoxY + 12, 9, $"Pagina {pageIndex}/{totalPages}");
        }

        // Provider box text
        var proveedorNombre = proveedor.Nombre;
        var proveedorDireccion = proveedor.Direccion ?? "-";
        var proveedorTelefono = proveedor.Telefono ?? "-";
        var proveedorCuit = proveedor.Cuit ?? "-";

        WriteText(sb, tableX + 10, providerBoxY + 60, 9, "Nombre", true);
        WriteText(sb, tableX + 90, providerBoxY + 60, 9, proveedorNombre);
        WriteText(sb, tableX + 10, providerBoxY + 44, 9, "Domicilio", true);
        WriteText(sb, tableX + 90, providerBoxY + 44, 9, proveedorDireccion);
        WriteText(sb, tableX + 10, providerBoxY + 28, 9, "Localidad", true);
        WriteText(sb, tableX + 90, providerBoxY + 28, 9, "-");
        WriteText(sb, tableX + 10, providerBoxY + 12, 9, "CUIT", true);
        WriteText(sb, tableX + 90, providerBoxY + 12, 9, proveedorCuit);

        WriteText(sb, tableX + 300, providerBoxY + 60, 9, "Telefono", true);
        WriteText(sb, tableX + 380, providerBoxY + 60, 9, proveedorTelefono);
        WriteText(sb, tableX + 300, providerBoxY + 44, 9, "C.P.", true);
        WriteText(sb, tableX + 380, providerBoxY + 44, 9, "-");
        WriteText(sb, tableX + 300, providerBoxY + 28, 9, "Provincia", true);
        WriteText(sb, tableX + 380, providerBoxY + 28, 9, "-");

        // Table header background
        sb.AppendLine("0.05 0.6 0.5 rg");
        sb.AppendLine($"{tableX} {headerY} {tableWidth} {headerHeight} re f");
        sb.AppendLine("1 1 1 rg");
        sb.AppendLine($"{tableX} {headerY} {tableWidth} {headerHeight} re S");

        // Column lines
        var colArticulo = tableX + 70;
        var colDescripcion = tableX + 430;
        sb.AppendLine($"{colArticulo} {headerY} 0 {headerHeight} re S");
        sb.AppendLine($"{colDescripcion} {headerY} 0 {headerHeight} re S");

        WriteText(sb, tableX + 10, headerY + 7, 10, "ARTICULO", true);
        WriteText(sb, colArticulo + 10, headerY + 7, 10, "DESCRIPCION", true);
        WriteText(sb, colDescripcion + 10, headerY + 7, 10, "CANTIDAD", true);

        sb.AppendLine("0 0 0 rg");

        // Items
        var rowY = headerY - 16;
        var index = (pageIndex - 1) * 20 + 1;
        foreach (var item in items)
        {
            var descripcion = item.Nombre;
            var cantidad = item.Cantidad.ToString("0.##", CultureInfo.InvariantCulture);
            WriteText(sb, tableX + 10, rowY, 10, Truncate(item.Sku, 12));
            WriteText(sb, colArticulo + 10, rowY, 10, Truncate(descripcion, 45));
            WriteText(sb, colDescripcion + 15, rowY, 10, cantidad);
            rowY -= 16;
            index++;
        }

        return sb.ToString();
    }

    private static void WriteText(StringBuilder sb, double x, double y, int size, string text, bool bold = false)
    {
        sb.AppendLine("BT");
        sb.AppendLine($"/{(bold ? "F2" : "F1")} {size} Tf");
        sb.AppendLine($"{x:0.##} {y:0.##} Td");
        sb.AppendLine($"({Escape(text)}) Tj");
        sb.AppendLine("ET");
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

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength);
    }
}
