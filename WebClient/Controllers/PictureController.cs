using DB.Repositories.Picture;
using Microsoft.AspNetCore.Mvc;
namespace WebClient.Controllers;

[Route("[controller]")]
public class PictureController : Controller
{
    private readonly IPictureRepository _pictureRepository;

    public PictureController()
    {
        _pictureRepository = new PictureDapperRepository(); //сменить потом на EF
    }

    [HttpPost]
    public dynamic UploadPicture(IFormFile data)
    {
        


        return "";
    }



}
