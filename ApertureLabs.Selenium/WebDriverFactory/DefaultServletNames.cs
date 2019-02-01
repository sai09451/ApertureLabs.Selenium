
/// <summary>
/// Contains the default servlet names. Meant to be used with the
/// <c>SeleniumHubOptions.Servlets</c>.
/// </summary>
public static class DefaultServletNames
{
    /// <summary>
    /// Gets the life cycle servlet.
    /// </summary>
    /// <value>
    /// The life cycle servlet.
    /// </value>
    public static string LifeCycleServlet => "org.openqa.grid.web.servlet.LifecycleServlet";

    /// <summary>
    /// Gets the resource servlet.
    /// </summary>
    /// <value>
    /// The resource servlet.
    /// </value>
    public static string ResourceServlet => "org.openqa.grid.web.servlet.ResourceServlet";

    /// <summary>
    /// Gets the console servlet.
    /// </summary>
    /// <value>
    /// The console servlet.
    /// </value>
    public static string ConsoleServlet => "org.openqa.grid.web.servlet.ConsoleServlet";

    /// <summary>
    /// Gets the grid1 heartbeat servlet.
    /// </summary>
    /// <value>
    /// The grid1 heartbeat servlet.
    /// </value>
    public static string Grid1HeartbeatServlet => "org.openqa.grid.web.servlet.Grid1HeartbeatServlet";
}
