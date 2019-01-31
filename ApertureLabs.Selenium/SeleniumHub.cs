using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ApertureLabs.Selenium
{
    public interface ISeleniumSession
    { }

    /// <summary>
    /// SeleniumHub.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class SeleniumHub : IDisposable
    {
        #region Fields

        private readonly IList<SeleniumNode> registeredNodes;
        private readonly JsonSerializerSettings jsonSerializerSettings;
        private readonly Process hubProcess;
        private readonly SeleniumHubOptions seleniumHubOptions;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumHub"/> class.
        /// </summary>
        public SeleniumHub()
            : this(SeleniumHubOptions.Default())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumHub"/> class.
        /// </summary>
        /// <param name="seleniumHubOptions">The selenium hub options.</param>
        public SeleniumHub(SeleniumHubOptions seleniumHubOptions)
        {
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ObjectCreationHandling = ObjectCreationHandling.Auto
            };

            // Default url.
            Uri = new Uri("127.0.0.1:4444/wd/hub");

            // Check if local hub is already running.
            if (IsDefaultPortBusy())
            {
                // Check if status 
            }
            else
            {
                // If no local hub create one.
            }
        }

        #endregion

        #region Properties

        public Uri Uri { get; private set; }

        #endregion

        #region Methods

        public void RegisterNode(SeleniumNode seleniumNode)
        {
            throw new NotImplementedException();
        }

        public void UnRegisterNode(SeleniumNode seleniumNode,
            bool stopNodeProcess = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ISeleniumSession> GetSessions()
        {
            throw new NotImplementedException();
        }

        public IHubStatus GetStatus()
        {
            var status = default(HubStatus);
            var uri = new Uri(Uri, "status");

            using (var client = new WebClient())
            {
                var json = client.DownloadString(uri);
                status = JsonConvert.DeserializeObject<HubStatus>(
                    json,
                    jsonSerializerSettings);
            }

            return status;
        }

        public void ShutDown()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        private bool IsDefaultPortBusy()
        {
            using (var tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect("127.0.0.1:4444", 4444);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private int GetFreePort()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SeleniumHub() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
