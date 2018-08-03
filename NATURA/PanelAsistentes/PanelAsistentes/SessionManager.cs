using System.Web;

namespace PanelAsistentes
{
    public class SessionManager
    {
        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        public static void Set<T>(string key, T data)
        {
            HttpContext context = HttpContext.Current;
            context.Session[key] = data;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            HttpContext context = HttpContext.Current;
            return (T)context.Session[key];
        }

        /// <summary>
        /// Delete the specified key.
        /// </summary>        
        /// <param name="key">The key.</param>        
        public static void Del(string key)
        {
            HttpContext context = HttpContext.Current;
            context.Session.Remove(key);
        }

        /// <summary>
        /// Delete all keys.
        /// </summary>                
        public static void DelAll()
        {
            HttpContext context = HttpContext.Current;
            context.Session.RemoveAll();
        }
    }
}