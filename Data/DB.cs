namespace LunchAndLearn_AIIntegration.Data
{
    using LunchAndLearn_AIIntegration.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public abstract class Entity
    {
        public int Id { get; set; }
    }

    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options) { }

        public DbSet<Kobold> Kobolds => Set<Kobold>();
        public DbSet<AI_KoboldAssessment> AI_KoboldAssessments => Set<AI_KoboldAssessment>();
    }

    public class TodoItem : Entity
    {
        public string Title { get; set; } = "";
        public bool Done { get; set; }
    }
}
