using ClassDesigner.Views;
using System.Windows;

namespace ClassDesigner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainWindow mainWindow;
        private static CodeGenerationWindow codeGenerationWindow;

        public static MainWindow MainWindow => mainWindow ??= new MainWindow();

        public static CodeGenerationWindow CodeGenerationWindow => codeGenerationWindow ??= new CodeGenerationWindow();

        public App()
        {
            InitializeComponent();
            MainWindow.Show();
            MainWindow.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            codeGenerationWindow?.Close();
        }
    }
}
