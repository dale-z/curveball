using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Tutorial16___Multiple_Viewport___Phone
{
    public partial class Options : PhoneApplicationPage
    {
        int gameMode = 0;
        public Options()
        {
            InitializeComponent();
        }

        private void Player_vs_CPU_Checked(object sender, RoutedEventArgs e)
        {
            gameMode = 1;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            gameMode = 2;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            gameMode = 3;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            gameMode = 4;
        }
    }
}