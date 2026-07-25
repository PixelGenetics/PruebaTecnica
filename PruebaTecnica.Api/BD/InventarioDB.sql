drop database InventarioDB;
Go

use InventarioDB;
Go

create table Category(
	Id int identity(1,1) not null constraint pk_category primary key (Id),
	Nombre nvarchar(100) not null constraint DF_category_nombre unique (Nombre),
	Estado bit not null constraint UQ_category_estado default 1
);
Go

create table Product(
	Id int identity(1,1) not null constraint pk_product primary key (Id),
	Codigo nvarchar(50) not null constraint uq_product_codigo unique (Codigo),
	Nombre nvarchar(150) not null,
	CategoryId int not null constraint fk_product_categoryId foreign key (CategoryId) references Category(Id),
	Precio decimal (20,2) not null constraint ck_product_precio check (Precio >= 0),
	Estado bit not null constraint df_produc_estado default 1,
	FechaCreacion datetime2 not null constraint df_product_fechaCreacion default sysdatetime(),
);
Go

create table [User](
	Id int identity(1,1) not null constraint pk_user primary key(Id),
	nombreUsuario nvarchar(50) not null constraint uq_user_nombreUsuario unique (nombreUsuario),
	contrasenia nvarchar(255) not null,
	nombre nvarchar(150) not null,
	correo nvarchar(150) not null constraint uq_user_correo unique (correo),
	estado bit not null constraint df_user_estado default 1,
);
Go

create table movInv(
	Id int identity(1,1) not null constraint pk_movInv primary key(Id),
	productId int not null constraint fk_movInv_productId foreign key (productId) references Product(Id),
	tipo nvarchar(10) not null constraint ck_movInv_tipo check (tipo IN('Entrada','Salida')),
	cantidad int not null constraint ck_movInv_cantidad check (cantidad > 0),
	fecha datetime2 not null constraint df_movInv_fecha default sysdatetime(),
	referencia nvarchar(200) not null,
);
Go

INSERT INTO Category (Nombre)
VALUES
    ('Electrónica'),
    ('Alimentos'),
    ('Limpieza');
GO

INSERT INTO Product
(
    Codigo,
    Nombre,
    CategoryId,
    Precio
)
VALUES
    ('ELEC-001', 'Teclado mecánico',           1, 45.99),
    ('ELEC-002', 'Mouse inalámbrico',           1, 22.50),
    ('ELEC-003', 'Monitor de 24 pulgadas',      1, 185.00),
    ('ELEC-004', 'Audífonos USB',               1, 35.75),

    ('ALIM-001', 'Café molido 500 g',           2, 8.75),
    ('ALIM-002', 'Arroz 1 kg',                  2, 1.90),
    ('ALIM-003', 'Aceite vegetal 1 litro',      2, 4.50),

    ('LIMP-001', 'Jabón líquido',               3, 3.50),
    ('LIMP-002', 'Detergente en polvo',         3, 6.25),
    ('LIMP-003', 'Desinfectante multiuso',      3, 4.99);
GO

INSERT INTO [User]
(
    NombreUsuario,
    Contrasenia,
    Nombre,
    Correo
)
VALUES
    (
        'admin',
        'HASH_ADMIN',
        'Administrador del sistema',
        'admin@inventario.com'
    ),
    (
        'kevin.borge',
        'HASH_KEVIN',
        'Kevin Antonio Borge',
        'kevin@inventario.com'
    );
GO

INSERT INTO MovInv
(
    ProductId,
    Tipo,
    Cantidad,
    Referencia
)
VALUES
    (1,  'ENTRADA', 30,  'Compra inicial de teclados'),
    (2,  'ENTRADA', 50,  'Compra inicial de mouse'),
    (3,  'ENTRADA', 15,  'Compra inicial de monitores'),
    (4,  'ENTRADA', 40,  'Compra inicial de audífonos'),
    (5,  'ENTRADA', 100, 'Ingreso de café del proveedor'),
    (6,  'ENTRADA', 150, 'Ingreso de arroz del proveedor'),
    (7,  'ENTRADA', 80,  'Ingreso de aceite vegetal'),
    (8,  'ENTRADA', 60,  'Ingreso de jabón líquido'),
    (9,  'ENTRADA', 70,  'Ingreso de detergente'),
    (10, 'ENTRADA', 55,  'Ingreso de desinfectante'),

    (1,  'SALIDA', 3,  'Venta factura F-0001'),
    (2,  'SALIDA', 8,  'Venta factura F-0002'),
    (5,  'SALIDA', 12, 'Venta factura F-0003'),
    (6,  'SALIDA', 20, 'Venta factura F-0004'),
    (9,  'SALIDA', 5,  'Producto entregado a sucursal');
GO
SELECT * FROM Category;
SELECT * FROM Product;
SELECT * FROM [User];
SELECT * FROM MovInv;

SELECT COUNT(*) AS TotalCategorias
FROM Category;

SELECT COUNT(*) AS TotalProductos
FROM Product;

SELECT COUNT(*) AS TotalMovimientos
FROM MovInv;