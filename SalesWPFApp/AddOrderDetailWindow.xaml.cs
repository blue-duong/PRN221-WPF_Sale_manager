using System.Windows;
using System.Windows.Controls;
using BusinessObject.Model;
using DataAccess.Model;
using DataAccess.Repository;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using static SalesWPFApp.AddOrderDetailWindow;

namespace SalesWPFApp
{
    public partial class AddOrderDetailWindow : Window
    {
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly Order order;
        public class CartItem
        {
            public Product Product { get; set; }
            public int Quantity { get; set; }
        }
        private ObservableCollection<CartItem> cartItems = new ObservableCollection<CartItem>();

        public AddOrderDetailWindow(IOrderRepository _orderRepository, IProductRepository _productRepository, Order _order)
        {
            InitializeComponent();
            this.productRepository = _productRepository;
            this.orderRepository = _orderRepository;
            this.order = _order;
            

            // Initialize the OrderDetails collection if null
            if (this.order.OrderDetails == null)
            {
                this.order.OrderDetails = new ObservableCollection<OrderDetail>();
            }

            // Load products into the ItemsControl
            LoadProducts();

            // Bind the shopping cart to the ListBox
            ShoppingCart.ItemsSource = cartItems;
        }

        private void LoadProducts()
        {
            ListProduct.ItemsSource = productRepository.List();
        }
        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int productId = (int)button.Tag;

            CartItem selectedCartItem = cartItems.FirstOrDefault(ci => ci.Product.ProductId == productId);
            if (selectedCartItem != null)
            {
                cartItems.Remove(selectedCartItem);
                MessageBox.Show("Product removed from cart successfully.");
            }
        }


        private void AddToCart(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int productId = (int)button.Tag;

            Product selectedProduct = productRepository.FindById(productId);

            if (selectedProduct != null)
            {
                var existingCartItem = cartItems.FirstOrDefault(ci => ci.Product.ProductId == productId);
                if (existingCartItem == null)
                {
                    cartItems.Add(new CartItem { Product = selectedProduct, Quantity = 1 });
                    MessageBox.Show("Product added to cart successfully.");
                }
                else
                {
                    existingCartItem.Quantity++;
                }
            }
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load the order from the database
                var dbOrder = orderRepository.FindById(order.OrderId);
                if (dbOrder == null)
                {
                    MessageBox.Show("Không tìm thấy đơn hàng.");
                    return;
                }

                foreach (CartItem cartItem in cartItems)
                {
                    OrderDetail newOrderDetail = new OrderDetail
                    {
                        OrderId = dbOrder.OrderId,
                        ProductId = cartItem.Product.ProductId,
                        UnitPrice = cartItem.Product.UnitPrice,
                        Quantity = cartItem.Quantity,
                        Discount = 0 // Default discount
                    };

                    dbOrder.OrderDetails.Add(newOrderDetail);
                }

                // Retry mechanism for optimistic concurrency
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    try
                    {
                        // Attempt to update order with new details
                        orderRepository.Update(dbOrder);

                        MessageBox.Show("Thanh toán thành công. Chi tiết đơn hàng đã được thêm.");
                        this.Close();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;

                        // Get the current entity values and the values in the database
                        var entry = ex.Entries.Single();
                        var clientValues = entry.CurrentValues;
                        var databaseEntry = entry.GetDatabaseValues();

                        if (databaseEntry == null)
                        {
                            MessageBox.Show("Đơn hàng đã bị xóa bởi người dùng khác.");
                            return;
                        }

                        var databaseValues = databaseEntry.ToObject();

                        // Refresh the original values to bypass the concurrency issue
                        entry.OriginalValues.SetValues(databaseEntry);

                        MessageBox.Show("Đơn hàng đã được cập nhật bởi người dùng khác. Các thay đổi của bạn đã được làm mới. Vui lòng thử lại.");
                    }
                } while (saveFailed);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }




    }
}
