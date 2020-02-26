using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T081_OC_DETA")]
    public class PurchaseOrderDetails
    {
        [Column("C_OC")]
        public decimal OcCode { get; set; }

        [Column("U_PREFIJO_OC")]
        public decimal PrefixOcCode { get; set; }

        [Column("U_SUFIJO_OC")]
        public decimal SuffixOcCode { get; set; }

        [Column("C_ARTICULO")]
        public decimal ArticleCode { get; set; }

        [Column("Q_BULTOS_EMPR_PED")]
        public decimal RequestedPackages { get; set; }

        [Column("Q_BULTOS_PROV_PED")]
        public decimal RequestedBulks { get; set; }

        [Column("Q_FACTOR_PROV_PED")]
        public decimal RequestedFactor { get; set; }

        [NotMapped]
        public string ArticleName { get; set; }

        [NotMapped]
        public string EanCode { get; set; }

        [NotMapped]
        public string Dun14Code { get; set; }

        [NotMapped]
        public string AlternativeEanCode1 { get; set; }

        [NotMapped]
        public string AlternativeEanCode2 { get; set; }

        [NotMapped]
        public string AlternativeEanCode3 { get; set; }

        [NotMapped]
        public string AlternativeEanCode4 { get; set; }

        [NotMapped]
        public decimal ProviderFactor { get; set; }

        public PurchaseOrderDetails Append(Article article)
        {
            ArticleName = article.Name;
            EanCode = article.EanCode;
            Dun14Code = article.Dun14Code;
            AlternativeEanCode1 = article.AlternativeEanCode1;
            AlternativeEanCode2 = article.AlternativeEanCode2;
            AlternativeEanCode3 = article.AlternativeEanCode3;
            AlternativeEanCode4 = article.AlternativeEanCode4;

            return this;
        }

        public PurchaseOrderDetails Append(ProviderArticle providerArticle)
        {
            ProviderFactor = providerArticle.ProviderFactor;

            return this;
        }
    }
}
