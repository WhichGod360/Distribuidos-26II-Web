<%@ Page Title="Pagar penalización · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfPagarYape.aspx.vb" Inherits="appWEB_Distribuidos.PagarYapeWeb" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HyperLink runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=cargos" Text="Volver a Mis penalizaciones" />
    <h1>Pagar penalización</h1>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <asp:Panel ID="pnlCargo" runat="server" Visible="false">
        <p class="summary-strip"><asp:Literal ID="litCargo" runat="server" Mode="Encode" /></p>
        <asp:Panel ID="pnlPago" runat="server" Visible="false" CssClass="editor-panel" DefaultButton="btnEnviar">
            <div class="yape-layout">
                <section>
                    <h2>Pago en Yape</h2>
                    <asp:Image runat="server" CssClass="yape-qr" ImageUrl="~/Lector/YapeQr.ashx" AlternateText="Código QR de Yape" />
                </section>
                <section class="yape-fields">
                    <h2>Envía tu reporte</h2>
                    <p class="field-help">Completa los datos después de yapear. Si ya hiciste la transferencia, usa el mismo comprobante para corregir tu reporte.</p>
                    <asp:Label runat="server" AssociatedControlID="txtNombre" Text="Nombre que aparece en el comprobante" />
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="input-field" MaxLength="120" autocomplete="off" />
                    <asp:Label runat="server" AssociatedControlID="txtCelular" Text="Celular de quien envió el Yape" />
                    <asp:TextBox ID="txtCelular" runat="server" CssClass="input-field" MaxLength="9" inputmode="numeric" autocomplete="off" />
                    <asp:Label runat="server" AssociatedControlID="txtCodigo" Text="Código de seguridad del comprobante (3 dígitos)" />
                    <asp:TextBox ID="txtCodigo" runat="server" CssClass="input-field" MaxLength="3" inputmode="numeric" autocomplete="off" />
                    <asp:Button ID="btnEnviar" runat="server" Text="Enviar reporte" CssClass="button primary" />
                </section>
            </div>
        </asp:Panel>
        <asp:Panel ID="pnlDemo" runat="server" Visible="false" CssClass="editor-panel">
            <h2>Pago de demostración</h2>
            <p>Escanea el QR y abre el enlace. La confirmación es simulada y no transfiere dinero.</p>
            <asp:Image ID="imgPagoDemo" runat="server" CssClass="yape-qr" AlternateText="QR de pago de demostración" />
            <p><asp:HyperLink ID="lnkPagoDemo" runat="server" Text="Abrir demostración en este dispositivo" /></p>
        </asp:Panel>
        <asp:Panel ID="pnlHistorial" runat="server" Visible="false">
        <h2>Historial de reportes</h2>
        <div class="table-scroll">
            <asp:GridView ID="grvHistorial" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None" EmptyDataText="Aún no has reportado un pago por Yape para este cargo.">
                <Columns>
                    <asp:BoundField DataField="IdPago" HeaderText="Reporte" />
                    <asp:BoundField DataField="FechaReporte" HeaderText="Enviado (hora de Perú)" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="Monto" HeaderText="Importe" DataFormatString="S/ {0:N2}" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    <asp:BoundField DataField="Motivo" HeaderText="Motivo de denegación" />
                </Columns>
            </asp:GridView>
        </div>
        <p><asp:HyperLink runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=avisos" Text="Ver avisos" /></p>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
