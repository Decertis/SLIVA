using System;
using System.IO;
namespace SLIVA.Views
{
    public static class PageManager
    {

        public static Page[] Pages;
        public static int PagesAmount;
        public static void Initialize(string pages_folder_url)
        {
            PagesAmount = Directory.GetFiles(pages_folder_url).Length;
            Pages = new Page[PagesAmount];
            for(int i = 0; i< PagesAmount; i++)
            {
                Pages[i] = new Page(Directory.GetFiles(pages_folder_url)[i]);
            }
        }
        public static void ListPages()
        {
            foreach(Page page in Pages)
            {
                Console.WriteLine(page.Name);
            }
        }
    }
}
