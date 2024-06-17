using System;
using System.Windows;
using BusinessObject.Model;
using DataAccess.Repository;
using SalesWPFApp;

namespace SaleWPFApp
{
    public partial class AddOrderWindow : Window
    {
        private readonly IOrderRepository orderRepository;
        private readonly IProductRepository productRepository;
        private readonly IMemberRepository memberRepository;
        private readonly Order orderToEdit;

        public AddOrderWindow(IOrderRepository _orderRepository, IProductRepository _productRepository, IMemberRepository _memberRepository, Order order = null)
        {
            InitializeComponent();
            this.orderRepository = _orderRepository;
            this.productRepository = _productRepository;
            this.memberRepository = _memberRepository;
            this.orderToEdit = order;

            // Load members into combobox
            MemberComboBox.ItemsSource = memberRepository.List();

            if (orderToEdit != null)
            {
                // Edit mode
                this.Title = "Edit Order";
                ActionButton.Content = "Save";
                MemberComboBox.SelectedValue = orderToEdit.MemberId;
                OrderDatePicker.SelectedDate = orderToEdit.OrderDate;
                RequiredDatePicker.SelectedDate = orderToEdit.RequiredDate;
                ShippedDatePicker.SelectedDate = orderToEdit.ShippedDate;
                FreightTextBox.Text = orderToEdit.Freight.ToString();
            }
            else
            {
                // Add mode
                this.Title = "Add Order";
                ActionButton.Content = "Add";
            }
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            if (orderToEdit != null)
            {
                // Update existing order
                orderToEdit.MemberId = (int)MemberComboBox.SelectedValue;
                orderToEdit.OrderDate = OrderDatePicker.SelectedDate.Value;
                orderToEdit.RequiredDate = RequiredDatePicker.SelectedDate.Value;
                orderToEdit.ShippedDate = ShippedDatePicker.SelectedDate.Value;
                orderToEdit.Freight = decimal.Parse(FreightTextBox.Text);

                orderRepository.Update(orderToEdit);
            }
            else
            {
                if (MemberComboBox.SelectedValue != null &&
                    OrderDatePicker.SelectedDate.HasValue &&
                    RequiredDatePicker.SelectedDate.HasValue &&
                    ShippedDatePicker.SelectedDate.HasValue &&
                    !string.IsNullOrWhiteSpace(FreightTextBox.Text))
                {
                    // Create new Order
                    Order newOrder = new Order
                    {
                        MemberId = (int)MemberComboBox.SelectedValue,
                        OrderDate = OrderDatePicker.SelectedDate.Value,
                        RequiredDate = RequiredDatePicker.SelectedDate.Value,
                        ShippedDate = OrderDatePicker.SelectedDate.Value,
                        Freight = decimal.Parse(FreightTextBox.Text)
                    };

                    orderRepository.Add(newOrder);

                    // Open AddOrderDetailWindow to add products to the order
                    var addOrderDetailWindow = new AddOrderDetailWindow(orderRepository, productRepository, newOrder);
                    addOrderDetailWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields.");
                    return; // Return to prevent closing the window when not all required fields are filled
                }
            }

            this.Close();
        }
    }
}
