using store.server.Infrastructure.Models.Main;

namespace store.server.Infrastructure.Models.Returns
{
    public class LandingProducts
    {
        public IEnumerable<Products>? LivingRoom { get; set; }
        public IEnumerable<Products>? Kitchen { get; set; }
        public IEnumerable<Products>? Bedroom { get; set; }
        public IEnumerable<Products>? DiningRoom { get; set; }
    }
}
