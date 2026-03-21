using Edinburgh_Internation_Students.Models;
using Microsoft.EntityFrameworkCore;

namespace Edinburgh_Internation_Students.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupIcebreaker> GroupIcebreakers { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<PollVote> PollVotes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageReaction> MessageReactions { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventAttendee> EventAttendees { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(60);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("User");

            entity.Property(e => e.PhoneCode)
                .HasMaxLength(5);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.UserId)
                .IsUnique();

            entity.Property(e => e.HomeCountry)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ShortBio)
                .HasMaxLength(500);

            entity.Property(e => e.Campus)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.MajorFieldOfStudy)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.YearOfStudy)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.Interests)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.PreferredGroupSize)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.MatchingPreference)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.Languages)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Groups
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.ExpiresAt);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // GroupMembers
        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.GroupId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.GroupId, e.UserId })
                .IsUnique();

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // GroupIcebreakers
        modelBuilder.Entity<GroupIcebreaker>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Icebreakers)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Question)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // ChecklistItems
        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.GroupId);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.ChecklistItems)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CompletedByUser)
                .WithMany()
                .HasForeignKey(e => e.CompletedBy)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Polls
        modelBuilder.Entity<Poll>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.GroupId);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Polls)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Question)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // PollOptions
        modelBuilder.Entity<PollOption>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Poll)
                .WithMany(p => p.Options)
                .HasForeignKey(e => e.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Text)
                .IsRequired()
                .HasMaxLength(255);
        });

        // PollVotes
        modelBuilder.Entity<PollVote>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.PollId, e.UserId })
                .IsUnique();

            entity.HasOne(e => e.Poll)
                .WithMany(p => p.Votes)
                .HasForeignKey(e => e.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Option)
                .WithMany(o => o.Votes)
                .HasForeignKey(e => e.OptionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.VotedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Messages
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.GroupId);
            entity.HasIndex(e => e.SenderId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Messages)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Sender)
                .WithMany()
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Content)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // MessageReactions
        modelBuilder.Entity<MessageReaction>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.MessageId, e.UserId, e.Emoji })
                .IsUnique();

            entity.HasOne(e => e.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Emoji)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Events
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.OrganizerId);

            entity.HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Category)
                .HasConversion<int>();

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // EventAttendees
        modelBuilder.Entity<EventAttendee>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.EventId, e.UserId })
                .IsUnique();

            entity.HasOne(e => e.Event)
                .WithMany(ev => ev.Attendees)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.RsvpAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Notifications
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Type)
                .HasConversion<int>();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Message)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Announcements
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Author)
                .WithMany()
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Content)
                .IsRequired();

            entity.Property(e => e.Priority)
                .HasConversion<int>();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // RefreshTokens
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Token)
                .IsUnique();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
//var userId = User.GetUserId();