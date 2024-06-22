using System;
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
                Title = "New codebase",
                Icon = "\uE943",
                Tag = "NewCodeVer",
                Description = $"{Strings.Resources.AppName} build 22621 (22H2 feature update) introduces a new codebase based on TowPad. It contains all the new features described below and more!"
            });

            WhatsNew.Add(new()
            {
                Title = "Home page",
                Icon = "\uEA8A",
                Tag = "HomePage",
                Description = $"Now you can see your recent files, and what's new in {Strings.Resources.AppName}!"
            });

            WhatsNew.Add(new()
            {
                Title = "Localization & Accessbility",
                Icon = "\uE774",
                Tag = "LocAndAcc",
                Description = $"You can contribute translations to {Strings.Resources.AppName}, and help make this app reach more countries! And elements in the app will be more accessible.\n\nNEW IN OCTOBER 2022 UPDATE:\nUltraTextEdit UWP has been translated into Czech, Slovak, Ukrainian, Hungarian, and Simplified Chinese\n\nNEW IN DECEMBER 2022 UPDATE:\nUsing UltraTextEdit UWP without a mouse just became easier! Use the new keyboard shortcuts/hotkeys to complete common actions in the app!\n\nNEW IN JUNE 2024 UPDATE: You can now enable spellchecking in Settings to allow UTE UWP to proofread your documents for potential spelling errors."
            });

            WhatsNew.Add(new()
            {
                Title = "Insider channels",
                Icon = "\uF1AD",
                Tag = "UTEInsider",
                Description = $"With {Strings.Resources.AppName} version 22H2, there are 3 Insider channels, instead of one Insider release at the end of a month. The Insider channels are the same as found on Windows (Dev, Beta, and Release Preview). The Insider channel you are in is indicated to the right of the app's title on the title bar."
            });

            WhatsNew.Add(new()
            {
                Title = "New Settings UI",
                Icon = "\uE713",
                Tag = "SettingsUI",
                Description = "On the redesigned Settings UI, you can set all UTE UWP settings as well as new settings from TowPad.\n\nNEW IN OCTOBER 2022 UPDATE:\nIcons for the Appearance section of the Settings menu added\n\nNEW IN JUNE 2023 UPDATE:\nThis release fixes some functionality of the Settings toggles. The values of the Settings toggles will now persist between openings of the Settings page."
            });

            WhatsNew.Add(new()
            {
                Title = "Compact mode",
                Icon = "\uE737",
                Tag = "CompactMode",
                Description = $"Now you can make {Strings.Resources.AppName} overlay over windows! (experimental)"
            });

            WhatsNew.Add(new()
            {
                Title = "Share",
                Icon = "\uE72D",
                Tag = "Share",
                Description = $"NEW IN DECEMBER 2022 UPDATE:\nSharing text from {Strings.Resources.AppName} to compatible apps is now available!"
            });

            WhatsNew.Add(new()
            {
                Title = "UI Updates",
                Icon = "\uE2B1",
                Tag = "UIU/UIR",
                Description = $"NEW IN DECEMBER 2022 UPDATE:\nThe Find/Replace Panel of the app got a Mica background!\n\nNEW IN OCTOBER 2023 UPDATE:\nThe color picker UI has been revamped, based on the one from TextPad"
            });

            WhatsNew.Add(new()
            {
                Title = "Table",
                Icon = "\uF261",
                Tag = "Table",
                Description = $"NEW IN FEBRUARY 2023 UPDATE: Now you can add tables to documents made in UTE UWP!\n(Only one per app run allowed for now though)\n\nNEW IN JUNE 2023 UPDATE:\nThis release adds a dialog which allows you to create tables of different sizes in your documents!"
            });

            WhatsNew.Add(new()
            {
                Title = "Comments",
                Icon = "\uE15F",
                Tag = "Comments",
                Description = $"NEW IN FEBRUARY 2023 UPDATE: Comments functionality is now available! \nJust tap or click the Comments buton to open the pane, then when you're finished, just go into the new Comments ribbon tab and click or tsp the button there to close the pane."
            });

            WhatsNew.Add(new()
            {
                Title = "Symbols",
                Icon = "\uED58",
                Tag = "Symbols",
                Description = $"NEW IN FEBRUARY 2023 UPDATE: Now you can add the multiplication and division symbols straight into your document from the app's new symbols menu!\n\nNEW IN JUNE 2024 UPDATE:\nThis release redesigns the symbol insertion experience to be more touch-friendly and introduces a More Symbols dialog to insert additional symbols."
            });

            WhatsNew.Add(new()
            {
                Title = "Time/Date",
                Icon = "\uE775",
                Tag = "TimeDate",
                Description = $"NEW IN JUNE 2023 UPDATE: This release adds date and time insertion!"
            });

            WhatsNew.Add(new()
            {
                Title = "Bulleting options/List Styles",
                Icon = "\uE133",
                Tag = "LocAndAcc",
                Description = $"NEW IN OCTOBER 2023 UPDATE: This release adds bulleting options back into UTE UWP!!"
            });

            WhatsNew.Add(new()
            {
                Title = "Text Templates",
                Icon = "\uE8D2",
                Tag = "LocAndAcc",
                Description = $"NEW IN JUNE 2024 UPDATE: This release allows you to use predefined text styles to create documents quicker and more easily!!"
            });

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
