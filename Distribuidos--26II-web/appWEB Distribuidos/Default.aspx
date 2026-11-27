<%@ Page Async="true" Language="VB" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="appWEB_Distribuidos.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Iniciar sesión · Lecturae</title>
    <link rel="stylesheet" href="Estilos/biblioteca.css?v=login-fit-20261126" />
</head>
<body class="auth-body login-page" data-aplicacion="lecturae-web-d">
    <form id="form1" runat="server">
        <table class="auth-layout" role="presentation">
            <tr>
                <td class="auth-brand auth-scene-login">
                    <img class="auth-scene-image" src="Recursos/login-escritorio.jpg" alt="Personas disfrutando de la lectura en una biblioteca" /><h1>LECTURAE</h1>
                    <p>Tu biblioteca, en un solo lugar.</p>
                </td>
                <td class="auth-form">
                    <img class="auth-form-logo" src="Recursos/lecturae.png" alt="Lecturae" /><h2>Iniciar sesión</h2>
                    <p>Ingresa a tu cuenta de biblioteca.</p>
                    <asp:Label ID="lblMensaje" runat="server" Visible="false" role="alert" /><table class="editor-table" role="presentation">
                        <tr>
                            <td>
                                <asp:Label ID="lblCorreo" runat="server" Text="Correo electrónico" AssociatedControlID="txtCorreo" /><asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" MaxLength="100" CssClass="input-field" autocomplete="username" /></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblClave" runat="server" Text="Contraseña" AssociatedControlID="txtClave" /><asp:TextBox ID="txtClave" runat="server" TextMode="Password" MaxLength="128" CssClass="input-field" autocomplete="current-password" /></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:PlaceHolder ID="captchaContenedor" runat="server" Visible="false">
                                    <div class="g-recaptcha" data-sitekey="<%: ClaveRecaptcha %>" data-size="normal"></div>
                                    <noscript>
                                        <p class="field-help">Activa JavaScript para completar la verificación de acceso.</p>
                                    </noscript>
                                    <script src="https://www.google.com/recaptcha/api.js?hl=es" async defer></script>
                                </asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="btnIngresar" runat="server" Text="Ingresar" CssClass="button primary auth-submit" /></td>
                        </tr>
                    </table>
                    <asp:HyperLink ID="lnkRecuperar" runat="server" NavigateUrl="~/Seguridad/wfRecuperar.aspx" Text="¿Olvidaste tu contraseña?" /><p class="field-help"><a href="Seguridad/wfRegistro.aspx">Crear cuenta de lector</a></p>
                </td>
            </tr>
            <tr>
                <td colspan="2" class="auth-footer">Lecturae · Sistema de biblioteca</td>
            </tr>
        </table>
    </form>
</body>
</html>

