using System.Collections.Generic;
using MockServer.Models;

namespace MockServer.Factories
{
    /// <summary>
    /// Used for creating/populating FrameworkModels.
    /// </summary>
    public interface IFrameworkModelFactory
    {
        IList<FrameworkModel> PrepareFrameworkModels();
    }
}