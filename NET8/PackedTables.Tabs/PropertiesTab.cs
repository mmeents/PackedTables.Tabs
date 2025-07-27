using PackedTables.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTableTabs.PropEditors;

namespace PackedTableTabs {
  public class PropertiesTab : TabPage {
    public string PageName {
      get { return this.Text; }
      set { this.Text = value; }
    }
    public PropertiesTab() {
      BasePanel = new Panel {
        Dock = DockStyle.Fill,
        AutoScroll = true
      };
      this.Controls.Add(BasePanel);
    }
    public PropertiesTab(string tabName) : base(tabName) {
      BasePanel = new Panel {
        Dock = DockStyle.Fill,
        AutoScroll = true
      };
      this.Controls.Add(BasePanel);
    }

    private Panel BasePanel { get; set; }
    private Panel? TitlePanel { get; set; }
    private Panel? MenuPanel { get; set; }
    private ConcurrentDictionary<int, IAmAFieldEditor> _propertyEditors = new ConcurrentDictionary<int, IAmAFieldEditor>();
    private Label? TitleLabel { get; set; }
    private RowModel? _tableRow;
    public RowModel? TableRow {
      get { return _tableRow; }
      set {
        _tableRow = value;
        if (value != null) {
          ResetToRow();
        } else {
          TitleLabel.Text = "";
        }
      }
    }

    private bool _disabled = false;
    public virtual bool Disabled {
      get { return _disabled; }
      set {
        _disabled = value;
        SetDisabled(value);
      }
    }

    private void SetDisabled(bool disabled) {
      _disabled = disabled;
      if (_disabled) {
        BasePanel.Enabled = false;
        if (TitleLabel != null) TitleLabel.Text = "Properties (Disabled)";
        if (_propertyEditors.Keys.Count > 0) {
          foreach (var editor in _propertyEditors.Values) {
            editor.Enabled = false;
          }
        }
      } else {
        BasePanel.Enabled = true;
        TitleLabel.Text = "Properties";
      }
    }


    public void ResetToRow() {
      if (TableRow != null) {
        BasePanel.Controls.Clear();
        foreach (var field in TableRow.RowFields.OrderByDescending(kvp => kvp.Key)) {
          var propertyEditor = new TextPropertyEditor();
          propertyEditor.Field = field.Value;
          propertyEditor.Dock = DockStyle.Top;
          propertyEditor.Height = 30; // Set a fixed height for each property editor
          _propertyEditors[field.Key] = propertyEditor;
          BasePanel.Controls.Add(propertyEditor);
        }
        MenuPanel = new Panel {
          Dock = DockStyle.Top,
          Height = 15,
          BackColor = System.Drawing.Color.LightGray
        };
        BasePanel.Controls.Add(MenuPanel);

        TitlePanel = new Panel {
          Dock = DockStyle.Top,
          Height = 30,
          BackColor = System.Drawing.Color.LightGray
        };
        // Initialize the properties tab with a label
        TitleLabel = new Label {
          Text = "Properties",
          TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };
        TitlePanel.Controls.Add(TitleLabel);
        BasePanel.Controls.Add(TitlePanel);

      } else {
        TitleLabel.Text = "Properties";
      }
    }



    public void SetLabelRight(int right) {
      if (_propertyEditors.Count > 0) {
        foreach (var editor in _propertyEditors.Values) {
          editor.LabelRight = right;
        }
      }
    }

  }
}
