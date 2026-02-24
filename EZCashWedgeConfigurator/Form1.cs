using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EZCashWedgeConfigurator
{
    public partial class EZcashWedgeConfigurator : Form
    {
        private BindingList<Yards> yardList = [];
        TestAPI testAPI = new();
        public string ConfigFilePath = string.Empty;
        public string EZcashToken = string.Empty;

        public EZcashWedgeConfigurator()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void EZcashWedgeConfigurator_Load(object sender, EventArgs e)
        {


        }

        private void LoadDataGrid()
        {
            // Setup DataGridView
            dgYards.AllowUserToAddRows = false;
            dgYards.RowHeadersVisible = false;
            dgYards.AllowUserToResizeColumns = false;
            dgYards.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgYards.DefaultCellStyle.ForeColor = Color.Black;
            dgYards.DataSource = yardList;

            DataGridViewButtonColumn addButtonColumn = new DataGridViewButtonColumn
            {
                //addButtonColumn.Width = 10;

                Text = "➕",
                UseColumnTextForButtonValue = true,
                Width = 35,
            };
            addButtonColumn.CellTemplate.ToolTipText = "Add yard information";
            dgYards.Columns.Add(addButtonColumn);

            // Add "Remove" button column
            DataGridViewButtonColumn removeButtonColumn = new DataGridViewButtonColumn
            {

                Text = "❌",
                UseColumnTextForButtonValue = true,
                Width = 35,
                ToolTipText = "Remove yard information"
            };

            removeButtonColumn.CellTemplate.ToolTipText = "Remove yard information";
            dgYards.Columns.Add(removeButtonColumn);

            dgYards.Columns["YardId"].Width = 250;

            // Handle cell click events for buttons
            dgYards.CellClick += DataGridView1_CellClick;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore header clicks
            if (e.RowIndex < 0) return;

            // Add button clicked
            if (e.ColumnIndex == 2)
            {
                int currentRowIndex = e.RowIndex;

                yardList.Add(new Yards());
                //dgYards.Rows.Insert(currentRowIndex + 1, "", "");
            }

            // Remove button clicked
            else if (e.ColumnIndex == 3)
            {
                if (dgYards.Rows.Count > 1)
                {
                    //dgYards.Rows.RemoveAt(e.RowIndex);
                    var item = dgYards.CurrentRow.DataBoundItem as Yards;
                    yardList.Remove(item);
                }
                else
                {
                    MessageBox.Show("At least one row must remain.", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new())
            {
                openFileDialog.Title = "Select config file to modify:";
                openFileDialog.Filter = "Config files (*.exe.config)|*.exe.config";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;                  // Restore last folder

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ConfigFilePath = openFileDialog.FileName;
                    LoadConfigInformation();
                }
            }
        }

        private void LoadConfigInformation()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    XDocument doc = XDocument.Load(ConfigFilePath);
                    XElement appSettings = doc.Root.Element("appSettings");
                    if (appSettings == null)
                    {
                        appSettings = new XElement("appSettings");
                        doc.Root.Add(appSettings);
                    }

                    XElement Portsetting = appSettings.Elements("add").FirstOrDefault(e => e.Attribute("key")?.Value == "Ip");
                    if (Portsetting != null)
                    {
                        txtWedgeIp.BeginInvoke((Action)(() =>
                        {
                            txtWedgeIp.Text = Portsetting.Attribute("value")?.Value;
                        }));
                    }

                    XElement EZCashAPIsetting = appSettings.Elements("add").FirstOrDefault(e => e.Attribute("key")?.Value == "EZCashAPI");
                    if (EZCashAPIsetting != null)
                    {
                        txtEZCashAPI.BeginInvoke((Action)(() =>
                        {
                            txtEZCashAPI.Text = EZCashAPIsetting.Attribute("value")?.Value;
                        }));
                    }

                    XElement EZCashTokensetting = appSettings.Elements("add").FirstOrDefault(e => e.Attribute("key")?.Value == "EZCashAPIToken");
                    if (EZCashTokensetting != null)
                    {
                        EZcashToken = EZCashTokensetting.Attribute("value")?.Value;
                        txtEZCashToken.BeginInvoke((Action)(() =>
                        {
                            txtEZCashToken.Text = EZcashToken;

                        }));
                    }


                    XElement DeleteArchived = appSettings.Elements("add").FirstOrDefault(e => e.Attribute("key")?.Value == "DeleteArchived");
                    if (DeleteArchived != null)
                    {
                        txtArchiveRollOutDays.BeginInvoke((Action)(() =>
                        {
                            txtArchiveRollOutDays.Text = DeleteArchived.Attribute("value")?.Value;

                        }));
                    }


                    XElement TraceFileSize = appSettings.Elements("add").FirstOrDefault(e => e.Attribute("key")?.Value == "TraceFileSize");
                    if (TraceFileSize != null)
                    {
                        txtTraceSize.BeginInvoke((Action)(() =>
                        {
                            txtTraceSize.Text = TraceFileSize.Attribute("value")?.Value;

                        }));
                    }

                    var yardItems = doc.Descendants("yardIdSection")
                            .Elements("add")
                            .Select(x => new
                            {
                                Key = (string)x.Attribute("key"),
                                Value = (string)x.Attribute("value")
                            })
                            .ToList();

                    foreach (var item in yardItems)
                    {
                        yardList.Add(new Yards { Port = item.Key, YardId = item.Value });
                    }
                    LoadDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in reading config");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtWedgeIp.Text) || string.IsNullOrEmpty(txtEZCashAPI.Text) || string.IsNullOrEmpty(txtEZCashToken.Text))
            {
                MessageBox.Show($"Provide required EZCash Information", "Warning");
                return;
            }
            else if (yardList != null && yardList.Count == 0)
            {

                MessageBox.Show($"Provide Yard Information", "Warning");
                return;

            }
            else if (yardList != null && yardList.Count > 0)
            {
                var valid = false;
                foreach (var item in yardList)
                {
                    if (!string.IsNullOrEmpty(item.YardId) && !string.IsNullOrEmpty(item.Port))
                    {
                        valid = true;
                    }

                }
                if (!valid)
                {
                    MessageBox.Show($"Provide Wedge Ip Address");
                    return;
                }
            }

            var encryptedEZCashToken = string.Empty;

            if (!string.IsNullOrEmpty(EZcashToken) && EZcashToken == txtEZCashToken.Text)
                encryptedEZCashToken = txtEZCashToken.Text;
            else
                encryptedEZCashToken = TokenEncryptDecrypt.Encrypt(txtEZCashToken.Text);

            Dictionary<string, string> configValue = [];
            configValue.Add("Ip", txtWedgeIp.Text.Trim());
            configValue.Add("EZCashAPI", txtEZCashAPI.Text.Trim());
            configValue.Add("EZCashAPIToken", encryptedEZCashToken);
            configValue.Add("TraceFileSize", txtTraceSize.Text.Trim());
            configValue.Add("DeleteArchived", txtArchiveRollOutDays.Text.Trim());


            ModifyEZCashConfig(ConfigFilePath, configValue);
        }

        private void ModifyEZCashConfig(string configPath, Dictionary<string, string> configValue)
        {
            var result = true;
            var backupPath = string.Empty;
            string tempPath = configPath + ".tmp";


            try
            {
                if (File.Exists(configPath))
                {
                    backupPath = configPath + ".bak";
                    File.Copy(configPath, backupPath, overwrite: true);

                    // Work in memory
                    XDocument doc = XDocument.Load(configPath);
                    XElement appSettings = doc.Root.Element("appSettings");

                    if (appSettings == null)
                    {
                        appSettings = new XElement("appSettings");
                        doc.Root.Add(appSettings);
                    }

                    if (configValue != null)
                    {
                        foreach (var item in configValue)
                        {

                            XElement setting = appSettings.Elements("add").FirstOrDefault(e => e.Attribute("key")?.Value == item.Key);

                            if (setting != null)
                            {
                                setting.SetAttributeValue("value", item.Value);
                            }

                        }
                    }

                    if (yardList != null && yardList.Count > 0)
                    {
                        XElement yardSection = doc.Root.Element("yardIdSection");

                        if (yardSection == null)
                        {
                            yardSection = new XElement("yardIdSection");
                            doc.Root.Add(yardSection);
                        }


                        yardSection.Elements("add").Remove();

                        foreach (var yard in yardList)
                        {
                            if (!string.IsNullOrEmpty(yard.Port) && !string.IsNullOrEmpty(yard.YardId))
                                yardSection.Add(new XElement("add",
                                    new XAttribute("key", yard.Port.Trim() ?? string.Empty),
                                    new XAttribute("value", yard.YardId.Trim() ?? string.Empty)
                                ));
                        }
                    }
                    doc.Save(tempPath);
                    File.Replace(tempPath, configPath, backupPath);


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ReplaceBackUpConfigOnError(backupPath);
            }
            finally
            {
                MessageBox.Show("Config information saved succesfully.", "Success");
                Application.Exit();
            }
        }

        private void ReplaceBackUpConfigOnError(string backupCnfigPath)
        {
            File.Replace(backupCnfigPath, ConfigFilePath, backupCnfigPath);
        }

        private void btnConnectEZCashAPI_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEZCashAPI.Text) && !string.IsNullOrEmpty(txtEZCashToken.Text))
            {

                var token = string.Empty;

                if (!string.IsNullOrEmpty(EZcashToken) && EZcashToken == txtEZCashToken.Text)
                    token = TokenEncryptDecrypt.Decrypt(txtEZCashToken.Text);
                else
                    token = txtEZCashToken.Text;


                var result = testAPI.GetRequestNew<List<Events>>("?limit=1", txtEZCashAPI.Text.Replace("customer_barcodes", "events"), token);
                if (result != null && result.Any())
                {
                    var yardResult = ValidateYards();
                    if (yardResult)
                        MessageBox.Show("Valid EZCash API and Token");
                }
                else
                {
                    MessageBox.Show("InValid EZCash API and Token");
                }
            }
            else
                MessageBox.Show("Provide API and Token ");
        }

        private bool ValidateYards()
        {
            var status = true;
            try
            {
                if (yardList != null && yardList.Any())
                {
                    var token = string.Empty;

                    if (!string.IsNullOrEmpty(EZcashToken) && EZcashToken == txtEZCashToken.Text)
                        token = TokenEncryptDecrypt.Decrypt(txtEZCashToken.Text);
                    else
                        token = txtEZCashToken.Text;

                    var api = txtEZCashAPI.Text.Replace("customer_barcodes", "events");


                    foreach (var item in yardList)
                    {
                        var yardid = item.YardId.Trim();
                        var result = testAPI.GetRequestNew<List<Events>>($"?yard_id={yardid}", api, token);

                        if (result != null && result.Any())
                        {
                            if (string.IsNullOrEmpty(result[0].yard_id))
                            {
                                status = false;
                                MessageBox.Show($"Invalid Yard Id{item.YardId}");
                                break;
                            }
                        }
                        else
                        {
                            status = false;
                            MessageBox.Show($"Invalid Yard Id{item.YardId}");
                            break;
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Provide yard information");
                }
            }
            catch (Exception ex)
            {
                status = false;
                MessageBox.Show($"{ex.Message}");
            }

            return status;
        }
    }

    public class Yards
    {
        public string Port { get; set; }
        public string YardId { get; set; }
    }

    public class CustomerBarcode
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Dev_id { get; set; } = string.Empty;
        public string CompanyNumber { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public string TranId { get; set; } = string.Empty;
        public string withdrawal_tran_id { get; set; } = string.Empty;
        public string Amount { get; set; } = "0";
        public string ActId { get; set; } = string.Empty;
        public int Used { get; set; } = 0;
        public int RowID { get; set; }
        public decimal DispensedAmount { get; set; }
        public bool authorized { get; set; }
        public decimal authorized_amount { get; set; }
        public decimal requested_amount { get; set; }
        public decimal Balance { get; set; }
        public string receipt_nbr { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public string payee { get; set; } = string.Empty;

        public string error_code { get; set; }
        public string error { get; set; }
        public decimal original_authorized_amount { get; set; }
    }

    public class Events()
    {
        public int company_id { get; set; }
        public string title { get; set; }
        public int account_type_id { get; set; }
        public string yard_id { get; set; }

    }
}
