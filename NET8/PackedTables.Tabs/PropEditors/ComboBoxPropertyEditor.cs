using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PackedTableTabs.PropEditors {

  [PropertyEditor(EditorType.ComboBox, ColumnType.String)]
  public partial class ComboBoxPropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public ComboBoxPropertyEditor() {
      InitializeComponent();
    }
    private bool _isEditing;
    private string? _originalValue;
    public event EventHandler? ValueChanged;

    private FieldModel? _fieldModel;
    public FieldModel? Field {
      get => _fieldModel;
      set {
        _fieldModel = value;
        if (value != null) {
          ResetToField();
        }
      }
    }

    // ComboBox specific properties
    public List<object> Items { get; set; } = new();
    public string DisplayMember { get; set; } = "";
    public string ValueMember { get; set; } = "";

    public void SetItems(params object[] items) {
      Items.Clear();
      Items.AddRange(items);
      RefreshComboBoxItems();
    }

    public void SetItems(IEnumerable<object> items) {
      Items.Clear();
      Items.AddRange(items);
      RefreshComboBoxItems();
    }

    private void RefreshComboBoxItems() {
      comboBox1.Items.Clear();
      foreach (var item in Items) {
        comboBox1.Items.Add(item);
      }
    }

    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        comboBox1.Left = value + 3;
        comboBox1.Width = this.Width - value - 6;
      }
    }

    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    public string PropertyValue {
      get => comboBox1.SelectedItem?.ToString() ?? "";
      set {
        var item = Items.FirstOrDefault(i => i.ToString() == value);
        if (item != null) {
          comboBox1.SelectedItem = item;
        }
      }
    }

    public bool Modified { get; set; }

    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        comboBox1.Enabled = value;
      }
    }

    private void ComboBox1_SelectedIndexChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.ValueString = comboBox1.SelectedItem?.ToString() ?? "";
        Modified = false;
      }
    }

    public void ResetToField() {
      if (Field != null) {
        PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
        PropertyValue = Field?.ValueString ?? "";
        _originalValue = Field?.ValueString;
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      BackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
    }
  }
}
