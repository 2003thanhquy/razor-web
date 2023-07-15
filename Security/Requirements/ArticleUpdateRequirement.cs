using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirements;

public class ArticleUpdateRequirement :IAuthorizationRequirement{
    
    public ArticleUpdateRequirement(int year =2020, int month =7, int day =7){
        Year = year;
        Month = month;  
        Day = day;
    }
    public int Year{get; set; }
    public int Month{get; set; }    
    public int Day{get; set; }

   
}