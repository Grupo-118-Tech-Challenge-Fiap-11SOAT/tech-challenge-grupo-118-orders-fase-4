namespace Domain;

public class BaseDomain
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    
    public BaseDomain(int id = 0, DateTimeOffset? createdAt = null, DateTimeOffset? updatedAt = null)
    {
        if (id != 0)
            this.Id = id;

        if (createdAt is not null)
            this.CreatedAt = createdAt.Value;

        if (updatedAt is not null)
            this.UpdatedAt = updatedAt.Value;
    }
}