using BusinessObject.Model;
using DataAccess.Model;
using DataAccess.Repository;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace SaleWPFApp
{
    /// <summary>
    /// Interaction logic for AdminOrder.xaml
    /// </summary>
    public partial class AdminOrder : Page
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        private readonly IMemberRepository memberRepository;
        public class OrderViewModel
        {
            public int OrderId { get; set; }
            public int MemberId { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? RequiredDate { get; set; }
            public DateTime? ShippedDate { get; set; }
            public decimal? Freight { get; set; }
            public decimal Total { get; set; }
        }

        public AdminOrder(IOrderRepository _orderRepository, IProductRepository _productRepository, IMemberRepository _memberRepository)
        {
            InitializeComponent();
            this.orderRepository = _orderRepository;
            this.productRepository = _productRepository;
            this.memberRepository = _memberRepository;
        }
        private void SortByTotal(object sender, RoutedEventArgs e)
        {
            var orders = orderRepository.List();
            var ordersWithTotal = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                MemberId = o.MemberId,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate,
                Freight = o.Freight,
                Total = orderRepository.GetOrderTotal(o.OrderId)
            }).OrderByDescending(o => o.Total).ToList();

            listView.ItemsSource = ordersWithTotal;
        }
        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is OrderViewModel selectedOrder)
    {

        // Enable buttons for edit, delete, details
        btnEdit.IsEnabled = true;
        btnDelete.IsEnabled = true;
        btnDetails.IsEnabled = true;

        // Example usage:
        // var detailsWindow = new OrderDetailsWindow(selectedOrderWithoutTotal, productRepository, orderRepository);
        // detailsWindow.ShowDialog();
    }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshListView();
        }

        private void RefreshListView()
        {
            var orders = orderRepository.List();
            var ordersWithTotal = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                MemberId = o.MemberId,
                OrderDate = o.OrderDate,
                RequiredDate = o.RequiredDate,
                ShippedDate = o.ShippedDate,
                Freight = o.Freight,
                Total = orderRepository.GetOrderTotal(o.OrderId)
            }).ToList();

            listView.ItemsSource = ordersWithTotal;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnDetails.IsEnabled = false;
        }

        private void Button_Search(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = StartDate.SelectedDate;
            DateTime? endDate = EndDate.SelectedDate;

            OrderFilter filter = new OrderFilter
            {
                StartDate = startDate,
                EndDate = endDate
            };
            listView.ItemsSource = orderRepository.FindAllBy(filter);
        }

        private void Button_OpenCreate(object sender, RoutedEventArgs e)
        {
            var createOrderWindow = new AddOrderWindow(orderRepository, productRepository, memberRepository);
            createOrderWindow.ShowDialog();
            RefreshListView();
        }

        

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is OrderViewModel selectedOrderViewModel)
            {
                if (MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Order selectedOrder = new Order
                    {
                        OrderId = selectedOrderViewModel.OrderId,
                        MemberId = selectedOrderViewModel.MemberId,
                        OrderDate = selectedOrderViewModel.OrderDate,
                        RequiredDate = selectedOrderViewModel.RequiredDate,
                        ShippedDate = selectedOrderViewModel.ShippedDate,
                        Freight = selectedOrderViewModel.Freight
                    };
                    // Xóa các OrderDetail liên quan trước
                    var orderDetails = orderRepository.GetOrderDetailsByOrderId(selectedOrder.OrderId);
                    foreach (var orderDetail in orderDetails)
                    {
                        orderRepository.RemoveOrderDetail(orderDetail);
                    }

                    // Xóa Order
                    orderRepository.Remove(selectedOrder);
                    RefreshListView();
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.");
            }
        }

        private void Button_Edit(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is OrderViewModel selectedOrderViewModel)
            {
                Order selectedOrder = new Order
                {
                    OrderId = selectedOrderViewModel.OrderId,
                    MemberId = selectedOrderViewModel.MemberId,
                    OrderDate = selectedOrderViewModel.OrderDate,
                    RequiredDate = selectedOrderViewModel.RequiredDate,
                    ShippedDate = selectedOrderViewModel.ShippedDate,
                    Freight = selectedOrderViewModel.Freight
                };
                var editOrderWindow = new AddOrderWindow(orderRepository, productRepository, memberRepository, selectedOrder);
                editOrderWindow.ShowDialog();
                RefreshListView();
            }
            else
            {
                MessageBox.Show("Please select an order to edit.");
            }
        }

        private void Button_Details(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem is OrderViewModel selectedOrderViewModel)
            {
                // Tạo một đối tượng Order mới chỉ với các thuộc tính cần thiết
                Order selectedOrder = new Order
                {
                    OrderId = selectedOrderViewModel.OrderId,
                    MemberId = selectedOrderViewModel.MemberId,
                    OrderDate = selectedOrderViewModel.OrderDate,
                    RequiredDate = selectedOrderViewModel.RequiredDate,
                    ShippedDate = selectedOrderViewModel.ShippedDate,
                    Freight = selectedOrderViewModel.Freight
                };

                var detailsWindow = new OrderDetailsWindow(selectedOrder, productRepository, orderRepository);
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an order to view details.");
            }
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView? listView = sender as ListView;
            GridView? gridView = listView != null ? listView.View as GridView : null;

            var width = listView != null ? listView.ActualWidth - SystemParameters.VerticalScrollBarWidth : this.Width;

            var column1 = 0.1;
            var column2 = 0.2;
            var column3 = 0.2;
            var column4 = 0.2;
            var column5 = 0.2;
            var column6 = 0.2;

            if (gridView != null && width >= 0)
            {
                gridView.Columns[0].Width = width * column1;
                gridView.Columns[1].Width = width * column2;
                gridView.Columns[2].Width = width * column3;
                gridView.Columns[3].Width = width * column4;
                gridView.Columns[4].Width = width * column5;
                gridView.Columns[5].Width = width * column6;
            }
        }
    }
}
