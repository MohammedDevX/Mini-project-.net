namespace Products_service.Services.SaveFiles
{
    public interface IMinioService
    {
        public Task<string> UploadImage(IFormFile file);
        public Task<string> GetImage(string imageName, int expiration);
    }
}
