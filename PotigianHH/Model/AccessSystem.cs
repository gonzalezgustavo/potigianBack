using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("TSistemasAccesos")]
    public class AccessSystem
    {
        [Key]
        [Column("Sistema")]
        public string Name { get; set; }

        [Column("AccessCode")]
        public string AccessCode { get; set; }
    }
}
