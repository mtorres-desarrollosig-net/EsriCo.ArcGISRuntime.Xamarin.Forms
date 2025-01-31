﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EsriCo.ArcGISRuntime.Xamarin.Forms.UI {
  /// <summary>
  /// 
  /// </summary>
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class ProcessingView : ContentView {
    /// <summary>
    /// 
    /// </summary>
    public static readonly BindableProperty IsProcessingProperty = BindableProperty.Create(
      nameof(IsProcessing),
      typeof(bool),
      typeof(ProcessingView),
      defaultValue: false);

    /// <summary>
    /// 
    /// </summary>
    public bool IsProcessing {
      get => (bool)GetValue(IsProcessingProperty);
      set => SetValue(IsProcessingProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly BindableProperty ProcessingMessageProperty = BindableProperty.Create(
      nameof(ProcessingMessage),
      typeof(string),
      typeof(ProcessingView));

    /// <summary>
    /// 
    /// </summary>
    public string ProcessingMessage {
      get => (string)GetValue(ProcessingMessageProperty);
      set => SetValue(ProcessingMessageProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public ProcessingView() {
      try {
        InitializeComponent();
        ProcessingMessage = AppResources.ProcessingMessageLabelText;
      }
      catch(Exception ex) {
        Console.WriteLine(ex.Message);
      }
    }
  }
}