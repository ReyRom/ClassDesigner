using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ClassDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow_Executed));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow_Executed));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow_Executed));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow_Executed));

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = MessageBox.MessageBox.Show("Выход", "Вы уверены, что хотите выйти? Несохраненные изменения могут быть утеряны", MessageBox.MessageBoxButtons.YesNo) != MessageBox.MessageBoxResult.Yes;
        }

        private void RestoreWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void MinimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void MaximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
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
