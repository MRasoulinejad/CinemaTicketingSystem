﻿using CinemaTicketingSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Common.Interfaces
{
    public interface ISeatRepository : IRepository<Seat>
    {
        void Update(Seat entity);
    }
}
