﻿

#pragma checksum "C:\Users\Denis\documents\visual studio 2013\Projects\RainMan\RainMan\RoutePredictions.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E2F15424B9B8C88CED556734A555D730"
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
    partial class RoutePredictions : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 40 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Controls.AppBar)(target)).Opened += this.CommandBar_Opened;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 41 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Checked += this.appInfo_Checked;
                 #line default
                 #line hidden
                #line 41 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target)).Unchecked += this.appInfo_Unchecked;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 42 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.routesAppBar_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 163 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.SuggestionGrid_Tapped;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 199 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Media.Animation.Timeline)(target)).Completed += this.fadeOutPaths_Completed;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 225 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Controls.ListViewBase)(target)).ItemClick += this.listPathNames_ItemClick;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 169 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Media.Animation.Timeline)(target)).Completed += this.fadeOutStoryBoardSuggestion_Completed;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 138 "..\..\RoutePredictions.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.RangeBase)(target)).ValueChanged += this.exitTimeSlider_ValueChanged;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


