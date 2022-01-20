using System.Drawing;
using System.Text.RegularExpressions;
using InvoiceClassifier.Model;
using Spire.Pdf;
using Spire.Pdf.General.Find;

namespace InvoiceClassifier.Extractor;

internal class PdfExtractor
{
    public static InvoiceCls Extract(string fileName)
    {
        if (!File.Exists(fileName)) throw new FileLoadException("文件不存在", fileName);
        using var doc = new PdfDocument();
        doc.LoadFromFile(fileName);
        if (doc.Pages.Count < 1) throw new Exception("无法获取PDF文件第一页");
        var page    = doc.Pages[0];
        var invoice = new InvoiceCls {FileName = fileName};
        HeadHandler(invoice, page);
        BuyerHandler(invoice, page);
        SellerHandler(invoice, page);
        FootHandler(invoice, page);
        PasswordHandler(invoice, page);
        DetailHandler(invoice, page);

        return invoice;
    }

    /// <summary>
    /// 处理明细
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="page"></param>
    private static void DetailHandler(InvoiceCls invoice, PdfPageBase page)
    {
        // 理论上在【价税合计(大写)】同一行
        var find = page.FindText(page.MediaBox, "价税合计", TextFindParameter.IgnoreCase).Finds.FirstOrDefault();
        if (find == null) return;

        var areaText = find.LineText.Replace("（", "(").Replace("）", ")");

        var totalAmountString = Regex.Match(areaText, @"价.*税.*合.*计.*大写\)(?<Str>.*)\(小.*写\)").Groups["Str"].Value.Trim();
        var totalAmountStr    = Regex.Match(areaText, @"\(小.*写\)(?<TotalAmount>.*)").Groups["TotalAmount"].Value.Trim();
        totalAmountStr = totalAmountStr.Replace("￥", "").Replace("¥", "");
        var totalAmount = decimal.TryParse(totalAmountStr, out var r)
                              ? r
                              : decimal.Zero;


        invoice.Detail = new InvoiceCls.DetailCls
        {
            Amount            = 0,
            TaxAmount         = 0,
            TotalAmount       = totalAmount,
            TotalAmountString = totalAmountString,
            Items             = null
        };

        // 用【税额】确定坐标
        find = page.FindText(page.MediaBox, @"税额", TextFindParameter.IgnoreCase).Finds.FirstOrDefault();
        if (find == null) return;
        var basePosition = find.Position;
        areaText = page.ExtractText(new RectangleF(
            0,
            basePosition.Y,
            page.Size.Width,
            page.Size.Height/3));
    }

    /// <summary>
    /// 处理密码
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="page"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void PasswordHandler(InvoiceCls invoice, PdfPageBase page)
    {
        // todo
    }

    /// <summary>
    /// 处理底部
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="page"></param>
    private static void FootHandler(InvoiceCls invoice, PdfPageBase page)
    {
        // 理论上都在【收款人】这一行
        var find = page.FindText(page.MediaBox, "收款人", TextFindParameter.IgnoreCase).Finds.FirstOrDefault();
        if (find == null) return;
        invoice.Payee    = Regex.Match(find.LineText, @"收款人:(?<Payee>.*).*复").Groups["Payee"].Value.Trim();
        invoice.Reviewer = Regex.Match(find.LineText, @"复.*核.*:(?<Reviewer>.*).*开").Groups["Reviewer"].Value.Trim();
        invoice.Drawer   = Regex.Match(find.LineText, @"开.*票.*人.*:(?<Drawer>.*).*销").Groups["Drawer"].Value.Trim();
    }

    /// <summary>
    /// 处理销售方
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="page"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void SellerHandler(InvoiceCls invoice, PdfPageBase page)
    {
        // 使用【收款人】作为基础坐标，获取左上角区域
        var find = page.FindText(page.MediaBox, "收款人", TextFindParameter.IgnoreCase).Finds.FirstOrDefault();
        if (find == null) return;
        var basePosition = find.Position;
        var areaText = page.ExtractText(new RectangleF(
            basePosition.X + 10,
            basePosition.Y - 60,
            page.Size.Width / 2,
            50));
        invoice.Seller = new InvoiceCls.SellerCls
        {
            Name    = Regex.Match(areaText, @"名.*称:(?<Name>.*)").Groups["Name"].Value.Trim(),
            Code    = Regex.Match(areaText, @"纳.*识别号:(?<Code>.*)").Groups["Code"].Value.Trim(),
            Address = Regex.Match(areaText, @"地.*址.*话:(?<Address>.*)").Groups["Address"].Value.Trim(),
            Account = Regex.Match(areaText, @"开.*户.*账.*号:(?<Account>.*)").Groups["Account"].Value.Trim(),
        };
    }

    /// <summary>
    /// 处理购买方
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="page"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void BuyerHandler(InvoiceCls invoice, PdfPageBase page)
    {
        // 使用【机器编号】作为基础坐标，获取左上角区域
        var find = page.FindText(page.MediaBox, "机器编号", TextFindParameter.IgnoreCase).Finds.FirstOrDefault();
        if (find == null) return;
        var basePosition = find.Position;
        var areaText = page.ExtractText(new RectangleF(
            basePosition.X + 10,
            basePosition.Y + 10,
            page.Size.Width / 2,
            60));

        invoice.Buyer = new InvoiceCls.BuyerCls
        {
            Name    = Regex.Match(areaText, @"名.*称:(?<Name>.*)").Groups["Name"].Value.Trim(),
            Code    = Regex.Match(areaText, @"纳.*识别号:(?<Code>.*)").Groups["Code"].Value.Trim(),
            Address = Regex.Match(areaText, @"地.*址.*话:(?<Address>.*)").Groups["Address"].Value.Trim(),
            Account = Regex.Match(areaText, @"开.*户.*账.*号:(?<Account>.*)").Groups["Account"].Value.Trim(),
        };
    }

    /// <summary>
    /// 处理头部
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="page"></param>
    private static void HeadHandler(InvoiceCls invoice, PdfPageBase page)
    {
        var fullText = page.ExtractText();

        // 使用【发票代码】作为基础坐标，获取左上角区域
        var find = page.FindText(page.MediaBox, "发票代码", TextFindParameter.IgnoreCase).Finds.FirstOrDefault();
        if (find == null) return;
        var basePosition = find.Position;
        var areaText = page.ExtractText(new RectangleF(
            basePosition.X,
            basePosition.Y,
            page.Size.Width / 3,
            60));

        invoice.Title         = Regex.Match(fullText, @"(?<Title>\S*普通发票|\S*专用发票)").Groups["Title"].Value.Trim();
        invoice.Type          = invoice.Title.Length > 4 ? invoice.Title[^4..] : string.Empty;
        invoice.MachineNumber = Regex.Match(fullText, @"机器编号:.*(?<MachineNumber>\d{12})").Groups["MachineNumber"].Value.Trim();
        invoice.Code          = Regex.Match(areaText, @"发票代码:.*(?<Code>\d{12})").Groups["Code"].Value.Trim();
        invoice.Number        = Regex.Match(areaText, @"发票号码:.*(?<Number>\d{8})").Groups["Number"].Value.Trim();
        invoice.CheckSum      = Regex.Match(areaText, @"校.*验.*码.*:(?<Checksum>.*)").Groups["Checksum"].Value.Trim();
        invoice.Date          = Regex.Match(areaText, @"开票日期:?(?<Date>.*)").Groups["Date"].Value.Trim();
    }
}