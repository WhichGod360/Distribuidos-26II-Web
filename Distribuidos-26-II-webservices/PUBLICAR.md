# Publicar los servicios de biblioteca

La carpeta `Publicacion` contiene la aplicación ASMX compilada en Release para .NET Framework 4.7.2. El archivo `Biblioteca-ASMX.zip` reúne ese mismo contenido y se puede descomprimir en la raíz de una aplicación ASP.NET de IIS.

1. Para probar el código en Visual Studio, el servicio lee `WebServices_Biblioteca/webServices/App_Data/connection-string.txt`. Si la web ya usa la base que necesitas, copia allí su archivo privado `App_Data/connection-string.txt`. Abre `WebServices_Biblioteca/WebServices_Biblioteca.slnx` e inicia `swBiblioteca.asmx` con IIS Express.
2. Para publicar, carga el contenido de la carpeta en la aplicación IIS del servicio. En el servidor, configura `BIBLIOTECA_CONNECTION_STRING` para el proceso o crea el archivo privado `App_Data/connection-string.txt` con la cadena SQL en una sola línea. El paquete deja esa configuración a cargo del servidor.
3. Abre `https://TU-SITIO/swBiblioteca.asmx?WSDL` para consultar las operaciones. Si IIS aloja el servicio bajo una ruta, incluye esa ruta antes de `swBiblioteca.asmx`.
4. Para probar una operación con datos, llama a `AutenticarWeb` y conserva las cookies del mismo cliente al llamar a `ObtenerSesion`, al CRUD y a `CerrarSesion`. Cada usuario debe tener su propio contenedor de cookies. Para los avisos del lector, `ListarMisAvisos` ya incluye las decisiones de Yape cuando el módulo está disponible.

## Regenerar la carpeta

Si cambiaste Negocio o DAO en escritorio, ejecuta primero `Distribuidos--26II-web/Build-Backend.ps1` desde el espacio de trabajo. En Visual Studio, abre el proyecto `webServices`, elige **Publicar → Carpeta** y usa la configuración **Release**. El perfil escribe en `Distribuidos-26-II-webservices/Publicacion`. Si necesitas un ZIP actualizado, comprime el contenido de esa carpeta.
