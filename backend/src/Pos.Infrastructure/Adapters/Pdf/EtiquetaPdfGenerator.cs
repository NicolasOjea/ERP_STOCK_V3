using System.Globalization;
using System.Text;
using Pos.Application.Abstractions;
using Pos.Application.DTOs.Etiquetas;

namespace Pos.Infrastructure.Adapters.Pdf;

public sealed class EtiquetaPdfGenerator : IEtiquetaPdfGenerator
{
    public byte[] Generate(EtiquetaPdfDataDto data)
    {
        var content = BuildContent(data.Items);
        return SimplePdfBuilder.Build(content);
    }

    private static string BuildContent(IReadOnlyList<EtiquetaItemDto> items)
    {
        var sb = new StringBuilder();
        sb.Append("BT\n/F1 12 Tf\n1 0 0 1 40 800 Tm\n");

        foreach (var item in items)
        {
            var nombre = Escape(item.Nombre);
            var precio = item.Precio.ToString("0.00", CultureInfo.InvariantCulture);
            var codigo = Escape(item.CodigoBarra);

            sb.Append($"({nombre}) Tj\n");
            sb.Append("0 -14 Td\n");
            sb.Append($"(Precio: {precio}) Tj\n");
            sb.Append("0 -14 Td\n");
            sb.Append($"(SKU: {codigo}) Tj\n");
            sb.Append("0 -24 Td\n");
        }

        sb.Append("ET\n");
        return sb.ToString();
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

    private static class SimplePdfBuilder
    {
        public static byte[] Build(string content)
        {
            var encoding = Encoding.Latin1;
            using var stream = new MemoryStream();

            void Write(string value)
            {
                var bytes = encoding.GetBytes(value);
                stream.Write(bytes, 0, bytes.Length);
            }

            Write("%PDF-1.4\n");

            var offsets = new List<long>();

            WriteObject(1, "<< /Type /Catalog /Pages 2 0 R >>");
            WriteObject(2, "<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
            WriteObject(3, "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>");
            WriteObject(4, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

            var contentBytes = encoding.GetBytes(content);
            WriteObject(5, $"<< /Length {contentBytes.Length} >>\nstream\n{content}\nendstream");

            var xrefPosition = stream.Position;
            Write($"xref\n0 {offsets.Count + 1}\n");
            Write("0000000000 65535 f \n");
            foreach (var offset in offsets)
            {
                Write($"{offset:D10} 00000 n \n");
            }

            Write($"trailer\n<< /Size {offsets.Count + 1} /Root 1 0 R >>\nstartxref\n{xrefPosition}\n%%EOF");

            return stream.ToArray();

            void WriteObject(int id, string body)
            {
                offsets.Add(stream.Position);
                Write($"{id} 0 obj\n{body}\nendobj\n");
            }
        }
    }
}
