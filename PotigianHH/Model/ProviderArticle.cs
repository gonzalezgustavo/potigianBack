using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T052_ARTICULOS_PROVEEDOR")]
    public class ProviderArticle
    {
        [Key]
        [Column("C_ARTICULO")]
        public decimal ArticleCode { get; set; }

        [Column("Q_FACTOR_PROVEEDOR")]
        public decimal ProviderFactor { get; set; }
    }
}
