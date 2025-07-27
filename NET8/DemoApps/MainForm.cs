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
      tabControl2.TabPages.Add(this.textTabPage);
    }

    private void MainForm_Shown(object sender, EventArgs e) {
      var table = PackedTableSet.AddTable("Customers");
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
