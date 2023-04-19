using Ion.HabboHotel.Habbos;
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

        public string Server { get; }
        public uint Port { get; }
        public string User { get; }
        public string Password { get; }
        public string Database { get; }
        public uint MinPoolSize { get; }
        public uint MaxPoolSize { get; }
        
        #endregion

        #region DbSets

        public DbSet<Habbo> Habbo { get; set; }

        #endregion

        public DatabaseContext(string server, uint port, string user, string password, string database, uint minPoolSize, uint maxPoolSize)
        {
            Server = server;
            Port = port;
            User = user;
            Password = password;
            Database = database;
            MinPoolSize = minPoolSize;
            MaxPoolSize = maxPoolSize;
        }

        #region Public methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqlConnectionStringBuilder = new MySqlConnectionStringBuilder();
            sqlConnectionStringBuilder.Server = Server;
            sqlConnectionStringBuilder.Port = Port;
            sqlConnectionStringBuilder.UserID = User;
            sqlConnectionStringBuilder.Database = Database;
            sqlConnectionStringBuilder.MinimumPoolSize = MinPoolSize;
            sqlConnectionStringBuilder.MaximumPoolSize = MaxPoolSize;

            optionsBuilder
                //.UseLazyLoadingProxies()
                .UseMySQL(sqlConnectionStringBuilder.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        #endregion
    }
}
