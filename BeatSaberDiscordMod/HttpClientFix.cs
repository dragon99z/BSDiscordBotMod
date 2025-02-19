using System;
using System.Net.Http;
using System.Reflection;

public static class HttpClientFix
{
    public static void ApplyFix()
    {
        try
        {
            Type handlerType = typeof(HttpClientHandler);
            PropertyInfo proxyProperty = handlerType.GetProperty("Proxy");

            if (proxyProperty != null)
            {
                Console.WriteLine("Patching HttpClientHandler.Proxy to prevent Mono crash...");
                proxyProperty.SetValue(new HttpClientHandler(), null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to patch HttpClientHandler.Proxy: {ex.Message}");
        }
    }
}
