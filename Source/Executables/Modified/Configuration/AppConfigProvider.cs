using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TestingDependencyIsolation.Modified.Configuration;

namespace TestingDependencyIsolation.Modified.Driver.Configuration {
    /// <summary>
    /// Type used to load application config values.
    /// </summary>
    public sealed class AppConfigProvider : IConfigurationProvider {
        #region --Constructors--

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigProvider"/> class.
        /// </summary>
        public AppConfigProvider() {
        } // end default constructor

        #endregion

        #region --Functions--

        /// <summary>
        /// Returns a value indicating whether or not the provider allows writing to config.
        /// </summary>
        /// <returns>true if the provider can write config values; false otherwise</returns>
        public bool CanWrite() {
            return false;
        } // end function CanWrite

        /// <summary>
        /// Returns the configuration value associated with the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the value to load from configuration</param>
        /// <returns>A configuration value</returns>
        /// <exception cref="ArgumentNullException">Thrown if the given <paramref name="key"/> is null or empty</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the given <paramref name="key"/> is not found</exception>
        public object GetValue(string key) {
            if (string.IsNullOrWhiteSpace(key)) {
                throw new ArgumentNullException(nameof(key), "No key provided to load from configuration.");
            }

            return ConfigurationManager.AppSettings[key] ?? throw new KeyNotFoundException(string.Format("Unable to find the key, {0}, in the configuration.", key));
        } // end overloaded function GetValue

        /// <summary>
        /// Returns the configuration value associated with the given <paramref name="key"/> and attempts to unbox it to the given <typeparamref name="TValue"/> type.
        /// </summary>
        /// <typeparam name="TValue">The type to unbox the configuration value to</typeparam>
        /// <param name="key">The key of the value to load from configuration</param>
        /// <returns>A configuration value of the given <typeparamref name="TValue"/> type.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the given <paramref name="key"/> is null or empty</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the given <paramref name="key"/> is not found</exception>
        /// <exception cref="InvalidCastException">Thrown if the value cannot be unboxed to the given <typeparamref name="TValue"/> type.</exception>
        public TValue GetValue<TValue>(string key) {
            object value = this.GetValue(key);

            return (TValue)Convert.ChangeType(value, typeof(TValue));
        } // end overloaded function GetValue

        /// <summary>
        /// Returns a value indicating whether or not the given <paramref name="key"/> is present in the configuration.
        /// </summary>
        /// <param name="key">The key to look up</param>
        /// <returns>true if a configuration value has been defined with the given <paramref name="key"/>; false otherwise</returns>
        public bool HasKey(string key) {
            return ConfigurationManager.AppSettings.AllKeys.Contains(key);
        } // end function HasKey

        /// <summary>
        /// Returns a value indicating whether or not the configuration value stored with the <paramref name="key"/> was able to be loaded into the <paramref name="value"/> parameter.
        /// This follows the "TrySomething" pattern where exceptions will not be thrown.  This is considered the "safe" method
        /// for loading values from config when exceptions should not be thrown.
        /// </summary>
        /// <param name="key">The key of the value to load from configuration</param>
        /// <param name="value">The variable where the configuration value will be stored (if found).</param>
        /// <returns>true if the value for <paramref name="key"/> was found; false otherwise</returns>
        public bool TryGetValue(string key, out object value) {
            var returnValue = false;

            if (this.HasKey(key)) {
                value = this.GetValue(key);
                returnValue = true;
            } else {
                value = null;
            }

            return returnValue;
        } // end overloaded function TryGetValue

        /// <summary>
        /// Returns a value indicating whether or not the configuration value stored with the <paramref name="key"/> was able to be loaded into the <paramref name="value"/> parameter and unboxed to the given <typeparamref name="TValue"/> type.
        /// This follows the "TrySomething" pattern where exceptions will not be thrown.  This is considered the "safe" method
        /// for loading values from config when exceptions should not be thrown.
        /// </summary>
        /// <typeparam name="TValue">The type to unbox the configuration value to</typeparam>
        /// <param name="key">The key of the value to load from configuration</param>
        /// <param name="value">The variable where the configuration value will be stored (if found).</param>
        /// <returns>true if the value for <paramref name="key"/> was found and unboxed to the given <typeparamref name="TValue"/> type; false otherwise</returns>
        public bool TryGetValue<TValue>(string key, out TValue value) {
            var returnValue = false;
            object boxedReturnValue = null;

            if (this.TryGetValue(key, out boxedReturnValue)) {
                try {
                    value = (TValue)Convert.ChangeType(boxedReturnValue, typeof(TValue));
                    returnValue = true;
                } catch (Exception) {
                    value = default(TValue);
                }
            } else {
                value = default(TValue);
            }

            return returnValue;
        } // end overloaded function TryGetValue

        #endregion
    } // end class AppConfigProvider
} // end namespace
