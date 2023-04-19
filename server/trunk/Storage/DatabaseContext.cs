using Deltar.Storage.Models.Habbo;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Ion.Storage
{
    public class DatabaseContext : DbContext
    {
        #region Fields

        private readonly string _server;
        private readonly uint _port;
        private readonly string _user;
        private readonly string _password;
        private readonly string _database;
        private readonly uint _minPoolSize;
        private readonly uint _maxPoolSize;

        #endregion

        #region DbSets

        public DbSet<Habbo> Habbo { get; set; }
        public DbSet<SsoTicket> SsoTicket { get; set; }

        #endregion

        public DatabaseContext(string server, uint port, string user, string password, string database, uint minPoolSize, uint maxPoolSize)
        {
            _server = server;
            _port = port;
            _user = user;
            _password = password;
            _database = database;
            _minPoolSize = minPoolSize;
            _maxPoolSize = maxPoolSize;
        }


        #region Public methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
            sqlConnectionStringBuilder.Server = _server;
            sqlConnectionStringBuilder.Port = _port;
            sqlConnectionStringBuilder.UserID = _user;
            sqlConnectionStringBuilder.Password = _password;

            sqlConnectionStringBuilder.Database = _database;
            sqlConnectionStringBuilder.MinimumPoolSize = _minPoolSize;
            sqlConnectionStringBuilder.MaximumPoolSize = _maxPoolSize;

            optionsBuilder
                //.UseLazyLoadingProxies()
                .UseMySQL(sqlConnectionStringBuilder.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Habbo>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Name).HasColumnName("username");
                entity.Property(x => x.Figure).HasColumnName("figure").HasDefaultValue();
                entity.Property(x => x.Gender).HasColumnName("gender").HasDefaultValue();
                entity.Property(x => x.Rank).HasColumnName("rank").HasDefaultValue();
                entity.Property(x => x.Credits).HasColumnName("credits").HasDefaultValue();
                entity.Property(x => x.ActivityPoints).HasColumnName("activity_points").HasDefaultValue();
                entity.Property(x => x.Motto).HasColumnName("motto").HasDefaultValue();
                entity.Property(x => x.JoinDate).HasColumnName("join_date").HasDefaultValue();
                entity.Property(x => x.LastOnline).HasColumnName("last_online").HasDefaultValue();
            });

            modelBuilder.Entity<SsoTicket>(entity =>
            {
                entity.ToTable("users_sso_tickets");
                entity.HasKey(x => new { x.Ticket, x.UserId });

                entity.Property(x => x.UserId).HasColumnName("user_id");
                entity.Property(x => x.Ticket).HasColumnName("sso_ticket");
                entity.Property(x => x.ExpireDate).HasColumnName("expires_at");
            });

            modelBuilder.Entity<SsoTicket>()
                .HasOne(e => e.HabboData)
                .WithMany(c => c.Tickets)
                .HasForeignKey(x => x.UserId);
        }

        #endregion
    }
}
