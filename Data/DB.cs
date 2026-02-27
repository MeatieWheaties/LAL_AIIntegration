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

        public DbSet<AIConfiguration> AI => Set<AIConfiguration>();

        public DbSet<AIConfigurationContext> AIContext => Set<AIConfigurationContext>();

        public DbSet<Assessment> Assessments => Set<Assessment>();

        public DbSet<Item> Items => Set<Item>();

        public DbSet<FoodRecommendation> Recommendations => Set<FoodRecommendation>();
    }

    public class TodoItem : Entity
    {
        public string Title { get; set; } = "";
        public bool Done { get; set; }
    }
}
