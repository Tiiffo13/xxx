using RepairPlus.Infrastucture;
using RepairPlus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RepairPlus.Controllers
{
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(DataContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Получение текущего пользователя
                var currentUser = _userManager.GetUserAsync(User).Result;

                // Проверка наличия текущего пользователя
                if (currentUser != null)
                {
                    // Присвоение UserId заказу
                    order.UserId = currentUser.Id;

                    DateTime now = DateTime.Now;

                    // Отдельно дата
                    DateTime dateOnly = now.Date;

                    // Отдельно время
                    TimeSpan timeOnly = new TimeSpan(now.Hour, now.Minute, 0);

                    order.Date = dateOnly;

                    order.Time = timeOnly.ToString();

                    // Добавление заказа в базу данных
                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    // Перенаправление на страницу успешного добавления заказа
                    return RedirectToAction("Index", "Home");
                }
            }

            // Обработка ошибок валидации
            return View(order);
        }
        [Authorize]
        public async Task<IActionResult> Details()
        {
            // Получаем текущего пользователя
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                // Если текущий пользователь является администратором, получаем все заказы
                if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
                {
                    var allOrders = _context.Orders.Include(o => o.User).ToList();
                    return View(allOrders);
                }

                // Получаем список заказов для текущего пользователя
                var userOrders = _context.Orders.Where(o => o.UserId == currentUser.Id).ToList();
                return View(userOrders);
            }

            return RedirectToAction("");
        }

    }
}
