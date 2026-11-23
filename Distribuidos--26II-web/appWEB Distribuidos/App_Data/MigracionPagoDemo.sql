-- Ejecutar únicamente en la base LocalDB de demostración.
SET XACT_ABORT ON;
IF CONVERT(int, SERVERPROPERTY('IsLocalDB')) <> 1 OR SERVERPROPERTY('IsLocalDB') IS NULL
   OR (DB_NAME() <> N'LecturaePruebas' AND DB_NAME() NOT LIKE N'LecturaePruebas[_]%')
    THROW 51010, N'Selecciona LecturaePruebas o una base LecturaePruebas_* en LocalDB.', 1;

IF OBJECT_ID(N'dbo.PagoDemoPenalizacion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PagoDemoPenalizacion (
        id_detalle_penalizacion int NOT NULL PRIMARY KEY REFERENCES dbo.DetallePenalizacion(id_detalle_penalizacion),
        id_lector int NOT NULL REFERENCES dbo.Usuario(id_usuario),
        monto decimal(7,2) NOT NULL CHECK (monto > 0),
        id_transaccion uniqueidentifier NOT NULL UNIQUE,
        fecha_utc datetime2(0) NOT NULL DEFAULT SYSUTCDATETIME()
    );
END;

EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_DetallePenalizacion_ProtegerDemo
ON dbo.DetallePenalizacion
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN dbo.PagoDemoPenalizacion demo ON demo.id_detalle_penalizacion=i.id_detalle_penalizacion
        WHERE i.estado_pago_detalle<>''Pagada'' OR i.monto_aplicado<>demo.monto
    )
        THROW 51006, N''El cargo tiene un pago de demostración confirmado.'', 1;
END;';
