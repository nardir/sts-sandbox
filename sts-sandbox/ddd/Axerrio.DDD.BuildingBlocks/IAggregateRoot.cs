﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public interface IAggregateRoot<T>
        where T: Entity<>
    {
    }
}
