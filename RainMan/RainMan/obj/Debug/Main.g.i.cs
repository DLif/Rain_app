﻿

#pragma checksum "C:\Users\Denis\documents\visual studio 2013\Projects\RainMan\RainMan\Main.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "30F7DE30DA18C2B29BC4FD33A0DA73DF"
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
    partial class Main : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.ImageBrush backgroundImg; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.DataTemplate PredictionItemTemplate; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard CurrentGridFadeOut; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard CurrentGridShow; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard FillWaterStory; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.DoubleAnimation FillWaterAnimation; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.CommandBar commandBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton mapAppButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton rainAreas; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton routesAppBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.SymbolIcon RadarMapButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid LayoutRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid ContentRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid itemsGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.GridView predictionGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid CurrentGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock title; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.StackPanel avgRain; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Shapes.Rectangle waterRec; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock avgInfo; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock info; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///Main.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            backgroundImg = (global::Windows.UI.Xaml.Media.ImageBrush)this.FindName("backgroundImg");
            PredictionItemTemplate = (global::Windows.UI.Xaml.DataTemplate)this.FindName("PredictionItemTemplate");
            CurrentGridFadeOut = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("CurrentGridFadeOut");
            CurrentGridShow = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("CurrentGridShow");
            FillWaterStory = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("FillWaterStory");
            FillWaterAnimation = (global::Windows.UI.Xaml.Media.Animation.DoubleAnimation)this.FindName("FillWaterAnimation");
            commandBar = (global::Windows.UI.Xaml.Controls.CommandBar)this.FindName("commandBar");
            mapAppButton = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("mapAppButton");
            rainAreas = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("rainAreas");
            routesAppBar = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("routesAppBar");
            RadarMapButton = (global::Windows.UI.Xaml.Controls.SymbolIcon)this.FindName("RadarMapButton");
            LayoutRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("LayoutRoot");
            ContentRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("ContentRoot");
            itemsGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("itemsGrid");
            predictionGrid = (global::Windows.UI.Xaml.Controls.GridView)this.FindName("predictionGrid");
            CurrentGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("CurrentGrid");
            title = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("title");
            avgRain = (global::Windows.UI.Xaml.Controls.StackPanel)this.FindName("avgRain");
            waterRec = (global::Windows.UI.Xaml.Shapes.Rectangle)this.FindName("waterRec");
            avgInfo = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("avgInfo");
            info = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("info");
        }
    }
}



