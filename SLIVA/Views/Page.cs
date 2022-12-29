using System;
using System.IO;
namespace SLIVA.Views
{
    public class Page
    {
        public string Name { get => _name;}
        string _name;

        public string Html { get => _html; }
        string _html;

        public string Url { get => _url; }
        string _url;

        public byte[] ByteContent { get => _byte_content; }
        private byte[] _byte_content;

        public Page(string url)
        {
            _url = url;
            if (File.Exists(_url))
            {
                string[] temp = Url.Split('/');
                _name = temp[temp.Length - 1];

                _byte_content = File.ReadAllBytes(Url);
                _html = File.ReadAllText(Url);
            }
            else
            {
                throw new NullReferenceException("Page doesn`t exist");
            }
        }

    }
}
