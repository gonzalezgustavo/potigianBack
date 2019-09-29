using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("TUsuarios")]
    public class User
    {
        [Key]
        [Column("Codigo")]
        public string Code { get; set; }

        [Column("Nombre")]
        public string Name { get; set; }

        [Column("AccessCode")]
        public string AccessCode { get; set; }
    }
}
