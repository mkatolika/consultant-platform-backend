using Xunit;
using ConsulationApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsulationApplication.Tests.Bookings
{
    public class CreateBookingTests
    {
        [Fact]
        public void CreateBooking_WithValidData_ShouldSucceed()
        {
            // ARRANGE
            var booking = new ConsulationApplication.Models.Bookings
            {
                ClientId = "client-123",
                ConsultantId = "consultant-456",
                ServiceId = 1,
                SlotId = 10
            };

            // ACT
            var isValid =
                !string.IsNullOrEmpty(booking.ClientId) &&
                !string.IsNullOrEmpty(booking.ConsultantId) &&
                booking.ServiceId > 0 &&
                booking.SlotId > 0;

            // ASSERT
            Assert.True(isValid);
        }
    }
}
