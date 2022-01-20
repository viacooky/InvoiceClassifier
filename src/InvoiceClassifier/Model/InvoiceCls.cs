using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceClassifier.Model;

internal class InvoiceCls
{
    public string? FileName { get; set; }
    /// <summary>
    /// 抬头
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 发票类型
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 机器编号
    /// </summary>
    public string? MachineNumber { get; set; }

    /// <summary>
    /// 发票代码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 发票号码
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// 开票日期
    /// </summary>
    public string? Date { get; set; }

    /// <summary>
    /// 校验码
    /// </summary>
    public string? CheckSum { get; set; }

    /// <summary>
    /// 购买方
    /// </summary>
    public BuyerCls? Buyer { get; set; } = new ();


    public class BuyerCls
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 地址、电话
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 开户行及账号
        /// </summary>
        public string? Account { get; set; }
    }

    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 销售方
    /// </summary>
    public SellerCls? Seller { get; set; } = new();


    public class SellerCls
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 地址、电话
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 开户行及账号
        /// </summary>
        public string? Account { get; set; }
    }

    /// <summary>
    /// 收款人
    /// </summary>
    public string? Payee { get; set; }

    /// <summary>
    /// 复核人
    /// </summary>
    public string? Reviewer { get; set; }

    /// <summary>
    /// 开票人
    /// </summary>
    public string? Drawer { get; set; }

    /// <summary>
    /// 交易明细
    /// </summary>
    public DetailCls? Detail { get; set; } = new();

    public class DetailCls
    {
        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 合计税额
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// 税价合计
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 税价合计大写
        /// </summary>
        public string TotalAmountString { get; set; }

        /// <summary>
        /// 项目列表
        /// </summary>
        public List<ItemCls>? Items { get; set; } = new();

        public class ItemCls
        {
            /// <summary>
            /// 货物或应税劳务、服务名称
            /// </summary>
            public string? Name { get; set; }

            /// <summary>
            /// 规格型号
            /// </summary>
            public string? Spec { get; set; }

            /// <summary>
            /// 单位
            /// </summary>
            public string? Unit { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            public decimal Qty { get; set; }

            /// <summary>
            /// 单价
            /// </summary>
            public decimal Price { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 税率
            /// </summary>
            public string? TaxRate { get; set; }

            /// <summary>
            /// 税额
            /// </summary>
            public decimal TaxAmount { get; set; }
        }
    }
}