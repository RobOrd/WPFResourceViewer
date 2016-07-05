using CODE.Framework.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.ResourceViewer
{
    public class IconWrapper
    {
        public string Name { get; set; }
        public ResourceDictionary Dictionary { get; set; }

        public Brush Icon
        {
            get { return (Brush)Dictionary[Name]; }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AttachResources();
            LoadIconsFromResource();
        }

        private void AttachResources()
        {
            var app = ConfigurationSettings.Settings["application"]; 
            var resource = ConfigurationSettings.Settings["resourceDictionary"];
            var split = app.Split('.');
            var appName = string.Join(".", split.Take(split.Count() - 1));

            resource = string.Format(@"pack://application:,,,/{0};component/{1}", appName, resource);

            System.Reflection.Assembly.LoadFrom(app);
            var rd = new ResourceDictionary();
            rd.Source = new Uri(resource);

            this.Resources.MergedDictionaries.Add(rd);
        }
        private void LoadIconsFromResource()
        {
            var list = new List<IconWrapper>();
            var keys = new List<string>();
            foreach (var key in Resources.MergedDictionaries[0].Keys)
                keys.Add(key.ToString());

            foreach (string key in keys.OrderBy(k => k))
                if (Resources.MergedDictionaries[0][key] is Brush)
                    list.Add(new IconWrapper { Name = key, Dictionary = Resources.MergedDictionaries[0] });

            icons.SelectionChanged += icons_SelectionChanged;
            icons.ItemsSource = list;
        }

        private void icons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            text.Text = (icons.SelectedItem as IconWrapper).Name;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var name = (icons.SelectedItem as IconWrapper).Name;

            Clipboard.SetText(name);
        }
    }
}
