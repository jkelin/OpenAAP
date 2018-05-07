namespace OpenAAP.Errors
{
    public class RouteNotFound : IError
    {
        public string Name { get; } = "ROUTE_NOT_FOUND";
        public string Message { get; } = "Route has not been found. You have probably requested incorrect URL.";
    }
}
