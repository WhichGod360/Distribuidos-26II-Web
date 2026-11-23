<%@ Page Title="Mi biblioteca · Lecturae" Language="VB" AutoEventWireup="false" MasterPageFile="~/pagWeb.Master" CodeBehind="wfMiBiblioteca.aspx.vb" Inherits="appWEB_Distribuidos.MiBiblioteca" %>
<asp:Content ID="Cabecera" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Contenido" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h1>Mi biblioteca</h1><p class="page-description">Descubre libros y consulta tus reservas, préstamos y avisos.</p><asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" />
<p class="summary-strip"><asp:Literal ID="litEstado" runat="server" Mode="Encode" /></p>
<nav class="tabs" aria-label="Secciones de mi biblioteca"><asp:HyperLink ID="lnkCatalogo" runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=catalogo" Text="Catálogo" /><asp:HyperLink ID="lnkCarrito" runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=carrito" Text="Carrito" /><asp:HyperLink ID="lnkReservas" runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=reservas" Text="Mis reservas" /><asp:HyperLink ID="lnkPrestamos" runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=prestamos" Text="Mis préstamos" /><asp:HyperLink ID="lnkCargos" runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=cargos" Text="Mis penalizaciones" /><asp:HyperLink ID="lnkAvisos" runat="server" NavigateUrl="~/Lector/wfMiBiblioteca.aspx?vista=avisos" Text="Avisos" /></nav>
<asp:Panel ID="pnlCatalogo" runat="server"><h2>Catálogo</h2><table class="toolbar-table" role="presentation"><tr><td><asp:Label ID="lblBuscar" runat="server" AssociatedControlID="txtBuscar" Text="Título, autor o editorial" /><asp:TextBox ID="txtBuscar" runat="server" CssClass="input-field" MaxLength="150" /></td><td><asp:Label ID="lblCategoria" runat="server" AssociatedControlID="ddlCategoria" Text="Categoría" /><asp:DropDownList ID="ddlCategoria" runat="server" CssClass="input-field" /></td><td><asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="button primary" /></td><td><asp:Button ID="btnActualizar" runat="server" Text="Actualizar" CssClass="button" /></td></tr></table><asp:ListView ID="lvCatalogo" runat="server" DataKeyNames="IdLibro">
    <LayoutTemplate>
        <div class="catalog-grid">
            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <article class="book-card">
            <div class="book-card-cover" aria-hidden="true">
                <span class="book-card-icon"></span>
                <span class="book-card-cover-caption">Imagen referencial</span>
            </div>
            <h3 class="book-card-title"><%#: Eval("TituloLibro") %></h3>
            <p class="book-card-author"><%#: Eval("Autor") %></p>
            <p class="book-card-category"><%#: Eval("Categoria") %></p>
            <p class="book-card-edition"><%#: Eval("EditorialLibro") %><span class="book-card-year"><%#: Eval("AnoPublicacion") %></span></p>
            <div class="book-card-actions">
                <p class="book-card-availability"><%#: Eval("Disponibles") %> disponible(s)</p>
                <asp:LinkButton ID="btnAgregarLibro" runat="server" CommandName="Select"
                    Text='<%# TextoAgregar(Container.DataItem) %>'
                    Enabled='<%# PuedeAgregarLibro(Container.DataItem) %>'
                    CssClass="button primary book-card-button" />
            </div>
        </article>
    </ItemTemplate>
    <EmptyDataTemplate>
        <p class="catalog-empty" role="status">No se encontraron libros. Prueba con otro título, autor o categoría.</p>
    </EmptyDataTemplate>
</asp:ListView>
<asp:DataPager ID="pgrCatalogo" runat="server" PagedControlID="lvCatalogo" PageSize="12" class="catalog-pager">
    <Fields>
        <asp:NextPreviousPagerField ShowFirstPageButton="false" ShowLastPageButton="false" ShowPreviousPageButton="true" ShowNextPageButton="false" PreviousPageText="Anterior" ButtonCssClass="catalog-page-link" />
        <asp:NumericPagerField ButtonCount="5" NumericButtonCssClass="catalog-page-link" CurrentPageLabelCssClass="catalog-current-page" />
        <asp:NextPreviousPagerField ShowFirstPageButton="false" ShowLastPageButton="false" ShowPreviousPageButton="false" ShowNextPageButton="true" NextPageText="Siguiente" ButtonCssClass="catalog-page-link" />
    </Fields>
</asp:DataPager></asp:Panel>
<asp:Panel ID="pnlCarrito" runat="server" Visible="false"><h2>Carrito de solicitud</h2><p class="field-help">Un ejemplar por título. Hasta cinco libros pendientes de devolución en total. El recojo requiere aprobación y vence cinco días después de solicitar.</p><div class="table-scroll"><asp:GridView ID="grvCarrito" runat="server" AutoGenerateColumns="false" DataKeyNames="IdLibro" CssClass="data-table" GridLines="None" EmptyDataText="Tu carrito está vacío." ShowHeaderWhenEmpty="true"><Columns><asp:ButtonField CommandName="Select" Text="Quitar" /><asp:BoundField DataField="TituloLibro" HeaderText="Libro" /><asp:BoundField DataField="Autor" HeaderText="Autores" /><asp:BoundField DataField="Disponibles" HeaderText="Disponibles ahora" /></Columns></asp:GridView></div><div class="form-actions"><asp:Button ID="btnSolicitar" runat="server" Text="Solicitar reserva" CssClass="button primary" /><asp:Button ID="btnVaciar" runat="server" Text="Vaciar carrito" CssClass="button" /></div></asp:Panel>
<asp:Panel ID="pnlReservas" runat="server" Visible="false"><h2>Mis reservas</h2><p class="field-help">Selecciona una reserva vigente para cancelarla.</p><asp:Panel ID="pnlCancelar" runat="server" Visible="false" CssClass="confirm-panel"><p><asp:Literal ID="litCancelar" runat="server" Mode="Encode" /></p><asp:Button ID="btnConfirmarCancelar" runat="server" Text="Confirmar cancelación" CssClass="button danger" /><asp:Button ID="btnNoCancelar" runat="server" Text="Conservar reserva" CssClass="button" /></asp:Panel><div class="table-scroll"><asp:GridView ID="grvReservas" runat="server" AutoGenerateColumns="false" DataKeyNames="IdSolicitud" CssClass="data-table" GridLines="None" EmptyDataText="Todavía no tienes reservas." ShowHeaderWhenEmpty="true"><Columns><asp:TemplateField><ItemTemplate><asp:LinkButton runat="server" CommandName="Select" Text="Cancelar reserva" Visible='<%# PuedeCancelarReserva(Container.DataItem) %>' /></ItemTemplate></asp:TemplateField><asp:BoundField DataField="IdSolicitud" HeaderText="N.º" /><asp:BoundField DataField="Libros" HeaderText="Libros" /><asp:BoundField DataField="LimiteLocal" HeaderText="Recojo hasta" DataFormatString="{0:dd/MM/yyyy HH:mm}" /><asp:BoundField DataField="EstadoActual" HeaderText="Estado" /></Columns></asp:GridView></div></asp:Panel>
<asp:Panel ID="pnlPrestamos" runat="server" Visible="false"><h2>Mis préstamos</h2><div class="table-scroll"><asp:GridView ID="grvPrestamos" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None" EmptyDataText="Todavía no tienes préstamos." ShowHeaderWhenEmpty="true"><Columns><asp:BoundField DataField="IdPrestamo" HeaderText="N.º" /><asp:BoundField DataField="TituloLibro" HeaderText="Libros" /><asp:BoundField DataField="Ejemplares" HeaderText="Ejemplares" /><asp:BoundField DataField="FechaInicio" HeaderText="Inicio" DataFormatString="{0:dd/MM/yyyy}" /><asp:BoundField DataField="FechaFin" HeaderText="Límite" DataFormatString="{0:dd/MM/yyyy}" /><asp:BoundField DataField="FechaRetorno" HeaderText="Devolución" DataFormatString="{0:dd/MM/yyyy}" /><asp:BoundField DataField="Estado" HeaderText="Estado" /></Columns></asp:GridView></div></asp:Panel>
<asp:Panel ID="pnlCargos" runat="server" Visible="false"><h2>Mis penalizaciones</h2><p class="field-help">Consulta cada cargo para ver las opciones de pago.</p><p class="summary-strip"><asp:Literal ID="litCargos" runat="server" Mode="Encode" /></p><table class="toolbar-table" role="presentation"><tr><td><asp:Label ID="lblPago" runat="server" AssociatedControlID="ddlPago" Text="Estado de pago" /><asp:DropDownList ID="ddlPago" runat="server" CssClass="input-field"><asp:ListItem Value="">Todos</asp:ListItem><asp:ListItem>Pendiente</asp:ListItem><asp:ListItem Value="PorRevisar">Por revisar</asp:ListItem><asp:ListItem>Pagada</asp:ListItem></asp:DropDownList></td><td><asp:Button ID="btnFiltrarCargos" runat="server" Text="Consultar" CssClass="button" /></td></tr></table><div class="table-scroll"><asp:GridView ID="grvCargos" runat="server" AutoGenerateColumns="false" CssClass="data-table" GridLines="None" EmptyDataText="No tienes penalizaciones registradas." ShowHeaderWhenEmpty="true"><Columns><asp:BoundField DataField="IdPrestamo" HeaderText="Préstamo" /><asp:BoundField DataField="TituloLibro" HeaderText="Libro" /><asp:BoundField DataField="Motivo" HeaderText="Motivo" /><asp:BoundField DataField="MontoPenalizacion" HeaderText="Importe" DataFormatString="S/ {0:N2}" /><asp:BoundField DataField="EstadoPagoTexto" HeaderText="Pago" /><asp:TemplateField HeaderText="Detalle"><ItemTemplate><asp:HyperLink runat="server" NavigateUrl='<%# RutaPago(Eval("IdDetallePenalizacion")) %>' Visible='<%# PuedeConsultarPago(Container.DataItem) %>' Text="Ver pago" /></ItemTemplate></asp:TemplateField></Columns></asp:GridView></div></asp:Panel>
<asp:Panel ID="pnlAvisos" runat="server" Visible="false"><h2>Avisos</h2><asp:Literal ID="litSinAvisos" runat="server" Mode="Encode" /><asp:Repeater ID="rptAvisos" runat="server"><ItemTemplate><article class="notice-card"><h3><%#: Eval("Asunto") %></h3><p><%#: Eval("Detalle") %></p></article></ItemTemplate></asp:Repeater></asp:Panel>
</asp:Content>
