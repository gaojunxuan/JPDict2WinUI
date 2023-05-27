using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPDict2.Helpers;

/// <summary>
/// This helper class can be used to create, store, retrieve and
/// remove settings in IsolatedStorageSettings.
/// </summary>
public class StorageHelper
{
    /// <summary>
    /// Stores the given key value pair.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="overwrite">If true, will overwrite the existing value.</param>
    /// <returns>True if success, false otherwise.</returns>
    public static bool StoreSetting(string key, object value, bool overwrite)
    {
        if (overwrite || !Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = value;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Retrieves a setting matching the given key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns>The value for the key or a default value if key is not found.</returns>
    public static T GetSetting<T>(string key)
    {
        if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
        {
            return (T)Windows.Storage.ApplicationData.Current.LocalSettings.Values[key];
        }
        return default(T);
    }

    /// <summary>
    /// Retrieves a setting matching the given key.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns>The value for the key or the default value if key is not found.</returns>
    public static T GetSetting<T>(string key, T defaultVal)
    {
        if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
        {
            return (T)Windows.Storage.ApplicationData.Current.LocalSettings.Values[key];
        }
        return defaultVal;
    }

    /// <summary>
    /// Removes a setting from IsolatedStorageSettings.
    /// </summary>
    /// <param name="key"></param>
    public static void RemoveSetting(string key)
    {
        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(key);
    }

    internal static void FlushToStorage()
    {

    }
}
