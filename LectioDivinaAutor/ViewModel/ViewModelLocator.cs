/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:LectioDivina.Autor.ViewModel"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;

using MvvmLight.Extensions.Wpf;

using Microsoft.Practices.ServiceLocation;
using LectioDivina.Service;

namespace LectioDivina.Autor.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                // todo - temp
                SimpleIoc.Default.Register<IContemplationDataService, ContemplationDataService>();                
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IContemplationDataService, ContemplationDataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MvvmLight.Extensions.IDialogService, MvvmLight.Extensions.Wpf.DialogService>();
            SimpleIoc.Default.Register<ICredentialsService, CredentialsService>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}