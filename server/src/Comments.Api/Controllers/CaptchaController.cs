using Comments.Application.Captcha;

using Microsoft.AspNetCore.Mvc;

namespace Comments.Api.Controllers;

[ApiController]
[Route("api/captcha")]
public class CaptchaController(ICaptchaService captchaService) : ControllerBase
{
    private readonly ICaptchaService _captchaService = captchaService;

    [HttpGet]
    public ActionResult<CaptchaDto> Get()
    {
        return Ok(_captchaService.Create());
    }
}