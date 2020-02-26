using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T231_PEDIDOS_PREPARACION")]
    public class RequestPreparation
    {
        [Column("C_CODIGO")]
        public string Code { get; set; }

        [Column("C_MOV")]
        public char? MovementFlag { get; set; }

        [Column("U_DOC_SUFIJO")]
        [Key]
        public decimal? DocumentSuffix { get; set; }

        [Column("F_ALTA_SIST")]
        public DateTime InsertDate { get; set; }

        [Column("F_INICIO")]
        public DateTime? StartDate { get; set; }

        [Column("F_FIN")]
        public DateTime? EndDate { get; set; }

        [Column("C_ESTADO")]
        public int? StatusCode { get; set; }

    }
}
