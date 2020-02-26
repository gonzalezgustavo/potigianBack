using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T231_PEDIDOS_DETA")]
    public class RequestDetails
    {
        [Column("C_DOC")]
        public decimal DocumentCode { get; set; }

        [Column("U_DOC_PREFIJO")]
        public decimal DocumentPrefix { get; set; }

        [Column("U_DOC_SUFIJO")]
        public decimal DocumentSuffix { get; set; }

        [Column("C_ARTICULO")]
        public decimal? ArticleCode { get; set; }

        [Column("N_ARTICULO")]
        public string ArticleName { get; set; }

        [Column("U_ITEM_PEDIDO")]
        public decimal? RequestItem { get; set; }

        [Column("I_PRECIO_BASE_ORIG")]
        public decimal? OriginBasePrice { get; set; }

        [Column("I_PRECIO_VTA")]
        public decimal? SalePrice { get; set; }

        [Column("C_FAMILIA")]
        public decimal? FamilyCode { get; set; }

        [Column("M_FACTOR_VTA_ESP")]
        public char? SpecialSaleFactor { get; set; }

        [Column("Q_BULTOS_GRAMOS")]
        public decimal? PackagesGrams { get; set; }

        [Column("Q_FACTOR_PZAS")]
        public decimal? PiecesFactor { get; set; }

        [Column("I_PRECIO_UNIT_ART")]
        public decimal? ArticleUnitaryPrice { get; set; }

        [Column("I_PRECIO_UNIT_ART_FINAL")]
        public decimal? FinalArticleUnitaryPrice { get; set; }

        [Column("K_IVA")]
        public decimal? IVA { get; set; }

        [Column("I_TOTAL_ART")]
        public decimal? ArticleTotal { get; set; }

        [Column("Q_PESO_TOTAL_ARTICULO")]
        public decimal? TotalArticleWeight { get; set; }

        [Column("F_ALTA_SIST")]
        public DateTime InsertDate { get; set; }

        [Column("K_DESC_APLIC_A_PRECIO_VTA")]
        public decimal? DiscountAppliedToSalePrice { get; set; }

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
        public decimal SaleFactor { get; set; }

        public RequestDetails Append(Article article)
        {
            EanCode = article.EanCode;
            Dun14Code = article.Dun14Code;
            AlternativeEanCode1 = article.AlternativeEanCode1;
            AlternativeEanCode2 = article.AlternativeEanCode2;
            AlternativeEanCode3 = article.AlternativeEanCode3;
            AlternativeEanCode4 = article.AlternativeEanCode4;

            return this;
        }

        public RequestDetails Append(BranchArticle branchArticle)
        {
            SaleFactor = branchArticle.SaleFactor;

            return this;
        }
    }
}
