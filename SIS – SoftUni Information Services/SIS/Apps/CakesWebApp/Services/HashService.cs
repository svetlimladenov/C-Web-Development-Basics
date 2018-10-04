﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace CakesWebApp.Services
{
    public class HashService : IHashService
    {
        public string StrongHash(string stringToHash)
        {
            var result = stringToHash;
            for (int i = 0; i < 3; i++)
            {
                result = Hash(result);
            }

            return result;
        }
                 
        public string Hash(string stringToHash)
        {
            stringToHash = stringToHash + "myAppSalt12312341234#";
            // SHA256 is disposable by inheritance.  
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
                // Get the hashed string.  
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                // Print the string.   

                return hash;
            }
        }
    }
}
