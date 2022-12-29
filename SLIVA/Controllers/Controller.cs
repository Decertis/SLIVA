using System;
using System.Text;
using System.Net;
using SLIVA.Data;
using SLIVA.Views;
using SLIVA.Models;

public abstract class Controller
{
    protected HttpListenerContext _context;
    protected MySqlManager mySqlManager;
    protected Client client;
    public Controller(HttpListenerContext context)
    {
        mySqlManager = new MySqlManager();
        this._context = context;
        string trimed_ip = _context.Request.RemoteEndPoint.ToString().Split(':')[0];
        if (!mySqlManager.ClientExists(trimed_ip))
            mySqlManager.InsertClient(trimed_ip);
        client = mySqlManager.GetClient(trimed_ip);
    }
    public virtual HttpListenerResponse GenerateHttpListenerResponse()
    {
        return _context.Response;
    }
    public virtual byte[] GenerateResponseContent()
    {
        return Encoding.UTF8.GetBytes("<h1>Vibe check passed</h1>");
    }
    public virtual Page GetPage(string page_name)
    {
        Page result = null;
        foreach (Page page in PageManager.Pages)
        {
            if (page.Name == page_name)
            {
                result = page;
                break;
            }
        }

        return result;
    }
    public virtual string GetPageWithUserData(Page page, User user)
    {
        return null;
    }
}