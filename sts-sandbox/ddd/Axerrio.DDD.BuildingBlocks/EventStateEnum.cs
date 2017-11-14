using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public enum EventStateEnum
    {
        NotPublished = 0,
        Published = 1,
        PublishedFailed = 2
    }
}
