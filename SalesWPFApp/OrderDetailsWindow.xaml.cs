using BusinessObject.Model;
using DataAccess.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SaleWPFApp
{
    public partial class OrderDetailsWindow : Window
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderDetailsWindow(Order order, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            InitializeComponent();
            _productRepository = productRepository;
            _orderRepository = orderRepository;

            // Bind order details to text blocks
            OrderIdTextBlock.Text = order.OrderId.ToString();
            MemberIdTextBlock.Text = order.MemberId.ToString();
            OrderDateTextBlock.Text = order.OrderDate.ToString("d");
            RequiredDateTextBlock.Text = order.RequiredDate?.ToString("d");
            ShippedDateTextBlock.Text = order.ShippedDate?.ToString("d");
            FreightTextBlock.Text = order.Freight?.ToString("F2");

            // Bind order details to DataGrid
            LoadOrderDetails(order.OrderId);
        }

        private void LoadOrderDetails(int orderId)
        {
            // Assuming YourRepository là repository của bạn
            var orderDetails = _orderRepository.GetOrderDetailsByOrderId(orderId);

            var orderDetailViewModels = orderDetails.Select(od => new OrderDetailViewModel
            {
                ProductId = od.ProductId,
                ProductName = _productRepository.FindById(od.ProductId).ProductName,
                UnitPrice = od.UnitPrice,
                Quantity = od.Quantity,
                Discount = od.Discount
            }).ToList();

            OrderDetailsDataGrid.ItemsSource = orderDetailViewModels;
        }
    }
    public class OrderDetailViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
    }
}
