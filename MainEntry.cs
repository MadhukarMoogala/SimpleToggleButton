using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal.Windows;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using HostApp = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices;
[assembly: ExtensionApplication(typeof(SimpleToggleButton.MainEntry))]
[assembly: CommandClass(typeof(SimpleToggleButton.ToggleCommand))]
namespace SimpleToggleButton
{
    public class MainEntry : IExtensionApplication
    {
     

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            folderPath  = Path.Combine(Directory.GetDirectoryRoot(folderPath), Thread.CurrentThread.CurrentUICulture.Name.ToString());
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath) == false) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        public void Initialize()
        {


            //RibbonContent.AddRibbonControls("Xyz", "pack://application:,,,/SimpleToggleButton;component/XyzControls.xaml");

            //Ideally adding ribboncontrol through API should work, for some reason, the RibbonControl is not registering correctly.

            //1. You can use Bundle mechanism to register system variable and ribbon control through PackageElements
            //In PackageElements, we can set RibbonControl and SystemVariables.

            //2. You can also register through win32 Registry API, requires admin access.
            //CreateSystemVariable();
            /*RegisterRibbonControl();*/
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

        }
        public void Terminate()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }
        public void CreateSystemVariable()
        {
            //Refer Variable Documentation.
            //https://help.autodesk.com/view/OARX/2022/ENU/?guid=OARX-ManagedRefGuide-__MEMBERTYPE_Properties_Autodesk_AutoCAD_Runtime_Variable
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;
            string rootKey = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.Current.MachineRegistryProductRootKey;
            rootKey = rootKey.Split(':')[0];
            string varKey = Path.Combine(rootKey, "Variables");
            try
            {
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(varKey, true))
                {
                    using (var xyzKey = regKey.CreateSubKey("XYZSTATE"))
                    {
                        xyzKey.SetValue("", 0, Microsoft.Win32.RegistryValueKind.DWord);
                        xyzKey.SetValue("PrimaryType", 5003, Microsoft.Win32.RegistryValueKind.DWord);
                        xyzKey.SetValue("TypeFlags", 0, Microsoft.Win32.RegistryValueKind.DWord);                        
                        xyzKey.SetValue("LowerBound", 0, Microsoft.Win32.RegistryValueKind.DWord);
                        xyzKey.SetValue("UpperBound", 1, Microsoft.Win32.RegistryValueKind.DWord);
                        xyzKey.SetValue("StorageType", 2, Microsoft.Win32.RegistryValueKind.DWord);                        
                    }
                }
            }
            catch (System.ArgumentNullException ex)
            {
                ed.WriteMessage(ex.Message);
            }
        }
        public void RegisterRibbonControl()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;
            string rootKey = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.Current.MachineRegistryProductRootKey;
            rootKey = rootKey.Split(':')[0];
            string varKey = Path.Combine(rootKey, "RibbonControls");
            try
            {
                using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(varKey, true))
                {
                    var uriPack = new Uri("pack://application:,,,/SimpleToggleButton;component/XyzControls.xaml");
                    regKey.SetValue("Xyz", uriPack.AbsoluteUri);
                }
            }
            catch (System.ArgumentNullException ex)
            {
                ed.WriteMessage(ex.Message);
            }
        }

    }
    public class ToggleCommand
    {
        [CommandMethod("RBToggle")]
        public void RBToggle()
        {
                      
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
        }        
    }

    


    public class ToggleButtonConverter : IValueConverter
    {
        public string OnMacro { get; set; }
        public string OffMacro { get; set; }
        virtual protected bool IsChecked(object value)
        {
            return System.Convert.ToBoolean(value);
        }
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
        private void RunMacro(string sMacro)
        {
            if (string.IsNullOrEmpty(sMacro))
                return;
            Document curDoc = AcadApp.DocumentManager.MdiActiveDocument;
            if (curDoc == null)
                return;
            // Add escape to cancel the current command if necessary
            string sMacro2 = sMacro;
            // cancel the current command, if needed
            {
                if (sMacro2[0] != '\'' && sMacro2[0] != '*' && sMacro2[0] != 3)
                {
                    int nPos = sMacro.IndexOf("^C");
                    if (nPos < 0)
                        sMacro2 = string.Format("{0}{1}{2}", System.Convert.ToChar(3), System.Convert.ToChar(3), sMacro2);
                }
            }
            try
            {
                //sMacro2 = string.Format("{0}{1}", new string((char)03, 2), sMacro2);
                curDoc.SendStringToExecute(sMacro2, false, false, true);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e2)
            {
                Debug.WriteLine(string.Format("Error: Exception in RunMacro: {0}", e2.Message));
            }
        }
    }
}
