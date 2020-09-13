<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DirectoryMonitor.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">
    <p>
      PATH TO LOCAL DIRECTORY
    </p>
    <p>
      <asp:TextBox ID="TextBox1" runat="server" Width="500px"></asp:TextBox>
      <asp:Button ID="Button1" runat="server" Text="Start" Width="100px" />
    </p>
    <asp:Label ID="Label1" runat="server" Text="žádný adresář" Height="200px" Width="600px"></asp:Label>
  </form>
</body>
</html>
