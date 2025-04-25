using BCrypt.Net;
using market3.DataBase;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


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
      
        public async Task Register(string name, string password,int roleid)
        {
            if (await _context.Users.AnyAsync(u=>u.Name == name))
            {
                await Clients.Caller.SendAsync("RegisterError", "Пользователь с таким именем существует");
                return;
            }
            string passwordHash = HashPassword(password);
            var user = new User { Name=name,Password=passwordHash,RoleId=roleid};
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            await Clients.Caller.SendAsync("RegisterSuccess");
        }
        public async Task Login(string name,string password)
        {
           var user = await _context.Users.FirstOrDefaultAsync(u =>u.Name == name);
            if(user ==null )
            {
                await Clients.Caller.SendAsync("LoginError", "Неверный логин или пароль");
                return;
            }
            await Clients.Caller.SendAsync("LoginSuccess");
            
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
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return
                    Convert.ToBase64String(hashBytes);
            }
        }
        private bool VerifyPassword(string password,string hash)
        {
            string hashedPassword = HashPassword(password);
            return hashedPassword== hash;
        }
    }
}
