<%@ Page Title="Panel de biblioteca · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfInicio.aspx.vb" Inherits="appWEB_Distribuidos.PanelInicio" %>

<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="heading-table" role="presentation">
        <tr>
            <td class="module-image">
                <img src="Recursos/lecturae.png" alt="" /></td>
            <td>
                <h1>
                    <asp:Literal ID="litTitulo" runat="server" Mode="Encode" /></h1>
                <p>Actividad, disponibilidad y atención diaria de la biblioteca.</p>
            </td>
        </tr>
    </table>
    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
    <div class="table-scroll">
        <table class="filter-table" role="presentation">
            <colgroup>
                <col />
                <col />
                <col />
                <col />
                <col style="width: 90px" />
                <col style="width: 90px" />
            </colgroup>
            <tr>
                <td>
                    <asp:Label ID="lblDesde" runat="server" AssociatedControlID="txtDesde" Text="Préstamos desde" /></td>
                <td>
                    <asp:Label ID="lblHasta" runat="server" AssociatedControlID="txtHasta" Text="Hasta" /></td>
                <td>
                    <asp:Label ID="lblCategoria" runat="server" AssociatedControlID="ddlCategoria" Text="Categoría" /></td>
                <td>
                    <asp:Label ID="lblEstado" runat="server" AssociatedControlID="ddlEstado" Text="Estado" /></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtDesde" runat="server" TextMode="Date" CssClass="input-field" /></td>
                <td>
                    <asp:TextBox ID="txtHasta" runat="server" TextMode="Date" CssClass="input-field" /></td>
                <td>
                    <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="input-field" /></td>
                <td>
                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="input-field">
                        <asp:ListItem>Todos</asp:ListItem>
                        <asp:ListItem>Activos</asp:ListItem>
                        <asp:ListItem>Vencidos</asp:ListItem>
                        <asp:ListItem>Devueltos</asp:ListItem>
                    </asp:DropDownList></td>
                <td>
                    <asp:Button ID="btnActualizar" runat="server" Text="Aplicar" CssClass="button primary" /></td>
                <td>
                    <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="button" /></td>
            </tr>
        </table>
    </div>
    <div class="quick-actions">
        <asp:HyperLink ID="lnkNuevo" runat="server" CssClass="button primary link-button" /><asp:HyperLink ID="lnkDevolver" runat="server" CssClass="button link-button" /><asp:HyperLink ID="lnkLectores" runat="server" NavigateUrl="~/Circulacion/wfLectores.aspx" Text="Buscar lectores" CssClass="button link-button" /><asp:HyperLink ID="lnkLibros" runat="server" NavigateUrl="~/Catalogo/wfLibros.aspx" Text="Consultar libros" CssClass="button link-button" /></div>
    <table class="stats-table" role="presentation">
        <tr>
            <td>
                <h2>
                    <asp:Literal ID="litCap0" runat="server" Mode="Encode" /></h2>
                <p class="stat-number">
                    <asp:Literal ID="litNum0" runat="server" Mode="Encode" /></p>
                <p>En el periodo seleccionado</p>
            </td>
            <td>
                <h2>
                    <asp:Literal ID="litCap1" runat="server" Mode="Encode" /></h2>
                <p class="stat-number">
                    <asp:Literal ID="litNum1" runat="server" Mode="Encode" /></p>
                <p>En el periodo seleccionado</p>
            </td>
            <td>
                <h2>
                    <asp:Literal ID="litCap2" runat="server" Mode="Encode" /></h2>
                <p class="stat-number">
                    <asp:Literal ID="litNum2" runat="server" Mode="Encode" /></p>
                <p>En el periodo seleccionado</p>
            </td>
            <td>
                <h2>Ejemplares disponibles</h2>
                <p class="stat-number">
                    <asp:Literal ID="litDisponibles" runat="server" Mode="Encode" /></p>
                <p>Ahora · categoría elegida</p>
            </td>
        </tr>
    </table>
    <p class="field-help">
        <asp:Literal ID="litResumen" runat="server" Mode="Encode" /></p>
    <table class="panels-table" role="presentation">
        <tr>
            <td>
                <section class="panel">
                    <h2>Préstamos por mes de inicio</h2>
                    <asp:Literal ID="litSinMeses" runat="server" Mode="Encode" /><asp:Repeater ID="rptMeses" runat="server">
                        <ItemTemplate>
                            <div class="chart-row"><span><%#: Eval("Etiqueta") %></span><meter min="0" max='<%#: Eval("Maximo") %>' value='<%#: Eval("Cantidad") %>'><%#: Eval("Cantidad") %></meter><strong><%#: Eval("Cantidad") %></strong></div>
                        </ItemTemplate>
                    </asp:Repeater>
                </section>
            </td>
            <td>
                <section class="panel">
                    <h2>Estado de la selección</h2>
                    <asp:GridView ID="grvEstados" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Préstamos" />
                        </Columns>
                    </asp:GridView>
                </section>
            </td>
        </tr>
    </table>
    <table class="panels-table" role="presentation">
        <tr>
            <td>
                <section class="panel">
                    <h2>Próximas devoluciones de la selección</h2>
                    <div class="table-scroll">
                        <asp:GridView ID="grvDevoluciones" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None" EmptyDataText="No hay devoluciones pendientes en esta selección." ShowHeaderWhenEmpty="true">
                            <Columns>
                                <asp:BoundField DataField="Lector" HeaderText="Lector" />
                                <asp:HyperLinkField DataNavigateUrlFields="IdPrestamo" DataNavigateUrlFormatString="~/Circulacion/wfPrestamos.aspx?buscar={0}" Text="Ver" HeaderText="Detalle" />
                                <asp:BoundField DataField="Libro" HeaderText="Libro" />
                                <asp:BoundField DataField="FechaLimite" HeaderText="Límite" DataFormatString="{0:dd/MM/yyyy}" />
                                <asp:BoundField DataField="Estado" HeaderText="Estado" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </section>
            </td>
            <td>
                <section class="panel">
                    <h2>Préstamos por categoría</h2>
                    <asp:GridView ID="grvCategorias" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None" EmptyDataText="No hay préstamos en esta selección." ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:BoundField DataField="Etiqueta" HeaderText="Categoría" />
                            <asp:BoundField DataField="Cantidad" HeaderText="Préstamos" />
                        </Columns>
                    </asp:GridView>
                    <p class="panel-note">Un préstamo puede pertenecer a varias categorías.</p>
                </section>
            </td>
        </tr>
    </table>
</asp:Content>
