<%@ Control Language="VB" AutoEventWireup="false" CodeBehind="DatosUsuario.ascx.vb" Inherits="appWEB_Distribuidos.DatosUsuario" %>
<table class="editor-table" role="presentation">
    <tr>
        <td class="field-label">
            <asp:Label ID="lblNombre" runat="server" AssociatedControlID="txtNombre" Text="Nombres" /></td>
        <td>
            <asp:TextBox ID="txtNombre" runat="server" CssClass="input-field" MaxLength="100" autocomplete="given-name" /></td>
    </tr>
    <tr>
        <td class="field-label">
            <asp:Label ID="lblPaterno" runat="server" AssociatedControlID="txtPaterno" Text="Apellido paterno" /></td>
        <td>
            <asp:TextBox ID="txtPaterno" runat="server" CssClass="input-field" MaxLength="100" /></td>
    </tr>
    <tr>
        <td class="field-label">
            <asp:Label ID="lblMaterno" runat="server" AssociatedControlID="txtMaterno" Text="Apellido materno" /></td>
        <td>
            <asp:TextBox ID="txtMaterno" runat="server" CssClass="input-field" MaxLength="100" /></td>
    </tr>
    <tr>
        <td class="field-label">
            <asp:Label ID="lblDni" runat="server" AssociatedControlID="txtDni" Text="DNI" /></td>
        <td>
            <asp:TextBox ID="txtDni" runat="server" CssClass="input-field" MaxLength="8" inputmode="numeric" /></td>
    </tr>
    <tr>
        <td class="field-label">
            <asp:Label ID="lblCorreo" runat="server" AssociatedControlID="txtCorreo" Text="Correo electrónico" /></td>
        <td>
            <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" CssClass="input-field" MaxLength="100" autocomplete="email" /></td>
    </tr>
    <tr>
        <td class="field-label">
            <asp:Label ID="lblNacimiento" runat="server" AssociatedControlID="txtNacimiento" Text="Fecha de nacimiento" /></td>
        <td>
            <asp:TextBox ID="txtNacimiento" runat="server" TextMode="Date" CssClass="input-field" /></td>
    </tr>
    <tr>
        <td class="field-label">
            <asp:Label ID="lblGenero" runat="server" AssociatedControlID="ddlGenero" Text="Género" /></td>
        <td>
            <asp:DropDownList ID="ddlGenero" runat="server" CssClass="input-field">
                <asp:ListItem Value="">Selecciona una opción</asp:ListItem>
                <asp:ListItem Value="1">Masculino</asp:ListItem>
                <asp:ListItem Value="0">Femenino</asp:ListItem>
            </asp:DropDownList></td>
    </tr>
</table>
<asp:Panel ID="pnlAcceso" runat="server" CssClass="password-fields">
    <h3>Datos de acceso</h3>
    <table class="editor-table" role="presentation">
        <tr>
            <td class="field-label">
                <asp:Label ID="lblClave" runat="server" AssociatedControlID="txtClave" Text="Contraseña" /></td>
            <td>
                <asp:TextBox ID="txtClave" runat="server" TextMode="Password" CssClass="input-field" MaxLength="128" autocomplete="new-password" data-password="true" /><p class="field-help" data-password-help="true">Mínimo 8 caracteres, una mayúscula y un número.</p>
            </td>
        </tr>
        <tr>
            <td class="field-label">
                <asp:Label ID="lblConfirmar" runat="server" AssociatedControlID="txtConfirmar" Text="Confirmar contraseña" /></td>
            <td>
                <asp:TextBox ID="txtConfirmar" runat="server" TextMode="Password" CssClass="input-field" MaxLength="128" autocomplete="new-password" /></td>
        </tr>
        <tr>
            <td class="field-label">
                <asp:Label ID="lblPregunta" runat="server" AssociatedControlID="txtPregunta" Text="Pregunta de seguridad" /></td>
            <td>
                <asp:TextBox ID="txtPregunta" runat="server" CssClass="input-field" MaxLength="255" /></td>
        </tr>
        <tr>
            <td class="field-label">
                <asp:Label ID="lblRespuesta" runat="server" AssociatedControlID="txtRespuesta" Text="Respuesta de seguridad" /></td>
            <td>
                <asp:TextBox ID="txtRespuesta" runat="server" TextMode="Password" CssClass="input-field" MaxLength="200" autocomplete="off" /></td>
        </tr>
    </table>
</asp:Panel>
