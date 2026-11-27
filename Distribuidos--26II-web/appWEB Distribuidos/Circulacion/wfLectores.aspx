<%@ Page Title="Buscar lectores · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfLectores.aspx.vb" Inherits="appWEB_Distribuidos.BuscarLectores" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Buscar lectores</h1>
    <p class="page-description">Consulta estudiantes activos y su historial de préstamos.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <table class="toolbar-table" role="presentation">
        <tr>
            <td>
                <asp:Label ID="lblBuscar" runat="server" AssociatedControlID="txtBuscar" Text="Nombre, apellido, DNI o correo" /><asp:TextBox ID="txtBuscar" runat="server" CssClass="input-field" MaxLength="150" /></td>
            <td>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="button primary" /></td>
            <td>
                <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="button" /></td>
        </tr>
    </table>
    <div class="table-scroll">
        <asp:GridView ID="grvLectores" runat="server" AutoGenerateColumns="false" DataKeyNames="IdUsuario" CssClass="data-table" GridLines="None" EmptyDataText="Busca un lector para consultar su historial." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Select" Text="Ver historial" />
                <asp:BoundField DataField="NombreCompleto" HeaderText="Lector" />
                <asp:BoundField DataField="NumeroDniUsuario" HeaderText="DNI" />
                <asp:BoundField DataField="EmailUsuario" HeaderText="Correo" />
            </Columns>
        </asp:GridView>
    </div>
    <asp:Panel ID="pnlHistorial" runat="server" Visible="false" CssClass="editor-panel">
        <h2>
            <asp:Literal ID="litLector" runat="server" Mode="Encode" /></h2>
        <div class="quick-actions">
            <asp:HyperLink ID="lnkNuevo" runat="server" Text="Registrar préstamo" CssClass="button primary link-button" /><asp:HyperLink ID="lnkPrestamos" runat="server" Text="Ver préstamos" CssClass="button link-button" /></div>
        <div class="table-scroll">
            <asp:GridView ID="grvPrestamos" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None" EmptyDataText="Este lector todavía no tiene préstamos." ShowHeaderWhenEmpty="true">
                <Columns>
                    <asp:BoundField DataField="IdPrestamo" HeaderText="N.º" />
                    <asp:BoundField DataField="TituloLibro" HeaderText="Libro" />
                    <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="FechaFin" HeaderText="Límite" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="FechaRetorno" HeaderText="Devolución" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>
