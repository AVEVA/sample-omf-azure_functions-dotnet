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
        public Uri CdsUri { get; set; }

        /// <summary>
        /// CONNECT data services Tenant ID
        /// </summary>
        public string CdsTenantId { get; set; }

        /// <summary>
        /// CONNECT data services Namespace ID
        /// </summary>
        public string CdsNamespaceId { get; set; }

        /// <summary>
        /// CONNECT data services Client ID
        /// </summary>
        public string CdsClientId { get; set; }

        /// <summary>
        /// CONNECT data services Client Secret
        /// </summary>
        public string CdsClientSecret { get; set; }
        #endregion
    }
}
