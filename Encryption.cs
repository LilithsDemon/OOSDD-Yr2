using System;
using System.Text;
using Crypto.AES;

namespace Encryption
{ 
    class Encryptions
    {
        private string passkey;

        public string EncryptMessage(string input) => AES.EncryptString(passkey, input);

        public string DecryptMessage(string input) => AES.DecryptString(passkey, input);

        public Encryptions(string given_passkey)
        {
            passkey = given_passkey;
        }
    }

}