using System.ComponentModel.DataAnnotations;

namespace ConsultationApplication.DTOs;

public class CreateSlotDto : IValidatableObject
{
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime <= DateTime.UtcNow)
            yield return new ValidationResult("Start time must be in the future.", [nameof(StartTime)]);

        if (EndTime <= StartTime)
            yield return new ValidationResult("End time must be after start time.", [nameof(EndTime)]);
    }
}