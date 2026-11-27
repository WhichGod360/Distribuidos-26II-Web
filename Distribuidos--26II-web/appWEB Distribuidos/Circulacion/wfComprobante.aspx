<%@ Page Title="Comprobante · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfComprobante.aspx.vb" Inherits="appWEB_Distribuidos.ComprobanteWeb" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Comprobante de biblioteca</h1>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" /><asp:Panel ID="pnlComprobante" runat="server" Visible="false" CssClass="receipt">
        <h2>
            <asp:Literal ID="litTipo" runat="server" Mode="Encode" /></h2>
        <p>
            <asp:Literal ID="litReferencia" runat="server" Mode="Encode" /></p>
        <p class="field-help">
            <asp:Literal ID="litEmision" runat="server" Mode="Encode" /></p>
        <div class="table-scroll">
            <asp:GridView ID="grvPrestamos" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="IdPrestamo" HeaderText="Préstamo" />
                    <asp:BoundField DataField="NombreLector" HeaderText="Lector" />
                    <asp:BoundField DataField="TituloLibro" HeaderText="Libro" />
                    <asp:BoundField DataField="IdEjemplar" HeaderText="Ejemplar" />
                    <asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="FechaFin" HeaderText="Límite" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="FechaRetorno" HeaderText="Devolución" DataFormatString="{0:dd/MM/yyyy}" />
                </Columns>
            </asp:GridView>
        </div>
        <asp:Panel ID="pnlCargos" runat="server" Visible="false">
            <h3>Cargos registrados</h3>
            <asp:GridView ID="grvCargos" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
                    <asp:BoundField DataField="MontoPenalizacion" HeaderText="Importe" DataFormatString="S/ {0:N2}" />
                    <asp:BoundField DataField="EstadoPagoTexto" HeaderText="Estado de pago" />
                </Columns>
            </asp:GridView>
        </asp:Panel>
        <p class="summary-strip">
            <asp:Literal ID="litTotal" runat="server" Mode="Encode" /></p>
        <p class="field-help">Este comprobante refleja el registro de biblioteca y los estados de pago indicados.</p>
    </asp:Panel>
</asp:Content>
