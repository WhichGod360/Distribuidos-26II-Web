# Biblioteca web Lecturae

Migración de la interfaz Windows Forms a ASP.NET Web Forms con Visual Basic y .NET Framework 4.7.2. Usa HTML, controles ASP.NET, CSS propio y JavaScript sin frameworks. Conserva la identidad visual de Lecturae y la estructura con tablas del curso.

## Abrir y ejecutar

1. Abrir Distribuidos2026-II-Web/Distribuidos2026-II-Web.slnx en Visual Studio, con las herramientas de desarrollo ASP.NET y el targeting pack .NET Framework 4.7.2.
2. Preparar la conexión SQL para el proceso de la aplicación. El iniciador Abrir-Lecturae.cmd puede usarse cuando ya existe el archivo privado de conexión indicado más abajo.
3. Restaurar packages.config, compilar y ejecutar con IIS Express.
4. Inicio de sesión: http://localhost:62578/Default.aspx. El lector entra en Mi biblioteca; administrador y bibliotecario, en el panel de inicio.

El inicio local de IIS Express está configurado por HTTP, sin puerto SSL. Si Visual Studio ya estaba abierto al actualizar esta configuración, recargar el proyecto o cerrar y abrir la solución antes de ejecutarlo de nuevo.

La compilación normal usa las DLL reales incluidas en Distribuidos2026-II-Web/Bibliotecas. No requiere compilar el escritorio ni usar .NET 10. El SDK .NET 10 solo corresponde a las herramientas opcionales para actualizar las DLL.

## Funciones

| Área | Página | Comportamiento |
| --- | --- | --- |
| Acceso | Default.aspx | Inicio y cierre de sesión; destino según rol |
| Registro | Seguridad/wfRegistro.aspx | Nueva cuenta de lector y validación de contraseña |
| Recuperación | Seguridad/wfRecuperar.aspx | Pregunta, respuesta y cambio de contraseña autorizado |
| Inicio | wfInicio.aspx | Estadísticas del backend, filtros, préstamos por mes y categoría, devoluciones y accesos por rol |
| Lectores | Circulacion/wfLectores.aspx | Búsqueda de estudiantes activos e historial |
| Préstamos | Circulacion/wfPrestamos.aspx | Listado, filtros, registro presencial, cálculo y confirmación de devolución |
| Solicitudes | Circulacion/wfSolicitudes.aspx | Aprobación, rechazo, selección de ejemplares por título y entrega |
| Comprobantes | Circulacion/wfComprobante.aspx | Consulta en pantalla de entregas y devoluciones |
| Usuarios | Administracion/wfUsuarios.aspx | Alta, edición, filtros por rol/estado y activación/desactivación |
| Bitácora | Administracion/wfBitacora.aspx | Filtros, detalle codificado y actualización automática opcional |
| Cargos | Administracion/wfDetallePenalizaciones.aspx | Alta y edición de motivo/pago, conservando importe y relaciones del cargo |
| Mi biblioteca | Lector/wfMiBiblioteca.aspx | Catálogo en tarjetas, carrito, reservas, préstamos propios, penalizaciones y avisos |
| Catálogo | Catalogo/wfLibros.aspx | Libros, relaciones con autores/categorías y ejemplares iniciales |
| Mantenimientos | Catalogo/wfAutores.aspx, wfCategorias.aspx, wfEjemplares.aspx | Autores, categorías y copias físicas |
| Penalizaciones | Administracion/wfPenalizaciones.aspx | Tipos, importes y tramos |

Los pagos se regularizan en ventanilla y la interfaz registra su estado, como el escritorio. Los comprobantes se muestran en pantalla dentro del diseño general de la aplicación, sin controles de impresión ni descarga.

## Backend, sesión y permisos

Las páginas llaman a capaNegocio; no incluyen SQL. Se reutilizan las DLL .NET Standard 2.0 adaptadas a la web, con un almacén de identidad por HttpContext.Session y ticket protegido de Forms Authentication. El código fuente del escritorio sirve de referencia y no se modificó durante esta migración.

Cada página verifica su acceso en servidor: usuarios y bitácora requieren administrador; circulación, cargos y mantenimientos requieren personal; Mi biblioteca requiere lector. Las consultas personales obtienen el lector desde la sesión. El carrito vive en la sesión del visitante. Los formularios mantienen ViewState firmado y vinculado a la sesión.

La devolución conserva en servidor el cálculo revisado; cambiar la fecha exige calcular otra vez. La entrega admite un selector de ejemplar por título. La edición de cargos conserva los identificadores y el importe del registro original. La capa de negocio y sus transacciones siguen resolviendo disponibilidad, concurrencia y reglas de circulación.

## Conexión privada

El backend lee BIBLIOTECA_CONNECTION_STRING, BIBLIOTECA_CONNECTION_FILE o el archivo privado de conexión de Biblioteca en LOCALAPPDATA. La configuración privada se prepara por separado en cada equipo. No se guardan credenciales en las páginas ni en Web.config.

La aplicación de entrega usa las DLL reales. Los cambios de un usuario autenticado se realizarán en la base configurada para esa aplicación.

Si se usa una conexión temporal mediante variables del proceso, debe proporcionarse de nuevo al reiniciar IIS Express o Visual Studio. El proyecto no contiene credenciales SQL guardadas.

## Actualizar el backend

Las DLL actuales ya incluyen los contratos usados por estas páginas. Para regenerarlas en otro momento, usar scripts/Build-Backend.ps1 con un backend compatible y adaptado a .NET Standard 2.0. La copia fuente del escritorio recibida en esta sesión tiene destino .NET 10: sus DLL compiladas directamente no sustituyen a las DLL compatibles de la web.
