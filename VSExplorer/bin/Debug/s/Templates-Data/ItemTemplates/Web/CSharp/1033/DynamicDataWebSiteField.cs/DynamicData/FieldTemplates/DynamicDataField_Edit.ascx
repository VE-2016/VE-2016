<%@ Control Language="C#" ClassName="$safeitemrootname$" Inherits="System.Web.DynamicData.FieldTemplateUserControl" %>

<asp:TextBox ID="TextBox1" runat="server" Text='<%# FieldValueEditString %>'></asp:TextBox>

<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator1" ControlToValidate="TextBox1" Display="Dynamic" Enabled="false" />
<asp:RegularExpressionValidator runat="server" ID="RegularExpressionValidator1" ControlToValidate="TextBox1" Display="Dynamic" Enabled="false" />
<asp:DynamicValidator runat="server" ID="DynamicValidator1" ControlToValidate="TextBox1" Display="Dynamic" />

<script runat="server">
    protected void Page_Load(object sender, EventArgs e) {
        TextBox1.MaxLength = Column.MaxLength;
        if (Column.MaxLength < 20)
            TextBox1.Columns = Column.MaxLength;
        TextBox1.ToolTip = Column.Description;

        SetUpValidator(RequiredFieldValidator1);
        SetUpValidator(RegularExpressionValidator1);
        SetUpValidator(DynamicValidator1);
    }
    
    protected override void ExtractValues(IOrderedDictionary dictionary) {
        dictionary[Column.Name] = ConvertEditedValue(TextBox1.Text);
    }

    public override Control DataControl {
        get {
            return TextBox1;
        }
    }
</script>
