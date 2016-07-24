using System;
using System.Collections.Generic;

namespace KidsPrize.Bus
{
    public class Command
    {
        private IDictionary<string, object> _headers = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public void SetHeader<T>(string key, T value)
        {
            _headers[key] = value;
        }

        public T GetHeader<T>(string key)
        {
            if (_headers.ContainsKey(key) && _headers[key] is T)
            {
                return (T)_headers[key];
            }
            return default(T);
        }
    }
}