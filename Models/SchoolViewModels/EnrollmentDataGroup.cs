using System;
using System.ComponentModel.DataAnnotations;

namespace Contoso_MVC_8_0_VS2022.Models.SchoolViewModels
{
  public class EnrollmentDateGroup
  {
    [DataType(DataType.Date)]
    public DateTime? EnrollmentDate { get; set; }

    public int StudentCount { get; set; }
  }
}
