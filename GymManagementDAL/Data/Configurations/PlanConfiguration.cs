using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Data.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(P => P.Name)
                   .HasColumnType("varchar").HasMaxLength(50);

            builder.Property(P => P.Description)
                   .HasColumnType("varchar").HasMaxLength(200);

            builder.Property(P => P.Price).HasPrecision(10, 2);

            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("CheckDurationDaysValid", "DurationDays between 1 and 365");
            });
        }
    }
}
