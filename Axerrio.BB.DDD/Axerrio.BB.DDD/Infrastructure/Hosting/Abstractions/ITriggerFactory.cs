using Quartz;

namespace Axerrio.BB.DDD.Infrastructure.Hosting.Abstractions
{
    public interface ITriggerFactory
    {
        ITrigger Create();
    }
}
