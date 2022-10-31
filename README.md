# How to Create a Custom Toggle Button

Here in this blog I have shown how to create a Ribbon Toggle Button using AutoCAD Ribbon Runtime API.

https://adndevblog.typepad.com/autocad/2015/03/how-to-use-toggle-button-ribbon-api.html

This code sample shows how can we register a custom ribbon control on AutoCAD Ribbon bar, to illustrate I created a simple toggle button, but however this logic can be extended to any other fancy controls.

### Logic:

- Create a [ResourceDictionary ]() to hold resources and controls in XAML

- Implement the binding logic to manage the state of ToggleButton.
  
  - Create a SystemVariable to hold the state of ToggleButton
  
  ```csharp
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
          {
              return IsChecked(value);
          }
          public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
          {
              bool isOn = (bool)value;
              if (isOn && !string.IsNullOrEmpty(OnMacro))
                  RunMacro(OnMacro);
              else if (!isOn && !string.IsNullOrEmpty(OffMacro))
                  RunMacro(OffMacro);
              return DataBindings.DoNothing;
          }
  ```

- Load this XAML in to AutoCAD Runtime, so this  custom dictionary gets merged in to the AutoCAD  main resource dictionary and you will see a unfied UX.

- If you are preparing CUIX using CUI editor, make sure, the `key `of ToggleButton should be same as the `Id` of ToggleButton in CUI. For example "XyzToggleButton"

```csharp
string menuName = (string)Application.GetSystemVariable("MENUNAME");           
            CustomizationSection cs = new CustomizationSection(menuName + ".cuix");
            var ribbonRoot = cs.MenuGroup.RibbonRoot;
            var homeTab = ribbonRoot.FindTab("ID_TabHome");
            var elementId = "ID_TogglePanel";
            var ribbonPanelSourceReference = homeTab.Find(elementId);
            if(ribbonPanelSourceReference is null)
            {
                var panel = homeTab.AddNewPanel(elementId, "TogglePanel");
                var row = panel.AddNewRibbonRow();
                row.AddNewToggleButton("XyzToggleButton", "XYZSTATE\nToggle", null, RibbonButtonStyle.LargeWithText);
                cs.Save();
            }
```

```xml
    <adw:RibbonToggleButton 
        x:Uid="RibbonToggleButton-Xyz" 
        x:Key="XyzToggleButton"
        Name="XYZ Toggle"
        Tag="XYZSTATE">
        <adw:RibbonToggleButton.IsCheckedBinding>
            <Binding Source="{x:Static acmgd:Application.UIBindings}" Path="SystemVariables[XYZSTATE].Value" Converter="{StaticResource XyzToggleButtonConverter}"/>
        </adw:RibbonToggleButton.IsCheckedBinding>
        <adw:RibbonToggleButton.Image>
            <BitmapImage x:Uid="BitmapImage_1" UriSource="Resources/Toggle.bmp" />
        </adw:RibbonToggleButton.Image>
        <adw:RibbonToggleButton.LargeImage>
            <BitmapImage x:Uid="BitmapImage_2" UriSource="Resources/Toggle.bmp"/>
        </adw:RibbonToggleButton.LargeImage>
    </adw:RibbonToggleButton>TE">
```

### Reading:

[How to: Convert Bound Data - WPF .NET Framework | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/how-to-convert-bound-data?view=netframeworkdesktop-4.8)

### Steps To Build

```bash
git clone https://github.com/MadhukarMoogala/SimpleToggleButton.git
cd SimpleToggleButton
msbuild SimpleToggleButton.csproj -property:Configuration=Debug
```

Expected Build Log:  here

### Steps To Run

- Copy `SimpleToggleButton.bundle`to %APPDATA%\Autodesk\ApplicationPlugins

- Launch AutoCAD
  
  ### Demo
  
  <img src="https://github.com/MadhukarMoogala/SimpleToggleButton/blob/master/Toggle.gif" width="450" height="450">

### License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](https://github.com/MadhukarMoogala/SimpleToggleButton/blob/master/LICENSE.txt) file for full details.

### Written By

Madhukar Moogala, [Forge Partner Development](http://forge.autodesk.com/) @galakar