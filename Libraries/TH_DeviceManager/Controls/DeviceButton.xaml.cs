﻿// Copyright (c) 2015 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Xml;

using Microsoft.Win32;

using TH_Configuration;

namespace TH_DeviceManager.Controls
{
    /// <summary>
    /// Interaction logic for DeviceButton.xaml
    /// </summary>
    public partial class DeviceButton : UserControl
    {
        public DeviceButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public DeviceManager devicemanager;

        public object Parent { get; set; }

        const System.Windows.Threading.DispatcherPriority contextidle = System.Windows.Threading.DispatcherPriority.ContextIdle;

        Configuration config;
        public Configuration Config
        {
            get { return config; }
            set
            {
                config = value;

                this.Dispatcher.BeginInvoke(new Action<Configuration>(SetDeviceConfig), contextidle, new object[] { config });
            }
        }

        void SetDeviceConfig(Configuration config)
        {
            if (config != null)
            {
                if (devicemanager != null)
                {
                    if (devicemanager.ManagerType == DeviceManagerType.Client) DeviceEnabled = config.ClientEnabled;
                    else if (devicemanager.ManagerType == DeviceManagerType.Server) DeviceEnabled = config.ServerEnabled;
                }
                //DeviceEnabled = config.Enabled;
                enabled_CHK.IsChecked = DeviceEnabled;

                //Shared = config.Shared;

                Description = config.Description.Description;
                Manufacturer = config.Description.Manufacturer;
                Model = config.Description.Model;
                Serial = config.Description.Serial;
                Id = config.Description.Device_ID;
            }
        }


        public bool DeviceEnabled
        {
            get { return (bool)GetValue(DeviceEnabledProperty); }
            set { SetValue(DeviceEnabledProperty, value); }
        }

        public static readonly DependencyProperty DeviceEnabledProperty =
            DependencyProperty.Register("DeviceEnabled", typeof(bool), typeof(DeviceButton), new PropertyMetadata(false));


        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(DeviceButton), new PropertyMetadata(null));


        public string Manufacturer
        {
            get { return (string)GetValue(ManufacturerProperty); }
            set { SetValue(ManufacturerProperty, value); }
        }

        public static readonly DependencyProperty ManufacturerProperty =
            DependencyProperty.Register("Manufacturer", typeof(string), typeof(DeviceButton), new PropertyMetadata(null));


        public string Model
        {
            get { return (string)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(string), typeof(DeviceButton), new PropertyMetadata(null));


        public string Serial
        {
            get { return (string)GetValue(SerialProperty); }
            set { SetValue(SerialProperty, value); }
        }

        public static readonly DependencyProperty SerialProperty =
            DependencyProperty.Register("Serial", typeof(string), typeof(DeviceButton), new PropertyMetadata(null));


        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(DeviceButton), new PropertyMetadata(null));


        public bool Shared
        {
            get { return (bool)GetValue(SharedProperty); }
            set { SetValue(SharedProperty, value); }
        }

        public static readonly DependencyProperty SharedProperty =
            DependencyProperty.Register("Shared", typeof(bool), typeof(DeviceButton), new PropertyMetadata(false));

        


        public delegate void Clicked_Handler(DeviceButton bt);
        public event Clicked_Handler ShareClicked;

        private void Share_Clicked(TH_WPF.Button bt)
        {
            if (ShareClicked != null) ShareClicked(this);
        }

        public event Clicked_Handler Clicked;

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (Clicked != null) Clicked(this);
            } 
        }

        public event Clicked_Handler RemoveClicked;

        private void Remove_Clicked(TH_WPF.Button bt)
        {
            if (RemoveClicked != null) RemoveClicked(this);
        }

        public event Clicked_Handler Enabled;
        public event Clicked_Handler Disabled;

        private void enabled_CHK_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            if (chk.IsMouseCaptured || chk.IsKeyboardFocused)
            {
                if (Enabled != null) Enabled(this);
            }
        }

        private void enabled_CHK_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            if (chk.IsMouseCaptured || chk.IsKeyboardFocused)
            {
                if (Disabled != null) Disabled(this);
            }
        }

        public event Clicked_Handler CopyClicked;

        private void Copy_Clicked(TH_WPF.Button bt)
        {
            if (CopyClicked != null) CopyClicked(this);
        }



        private void SaveToFile_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (config != null)
            {
                
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = TH_Global.FileLocations.TrakHoundTemp;
                saveFileDialog.Filter = "XML File (*.xml)|*.xml";
                saveFileDialog.Title = "Save Device Configuration File";
                saveFileDialog.FileName = config.Description.Description + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";
                bool? result = saveFileDialog.ShowDialog();
                if (result == true)
                {
                    string savePath = saveFileDialog.FileName;

                    if (savePath != null)
                    {
                        config.ConfigurationXML.Save(savePath);

                        System.Diagnostics.Process.Start("explorer.exe", savePath);
                    }
                }
            }
        }

        //Thread saveDevice_THREAD;

        //void SaveDevice(Configuration config)
        //{
        //    if (config != null)
        //    {
        //        DeviceLoading = true;

        //        if (saveDevice_THREAD != null) saveDevice_THREAD.Abort();

        //        saveDevice_THREAD = new Thread(new ParameterizedThreadStart(SaveDevice_Worker));
        //        saveDevice_THREAD.Start(config);
        //    }
        //}

        //void SaveDevice_Worker(object o)
        //{
        //    Configuration config = (Configuration)o;

        //    DataTable dt = TH_Configuration.Converter.XMLToTable(config.ConfigurationXML);
        //    if (dt != null)
        //    {
        //        dt.TableName = config.TableName;

        //        if (ConfigurationPages != null)
        //        {
        //            foreach (ConfigurationPage page in ConfigurationPages)
        //            {
        //                this.Dispatcher.BeginInvoke(new Action<DataTable, ConfigurationPage>(SelectDevice_GUI), background, new object[] { dt, page });
        //            }
        //        }
        //    }

        //    this.Dispatcher.BeginInvoke(new Action<DataTable>(SelectDevice_Finished), background, new object[] { dt });
        //}



        public bool EnableLoading
        {
            get { return (bool)GetValue(EnableLoadingProperty); }
            set { SetValue(EnableLoadingProperty, value); }
        }

        public static readonly DependencyProperty EnableLoadingProperty =
            DependencyProperty.Register("EnableLoading", typeof(bool), typeof(DeviceButton), new PropertyMetadata(false));




    }
}
