using EasyVersioning.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace KidsPrize.Http.Controllers
{
    [InheritableApiVersion("1", Deprecated = true)]
    [InheritableApiVersion("2")]
    public abstract class VersionedController : Controller
    {

    }

}