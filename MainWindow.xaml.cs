using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Gundam.Spike.ScreenInfo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Screen> _ScreensCollection;
        public ObservableCollection<Screen> ScreensCollection
        {
            get
            {
                if(_ScreensCollection == null)
                {
                    _ScreensCollection = new ObservableCollection<Screen>(Screen.AllScreens);
                }
                return _ScreensCollection;
            }
            set
            {
                _ScreensCollection = value;
                NotifyChanged(() => ScreensCollection);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyChanged<T>(Expression<Func<T>> propertyName)
        {
            if (PropertyChanged != null)
            {
                var memberExpression = propertyName.Body as MemberExpression;
                if (memberExpression != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
            }
        }
    }
}
