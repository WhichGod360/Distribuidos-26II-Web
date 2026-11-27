<%@ Page Title="Autores · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfAutores.aspx.vb" Inherits="appWEB_Distribuidos.Autores" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Autores</h1>
    <p class="page-description">Gestiona los autores y sus datos biográficos.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" /><table class="toolbar-table" role="presentation">
        <tr>
            <td>
                <asp:Label ID="lblBuscar" runat="server" Text="Buscar" AssociatedControlID="txtBuscar" /><asp:TextBox ID="txtBuscar" runat="server" CssClass="input-field" MaxLength="150" /></td>
            <td>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="button" /></td>
            <td>
                <asp:Button ID="btnNuevo" runat="server" Text="Agregar" CssClass="button primary" /></td>
        </tr>
    </table>
    <asp:Panel ID="pnlEditor" runat="server" Visible="false" CssClass="editor-panel">
        <h2>
            <asp:Literal ID="litEditor" runat="server" Mode="Encode" Text="Agregar registro" /></h2>
        <table class="editor-table" role="presentation">
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtNombre" runat="server" AssociatedControlID="txtNombre" Text="Nombre" /></td>
                <td>
                    <asp:TextBox ID="txtNombre" runat="server" MaxLength="150" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtNacionalidad" runat="server" AssociatedControlID="txtNacionalidad" Text="Nacionalidad" /></td>
                <td>
                    <asp:TextBox ID="txtNacionalidad" runat="server" MaxLength="100" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtNacimiento" runat="server" AssociatedControlID="txtNacimiento" Text="Fecha de nacimiento" /></td>
                <td>
                    <asp:TextBox ID="txtNacimiento" runat="server" MaxLength="10" CssClass="input-field" TextMode="Date" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtFallecimiento" runat="server" AssociatedControlID="txtFallecimiento" Text="Fecha de fallecimiento" /></td>
                <td>
                    <asp:TextBox ID="txtFallecimiento" runat="server" MaxLength="10" CssClass="input-field" TextMode="Date" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtBiografia" runat="server" AssociatedControlID="txtBiografia" Text="Biografía" /></td>
                <td>
                    <asp:TextBox ID="txtBiografia" runat="server" MaxLength="4000" CssClass="input-field" TextMode="MultiLine" Rows="4" /></td>
            </tr>
            <tr>
                <td colspan="2" class="form-actions">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="button primary" /><asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="button" /><asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CssClass="button danger" Visible="false" /></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlConfirmar" runat="server" Visible="false" CssClass="confirm-panel">
        <p>
            <asp:Literal ID="litConfirmacion" runat="server" Mode="Encode" /></p>
        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar eliminar" CssClass="button danger" /><asp:Button ID="btnNoEliminar" runat="server" Text="Cancelar" CssClass="button" /></asp:Panel>
    <p class="field-help">No se puede eliminar un autor asociado a libros.</p>
    <div class="table-scroll">
        <asp:GridView ID="grvDatos" runat="server" AutoGenerateColumns="false" DataKeyNames="IdAutor" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="10" EmptyDataText="No se encontraron registros." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Select" Text="Editar" HeaderText="Acciones" />
                <asp:BoundField DataField="IdAutor" HeaderText="ID" />
                <asp:BoundField DataField="NombreAutor" HeaderText="Nombre" />
                <asp:BoundField DataField="NacionalidadAutor" HeaderText="Nacionalidad" />
                <asp:BoundField DataField="FechaNacimientoAutor" HeaderText="Nacimiento" DataFormatString="{0:dd/MM/yyyy}" />
            </Columns>
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
    </div>
</asp:Content>
