using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ConsultationApplication.Controllers;
using ConsultationApplication.Data;
using ConsultationApplication.DTOs;
using ConsultationApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultationApplication.Tests.Bookings;

public class CreateBookingTests
{
    [Fact]
    public async Task CreateBooking_UsesAuthenticatedUserAsClient()
    {
        await using var context = CreateContext();
        await SeedBookableSlot(context);
        var controller = CreateController(context, "authenticated-client");

        var result = await controller.CreateBooking(new BookingDto
        {
            ConsultantId = "consultant-1",
            ServiceId = 1,
            SlotId = 1
        });

        var created = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        var booking = await context.Bookings.SingleAsync();
        Assert.Equal("authenticated-client", booking.ClientId);
        Assert.False((await context.Slots.FindAsync(1))!.IsAvailable);
    }

    [Fact]
    public async Task CreateBooking_RejectsSlotOwnedByAnotherConsultant()
    {
        await using var context = CreateContext();
        await SeedBookableSlot(context);
        var controller = CreateController(context, "authenticated-client");

        var result = await controller.CreateBooking(new BookingDto
        {
            ConsultantId = "different-consultant",
            ServiceId = 1,
            SlotId = 1
        });

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Empty(context.Bookings);
    }

    [Fact]
    public void RegisterDto_RequiresUsernamePasswordAndFullName()
    {
        var errors = Validate(new RegisterDto());

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(RegisterDto.Username)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(RegisterDto.Password)));
        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(RegisterDto.FullName)));
    }

    [Fact]
    public void CreateSlotDto_RejectsEndBeforeStart()
    {
        var start = DateTime.UtcNow.AddDays(1);
        var errors = Validate(new CreateSlotDto { StartTime = start, EndTime = start.AddMinutes(-30) });

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(CreateSlotDto.EndTime)));
    }

    private static ConsultationAppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ConsultationAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ConsultationAppDbContext(options);
    }

    private static BookingController CreateController(ConsultationAppDbContext context, string userId)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, "User")
        ], "TestAuthentication");

        return new BookingController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            }
        };
    }

    private static async Task SeedBookableSlot(ConsultationAppDbContext context)
    {
        var consultantUser = new AppUser { Id = "consultant-1", UserName = "consultant", FullName = "Test Consultant" };
        var client = new AppUser { Id = "authenticated-client", UserName = "client", FullName = "Test Client" };
        var department = new Department { Id = 1, Name = "Technology" };
        var service = new Services { Id = 1, Name = "Code Review", Description = "Professional code review", Price = 500, Department = department };
        var consultant = new Consultant
        {
            Id = 1,
            UserId = consultantUser.Id,
            User = consultantUser,
            Specialization = ".NET",
            Qualification = "Microsoft certification",
            LicenseNumber = "LIC-001",
            IsApproved = true
        };

        context.AddRange(consultantUser, client, department, service, consultant);
        context.ConsultantServices.Add(new ConsultantService { Consultant = consultant, Service = service });
        context.Slots.Add(new Slot
        {
            Id = 1,
            ConsultantId = consultantUser.Id,
            Consultant = consultantUser,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        });
        await context.SaveChangesAsync();
    }

    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }
}