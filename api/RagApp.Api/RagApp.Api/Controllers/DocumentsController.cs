using Microsoft.AspNetCore.Mvc;
using RagApp.Api.Models;
using RagApp.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using RagApp.Api.DTOs;
using System.IO;

namespace RagApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DocumentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDocuments()
        {

            var documents = await _context.Documents
                .Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    UploadedAt = d.UploadedAt
                }).ToListAsync();

            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }
            var documentResponseDto = new DocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                UploadedAt = document.UploadedAt
            };
            return Ok(documentResponseDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                return BadRequest("A file name is required.");
            }

            if (file.FileName.Length > 255)
            {
                return BadRequest("File name cannot exceed 255 characters.");
            }
            var originalFileName = Path.GetFileName(file.FileName);

            var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };

            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Only PDF, DOCX, and TXT files are allowed.");
            }

            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                return BadRequest("File size cannot exceed 10 MB.");
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "uploads"
            );

            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(originalFileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (IOException)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return StatusCode(500, "The file could not be saved.");
            }
            var document = new Document
            {
                FileName = originalFileName,
                FilePath = Path.Combine("uploads", uniqueFileName),
                UploadedAt = DateTime.UtcNow
            };

            try
            {
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                throw;
            }
            var documentDtoResponse = new DocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                UploadedAt = document.UploadedAt

            };
            return Ok(documentDtoResponse);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                document.FilePath
            );

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocument(int id, DocumentDto document)
        {
            var doc = await _context.Documents.FindAsync(id);

            if (doc == null)
            {
                return NotFound();
            }

            doc.FileName = document.FileName;
            await _context.SaveChangesAsync();
            var documentResponseDto = new DocumentDto
            {
                Id = doc.Id,
                FileName = doc.FileName,
                UploadedAt = doc.UploadedAt
            };
            return Ok(documentResponseDto);
        }

        [HttpGet("{id}/file")]
        public async Task<IActionResult> GetDocumentFile(int id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                document.FilePath
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            var contentType = document.FileName switch
            {
                _ when document.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                    => "application/pdf",

                _ when document.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                    => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

                _ when document.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                    => "text/plain",

                _ => "application/octet-stream"
            };

            return File(fileBytes, contentType);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                document.FilePath
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            var contentType = document.FileName switch
            {
                _ when document.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                    => "application/pdf",

                _ when document.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
                    => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

                _ when document.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
                    => "text/plain",

                _ => "application/octet-stream"
            };

            return File(fileBytes, contentType, document.FileName);
        }
    }
}

