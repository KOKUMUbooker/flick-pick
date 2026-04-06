using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WatchHive.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.CreateTable(
                name: "Movies",
                schema: "app",
                columns: table => new
                {
                    TmdbId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    PosterPath = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Overview = table.Column<string>(type: "text", nullable: true),
                    VoteAverage = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.TmdbId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleValue = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    EmailVerificationToken = table.Column<string>(type: "text", nullable: true),
                    EmailVerificationTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PasswordResetTokenHash = table.Column<string>(type: "text", nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "app",
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MoviePreferences",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Genre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviePreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoviePreferences_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    JwtId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedByIp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupInvitations",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    InviteeUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupInvitations_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "app",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupInvitations_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupInvitations_Users_InviteeUserId",
                        column: x => x.InviteeUserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieNightEvents",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    SelectedMovieId = table.Column<int>(type: "integer", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieNightEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieNightEvents_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "app",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieNightEvents_Movies_SelectedMovieId",
                        column: x => x.SelectedMovieId,
                        principalSchema: "app",
                        principalTable: "Movies",
                        principalColumn: "TmdbId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieNightEvents_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                schema: "app",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "app",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroups_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieNightEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 4, 5, 19, 28, 23, 852, DateTimeKind.Utc).AddTicks(4151)),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_MovieNightEvents_MovieNightEventId",
                        column: x => x.MovieNightEventId,
                        principalSchema: "app",
                        principalTable: "MovieNightEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MovieNightRatings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieNightEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieNightRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieNightRatings_MovieNightEvents_MovieNightEventId",
                        column: x => x.MovieNightEventId,
                        principalSchema: "app",
                        principalTable: "MovieNightEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieNightRatings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MovieSuggestions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    SuggestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieNightEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDisqualified = table.Column<bool>(type: "boolean", nullable: false),
                    UpvoteCount = table.Column<int>(type: "integer", nullable: false),
                    DownVoteCount = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieSuggestions_MovieNightEvents_MovieNightEventId",
                        column: x => x.MovieNightEventId,
                        principalSchema: "app",
                        principalTable: "MovieNightEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieSuggestions_Movies_MovieId",
                        column: x => x.MovieId,
                        principalSchema: "app",
                        principalTable: "Movies",
                        principalColumn: "TmdbId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieSuggestions_Users_SuggestedById",
                        column: x => x.SuggestedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieSuggestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    VoteType = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_MovieSuggestions_MovieSuggestionId",
                        column: x => x.MovieSuggestionId,
                        principalSchema: "app",
                        principalTable: "MovieSuggestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "app",
                table: "Roles",
                columns: new[] { "Id", "Created", "LastModified", "RoleValue" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 22, 704, DateTimeKind.Unspecified).AddTicks(7285), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 22, 704, DateTimeKind.Unspecified).AddTicks(7285), new TimeSpan(0, 0, 0, 0, 0)), 1 },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 22, 704, DateTimeKind.Unspecified).AddTicks(7285), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 22, 704, DateTimeKind.Unspecified).AddTicks(7285), new TimeSpan(0, 0, 0, 0, 0)), 2 }
                });

            migrationBuilder.InsertData(
                schema: "app",
                table: "Users",
                columns: new[] { "Id", "Created", "Email", "EmailVerificationToken", "EmailVerificationTokenExpiry", "EmailVerified", "FullName", "LastModified", "PasswordHash", "PasswordResetTokenExpiry", "PasswordResetTokenHash", "RoleId" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 22, 707, DateTimeKind.Unspecified).AddTicks(1470), new TimeSpan(0, 0, 0, 0, 0)), "admin@sys.com", null, null, true, "System Administrator", new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 22, 707, DateTimeKind.Unspecified).AddTicks(1470), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$ISaWeLCSlQhH1cE09p7W4.vemci26uycb0BsK9cu4DBBY/1nnqveu", null, null, new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("00000000-0000-0000-0000-000000000002"), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 23, 96, DateTimeKind.Unspecified).AddTicks(4522), new TimeSpan(0, 0, 0, 0, 0)), "john@app.com", null, null, true, "John Doe", new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 23, 96, DateTimeKind.Unspecified).AddTicks(4522), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$i8xyex20OQWLkEO.ZL8XQeqPcOyeeSJMfTv8xZywVM4YM5Wb7.PjG", null, null, new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0000-000000000003"), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 23, 96, DateTimeKind.Unspecified).AddTicks(4522), new TimeSpan(0, 0, 0, 0, 0)), "jane@app.com", null, null, true, "Jane Doe", new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 23, 96, DateTimeKind.Unspecified).AddTicks(4522), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$BJ6T6SM.WQ.i/kZNf8bIaubgOqvpB.7nc.faHjMh4FBfS/UD2kqdq", null, null, new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("00000000-0000-0000-0000-000000000004"), new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 23, 96, DateTimeKind.Unspecified).AddTicks(4522), new TimeSpan(0, 0, 0, 0, 0)), "po@app.com", null, null, true, "Dragon Warrior", new DateTimeOffset(new DateTime(2026, 4, 5, 19, 28, 23, 96, DateTimeKind.Unspecified).AddTicks(4522), new TimeSpan(0, 0, 0, 0, 0)), "$2a$11$6dWajxkeo6RMZAl8/CYrouL7dRjnlU5xj/pJV2AGgaJHBae8x2QJi", null, null, new Guid("00000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_MovieNightEventId",
                schema: "app",
                table: "ChatMessages",
                column: "MovieNightEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                schema: "app",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvitations_CreatedById",
                schema: "app",
                table: "GroupInvitations",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvitations_GroupId",
                schema: "app",
                table: "GroupInvitations",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvitations_InviteeUserId",
                schema: "app",
                table: "GroupInvitations",
                column: "InviteeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatedById",
                schema: "app",
                table: "Groups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name_CreatedById",
                schema: "app",
                table: "Groups",
                columns: new[] { "Name", "CreatedById" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieNightEvents_CreatedById",
                schema: "app",
                table: "MovieNightEvents",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MovieNightEvents_GroupId",
                schema: "app",
                table: "MovieNightEvents",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieNightEvents_SelectedMovieId",
                schema: "app",
                table: "MovieNightEvents",
                column: "SelectedMovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieNightRatings_MovieNightEventId",
                schema: "app",
                table: "MovieNightRatings",
                column: "MovieNightEventId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieNightRatings_UserId_MovieNightEventId",
                schema: "app",
                table: "MovieNightRatings",
                columns: new[] { "UserId", "MovieNightEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MoviePreferences_UserId",
                schema: "app",
                table: "MoviePreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSuggestions_MovieId",
                schema: "app",
                table: "MovieSuggestions",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieSuggestions_MovieNightEventId_MovieId",
                schema: "app",
                table: "MovieSuggestions",
                columns: new[] { "MovieNightEventId", "MovieId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieSuggestions_SuggestedById",
                schema: "app",
                table: "MovieSuggestions",
                column: "SuggestedById");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ClientId",
                schema: "app",
                table: "RefreshTokens",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Expires",
                schema: "app",
                table: "RefreshTokens",
                column: "Expires");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_IsRevoked",
                schema: "app",
                table: "RefreshTokens",
                column: "IsRevoked");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                schema: "app",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                schema: "app",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_IsRevoked_Expires",
                schema: "app",
                table: "RefreshTokens",
                columns: new[] { "UserId", "IsRevoked", "Expires" });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                schema: "app",
                table: "UserGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId_GroupId",
                schema: "app",
                table: "UserGroups",
                columns: new[] { "UserId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "app",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "app",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_MovieSuggestionId",
                schema: "app",
                table: "Votes",
                column: "MovieSuggestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_MovieSuggestionId",
                schema: "app",
                table: "Votes",
                columns: new[] { "UserId", "MovieSuggestionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages",
                schema: "app");

            migrationBuilder.DropTable(
                name: "GroupInvitations",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MovieNightRatings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MoviePreferences",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserGroups",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Votes",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MovieSuggestions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "MovieNightEvents",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Movies",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "app");
        }
    }
}
