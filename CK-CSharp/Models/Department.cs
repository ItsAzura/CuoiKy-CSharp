using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CK_CSharp.Models
{
    public class Department
    {
        [Key]
        [Display(Name = "Id Phòng Ban")]
        public int DepartmentId { get; set; }

        [Display(Name = "Tên Phòng Ban")]
        public string Name { get; set; }

        [Display(Name = "Mô Tả")]
        public string Description { get; set; }

        [ForeignKey("Company")]
        [Display(Name = "Công Ty Id")]
        public int? CompanyId { get; set; }

        public Company Company { get; set; }

        public List<Employee> Employees { get; set; }
    }
}
