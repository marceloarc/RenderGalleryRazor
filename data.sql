insert into Categorias (Nome,Image,Descricao) VALUES ('Animes','images/gojo2.png','Animes');
insert into Categorias (Nome,Image,Descricao) VALUES ('PixelArt','images/anima2.png','PixelArt');
insert into Categorias (Nome,Image,Descricao) VALUES ('Realismo','images/real2.png','Realismo');
insert into Categorias (Nome,Image,Descricao) VALUES ('Personagens','images/Personagens2.jpg','Personagens');
insert into Categorias (Nome,Image,Descricao) VALUES ('Retrô','images/retrô2.png','Retrô');
insert into Categorias (Nome,Image,Descricao) VALUES ('Cyberpunk','images/cyber2.png','Cyberpunk');

-- Inserir o Plano Básico
INSERT INTO Planos (Nome, Preco, LimitePublicacoes, Taxa, Cor)
VALUES ('Básico', 0, 10, 0.07, '#00FFFF');

-- Obter o ID do último plano inserido (Básico)
DECLARE @planoBasicoId INT;
SELECT @planoBasicoId = SCOPE_IDENTITY();

-- Inserir as vantagens do Plano Básico
INSERT INTO Vantagens (PlanoId, descricao)
VALUES (@planoBasicoId, 'Limite de 10 postagens'),
       (@planoBasicoId, 'Possibilidade de saque em até 30 dias'),
       (@planoBasicoId, 'Ao atingir 50 curtidas, recebe o selo de verificado');

-- Inserir o Plano Pro
INSERT INTO Planos (Nome, Preco, LimitePublicacoes, Taxa, Cor)
VALUES ('Pro', 30.0, 50, 0.05, '#A020F0');

-- Obter o ID do último plano inserido (Pro)
DECLARE @planoProId INT;
SELECT @planoProId = SCOPE_IDENTITY();

-- Inserir as vantagens do Plano Pro
INSERT INTO Vantagens (PlanoId, descricao)
VALUES (@planoProId, 'Limite de 30 postagens'),
       (@planoProId, 'Possibilidade de saque em até 20 dias'),
       (@planoProId, 'Ao atingir 20 curtidas, recebe o selo de verificado');

-- Inserir o Plano Ultimate
INSERT INTO Planos (Nome, Preco, LimitePublicacoes, Taxa, Cor)
VALUES ('Ultimate', 60.0, 50, 0.05, '#FF0000');

-- Obter o ID do último plano inserido (Ultimate)
DECLARE @planoUltimateId INT;
SELECT @planoUltimateId = SCOPE_IDENTITY();

-- Inserir as vantagens do Plano Ultimate
INSERT INTO Vantagens (PlanoId, descricao)
VALUES (@planoUltimateId, 'Sem limite de postagens'),
       (@planoUltimateId, 'Possibilidade de saque em até 10 dias'),
       (@planoUltimateId, 'Recebe o selo de verificado na mesma hora');