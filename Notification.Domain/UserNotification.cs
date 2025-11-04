using System.ComponentModel.DataAnnotations;

namespace Notification.Domain;

public class UserNotification
{
    [Key]
    [Required]
    public Guid Id { get; set; }
    [Required]
    public int ToUserId { get; set; }
    [Required]
    public int FromUserId { get; set;}
    [Required]
    public required string Message { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public bool IsRead { get; set; }
}
