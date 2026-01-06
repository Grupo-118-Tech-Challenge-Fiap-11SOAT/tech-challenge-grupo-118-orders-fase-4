namespace Application.Base.Dtos;

public abstract class BaseDto
{
    protected BaseDto()
    {
        Error = false;
        ErrorMessage = string.Empty;
    }

    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    public string ErrorMessage { get; set; }
    public bool Error { get; set; } = false;
}

