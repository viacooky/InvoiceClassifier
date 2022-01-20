using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using InvoiceClassifier.Extractor;


string? input;


#if DEBUG
input = "pdf/1.pdf";
#else
if (args.Length < 1)
{
    Console.WriteLine("请输入文件名或目录>");
    input = Console.ReadLine();
}
else
{
    input = args[0];
}
#endif

if (File.Exists(input))
{
    ShowInvoice(input);
}

if (Directory.Exists(input))
{
    foreach (var file in Directory.GetFiles(input, "*.pdf", SearchOption.AllDirectories))
    {
        ShowInvoice(file);
    }
}

Console.WriteLine("Done");
Console.ReadLine();



static void ShowInvoice(string fileName)
{
    if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("文件不是.pdf后缀");
        return;
    }

    var invoice = PdfExtractor.Extract(fileName);
    var rs = JsonSerializer.Serialize(invoice, new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder       = JavaScriptEncoder.Create(UnicodeRanges.All)
    });
    Console.WriteLine(rs);
}