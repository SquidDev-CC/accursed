using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Accursed.Models;

namespace Accursed.Migrations
{
    [DbContext(typeof(AccursedDbContext))]
    partial class AccursedDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("Accursed.Models.File", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("DownloadId");

                    b.Property<string>("Name");

                    b.Property<string>("NormalisedName");

                    b.Property<int>("VersionId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Accursed.Models.Mod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Slug");

                    b.Property<DateTime>("VersionRefresh");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Accursed.Models.ModVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<uint>("DownloadId");

                    b.Property<string>("FancyName");

                    b.Property<string>("MCVersion");

                    b.Property<int>("ModId");

                    b.Property<string>("Name");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("Accursed.Models.File", b =>
                {
                    b.HasOne("Accursed.Models.ModVersion")
                        .WithMany()
                        .HasForeignKey("VersionId");
                });

            modelBuilder.Entity("Accursed.Models.ModVersion", b =>
                {
                    b.HasOne("Accursed.Models.Mod")
                        .WithMany()
                        .HasForeignKey("ModId");
                });
        }
    }
}
