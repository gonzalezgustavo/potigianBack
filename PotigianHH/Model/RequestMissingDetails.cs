using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T231_PEDIDOS_DETA_FALTANTES")]
    public class RequestMissingDetails
    {
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

        [Column("K_IVA")]
        public decimal? IVA { get; set; }

        [Column("I_TOTAL_ART")]
        public decimal? ArticleTotal { get; set; }

        [Column("Q_PESO_TOTAL_ARTICULO")]
        public decimal? TotalArticleWeight { get; set; }

        [Column("Q_UNID_GRAMOS_PEDIDO")]
        public decimal? RequestGramsUnit { get; set; }

        [Column("Q_UNID_GRAMOS_CUMPLIDO")]
        public decimal? DeliveredGramsUnit { get; set; }

        [Column("Q_UNID_GRAMOS_SALDO")]
        public decimal? PendingGramsUnit { get; set; }

        [Column("F_ALTA_SIST")]
        public DateTime InsertDate { get; set; }

        [Column("K_DESC_APLIC_A_PRECIO_VTA")]
        public decimal? DiscountAppliedToSalePrice { get; set; }

        public RequestMissingDetails()
        {
        }

        public RequestMissingDetails(RequestDetails details, int actualCount)
        {
            this.DocumentPrefix = details.DocumentPrefix;
            this.DocumentSuffix = details.DocumentSuffix;
            this.ArticleCode = details.ArticleCode;
            this.ArticleName = details.ArticleName;
            this.RequestItem = details.RequestItem;
            this.OriginBasePrice = details.OriginBasePrice;
            this.SalePrice = details.SalePrice;
            this.FamilyCode = details.FamilyCode;
            this.SpecialSaleFactor = details.SpecialSaleFactor;
            this.PackagesGrams = details.PackagesGrams;
            this.PiecesFactor = details.PiecesFactor;
            this.ArticleUnitaryPrice = details.ArticleUnitaryPrice;
            this.IVA = details.IVA;
            this.ArticleTotal = details.ArticleTotal;
            this.TotalArticleWeight = details.TotalArticleWeight;
            this.RequestGramsUnit = details.PackagesGrams;
            this.DeliveredGramsUnit = actualCount;
            this.PendingGramsUnit = this.PackagesGrams - this.DeliveredGramsUnit;
            this.InsertDate = DateTime.Now;
            this.DiscountAppliedToSalePrice = details.DiscountAppliedToSalePrice;
        }
    }
}
