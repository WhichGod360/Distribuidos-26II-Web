# Préstamo con varios ejemplares

La entrega directa y la entrega de una solicitud asignan una sola referencia a todos los ejemplares. El listado muestra una fila por operación; la devolución actualiza todas las copias en una transacción. Se genera un comprobante conjunto y, si corresponde, una sola mora fija.

La tabla `Prestamo` conserva una fila por ejemplar para mantener sus relaciones existentes. La nueva columna `id_operacion` agrupa esas filas en un préstamo lógico. Los registros anteriores, cuyo `id_operacion` queda vacío, siguen siendo préstamos individuales.

## Base existente

Antes de usar la versión nueva contra la base existente, ejecutar `agrupar_prestamos.sql` en esa base. La migración agrega una columna y un índice; no altera los préstamos anteriores. Para una base nueva, `bd_fdinal_biblioteca.sql` ya contiene esos cambios.

## Verificación local

La solución de escritorio y la web compilaron en este equipo. En una base LocalDB aislada se probaron entregas directas de dos y tres ejemplares, una solicitud de tres libros, sus devoluciones conjuntas, los comprobantes, la prevención de doble devolución y la reversión completa de una entrega con un ejemplar inexistente.

La página publicada en `lecturae.somee.com` todavía muestra la versión anterior. Para verla allí hay que ejecutar primero la migración en la base de Somee y después publicar los archivos web y bibliotecas actualizados.
