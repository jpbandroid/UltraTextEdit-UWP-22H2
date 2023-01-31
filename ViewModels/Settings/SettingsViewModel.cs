﻿using Microsoft.Graphics.Canvas.Text;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using UltraTextEdit_UWP.Helpers;
using UltraTextEdit_UWP.Views.Settings;

namespace UltraTextEdit_UWP.ViewModels
{
    public class SettingsViewModel : SettingsManager
    {
        public SettingsViewModel()
        {
            Languages = new ObservableCollection<Language>
{
    new Language { DisplayName = "English", LanguageCode = "en-US" },
    new Language { DisplayName = "简体中文", LanguageCode = "zh-CN" }
};
        }


        private ObservableCollection<Language> _languages;
        private Language _selectedLanguage;

        public ObservableCollection<Language> Languages
        {
            get => _languages;
            set => _languages = value;
        }

        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                this.PrimaryLanguageOverride = value.LanguageCode;
            }
        }

        public string PrimaryLanguageOverride
        {
            get => Get("Appearance", nameof(PrimaryLanguageOverride), "en-US");
            set
            {
                Set("Appearance", nameof(PrimaryLanguageOverride), value);
            }
        }

        public List<string> Fonts
        {
            get
            {
                return CanvasTextFormat.GetSystemFontFamilies().OrderBy(f => f).ToList();
            }
        }

        #region Appearance
        public int DocumentViewPadding
        {
            get => Get("Appearance", nameof(DocumentViewPadding), 11);
            set => Set("Appearance", nameof(DocumentViewPadding), value);
        }

        public string DefaultFont
        {
            get => Get("Appearance", nameof(DefaultFont), "Segoe UI");
            set => Set("Appearance", nameof(DefaultFont), value);
        }

        // Modes:
        // 0. No wrap
        // 1. Wrap
        // 2. Wrap whole words

        public int TextWrapping
        {
            get => Get("Appearance", nameof(TextWrapping), 0);
            set => Set("Appearance", nameof(TextWrapping), value);
        }

        // Modes:
        // 0. Light
        // 1. Dark
        // 2. Default

        public int Theme
        {
            get => Get("Appearance", nameof(Theme), 2);
            set
            {
                Set("Appearance", nameof(Theme), value);
                switch (value)
                {
                    case 0:
                        (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Light;
                        break;
                    case 1:
                        (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Dark;
                        break;
                    case 2:
                        (Window.Current.Content as Frame).RequestedTheme = ElementTheme.Default;
                        break;
                }
            }
        }
        #endregion
    }
}