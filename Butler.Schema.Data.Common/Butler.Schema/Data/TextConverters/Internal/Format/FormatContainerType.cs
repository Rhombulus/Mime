// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.FormatContainerType
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal enum FormatContainerType : byte
  {
    Null = (byte) 0,
    TableContainer = (byte) 7,
    TableDefinition = (byte) 8,
    TableColumnGroup = (byte) 9,
    TableColumn = (byte) 10,
    Inline = (byte) 18,
    HyperLink = (byte) 19,
    Bookmark = (byte) 20,
    Area = (byte) 22,
    BaseFont = (byte) 24,
    Form = (byte) 25,
    FieldSet = (byte) 26,
    Label = (byte) 27,
    Input = (byte) 28,
    Button = (byte) 29,
    Legend = (byte) 30,
    TextArea = (byte) 31,
    Select = (byte) 32,
    OptionGroup = (byte) 33,
    Option = (byte) 34,
    Text = (byte) 36,
    PropertyContainer = (byte) 37,
    InlineObjectFlag = (byte) 64,
    Image = (byte) 85,
    BlockFlag = (byte) 128,
    Root = (byte) 129,
    Document = (byte) 130,
    Fragment = (byte) 131,
    Block = (byte) 132,
    BlockQuote = (byte) 133,
    HorizontalLine = (byte) 134,
    TableCaption = (byte) 139,
    TableExtraContent = (byte) 140,
    Table = (byte) 141,
    TableRow = (byte) 142,
    TableCell = (byte) 143,
    List = (byte) 144,
    ListItem = (byte) 145,
    Map = (byte) 151,
  }
}
