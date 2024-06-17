using System.Windows;
using System.Windows.Controls;
using BusinessObject.Model;

namespace SaleWPFApp
{
    public partial class UserProfile : UserControl
    {
        private Member _member;

        public UserProfile(Member member)
        {
            InitializeComponent();
            _member = member;
            DataContext = _member; // Bind data context to the Member object
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the password from the PasswordBox
            string newPassword = passwordBox.Password;

            // Update the password in the _member object
            _member.Password = newPassword;

            // Save changes to the database
            SaveMemberToDatabase(_member);

            MessageBox.Show("Profile saved!");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle cancel logic here
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }

        private void SaveMemberToDatabase(Member member)
        {
            // Implement your database save logic here
            // Example:
            using (var context = new SaleManagerContext())
            {
                context.Members.Update(member);
                context.SaveChanges();
            }
        }
    }
}
