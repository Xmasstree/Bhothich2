using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bhothich2;

public partial class TestContext : DbContext
{
    public TestContext()
    {
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=test;Username=postgres;Password=AbobaabobA");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.telegramID)
                .HasMaxLength(50)
                .HasColumnName("telegramid");
            entity.Property(e => e.Answers).HasColumnName("answers");
            entity.Property(e => e.Alive).HasDefaultValueSql("true").HasColumnName("alive");
            entity.Property(e => e.Bullet).HasDefaultValueSql("true").HasColumnName("bullet");
            entity.Property(e => e.Points).HasColumnName("points");
            entity.Property(e => e.TargetId)
                .HasMaxLength (50)
                .HasColumnName ("targetid");
            entity.Property(e => e.TimeOfDeath)
                .HasDefaultValueSql("'2004-12-16 00:00:00'::timestamp without time zone")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timeofdeath");
            entity.Property(e => e.TimeOfShot)
                .HasDefaultValueSql("'2004-12-16 00:00:00'::timestamp without time zone")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timeofshot");
            entity.Property(e => e.CorrectOfShot).HasDefaultValueSql("false").HasColumnName("correctofshot");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
