-- Paso 1: Guardar los IDs de los empleados que queremos MANTENER
-- (Asumiendo que el nombre coincide exactamente)

-- Paso 2: Eliminar registros de asistencia de empleados que NO son los que queremos mantener
DELETE FROM RegistrosAsistencia 
WHERE EmpleadoId NOT IN (
    SELECT Id FROM Empleados 
    WHERE Nombre = 'Administrador del Sistema' 
       OR Nombre = 'Christian Galicia Graciano'
);

-- Paso 3: Eliminar reportes guardados de empleados que NO son los que queremos mantener
DELETE FROM ReportesGuardados 
WHERE EmpleadoId NOT IN (
    SELECT Id FROM Empleados 
    WHERE Nombre = 'Administrador del Sistema' 
       OR Nombre = 'Christian Galicia Graciano'
)
AND EmpleadoId IS NOT NULL;

-- Paso 4: Eliminar los empleados que NO queremos mantener
DELETE FROM Empleados 
WHERE Nombre != 'Administrador del Sistema' 
  AND Nombre != 'Christian Galicia Graciano';

-- Verificar resultado
SELECT Id, Nombre, Email FROM Empleados;
