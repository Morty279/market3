using market3.DataBase;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;



namespace market3
{
    public class ProductHub : Hub
    {
        static Dictionary<string, HubCallerContext> clients = new();
      
        private readonly InternetMarketBalkaContext _context;
        public ProductHub(InternetMarketBalkaContext context)
        {         
            _context = context;
          
        }

        public override Task OnConnectedAsync()
        {
            var feature = Context.Features.Get<IHttpConnectionFeature>();
            var address = feature.RemoteIpAddress.ToString();
            if (clients.ContainsKey(address))
            {
                clients[address].Abort();
                clients.Remove(address);
            }
            clients.Add(address, Context);


            return base.OnConnectedAsync();
        }

        public async Task AddProduct(string name,string description, byte[] image, int categoryId, int price, int quantity)
        {
            var tovar = new Tovar { Name = name, Description = description, Image = image, CategoryId = categoryId, Price = price, Quantity = quantity };
            await _context.Tovars.AddAsync(tovar);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("ProductAdded");
        }
        public async Task GetCategoriesAsync()
        {
            var category = await _context.Categories.ToListAsync();
                await Clients.Caller.SendAsync("ReceiveCategory", category);
        }
      
        public async Task Register(string name, string password,int roleid)
        {
            if (await _context.Users.AnyAsync(u=>u.Name == name))
            {
                await Clients.Caller.SendAsync("RegisterError", "Пользователь с таким именем существует");
                return;
            }
            else
            {
                string passwordHash = HashPassword(password);
                var user = new User { Name = name, Password = passwordHash, RoleId = roleid };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await Clients.Caller.SendAsync("RegisterSuccess","Регистрация прошла успешна");
               
            }
            
        }
        public async Task Login(string name,string password)
        {
           var user = await _context.Users.FirstOrDefaultAsync(u =>u.Name==name);
            if(user ==null)
            {
                await Clients.Caller.SendAsync("LoginError", "Неверный логин или пароль ");
                return;
            }
            if (!VerifyPasswordHash(password, user.Password))
            {
                await Clients.Caller.SendAsync("LoginError", "Неверный логин или пароль ");
                return;
            }
            else
            {
                await Clients.Caller.SendAsync("LoginSuccess" , "Успешный вход");
            }
            
            
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
       public async Task GetRole()
        {
            var role =await _context.Roles.ToListAsync();
            await Clients.Caller.SendAsync("ReceiveRole", role);
        }
        public async Task GetUser()
        {
            var user = await _context.Users.ToListAsync();
            await Clients.Caller.SendAsync("ReceiveUser", user);
        }
        public async Task AddCart(int TovarId, int Quantity, decimal Price)
        {
            var TovarInZakaz = new TovarInZakaz { TovarId = TovarId, Quantity = Quantity, Price = Price };
            var exictingTovar = await _context.TovarInZakazs.FirstOrDefaultAsync(x => x.TovarId == TovarId);
            if (exictingTovar != null)
            {
                await Clients.All.SendAsync("CartDob", "Товар добавлен");
            }
            else
            {
                _context.TovarInZakazs.Add(TovarInZakaz);
                await _context.SaveChangesAsync();
                await Clients.Caller.SendAsync("CartAdd", TovarInZakaz);
            }
        }
        public async Task DeleteCart(int TovarId)
        {
            var CartTovar = await _context.TovarInZakazs.FindAsync(TovarId);
            if (CartTovar != null)
            {
                _context.TovarInZakazs.Remove(CartTovar);
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("CartUpdate");
            }
        }
        public async Task UpdateQuantity(int TovarId, int newQuantity)
        {
            var cartTovar = await _context.TovarInZakazs.FindAsync(TovarId);
            if (cartTovar != null)
            {
                cartTovar.Quantity = newQuantity;
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("CartUpdate");
                
            }
        }
        public async Task GetCartTovar()
        {
            var cart = await _context.TovarInZakazs.Include(s=>s.Tovar).ToListAsync();
            await Clients.Caller.SendAsync("ReciveCart",cart);
        }
        public async Task SearchTovars(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                await Clients.Caller.SendAsync("ReciveSearchResults", new List<Tovar>());
                return;
            }
            var results = await _context.Tovars.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)).ToListAsync();
            await Clients.Caller.SendAsync("ReciveSearchResults", results);
        }
        
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return
                    Convert.ToBase64String(hashBytes);
            }
        }
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            string newHash = HashPassword(password);
            return string.Equals(newHash, storedHash, StringComparison.Ordinal);
        }
    }
}
