using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementDAL.Data.Configurations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(G => G.Name)
                   .HasColumnType("varchar").HasMaxLength(50);

            builder.Property(G => G.Email)
                   .HasColumnType("varchar").HasMaxLength(100);

            builder.Property(G => G.Phone)
                   .HasColumnType("varchar").HasMaxLength(11);

            builder.ToTable(Tb =>
            {
                Tb.HasCheckConstraint("CheckEmailValid","Email like '_%@_%._%'");
                Tb.HasCheckConstraint("CheckPhoneValid", "Phone like '01%' and Phone not like '%[^0-9]%'");
            });

            builder.HasIndex(G => G.Email).IsUnique();
            builder.HasIndex(G => G.Phone).IsUnique();

            builder.OwnsOne(G => G.Address, AddressBuilder =>
            {
                AddressBuilder.Property(A => A.BuildingNumber)
                              .HasColumnName("BuildingNumber");

                AddressBuilder.Property(A => A.Street)
                              .HasColumnName("Street").HasColumnType("varchar").HasMaxLength(30);

                AddressBuilder.Property(A => A.City)
                              .HasColumnName("City").HasColumnType("varchar").HasMaxLength(30);
            });
        }
    }
}
