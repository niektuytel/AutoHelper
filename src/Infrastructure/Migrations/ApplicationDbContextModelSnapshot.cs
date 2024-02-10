﻿// <auto-generated />
using System;
using AutoHelper.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;

#nullable disable

namespace AutoHelper.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AutoHelper.Domain.Entities.Admin.RequestLogItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsSolved")
                        .HasColumnType("bit");

                    b.Property<int>("LogLevel")
                        .HasColumnType("int");

                    b.Property<string>("LogMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestTypeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RequestLogs");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Conversations.ConversationItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ConversationType")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GarageItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("GarageLookupIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("RelatedServiceIdsString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VehicleLicensePlate")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("GarageItemId");

                    b.HasIndex("GarageLookupIdentifier");

                    b.HasIndex("VehicleLicensePlate");

                    b.ToTable("Conversations");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Conversations.ConversationMessageItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConversationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MessageContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiverContactIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReceiverContactType")
                        .HasColumnType("int");

                    b.Property<string>("SenderContactIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SenderContactType")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("WhatsappMessageId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.ToTable("ConversationMessages");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GarageLookupIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GarageLookupIdentifier");

                    b.ToTable("Garages");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageLookupItem", b =>
                {
                    b.Property<string>("Identifier")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConversationContactEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConversationContactWhatsappNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DaysOfWeekString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("GarageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageThumbnail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Geometry>("Location")
                        .HasColumnType("geography");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("Rating")
                        .HasColumnType("real");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserRatingsTotal")
                        .HasColumnType("int");

                    b.Property<string>("Website")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("WhatsappNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Identifier");

                    b.ToTable("GarageLookups");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageLookupServiceItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ExpectedNextDateIsRequired")
                        .HasColumnType("bit");

                    b.Property<bool>("ExpectedNextOdometerReadingIsRequired")
                        .HasColumnType("bit");

                    b.Property<string>("GarageLookupIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("VehicleFuelType")
                        .HasColumnType("int");

                    b.Property<int>("VehicleType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GarageLookupIdentifier");

                    b.ToTable("GarageLookupServices");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageServiceItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ExpectedNextDateIsRequired")
                        .HasColumnType("bit");

                    b.Property<bool>("ExpectedNextOdometerReadingIsRequired")
                        .HasColumnType("bit");

                    b.Property<Guid>("GarageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("VehicleFuelType")
                        .HasColumnType("int");

                    b.Property<int>("VehicleType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GarageId");

                    b.ToTable("GarageServices");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Messages.NotificationItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("GeneralType")
                        .HasColumnType("int");

                    b.Property<string>("JobId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MetadataString")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("ReceiverContactIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReceiverContactType")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TriggerDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("VehicleLicensePlate")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("VehicleType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("VehicleLicensePlate");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Vehicles.VehicleLookupItem", b =>
                {
                    b.Property<string>("LicensePlate")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfAscription")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateOfMOTExpiry")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastModified")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LicensePlate");

                    b.HasIndex("LicensePlate")
                        .IsUnique();

                    b.ToTable("VehicleLookups");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Vehicles.VehicleServiceLogItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AttachedFile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpectedNextDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ExpectedNextOdometerReading")
                        .HasColumnType("int");

                    b.Property<string>("GarageLookupIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MetaData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OdometerReading")
                        .HasColumnType("int");

                    b.Property<string>("ReporterEmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReporterName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReporterPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("VehicleLicensePlate")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("GarageLookupIdentifier");

                    b.HasIndex("VehicleLicensePlate");

                    b.ToTable("VehicleServiceLogs");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Vehicles.VehicleTimelineItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExtraDataTableJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("VehicleLicensePlate")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<Guid?>("VehicleServiceLogId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VehicleLicensePlate");

                    b.HasIndex("VehicleServiceLogId");

                    b.ToTable("VehicleTimelineItems");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Conversations.ConversationItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Garages.GarageItem", null)
                        .WithMany("Conversations")
                        .HasForeignKey("GarageItemId");

                    b.HasOne("AutoHelper.Domain.Entities.Garages.GarageLookupItem", "RelatedGarage")
                        .WithMany()
                        .HasForeignKey("GarageLookupIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AutoHelper.Domain.Entities.Vehicles.VehicleLookupItem", "RelatedVehicleLookup")
                        .WithMany()
                        .HasForeignKey("VehicleLicensePlate")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelatedGarage");

                    b.Navigation("RelatedVehicleLookup");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Conversations.ConversationMessageItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Conversations.ConversationItem", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Garages.GarageLookupItem", "Lookup")
                        .WithMany()
                        .HasForeignKey("GarageLookupIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lookup");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageLookupServiceItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Garages.GarageLookupItem", "GarageLookup")
                        .WithMany("Services")
                        .HasForeignKey("GarageLookupIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GarageLookup");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageServiceItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Garages.GarageItem", "Garage")
                        .WithMany("Services")
                        .HasForeignKey("GarageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Garage");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Messages.NotificationItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Vehicles.VehicleLookupItem", "RelatedVehicleLookup")
                        .WithMany()
                        .HasForeignKey("VehicleLicensePlate")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RelatedVehicleLookup");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Vehicles.VehicleServiceLogItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Garages.GarageLookupItem", "GarageLookup")
                        .WithMany()
                        .HasForeignKey("GarageLookupIdentifier")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AutoHelper.Domain.Entities.Vehicles.VehicleLookupItem", "VehicleLookup")
                        .WithMany("ServiceLogs")
                        .HasForeignKey("VehicleLicensePlate")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("GarageLookup");

                    b.Navigation("VehicleLookup");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Vehicles.VehicleTimelineItem", b =>
                {
                    b.HasOne("AutoHelper.Domain.Entities.Vehicles.VehicleLookupItem", "VehicleLookup")
                        .WithMany("Timeline")
                        .HasForeignKey("VehicleLicensePlate")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("VehicleLookup");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Conversations.ConversationItem", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageItem", b =>
                {
                    b.Navigation("Conversations");

                    b.Navigation("Services");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Garages.GarageLookupItem", b =>
                {
                    b.Navigation("Services");
                });

            modelBuilder.Entity("AutoHelper.Domain.Entities.Vehicles.VehicleLookupItem", b =>
                {
                    b.Navigation("ServiceLogs");

                    b.Navigation("Timeline");
                });
#pragma warning restore 612, 618
        }
    }
}
