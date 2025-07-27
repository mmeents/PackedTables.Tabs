using PackedTables.Net;
using PackedTableTabs.PropEditors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackedTableTabs {
  public class PropertiesTab : TabPage {
    private string _titleLabel = "Properties";
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
      _titleLabel = "Properties";
    }
    public PropertiesTab(string tabName) : base(tabName) {
      BasePanel = new Panel {
        Dock = DockStyle.Fill,
        AutoScroll = true
      };
      this.Controls.Add(BasePanel);
      _titleLabel = tabName;
    }
    private Button? OkButton { get; set; }
    private Button? CancelButton { get; set; }

    private bool _isEditing = false;
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
    private int _labelRight = 50; // Default label right position
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
        if (TitleLabel != null) TitleLabel.Text = $"{_titleLabel} (Disabled)";
        if (_propertyEditors.Keys.Count > 0) {
          foreach (var editor in _propertyEditors.Values) {
            editor.Enabled = false;
          }
        }
      } else {
        BasePanel.Enabled = true;
        TitleLabel.Text = $"{_titleLabel}";
      }
    }

   
    private void OkButton_Click(object? sender, EventArgs e) {
      if (TableRow?.Owner != null) {
        try {
          if (TableRow.Owner.FindFirst("Id", TableRow.Id)) {
            TableRow.Owner.Edit();
          } 
          foreach (var field in TableRow.RowFields.OrderByDescending(kvp => kvp.Key)) {            
            var editor = _propertyEditors[field.Key];
            if ( editor.Modified) { 
              editor.CommitToField(); // Commit changes to the field
              editor.Modified = false; // Reset modified state
            }
          }
          if (TableRow.Owner.State != TableState.Browse) {
            TableRow.Owner.Post(); 
          }
          
          SetEditingMode(false);
        } catch (Exception ex) {
          MessageBox.Show($"Validation failed: {ex.Message}", "Error",
              MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
    }

    private void CancelButton_Click(object? sender, EventArgs e) {
      if (TableRow?.Owner != null) {        
        SetEditingMode(false);
        ResetToRow(); // Refresh display
      }
    }
    public void ResetToRow() {
      if (TableRow != null) {
        BasePanel.Controls.Clear();

        MenuPanel = new Panel {
          Dock = DockStyle.Top,
          Height = 24
        };
        BasePanel.Controls.Add(MenuPanel);

        foreach (var field in TableRow.RowFields.OrderByDescending(kvp => kvp.Key)) {
          var propertyEditor = new TextPropertyEditor();
          propertyEditor.Field = field.Value;
          propertyEditor.Dock = DockStyle.Top;
          propertyEditor.Height = 30; // Set a fixed height for each property editor
          propertyEditor.LabelRight = 50; // Set a default label right position
          propertyEditor.ValueChanged += PropertyEditor_ValueChanged;
          _propertyEditors[field.Key] = propertyEditor;
          BasePanel.Controls.Add(propertyEditor);
          _titleLabel = TableRow.Owner?.Name ?? "PrNotSet";
        }     

        TitlePanel = new Panel {
          Dock = DockStyle.Top,
          Height = 30,
          BackColor = System.Drawing.Color.LightGray
        };
        // Initialize the properties tab with a label
        TitleLabel = new Label {
          Text = _titleLabel,
          TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        };

        TitlePanel.Controls.Add(TitleLabel);
        BasePanel.Controls.Add(TitlePanel);
        CreateMenuButtons();

      } else {
        TitleLabel.Text = "Properties";
      }
    }

    private void PropertyEditor_ValueChanged(object? sender, EventArgs e) {
      if (!_isEditing) { 
        SetEditingMode(true); // Enable editing mode if a property changes
      }      
    }

    private void CreateMenuButtons() {
      if (MenuPanel == null) return;

      MenuPanel.Controls.Clear();
      int left = _labelRight;
      // OK Button
      OkButton = new Button {
        Text = "OK",
        Size = new Size(50, 20),
        Location = new Point(left+2, 2),
        Enabled = false, // Start disabled
      };
      OkButton.Click += OkButton_Click;

      // Cancel Button  
      CancelButton = new Button {
        Text = "Cancel",
        Size = new Size(50, 20),
        Location = new Point(left+OkButton.Width+ 7, 2),
        Enabled = false, // Start disabled
      };
      CancelButton.Click += CancelButton_Click;

      MenuPanel.Controls.Add(OkButton);
      MenuPanel.Controls.Add(CancelButton);
      if (MenuPanel.Visible) MenuPanel.Visible = false;
    }


    public void SetLabelRight(int right) {
      _labelRight = right;
      if (_propertyEditors.Count > 0) {        
        foreach (var editor in _propertyEditors.Values) {
          editor.LabelRight = right;
        }
      }
      if (MenuPanel != null && OkButton != null && CancelButton != null) {
        OkButton.Left = _labelRight + 2;
        CancelButton.Left = OkButton.Left + OkButton.Width + 5;
      }
    }

    private void SetEditingMode(bool editing) {
      _isEditing = editing;
      if ( MenuPanel == null ) {
        return;          
      }
      if (!MenuPanel.Visible) MenuPanel.Visible = true;
      if (OkButton != null) OkButton.Enabled = editing;
      if (CancelButton != null) CancelButton.Enabled = editing;
      if (MenuPanel.Visible != editing) MenuPanel.Visible = editing;

      // Update visual state of all editors
      foreach (var editor in _propertyEditors.Values) {
        if (editor is IEditStateAware editAware) {
          editAware.SetEditingState(editing);
        }
      }

      // Update title to show state
      if (TitleLabel != null) {
        TitleLabel.Text = editing ? $"{_titleLabel} (Editing)" : _titleLabel;     
      }
    }

  }

  public static class PropertiesTabColors {
    public static readonly Color NormalBackground = Color.White;
    public static readonly Color EditingBackground = Color.FromArgb(244, 245, 220);
    
    public static readonly Color LabelBackground = Color.FromArgb(244, 245, 220);
    public static readonly Color EditingLabelBackground = Color.FromArgb(244, 245, 220);
    public static readonly Color DisabledBackground = Color.LightGray;
  }
}
