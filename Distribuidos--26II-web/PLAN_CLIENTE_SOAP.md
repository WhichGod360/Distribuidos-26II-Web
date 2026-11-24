# Cliente Web Forms conectado a Libreria

Los eventos de los botones ASPX usan `Libreria.swBibliotecaSoapClient`. El proyecto web compila con el proxy de `Connected Services/Libreria`. El ASMX ejecuta las validaciones, las transacciones SQL y la bitácora con las DLL del servidor.

## Conexión y sesión

El endpoint `swBibliotecaSoap` del `Web.config` del cliente apunta a `http://webservicesbiblioteca.somee.com/swBiblioteca.asmx`. El contrato publicado expone 87 operaciones. [Consultar el WSDL](http://webservicesbiblioteca.somee.com/swBiblioteca.asmx?WSDL)

`ServicioBiblioteca` abre un cliente por petición y guarda las cookies de Somee en la sesión ASP.NET del usuario. El inicio llama a `AutenticarWeb`, las páginas protegidas comprueban la identidad con `ObtenerSesion` y el botón Salir llama a `CerrarSesion`. El cliente muestra los mensajes de validación del servicio en los formularios.

## Funciones migradas

- **Seguridad y administración.** Inicio y cierre de sesión, registro, recuperación de cuenta, usuarios, aprobación de lectores y bitácora.
- **Catálogo y panel.** Autores, categorías, libros y sus relaciones, ejemplares, tipos de penalización, cargos e indicadores.
- **Circulación y pagos.** Catálogo del lector, carrito, solicitudes, préstamo directo, entrega conjunta, devolución, comprobantes, avisos y reportes Yape. El ASMX también emite y valida el token del pago de demostración.

## Comprobaciones realizadas

| Entorno | Resultado |
| --- | --- |
| Cliente en IIS Express conectado a Somee | `HelloWorld` respondió «Hola a todos». El formulario de acceso mostró el rechazo esperado para una cuenta ficticia. |
| Publicación Release | La web y los ASPX compilaron. La salida publicada excluye las cuatro DLL de las capas de escritorio y el archivo de conexión SQL. |
| Web Forms con ASMX y base local de prueba | Se verificaron dos sesiones con roles distintos, 13 páginas de personal, las seis vistas del lector, el CRUD de Autores desde sus botones y el registro en bitácora. Cerrar la sesión del administrador dejó activa la del lector. |
| Circulación en la base de prueba | Se aprobó una reserva de dos títulos. La entrega incompleta fue rechazada y la completa generó un comprobante con ambos ejemplares. La devolución generó un único cargo por mora. |
| Yape en la base de prueba | Se denegó un reporte ficticio, se confirmó el siguiente y se verificaron el historial y los avisos. Repetir la confirmación conservó el saldo. |
| Interrupción del ASMX de prueba | Al intentar guardar un autor, el formulario mostró el error de conexión y conservó el texto para reintentar. |

## Pendiente en el alojamiento

La conexión HTTP a Somee respondió. La prueba HTTPS falló durante la negociación TLS. Para habilitar el acceso con cuentas reales, verificar que el dominio sirva el ASMX con un certificado válido, cambiar la dirección del endpoint a HTTPS y activar `<security mode="Transport" />` en su `basicHttpBinding`. Después, verificar un inicio de sesión válido y el acceso a una página protegida contra el servicio publicado.
