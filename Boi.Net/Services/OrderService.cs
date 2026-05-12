using Boi.Net.Data;
using Boi.Net.DTOs.BookDTOs;
using Boi.Net.DTOs.OrderDTOs;
using Boi.Net.Model;
using Microsoft.EntityFrameworkCore;

namespace Boi.Net.Services
{
    public class OrderService
    {

        private readonly BoiNetDbContext _context;

        public OrderService(BoiNetDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(string userId, CreateOrderDto createOrderDto)
        {

            var order = new Order
            {
                UserId = userId,
                OrderStatus = "Pending",
                PaymentStatus = "Unpaid",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0
            };

            decimal totalAmount = 0;

            foreach(var item in createOrderDto.OrderItems)
            {
                var book = await _context.Books.FindAsync(item.BookId);

                if(book == null || !book.IsAvailable)
                {
                    throw new Exception("This Book is on Found.");
                };

                var orderItem = new OrderItem
                {
                    BookId = book.Id,
                    Quantity = item.Quantity,
                    Price = book.Price
                };

                order.OrderItems.Add(orderItem);

                totalAmount += book.Price * item.Quantity;

            };

            order.TotalAmount = totalAmount;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;

        }



        public async Task<object> GetMyPurchaseBookAsync(string userId)
        {

            var purchasedBook = await _context.OrderItems
                .Where(orderItem => orderItem.Order!.UserId == userId && orderItem.Order.PaymentStatus == "Paid")
                .Select(orderItem => new
                {

                    Book = orderItem.Book!.Id,
                    Title = orderItem.Book.Title,
                    Author = orderItem.Book.Author,
                    CoverPhoto = orderItem.Book.CoverPhoto,
                    PageCount = orderItem.Book.PageCount

                })
                .Distinct().ToListAsync();


            return purchasedBook;

        }


    }
}
