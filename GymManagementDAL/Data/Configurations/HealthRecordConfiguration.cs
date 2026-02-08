using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Data.Configurations
{
    public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {
            builder.ToTable("Members")
                   .HasKey(X => X.Id);

            builder.HasOne<Member>()
                   .WithOne(X => X.HealthRecord)
                   .HasForeignKey<HealthRecord>(X => X.Id);

            builder.Ignore(X => X.CreatedAt);
            builder.Ignore(X => X.UpdatedAt);
        }
    }
}
