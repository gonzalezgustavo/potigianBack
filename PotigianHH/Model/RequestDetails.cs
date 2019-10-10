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

        [Column("I_BAUTIZADO")]
        public decimal? Baptized { get; set; }

        [Column("I_PRECIO_MANUAL")]
        public decimal? ManualPrice { get; set; }

        [Column("C_FAMILIA")]
        public decimal? FamilyCode { get; set; }

        [Column("M_FACTOR_VTA_ESP")]
        public char? SpecialSaleFactor { get; set; }

        [Column("M_VENDE_POR_PESO")]
        public char? SalePerWeight { get; set; }

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

        [Column("C_ENVASE")]
        public decimal? ContainerCode { get; set; }

        [Column("I_TOTAL_NETO_ENVASES")]
        public decimal? TotalContainerNet { get; set; }

        [Column("Q_UNID_GRAMOS_PEDIDO")]
        public decimal? RequestGramsUnit { get; set; }

        [Column("Q_UNID_GRAMOS_CUMPLIDO")]
        public decimal? DeliveredGramsUnit { get; set; }

        [Column("Q_UNID_GRAMOS_SALDO")]
        public decimal? PendingGramsUnit { get; set; }

        [Column("C_DOC_LETRA_FACT")]
        public char? BillDocumentLetter { get; set; }

        [Column("U_DOC_PREFIJO_FACT")]
        public decimal? BillDocumentPrefix { get; set; }

        [Column("U_DOC_SUFIJO_FACT")]
        public decimal? BillDocumentSuffix { get; set; }

        [Column("F_ALTA_SIST")]
        public DateTime InsertDate { get; set; }

        [Column("K_DESC_APLIC_A_PRECIO_VTA")]
        public decimal? DiscountAppliedToSalePrice { get; set; }

        [Column("M_MOBILE")]
        public char? MobileFlag { get; set; }

        [Column("M_CAJA_CERRADA")]
        public char? ClosedBoxFlag { get; set; }

        [Column("M_MODIFICA_LINEA")]
        public char? LineModifiedFlag { get; set; }

        [Column("M_FACTOR_VTA_FRACCION")]
        public char? FractionSaleFactorFlag { get; set; }

        [Column("M_MODIFICO_PRECIO")]
        public char? ModifiedPriceFlag { get; set; }

        [Column("C_USUARIO_MODIFICO_PRECIO")]
        public string ModifiedPriceUserCode { get; set; }

        [Column("M_NEGOCIO_ESPECIAL")]
        public char? SpecialBusinessFlag { get; set; }

        [Column("I_PRECIO_VTA_NEGOCIO_ESPECIAL")]
        public decimal? SpecialBusinessSalePrice { get; set; }

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
