using Microsoft.AspNetCore.Mvc;
using RenderGallery.Models;
using RenderGallery.Util;
using System;
using System.Data.Entity;

namespace RenderGallery.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        private readonly DatabaseContext db;


        public UserController(DatabaseContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public List<Artista> Get()
        {
            List<Artista> artistas = new List<Artista>();
            artistas = db.Artistas.Include(a => a.User.Name).ToList();
      
            return artistas;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterArtista(Artista artista)
        {
            if(ModelState.IsValid) 
            {
                User user = db.Users.Where(x => x.Email == artista.User.Email).FirstOrDefault();

                if(user!=null)
                {
                    TempData["erro"] = "Artista já cadastrado!";
                    return Ok(TempData);
                }
                else
                {
                    string hash = BCrypt.Net.BCrypt.HashPassword(artista.User.Password);
                    artista.User.Password = hash;
                    artista.User.Usuario = Models.User.TipoUsuario.Artista;
                    artista.dataHora = DateTime.Now;
                    db.Artistas.Add(artista);
                    db.SaveChanges();
                    TempData["sucesso"] = "Artista Cadastrado com sucesso!";
                    return Ok(TempData);
                }

            }
            else
            {
                TempData["erro"] = "Algo deu errado!";
                return Ok(TempData);
            }
            
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                string cpf = cliente.document.Replace(".", "");
                cpf = cpf.Replace("-", "");
                Cliente user = db.Clientes.Where(x => x.User.Email == cliente.User.Email || x.document == cpf).FirstOrDefault();

                if (user != null)
                {
                    TempData["erro"] = "Cliente já cadastrado!";
                    return Ok(TempData);

                }
                else
                {

                    bool isValid = false;
                    if(cpf.Length > 11)
                    {
                        isValid = Functions.ValidaCNPJ(cliente.document);
                    }
                    else
                    {
                        isValid = Functions.ValidaCPF(cliente.document);
                    }

                    if (isValid)
                    {
                        string hash = BCrypt.Net.BCrypt.HashPassword(cliente.User.Password);
                        cliente.User.Password = hash;
                        cliente.User.Usuario = Models.User.TipoUsuario.Cliente;
                        cliente.dataHora = DateTime.Now;
                        cliente.document = cpf;
                        db.Clientes.Add(cliente);
                        db.SaveChanges();
                        TempData["sucesso"] = "Cliente Cadastrado com sucesso!";
                      
                    }
                    else
                    {
                        TempData["erro"] = "Documento invalido";
                    }

                }

            }
            else
            {
                TempData["erro"] = "Algo deu errado!";
               
            }
            return Ok(TempData);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> EditarCliente(Cliente cli)
        {
            if (ModelState.IsValid)
            {
                Cliente cliente = db.Clientes.Where(x => x.Id == cli.Id).FirstOrDefault();

                if (cliente != null)
                {
                    cliente = cli;
                    db.SaveChanges();
                    TempData["sucesso"] = "Cliente Editado!";
               

                }
                else
                {
                    TempData["erro"] = "Cliente não encontrado!";
                }

            }
            else
            {
                TempData["erro"] = "Algo deu errado!";
          
            }
            return Ok(TempData);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> EditarArtista(Artista art)
        {
            if (ModelState.IsValid)
            {
                Artista artista = db.Artistas.Where(x => x.Id == art.Id).FirstOrDefault();

                if (artista != null)
                {
                    artista = art;
                    db.SaveChanges();
                    TempData["sucesso"] = "Artista Editado!";


                }
                else
                {
                    TempData["erro"] = "Artista não encontrado!";
                }

            }
            else
            {
                TempData["erro"] = "Algo deu errado!";

            }
            return Ok(TempData);
        }

    }
}