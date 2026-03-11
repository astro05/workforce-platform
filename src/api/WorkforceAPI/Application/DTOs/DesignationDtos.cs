using System.ComponentModel.DataAnnotations;

namespace WorkforceAPI.Application.DTOs;

public class DesignationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Level { get; set; }
    public int EmployeeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateDesignationDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Level { get; set; }
}

public class UpdateDesignationDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Level { get; set; }
}