<%@ Page Title="Cargos y pagos · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfDetallePenalizaciones.aspx.vb" Inherits="appWEB_Distribuidos.DetallePenalizacionesWeb" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server">
    <style>
        @media (min-width: 761px) {
            .toolbar-cargos td:first-child {
                width: 38%;
            }

            .toolbar-cargos td:nth-child(2) {
                min-width: 11rem;
            }

            .toolbar-cargos td:nth-child(3) {
                min-width: 7.5rem;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Cargos y estados de pago</h1>
    <asp:HyperLink runat="server" NavigateUrl="~/Administracion/wfPagosYape.aspx" Text="Revisar pagos por Yape" /><p class="page-description">Detalle de penalizaciones asociadas a préstamos y regularización en ventanilla.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <table class="toolbar-table toolbar-cargos" role="presentation">
        <tr>
            <td>
                <asp:Label ID="lblBuscar" runat="server" AssociatedControlID="txtBuscar" Text="Lector, libro, préstamo o motivo" />
                <asp:TextBox ID="txtBuscar" runat="server" CssClass="input-field" MaxLength="150" />
            </td>
            <td>
                <asp:Label ID="lblTipoFiltro" runat="server" AssociatedControlID="ddlTipoFiltro" Text="Penalización" />
                <asp:DropDownList ID="ddlTipoFiltro" runat="server" CssClass="input-field" />
            </td>
            <td>
                <asp:Label ID="lblPagoFiltro" runat="server" AssociatedControlID="ddlPagoFiltro" Text="Estado de pago" />
                <asp:DropDownList ID="ddlPagoFiltro" runat="server" AutoPostBack="True" CssClass="input-field">
                    <asp:ListItem Value="">Todos</asp:ListItem>
                    <asp:ListItem>Pendiente</asp:ListItem>
                    <asp:ListItem Value="PorRevisar">Por revisar</asp:ListItem>
                    <asp:ListItem>Pagada</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="button" /></td>
            <td>
                <asp:Button ID="btnNuevo" runat="server" Text="Agregar cargo" CssClass="button primary" /></td>
        </tr>
    </table>
    <asp:Panel ID="pnlEditor" runat="server" Visible="false" CssClass="editor-panel">
        <h2>
            <asp:Literal ID="litEditor" runat="server" Mode="Encode" /></h2>
        <p class="field-help">
            <asp:Literal ID="litImporte" runat="server" Mode="Encode" /></p>
        <table class="editor-table" role="presentation">
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblTipo" runat="server" AssociatedControlID="ddlTipo" Text="Tipo de penalización" /></td>
                <td>
                    <asp:DropDownList ID="ddlTipo" runat="server" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblPrestamo" runat="server" AssociatedControlID="ddlPrestamo" Text="Préstamo" /></td>
                <td>
                    <asp:DropDownList ID="ddlPrestamo" runat="server" CssClass="input-field" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblMotivo" runat="server" AssociatedControlID="txtMotivo" Text="Motivo" /></td>
                <td>
                    <asp:TextBox ID="txtMotivo" runat="server" CssClass="input-field" TextMode="MultiLine" Rows="3" MaxLength="255" /></td>
            </tr>
            <tr>
                <td class="field-label">
                    <asp:Label ID="lblPago" runat="server" AssociatedControlID="ddlPago" Text="Estado de pago" /></td>
                <td>
                    <asp:DropDownList ID="ddlPago" runat="server" CssClass="input-field">
                        <asp:ListItem>Pendiente</asp:ListItem>
                        <asp:ListItem Value="PorRevisar">Por revisar</asp:ListItem>
                        <asp:ListItem>Pagada</asp:ListItem>
                    </asp:DropDownList></td>
            </tr>
            <tr>
                <td colspan="2" class="form-actions">
                    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="button primary" /><asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="button" /></td>
            </tr>
        </table>
    </asp:Panel>
    <div class="table-scroll">
        <asp:GridView ID="grvDatos" runat="server" AutoGenerateColumns="false" DataKeyNames="IdDetallePenalizacion" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="15" EmptyDataText="No se encontraron cargos." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Select" Text="Editar" />
                <asp:BoundField DataField="IdDetallePenalizacion" HeaderText="ID" />
                <asp:BoundField DataField="TipoPenalizacion" HeaderText="Penalización" />
                <asp:BoundField DataField="MontoPenalizacion" HeaderText="Importe" DataFormatString="S/ {0:N2}" />
                <asp:BoundField DataField="IdPrestamo" HeaderText="Préstamo" />
                <asp:BoundField DataField="NombreUsuario" HeaderText="Lector" />
                <asp:BoundField DataField="TituloLibro" HeaderText="Libro" />
                <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
                <asp:BoundField DataField="EstadoPagoTexto" HeaderText="Pago" />
            </Columns>
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
    </div>
</asp:Content>
