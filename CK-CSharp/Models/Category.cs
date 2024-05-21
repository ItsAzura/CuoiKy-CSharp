using System.ComponentModel.DataAnnotations;

namespace CK_CSharp.Models
{
    public class Category
    {
        [Key]
        [Display(Name = "Id Thể Loại")]
        public int CategoryId { get; set; }

        [Display(Name = "Tên Thể Loại")]
        public string Name { get; set; }

        [Display(Name = "Mô Tả")]
        public string Description { get; set; }

    }
}
