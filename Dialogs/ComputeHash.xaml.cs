using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UltraTextEdit_UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UltraTextEdit_UWP.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComputeHash : Page
    {
        public ComputeHash()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = (BasePage.Current.Content as Frame).Content as MainPage;
            string docText = mainPage.docText;
            docText = EncryptorsDecryptors.Base64Encode(docText);
            base64_result.Text = docText;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = (BasePage.Current.Content as Frame).Content as MainPage;
            string docText = mainPage.docText;
            docText = EncryptorsDecryptors.Base64Decode(docText);
            base64_result.Text = docText;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = (BasePage.Current.Content as Frame).Content as MainPage;
            string docText = mainPage.docText;
            docText = EncryptorsDecryptors.SHA1Encrypt(docText);
            sha1_result.Text = docText;
        }
    }
}
