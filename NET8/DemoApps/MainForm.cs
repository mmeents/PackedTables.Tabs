using System.Windows.Forms;
using PackedTables.Net;
using PackedTableTabs;

namespace DemoApps {
  public partial class MainForm : Form {
    private PropertiesTab propertiesTab;
    private PackedTableSet PackedTableSet = new PackedTableSet();
    public MainForm() {
      InitializeComponent();
      propertiesTab = new PropertiesTab("Properties");
      tabControl1.TabPages.Add(this.propertiesTab);
    }

    private void MainForm_Shown(object sender, EventArgs e) {
      var table = PackedTableSet.AddTable("table");
      table.AddColumn("Key", ColumnType.String);
      table.AddColumn("Value", ColumnType.String);
      table.AddColumn("Description", ColumnType.String);
      table.AddColumn("Type", ColumnType.String);


      table.AddRow();
      table.Current["Key"].Value = "Name";
      table.Current["Value"].Value = "Test Table";
      table.Current["Description"].Value = "This is a test table.";
      table.Current["Type"].Value = "Test";

      table.Post();

      propertiesTab.TableRow = table.Current;
      propertiesTab.SetLabelRight(69);
    }
  }
}
