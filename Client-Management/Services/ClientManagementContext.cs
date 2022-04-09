using Client_Management.Model;
using Microsoft.EntityFrameworkCore;

namespace Client_Management.Services
{
    class ClientManagementContext : DbContext
    {
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupParameter> GroupParameters { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
           => options.UseMySql("server=" + Settings.GetInstance().DbServer + ";port=" + Settings.GetInstance().DbPort + ";database=ClientManagement;user=" + Settings.GetInstance().DbUser + ";password=" + Settings.GetInstance().DbPassword);
           //=> options.UseMySql("server=127.0.0.1;port=3307;database=ClientManagement;user=root;password=password");
    }
}
