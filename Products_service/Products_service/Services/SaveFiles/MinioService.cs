using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Products_service.Services.SaveFiles;

public class MinioService : IMinioService
{
    private readonly IMinioClient minio;
    private readonly string bucket;

    public MinioService(IConfiguration config)
    {
        bucket = config["Minio:BucketName"];

        minio = new MinioClient()
            .WithEndpoint(config["Minio:Endpoint"])
            .WithCredentials(
                config["Minio:AccessKey"],
                config["Minio:SecretKey"]
            )
            .WithSSL(bool.Parse(config["Minio:UseSSL"]))
            .Build();
    }

    public async Task<string> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Fichier invalide");

        if (!file.ContentType.StartsWith("image/"))
            throw new ArgumentException("Only images !!");

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(fileName)
            .WithStreamData(file.OpenReadStream())
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType)
        );

        return fileName;
    }

    public async Task<string> GetImage(string imageName, int expiration=3600)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucket)
            .WithObject(imageName)
            .WithExpiry(expiration);
        return await minio.PresignedGetObjectAsync(args);
    }
}
