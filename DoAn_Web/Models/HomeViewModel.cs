using System.Collections.Generic;

namespace DoAn_Web.Models
{
    public class HomeViewModel
    {
        public List<JobPosting> Jobs { get; set; } = new List<JobPosting>();
        public List<Company> Companies { get; set; } = new List<Company>();
        public List<Industry> Industries { get; set; } = new List<Industry>();
    }
}
