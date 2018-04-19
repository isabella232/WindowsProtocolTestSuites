﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Protocols.TestManager.SMBDPlugin.Detector;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Protocols.TestManager.SMBDPlugin
{

    public partial class DetectionResultControl : UserControl
    {
        public DetectionResultControl()
        {
            InitializeComponent();
        }

        public void LoadDetectionInfo(DetectionInfo detectionInfo)
        {
            this.info = detectionInfo;

            ResultMapList.ItemsSource = resultItemMapList;
        }

        #region Properties

        private DetectionInfo info = null;

        private const string dialectsDescription = "\"Max Supported Dialect\" is the selected one in the Negotiate Response by server when a Negotiate Request is sent to SUT with Dialects Smb2.002, Smb2.1, Smb3.0 and Smb3.02.";
        private const string capabilitiesDescription = "\"Capabilities\" are found supported or not supported by analyzing the flags set in Negotiate Response when a Negotiate Request is sent to SUT with all defined flags in TD set in Capabilities field.";
        private const string ioctlCodesDescription = "\"IoCtl Codes\" are found supported or not supported by analyzing IOCTL Responses when the following IOCTL Requests are sent to SUT.";
        private const string createContextsDescription = "\"Creat Contexts\" are found supported or not supported by analyzing Create Responses when the Create Requests with the following create contexts are sent to SUT.";
        private const string rsvdDescription = "\"RSVD Implementation\" is detected by sending Create Request with SVHDX_OPEN_DEVICE_CONTEXT\\SVHDX_OPEN_DEVICE_CONTEXT_V2.";
        private const string sqosDescription = "\"SQOS Implementation\" is detected by sending SQOS get status request.";


        private ResultItemMap dialectsItems = new ResultItemMap() { Header = "Max Smb Version Supported", Description = dialectsDescription };
        private ResultItemMap capabilitiesItems = new ResultItemMap() { Header = "Capabilities", Description = capabilitiesDescription };
        private ResultItemMap ioctlCodesItems = new ResultItemMap() { Header = "IoCtl Codes", Description = ioctlCodesDescription };
        private ResultItemMap createContextsItems = new ResultItemMap() { Header = "Create Contexts", Description = createContextsDescription };

        private ResultItemMap rsvdItems = new ResultItemMap() { Header = "Remote Shared Virtual Disk (RSVD)", Description = rsvdDescription };
        private ResultItemMap sqosItems = new ResultItemMap() { Header = "Storage Quality of Service (SQOS)", Description = sqosDescription };

        private List<ResultItemMap> resultItemMapList = new List<ResultItemMap>();

        #endregion

        #region Private functions

  
        private void AddResultItem(ref ResultItemMap resultItemMap, string value, DetectResult result)
        {
            string imagePath = string.Empty;
            switch (result)
            {
                case DetectResult.Supported:
                    imagePath = "/SMBDPlugin;component/Icons/supported.png";
                    break;
                case DetectResult.UnSupported:
                    imagePath = "/SMBDPlugin;component/Icons/unsupported.png";
                    break;
                case DetectResult.DetectFail:
                    imagePath = "/SMBDPlugin;component/Icons/undetected.png";
                    break;
                default:
                    break;
            }

            ResultItem item = new ResultItem() { DetectedResult = result, ImageUrl = imagePath, Name = value };
            resultItemMap.ResultItemList.Add(item);
        }

       
        #endregion

        #region Private events

        private void MapSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox selectedMap = sender as ListBox;

            //Keep all map headers unselected
            selectedMap.UnselectAll();
        }

        private void ItemSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox selectedList = sender as ListBox;

            if (selectedList.SelectedIndex != -1)
            {
                SetOtherItemsUnselected(sender);

                ResultItem tempItem = (ResultItem)selectedList.SelectedItem;

                if (tempItem.DetectedResult == DetectResult.UnSupported)
                {
                    this.ItemDescription.Text = tempItem.Name + " is found not supported after detection";
                    return;
                }
                //if (!info.detectExceptions.ContainsKey(tempItem.Name))
                //{
                //    this.ItemDescription.Text = tempItem.Name + " is found supported after detection";
                //    return;
                //}
                //string log = info.detectExceptions[tempItem.Name];
                //if (!string.IsNullOrEmpty(log))
                //{
                //    this.ItemDescription.Text = log;
                //}
            }
        }

        private void SetOtherItemsUnselected(object sender)
        {
            ListBox selectedList = sender as ListBox;
            ResultItem selectedItem = (ResultItem)selectedList.SelectedItem;

            foreach (object obj in this.ResultMapList.Items)
            {
                //Find the controls in the DataTemplate with the help of VisualTreeHelper class
                ListBoxItem lbi = this.ResultMapList.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
                ContentPresenter tempContentPresenter = FindVisualChild<ContentPresenter>(lbi);
                if (tempContentPresenter != null)
                {
                    DataTemplate tempDataTemplate = tempContentPresenter.ContentTemplate;
                    Expander mapHeader = tempDataTemplate.FindName("ResultMapHeader", tempContentPresenter) as Expander;
                    ListBox itemList = tempDataTemplate.FindName("ResultItemList", tempContentPresenter) as ListBox;
                    
                    //Keep the current selection
                    if (!itemList.Items.Contains(selectedItem))
                        itemList.UnselectAll();
                }
            }
        }

        private void ResultMapHeader_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander expander = sender as Expander;

            foreach (object obj in this.ResultMapList.Items)
            {
                ListBoxItem lbi = this.ResultMapList.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;
                ContentPresenter tempContentPresenter = FindVisualChild<ContentPresenter>(lbi);
                if (tempContentPresenter != null)
                {
                    DataTemplate tempDataTemplate = tempContentPresenter.ContentTemplate;
                    Expander mapHeader = tempDataTemplate.FindName("ResultMapHeader", tempContentPresenter) as Expander;
                    ListBox itemList = tempDataTemplate.FindName("ResultItemList", tempContentPresenter) as ListBox;

                    //Find the target list and clear the selection
                    if (expander.Header == mapHeader.Header)
                    {
                        if (itemList.SelectedIndex != -1)
                            this.ItemDescription.Text = string.Empty;

                        itemList.UnselectAll();
                        break;
                    }
                }
            }
        }

        private void ResultMapHeader_MouseEnter(object sender, MouseEventArgs e)
        {
            Expander mapHeader = sender as Expander;
            foreach (ResultItemMap map in resultItemMapList)
            {
                if (map.Header == mapHeader.Header.ToString())
                {
                    this.ItemDescription.Visibility = System.Windows.Visibility.Collapsed;
                    this.MapDescription.Visibility = System.Windows.Visibility.Visible;
                    this.MapDescription.Text = map.Description;
                }
            }
        }

        private void ResultMapHeader_MouseLeave(object sender, MouseEventArgs e)
        {
            this.MapDescription.Visibility = System.Windows.Visibility.Collapsed;
            this.ItemDescription.Visibility = System.Windows.Visibility.Visible;
        }

        private void ResultMapList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.MapDescription.Visibility = System.Windows.Visibility.Collapsed;
            this.ItemDescription.Visibility = System.Windows.Visibility.Visible;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = sender as ScrollViewer;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion

    }
}
