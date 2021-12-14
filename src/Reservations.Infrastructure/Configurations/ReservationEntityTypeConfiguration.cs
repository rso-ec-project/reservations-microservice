using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reservations.Domain.ReservationAggregate;

namespace Reservations.Infrastructure.Configurations
{
    public class ReservationEntityTypeConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("reservation");

            builder.Property(x => x.From)
                .HasColumnName("from")
                .IsRequired();

            builder.Property(x => x.To)
                .HasColumnName("to")
                .IsRequired();

            builder.Property(x => x.Code)
                .HasColumnName("code")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.ChargerId)
                .HasColumnName("charger_id")
                .IsRequired();

            builder.Property(x => x.StatusId)
                .HasColumnName("status_id")
                .IsRequired();

            builder.HasOne(x => x.Status)
                .WithMany(y => y.Reservations)
                .HasForeignKey(z => z.StatusId)
                .HasConstraintName("FK_Reservation_Status_StatusId");
        }
    }
}
