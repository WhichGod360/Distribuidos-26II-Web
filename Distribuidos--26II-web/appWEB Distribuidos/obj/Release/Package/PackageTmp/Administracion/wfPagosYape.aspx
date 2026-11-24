<%@ Page Title="Revisar pagos por Yape · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfPagosYape.aspx.vb" Inherits="appWEB_Distribuidos.RevisionYapeWeb" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HyperLink runat="server" NavigateUrl="~/Administracion/wfDetallePenalizaciones.aspx" Text="Volver a Cargos y pagos" />
    <h1>Revisar pagos por Yape</h1>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <asp:Panel ID="pnlBandeja" runat="server" Visible="false">
        <asp:Panel ID="pnlDetalle" runat="server" Visible="false" CssClass="editor-panel">
            <h2><asp:Literal ID="litTitulo" runat="server" Mode="Encode" /></h2>
            <dl class="yape-datos">
                <dt>Lector</dt><dd><asp:Literal ID="litLector" runat="server" Mode="Encode" /></dd>
                <dt>Importe</dt><dd><asp:Literal ID="litMonto" runat="server" Mode="Encode" /></dd>
                <dt>Nombre en Yape</dt><dd><asp:Literal ID="litNombre" runat="server" Mode="Encode" /></dd>
                <dt>Celular</dt><dd><asp:Literal ID="litCelular" runat="server" Mode="Encode" /></dd>
                <dt>Código de seguridad</dt><dd><asp:Literal ID="litCodigo" runat="server" Mode="Encode" /></dd>
                <dt>Estado</dt><dd><asp:Literal ID="litEstado" runat="server" Mode="Encode" /></dd>
            </dl>
            <asp:Panel ID="pnlDecision" runat="server" Visible="false" CssClass="yape-fields">
                <p class="field-help">Comprueba el ingreso en los movimientos de Yape de la cuenta receptora. Coteja el nombre, importe y código, y verifica que el movimiento no se haya usado en otro cargo.</p>
                <asp:CheckBox ID="chkVerificado" runat="server" Text="Verifiqué este ingreso en la cuenta receptora" />
                <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar pago" CssClass="button primary" />
                <asp:Label runat="server" AssociatedControlID="txtMotivo" Text="Motivo de denegación (visible para el lector)" />
                <asp:TextBox ID="txtMotivo" runat="server" CssClass="input-field" TextMode="MultiLine" Rows="3" MaxLength="255" />
                <asp:Button ID="btnDenegar" runat="server" Text="Denegar reporte" CssClass="button" />
            </asp:Panel>
        </asp:Panel>
        <div class="form-actions">
            <asp:Label runat="server" AssociatedControlID="ddlEstado" Text="Estado del reporte" />
            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="input-field">
                <asp:ListItem Value="EnRevision">En revisión</asp:ListItem>
                <asp:ListItem Value="Confirmado">Confirmados</asp:ListItem>
                <asp:ListItem Value="Denegado">Denegados</asp:ListItem>
                <asp:ListItem Value="">Todos</asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="button" />
        </div>
        <div class="table-scroll">
            <asp:GridView ID="grvReportes" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="15" EmptyDataText="No hay reportes en el estado seleccionado.">
                <Columns>
                    <asp:BoundField DataField="IdPago" HeaderText="Reporte" />
                    <asp:BoundField DataField="Lector" HeaderText="Lector" />
                    <asp:BoundField DataField="IdDetalle" HeaderText="Cargo" />
                    <asp:BoundField DataField="Monto" HeaderText="Importe" DataFormatString="S/ {0:N2}" />
                    <asp:BoundField DataField="FechaReporte" HeaderText="Enviado (hora de Perú)" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                    <asp:HyperLinkField Text="Abrir reporte" DataNavigateUrlFields="IdPago" DataNavigateUrlFormatString="~/Administracion/wfPagosYape.aspx?id={0}" />
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>
