using System.Windows.Forms;

namespace finantic.OQPlugins
{
    public partial class AccountEditorControl : UserControl
    {
        public AccountEditorControl()
        {
            InitializeComponent();
            dgvAccounts.AutoGenerateColumns = true;           
        }

        #region Properties
        private AccountSettings _accountSettings;
        public AccountSettings AccountSettings
        {
            get
            {
                dgvAccounts.CurrentCell = null; // committ changes               
                return _accountSettings;
            }
            set
            {                
                _accountSettings = value;
                dgvAccounts.DataSource = _accountSettings.Table;
                dgvAccounts.Columns["Account"].ReadOnly = true;
                dgvAccounts.Columns["Live"].ReadOnly = true;
                dgvAccounts.Columns["Memo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
        #endregion   
    }
}
