﻿

#pragma checksum "C:\Users\Denis\documents\visual studio 2013\Projects\RainMan\RainMan\RainAmount.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C8E30755A696831996F5DC82411D55AE"
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
    partial class RainAmount : global::Windows.UI.Xaml.Controls.Page
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton acceptAppBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarButton undoAppBar; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.AppBarToggleButton enableAddress; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid LayoutRoot; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Maps.MapControl map; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid TipGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid overlayGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid addressGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard addressGridFadeIn; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Media.Animation.Storyboard addressGridFadeOut; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBox addressTextBox; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Grid RequestGrid; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button GoBtn; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button CancelButton; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.DatePicker date; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ProgressRing progress; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.TextBlock ResultText; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.ToggleSwitch usePredictions; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.Button predictionNumBtn; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.MenuFlyoutItem prediction30; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.MenuFlyoutItem prediction20; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private global::Windows.UI.Xaml.Controls.MenuFlyoutItem prediction10; 
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        private bool _contentLoaded;

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;
            global::Windows.UI.Xaml.Application.LoadComponent(this, new global::System.Uri("ms-appx:///RainAmount.xaml"), global::Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
 
            acceptAppBar = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("acceptAppBar");
            undoAppBar = (global::Windows.UI.Xaml.Controls.AppBarButton)this.FindName("undoAppBar");
            enableAddress = (global::Windows.UI.Xaml.Controls.AppBarToggleButton)this.FindName("enableAddress");
            LayoutRoot = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("LayoutRoot");
            map = (global::Windows.UI.Xaml.Controls.Maps.MapControl)this.FindName("map");
            TipGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("TipGrid");
            overlayGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("overlayGrid");
            addressGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("addressGrid");
            addressGridFadeIn = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("addressGridFadeIn");
            addressGridFadeOut = (global::Windows.UI.Xaml.Media.Animation.Storyboard)this.FindName("addressGridFadeOut");
            addressTextBox = (global::Windows.UI.Xaml.Controls.TextBox)this.FindName("addressTextBox");
            RequestGrid = (global::Windows.UI.Xaml.Controls.Grid)this.FindName("RequestGrid");
            GoBtn = (global::Windows.UI.Xaml.Controls.Button)this.FindName("GoBtn");
            CancelButton = (global::Windows.UI.Xaml.Controls.Button)this.FindName("CancelButton");
            date = (global::Windows.UI.Xaml.Controls.DatePicker)this.FindName("date");
            progress = (global::Windows.UI.Xaml.Controls.ProgressRing)this.FindName("progress");
            ResultText = (global::Windows.UI.Xaml.Controls.TextBlock)this.FindName("ResultText");
            usePredictions = (global::Windows.UI.Xaml.Controls.ToggleSwitch)this.FindName("usePredictions");
            predictionNumBtn = (global::Windows.UI.Xaml.Controls.Button)this.FindName("predictionNumBtn");
            prediction30 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)this.FindName("prediction30");
            prediction20 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)this.FindName("prediction20");
            prediction10 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)this.FindName("prediction10");
        }
    }
}


