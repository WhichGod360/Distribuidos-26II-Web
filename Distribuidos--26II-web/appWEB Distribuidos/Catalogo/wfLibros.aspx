<%@ Page Title="Libros y ejemplares · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfLibros.aspx.vb" Inherits="appWEB_Distribuidos.Libros" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Libros y ejemplares</h1>
    <p class="page-description">Gestiona los títulos, sus autores, categorías y copias físicas.</p>
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
                    <asp:Label ID="lbltxtTitulo" runat="server" AssociatedControlID="txtTitulo" Text="Título" /></td>
                <td>
                    <asp:TextBox ID="txtTitulo" runat="server" MaxLength="200" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtEditorial" runat="server" AssociatedControlID="txtEditorial" Text="Editorial" /></td>
                <td>
                    <asp:TextBox ID="txtEditorial" runat="server" MaxLength="150" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtAnio" runat="server" AssociatedControlID="txtAnio" Text="Año de publicación" /></td>
                <td>
                    <asp:TextBox ID="txtAnio" runat="server" MaxLength="4" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblddlPublico" runat="server" AssociatedControlID="ddlPublico" Text="Público" /></td>
                <td>
                    <asp:DropDownList ID="ddlPublico" runat="server" CssClass="input-field">
                        <asp:ListItem>General</asp:ListItem>
                        <asp:ListItem>Universitario</asp:ListItem>
                        <asp:ListItem>Infantil</asp:ListItem>
                        <asp:ListItem>Juvenil</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblchkActivo" runat="server" AssociatedControlID="chkActivo" Text="Estado" /></td>
                <td>
                    <asp:CheckBox ID="chkActivo" runat="server" Text="Activo" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbllstAutores" runat="server" AssociatedControlID="lstAutores" Text="Autores" /></td>
                <td>
                    <div class="check-list">
                        <asp:CheckBoxList ID="lstAutores" runat="server" RepeatLayout="Table" /></div>
                </td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbllstCategorias" runat="server" AssociatedControlID="lstCategorias" Text="Categorías" /></td>
                <td>
                    <div class="check-list">
                        <asp:CheckBoxList ID="lstCategorias" runat="server" RepeatLayout="Table" /></div>
                </td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lbltxtCantidad" runat="server" AssociatedControlID="txtCantidad" Text="Ejemplares iniciales" /></td>
                <td>
                    <asp:TextBox ID="txtCantidad" runat="server" MaxLength="3" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:HyperLink ID="lnkEjemplares" runat="server" Text="Gestionar ejemplares de este libro" Visible="false" /></td>
            </tr>
            <tr>
                <td colspan="2" class="form-actions">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="button primary" /><asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="button" /><asp:Button ID="btnEliminar" runat="server" Text="Desactivar" CssClass="button danger" Visible="false" /></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="pnlConfirmar" runat="server" Visible="false" CssClass="confirm-panel">
        <p>
            <asp:Literal ID="litConfirmacion" runat="server" Mode="Encode" /></p>
        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar desactivar" CssClass="button danger" /><asp:Button ID="btnNoEliminar" runat="server" Text="Cancelar" CssClass="button" /></asp:Panel>
    <p class="field-help">La baja conserva el historial. No se desactivan libros con préstamos registrados o reservas vigentes. Las copias se gestionan en Ejemplares.</p>
    <div class="table-scroll">
        <asp:GridView ID="grvDatos" runat="server" AutoGenerateColumns="false" DataKeyNames="IdLibro" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="10" EmptyDataText="No se encontraron registros." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Select" Text="Editar" HeaderText="Acciones" />
                <asp:BoundField DataField="IdLibro" HeaderText="ID" />
                <asp:BoundField DataField="TituloLibro" HeaderText="Título" />
                <asp:BoundField DataField="EditorialLibro" HeaderText="Editorial" />
                <asp:BoundField DataField="Autor" HeaderText="Autores" />
                <asp:BoundField DataField="Categoria" HeaderText="Categorías" />
                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate><%#: If(CBool(Eval("EstadoLibro")), "Activo", "Inactivo") %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StockTotal" HeaderText="Ejemplares" />
            </Columns>
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
    </div>
</asp:Content>
