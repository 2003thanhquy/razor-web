using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext _myBlogContext;
    public IndexModel(ILogger<IndexModel> logger, AppDbContext myBlogContext)
    {
        _logger = logger;
        _myBlogContext = myBlogContext;   
    }
    
    public void OnGet()
    {
        var posts = (from article in _myBlogContext.articles
                orderby article.Created descending
                select article
        ).ToList();

        ViewData["posts"] = posts;
    }
}
