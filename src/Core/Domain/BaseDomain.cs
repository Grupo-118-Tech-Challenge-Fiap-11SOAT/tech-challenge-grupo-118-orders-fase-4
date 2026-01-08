using Domain.Products.Entities;

namespace Domain;

public class BaseDomain <TId>
{
    public TId Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public bool IsActive { get; set; }
    
    public BaseDomain(TId id, DateTimeOffset? createdAt = null, DateTimeOffset? updatedAt = null)
    {
        if (!Equals(id, default))
            this.Id = id;

        if (createdAt is not null)
            this.CreatedAt = createdAt.Value;

        if (updatedAt is not null)
            this.UpdatedAt = updatedAt.Value;
    }
    
    public BaseDomain()
    {
    }
}