using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Content.Application.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Content.Api;

public class MaterialUploadRequest
{
    public required IFormFile File { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    
    // ✅ CHANGED: Both IDs are now strings to prevent crashes on empty input
    public string? LessonId { get; set; } 
    public string? UploadedByUserId { get; set; }
}

public static class ContentEndpoints
{
    public static void MapContentEndpoints(this WebApplication app)
    {
        app.MapPost("/api/content/courses/{courseId}/materials",
            async (
                Guid courseId,
                [FromForm] MaterialUploadRequest request,
                MaterialService service
            ) =>
            {
                // 1. Safe Parsing for User ID (Required)
                if (string.IsNullOrWhiteSpace(request.UploadedByUserId) || 
                    !Guid.TryParse(request.UploadedByUserId, out var uploadedByUserId))
                {
                    return Results.BadRequest("A valid UploadedByUserId is required.");
                }

                // 2. Safe Parsing for Lesson ID (Optional)
                Guid? parsedLessonId = null;
                if (!string.IsNullOrWhiteSpace(request.LessonId) && 
                    Guid.TryParse(request.LessonId, out var lid))
                {
                    parsedLessonId = lid;
                }

                // 3. Handle File
                var file = request.File;
                var uploadsPath = Path.Combine("uploads", courseId.ToString());
                Directory.CreateDirectory(uploadsPath);

                var filePath = Path.Combine(uploadsPath, file.FileName);
                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                // 4. Save to DB
                var material = await service.UploadAsync(
                    courseId,
                    parsedLessonId,
                    request.Title ?? file.FileName,
                    file.FileName,
                    filePath,
                    file.ContentType,
                    file.Length,
                    uploadedByUserId, // Use the parsed GUID
                    request.Description
                );

                return Results.Created(
                    $"/api/content/materials/{material.Id}",
                    material
                );
            })
            .DisableAntiforgery();
    }
}