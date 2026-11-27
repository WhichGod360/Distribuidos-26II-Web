<%@ Page Title="Lectores pendientes · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfLectoresPendientes.aspx.vb" Inherits="appWEB_Distribuidos.LectoresPendientesWeb" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Lectores pendientes de aprobación</h1>
    <p class="page-description">Cuentas de lector registradas que aún no pueden iniciar sesión hasta ser aprobadas.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <table class="toolbar-table" role="presentation">
        <tr>
            <td>
                <asp:Label ID="lblBuscar" runat="server" AssociatedControlID="txtBuscar" Text="Nombre, DNI o correo" /><asp:TextBox ID="txtBuscar" runat="server" CssClass="input-field" MaxLength="150" /></td>
            <td>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="button" /></td>
        </tr>
    </table>
    <asp:Panel ID="pnlConfirmar" runat="server" Visible="false" CssClass="confirm-panel">
        <p>
            <asp:Literal ID="litConfirmar" runat="server" Mode="Encode" /></p>
        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar rechazo" CssClass="button primary" /><asp:Button ID="btnNoRechazar" runat="server" Text="Cancelar" CssClass="button" /></asp:Panel>
    <div class="table-scroll">
        <asp:GridView ID="grvDatos" runat="server" AutoGenerateColumns="false" DataKeyNames="IdUsuario" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="15" EmptyDataText="No hay lectores pendientes de aprobación." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Aprobar" Text="Aprobar" />
                <asp:ButtonField CommandName="Rechazar" Text="Rechazar" />
                <asp:BoundField DataField="IdUsuario" HeaderText="ID" />
                <asp:BoundField DataField="NombreCompleto" HeaderText="Lector" />
                <asp:BoundField DataField="NumeroDniUsuario" HeaderText="DNI" />
                <asp:BoundField DataField="EmailUsuario" HeaderText="Correo" />
            </Columns>
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
    </div>
</asp:Content>
