using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Web.Areas.Admin.Models.AudioFiles;

public class AudioFileEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان فایل صوتی الزامی است.")]
    [StringLength(200, ErrorMessage = "عنوان نمی‌تواند بیشتر از 200 کاراکتر باشد.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "توضیحات نمی‌تواند بیشتر از 2000 کاراکتر باشد.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "انتخاب سخنران الزامی است.")]
    public int SpeakerId { get; set; }

    public List<int> CategoryIds { get; set; } = new();

    public bool IsPublished { get; set; }

    public bool IsDownloadable { get; set; }

    // فایل صوتی جدید - اختیاری
    public IFormFile? Audio { get; set; }

    // کاور جدید - اختیاری
    public IFormFile? CoverImage { get; set; }

    // اطلاعات فایل فعلی
    public string? CurrentFileName { get; set; }

    public string? CurrentCoverImageUrl { get; set; }

    public long CurrentFileSize { get; set; }
    public string? CurrentCoverImage { get; internal set; }
    public string? CurrentAudioFile { get; internal set; }
}