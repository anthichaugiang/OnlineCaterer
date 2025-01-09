using catere_be.Data;
using catere_be.Dto;
using catere_be.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace catere_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderRequestModel request)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var customerOrder = new CustomerOrder
                        {
                            CustomerId = request.CustomerId,
                            RoomId = request.RoomId,
                            DeliveryDate = request.DeliveryDate,
                            VAT = request.VAT,
                            Status = "Pending",
                            PeopleCount = request.NumPeople,
                            SupplierId = request.SupplierId,
                            Name = request.Name,
                            PhoneNumber = request.PhoneNumber,
                            TotalPrice = request.TotalPrice,
                            IsActive = true, // Chỉnh thành true nếu cần
                            CustomerOrderMenu = new List<CustomerOrderMenu>()
                        };

                        _context.CustomerOrder.Add(customerOrder);
                        _context.SaveChanges();

                        foreach (var menuItem in request.ListMenu)
                        {
                            var orderMenu = new CustomerOrderMenu
                            {
                                MenuItemId = menuItem.MenuItemId,
                                RoomId = request.RoomId,
                                OrderId = customerOrder.OrderId,
                                Quantity = menuItem.Quantity,
                                IsActive = true // Chỉnh thành true nếu cần
                            };

                            customerOrder.CustomerOrderMenu.Add(orderMenu);
                        }

                        _context.CustomerOrderMenu.AddRange(customerOrder.CustomerOrderMenu);
                        _context.SaveChanges();

                        transaction.Commit();
                        return Ok(new { success = true, message = "Order created successfully." });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return BadRequest(new { success = false, message = "Failed to create order.", error = ex.Message });
                    }
                }
            }
            return BadRequest(new { success = false, message = "Invalid data." });
        }
        [HttpGet("GetOrdersBySupplier/{supplierId}")]
        public IActionResult GetOrdersBySupplier(int supplierId)
        {
            var orders = _context.CustomerOrder
                                 .Where(o => o.SupplierId == supplierId && o.IsActive)
                                 .Include(o => o.CustomerOrderMenu)
                                 .ToList();

            if (orders == null || orders.Count == 0)
            {
                return NotFound(new { success = false, message = "No orders found for the supplier." });
            }

            var orderDtos = orders.Select(o => new CustomerOrderDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                RoomId = o.RoomId,
                DeliveryDate = o.DeliveryDate,
                VAT = o.VAT,
                Status = o.Status,
                PeopleCount = o.PeopleCount,
                SupplierId = o.SupplierId,
                Name = o.Name,
                PhoneNumber = o.PhoneNumber,
                TotalPrice = o.TotalPrice,
                IsActive = o.IsActive,
                CustomerOrderMenus = o.CustomerOrderMenu.Select(m => new CustomerOrderMenuDto
                {
                    OrderMenuId = m.OrderMenuId,
                    MenuItemId = m.MenuItemId,
                    RoomId = m.RoomId,
                    OrderId = m.OrderId,
                    Quantity = m.Quantity,
                    IsActive = m.IsActive
                }).ToList()
            }).ToList();

            return Ok(new { success = true, data = orderDtos });
        }


        [HttpGet("GetOrdersByCustomer/{customerId}")]
        public IActionResult GetOrdersByCustomer(int customerId)
        {
            var orders = _context.CustomerOrder
                                 .Where(o => o.CustomerId == customerId && o.IsActive)
                                 .Include(o => o.CustomerOrderMenu)
                                 .ToList();

            if (orders == null || orders.Count == 0)
            {
                return NotFound(new { success = false, message = "No orders found for the customer." });
            }

            var orderDtos = orders.Select(o => new CustomerOrderDto
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                RoomId = o.RoomId,
                DeliveryDate = o.DeliveryDate,
                VAT = o.VAT,
                Status = o.Status,
                PeopleCount = o.PeopleCount,
                SupplierId = o.SupplierId,
                Name = o.Name,
                PhoneNumber = o.PhoneNumber,
                TotalPrice = o.TotalPrice,
                IsActive = o.IsActive,
                CustomerOrderMenus = o.CustomerOrderMenu.Select(m => new CustomerOrderMenuDto
                {
                    OrderMenuId = m.OrderMenuId,
                    MenuItemId = m.MenuItemId,
                    RoomId = m.RoomId,
                    OrderId = m.OrderId,
                    Quantity = m.Quantity,
                    IsActive = m.IsActive
                }).ToList()
            }).ToList();

            return Ok(new { success = true, data = orderDtos });
        }

        [HttpDelete("DeleteOrder/{orderId}")]
        public IActionResult DeleteOrder(int orderId)
        {
            var order = _context.CustomerOrder
                                .Include(o => o.CustomerOrderMenu)
                                .FirstOrDefault(o => o.OrderId == orderId && o.IsActive);

            if (order == null)
            {
                return NotFound(new { success = false, message = "Order not found." });
            }

            // Đánh dấu order là không hoạt động thay vì xóa vĩnh viễn
            order.IsActive = true;

            // Cũng đánh dấu tất cả các CustomerOrderMenu liên quan là không hoạt động
            foreach (var orderMenu in order.CustomerOrderMenu)
            {
                orderMenu.IsActive = false;
            }

            _context.SaveChanges();
            return Ok(new { success = true, message = "Order deleted successfully." });
        }


    }
}