using System.ComponentModel.DataAnnotations;

namespace Contoso_MVC_8_0_VS2022.Models
{
  public class OfficeAssignment
  {
    [Key]
    public int InstructorID { get; set; }
    [StringLength(50)]
    [Display(Name = "Office Location")]
    public string Location { get; set; }

    public Instructor Instructor { get; set; }
  }
}
