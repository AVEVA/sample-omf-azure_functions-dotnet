using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeather
{
    public class AppSettings
    {
        #region OpenWeather API

        /// <summary>
        /// OpenWeather API Endpoint URI
        /// </summary>
        public Uri OpenWeatherUri { get; set; }

        /// <summary>
        /// OpenWeather API Key
        /// </summary>
        public string OpenWeatherKey { get; set; }

        /// <summary>
        /// OpenWeather API Queries, | separated
        /// </summary>
        public string OpenWeatherQueries { get; set; }
        #endregion

        #region CONNECT data services

        /// <summary>
        /// CONNECT data services OMF Endpoint URI
        /// </summary>
        public Uri Resource { get; set; }

        /// <summary>
        /// CONNECT data services Tenant ID
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// CONNECT data services Namespace ID
        /// </summary>
        public string NamespaceId { get; set; }

        /// <summary>
        /// CONNECT data services Client ID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// CONNECT data services Client Secret
        /// </summary>
        public string ClientSecret { get; set; }
        #endregion
    }
}
