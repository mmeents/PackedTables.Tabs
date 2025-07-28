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

namespace PackedTableTabs.PropEditors {
  public partial class DateTimePropertyEditor : UserControl, IAmAFieldEditor, IEditStateAware {
    public DateTimePropertyEditor() {
      InitializeComponent();
    }

    private bool _isEditing;
    private DateTime? _originalValue;

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

    public int LabelRight {
      get => lbName.Left + lbName.Width;
      set {
        lbName.Left = value - lbName.Width;
        lbName.TextAlign = ContentAlignment.TopRight;
        dateTimePicker1.Left = value + 3;
        dateTimePicker1.Width = this.Width - value - 6;
      }
    }

    public string PropertyName {
      get => lbName.Text;
      set => lbName.Text = value;
    }

    public string PropertyValue {
      get => dateTimePicker1.Value.AsStrDate();
      set {
        if (DateTime.TryParse(value, out DateTime result)) {
          dateTimePicker1.Value = result;
        }
      }
    }

    public bool Modified { get; set; }

    public new bool Enabled {
      get => base.Enabled;
      set {
        base.Enabled = value;
        lbName.Enabled = value;
        dateTimePicker1.Enabled = value;
      }
    }

    private void DateTimePicker1_ValueChanged(object? sender, EventArgs e) {
      if (!Modified) Modified = true;
      ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CommitToField() {
      if (Field == null) return;
      if (Modified) {
        Field.Value = dateTimePicker1.Value;
        Modified = false;
      }
    }

    public void ResetToField() {
      if (Field != null) {
        PropertyName = Field?.OwnerRow?.Owner?.Columns[Field.ColumnId].ColumnName ?? "";
        var dateValue = Field?.Value?.AsDateTime() ?? DateTime.Now;
        dateTimePicker1.Value = dateValue;
        _originalValue = dateValue;
        Modified = false;
      }
    }

    public void SetEditingState(bool editing) {
      _isEditing = editing;
      dateTimePicker1.CalendarTitleBackColor = editing ? PropertiesTabColors.EditingBackground : PropertiesTabColors.NormalBackground;
    }
  }
}
