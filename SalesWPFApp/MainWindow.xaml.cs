using BusinessObject.Model;
using DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows;

namespace SaleWPFApp
{
    public partial class MainWindow : Window
    {
        private readonly IProductRepository productRepository;
        private readonly IMemberRepository memberRepository;
        private readonly IOrderRepository orderRepository;

        public MainWindow(IProductRepository _productRepository, IMemberRepository _memberRepository, IOrderRepository _orderRepository)
        {
            InitializeComponent();
            this.productRepository = _productRepository;
            this.memberRepository = _memberRepository;
            this.orderRepository = _orderRepository;
        }

        public void ResetFormLogin()
        {
            txtBoxUsername.Text = null;
            pwdBoxPassword.Password = null;
        }

        private void Btn_login(object sender, RoutedEventArgs e)
        {
            string username = txtBoxUsername.Text.ToString();
            string password = pwdBoxPassword.Password.ToString();

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                // Authenticate against admin credentials
                if (IsAdminAccount(username, password))
                {
                    Session.Username = username;
                    this.Hide();
                    AdminManager adminManager = new AdminManager(this, productRepository, memberRepository, orderRepository);
                    adminManager.Show();
                    ResetFormLogin();
                }
                // Authenticate against member repository
                else if (AuthenticateUser(username, password) is Member authenticatedMember)
                {
                    Session.Username = username;
                    this.Hide();
                    Home home = new Home(authenticatedMember, this, productRepository, orderRepository, memberRepository);
                    home.Show();
                    ResetFormLogin();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            else
            {
                MessageBox.Show("Please enter username and password.");
            }
        }

        private bool IsAdminAccount(string username, string password)
        {
            var filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var account = new ConfigurationBuilder().AddJsonFile(filePath).Build().GetSection("account");
            return username.Equals(account["username"]) && password.Equals(account["password"]);
        }

        private Member AuthenticateUser(string username, string password)
        {
            return memberRepository.FindByEmailAndPassword(username, password);
        }
    }
}
