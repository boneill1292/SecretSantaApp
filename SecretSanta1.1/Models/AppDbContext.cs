﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SecretSantaApp.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<CustomUser> CustomUsers { get; set; }
        public DbSet<GroupMembership> GroupMembership { get; set; }
        public DbSet<GroupRules> GroupRules { get; set; }
        public DbSet<GroupMessages> GroupMessages { get; set; }
        public DbSet<MemberConditions> MemberConditions { get; set; }
        public DbSet<CustomUserDetails> CustomUserDetails { get; set; }
        public DbSet<GroupPairings> GroupPairings { get; set; }
    }
}