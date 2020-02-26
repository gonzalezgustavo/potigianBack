using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PotigianHH.Model
{
    [Table("T202_PREPARADORES_PRODUCTIVIDAD")]
    public class PreparerProductivity
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

        [Column("N_DIAS")]
        public int? Days { get; set; }

        [Column("N_HORAS")]
        public int? Hours { get; set; }

        [Column("N_MINUTOS")]
        public int? Minutes { get; set; }

        [Column("N_SEGUNDOS")]
        public int? Seconds { get; set; }

        [Column("C_ESTADO")]
        public int? StatusCode { get; set; }

        public static PreparerProductivity FromPreparation(RequestPreparation prep)
        {
            return new PreparerProductivity
            {
                Code = prep.Code,
                DocumentSuffix = prep.DocumentSuffix,
                MovementFlag = prep.MovementFlag,
                InsertDate = prep.InsertDate,
                StartDate = prep.StartDate,
                EndDate = prep.EndDate,
                StatusCode = prep.StatusCode
            };
        }

    }
}
