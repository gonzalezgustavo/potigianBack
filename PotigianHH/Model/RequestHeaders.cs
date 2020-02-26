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

        [Column("N_RECORRIDO")]
        public decimal? PathNumber { get; set; }

        [Column("N_REPARTO")]
        public decimal? DistributionNumber { get; set; }

        [Column("C_PREPARADOR")]
        public string PreparerCode { get; set; }

        [Column("C_IMPRESORA")]
        public int Printer { get; set; }
    }
}
