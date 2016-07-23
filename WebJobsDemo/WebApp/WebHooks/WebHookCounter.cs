namespace WebApp.WebHooks
{
    public static class WebHookCounter
    {
        private static readonly object _lock = new object();

        public static int Count { get; private set; }

        public static void AddToCount()
        {
            lock (_lock)
            {
                Count = Count + 1;
            }
        }
    }
}