using System.Net;

namespace MyroWebClient.Extensions
{
    public static class CookieContainerExtensions
    {
        public static CookieContainer AddCookiesFromHeader(this CookieContainer container, string[] cookies, string host)
        {
            container = container ?? new CookieContainer();
            if (cookies is null) return container;

            for (int i = 0; i < cookies.Length; i++)
            {
                // separate cookies from each other (format: cookie1=value1; subCookie=value; ...)
                var separatedCookies = cookies[i].Split(';');
                // separate name and value (format: cookie=value)
                var separatedCookie = separatedCookies[0].Split('=');
            
                container.Add(new Cookie(separatedCookie[0], separatedCookie[1], "", host));
            }

            return container;
        }
    }
}
