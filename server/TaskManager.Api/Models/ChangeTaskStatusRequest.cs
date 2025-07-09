using System.ComponentModel.DataAnnotations;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Models;

public class ChangeTaskStatusRequest
{
    [Required]
    public TaskStatusType Status { get; set; }
}