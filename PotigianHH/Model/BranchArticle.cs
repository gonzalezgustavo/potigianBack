using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T051_ARTICULOS_SUCURSAL")]
    public class BranchArticle
    {
        [Column("C_ARTICULO")]
        [Key]
        public decimal? ArticleCode { get; set; }

        [Column("Q_FACTOR_VTA_SUCU")]
        public decimal SaleFactor { get; set; }
    }
}
