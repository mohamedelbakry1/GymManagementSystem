using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Data.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("CheckCapacityValid", "Capacity between 1 and 25");
                Tb.HasCheckConstraint("CheckEndDateValid", "EndDate > StartDate");
            });

            builder.HasOne(S => S.SessionCategory)
                   .WithMany(C => C.Sessions)
                   .HasForeignKey(S => S.CategoryId);

            builder.HasOne(S => S.SessionTrainer)
                   .WithMany(T => T.TrainerSessions)
                   .HasForeignKey(S => S.TrainerId);
        }
    }
}
