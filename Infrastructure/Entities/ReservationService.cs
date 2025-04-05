using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Infrastructure.Entities
{
    public class ReservationService
    {
        public Guid ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
