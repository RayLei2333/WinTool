﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Desktop.ViewModel;

namespace Desktop.Views
{
    /// <summary>
    /// LargeIconView.xaml 的交互逻辑
    /// </summary>
    public partial class LargeIconView : System.Windows.Controls.UserControl
    {
        public LargeIconView()
        {
            InitializeComponent();
        }

        public LargeIconView(FileViewTypeViewModel fileViewTypeViewModel) : this()
        {
            this.DataContext = fileViewTypeViewModel;
        }
    }
}
