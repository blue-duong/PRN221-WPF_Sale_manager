using DataAccess.Repository;
using System.Windows;
using System.Windows.Input;

namespace SaleWPFApp
{
    /// <summary>
    /// Interaction logic for AdminManager.xaml
    /// </summary>
    public partial class AdminManager : Window
    {
        private readonly IProductRepository productRepository;
        private readonly IMemberRepository memberRepository;
        private readonly IOrderRepository orderRepository;
        private readonly MainWindow mainWindow;
        public AdminManager(MainWindow _mainWindow, IProductRepository _productRepository, IMemberRepository _memberRepository, IOrderRepository _orderRepository)
        {
            InitializeComponent();
            this.productRepository = _productRepository;
            this.memberRepository = _memberRepository;
            this.orderRepository = _orderRepository;
            this.mainWindow = _mainWindow;
            Closing += AdminManager_Closing;
        }

        private void AdminManager_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.Show();
        }

        private void Goto_AdminProduct(object sender, RoutedEventArgs e)
        {
            AdminProduct adminProduct = new AdminProduct(productRepository);
            frameMain.Content = adminProduct;
        }

        private void Goto_AdminMember(object sender, RoutedEventArgs e)
        {
            AdminMember adminMember = new AdminMember(memberRepository);
            frameMain.Content = adminMember;
        }

        private void Goto_AdminOrder(object sender, RoutedEventArgs e)
        {
            AdminOrder adminOrder = new AdminOrder(orderRepository, productRepository, memberRepository); // Truyền cả ba tham số ở đây
            frameMain.Content = adminOrder;
        }

        private void Button_Logout(object sender, RoutedEventArgs e)
        {
            this.Close();
            mainWindow.Show();
        }
    }
}
