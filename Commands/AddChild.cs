using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using KidsPrize.Extensions;
using KidsPrize.Models;
using Microsoft.EntityFrameworkCore;

public class AddChild : Command
{
    [Required]
    public Guid ChildUid { get; set; }
    [Required]
    public string Name  { get; set; }
    [Required]
    [RegularExpression(@"^(Male|Female)$")]
    public string Gender { get; set; }
}

public class AddChildHandler : IHandleMessages<AddChild>, IHasUser
{
    private readonly KidsPrizeDbContext _context;
    public ClaimsPrincipal User { get; set; }
    public AddChildHandler(KidsPrizeDbContext context)
    {
        this._context = context;
    }

    public async Task Handle(AddChild command)
    {
        var userUid = User.UserUid();
        var user = await _context.Users.Include(i => i.Children).FirstAsync(i => i.Uid == userUid);
        user.AddChild(new Child(0, command.ChildUid, command.Name, command.Gender, 0));
        await _context.SaveChangesAsync();
    }
}