using Autodesk.AutoCAD.Customization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SimpleToggleButton
{
    public static class CuixExtension
    {
        public static RibbonTabSource AddNewTab(
            this CustomizationSection instance,
            string name,
            string text = null)
        {
            if (text == null)
                text = name;
            var ribbonRoot = instance.MenuGroup.RibbonRoot;
            var id = "tab" + name;
            var ribbonTabSource = new RibbonTabSource(ribbonRoot)
            {
                Name = name,
                Text = text,
                Id = id,
                ElementID = id
            };
            ribbonRoot.RibbonTabSources.Add(ribbonTabSource);
            return ribbonTabSource;
        }
        public static RibbonPanelSource AddNewPanel(
            this RibbonTabSource instance,
            string elementId,
            string name,
            string text = null)
        {
            if (text == null)
                text = name;
            var ribbonRoot = instance.CustomizationSection.MenuGroup.RibbonRoot;
            var panels = ribbonRoot.RibbonPanelSources;
            var id = elementId;
            var ribbonPanelSource = new RibbonPanelSource(ribbonRoot)
            {
                Name = name,
                Text = text,
                Id = id,
                ElementID = id
            };
            panels.Add(ribbonPanelSource);
            var ribbonPanelSourceReference = new RibbonPanelSourceReference(instance)
            {
                PanelId = ribbonPanelSource.ElementID                
            };
            instance.Items.Add(ribbonPanelSourceReference);            
            return ribbonPanelSource;
        }
        public static RibbonRow AddNewRibbonRow(this RibbonPanelSource instance)
        {
            var row = new RibbonRow(instance);
            instance.Items.Add(row);
            return row;
        }
        public static RibbonRow AddNewRibbonRow(this RibbonRowPanel instance)
        {
            var row = new RibbonRow(instance);
            instance.Items.Add(row);
            return row;
        }
        public static RibbonRowPanel AddNewPanel(this RibbonRow instance)
        {
            var row = new RibbonRowPanel(instance);
            instance.Items.Add(row);
            return row;
        }
        public static RibbonCommandButton AddNewButton(
            this RibbonRow instance,
            string text,
            string commandFriendlyName,
            string command,
            string commandDescription,
            string smallImagePath,
            string largeImagePath,
            RibbonButtonStyle style)
        {
            var button = NewButton(instance,
                                   text,
                                   commandFriendlyName,
                                   command, commandDescription,
                                   smallImagePath,
                                   largeImagePath,
                                   style);
            instance.Items.Add(button);
            return button;
        }
        public static RibbonCommandButton AddNewButton(
            this RibbonSplitButton instance,
            string text,
            string commandFriendlyName,
            string command,
            string commandDescription,
            string smallImagePath,
            string largeImagePath,
            RibbonButtonStyle style)
        {
            var button = NewButton(instance,
                                   text,
                                   commandFriendlyName,
                                   command, commandDescription,
                                   smallImagePath,
                                   largeImagePath,
                                   style);
            instance.Items.Add(button);
            return button;
        }
        public static RibbonSplitButton AddNewSplitButton(this RibbonRow instance,
                                                               string text,
                                                               RibbonSplitButtonBehavior behavior,
                                                               RibbonSplitButtonListStyle listStyle,
                                                               RibbonButtonStyle style)
        {
            var button = new RibbonSplitButton(instance)
            {
                Text = text,
                Behavior = behavior,
                ListStyle = listStyle,
                ButtonStyle = style
            };
            instance.Items.Add(button);
            return button;
        }
        public static RibbonToggleButton AddNewToggleButton(this RibbonRow instance,
                                                              string id,
                                                              string text,
                                                              string keyTip,
                                                              RibbonButtonStyle style)
        {
            var button = new RibbonToggleButton(instance)
            {
                Text = text,
                IsHiddenInEditor = false,
                Id=id,
                KeyTip=keyTip,
                ButtonStyle=style
            };
            instance.Items.Add(button);
            return button;
        }
        public static RibbonSeparator AddNewSeparator(this RibbonRow instance,
                                                           RibbonSeparatorStyle style = RibbonSeparatorStyle.Line)
        {
            var separator = new RibbonSeparator(instance)
            {
                SeparatorStyle = style
            };
            instance.Items.Add(separator);
            return separator;
        }
        public static RibbonSeparator AddNewSeparator(this RibbonSplitButton instance,
                                                           RibbonSeparatorStyle style = RibbonSeparatorStyle.Line)
        {
            var separator = new RibbonSeparator(instance)
            {
                SeparatorStyle = style
            };
            instance.Items.Add(separator);
            return separator;
        }
        private static RibbonCommandButton NewButton(RibbonItem parent,
                                                     string text,
                                                     string commandFriendlyName,
                                                     string command,
                                                     string commandDescription,
                                                     string smallImagePath,
                                                     string largeImagePath,
                                                     RibbonButtonStyle style)
        {
            var customizationSection = parent.CustomizationSection;
            var macroGroups = customizationSection.MenuGroup.MacroGroups;
            MacroGroup macroGroup;
            if (macroGroups.Count == 0)
                macroGroup = new MacroGroup("MacroGroup", customizationSection.MenuGroup);
            else
                macroGroup = macroGroups[0];
            var button = new RibbonCommandButton(parent);
            button.Text = text;
            var commandMacro = "^C^C_" + command;
            var commandId = "ID_" + command;
            var buttonId = "btn" + command;
            var labelId = "lbl" + command;
            var menuMacro = macroGroup.CreateMenuMacro(commandFriendlyName,
                                                             commandMacro,
                                                             commandId,
                                                             commandDescription,
                                                             MacroType.Any,
                                                             smallImagePath,
                                                             largeImagePath,
                                                             labelId);
            var macro = menuMacro.macro;
            MenuMacroCollection mc = menuMacro.Parent.MenuMacros;
            macro.CLICommand = command;
            button.MacroID = menuMacro.ElementID;
            button.ButtonStyle = style;
            return button;
        }
    }
}
