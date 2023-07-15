using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirements;

public class GenZRequirement :IAuthorizationRequirement{
    public GenZRequirement(int fromYear = 1990, int toYear = 2003){
        FromYear = fromYear;
        ToYear = toYear;   
    }
    public int FromYear { get; set;}
    public int ToYear { get; set;}
}