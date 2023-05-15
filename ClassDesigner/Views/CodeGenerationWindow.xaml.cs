using System;
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
using System.Windows.Shapes;

namespace ClassDesigner.Views
{
    /// <summary>
    /// Логика взаимодействия для CodeGenerationWindow.xaml
    /// </summary>
    public partial class CodeGenerationWindow : Window
    {
        public CodeGenerationWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow_Executed));
        }

        private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
