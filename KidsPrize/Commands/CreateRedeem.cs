using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using KidsPrize.Extensions;
using MediatR;
using E = KidsPrize.Models;
using R = KidsPrize.Resources;

namespace KidsPrize.Commands
{
    public class CreateRedeem : Command, IRequest<R.Redeem>
    {
        [Required]
        public Guid ChildId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        public int Value { get; set; }
    }

    public class CreateRedeemHandler : IAsyncRequestHandler<CreateRedeem, R.Redeem>
    {
        private readonly KidsPrizeContext _context;
        private readonly IMapper _mapper;

        public CreateRedeemHandler(KidsPrizeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<R.Redeem> Handle(CreateRedeem message)
        {
            // Ensure the child blongs to current user
            var child = await this._context.GetChildOrThrow(message.UserId(), message.ChildId);
            var redeem = new E.Redeem(child, DateTimeOffset.Now, message.Description, message.Value);

            child.Update(null, null, child.TotalScore - message.Value);
            this._context.Redeems.Add(redeem);
            await _context.SaveChangesAsync();

            return _mapper.Map<R.Redeem>(redeem);
        }
    }
}