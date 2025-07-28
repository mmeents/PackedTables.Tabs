using System.Windows.Forms;
using PackedTables.Net;
using PackedTableTabs;

namespace DemoApps {
  public partial class MainForm : Form {
    private MessageLogTab textTabPage;
    private PropertiesTab propertiesTab;
    private PackedTableSet PackedTableSet = new PackedTableSet();
    public MainForm() {
      InitializeComponent();
      propertiesTab = new PropertiesTab("Props");      
      tabControl1.TabPages.Add(this.propertiesTab);

      this.textTabPage = new MessageLogTab("Errors");
      textTabPage.IsHideable = true;
      textTabPage.OnHideEvent += () => {
        splitContainer1.Panel2Collapsed = true;
      };
      textTabPage.OnShowEvent += () => {
        splitContainer1.Panel2Collapsed = false;
      };
      propertiesTab.LogTab = this.textTabPage;
      tabControl2.TabPages.Add(this.textTabPage);
    }

    private void MainForm_Shown(object sender, EventArgs e) {
      var table = PackedTableSet.AddTable("Customers");
      table.AddColumn("Key", ColumnType.String);
      table.AddColumn("Value", ColumnType.String);
      table.AddColumn("Description", ColumnType.String);
      table.AddColumn("Type", ColumnType.Int32);
      table.AddColumn("IsActive", ColumnType.Boolean);
      table.AddColumn("CreatedDate", ColumnType.DateTime);
      table.AddColumn("CustomerId", ColumnType.Guid);
      table.AddColumn("CreditLimit", ColumnType.Decimal);


      table.AddRow();
      table.Current["Key"].Value = "Name";
      table.Current["Value"].Value = "Test Table";
      table.Current["Description"].Value = "This is a test table.";
      table.Current["Type"].Value = 36;
      table.Current["IsActive"].Value = true;
      table.Current["CreatedDate"].Value = DateTime.Now.AddMonths(-6);
      table.Current["CustomerId"].Value = Guid.NewGuid();
      table.Current["CreditLimit"].Value = 5000.50m;

      table.Post();

      var schema = table.GetUISchema();
      schema.ConfigureColumn("Id").AsReadOnly().WithTooltip("System generated ID");
      schema.ConfigureColumn("Key").WithOrder(1);
      schema.ConfigureColumn("IsActive").WithOrder(2);
      schema.ConfigureColumn("CreditLimit").WithOrder(3).WithTooltip("Maximum credit allowed");
      schema.ConfigureColumn("CreatedDate").WithOrder(4);

      propertiesTab.TableRow = table.Current;
      propertiesTab.UISchema = schema;
      propertiesTab.SetLabelRight(69);
    }
  }
}
