using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T231_PEDIDOS_CABE")]
    public class RequestHeaders
    {
        [Column("C_DOC")]
        public decimal DocumentCode { get; set; }

        [Column("U_DOC_PREFIJO")]
        public decimal DocumentPrefix { get; set; }

        [Column("U_DOC_SUFIJO")]
        public decimal DocumentSuffix { get; set; }

        [Column("C_SUCU_ORIG_ALTA")]
        public decimal? OriginBranch { get; set; }

        [Column("F_ALTA_SIST")]
        public DateTime InsertDate { get; set; }

        [Column("C_SITUAC")]
        public decimal? SituationCode { get; set; }

        [Column("F_SITUAC")]
        public DateTime SituationDate { get; set; }

        [Column("P_UTILIDAD")]
        public decimal? Utility { get; set; }

        [Column("I_TOTAL")]
        public decimal? TotalPrice { get; set; }

        [Column("Q_ITEMS_DOC")]
        public decimal? DocumentItems { get; set; }

        [Column("Q_TOTAL_BULTOS")]
        public decimal? TotalPackages { get; set; }

        [Column("Q_TOTAL_PESO")]
        public decimal? TotalWeight { get; set; }

        [Column("C_CLIENTE")]
        public decimal? ClientCode { get; set; }

        [Column("N_CLIENTE")]
        public string ClientName { get; set; }

        [Column("C_CUIT_CLIENTE")]
        public string ClientCUIT { get; set; }

        [Column("C_PROVINCIA_CLIENTE")]
        public decimal? ClientProvinceCode { get; set; }

        [Column("N_DIRECCION_CLIENTE")]
        public string ClientAddress { get; set; }

        [Column("C_POSTAL_CLIENTE")]
        public string ClientPostalCode { get; set; }

        [Column("N_LOCALIDAD_CLIENTE")]
        public string ClientTown { get; set; }

        [Column("C_COND_IVA_CLIENTE")]
        public decimal? ClientIVACondition { get; set; }

        [Column("M_EXENTO_IVA")]
        public char? ClientIVAExemptFlag { get; set; }

        [Column("M_IB_CLIENTE")]
        public char? ClientGrossIncomeFlag { get; set; }

        [Column("C_FORMA_PAGO")]
        public decimal? PaymentMethodCode { get; set; }

        [Column("C_COMISIONISTA")]
        public decimal? CommissionAgentCode { get; set; }

        [Column("N_COMISIONISTA")]
        public string CommissionAgentName { get; set; }

        [Column("C_VENDEDOR")]
        public decimal? SellerCode { get; set; }

        [Column("N_VENDEDOR")]
        public string SellerName { get; set; }

        [Column("M_BAUTIZADOS")]
        public char? BaptizedFlag { get; set; }

        [Column("K_RECAR_ART_OFER")]
        public decimal? ArticleForSaleRecharge { get; set; }

        [Column("K_RECAR_ART_NO_OFER")]
        public decimal? ArticleNotForSaleRecharge { get; set; }

        [Column("C_TARJETA")]
        public decimal? CardCode { get; set; }

        [Column("U_CUOTAS_TARJETA")]
        public decimal? CardInstallmentCount { get; set; }

        [Column("K_RECAR_TARJETA")]
        public decimal? CardRecharge { get; set; }

        [Column("C_USUARIO")]
        public string UserCode { get; set; }

        [Column("C_TERMINAL")]
        public string TerminalCode { get; set; }

        [Column("D_OBSERVACION")]
        public string Observations { get; set; }

        [Column("M_MOBILE")]
        public char? MobileFlag { get; set; }

        [Column("C_CLIENTES_CF")]
        public decimal? CFClientCode { get; set; }

        [Column("C_USUARIO_PEDIDO")]
        public string RequestUserCode { get; set; }

        [Column("N_REFERENCIA")]
        public decimal? ReferenceNumber { get; set; }

        [Column("N_RECORRIDO")]
        public int? PathNumber { get; set; }

        [Column("N_REPARTO")]
        public int? DistributionNumber { get; set; }

        [Column("M_HABILITADO")]
        public string EnabledFlag { get; set; }

        [Column("C_PREPARADOR")]
        public string PreparerCode { get; set; }

        [Column("F_PREPARACION")]
        public DateTime? PreparerDate { get; set; }

        [Column("N_ORDEN_COMP")]
        public string PurchaseOrderNumber { get; set; }

        [Column("N_PEDIDO_APP")]
        public string AppRequestNumber { get; set; }

        [Column("N_MOD_VTA")]
        public char? SaleMode { get; set; }
    }
}
