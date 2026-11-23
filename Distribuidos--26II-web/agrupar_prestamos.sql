-- Ejecutar una sola vez en una base existente, antes de iniciar la nueva versión.
IF COL_LENGTH('dbo.Prestamo', 'id_operacion') IS NULL
    ALTER TABLE dbo.Prestamo ADD id_operacion UNIQUEIDENTIFIER NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Prestamo') AND name = 'IX_Prestamo_Operacion')
    CREATE INDEX IX_Prestamo_Operacion ON dbo.Prestamo(id_operacion,id_prestamo) WHERE id_operacion IS NOT NULL;
GO
