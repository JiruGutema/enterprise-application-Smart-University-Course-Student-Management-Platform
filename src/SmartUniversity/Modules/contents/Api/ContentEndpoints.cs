using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartUniversity.Modules.Content.Application.Services;
using SmartUniversity.Modules.Content.Application.DTOs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SmartUniversity.Modules.Content.Api;

public static class ContentEndpoints
{
    public static void MapContentEndpoints(this WebApplication app)
    {
       
        var group = app.MapGroup("/api/content")
                       .WithTags("Content Management")
                       .DisableAntiforgery(); // Apply security setting to the whole group

        // 1. UPLOAD MATERIAL
        // Note: URL is now just "/courses/..." because "/api/content" is in the group above
        group.MapPost("/courses/{courseId}/materials", async (Guid courseId, [FromForm] MaterialUploadRequest req, MaterialService service) => {
            if (!Guid.TryParse(req.UploadedByUserId, out var uid)) return Results.BadRequest("Invalid User ID");
            Guid.TryParse(req.LessonId, out var lid);
            
            var path = Path.Combine("uploads", courseId.ToString());
            Directory.CreateDirectory(path);
            var fullPath = Path.Combine(path, req.File.FileName);
            using (var s = File.Create(fullPath)) await req.File.CopyToAsync(s);

            var m = await service.UploadAsync(courseId, lid == Guid.Empty ? null : lid, req.Title ?? req.File.FileName, req.File.FileName, fullPath, req.File.ContentType, req.File.Length, uid, req.Description);
            return Results.Created($"/api/content/materials/{m.Id}", m);
        });

        // 2. LIST MATERIALS
        group.MapGet("/courses/{courseId}/materials", async (Guid courseId, [FromQuery] Guid? lessonId, [FromQuery] string? fileType, [FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int? page, [FromQuery] int? pageSize, MaterialService service) => {
            return Results.Ok(await service.GetMaterialsForCourseAsync(courseId, lessonId, fileType, search, sort, page ?? 1, pageSize ?? 20));
        });

        // 3. GET SINGLE MATERIAL METADATA
        group.MapGet("/materials/{materialId}", async (Guid materialId, MaterialService service) => {
            var m = await service.GetMaterialByIdAsync(materialId);
            return m != null ? Results.Ok(m) : Results.NotFound();
        });

        // 4. DOWNLOAD FILE
        group.MapGet("/materials/{materialId}/download", async (Guid materialId, MaterialService service) => {
            var m = await service.GetMaterialByIdAsync(materialId); 
            if (m == null || !File.Exists(m.FilePath)) return Results.NotFound("File not found");
            return Results.File(File.OpenRead(m.FilePath), "application/octet-stream", m.FileName);
        });

        // 5. UPDATE METADATA
        group.MapPut("/materials/{materialId}", async (Guid materialId, [FromBody] UpdateMaterialRequest req, MaterialService service) => {
            var m = await service.UpdateMetadataAsync(materialId, req.Title, req.Description, req.LessonId);
            return m != null ? Results.Ok(m) : Results.NotFound();
        });

        // 6. REPLACE FILE
        group.MapPost("/materials/{materialId}/replace", async (Guid materialId, IFormFile file, MaterialService service) => {
            var m = await service.ReplaceFileAsync(materialId, file);
            return m != null ? Results.Ok(m) : Results.NotFound();
        });

        // 7. DELETE MATERIAL
        group.MapDelete("/materials/{materialId}", async (Guid materialId, MaterialService service) => {
            return await service.DeleteAsync(materialId) ? Results.NoContent() : Results.NotFound();
        });

        // 8. STUDENT DASHBOARD
        group.MapGet("/my/materials", (Guid? courseId, [FromQuery] bool recentFirst) => {
            return Results.Ok(new List<CourseMaterialsDashboardDto>()); 
        });
    }
}