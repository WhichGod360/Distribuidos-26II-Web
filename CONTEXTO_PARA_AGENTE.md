# Contexto de la migración de Biblioteca Lecturae

Última actualización: 8 de noviembre de 2026. Este documento resume el trabajo realizado y las decisiones del usuario para continuar en otra sesión. Describe el estado observado en esa fecha; comprobar el estado actual antes de hacer cambios.

## Objetivo y acuerdos con el usuario

- Migrar las interfaces de Windows Forms a ASP.NET Web Forms (`.aspx`) con HTML, CSS propio y JavaScript, reutilizando las DLL del sistema.
- Mantener la identidad visual de Lecturae y el diseño general del panel principal. No incorporar frameworks de CSS.
- Los reportes y comprobantes deben mostrarse en pantalla. El usuario pidió quitar impresión y descarga; no añadir esas funciones sin una nueva solicitud.
- El cambio de contraseña se ofrece desde la recuperación del inicio de sesión. El usuario pidió retirar su acceso de la barra lateral.
- El catálogo del estudiante debe verse como tarjetas, tomando como referencia el escritorio.
- Conservar la imagen del escritorio en el login. El usuario probó la imagen en el registro de lectores y pidió retirarla; ese registro debe mantener el diseño sencillo anterior.
- En la limpieza final, el usuario autorizó eliminar `tests` y `docs`, conservar `scripts` y `tools`, y detener la web para limpiar temporales.
- El usuario NO autorizó guardar las credenciales SQL en un archivo privado. Sí autorizó una conexión temporal en memoria, que ya terminó al detener IIS Express.

## Ubicación de los proyectos

Raíz de este espacio de trabajo:

```text
C:\Users\danix\OneDrive\Desktop\Distribuidos-26II-Web
```

Proyecto de escritorio usado como referencia, fuera de esta raíz:

```text
C:\Users\danix\OneDrive\Desktop\PARA LA U\Distribuidos 2026 -II\Distribuidos-2026II
```

El proyecto de escritorio no se modificó durante esta migración. No confundirlo con las antiguas carpetas de capas vacías del proyecto web.

Dentro de la raíz web:

| Ruta relativa a la raíz | Uso |
| --- | --- |
| `Distribuidos-26II-Web-main/Distribuidos2026-II-Web/Distribuidos2026-II-Web.slnx` | Solución que se abre en Visual Studio |
| `Distribuidos-26II-Web-main/Distribuidos2026-II-Web/appWEB Distribuidos/` | Aplicación web; en adelante, `APP` |
| `Distribuidos-26II-Web-main/Distribuidos2026-II-Web/Bibliotecas/` | DLL reales y sus dependencias |
| `Distribuidos-26II-Web-main/Distribuidos2026-II-Web/packages/` | Dependencias NuGet del proyecto |
| `Distribuidos-26II-Web-main/scripts/Build-Backend.ps1` | Herramienta opcional para regenerar DLL |
| `Distribuidos-26II-Web-main/tools/BackendWeb/BackendWeb.csproj` | Proyecto auxiliar para esa regeneración |
| `Distribuidos-26II-Web-main/README.md` | Guía funcional y de ejecución actualizada |
| `Abrir-Lecturae.cmd` y `Abrir-Lecturae.ps1` | Iniciadores locales de Visual Studio |

## Arquitectura y archivos principales

La web usa Visual Basic y .NET Framework 4.7.2. Las páginas llaman a `capaNegocio`; las reglas y el acceso SQL se resuelven mediante las DLL. Las páginas no contienen consultas SQL.

Las DLL actuales son compatibles con la web y las capas adaptadas usan .NET Standard 2.0. La copia del escritorio recibida tiene destino .NET 10: no reemplazar las DLL web directamente por una compilación de esa copia. La compilación habitual de la web usa las DLL incluidas y no requiere compilar el escritorio.

Archivos dentro de `APP`:

- `pagWeb.Master` y sus archivos VB: estructura compartida, navegación y sesión.
- `Estilos/biblioteca.css`: estilos propios compartidos y adaptación de las vistas.
- `Scripts/biblioteca.js`: ayuda visual de requisitos de contraseña y actualización automática opcional. La actualización espera que la pestaña esté visible y evita interrumpir la edición de campos.
- `Comun/`: utilidades compartidas de las páginas y acceso por rol.
- `Default.aspx`: acceso; `wfInicio.aspx`: panel principal.
- `Seguridad/wfRegistro.aspx`: alta de lector; `Seguridad/wfRecuperar.aspx`: recuperación y cambio de contraseña.
- `Lector/wfMiBiblioteca.aspx`: catálogo en tarjetas, carrito, reservas, préstamos, penalizaciones y avisos del lector.
- `Catalogo/`: libros, autores, categorías y ejemplares.
- `Circulacion/`: lectores, préstamos, solicitudes y comprobantes.
- `Administracion/`: usuarios, bitácora, tipos de penalización y detalle de cargos.
- `Recursos/login-escritorio.jpg`: imagen del escritorio que se conserva en el login.

El catálogo del lector usa un `ListView`, tarjetas con un libro ilustrado mediante CSS, filtros por texto/categoría y paginación de 12 títulos. La acción de agregar al carrito conserva el flujo del servidor y las reglas del backend. No requiere una librería JavaScript para construir las tarjetas.

La sesión web usa identidad por `HttpContext.Session` y Forms Authentication. Las páginas verifican permisos en servidor: administrador para usuarios/bitácora, personal para circulación y mantenimientos, lector para Mi biblioteca. Las consultas personales toman la identidad desde la sesión y el carrito pertenece a esa sesión. El ViewState está firmado y vinculado a la sesión.

En circulación se conserva el cálculo de devolución en servidor y se exige recalcular si cambia la fecha; las entregas permiten elegir ejemplares por título. La edición de cargos conserva sus relaciones e importe original. La capa de negocio mantiene las transacciones y las reglas de disponibilidad.

## Base de datos y estado de la conexión

Se comprobó que el escritorio usa una base SQL en Somee y que la web puede acceder a esa misma base mediante las DLL compatibles. La conexión original está en la capa de conexión del escritorio. No copiar su contenido a este documento ni al repositorio.

El backend admite `BIBLIOTECA_CONNECTION_STRING`, `BIBLIOTECA_CONNECTION_FILE` y el archivo privado de Biblioteca en `%LOCALAPPDATA%`. Son mecanismos admitidos, no evidencia de que exista una configuración guardada.

Estado comprobado al redactar este archivo:

- IIS Express está detenido; no hay un proceso `iisexpress` activo.
- La conexión que se utilizó en la sesión era temporal, en el entorno del proceso, y dejó de estar disponible al detenerlo.
- No existe `%LOCALAPPDATA%\Biblioteca\connection-string.txt` en este equipo.
- El usuario rechazó guardar ese archivo. No crearlo ni persistir secretos sin nueva autorización.
- No registrar en documentos, comandos visibles o registros las credenciales SQL ni las contraseñas de prueba proporcionadas en el chat.

Para ejecutar otra vez será necesario proporcionar la conexión al nuevo proceso de la web. Si se abre sin ella, puede aparecer: «Configura la conexión privada de Biblioteca antes de realizar operaciones de datos». Ese mensaje indica que falta la configuración de conexión, no que las DLL sean de demostración.

`Abrir-Lecturae.ps1` actualmente exige el archivo privado antes mencionado, prepara temporales en `.local` y abre Visual Studio. Por eso no es un iniciador utilizable con el estado actual de conexión temporal ya terminada. Se conserva como herramienta, sin haber guardado el archivo que exige.

## Ejecución y validación

Abrir la solución con Visual Studio y las herramientas ASP.NET / targeting pack de .NET Framework 4.7.2. El proyecto usa IIS Express por HTTP, con URL local `http://localhost:62578/Default.aspx` y sin puerto SSL configurado.

La última comprobación tras la limpieza fue una compilación Debug correcta mediante MSBuild de Visual Studio 18 Community. También se verificó que todos los archivos y DLL referenciados por el proyecto siguieran presentes y que `git diff --check` no reportara problemas.

En fases anteriores se validaron compilaciones Debug/Release y se ejecutaron pruebas HTTP de los flujos migrados. Esas pruebas son antecedentes de la sesión: sus archivos fueron eliminados después por petición expresa del usuario y no constituyen una suite disponible actualmente.

Las operaciones de un usuario autenticado actúan sobre la base real configurada. Al comprobar una interfaz, distinguir las consultas de las acciones que modifican datos.

## Limpieza realizada y pendiente

Se eliminaron los archivos de `tests` y `docs`, metadatos de cuatro proyectos locales vacíos (`capaConexion`, `capaDAO`, `capaDatos`, `capaNegocio`), marcadores `.gitkeep` innecesarios y preferencias locales del proyecto (`.vbproj.user`). Algunas de esas carpetas permanecen vacías en disco.

Se conservaron los archivos funcionales, las DLL de `Bibliotecas`, las dependencias de `packages`, el contenido de ejecución de `APP/bin`, recursos, estilos, scripts, la solución y las herramientas de mantenimiento. También se conservó `.git`. No eliminar las DLL de ejecución tratándolas como temporales.

Quedaron estas carpetas de caché: el entorno rechazó el borrado de carpetas por su política y se informó al usuario de que podía eliminarlas manualmente con Visual Studio cerrado. La eliminación NO quedó completada ni se confirmó después:

| Ruta relativa a la raíz | Tamaño aproximado observado |
| --- | --- |
| `.local/` | 193,71 MB; copias de pruebas, precompilados, diagnósticos y temporales |
| `Distribuidos-26II-Web-main/Distribuidos2026-II-Web/.vs/` | 0,40 MB; caché de Visual Studio |
| `Distribuidos-26II-Web-main/Distribuidos2026-II-Web/appWEB Distribuidos/obj/` | 0,81 MB; caché de compilación |

Comprobar si el usuario ya las eliminó antes de dar la limpieza por terminada. No confundir estas carpetas con `bin`, `Bibliotecas` o `packages`. Los lanzadores y las compilaciones pueden volver a generar temporales.

El README se actualizó para quitar enlaces a las pruebas y documentos retirados. Este archivo de contexto en la raíz se añade después de esa limpieza por una nueva solicitud explícita del usuario.

## Continuidad del trabajo

El usuario dio por buenas las interfaces y pidió este documento como cierre; no hay una nueva funcionalidad solicitada. Si se retoma el trabajo, respetar los acuerdos de apariencia, comprobantes y conexión. Revisar el estado de Git antes de editar: hay cambios de la migración y eliminaciones previas del usuario; no restaurarlos ni descartarlos automáticamente.

No ejecutar `Build-Backend.ps1` como parte de una compilación normal. Ese script sobrescribe DLL de `Bibliotecas` y necesita un `BackendRoot` compatible; el valor predeterminado puede no corresponder al escritorio de este equipo. Mantener `scripts` y `tools` fue una decisión explícita del usuario.
