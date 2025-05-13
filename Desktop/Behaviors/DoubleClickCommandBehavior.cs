using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Desktop.Behaviors
{
    public class DoubleClickCommandBehavior : Behavior<Grid>
    {
        public static readonly DependencyProperty CommandProperty =
       DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DoubleClickCommandBehavior), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
      DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(DoubleClickCommandBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private DateTime _lastClickTime;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        }

        [DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();

        public static int GetDoubleClickInterval()
        {
            return (int)GetDoubleClickTime();
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var now = DateTime.Now;
            if ((now - _lastClickTime).TotalMilliseconds < GetDoubleClickInterval())
            {
                if (Command?.CanExecute(CommandParameter) == true)
                    Command.Execute(CommandParameter);
            }
            _lastClickTime = now;
        }
    }
}
