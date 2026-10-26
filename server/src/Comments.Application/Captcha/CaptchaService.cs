using System.Security.Cryptography;

using Microsoft.Extensions.Caching.Memory;

namespace Comments.Application.Captcha;

public class CaptchaService(
    IMemoryCache memoryCache,
    ICaptchaImageGenerator imageGenerator
) : ICaptchaService
{
    private const int CodeLength = 5;
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz34679";
    private const string Prefix = "captcha";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ICaptchaImageGenerator _imageGenerator = imageGenerator;

    public CaptchaDto Create()
    {
        string code = RandomNumberGenerator.GetString(Alphabet, CodeLength);
        Guid id = Guid.NewGuid();
        _memoryCache.Set($"{Prefix}:{id}", code, Ttl);

        byte[] image = _imageGenerator.Generate(code);
        string url = "data:image/png;base64," + Convert.ToBase64String(image);

        return new CaptchaDto(id, url);
    }

    public bool Validate(Guid id, string code)
    {
        bool isValid = false;
        if (_memoryCache.TryGetValue($"{Prefix}:{id}", out string? saved))
        {
            isValid = string.Equals(saved, code, StringComparison.OrdinalIgnoreCase);
        }
        _memoryCache.Remove($"{Prefix}:{id}");
        return isValid;
    }
}