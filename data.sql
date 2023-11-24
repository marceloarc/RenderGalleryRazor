Insert into users (Name,Email,Password,Telefone,Usuario,Pic) VALUES ('Marcelo','marceloaugusto96@hotmail.com','vidaloka','2323',1,'images/person_5.jpg');
Insert into users (Name,Email,Password,Telefone,Usuario,Pic) VALUES ('Pedro','pedro@gmail.com','vidaloka','2323',1,'images/person_6.jpg');
insert into Categorias (Nome,Image,Descricao) VALUES ('Animes','images/gojo2.png','Animes');

insert into Artistas (User_Id,dataHora) VALUES (1,CONVERT(DATE,'13-12-2019',104));

insert into Publicacoes (Nome,Descricao,dataHora,Artista_Id) VALUES ('teste','teste',CONVERT(DATE,'13-12-2019',104),1);


insert into Arts (Arte,Path,Valor,Quantidade,categoria_id,dataHora,publi_id,"like",deslike) VALUES ('Teste','images/cu.png',100,100,1,CONVERT(DATE,'13-12-2019',104),1,10,20);

