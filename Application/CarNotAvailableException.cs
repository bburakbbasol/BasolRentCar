using System;

namespace Rent.Application.Exceptions
{
    public class CarNotAvailableException : Exception
    {
        public CarNotAvailableException() : base("The car is not available for the selected dates.")
        {
        }
    }
}
