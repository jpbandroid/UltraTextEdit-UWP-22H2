using MicaForUWP.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using UltraTextEdit_UWP.Helpers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Management.Deployment;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UltraTextEdit_UWP.Views.UTEUpdate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UTEUpdate : Page
    {
        public UTEUpdate()
        {
            this.InitializeComponent();

            if (BuildInfo.BeforeWin11)
            {
                if (App.Current.RequestedTheme == ApplicationTheme.Light)
                {
                    Application.Current.Resources["AppTitleBarBrush"] = new BackdropMicaBrush()
                    {
                        LuminosityOpacity = 0.8F,
                        TintOpacity = 0F,
                        BackgroundSource = BackgroundSource.WallpaperBackdrop,
                        Opacity = 1,
                        TintColor = Windows.UI.Color.FromArgb(255, 230, 230, 230),
                        FallbackColor = Windows.UI.Color.FromArgb(255, 230, 230, 230)
                    };
                    this.Background = (Brush)Application.Current.Resources["AppTitleBarBrush"];
                }
                else
                {
                    Application.Current.Resources["AppTitleBarBrush"] = new BackdropMicaBrush()
                    {
                        LuminosityOpacity = 0.8F,
                        TintOpacity = 0F,
                        BackgroundSource = BackgroundSource.WallpaperBackdrop,
                        Opacity = 1,
                        TintColor = Windows.UI.Color.FromArgb(255, 25, 25, 25),
                        FallbackColor = Windows.UI.Color.FromArgb(25, 25, 25, 25)
                    };
                    this.Background = (Brush)Application.Current.Resources["AppTitleBarBrush"];
                }

            }
            else
            {

            }
        

            var appViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;

            appViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            Window.Current.SetTitleBar(AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            AppTitleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }



        //check for an update on my server
        private async void CheckUpdate(object sender, RoutedEventArgs e)
        {
            updatetext.Text = "Checking for updates";
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://garoag.com/jpb/wp-content/uploads/Version22H2.txt");
            Stream stream2 = client.OpenRead("https://garoag.com/jpb/wp-content/uploads/updatedescript22h2.txt");
            StreamReader reader = new StreamReader(stream);
            //StreamReader reader2 = new StreamReader(stream2);
            var newVersion = new Version(await reader.ReadToEndAsync());
            //var newVersiondescription = await reader2.ReadToEndAsync();
            Package package = Package.Current;
            PackageVersion packageVersion = package.Id.Version;
            var currentVersion = new Version(string.Format("{0}.{1}.{2}.{3}", packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision));

            //compare package versions
            if (newVersion.CompareTo(currentVersion) > 0)
            {
                updatetext.Text = "Updates available";
                updatebutton2.Content = "Install updates";
                updatebutton2.Visibility = Visibility.Visible;
                updatebutton3.Visibility = Visibility.Visible;
                //updateinfo.Title = newVersiondescription;
                var messageDialog = new MessageDialog("Found an update.");
                messageDialog.Commands.Add(new UICommand(
                    "Update",
                    new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.Commands.Add(new UICommand(
                    "Close",
                    new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;
                await messageDialog.ShowAsync();
            }
            else
            {
                updatestatusok.Visibility = Visibility.Visible;
                updatetext.Text = "You're up to date";
                var messageDialog = new MessageDialog("Did not find an update.");
                await messageDialog.ShowAsync();
            }
        }

        // Queue up the update and close the current app instance.
        private async void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Update")
            {
                PackageManager packagemanager = new PackageManager();
                await packagemanager.AddPackageAsync(
                    new Uri("https://github.com/jpbandroid/UpdateZips/raw/main/UTE-23H1-latest.msixbundle"),
                    null,
                    DeploymentOptions.ForceApplicationShutdown
                );
            }
            if (command.Label == "Close")
            {
                updatestatusnotok.Visibility = Visibility.Visible;
                updatetext.Text = "Updates failed";
            }
        }

        private void Icon_Personalize(object sender, RoutedEventArgs e)
        {
            if (accentswitch.IsOn == true)
            {
                updateicon.Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"]);
            } else
            {
                updateicon.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 212));
            }
        }

        private async void update(object sender, RoutedEventArgs e)
        {
            PackageManager packagemanager = new PackageManager();
            await packagemanager.AddPackageAsync(
                new Uri("https://occoam.com/jpb/wp-content/uploads/UTEUWP22621_latest.msixbundle"),
                null,
                DeploymentOptions.ForceApplicationShutdown
            );
        }

        private void updatecancel(object sender, RoutedEventArgs e)
        {
            updatestatusnotok.Visibility = Visibility.Visible;
            updatetext.Text = "Updates failed";
        }
    }
}
