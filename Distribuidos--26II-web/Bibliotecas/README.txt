Las cuatro DLL de las capas de escritorio se compilan desde backend para .NET Framework 4.7.2.
Los eventos de las páginas Web Forms llaman directamente a capaNegocio. El proyecto web referencia las DLL de esta carpeta y MSBuild las copia a bin.

Desde la carpeta de la web, ejecutar .\Build-Backend.ps1 para actualizar capaDatos,
capaNegocio, capaDAO, capaConexion y sus sumas SHA-256. Después, compilar Distribuidos2026-II-Web.slnx e iniciar la web en IIS Express.

Dependencias de las capas compartidas: Microsoft.AspNetCore.Cryptography.Internal 2.3.12,
Microsoft.AspNetCore.Cryptography.KeyDerivation 2.3.13 y System.Data.SqlClient 4.9.1.
La web usa QRCoder 1.9.0 (MIT) para generar la imagen del QR de demostración.
La carpeta LICENCIAS contiene sus metadatos de licencia.
