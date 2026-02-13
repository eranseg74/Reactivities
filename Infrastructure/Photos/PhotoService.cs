using Application.Interfaces;
using Application.Profiles.DTOs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos;

public class PhotoService : IPhotoService
{
    public readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> options)
    {
        var account = new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account); // Now the _cloudinary will have access the Cloudinary functionallity
    }
    public async Task<string> DeletePhoto(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        if (result.Error != null)
        {
            throw new Exception(result.Error.Message);
        }
        return result.Result;
    }

    public async Task<PhotoUploadResult?> UploadPhoto(IFormFile file)
    {
        if (file.Length > 0)
        {
            // We use the using keyword because the stream will contain the file data which consumes space in memory. defining the using keyword means that this property will be disposed of once the method is finished
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                // We implement to cropping and size in the client
                // Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                Folder = "Reactivities"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }
            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.AbsoluteUri
            };
        }
        return null;
    }
}
