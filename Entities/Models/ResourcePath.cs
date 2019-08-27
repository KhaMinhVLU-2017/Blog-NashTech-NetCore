using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class ResourcePath
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourcePathID { get;set; }

        public int RoleID { get; set; }

        public virtual Role Role { get; set; }

        public string Path { get; set; }
    }
}
