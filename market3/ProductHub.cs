using market3.DataBase;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace market3
{
    public class ProductHub : Hub
    {
        private readonly InternetMarketBalkaContext _context;
        private readonly IHubContext<ProductHub> _hubContext;
        public ProductHub(InternetMarketBalkaContext context, IHubContext<ProductHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
           
        }
        public async Task AddProduct(Tovar tovar)
        {
            _context.Tovars.Add(tovar);
            _context.SaveChangesAsync();
            await Clients.All.SendAsync("ProductAdded", tovar);
        }
        public async Task GetCategoriesAsync()
        {
            var category = await _context.Categories.ToListAsync();
                await Clients.Caller.SendAsync("ReceiveCategory", category);
        }
      
        public async Task<bool> RegisterAsync(string name, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Name=name, Password=passwordHash};
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("UserRegistered");
                return true;
        }
        public async Task<User?>GetUserAsync(string name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
        }

        public async Task GetProduct()
        {
            var product = await _context.Tovars.ToListAsync();
            await Clients.Caller.SendAsync("ReceiveProducts", product);
        }

        public async Task GetProductById(int id)
        {
            var product = await _context.Tovars.Include(s=>s.Category).FirstOrDefaultAsync(s=>s.Id == id);
            await Clients.Caller.SendAsync("ReceiveProduct", (TovarDTO)product);
        }
       

    }
}
