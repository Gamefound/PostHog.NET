using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace PostHog.Model
{
    public class Properties : IReadOnlyDictionary<string, object>
    {
        [JsonIgnore]
        private readonly Dictionary<string, object> _eventProperties = new Dictionary<string, object>();

        [JsonIgnore]
        private readonly Dictionary<string, object> _userPropertiesToSet = new Dictionary<string, object>();

        [JsonIgnore]
        private readonly Dictionary<string, object> _userPropertiesToSetOnce = new Dictionary<string, object>();

        public Properties()
        {
        }

        public Properties(Dictionary<string, object> eventProperties, Dictionary<string, object>? userPropertiesToSet = null, Dictionary<string, object>? userPopertiesToSetOnce = null)
        {
            _eventProperties = eventProperties;
            if (userPropertiesToSet != null)
            {
                _userPropertiesToSet = userPropertiesToSet;
            }

            if (userPopertiesToSetOnce != null)
            {
                _userPropertiesToSetOnce = userPopertiesToSetOnce;
            }
        }

        [JsonConstructor]
        protected Properties(Dictionary<string, object> eventProperties)
        {
            _eventProperties = eventProperties;
        }

        public int Count => _eventProperties.Count;

        public IEnumerable<string> Keys => _eventProperties.Keys;

        public IEnumerable<object> Values => _eventProperties.Values;

        public object this[string key] => _eventProperties[key];

        public bool ContainsKey(string key)
        {
            return _eventProperties.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _eventProperties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Properties SetEventProperty(string key, object value)
        {
            _eventProperties[key] = value;
            return this;
        }

        public Properties SetUserPopertyOnce(string key, object value)
        {
            _userPropertiesToSetOnce[key] = value;
            _eventProperties["$set_once"] = _userPropertiesToSetOnce;

            return this;
        }

        public Properties SetUserProperty(string key, object value)
        {
            _userPropertiesToSet[key] = value;
            _eventProperties["$set"] = _userPropertiesToSet;

            return this;
        }

        public bool TryGetValue(string key, out object value)
        {
            return _eventProperties.TryGetValue(key, out value);
        }
    }
}