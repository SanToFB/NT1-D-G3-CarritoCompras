using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CarritoCompras_NT1.Extensions
{
    public static class StringExtensions
    {
        //Metodo de Extension, extendemos la clase String. Con el this dentro del parametro hacemos referencia al objeto en cuestion.
        public static byte[] Encriptar(this string texto)
        {
            return new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(texto));
        }

        public static void ValidarPassword(this string password)
        {
            /*IsNullOrWhiteSpace => true si es empty, null o vacio. */
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("La contraseña es requerida.");
            }

            if (password.Length < 6)
            {
                throw new Exception("La contraseña debe tener al menos 6 caracteres.");
            }

            //Restricciones de Seguridad:

            bool contieneUnNumero = new Regex("[0-9]").Match(password).Success;
            bool contieneUnaMinuscula = new Regex("[a-z]").Match(password).Success;
            bool contieneUnaMayuscula = new Regex("[A-Z]").Match(password).Success;

            if (!contieneUnaMayuscula || !contieneUnaMinuscula || !contieneUnNumero)
            {
                throw new Exception("La contraseña debe tener al menos un número, una letra minúscula y una mayúscula.");
            }
        }
    }
}
