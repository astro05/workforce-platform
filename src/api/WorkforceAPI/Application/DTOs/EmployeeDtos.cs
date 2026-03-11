using System.ComponentModel.DataAnnotations;

namespace WorkforceAPI.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal Salary { get; set; }
    public DateTime JoiningDate { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? AvatarUrl { get; set; }
    public string[] Skills { get; set; } = [];
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int DesignationId { get; set; }
    public string DesignationName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateEmployeeDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value.")]
    public decimal Salary { get; set; }

    [Required]
    public DateTime JoiningDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required.")]
    public int DepartmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DesignationId is required.")]
    public int DesignationId { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    public string[] Skills { get; set; } = [];
}

public class UpdateEmployeeDto
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive value.")]
    public decimal Salary { get; set; }

    public bool IsActive { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required.")]
    public int DepartmentId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DesignationId is required.")]
    public int DesignationId { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(300)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }

    public string[] Skills { get; set; } = [];
}