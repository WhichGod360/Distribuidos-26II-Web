<%@ Page Title="Usuarios · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfUsuarios.aspx.vb" Inherits="appWEB_Distribuidos.UsuariosWeb" %>

<%@ Register Src="~/Comun/DatosUsuario.ascx" TagPrefix="uc" TagName="DatosUsuario" %>
<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Gestión de usuarios</h1>
    <p class="page-description">Cuentas, roles y estado de acceso a la biblioteca.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <table class="toolbar-table" role="presentation">
        <tr>
            <td>
                <asp:Label ID="lblBuscar" runat="server" AssociatedControlID="txtBuscar" Text="Nombre, DNI o correo" /><asp:TextBox ID="txtBuscar" runat="server" CssClass="input-field" MaxLength="150" /></td>
            <td>
                <asp:Label ID="lblRolFiltro" runat="server" AssociatedControlID="ddlRolFiltro" Text="Rol" /><asp:DropDownList ID="ddlRolFiltro" runat="server" CssClass="input-field" /></td>
            <td>
                <asp:Label ID="lblEstadoFiltro" runat="server" AssociatedControlID="ddlEstadoFiltro" Text="Estado" /><asp:DropDownList ID="ddlEstadoFiltro" runat="server" CssClass="input-field">
                    <asp:ListItem>Todos</asp:ListItem>
                    <asp:ListItem>Activo</asp:ListItem>
                    <asp:ListItem>Inactivo</asp:ListItem>
                </asp:DropDownList></td>
            <td>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="button" /></td>
            <td>
                <asp:Button ID="btnNuevo" runat="server" Text="Agregar" CssClass="button primary" /></td>
        </tr>
    </table>
    <asp:Panel ID="pnlEditor" runat="server" Visible="false" CssClass="editor-panel">
        <h2>
            <asp:Literal ID="litEditor" runat="server" Mode="Encode" /></h2>
        <uc:DatosUsuario ID="campos" runat="server" />
        <table class="editor-table" role="presentation">
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblRol" runat="server" AssociatedControlID="ddlRol" Text="Rol" /></td>
                <td>
                    <asp:DropDownList ID="ddlRol" runat="server" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblEstado" runat="server" AssociatedControlID="ddlEstado" Text="Estado" /></td>
                <td>
                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="input-field">
                        <asp:ListItem>Activo</asp:ListItem>
                        <asp:ListItem>Inactivo</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td colspan="2" class="form-actions">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="button primary" /><asp:Button ID="btnEstado" runat="server" Text="Cambiar estado" CssClass="button" /><asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="button" /></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlConfirmar" runat="server" Visible="false" CssClass="confirm-panel">
        <p>
            <asp:Literal ID="litConfirmar" runat="server" Mode="Encode" /></p>
        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar cambio de estado" CssClass="button primary" /><asp:Button ID="btnNoCambiar" runat="server" Text="Cancelar" CssClass="button" /></asp:Panel>
    <div class="table-scroll">
        <asp:GridView ID="grvDatos" runat="server" AutoGenerateColumns="false" DataKeyNames="IdUsuario" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="15" EmptyDataText="No se encontraron usuarios." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Select" Text="Editar" />
                <asp:BoundField DataField="IdUsuario" HeaderText="ID" />
                <asp:BoundField DataField="NombreCompleto" HeaderText="Usuario" />
                <asp:BoundField DataField="NumeroDniUsuario" HeaderText="DNI" />
                <asp:BoundField DataField="EmailUsuario" HeaderText="Correo" />
                <asp:BoundField DataField="NombreRol" HeaderText="Rol" />
                <asp:BoundField DataField="EstadoUsuario" HeaderText="Estado" />
            </Columns>
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
    </div>
</asp:Content>
