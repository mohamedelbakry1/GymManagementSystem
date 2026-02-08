using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Data.Configurations
{
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {
            builder.Property(X => X.CreatedAt)
                   .HasColumnName("StartDate")
                   .HasDefaultValueSql("GETDATE()");

            builder.HasKey(X => new { X.PlanId, X.MemberId });

            builder.Ignore(X => X.Id);
        }
    }
}
