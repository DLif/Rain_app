﻿

#pragma checksum "C:\Users\Denis\documents\visual studio 2013\Projects\RainMan\RainMan\RadarPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "35FB25336962C9C4090A071D66B841F7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RainMan
{
    partial class RadarPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 15 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AppBarButton_Click_1;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 51 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AppBarButton_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 18 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.FlyoutBase)(target)).Opening += this.Flyout_Opening;
                 #line default
                 #line hidden
                #line 18 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.FlyoutBase)(target)).Closed += this.Flyout_Closed;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 39 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).GotFocus += this.location_GotFocus;
                 #line default
                 #line hidden
                #line 39 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).KeyDown += this.location_KeyDown;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 40 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.goButton_Click;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 75 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.map_Loaded;
                 #line default
                 #line hidden
                #line 75 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Maps.MapControl)(target)).ZoomLevelChanged += this.map_ZoomLevelChanged;
                 #line default
                 #line hidden
                #line 75 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Maps.MapControl)(target)).MapTapped += this.map_MapTapped;
                 #line default
                 #line hidden
                #line 75 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.map_Tapped;
                 #line default
                 #line hidden
                #line 75 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Maps.MapControl)(target)).PitchChanged += this.map_PitchChanged;
                 #line default
                 #line hidden
                #line 75 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Maps.MapControl)(target)).HeadingChanged += this.map_HeadingChanged;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 104 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerEntered += this.bottomPanel_PointerEntered;
                 #line default
                 #line hidden
                #line 104 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerExited += this.bottomPanel_PointerExited;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 137 "..\..\RadarPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.RangeBase)(target)).ValueChanged += this.timeSlider_ValueChanged;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


