﻿using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RenderGalleyRazor.Models
{
    [JsonObject(IsReference = true)]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(250)]
        public string Password { get; set; }

        [StringLength(250)]
        [Required]
        public string? Telefone { get; set; }

        [EnumDataType(typeof(TipoUsuario))]
        [Required]
        public TipoUsuario Usuario { get; set; }
        public enum TipoUsuario { Administrador = 0, Cliente = 1, Artista = 2 }
        [StringLength(250)]
        public string? Pic { get; set; }

    }
}
