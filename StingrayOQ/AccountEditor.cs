using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace finantic.OQPlugins
{
    // UITypeEditor for Account Information
    // Used in the PropertyGrid
    public class AccountEditor : UITypeEditor
    {
        private AccountEditorControl editor;

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            // create the GUI
            if (editor == null) editor = new AccountEditorControl();
            
            // initialize UI with current settings
            editor.AccountSettings = (AccountSettings) value;
            
            editService.DropDownControl(editor); // show UI
            // back from UI, save changes
            editor.AccountSettings.Save();

            return editor.AccountSettings; // return updated value (same reference as before)
        }
    }
}
