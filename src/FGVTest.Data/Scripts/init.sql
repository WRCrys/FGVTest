IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'FGVTest')
BEGIN
    CREATE DATABASE FGVTest;
END;
GO

USE FGVTest;
GO

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name = 'Cliente' AND xtype = 'U')
BEGIN
    CREATE TABLE Cliente (
        CodCliente  INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
        CNPJ        VARCHAR(18)   NOT NULL,
        Nome        NVARCHAR(200) NOT NULL,
        Email       NVARCHAR(200) NOT NULL,
        DataCadastro DATETIME     NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UQ_Cliente_CNPJ UNIQUE (CNPJ)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name = 'Produto' AND xtype = 'U')
BEGIN
    CREATE TABLE Produto (
        CodProduto INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Nome       NVARCHAR(200)   NOT NULL,
        Preco      DECIMAL(18, 2)  NOT NULL,
        Estoque    INT             NOT NULL DEFAULT 0
    );
END;

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name = 'Pedido' AND xtype = 'U')
BEGIN
    CREATE TABLE Pedido (
        CodPedido  INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
        CodCliente INT            NOT NULL,
        DataPedido DATETIME       NOT NULL DEFAULT GETDATE(),
        ValorTotal DECIMAL(18, 2) NOT NULL DEFAULT 0,
        CONSTRAINT FK_Pedido_Cliente FOREIGN KEY (CodCliente)
            REFERENCES Cliente (CodCliente) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE name = 'ItensPedido' AND xtype = 'U')
BEGIN
    CREATE TABLE ItensPedido (
        CodPedido      INT            NOT NULL,
        CodProduto     INT            NOT NULL,
        Quantidade     INT            NOT NULL,
        PrecoUnitario  DECIMAL(18, 2) NOT NULL,
        CONSTRAINT PK_ItensPedido PRIMARY KEY (CodPedido, CodProduto),
        CONSTRAINT FK_ItensPedido_Pedido FOREIGN KEY (CodPedido)
            REFERENCES Pedido (CodPedido) ON DELETE NO ACTION,
        CONSTRAINT FK_ItensPedido_Produto FOREIGN KEY (CodProduto)
            REFERENCES Produto (CodProduto) ON DELETE NO ACTION
    );
END;
