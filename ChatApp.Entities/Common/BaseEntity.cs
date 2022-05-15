using System.ComponentModel.DataAnnotations;

namespace ChatApp.Entities.Common
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}
