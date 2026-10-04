namespace OrderSystem.Application
{
    public static class OrderCacheKeys
    {
        public static string ForOrder(Guid id) => $"order:{id}";
    }
}
