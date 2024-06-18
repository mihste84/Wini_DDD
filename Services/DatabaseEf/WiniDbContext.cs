using Microsoft.EntityFrameworkCore;
using DatabaseCommon.Models;

namespace DatabaseEf;

public partial class WiniDbContext : DbContext
{
    public WiniDbContext()
    {
    }

    public WiniDbContext(DbContextOptions<WiniDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingRow> BookingRows { get; set; }

    public virtual DbSet<BookingStatusLog> BookingStatusLogs { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<RecipientMessage> RecipientMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasIndex(e => e.BookingId, "IX_Attachments_BookingId");

            entity.HasIndex(e => new { e.BookingId, e.Name, e.Size }, "IX_Attachments_BookingId_Name_Size")
                .IsUnique()
                .HasFilter("([BookingId] IS NOT NULL AND [Name] IS NOT NULL AND [Size] IS NOT NULL)");

            entity.Property(e => e.ContentType).HasMaxLength(200);
            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(300);
            entity.Property(e => e.Path).HasMaxLength(400);

            entity.HasOne(d => d.Booking).WithMany(p => p.Attachments).HasForeignKey(d => d.BookingId);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasIndex(e => new { e.BookingDate, e.Status }, "IX_Bookings_BookingDate_Status");

            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken(false);
            entity.Property(e => e.TextToE1).HasMaxLength(30);
            entity.Property(e => e.Updated).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);
        });

        modelBuilder.Entity<BookingRow>(entity =>
        {
            entity.HasIndex(e => e.BookingId, "IX_BookingRows_BookingId");

            entity.Property(e => e.Account).HasMaxLength(6);
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Authorizer).HasMaxLength(200);
            entity.Property(e => e.BusinessUnit).HasMaxLength(12);
            entity.Property(e => e.CostObject1).HasMaxLength(12);
            entity.Property(e => e.CostObject2).HasMaxLength(12);
            entity.Property(e => e.CostObject3).HasMaxLength(12);
            entity.Property(e => e.CostObject4).HasMaxLength(12);
            entity.Property(e => e.CostObjectType1).HasMaxLength(1);
            entity.Property(e => e.CostObjectType2).HasMaxLength(1);
            entity.Property(e => e.CostObjectType3).HasMaxLength(1);
            entity.Property(e => e.CostObjectType4).HasMaxLength(1);
            entity.Property(e => e.CurrencyCode).HasMaxLength(3);
            entity.Property(e => e.ExchangeRate).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Remark).HasMaxLength(30);
            entity.Property(e => e.Subledger).HasMaxLength(8);
            entity.Property(e => e.SubledgerType).HasMaxLength(1);
            entity.Property(e => e.Subsidiary).HasMaxLength(8);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingRows).HasForeignKey(d => d.BookingId);
        });

        modelBuilder.Entity<BookingStatusLog>(entity =>
        {
            entity.HasIndex(e => new { e.BookingId, e.Status, e.Created }, "IX_BookingStatusLogs_BookingId").IsUnique();

            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingStatusLogs).HasForeignKey(d => d.BookingId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasIndex(e => new { e.BookingId, e.Created }, "IX_Comments_BookingId").IsUnique();

            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.Value).HasMaxLength(300);

            entity.HasOne(d => d.Booking).WithMany(p => p.Comments).HasForeignKey(d => d.BookingId);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_Companies_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(5);
            entity.Property(e => e.CountryCode).HasMaxLength(2);
            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Updated).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);
        });

        modelBuilder.Entity<RecipientMessage>(entity =>
        {
            entity.HasIndex(e => e.BookingId, "IX_RecipientMessages_BookingId");

            entity.HasIndex(e => new { e.BookingId, e.Recipient }, "IX_RecipientMessages_BookingId_Name_Size")
                .IsUnique()
                .HasFilter("([BookingId] IS NOT NULL AND [Recipient] IS NOT NULL)");

            entity.Property(e => e.Created).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.Recipient).HasMaxLength(200);
            entity.Property(e => e.Value).HasMaxLength(300);

            entity.HasOne(d => d.Booking).WithMany(p => p.RecipientMessages).HasForeignKey(d => d.BookingId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
