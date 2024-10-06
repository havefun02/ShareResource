namespace ShareResource.Policies
{
    public class RouteManager
    {
        private readonly List<string> _routes;
        public RouteManager(IConfiguration configuration) {
            var configSection = configuration.GetSection("routes");
            if (configSection == null)
            {
                _routes = new List<string>();
            }
            else
            {
                var routeSection = configSection.GetSection("free-routes");
                if (routeSection != null)
                {
                    _routes = routeSection.Get<List<string>>()!;
                }
                else
                {
                    _routes = new List<string>();
                }
            }


        }
        public bool IsIgnore(string path)
        {
            return _routes.Contains(path);
        }
    }
}
