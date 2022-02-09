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

        #region AVEVA Data Hub

        /// <summary>
        /// AVEVA Data Hub OMF Endpoint URI
        /// </summary>
        public Uri AdhUri { get; set; }

        /// <summary>
        /// AVEVA Data Hub Tenant ID
        /// </summary>
        public string AdhTenantId { get; set; }

        /// <summary>
        /// AVEVA Data Hub Namespace ID
        /// </summary>
        public string AdhNamespaceId { get; set; }

        /// <summary>
        /// AVEVA Data Hub Client ID
        /// </summary>
        public string AdhClientId { get; set; }

        /// <summary>
        /// AVEVA Data Hub Client Secret
        /// </summary>
        public string AdhClientSecret { get; set; }
        #endregion
    }
}
