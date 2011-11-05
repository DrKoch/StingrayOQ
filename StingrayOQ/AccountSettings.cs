using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Microsoft.Win32; // Registry

namespace finantic.OQPlugins
{
    /// <summary>
    /// Maintain active state and memo string for all known accounts
    /// state is persisted to Registry
    /// </summary>
    public class AccountSettings
    {       
        private DataTable accountInfoTable;

        #region constructor
        public AccountSettings()
        {
            accountInfoTable = new DataTable();
            DataColumn[] primaryKey = new DataColumn[1];
            accountInfoTable.Columns.Add("Active", typeof(bool));
            primaryKey[0] = 
            accountInfoTable.Columns.Add("Account", typeof(string));
            accountInfoTable.Columns.Add("Live", typeof(bool));
            accountInfoTable.Columns.Add("Memo", typeof(string));
            accountInfoTable.PrimaryKey = primaryKey;
        }
        #endregion

        #region Properties
        /// <summary>
        /// return internal DataTable
        /// </summary>
        public DataTable Table
        {
            get { return accountInfoTable;  }
        }
        #endregion

        #region public methods
        /// <summary>
        /// return active state of account
        /// </summary>
        /// <param name="account">account name</param>
        /// <returns>active state</returns>
        public bool IsActive(string account)
        {
            DataRow row = accountInfoTable.Rows.Find(account);
            if (row == null) return false;
            return (bool) row["Active"];
        }

        /// <summary>
        /// retturn Memo string for account
        /// </summary>
        /// <param name="account">account name</param>
        /// <returns>memo string</returns>
        public string GetMemo(string account)
        {
            DataRow row = accountInfoTable.Rows.Find(account);
            if(row == null) return "";
            return (string)row["Memo"];
        }

        /// <summary>
        /// connection to broker stopped. Clear all "Live" marks
        /// </summary>
        public void Disconnect()
        {
            foreach (DataRow row in accountInfoTable.Rows) row["Live"] = false;
        }

        /// <summary>
        /// Add account to table if not there, mark as "Live"
        /// </summary>
        /// <param name="acct">account name</param>
        /// <returns></returns>
        public void AddAccountIf(string acct)
        {
            DataRow row = accountInfoTable.Rows.Find(acct);
            if (row == null)
            {
                row = accountInfoTable.NewRow(); // insert new row
                accountInfoTable.Rows.Add(row);
                row["Active"] = true;
                row["Account"] = acct;
                row["Memo"] = "";
            }
            row["Live"] = true;                      
        }

        #region Load Save
        const string registryKeyName = "Software\\finantic\\StingrayOQ";

        /// <summary>
        /// save internal table to registry
        /// </summary>
        public void Save()
        {
            RegistryKey Key;
            Key = Registry.CurrentUser.OpenSubKey(registryKeyName, true);
            if (Key == null)
            {
                Key = Registry.CurrentUser.CreateSubKey(registryKeyName);
            }
            int numAccounts = accountInfoTable.Rows.Count;
            string[] strings = new string[numAccounts];
            for(int i = 0; i < numAccounts; i++)
            {
                DataRow row = accountInfoTable.Rows[i];
                string memo = (string)row["Memo"];
                // do not allow \t in memo strings
                memo = memo.Replace('\t', ' ');               
                string active = ((bool)row["Active"]) ? "1" : "0";
                strings[i] += active + "\t"
                          + (string)row["Account"] + "\t"
                          + memo;               
            }
            Key.SetValue("AccountSettings", strings);
        }

        /// <summary>
        /// load internal table from Registry
        /// </summary>
        public void Load()
        {
            accountInfoTable.Clear();

            RegistryKey Key;
            Key = Registry.CurrentUser.OpenSubKey(registryKeyName, true);
            if (Key == null) return; // no info available
            
            string[] regstrings = (string[]) Key.GetValue("AccountSettings");
            if (regstrings == null) return; // no info available
            
            foreach (string rawRow in regstrings)
            {
                if (string.IsNullOrWhiteSpace(rawRow)) continue;

                string[] fields = rawRow.Split('\t');
                if (fields.Length < 2) continue;
                DataRow drow = accountInfoTable.NewRow();

                if (fields[0].StartsWith("1")) drow["Active"] = true;
                else drow["Active"] = false;
                drow["Account"] = fields[1];
                drow["Live"] = false;
                if (fields.Length > 2)
                {
                    drow["Memo"] = fields[2];
                }
                else drow["Memo"] = "";
                accountInfoTable.Rows.Add(drow);
            }                     
        }
        #endregion
  
        /// <summary>
        /// return a comma separated list of active accounts
        /// </summary>
        /// <returns>active accounts</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(DataRow row in accountInfoTable.Rows)
            {
                if ((bool)row["Active"])
                {
                    if (sb.Length > 0) sb.Append(',');
                    sb.Append((string) row["Account"]);
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}
