<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="wfRegistro.aspx.vb" Inherits="appWEB_Distribuidos.RegistroLectorWeb" %>

<%@ Register Src="~/Comun/DatosUsuario.ascx" TagPrefix="uc" TagName="DatosUsuario" %>
<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Crear cuenta · Lecturae</title>
    <link rel="stylesheet" href="../Estilos/biblioteca.css" />
    <script src="../Scripts/biblioteca.js" defer="defer"></script>
</head>
<body class="auth-body">
    <form id="form1" runat="server">
        <main class="register-layout">
            <a href="../Default.aspx">Volver al inicio de sesión</a><h1>Crear cuenta de lector</h1>
            <p class="page-description">Únete a Lecturae para consultar el catálogo y solicitar libros.</p>
            <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" /><asp:Panel ID="pnlFormulario" runat="server">
                <uc:DatosUsuario ID="campos" runat="server" />
                <asp:Button ID="btnRegistrar" runat="server" Text="Crear mi cuenta" CssClass="button primary" />
            </asp:Panel>
            <asp:HyperLink ID="lnkIngresar" runat="server" NavigateUrl="~/Default.aspx" Text="Iniciar sesión" Visible="false" CssClass="button primary link-button" /></main>
    </form>
</body>
</html>
