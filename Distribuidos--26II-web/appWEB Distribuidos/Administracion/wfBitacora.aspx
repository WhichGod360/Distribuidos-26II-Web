<%@ Page Title="Bitácora · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfBitacora.aspx.vb" Inherits="appWEB_Distribuidos.BitacoraWeb" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Bitácora</h1>
    <p class="page-description">Consulta los movimientos registrados por el sistema.</p>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <table class="editor-table filter-panel" role="presentation">
        <tr>
            <td>
                <asp:Label ID="lblDesde" runat="server" AssociatedControlID="txtDesde" Text="Desde" /><asp:TextBox ID="txtDesde" runat="server" TextMode="Date" CssClass="input-field" /></td>
            <td>
                <asp:Label ID="lblHasta" runat="server" AssociatedControlID="txtHasta" Text="Hasta" /><asp:TextBox ID="txtHasta" runat="server" TextMode="Date" CssClass="input-field" /></td>
            <td>
                <asp:Label ID="lblUsuario" runat="server" AssociatedControlID="txtUsuario" Text="ID de usuario (opcional)" /><asp:TextBox ID="txtUsuario" runat="server" CssClass="input-field" MaxLength="10" inputmode="numeric" /></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblObjeto" runat="server" AssociatedControlID="txtObjeto" Text="Objeto" /><asp:TextBox ID="txtObjeto" runat="server" CssClass="input-field" MaxLength="30" /></td>
            <td>
                <asp:Label ID="lblAccion" runat="server" AssociatedControlID="ddlAccion" Text="Acción" /><asp:DropDownList ID="ddlAccion" runat="server" CssClass="input-field">
                    <asp:ListItem Value="">Todas las acciones</asp:ListItem>
                    <asp:ListItem>INSERT</asp:ListItem>
                    <asp:ListItem>UPDATE</asp:ListItem>
                    <asp:ListItem>DELETE</asp:ListItem>
                    <asp:ListItem>SELECT</asp:ListItem>
                    <asp:ListItem>BUSQUEDA</asp:ListItem>
                    <asp:ListItem>LISTADO</asp:ListItem>
                </asp:DropDownList></td>
            <td class="form-actions">
                <asp:Button ID="btnAplicar" runat="server" Text="Aplicar" CssClass="button primary" /><asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="button" /></td>
        </tr>
    </table>
    <div class="quick-actions" data-auto-refresh="3">
        <asp:CheckBox ID="chkAutomatico" runat="server" Text="Actualizar automáticamente cada 3 segundos" />
        <asp:Button ID="btnActualizar" runat="server" Text="Actualizar" CssClass="button" data-refresh-button="true" /></div>
    <p class="field-help">
        <asp:Literal ID="litResumen" runat="server" Mode="Encode" /></p>
    <div class="table-scroll">
        <asp:GridView ID="grvDatos" runat="server" AutoGenerateColumns="false" DataKeyNames="IdMan" CssClass="data-table" GridLines="None" AllowPaging="true" PageSize="20" EmptyDataText="No se encontraron movimientos." ShowHeaderWhenEmpty="true">
            <Columns>
                <asp:ButtonField CommandName="Select" Text="Detalle" />
                <asp:BoundField DataField="IdMan" HeaderText="ID" />
                <asp:BoundField DataField="FechaMan" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="HoraMan" HeaderText="Hora" />
                <asp:BoundField DataField="IdUsuario" HeaderText="Usuario" />
                <asp:BoundField DataField="Objeto" HeaderText="Objeto" />
                <asp:BoundField DataField="Accion" HeaderText="Acción" />
            </Columns>
            <PagerStyle CssClass="grid-pager" />
        </asp:GridView>
    </div>
    <asp:Panel ID="pnlDetalle" runat="server" Visible="false" CssClass="editor-panel">
        <h2>Detalle del movimiento</h2>
        <h3>Datos registrados</h3>
        <pre class="audit-detail"><asp:Literal ID="litCadena" runat="server" Mode="Encode" /></pre>
    </asp:Panel>
</asp:Content>
