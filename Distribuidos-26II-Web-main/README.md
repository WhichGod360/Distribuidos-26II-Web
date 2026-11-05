# Biblioteca web Lecturae

ASP.NET Web Forms, Visual Basic y .NET Framework 4.7.2. Interfaz con tablas HTML y controles ASP.NET, siguiendo el ejemplo de la profesora y el estilo del escritorio LECTURAE. CSS propio, sin Bootstrap, Tailwind ni frameworks JavaScript.

## Iniciar

Abrir `Distribuidos2026-II-Web/Distribuidos2026-II-Web.slnx` en Visual Studio con desarrollo ASP.NET/Web y targeting pack 4.7.2. Restaurar packages.config, compilar y ejecutar IIS Express.

**Default.aspx es el inicio de sesión**, configurado como página inicial. El proyecto usa HTTP local `http://localhost:62578/`, sin pedir certificado de desarrollo. En la verificación desde consola se usa `http://localhost:62578/Default.aspx`. Una sesión válida lleva a `wfInicio.aspx`, que utiliza pagWeb.Master.

Las DLL reales están incluidas en `Distribuidos2026-II-Web/Bibliotecas`; la compilación normal no depende de la carpeta del escritorio ni del SDK .NET 10. No reemplazarlas por las antiguas DLL net10.0.

## Funciones implementadas

| Página | Funciones |
| --- | --- |
| Default.aspx | Inicio de sesión por correo y contraseña con usuarioNegocio.IniciarSesion |
| Seguridad/wfRecuperar.aspx | Pregunta de seguridad, verificación de respuesta y cambio de contraseña con token de cinco minutos |
| Catalogo/wfLibros.aspx | Buscar, agregar, editar y desactivar libros; selección de varios autores/categorías; ejemplares iniciales y enlace a gestión de copias |
| Catalogo/wfAutores.aspx | Buscar, agregar, editar y eliminar autores respetando restricciones de asociación |
| Catalogo/wfCategorias.aspx | Buscar, agregar, editar, desactivar y reactivar categorías |
| Catalogo/wfEjemplares.aspx | Seleccionar libro, listar copias, agregar y activar/desactivar ejemplares |
| Administracion/wfPenalizaciones.aspx | Buscar, agregar, editar, desactivar y reactivar tipos de penalización, importes y tramos |

Las bajas conservan las reglas del escritorio: categorías, penalizaciones, libros y ejemplares se desactivan mediante los métodos existentes. Autores no se pueden eliminar si están asociados a libros. En ejemplares, la edición es cambio de estado, sin cambiar de libro ni inventar una eliminación física. Se confirman las bajas en servidor antes de ejecutarlas.

El resto de módulos permanece pendiente: lectores, préstamos, solicitudes, gestión de usuarios, bitácora y estadísticas del inicio. No se implementó registro de nuevas cuentas, pagos ni detalles de penalización de préstamos. El menú solo habilita los cinco mantenimientos solicitados; los lectores no tienen acceso a ellos.

## Organización y estilo del curso

La maestra conserva cabecera, menú y pie en tablas HTML, asp:ContentPlaceHolder y eventos .Master.vb para navegar. Cada mantenimiento tiene .aspx, .aspx.vb y .designer.vb con controles Button, TextBox, DropDownList, CheckBox, CheckBoxList y GridView. Las páginas llaman a capaNegocio; no contienen SQL. Comun/Paginas.vb reúne validación de acceso, mensajes, formato y protección de formularios sin duplicar las reglas de negocio.

La identidad se mantiene por visitante en HttpContext.Session y se restaura desde un ticket protegido de Forms Authentication. El login abandona la sesión anónima; la cookie no es persistente y dura 30 minutos. Cerrar sesión elimina ticket y estado. La maestra muestra nombres codificados, los listados codifican sus campos y los formularios usan ViewState firmado vinculado a la sesión. Los mantenimientos verifican personal activo en servidor mediante seguridadNegocio. Hay límite de intentos en login y recuperación.

Cambiar contraseña desde el menú cierra la sesión y abre el flujo de verificación por pregunta de seguridad, igual que la recuperación del escritorio. La autorización permanece en Session y no se coloca en URL ni campos HTML.

## Conexión privada

El backend lee BIBLIOTECA_CONNECTION_STRING, BIBLIOTECA_CONNECTION_FILE o `%LOCALAPPDATA%\Biblioteca\connection-string.txt`. En este equipo existe la configuración privada fuera del repositorio. Para otro equipo debe configurarse por separado. No hay credenciales en Web.config ni en las páginas. Los cambios reales realizados por un usuario autenticado afectarán la misma biblioteca que el escritorio.

## Verificación y límites

- Compilación Debug/Release y precompilación ASP.NET finales satisfactorias.
- Login y recuperación cargan por HTTP. Los cinco mantenimientos rechazan acceso anónimo.
- 35 comprobaciones HTTP reproducibles en copia aislada con backend EN MEMORIA: altas, ediciones, bajas confirmadas, libros con relaciones, ejemplares, permisos, logout, recuperación, cambio de clave, ViewState y límite de intentos. Véase tests/FlujosWeb/README.md.
- Nueve comprobaciones adicionales del adaptador real de sesión.
- Se abrió y cerró la conexión SQL privada real, sin ejecutar consultas. No se ejecutaron CRUD ni cambios de contraseña contra Somee durante la verificación: requieren una cuenta y datos de prueba autorizados.
- La prueba en memoria valida el flujo web y sus llamadas; no sustituye las pruebas de SQL ni concurrencia transaccional reales. Las reglas SQL siguen siendo las del backend original.
- Si los nombres agregados de autores/categorías de un libro admiten interpretaciones ambiguas, la edición se detiene para evitar perder relaciones.

El disco C: se quedó casi sin espacio durante las pruebas; los archivos temporales de verificación se prepararon en D:. El proyecto de entrega permanece en su ruta original de Downloads.

## Actualizar backend

Las cuatro capas originales se compilan para .NET Standard 2.0 y el escritorio mantiene su interfaz .NET 10. Para regenerar las DLL locales tras cambiar esas capas, ejecutar explícitamente scripts/Build-Backend.ps1 -BackendRoot 'RUTA\Distribuidos-2026II' -Configuration Release. No se necesita para compilar la entrega actual.

## Abrir desde D: en este equipo

Ejecutar `D:\Distribuidos-26II-Web-main\Abrir-Lecturae.cmd`. Abre una nueva instancia de Visual Studio con la solución correcta y dirige los temporales a `.local` en D:, porque C: está casi lleno. No cambia las variables globales de Windows. El iniciador también actualiza compilation/tempDirectory en Web.config para esta ubicación local; si se traslada a otra carpeta, volver a ejecutar el iniciador para actualizarla.

En el Explorador de soluciones, expandir appWEB Distribuidos y las carpetas Catalogo, Administracion y Seguridad para ver los formularios. Default.aspx es el login, wfInicio.aspx el panel. El ejemplo de la profesora tiene otro nombre de solución: solWEBSistemasDistribuidos_B.

La instancia antigua del puerto 51843 corresponde a la copia previa. Para la entrega de D: usar exclusivamente http://localhost:62578/Default.aspx. Se verificó la apertura y cierre de la conexión mediante la DLL real dentro de IIS Express, sin consultas SQL.

