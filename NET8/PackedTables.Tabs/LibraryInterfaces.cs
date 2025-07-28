using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackedTables.Net;

namespace PackedTableTabs {
  /// <summary>
  /// Interface for a field viewer/editor.
  /// </summary>
  public interface IAmAFieldViewer {
    public FieldModel? Field { get; set; }
    public int LabelRight { get; set; }
    public string PropertyName { get; set; }
    public string PropertyValue { get; set; }
    public bool Modified { get; set; }
    public bool Enabled { get; set; }
    public void ResetToField();

  }

  public interface IAmAFieldEditor : IAmAFieldViewer {

    public event EventHandler? ValueChanged;
    public void CommitToField();

  }

  public interface IEditStateAware {
    void SetEditingState(bool editing);
  }

}
