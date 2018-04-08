using Microsoft.Practices.Unity;
using Prism.Unity;
using HebbPerceptron.Views;
using System.Windows;

namespace HebbPerceptron
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
