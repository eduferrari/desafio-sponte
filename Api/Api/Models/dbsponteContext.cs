using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Models
{
    public partial class dbsponteContext : DbContext
    {
        public dbsponteContext()
        {
        }

        public dbsponteContext(DbContextOptions<dbsponteContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Alunos> Alunos { get; set; }
        public virtual DbSet<Cursos> Cursos { get; set; }
        public virtual DbSet<Matriculas> Matriculas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("server=pya32hb7m2.underplatform.com;port=3306;user=usrsponte;password=aTk83h@2YQpcsgck;database=dbsponte");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alunos>(entity =>
            {
                entity.HasKey(e => e.AlunoId)
                    .HasName("PRIMARY");

                entity.ToTable("alunos");

                entity.Property(e => e.AlunoId)
                    .HasColumnName("AlunoID")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Cpf)
                    .IsRequired()
                    .HasColumnName("CPF")
                    .HasMaxLength(20)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DataNascimento)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'0000-00-00'");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Foto)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<Cursos>(entity =>
            {
                entity.HasKey(e => e.CursoId)
                    .HasName("PRIMARY");

                entity.ToTable("cursos");

                entity.Property(e => e.CursoId)
                    .HasColumnName("CursoID")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.DataLimiteMatricula)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'0000-00-00'");

                entity.Property(e => e.Duracao).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<Matriculas>(entity =>
            {
                entity.HasKey(e => e.MatriculaId)
                    .HasName("PRIMARY");

                entity.ToTable("matriculas");

                entity.Property(e => e.MatriculaId)
                    .HasColumnName("MatriculaID")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AlunoId)
                    .HasColumnName("AlunoID")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Cursos)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasDefaultValueSql("'0000-00-00'");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
