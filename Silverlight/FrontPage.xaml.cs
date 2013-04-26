using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Tutorial16___Multiple_Viewport___Phone
{
    public partial class FrontPage : PhoneApplicationPage
    {
        string userName = "";
        public FrontPage()
        {
            InitializeComponent();
        }
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            userName = userBox.Text;
        }

        private void TextBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            userBox.Text = "";
            ErrorBox.Text = "";
        }

        private void userBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                userName = userBox.Text;
                this.Focus();
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (userBox.Text != "" && userBox.Text != "Username:")
                NavigationService.Navigate(new Uri(string.Format("/GamePage.xaml?Username={0}", userName), UriKind.Relative));
            else ErrorBox.Text = "Cannot start without entering a valid username";
        }


        //private void TextBox_Start(object sender, System.Windows.Input.TextCompositionEventArgs e)
        //{
        //    userBox.Text = "";
        //}

    }
}