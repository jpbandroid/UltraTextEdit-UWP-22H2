﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using UltraTextEdit_UWP.ViewModels;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.System;
using Windows.ApplicationModel.DataTransfer;
using MicaForUWP.Media;
using UltraTextEdit_UWP.Helpers;

namespace UltraTextEdit_UWP.Views
{
    public sealed partial class HomePage : Page
    {
        private ObservableCollection<RecentlyUsedViewModel> list = new();
        private ObservableCollection<WhatsNewItemViewModel> WhatsNew = new();
        private WhatsNewItemViewModel SelectedItem = new()
        {
            Title = "Select an item from the sidebar",
            Description = "To view its details."
        };
        private bool IsListEmpty = false;

        public HomePage()
        {
            InitializeComponent();

            if (BuildInfo.BeforeWin11)
            {
                Application.Current.Resources["AppTitleBarBrush"] = new BackdropMicaBrush()
                {
                    LuminosityOpacity = 0.8F,
                    TintOpacity = 0F,
                    BackgroundSource = BackgroundSource.WallpaperBackdrop,
                    Opacity = 1,
                    TintColor = Color.FromArgb(255, 230, 230, 230),
                    FallbackColor = Color.FromArgb(255, 230, 230, 230)
                };
                this.Background = (Brush)Application.Current.Resources["AppTitleBarBrush"];
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

            Loaded += HomePage_Loaded;
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

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            list.Clear();
            WhatsNew.Clear();

            var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;

            foreach (Windows.Storage.AccessCache.AccessListEntry entry in mru.Entries)
            {
                StorageFile file = await mru.GetFileAsync(entry.Token);

                list.Add(new()
                {
                    Name = file.Name,
                    OriginalFile = file,
                    Path = file.Path,
                    Token = entry.Token
                });
            }

            WhatsNew.Add(new()
            {
                Title = "New dialogs!",
                Icon = "\uE775",
                Tag = "LocAndAcc",
                Description = $"This release adds two brand new dialogs, both of which can also be launched via Help menu! One is the First Run dialog, which runs the first time this app is run. The second is the What's New dialog, a dialog that shows what has changed between builds."
            });
            WhatsNew.Add(new()
            {
                Title = "Redesigned app icon",
                Icon = "\uE914",
                Tag = "LocAndAcc",
                Description = $"This release also redesigns the app icon of UltraTextEdit UWP to make it look more professional, indicating the vision for the future of the app (to have more Word-like features and maybe become a Word competitor)"
            });
            WhatsNew.Add(new()
            {
                Title = "Time/Date",
                Icon = "\uE775",
                Tag = "LocAndAcc",
                Description = $"This release adds date and time insertion!"
            });
            WhatsNew.Add(new()
            {
                Title = "Tabs",
                Icon = "\uF7ED",
                Tag = "LocAndAcc",
                Description = $"This release finally adds Tabs, after several user feature requests and a lot of consideration about when they will be released!\nThey allow you to open multiple documents at once, like in the File Explorer.\nYou can also open the traditional untabbed UTE window by clicking on 'New Untabbed Window' in the app's File Menu. You can also launch a new Tabbed Window by clicking on 'New Tabbed Window' in the same menu."
            });

            //WhatsNew.Add(new()
            //{
            //Title = "Windows support changes",
            //Icon = "\uEC24",
            //Tag = "UTEInsider",
            //Description = $"With new Dev Channel builds of {Strings.Resources.AppName} starting with this one, there is no support for Windows 10 1809, 1903, and 1909 (but these Windows 10 versions will still be supported on the 22H2 Stable, Beta, and Release Preview branches). Only Windows 10 version 20H1 (2004) and later, and Windows 11 original release (21H2) and later are supported.\nThis build of {Strings.Resources.AppName} also increments the target Windows version to Windows 11, version 22H2 (build 22621)."
            //});

            IsListEmpty = list.Count <= 0;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame && rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private async void OpenFileLocation_Click(object sender, RoutedEventArgs e)
        {
            RecentlyUsedViewModel item = (sender as MenuFlyoutItem).Tag as RecentlyUsedViewModel;

            try
            {
                await Launcher.LaunchFolderPathAsync(item.Path.Replace(item.Name, ""));
            }
            catch (Exception e1)
            {
                System.Diagnostics.Debug.WriteLine($"An error occured while opening the folder, {e1.Message}");
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            RecentlyUsedViewModel item = (sender as MenuFlyoutItem).Tag as RecentlyUsedViewModel;
            Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Remove(item.Token);
            list.Remove(item);

            IsListEmpty = list.Count <= 0;
        }

        private void CopyLocation_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText((sender as MenuFlyoutItem).Tag as string);
            Clipboard.SetContent(dataPackage);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is WhatsNewItemViewModel item)
            {
                SelectedItem = item;

                FontIconWhatsNew.Glyph = item.Icon;
                TitleWhatsNew.Text = item.Title;
                DescWhatsNew.Text = item.Description;
            }
        }
    }
}
