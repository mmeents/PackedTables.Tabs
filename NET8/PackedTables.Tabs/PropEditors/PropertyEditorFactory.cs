using PackedTables.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackedTableTabs.PropEditors {

  public enum EditorType {
    Text,
    Boolean,
    Numeric,
    DateTime,
    Guid,
    ComboBox,
    Color,
    FilePicker,
    Memo
  }

  /// <summary>
  /// Factory for creating property editors based on column types and metadata
  /// </summary>
  public static class PropertyEditorFactory {

    /// <summary>
    /// Creates an editor based on the column type with automatic type inference
    /// </summary>
    public static IAmAFieldEditor? CreateEditor(FieldModel field, TableUISchema? schema = null) {
      if (field?.OwnerRow?.Owner == null) return null;

      var column = field.OwnerRow.Owner.Columns.Values
          .FirstOrDefault(c => c.Id == field.ColumnId);

      if (column == null) return null;

      EditorType editorType;
      if (schema != null) {
        var config = schema.GetConfig(column.ColumnName);
        editorType = config.EditorType ?? InferEditorType(column.ColumnType);
      } else {
        editorType = InferEditorType(column.ColumnType);
      }
      return CreateEditor(editorType, field);
    }

    /// <summary>
    /// Creates an editor of a specific type
    /// </summary>
    public static IAmAFieldEditor? CreateEditor(EditorType editorType, FieldModel? field = null) {
      IAmAFieldEditor editor = editorType switch {
        EditorType.Boolean => new BooleanPropertyEditor(),
        EditorType.Numeric => new NumericPropertyEditor(),
        EditorType.DateTime => new DateTimePropertyEditor(),
        EditorType.Guid => new GuidPropertyEditor(),
        EditorType.Text => new TextPropertyEditor(),
        EditorType.ComboBox => new ComboBoxPropertyEditor(),
        EditorType.Color => new ColorPropertyEditor(),
        EditorType.FilePicker => new FilePickerPropertyEditor(),
        EditorType.Memo => new MemoPropertyEditor(),
        _ => new TextPropertyEditor()
      };

      if (editor != null && field != null) {
        editor.Field = field;
      }

      return editor;
    }

    /// <summary>
    /// Infers the best editor type based on column type
    /// </summary>
    public static EditorType InferEditorType(ColumnType columnType) {
      return columnType switch {
        ColumnType.Boolean => EditorType.Boolean,
        ColumnType.Int32 or ColumnType.Int64 or ColumnType.Decimal => EditorType.Numeric,
        ColumnType.DateTime => EditorType.DateTime,
        ColumnType.Guid => EditorType.Guid,
        ColumnType.String => EditorType.Text,
        ColumnType.Bytes => EditorType.Text, // Could be FilePicker in future
        _ => EditorType.Text
      };
    }

    /// <summary>
    /// Gets the recommended height for an editor type
    /// </summary>
    public static int GetRecommendedHeight(EditorType editorType) {
      return editorType switch {
        EditorType.Memo => 80,
        EditorType.Color => 30,
        EditorType.FilePicker => 30,
        _ => 30
      };
    }

    /// <summary>
    /// Registers a custom editor factory function for extensibility
    /// </summary>
    private static readonly Dictionary<EditorType, Func<IAmAFieldEditor>> _customFactories = new();

    public static void RegisterCustomEditor(EditorType editorType, Func<IAmAFieldEditor> factory) {
      _customFactories[editorType] = factory;
    }
  }

  /// <summary>
  /// Attribute for marking editors with their supported column types
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class PropertyEditorAttribute : Attribute {
    public ColumnType[] SupportedTypes { get; }
    public EditorType EditorType { get; }

    public PropertyEditorAttribute(EditorType editorType, params ColumnType[] supportedTypes) {
      EditorType = editorType;
      SupportedTypes = supportedTypes;
    }
  }

  /// <summary>
  /// Configuration for customizing editor behavior per column
  /// </summary>
  public class ColumnEditorConfig {
    public EditorType EditorType { get; set; }
    public bool ReadOnly { get; set; }
    public int DisplayOrder { get; set; }
    public string? GroupName { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();

    // ComboBox specific
    public List<object> ComboBoxItems { get; set; } = new();
    public string? DisplayMember { get; set; }
    public string? ValueMember { get; set; }

    // Validation specific
    public string? ValidationRegex { get; set; }
    public string? ValidationMessage { get; set; }
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }

    // UI specific
    public string? Tooltip { get; set; }
    public string? HelpText { get; set; }
  }

  /// <summary>
  /// Extension methods for easier configuration
  /// </summary>
  public static class ColumnEditorConfigExtensions {
    public static ColumnEditorConfig AsComboBox(this ColumnEditorConfig config, params object[] items) {
      config.EditorType = EditorType.ComboBox;
      config.ComboBoxItems.AddRange(items);
      return config;
    }

    public static ColumnEditorConfig AsReadOnly(this ColumnEditorConfig config) {
      config.ReadOnly = true;
      return config;
    }

    public static ColumnEditorConfig WithValidation(this ColumnEditorConfig config,
        string regex, string message) {
      config.ValidationRegex = regex;
      config.ValidationMessage = message;
      return config;
    }

    public static ColumnEditorConfig WithRange<T>(this ColumnEditorConfig config,
        T minValue, T maxValue) where T : IComparable {
      config.MinValue = minValue;
      config.MaxValue = maxValue;
      return config;
    }

    public static ColumnEditorConfig WithTooltip(this ColumnEditorConfig config, string tooltip) {
      config.Tooltip = tooltip;
      return config;
    }

    public static ColumnEditorConfig InGroup(this ColumnEditorConfig config, string groupName) {
      config.GroupName = groupName;
      return config;
    }

    public static ColumnEditorConfig WithDisplayOrder(this ColumnEditorConfig config, int order) {
      config.DisplayOrder = order;
      return config;
    }
  }
}
