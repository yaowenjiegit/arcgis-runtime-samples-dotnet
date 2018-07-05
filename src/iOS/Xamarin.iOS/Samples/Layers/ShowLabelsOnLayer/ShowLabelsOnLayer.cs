// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using System;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using Foundation;
using UIKit;

namespace ArcGISRuntime.Samples.ShowLabelsOnLayer
{
    [Register("ShowLabelsOnLayer")]
    [ArcGISRuntime.Samples.Shared.Attributes.Sample(
        "Show labels on layer",
        "Layers",
        "This sample demonstrates how to show labels on a feature layer",
        "The labeling of the names on the US Highways layer is accomplished by supplying a JSON string to the FeatureLayer's LabelDefinition. The JSON is based on the new ArcGIS web map specification.",
        "")]
    public class ShowLabelsOnLayer : UIViewController
    {
        // Create and hold a reference to the MapView.
        private readonly MapView _myMapView = new MapView();

        public ShowLabelsOnLayer()
        {
            Title = "Show labels on layer";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CreateLayout();
            Initialize();
        }

        public override void ViewDidLayoutSubviews()
        {
            try
            {
                nfloat topMargin = NavigationController.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height;

                // Reposition controls.
                _myMapView.Frame = new CoreGraphics.CGRect(0, 0, View.Bounds.Width, View.Bounds.Height);
                _myMapView.ViewInsets = new UIEdgeInsets(topMargin, 0, 0, 0);

                base.ViewDidLayoutSubviews();
            }
            // Needed to prevent crash when NavigationController is null. This happens sometimes when switching between samples.
            catch (NullReferenceException)
            {
            }
        }

        private void CreateLayout()
        {
            // Add MapView to the page.
            View.AddSubviews(_myMapView);
        }

        private async void Initialize()
        {
            // Create a map with a light gray canvas basemap.
            Map sampleMap = new Map(Basemap.CreateLightGrayCanvas())
            {
                InitialViewpoint = new Viewpoint(new MapPoint(-100.175709, 39.221225, SpatialReferences.Wgs84), 20000000)
            };

            // Assign the map to the MapView.
            _myMapView.Map = sampleMap;

            // Define the URL string for the US highways feature layer.
            string highwaysUrlString = "http://sampleserver6.arcgisonline.com/arcgis/rest/services/USA/MapServer/1";

            // Create a service feature table from the URL to the US highways feature service.
            ServiceFeatureTable highwaysServiceFeatureTable = new ServiceFeatureTable(new Uri(highwaysUrlString));

            // Create a feature layer from the service feature table.
            FeatureLayer highwaysFeatureLayer = new FeatureLayer(highwaysServiceFeatureTable);

            // Add the US highways feature layer to the operations layers collection of the map.
            sampleMap.OperationalLayers.Add(highwaysFeatureLayer);

            // Load the US highways feature layer - this way we can obtain it's extent.
            await highwaysFeatureLayer.LoadAsync();

            // Help regarding the JSON syntax for defining the LabelDefinition.FromJson syntax can be found here:
            // https://developers.arcgis.com/web-map-specification/objects/labelingInfo/
            // This particular JSON string will have the following characteristics:
            // (1) The 'labelExpressionInfo' defines that the label text displayed comes from the field 'rte_num1' in the 
            //     feature service and will be prefaced with an "I -". Example: "I - 10", "I - 15", "I - 95", etc.
            // (2) The 'labelPlacement' will be placed above and along the highway polyline segment.
            // (3) The 'where' clause restricts the labels to be displayed that has valid (non-empty) data. Empty data
            //     for this service has a single blank space in the 'rte_num1' field.
            // (4) The 'symbol' for the labeled text will be blue with a yellow halo.
            string theJsonString =
                @"{
                    ""labelExpressionInfo"":{""expression"":""'I - ' + $feature.rte_num1""},
                    ""labelPlacement"":""esriServerLinePlacementAboveAlong"",
                    ""where"":""rte_num1 <> ' '"",
                    ""symbol"":
                        { 
                            ""angle"":0,
                            ""backgroundColor"":[0,0,0,0],
                            ""borderLineColor"":[0,0,0,0],
                            ""borderLineSize"":0,
                            ""color"":[0,0,255,255],
                            ""font"":
                                {
                                    ""decoration"":""none"",
                                    ""size"":15,
                                    ""style"":""normal"",
                                    ""weight"":""normal""
                                },
                            ""haloColor"":[255,255,0,255],
                            ""haloSize"":1.5,
                            ""horizontalAlignment"":""center"",
                            ""kerning"":false,
                            ""type"":""esriTS"",
                            ""verticalAlignment"":""middle"",
                            ""xoffset"":0,
                            ""yoffset"":0
                        }
               }";

            // Create a label definition from the JSON string. 
            LabelDefinition highwaysLabelDefinition = LabelDefinition.FromJson(theJsonString);

            // Add the label definition to the feature layer's label definition collection.
            highwaysFeatureLayer.LabelDefinitions.Add(highwaysLabelDefinition);

            // Enable the visibility of labels to be seen.
            highwaysFeatureLayer.LabelsEnabled = true;
        }
    }
}