using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T050_ARTICULOS")]
    public class Article
    {
        [Key]
        [Column("C_ARTICULO")]
        public decimal Code { get; set; }

        [Column("C_EAN")]
        public string EanCode { get; set; }

        [Column("C_DUN14")]
        public string Dun14Code { get; set; }

        [Column("C_EAN_ALTERNATIVO_1")]
        public string AlternativeEanCode1 { get; set; }

        [Column("C_EAN_ALTERNATIVO_2")]
        public string AlternativeEanCode2 { get; set; }

        [Column("C_EAN_ALTERNATIVO_3")]
        public string AlternativeEanCode3 { get; set; }

        [Column("C_EAN_ALTERNATIVO_4")]
        public string AlternativeEanCode4 { get; set; }
    }
}
