-- Crear o verificar que exista al menos un área
INSERT OR IGNORE INTO Areas (Id, Nombre) 
VALUES ('11111111-1111-1111-1111-111111111111', 'Sin Área');

-- Crear o verificar que exista al menos un rol
INSERT OR IGNORE INTO Roles (Id, Nombre) 
VALUES ('22222222-2222-2222-2222-222222222222', 'Empleado General');

-- Verificar
SELECT * FROM Areas;
SELECT * FROM Roles;
