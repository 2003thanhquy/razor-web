using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace App.Models;

public class AppDbContext : IdentityDbContext<AppUser>{

        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) {

         }
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder){
                base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);

            foreach(var entityType in builder.Model.GetEntityTypes()){
                var tableName = entityType.GetTableName();
                if(tableName.StartsWith("AspNet")){
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
        public  DbSet<Article> articles  {get; set;}    

    }