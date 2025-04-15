using market3.DataBase;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace market3
{
    public class ProductHub : Hub
    {
        private readonly InternetMarketBalkaContext _context;
        public ProductHub(InternetMarketBalkaContext context)
        {
            _context = context;
           
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
      
        public async Task Registr(string name, string password)
        {
            if (_context.Users.Any(u =>u.Name.ToLower()==name.ToLower()))
            {
                Clients.Caller.SendAsync("Ошибка регистрации", "Пользователь с таким именем уже существует.");
                return;
            }
            var user = new User { Name = name, Password = password };
            _context.Add(user);
            await _context.SaveChangesAsync();
            Clients.Caller.SendAsync("Регистрация прошла успешна.");
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
