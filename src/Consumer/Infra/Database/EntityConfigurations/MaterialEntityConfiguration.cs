using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.EntityConfigurations;

public class MaterialEntityConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(m => m.Deleted).IsRequired();

        builder.Property(m => m.Name).IsRequired().HasMaxLength(64);

        builder.Property(m => m.Description).HasMaxLength(255);

        builder.Property(m => m.Price).IsRequired().HasColumnType("decimal(18,2)");

        builder.Property(m => m.StockQuantity).IsRequired();

        builder.Property(m => m.CreatedAt).IsRequired();

        builder.Property(m => m.CreatedBy).IsRequired();

        builder.Property(m => m.ModifiedAt).IsRequired(false);

        builder.Property(m => m.ModifiedBy).IsRequired(false);
    }
}
