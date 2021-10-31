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

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasColumnName("id")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("text")
                    .HasColumnName("claim_type");

                b.Property<string>("ClaimValue")
                    .HasColumnType("text")
                    .HasColumnName("claim_value");

                b.Property<string>("RoleId")
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("role_id");

                b.HasKey("Id")
                    .HasName("pk_role_claims");

                b.HasIndex("RoleId")
                    .HasDatabaseName("ix_role_claims_role_id");

                b.ToTable("role_claims");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer")
                    .HasColumnName("id")
                    .HasAnnotation("Npgsql:ValueGenerationStrategy",
                        NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("text")
                    .HasColumnName("claim_type");

                b.Property<string>("ClaimValue")
                    .HasColumnType("text")
                    .HasColumnName("claim_value");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("user_id");

                b.HasKey("Id")
                    .HasName("pk_user_claims");

                b.HasIndex("UserId")
                    .HasDatabaseName("ix_user_claims_user_id");

                b.ToTable("user_claims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.Property<string>("LoginProvider")
                    .HasColumnType("text")
                    .HasColumnName("login_provider");

                b.Property<string>("ProviderKey")
                    .HasColumnType("text")
                    .HasColumnName("provider_key");

                b.Property<string>("ProviderDisplayName")
                    .HasColumnType("text")
                    .HasColumnName("provider_display_name");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("user_id");

                b.HasKey("LoginProvider", "ProviderKey")
                    .HasName("pk_user_logins");

                b.HasIndex("UserId")
                    .HasDatabaseName("ix_user_logins_user_id");

                b.ToTable("user_logins");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("text")
                    .HasColumnName("user_id");

                b.Property<string>("LoginProvider")
                    .HasColumnType("text")
                    .HasColumnName("login_provider");

                b.Property<string>("Name")
                    .HasColumnType("text")
                    .HasColumnName("name");

                b.Property<string>("Value")
                    .HasColumnType("text")
                    .HasColumnName("value");

                b.HasKey("UserId", "LoginProvider", "Name")
                    .HasName("pk_user_tokens");

                b.ToTable("user_tokens");
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("text")
                    .HasColumnName("id");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("text")
                    .HasColumnName("concurrency_stamp");

                b.Property<string>("Name")
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("name");

                b.Property<string>("NormalizedName")
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("normalized_name");

                b.HasKey("Id")
                    .HasName("pk_roles");

                b.HasIndex("NormalizedName")
                    .IsUnique()
                    .HasDatabaseName("ix_role_name");

                b.ToTable("roles");
            });

            modelBuilder.Entity<User>(b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("text")
                    .HasColumnName("id");

                b.Property<int>("AccessFailedCount")
                    .HasColumnType("integer")
                    .HasColumnName("access_failed_count");

                b.Property<bool>("Activated")
                    .HasColumnType("boolean")
                    .HasColumnName("activated");

                b.Property<string>("ActivationKey")
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)")
                    .HasColumnName("activation_key");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("text")
                    .HasColumnName("concurrency_stamp");

                b.Property<string>("CreatedBy")
                    .HasColumnType("text")
                    .HasColumnName("created_by");

                b.Property<DateTime>("CreatedDate")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_date");

                b.Property<string>("Email")
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("email");

                b.Property<bool>("EmailConfirmed")
                    .HasColumnType("boolean")
                    .HasColumnName("email_confirmed");

                b.Property<string>("FirstName")
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("first_name");

                b.Property<string>("ImageUrl")
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("image_url");

                b.Property<string>("LangKey")
                    .HasMaxLength(6)
                    .HasColumnType("character varying(6)")
                    .HasColumnName("lang_key");

                b.Property<string>("LastModifiedBy")
                    .HasColumnType("text")
                    .HasColumnName("last_modified_by");

                b.Property<DateTime>("LastModifiedDate")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("last_modified_date");

                b.Property<string>("LastName")
                    .HasMaxLength(50)
                    .HasColumnType("character varying(50)")
                    .HasColumnName("last_name");

                b.Property<bool>("LockoutEnabled")
                    .HasColumnType("boolean")
                    .HasColumnName("lockout_enabled");

                b.Property<DateTimeOffset?>("LockoutEnd")
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("lockout_end");

                b.Property<string>("Login")
                    .HasColumnType("text")
                    .HasColumnName("login");

                b.Property<string>("NormalizedEmail")
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("normalized_email");

                b.Property<string>("NormalizedUserName")
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("normalized_user_name");

                b.Property<string>("PasswordHash")
                    .HasColumnType("text")
                    .HasColumnName("password_hash");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("text")
                    .HasColumnName("phone_number");

                b.Property<bool>("PhoneNumberConfirmed")
                    .HasColumnType("boolean")
                    .HasColumnName("phone_number_confirmed");

                b.Property<DateTime?>("ResetDate")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("reset_date");

                b.Property<string>("ResetKey")
                    .HasMaxLength(20)
                    .HasColumnType("character varying(20)")
                    .HasColumnName("reset_key");

                b.Property<string>("SecurityStamp")
                    .HasColumnType("text")
                    .HasColumnName("security_stamp");

                b.Property<bool>("TwoFactorEnabled")
                    .HasColumnType("boolean")
                    .HasColumnName("two_factor_enabled");

                b.Property<string>("UserName")
                    .HasMaxLength(256)
                    .HasColumnType("character varying(256)")
                    .HasColumnName("user_name");

                b.HasKey("Id")
                    .HasName("pk_users");

                b.HasIndex("NormalizedEmail")
                    .HasDatabaseName("ix_email");

                b.HasIndex("NormalizedUserName")
                    .IsUnique()
                    .HasDatabaseName("ix_username");

                b.ToTable("users");
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("text")
                    .HasColumnName("user_id");

                b.Property<string>("RoleId")
                    .HasColumnType("text")
                    .HasColumnName("role_id");

                b.Property<string>("UserId1")
                    .HasColumnType("text")
                    .HasColumnName("user_id1");

                b.HasKey("UserId", "RoleId")
                    .HasName("pk_user_roles");

                b.HasIndex("RoleId")
                    .HasDatabaseName("ix_user_roles_role_id");

                b.HasIndex("UserId")
                    .HasDatabaseName("ix_user_roles_user_id");

                b.ToTable("user_roles");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .HasConstraintName("fk_role_claims_roles_role_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasConstraintName("fk_user_claims_users_user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasConstraintName("fk_user_logins_users_user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.HasOne<User>()               
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasConstraintName("fk_user_tokens_users_user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasOne<Role>()
                    .WithMany("UserRoles")
                    .HasForeignKey("RoleId")
                    .HasConstraintName("fk_user_roles_roles_role_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne<User>()
                    .WithMany("UserRoles")
                    .HasForeignKey("UserId")
                    .HasConstraintName("fk_user_roles_users_user_id")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne<User>("User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .HasConstraintName("fk_user_roles_users_user_id");

                b.Navigation("Role");

                b.Navigation("User");
            });

            modelBuilder.Entity<Role>(b => { b.Navigation("UserRoles"); });

            modelBuilder.Entity<User>(b => { b.Navigation("UserRoles"); });
        }

        /// <summary>
        /// SaveChangesAsync with entities audit
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default(CancellationToken)
        )
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is IAuditedEntityBase && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            string modifiedOrCreatedBy = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    ((IAuditedEntityBase)entityEntry.Entity).CreatedDate = DateTime.Now;
                    ((IAuditedEntityBase)entityEntry.Entity).CreatedBy = modifiedOrCreatedBy;
                }
                else
                {
                    Entry((IAuditedEntityBase)entityEntry.Entity).Property(p => p.CreatedDate).IsModified = false;
                    Entry((IAuditedEntityBase)entityEntry.Entity).Property(p => p.CreatedBy).IsModified = false;
                }

                ((IAuditedEntityBase)entityEntry.Entity).LastModifiedDate = DateTime.Now;
                ((IAuditedEntityBase)entityEntry.Entity).LastModifiedBy = modifiedOrCreatedBy;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
