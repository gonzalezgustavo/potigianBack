using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PotigianHH.Model
{
    [Table("T202_PREPARADORES")]
    public class Preparer
    {
        public int Id => int.Parse(Code);

        [Column("N_DESCRIPCION")]
        public string Name { get; set; }

        [Column("C_TIPO")]
        public char Type { get; set;  }

        [JsonIgnore]
        [Key]
        [Column("C_CODIGO")]
        public string Code { get; set; }
    }
}
