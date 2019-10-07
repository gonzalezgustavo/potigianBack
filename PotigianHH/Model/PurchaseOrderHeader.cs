﻿using System;
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

        [Column("F_ALTA_SIST")]
        public DateTime InsertDate { get; set; }

        [Column("C_COMPRADOR")]
        public decimal BuyerCode { get; set; }
    }
}