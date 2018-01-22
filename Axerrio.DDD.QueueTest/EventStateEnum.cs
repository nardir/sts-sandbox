using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.DDD.BuildingBlocks
{
    public enum EventStateEnum
    {
        NotPublished = 0,
        Publishing = 1,
        Published = 2,
        PublishedFailed = 99
    }
}
