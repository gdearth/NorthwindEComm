using System.Diagnostics.Metrics;

namespace NorthWindsEComm.CrudHelper.Cache;

public class CacheHitMetrics
{
    private readonly Counter<int> _hitItemsCount;
    private readonly Counter<int> _missItemsCount;
    private readonly Counter<int> _hitMethodCount;
    private readonly Counter<int> _missMethodCount;
    
    public CacheHitMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Cache.Store");
        _hitItemsCount = meter.CreateCounter<int>("cache.store.hit_item_count");
        _missItemsCount = meter.CreateCounter<int>("cache.store.miss_item_count");
        _hitMethodCount = meter.CreateCounter<int>("cache.store.hit_method_count");
        _missMethodCount = meter.CreateCounter<int>("cache.store.miss_method_count");
    }

    public void Hit(int quantity = 1)
    {
        _hitMethodCount.Add(1);
        _hitItemsCount.Add(quantity);
    }

    public void Miss(int quantity = 1)
    {
        _missMethodCount.Add(1);
        _missItemsCount.Add(quantity);
    }
}