﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Xamarin.Forms;

using EsriCo.ArcGISRuntime.Xamarin.Forms.Model;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EsriCo.ArcGISRuntime.Xamarin.Forms.UI
{
  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class LayerListPanelView : ListPanelView
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly BindableProperty MapViewProperty = BindableProperty.Create(
      nameof(MapView),
      typeof(MapView),
      typeof(LayerListPanelView),
      propertyChanged: OnMapViewPropertyChanged);

    /// <summary>
    /// 
    /// </summary>
    public MapView MapView
    {
      get => (MapView)GetValue(MapViewProperty);
      set => SetValue(MapViewProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private static void OnMapViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
      var panelView = bindable as LayerListPanelView;
      if(newValue is MapView newMapView)
      {
        if(newMapView.Map != null)
        {
          panelView.SetMap(newMapView.Map);
        }
        else
        {
          newMapView.PropertyChanged += (s, e) =>
          {
            if(e.PropertyName == nameof(newMapView.Map) && newMapView.Map != null)
            {
              panelView.SetMap(newMapView.Map);
            }
          };
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private Map Map { get; set; }

    /// <summary>
    /// 
    /// </summary>
    private ObservableCollection<LayerInfos> _layerInfosList;

    /// <summary>
    /// 
    /// </summary>
    public ObservableCollection<LayerInfos> LayerInfosList
    {
      get => _layerInfosList;
      set
      {
        _layerInfosList = value;
        OnPropertyChanged(nameof(LayerInfosList));
      }
    }

    private bool CollectionHandlerAdded;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    public async void SetMap(Map map)
    {
      if(map != null)
      {
        Map = map;
        if(map.OperationalLayers.Count > 0)
        {
          await SetLayerInfos();
        }
        else
        {
          if(!CollectionHandlerAdded)
          {
            Map.OperationalLayers.CollectionChanged += async (o, e) =>
            {
              await SetLayerInfos();
            };
            CollectionHandlerAdded = true;
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task SetLayerInfosFromLoadedMap()
    {
      var layerInfos = await Device.InvokeOnMainThreadAsync(() => GetLayerInfosFromLoadedMap());
      LayerInfosList = layerInfos != null ?
        new ObservableCollection<LayerInfos>(layerInfos) :
        null;
    }

    /// <summary>
    /// 
    /// </summary>
    private async Task<List<LayerInfos>> GetLayerInfosFromLoadedMap()
    {
      var listLayerInfos = new List<LayerInfos>();

      foreach(var ol in Map.OperationalLayers)
      {
        var layerInfos = new LayerInfos()
        {
          GroupLayerInfo = new LayerInfo { Layer = ol }
        };
        ol.SublayerContents
              .ToList()
              .ForEach(sl =>
              {
                layerInfos.SubLayerInfos.Add(new LayerInfo()
                {
                  ParentInfo = layerInfos.GroupLayerInfo,
                  Layer = sl as Layer
                });
              });
        listLayerInfos.Add(layerInfos);

        var legendInfos = await layerInfos.GroupLayerInfo.Layer.GetLegendInfosAsync();
        await layerInfos.GroupLayerInfo.SetLegendInfos(legendInfos);

        foreach(var sli in layerInfos.SubLayerInfos)
        {
          var subLegendInfos = await sli.Layer.GetLegendInfosAsync();
          await sli.SetLegendInfos(subLegendInfos);
        }
      }
      return listLayerInfos;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private async Task SetLayerInfos()
    {
      if(Map != null)
      {
        if(Map.LoadStatus != LoadStatus.Loaded)
        {
          Map.Loaded += async (o, e) =>
          {
            await SetLayerInfosFromLoadedMap();
          };
          await Map.LoadAsync();
        }
        else
        {
          await SetLayerInfosFromLoadedMap();
        }
      }
    }

    public LayerListPanelView() => InitializeComponent();
  }
}