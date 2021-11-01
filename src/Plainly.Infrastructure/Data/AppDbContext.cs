using Plainly.Domain;
using Plainly.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Plainly.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<
        User, Role, string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>
    >
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(
            DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor = null
        ) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            BuildIdentityTables(modelBuilder);
        }

        private void BuildIdentityTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.Property(rc => rc.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasColumnName("id")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property(rc => rc.ClaimType)
                    .HasColumnType("text")
                    .HasColumnName("claim_type");

                b.Property(rc => rc.ClaimValue)
                    .HasColumnType("text")
                    .HasColumnName("claim_value");

                b.Property(rc => rc.RoleId)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("role_id");

                b.HasKey(rc => rc.Id)
                    .HasName("pk_role_claim");

                b.HasIndex(rc => rc.RoleId)
                    .HasDatabaseName("ix_role_claim_role_id");

                b.ToTable("role_claim");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.Property(uc => uc.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasColumnName("id")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property(uc => uc.ClaimType)
                    .HasColumnType("text")
                    .HasColumnName("claim_type");

                b.Property(uc => uc.ClaimValue)
                    .HasColumnType("text")
                    .HasColumnName("claim_value");

                b.Property(uc => uc.UserId)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("user_id");

                b.HasKey(uc => uc.Id)
                    .HasName("pk_user_claim");

                b.HasIndex(uc => uc.UserId)
                    .HasDatabaseName("ix_user_claim_user_id");

                b.ToTable("user_claim");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property(ul => ul.LoginProvider)
                    .HasColumnType("text")
                    .HasColumnName("login_provider");

                b.Property(ul => ul.ProviderKey)
                    .HasColumnType("text")
                    .HasColumnName("provider_key");

                b.Property(ul => ul.ProviderDisplayName)
                    .HasColumnType("text")
                    .HasColumnName("provider_display_name");

                b.Property(ul => ul.UserId)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("user_id");

                b.HasKey(ul =>
                    new {
                        ul.LoginProvider,
                        ul.ProviderKey
                    })
                    .HasName("pk_user_login");

                b.HasIndex(ul => ul.UserId)
                    .HasDatabaseName("ix_user_logins_user_id");

                b.ToTable("user_login");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property(ut => ut.UserId)
                    .HasColumnType("text")
                    .HasColumnName("user_id");

                b.Property(ut => ut.LoginProvider)
                    .HasColumnType("text")
                    .HasColumnName("login_provider");

                b.Property(ut => ut.Name)
                    .HasColumnType("text")
                    .HasColumnName("name");

                b.Property(ut => ut.Value)
                    .HasColumnType("text")
                    .HasColumnName("value");

                b.HasKey(ut => new
                    {
                        ut.UserId, 
                        ut.LoginProvider, 
                        ut.Name,
                    })
                    .HasName("pk_user_token");

                b.ToTable("user_token");
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.Property(r => r.Id)
                    .HasColumnType("text")
                    .HasColumnName("id");

                b.Property(r => r.ConcurrencyStamp)
                    .IsConcurrencyToken()
                    .HasColumnType("text")
                    .HasColumnName("concurrency_stamp");

                b.Property(r => r.Name)
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("name");

                b.Property(r => r.NormalizedName)
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("normalized_name");

                b.HasKey(r => r.Id)
                    .HasName("pk_role");

                b.HasIndex(r => r.NormalizedName)
                    .IsUnique()
                    .HasDatabaseName("ix_role_name");

                b.ToTable("role");
            });

            modelBuilder.Entity<User>(b =>
            {
                b.Property(u => u.Id)
                    .HasColumnType("text")
                    .HasColumnName("id");

                b.Property(u => u.AccessFailedCount)
                    .HasColumnType("integer")
                    .HasColumnName("access_failed_count");

                b.Property(u => u.Activated)
                    .HasColumnType("boolean")
                    .HasColumnName("activated");

                b.Property(u => u.ActivationKey)
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)")
                    .HasColumnName("activation_key");

                b.Property(u => u.ConcurrencyStamp)
                    .IsConcurrencyToken()
                    .HasColumnType("text")
                    .HasColumnName("concurrency_stamp");

                b.Property(u => u.CreatedBy)
                    .HasColumnType("text")
                    .HasColumnName("created_by");

                b.Property(u => u.CreatedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_date");

                b.Property(u => u.Email)
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("email");

                b.Property(u => u.EmailConfirmed)
                    .HasColumnType("boolean")
                    .HasColumnName("email_confirmed");

                b.Property(u => u.FirstName)
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("first_name");

                b.Property(u => u.ImageUrl)
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("image_url");

                b.Property(u => u.LangKey)
                    .HasMaxLength(6)
                    .HasColumnType("character varying(6)")
                    .HasColumnName("lang_key");

                b.Property(u => u.LastModifiedBy)
                    .HasColumnType("text")
                    .HasColumnName("last_modified_by");

                b.Property(u => u.LastModifiedDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("last_modified_date");

                b.Property(u => u.LastName)
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("last_name");

                b.Property(u => u.LockoutEnabled)
                    .HasColumnType("boolean")
                    .HasColumnName("lockout_enabled");

                b.Property(u => u.LockoutEnd)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("lockout_end");

                b.Property(u => u.Login)
                    .HasColumnType("text")
                    .HasColumnName("login");

                b.Property(u => u.NormalizedEmail)
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("normalized_email");

                b.Property(u => u.NormalizedUserName)
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("normalized_user_name");

                b.Property(u => u.PasswordHash)
                    .HasColumnType("text")
                    .HasColumnName("password_hash");

                b.Property(u => u.PhoneNumber)
                    .HasColumnType("text")
                    .HasColumnName("phone_number");

                b.Property(u => u.PhoneNumberConfirmed)
                    .HasColumnType("boolean")
                    .HasColumnName("phone_number_confirmed");

                b.Property(u => u.ResetDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("reset_date");

                b.Property(u => u.ResetKey)
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)")
                    .HasColumnName("reset_key");

                b.Property(u => u.SecurityStamp)
                    .HasColumnType("text")
                    .HasColumnName("security_stamp");

                b.Property(u => u.TwoFactorEnabled)
                    .HasColumnType("boolean")
                    .HasColumnName("two_factor_enabled");

                b.Property(u => u.UserName)
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("user_name");

                b.HasKey(u => u.Id)
                    .HasName("pk_user");

                b.HasIndex(u => u.NormalizedEmail)
                    .HasDatabaseName("ix_email");

                b.HasIndex(u => u.NormalizedUserName)
                    .IsUnique()
                    .HasDatabaseName("ix_username");

                b.ToTable("user");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(rc => rc.RoleId)
                    .HasConstraintName("fk_role_claim_role_role_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(uc => uc.UserId)
                    .HasConstraintName("fk_user_claim_user_user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ul => ul.UserId)
                    .HasConstraintName("fk_user_logins_user_user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.HasOne<User>()               
                    .WithMany()
                    .HasForeignKey(ut => ut.UserId)
                    .HasConstraintName("fk_user_tokens_user_user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<User>(b =>
                b.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<UserRole>(
                    j => j.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .HasConstraintName("fk_user_role_role_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired(),
                    j => j.HasOne(ur => ur.User)
                        .WithMany(u => u.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .HasConstraintName("fk_user_role_user_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired(),
                    j =>
                    {
                        j.Property(ur => ur.UserId)
                            .HasColumnType("text")
                            .HasColumnName("user_id");
            
                        j.Property(ur => ur.RoleId)
                            .HasColumnType("text")
                            .HasColumnName("role_id");
            
                        j.HasKey(ur => new { ur.UserId, ur.RoleId, })
                            .HasName("pk_user_role");
            
                        j.HasIndex(ur => ur.RoleId)
                            .HasDatabaseName("ix_user_role_role_id");
            
                        j.HasIndex(ur => ur.UserId)
                            .HasDatabaseName("ix_user_role_user_id");

                        j.Navigation(ur => ur.Role);

                        j.Navigation(ur => ur.User);
                        
                        j.ToTable("user_role");
                        
                    })
                );

            modelBuilder.Entity<Role>(b =>
            {
                b.Navigation(r => r.UserRoles);
            });
            
            modelBuilder.Entity<User>(b =>
            {
                b.Navigation(u => u.UserRoles);
            });
        }
        
        /// <summary>
        /// SaveChangesAsync with entities audit
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditedEntityBase && 
                            e.State is EntityState.Added or EntityState.Modified);

            string modifiedOrCreatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            foreach (var entityEntry in entries)
            {
                IAuditedEntityBase entity = (IAuditedEntityBase)entityEntry.Entity;
                
                if (entityEntry.State == EntityState.Added)
                {
                    entity.CreatedDate = DateTime.Now;
                    entity.CreatedBy = modifiedOrCreatedBy;
                }
                else
                {
                    Entry(entity).Property(p => p.CreatedDate).IsModified = false;
                    Entry(entity).Property(p => p.CreatedBy).IsModified = false;
                }

                entity.LastModifiedDate = DateTime.Now;
                entity.LastModifiedBy = modifiedOrCreatedBy;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
