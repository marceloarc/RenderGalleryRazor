using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using RenderGalleyRazor.Models;

namespace RenderGallery.Util
{
    public class Functions
    {
        private static string caminhoServidor;

        public Functions(IWebHostEnvironment sistema)
        {
            caminhoServidor = sistema.WebRootPath;
        }

        public class ImageHashing
        {
            public static string CalculateImageHash(byte[] imageBytes)
            {
                using (var md5 = MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(imageBytes);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static bool ImageExistsInSystem(string hash)
        {
            // Diretório raiz onde todas as imagens estão armazenadas
            string rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Procurar em todos os subdiretórios de usuários
            foreach (var userDirectory in Directory.GetDirectories(rootDirectory))
            {
                var files = Directory.EnumerateFiles(userDirectory);

                foreach (var file in files)
                {
                    if (Path.GetFileNameWithoutExtension(file) == hash)
                    {
                        return true; // Se encontrar um arquivo com o mesmo hash, a imagem existe no sistema
                    }
                }
            }

            return false; // Se não houver correspondências em nenhum diretório de usuário
        }



        public static (string path, bool alreadyExistsForUser, bool alreadyExistsInSystem) WriteFile(IFormFile img, int userId)
        {
            // Ler os bytes da imagem
            using (MemoryStream memoryStream = new MemoryStream())
            {
                img.CopyTo(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                // Calcular o hash baseado nos bytes da imagem
                string hash = ImageHashing.CalculateImageHash(imageBytes);

                // Definir o caminho para salvar a imagem com base no hash e no ID do usuário
                string caminhoCompleto = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", userId.ToString());
                if (!Directory.Exists(caminhoCompleto))
                {
                    Directory.CreateDirectory(caminhoCompleto);
                }

                string path = Path.Combine(caminhoCompleto, hash + Path.GetExtension(img.FileName));

                // Verificar se o arquivo já existe para o usuário específico
                bool alreadyExistsForUser = File.Exists(path);

                // Verificar se o arquivo já existe no sistema
                bool alreadyExistsInSystem = ImageExistsInSystem(hash);

                if (!alreadyExistsForUser && !alreadyExistsInSystem)
                {
                    // Se não existir, salvar a imagem com o nome baseado no hash
                    using (Stream stream = new FileStream(path, FileMode.Create))
                    {
                        img.CopyTo(stream);
                    }
                }

                return (path, alreadyExistsForUser, alreadyExistsInSystem);
            }
        }

        public static string WriteFilePerfil(IFormFile img)
        {
            string caminhoCompleto = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images");

            if (!Directory.Exists(caminhoCompleto))
            {
                Directory.CreateDirectory(caminhoCompleto);
            }
            string path = caminhoCompleto + "\\" + GetTimestamp(DateTime.Now) + System.IO.Path.GetExtension(img.FileName);
            string name = Path.GetFileName(path);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                img.CopyTo(stream);
            }
            return path;


        }


        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static bool ValidaCPF(string vrCPF)
        {
            string valor = vrCPF.Replace(".", "");
            valor = valor.Replace("-", "");

            if (valor.Length != 11)
                return false;

            bool igual = true;
            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                    valor[i].ToString());

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;
            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;
            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else
                if (numeros[10] != 11 - resultado)
                return false;

            return true;
        }

        public static bool ValidaCNPJ(string vrCNPJ)
        {
            string CNPJ = vrCNPJ.Replace(".", "");
            CNPJ = CNPJ.Replace("/", "");
            CNPJ = CNPJ.Replace("-", "");

            int[] digitos, soma, resultado;
            int nrDig;
            string ftmt;
            bool[] CNPJOk;

            ftmt = "6543298765432";
            digitos = new int[14];

            soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;

            resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;

            CNPJOk = new bool[2];
            CNPJOk[0] = false;
            CNPJOk[1] = false;

            try
            {
                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(CNPJ.Substring(nrDig, 1));

                    if (nrDig <= 11)
                        soma[0] += (digitos[nrDig] *
                          int.Parse(ftmt.Substring(
                          nrDig + 1, 1)));

                    if (nrDig <= 12)
                        soma[1] += (digitos[nrDig] *
                          int.Parse(ftmt.Substring(
                          nrDig, 1)));
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);

                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        CNPJOk[nrDig] = (
                        digitos[12 + nrDig] == 0);
                    else
                        CNPJOk[nrDig] = (
                        digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }
                return (CNPJOk[0] && CNPJOk[1]);
            }
            catch
            {
                return false;
            }

        }
    }
}
