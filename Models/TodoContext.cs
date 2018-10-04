using Evaluation.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Evaluation.API.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // set name of collection
            var db = modelBuilder.Entity<TodoItem>().Metadata;
            db.CosmosSql().CollectionName = nameof(TodoItems);
        }

        public DbSet<TodoItem> TodoItems { get; set; }
    }
}