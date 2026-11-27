<%@ Page Title="Solicitudes y reservas · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfSolicitudes.aspx.vb" Inherits="appWEB_Distribuidos.SolicitudesWeb" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Solicitudes y reservas</h1>
    <p class="page-description">Revisa las solicitudes de los lectores y prepara su entrega presencial.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <table class="toolbar-table" role="presentation">
        <tr>
            <td>
                <asp:Label ID="lblBuscar" runat="server" AssociatedControlID="txtBuscar" Text="Lector, DNI o libro" /><asp:TextBox ID="txtBuscar" runat="server" CssClass="input-field" MaxLength="150" /></td>
            <td>
                <asp:Label ID="lblEstado" runat="server" AssociatedControlID="ddlEstado" Text="Estado" /><asp:DropDownList ID="ddlEstado" runat="server" CssClass="input-field">
                    <asp:ListItem>Todos</asp:ListItem>
                    <asp:ListItem>Pendiente</asp:ListItem>
                    <asp:ListItem>Aprobada</asp:ListItem>
                    <asp:ListItem>Rechazada</asp:ListItem>
                    <asp:ListItem>Atendida</asp:ListItem>
                    <asp:ListItem>Cancelada</asp:ListItem>
                    <asp:ListItem>Vencida</asp:ListItem>
                </asp:DropDownList></td>
            <td>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="button" /></td>
        </tr>
    </table>
    <asp:Panel ID="pnlDetalle" runat="server" Visible="false" CssClass="editor-panel">
        <h2>
            <asp:Literal ID="litDetalle" runat="server" Mode="Encode" /></h2>
        <p class="field-help">
            <asp:Literal ID="litDatos" runat="server" Mode="Encode" /></p>
        <div class="quick-actions">
            <asp:Button ID="btnAprobar" runat="server" Text="Aprobar" CssClass="button primary" /><asp:Button ID="btnRechazar" runat="server" Text="Rechazar" CssClass="button danger" /><asp:Button ID="btnPreparar" runat="server" Text="Preparar entrega" CssClass="button primary" /><asp:HyperLink ID="lnkComprobante" runat="server" Text="Ver comprobante" CssClass="button link-button" /><asp:Button ID="btnCerrar" runat="server" Text="Cerrar" CssClass="button" /></div>
        <asp:Panel ID="pnlConfirmar" runat="server" Visible="false" CssClass="confirm-panel">
            <p>
                <asp:Literal ID="litConfirmar" runat="server" Mode="Encode" /></p>
            <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" CssClass="button primary" /><asp:Button ID="btnCancelarAccion" runat="server" Text="Cancelar" CssClass="button" /></asp:Panel>
        <asp:Panel ID="pnlEntrega" runat="server" Visible="false">
            <h2>Selecciona un ejemplar por título</h2>
            <p class="field-help">La entrega registra los préstamos con plazo de cinco días.</p>
            <div class="table-scroll">
                <asp:GridView ID="grvEntrega" runat="server" AutoGenerateColumns="false" DataKeyNames="IdLibro" CssClass="data-table" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="TituloLibro" HeaderText="Libro" />
                        <asp:TemplateField HeaderText="Ejemplar disponible">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlCopia" runat="server" CssClass="input-field" /></ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
            <asp:Button ID="btnEntregar" runat="server" Text="Confirmar entrega y registrar préstamos" CssClass="button primary" /></asp:Panel>
    </asp:Panel>
    <div class="table-scroll">
        <asp:GridView ID="grvDatos" runat="server" AutoGenerateColumns="false" DataKeyNames="IdSolicitud" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="15" EmptyDataText="No se encontraron solicitudes." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Select" Text="Revisar" />
                <asp:BoundField DataField="IdSolicitud" HeaderText="N.º" />
                <asp:BoundField DataField="NombreLector" HeaderText="Lector" />
                <asp:BoundField DataField="Libros" HeaderText="Libros" />
                <asp:TemplateField HeaderText="Solicitada">
                    <ItemTemplate><%#: FechaSolicitudLima(Container.DataItem) %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="LimiteLocal" HeaderText="Límite de recojo" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:BoundField DataField="EstadoActual" HeaderText="Estado" />
            </Columns>
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
    </div>
</asp:Content>
