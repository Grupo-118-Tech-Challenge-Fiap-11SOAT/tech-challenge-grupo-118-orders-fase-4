using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Database.Postgres.Order.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Domain.Order.Entities.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Order.Entities.Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired();

        builder.Property(o => o.Cpf)
            .HasMaxLength(11);

        builder.Property(o => o.Total)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasColumnType("datetimeoffset")
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnAdd();

        builder.Property(o => o.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .HasDefaultValueSql("SYSDATETIMEOFFSET()")
            .ValueGeneratedOnUpdate();

        builder.Ignore(o => o.IsActive);

        // Relationship (optional, but recommended for navigation)
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    

    public void Configure(EntityTypeBuilder<object> builder)
    {
        throw new NotImplementedException();
    }
}