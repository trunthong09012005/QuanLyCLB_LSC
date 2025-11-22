using System;
using System.Collections.Generic;

namespace QL_CLB_LSC1.ViewModels
{
    public class HomeViewModel
    {
        // Thống kê
        public int TotalMembers { get; set; }
        public int TotalActivities { get; set; }
        public int TotalDepartments { get; set; }

        // Danh sách hình ảnh gallery
        public List<GalleryItem> GalleryItems { get; set; } = new List<GalleryItem>();

        // Danh sách hoạt động nổi bật
        public List<ActivityPreview> ActivityPreviews { get; set; } = new List<ActivityPreview>();
    }

    public class GalleryItem
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Badge { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
    }

    public class ActivityPreview
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string GradientClass { get; set; }
    }
}