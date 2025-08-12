using Core.Installers.Base;
using Elements.Widgets.ActiveUsers.Presenter.Components.Diagram.Factory;
using Elements.Widgets.ActiveUsers.Presenter.Components.Factory;
using Elements.Widgets.Calendar.Presenter.Components.View.DataItems.Factory;
using Elements.Widgets.Calendar.Presenter.Components.View.Factory;
using Elements.Widgets.Calendar.Presenter.Components.View.TimeScale.Factory;
using Elements.Widgets.Camera.Presenter.Components.Factory;
using Elements.Widgets.Common.Controller;
using Elements.Widgets.EventsStat.Presenter.Components.Factory;
using Elements.Widgets.IncidentYear.Presenter.Components.View.Diagram.Factory;
using Elements.Widgets.IncidentYear.Presenter.Components.View.Factory;
using Elements.Widgets.KcTable.Presenter.Components.Factory;
using Elements.Widgets.Map.Controller;
using Elements.Widgets.Map.Controller.Parser;
using Elements.Widgets.Map.Presenter.Components.Panels.Breadcrumbs.Items.Factory;
using Elements.Widgets.Map.Presenter.Components.Panels.Legends.Items.Factory;
using Elements.Widgets.Map.Presenter.Components.Panels.LegendSubLayers.Items.Factory;
using Elements.Widgets.Map.Presenter.Components.Panels.Regions.Items.Factory;
using Elements.Widgets.Map.Presenter.Components.Screen.Indicators.Factory;
using Elements.Widgets.Map.Presenter.Components.Screen.Source.Factory;
using Elements.Widgets.Map.Presenter.Components.Screen.Source.Layers.GeoPoints.Factory;
using Elements.Widgets.Map.Presenter.Components.Screen.Source.Model3D.Factory;
using Elements.Widgets.News.Presenter.Components.Factory;
using Elements.Widgets.Vks.Presenter.Components.Diagram.Factory;
using Elements.Widgets.Vks.Presenter.Components.Factory;
using Elements.Widgets.Weather.Presenter.Components.Factory;

namespace Installers
{
    public class ExtendedElementAddonsInstaller : AutoResolvingInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<WidgetsUpdateController>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<CameraViewFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<CalendarViewFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<CalendarTimeScaleItemFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<CalendarDataItemFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<WeatherCameraViewFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<WeatherViewFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<NewsArticleFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<IncidentDiagramComponentFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<IncidentDiagramSegmentFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<IncidentYearViewFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<KcDataItemFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<ActiveUsersDiagramComponentFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<ActiveUsersDiagramFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<ActiveUsersViewFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<VksDiagramComponentFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<VksDiagramFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<VksViewFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<EventsStatDiagramComponentFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<EventsStatLegendFactory>().AsSingle().NonLazy();
            
            Container.BindInterfacesTo<MapInfoParser>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapInfoProvider>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapSourceFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapSourcesFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapModel3DFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapGeoPointFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapRegionIndicatorFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapBreadcrumbItemFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapRegionItemFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapLegendItemFactory>().AsSingle().NonLazy();
            Container.BindInterfacesTo<MapLegendSubLayerItemFactory>().AsSingle().NonLazy();
            
            base.InstallBindings();
        }
        
        protected override void ResolveAll()
        {
            Resolve<IWidgetsUpdateController>();
            Resolve<IWidgetsUpdatingRegister>();
            
            Resolve<ICameraViewFactory>();
            
            Resolve<ICalendarViewFactory>();
            Resolve<ICalendarTimeScaleItemFactory>();
            Resolve<ICalendarDataItemFactory>();
            
            Resolve<IWeatherCameraViewFactory>();
            Resolve<IWeatherViewFactory>();
            
            Resolve<INewsArticleFactory>();
            
            Resolve<IIncidentDiagramComponentFactory>();
            Resolve<IIncidentDiagramSegmentFactory>();
            
            Resolve<IIncidentYearViewFactory>();
            
            Resolve<IKcDataItemFactory>();
            
            Resolve<IActiveUsersDiagramComponentFactory>();
            Resolve<IActiveUsersDiagramFactory>();
            Resolve<IActiveUsersViewFactory>();
            
            Resolve<IVksDiagramComponentFactory>();
            Resolve<IVksDiagramFactory>();
            
            Resolve<IVksViewFactory>();
            
            Resolve<IEventsStatDiagramComponentFactory>();
            Resolve<IEventsStatLegendFactory>();
            
            Resolve<IMapInfoParser>();
            Resolve<IMapInfoProvider>();
            Resolve<IMapSourceFactory>();
            Resolve<IMapSourcesFactory>();
            Resolve<IMapModel3DFactory>();
            Resolve<IMapGeoPointFactory>();
            Resolve<IMapRegionIndicatorFactory>();
            Resolve<IMapBreadcrumbItemFactory>();
            Resolve<IMapRegionItemFactory>();
            Resolve<IMapLegendItemFactory>();
            Resolve<IMapLegendSubLayerItemFactory>();
            
            base.ResolveAll();
        }
    }
}