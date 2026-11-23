Las cuatro DLL de las capas de escritorio se compilan desde backend para .NET Framework 4.7.2.
El servicio swBiblioteca.asmx las referencia desde esta carpeta. La aplicación web llama al servicio mediante Librerias.

Desde la carpeta de la web, ejecutar .\Build-Backend.ps1 para actualizar capaDatos,
capaNegocio, capaDAO, capaConexion y sus sumas SHA-256. Después, compilar la solución del servicio.

Dependencias del servicio: Microsoft.AspNetCore.Cryptography.Internal 2.3.12,
Microsoft.AspNetCore.Cryptography.KeyDerivation 2.3.13 y System.Data.SqlClient 4.9.1.
La web usa QRCoder 1.9.0 (MIT) para generar la imagen del QR de demostración.
La carpeta LICENCIAS contiene sus metadatos de licencia.
