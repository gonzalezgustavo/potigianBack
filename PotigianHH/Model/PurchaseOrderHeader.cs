using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T080_OC_CABE")]
    public class PurchaseOrderHeader
    {
        [Column("C_OC")]
        public decimal OrderCode { get; set; }

        [Column("U_PREFIJO_OC")]
        public decimal OrderPrefix { get; set; }

        [Column("U_SUFIJO_OC")]
        public decimal OrderSuffix { get; set; }

        [Column("C_PROVEEDOR")]
        public decimal ProviderCode { get; set; }

        [Column("C_SITUAC")]
        public decimal Situation { get; set; }

        [Column("F_EMISION")]
        public DateTime? EmissionDate { get; set; }

        [Column("C_COMPRADOR")]
        public decimal BuyerCode { get; set; }

        [Column("C_SUCU_DESTINO_ALT")]
        public decimal AlternativeDestinationBranch { get; set; }

        [Column("M_OC_MADRE")]
        public char MotherFlag { get; set; }

        [NotMapped]
        public int Items { get; set; }
    }
}
