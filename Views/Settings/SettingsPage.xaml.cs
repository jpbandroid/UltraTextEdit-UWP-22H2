﻿using CommunityToolkit.Mvvm.Input;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UltraTextEdit_UWP.Views.Settings
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            var ver = typeof(App).GetTypeInfo().Assembly.GetName().Version;



            var appViewTitleBar = ApplicationView.GetForCurrentView().TitleBar;

            appViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            Window.Current.SetTitleBar(AppTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
            {
                soundToggle.IsOn = true;
            }
            else
            {
                soundToggle.IsOn = false;
            }

            if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On)
            {
                spatialAudioBox.IsChecked = true;
            }

            if (Application.Current.FocusVisualKind == FocusVisualKind.HighVisibility)
            {
                HighVisibility.IsChecked = true;
            }
            else
            {
                RevealFocus.IsChecked = true;
            }
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

        #region CopyVersionCommand

        internal IRelayCommand CopyVersionCommand { get; }

        private void ExecuteCopyVersionCommand()
        {

                var data = new DataPackage
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                data.SetText(aboutblock.Title + " version " + aboutblock.Description);

                Clipboard.SetContentWithOptions(data, new ClipboardContentOptions() { IsAllowedInHistory = true, IsRoamable = true });
                Clipboard.Flush();
        }
        #endregion

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                rootFrame.Navigate(typeof(UTEUpdate.UTEUpdate));
            }
        }

        private void CopyVerInfo(object sender, RoutedEventArgs e)
        {
            var data = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            data.SetText(aboutblock.Title + " version " + aboutblock.Description);

            Clipboard.SetContentWithOptions(data, new ClipboardContentOptions() { IsAllowedInHistory = true, IsRoamable = true });
            Clipboard.Flush();
        }

        private void HighVisibility_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.FocusVisualKind = FocusVisualKind.HighVisibility;
        }

        private void RevealFocus_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.FocusVisualKind = FocusVisualKind.Reveal;
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                spatialAudioBox.IsEnabled = true;
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else
            {
                spatialAudioBox.IsEnabled = false;
                spatialAudioBox.IsChecked = false;

                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
        }
        private void spatialAudioBox_Checked(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }
        }

        private void spatialAudioBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
        }
    }
}
