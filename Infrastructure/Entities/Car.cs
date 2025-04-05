﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rent.Infrastructure.Entities
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string PlateNumber { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsAvailable { get; set; }
        public  bool IsDeleted { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
    }
}
