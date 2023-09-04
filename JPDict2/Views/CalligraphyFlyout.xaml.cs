using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using JPDict2.Core.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JPDict2.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalligraphyFlyout : Page
    {


        public string Character
        {
            get => (string)GetValue(CharacterProperty);
            set => SetValue(CharacterProperty, value);
        }

        // Using a DependencyProperty as the backing store for Character.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterProperty =
            DependencyProperty.Register("Character", typeof(string), typeof(CalligraphyFlyout), new PropertyMetadata(0));


        public List<KanjiGuideStep> StrokesGuide
        {
            get => (List<KanjiGuideStep>)GetValue(StrokesGuideProperty);
            set => SetValue(StrokesGuideProperty, value);
        }

        // Using a DependencyProperty as the backing store for StrokesGuide.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokesGuideProperty =
            DependencyProperty.Register("StrokesGuide", typeof(List<KanjiGuideStep>), typeof(CalligraphyFlyout), new PropertyMetadata(0));



        public CalligraphyFlyout()
        {
            this.InitializeComponent();
        }

        private void KanjiSvg_Loaded(object sender, RoutedEventArgs e)
        {
            var transform = new ScaleTransform();
            transform.ScaleX = 0.4;
            transform.ScaleY = 0.4;
            (sender as Microsoft.UI.Xaml.Shapes.Path).Data.Transform = transform;
        }
    }

}
