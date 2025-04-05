using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Application.Interfaces
{
    namespace Rent.Application.Interfaces
    {
        public interface ICarAvailabilityChecker
        {
            Task<bool> IsCarAvailable(Guid carId, DateTime startDate, DateTime endDate);
        }
    }

}
