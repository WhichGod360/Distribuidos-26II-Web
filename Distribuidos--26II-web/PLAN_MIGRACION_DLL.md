# Migración de Web Forms a las DLL de escritorio

**Estado actual.** La web ejecuta las cuatro DLL de .NET Framework 4.7.2 en el proceso de ASP.NET. Para iniciarla, compila las capas con `Build-Backend.ps1` y abre `Distribuidos2026-II-Web.slnx` en Visual Studio con IIS Express. Configura la base del proceso web mediante `BIBLIOTECA_CONNECTION_STRING` o el archivo privado `App_Data/connection-string.txt`. Las secciones siguientes conservan el plan original.

La web seguirá usando sus páginas ASPX. Al pulsar un botón, ASP.NET ejecutará su evento en el archivo `.aspx.vb`, que llamará directamente a `capaNegocio`. Las cuatro capas compartirán el código fuente del escritorio. Esta primera entrega es el plan de trabajo.

## Punto de partida

- La aplicación apunta a .NET Framework 4.7.2. Hoy, por ejemplo, `Catalogo/wfAutores.aspx.vb` ya recibe `btnBuscar.Click`, pero su listado llama a `Servicio.ListarAutores()`. La página usa el cliente SOAP y los tipos generados en `Librerias`. El servicio es quien invoca las DLL.
- `backend/` ya enlaza los archivos VB del escritorio. `Build-Backend.ps1` compila las cuatro capas en Release para `net472`, las copia a `Bibliotecas/` y actualiza sus huellas. Esta es la ruta compatible con la aplicación ASP.NET. Los binarios del escritorio compilados para .NET 10 tienen otro destino.
- En el ejemplo `solWEBSistemasDistribuidos_B_WebServices/appWEB/wfEmpleado.aspx.vb`, cada botón tiene un evento con nombre, llama a una operación y muestra el resultado en la página. Se adoptará esa estructura. La llamada del botón irá a la clase de Negocio. La integración de servicios web tendrá su propia etapa posterior.

## Cómo quedará un botón

Entiendo «vps» como la preferencia por concentrar las acciones en los propios botones ASPX y reducir el código auxiliar. El marcado declara el control y el archivo VB de la misma página contiene su evento. Los auxiliares cortos como `Listar()` se conservan cuando varios botones necesitan refrescar la misma grilla.

Ejemplo para el botón Guardar de un autor nuevo. Se usa la firma existente de `autorNegocio.agregar` y el auxiliar de fechas que ya utiliza la web. Los nombres de los controles corresponden a la página de autores.

```aspx
<asp:Button ID="btnGuardar" runat="server" Text="Guardar" />
```

```vb
Private ReadOnly autores As New capaNegocio.autorNegocio()

Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
    Try
        autores.agregar(txtNombre.Text, txtNacionalidad.Text,
                        FechaOpcional(txtNacimiento.Text), FechaOpcional(txtFallecimiento.Text),
                        txtBiografia.Text)
        lblMensaje.Text = "Autor registrado."
        Listar()
    Catch ex As Exception
        lblMensaje.Text = Server.HtmlEncode(ex.Message)
    End Try
End Sub
```

En esa página, `Listar()` consultará `autores.listarTodos()`, aplicará el filtro de la grilla y llamará a `DataBind()`. Editar usará `actualizar` con el ID seleccionado. Eliminar pedirá confirmación y llamará a `eliminar`. La carga inicial irá dentro de `If Not IsPostBack Then` para conservar los datos escritos durante cada postback. Negocio seguirá comprobando formatos, permisos y reglas. DAO conservará las consultas parametrizadas, las transacciones y la bitácora.

## Orden de migración

| Etapa | Trabajo | Comprobación antes de seguir |
| --- | --- | --- |
| 1. DLL y sesión | Ejecutar `Build-Backend.ps1`. Referenciar las DLL de `Bibliotecas/` desde el proyecto web y verificar sus dependencias en `bin/`. Configurar la conexión SQL del proceso web. Adaptar el almacenamiento por sesión HTTP tomando como referencia `AlmacenSesionServicio.vb` del servicio. Registrar el adaptador con `SesionUsuario.ConfigurarAlmacen` al iniciar la aplicación. | Abrir dos sesiones de navegador con cuentas diferentes. Cada operación debe conservar su usuario y rol, incluso después de cerrar la otra sesión. |
| 2. Acceso y piloto | Migrar acceso, registro, recuperación y cierre de sesión. Revisar `Comun/Paginas.vb` y los controles de acceso. Migrar Autores de principio a fin usando entidades de `capaDatos`. | Con el servicio SOAP detenido, iniciar sesión, listar, buscar, registrar, editar y eliminar un autor de prueba. Comprobar el mensaje de validación, los permisos y el actor en bitácora. |
| 3. Mantenimientos | Aplicar el patrón a Categorías, Libros, Ejemplares, Usuarios, Lectores pendientes y Penalizaciones. Revisar combos y relaciones que hoy reciben tipos SOAP. | Probar un cambio válido y otro inválido por módulo. Verificar selección, filtros y paginación después del postback. |
| 4. Circulación | Migrar Mi biblioteca, Lectores, Solicitudes, Préstamos y Comprobantes. Los botones llamarán a las operaciones completas de `solicitudPrestamoNegocio`, `prestamoNegocio` y `comprobanteNegocio`. | Reservar dos títulos y entregar ambos. Forzar una selección inválida en el segundo y verificar que la reserva siga aprobada sin préstamos parciales. Probar devolución con mora y comprobante del grupo. |
| 5. Cargos, pagos y panel | Migrar cargos, reporte y revisión de Yape, avisos, bitácora e indicadores. Usar `pagoYapeNegocio` y `dashboardNegocio` para los datos. | Con datos ficticios, denegar y confirmar reportes, repetir una confirmación y comprobar saldo e historial. Comparar los indicadores con escritorio usando los mismos filtros. |

Durante la transición, una página pendiente que aún use SOAP necesitará la sesión autenticada del servicio. Antes de cambiar el inicio de sesión habitual, probar el piloto de forma aislada o mantener operativo el acceso anterior hasta completar las páginas dependientes. Conservar el proyecto ASMX para la etapa futura.

## CSS y JavaScript

Mantener `Estilos/biblioteca.css` como hoja propia. Usar controles Web Forms para buscar, guardar, confirmar, listar y paginar. Los indicadores se calculan en la DLL y pueden mostrarse como texto o gráficos. Reservar JavaScript nativo para la presentación de un gráfico u otra interacción puntual que lo necesite. En la revisión de `Scripts/biblioteca.js` y `Scripts/pago-demo.js`, identificar por separado las ayudas visuales, el refresco automático y la demostración de pagos. Las reglas definitivas se ejecutan en el servidor. No incorporar frameworks CSS ni JavaScript.

## Cierre y verificación

1. Buscar en las páginas, la página maestra y `Comun/` las llamadas a `Servicio.`, `ClienteSoap` y los tipos de `Librerias`. Retirar de la aplicación web el proxy, la configuración del cliente y el cierre de canal en `Global.asax.vb` cuando termine su último consumidor.
2. Compilar la solución web con MSBuild para aplicaciones ASP.NET Framework y precompilar las páginas ASPX. Iniciar la web en IIS Express con el servicio detenido y una base SQL de prueba.
3. Recorrer los flujos como administrador, bibliotecario y lector. Probar también el acceso directo a una página de otro rol, la expiración de sesión y dos intentos de prestar el mismo ejemplar. Para dar la migración por terminada, los botones deben funcionar de principio a fin, respetar los permisos y dejar el mismo resultado en datos y bitácora que el escritorio.
